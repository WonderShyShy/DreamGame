using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;

// 添加简化版GameProcessController，避免外部依赖
public static class GameProcessController
{
    public static bool IsProcessingActions = false;
}

/// <summary>
/// 负责处理方块的拖动交互
/// 简化版的TouchMoving组件，专注于拖拽功能
/// </summary>
public class TouchMoving1 : MonoBehaviour
{
    // 全局静态变量，用于跟踪当前正在拖动的方块
    // 使用PiecesManager类型，简化了设计，直接操作方块
    private static PiecesManager currentDraggingPiece = null;
    
    // 移动相关参数
    private bool isDragging = false;         // 是否正在拖动
    private Vector3 touchStartPosition;      // 触摸/点击开始的位置
    
    // 添加公共属性，允许其他组件访问拖拽状态
    public bool IsDragging { get { return isDragging; } }
    
    // 棋盘参考
    private GridManager gridManager;         // 棋盘管理器引用，用于获取格子信息和更新棋盘
    
    // 拖动效果相关
    private GameObject dragEffectContainer;  // 拖拽效果容器
    [Header("拖动效果设置")]
    [SerializeField] private float dragEffectAlpha = 0.6f;  // 拖拽效果透明度
    [SerializeField] private float dragEffectOffset = 0.05f; // 拖拽效果偏移量
    [SerializeField] private float columnWidthScale = 2f; // 列宽度比例，相对于格子宽度
    [SerializeField] private float columnHeightScale = 2f; // 列高度比例，相对于棋盘高度
    [SerializeField] private int effectSortingOrder = 10; // 效果的排序顺序，确保显示在棋盘上方
    [SerializeField] private Color effectColor = new Color(0.3f, 0.7f, 1f, 0.6f); // 效果颜色
    
    // 吸附效果相关
    [Header("吸附效果设置")]
    [SerializeField] private float slowDragThreshold = 300f; // 慢速拖动的速度阈值
    [SerializeField] private float snapThreshold = 0.05f; // 吸附阈值（距离格子中心多远触发吸附）
    [SerializeField] private float pullOutDistance = 0.05f; // 拔出时额外位移距离
    [SerializeField] private bool enableSnapEffect = true; // 是否启用吸附效果
    
    // 速度检测相关
    private Vector2 lastPosition;      // 上一帧位置
    private float lastPositionTime;    // 上一帧时间
    private Vector2 currentVelocity;   // 当前速度
    private float dragSpeed;           // 拖动速度标量
    
    // 吸附状态
    private bool isSnapped = false;    // 当前是否处于吸附状态
    private int snappedColumn = -1;    // 当前吸附的列
    private int dragDirection = 0;     // 拖动方向 (-1左, 1右)
    
    /// <summary>
    /// 初始化组件，获取必要的引用
    /// </summary>
    void Start()
    {
        // 锁定帧率为60FPS
        Application.targetFrameRate = 60;
        
        // 获取场景中的GridManager
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("场景中没有GridManager组件！");
        }
        
        // 初始化速度检测变量
        lastPosition = Vector2.zero;
        lastPositionTime = 0f;
        currentVelocity = Vector2.zero;
        dragSpeed = 0f;
    }
    
    /// <summary>
    /// 每帧更新，处理用户输入和拖动逻辑
    /// </summary>
    void Update()
    {
    
        // 处理鼠标按下事件 - 开始拖动
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPosition = GetTouchPosition(); // 记录点击位置
            
            // 根据点击位置计算对应的棋盘行列坐标
            Debug.Log($"gridManager.cellSize: {gridManager.cellSize}, 检测对象: {gridManager.startOffset}");
            var rowcol = PiecesManager.getRowCol(touchStartPosition.y, touchStartPosition.x, gridManager.cellSize, gridManager.startOffset);
            
            Debug.Log($"rowcol: {rowcol.Key}, {rowcol.Value}检测对象: {gameObject.name}");
            // 查找点击位置是否有方块
            currentDraggingPiece = gridManager.FindPiece(rowcol.Key, rowcol.Value);
            if (currentDraggingPiece == null)
            {
                return; // 未点击到方块，直接返回
            }

            // 设置拖动状态并开始拖动
            isDragging = true;
            TouchBegin();
            
            // 初始化速度检测
            lastPosition = touchStartPosition;
            lastPositionTime = Time.time;
            return;
        }

        // 处理鼠标释放事件 - 结束拖动
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                return; // 不在拖动状态，忽略
            }
            isDragging = false;
            
            // 重置吸附状态
            isSnapped = false;
            snappedColumn = -1;
            
            TouchEnd(); // 结束拖动并处理位置更新
            return;
        }
        
        // 处理拖动中的移动
        if(isDragging)
        {
            TouchDrag();
        }
    }

    // 存储当前方块可移动的列范围限制
    private KeyValuePair<int, int> limitcol;
    // 存储可移动的X坐标范围（世界坐标）
    private KeyValuePair<float, float> limitposx;
    
    /// <summary>
    /// 开始拖动方块时的处理
    /// </summary>
    public void TouchBegin()
    {
        // 检查方块可移动范围，返回的是可移动的最小列和最大列
        limitcol = gridManager.CheckPiece(currentDraggingPiece);
        Debug.unityLogger.Log(string.Format("cols={0},{1}", limitcol.Key, limitcol.Value));
        
        isDragging = true;
        
        // 计算方块可移动的最左边界位置（世界坐标）
        var minx = PiecesManager.getPosition(currentDraggingPiece.mRow, limitcol.Key, gridManager.cellSize, gridManager.startOffset).x;
        
        // 修改：正确计算最右边界位置，考虑方块宽度
        // 这里计算的是方块左侧的X坐标，让其最大只能到达(最大列位置 - 方块宽度 + 1)的位置
        int maxAllowedCol = limitcol.Value - currentDraggingPiece.mCount + 1;
        var maxx = PiecesManager.getPosition(currentDraggingPiece.mRow, maxAllowedCol, gridManager.cellSize, gridManager.startOffset).x;
        
        // 保存X坐标移动范围限制
        limitposx = new KeyValuePair<float, float>(minx, maxx);
        
        if (Debug.isDebugBuild)
        {
            Debug.Log($"方块宽度: {currentDraggingPiece.mCount}, 可移动列范围: {limitcol.Key}~{limitcol.Value}, X坐标范围: {minx}~{maxx}");
        }
        
        // 创建拖动效果
        CreateDragEffects();
    }

    /// <summary>
    /// 拖动过程中的处理，更新方块位置
    /// </summary>
    public void TouchDrag()
    {
        // 获取当前触摸/鼠标位置
        var dragpos = GetTouchPosition();
        
        // 计算速度
        float currentTime = Time.time;
        if (currentTime > lastPositionTime) // 避免除零
        {
            currentVelocity = ((Vector2)dragpos - lastPosition) / (currentTime - lastPositionTime);
            dragSpeed = currentVelocity.magnitude;
            
            if (Debug.isDebugBuild && Time.frameCount % 30 == 0) // 每30帧输出一次
            {
                Debug.Log($"拖动速度: {dragSpeed}");
            }
        }
        
        // 更新位置记录
        lastPosition = dragpos;
        lastPositionTime = currentTime;
        
        // 计算水平方向移动的距离
        var diffx = dragpos.x - touchStartPosition.x;
        
        // 获取方块在网格中的原始位置（世界坐标）
        var oldpos = PiecesManager.getPosition(currentDraggingPiece.mRow, currentDraggingPiece.mCol, gridManager.cellSize, gridManager.startOffset);
        // 计算新的X坐标
        var newx = oldpos.x + diffx;
        
        // 限制X坐标在允许范围内
        if (newx < limitposx.Key)
        {
            newx = limitposx.Key; // 限制在最左边界
        }
        else if (newx > limitposx.Value)
        {
            newx = limitposx.Value; // 限制在最右边界
        }
        
        // 处理慢速拖动的吸附效果
        if (enableSnapEffect && dragSpeed < slowDragThreshold)
        {
            ApplySnapEffect(ref newx, oldpos);
        }
        else if (isSnapped)
        {
            // 如果速度变快，从吸附状态解除
            isSnapped = false;
            snappedColumn = -1;
        }
        
        // 更新方块位置，只修改X坐标，保持Y和Z不变
        currentDraggingPiece.transform.position = new Vector3(newx, oldpos.y, oldpos.z);
        
        // 更新拖动效果位置
        UpdateDragEffects(newx, oldpos.y);
    }
    
    /// <summary>
    /// 应用吸附效果
    /// </summary>
    /// <param name="newx">引用参数，方块的X坐标</param>
    /// <param name="oldpos">方块在网格中的原始位置</param>
    private void ApplySnapEffect(ref float newx, Vector3 oldpos)
    {
        // 计算当前位置最接近的列
        float relativeX = newx - gridManager.startOffset.x;
        int nearestCol = Mathf.RoundToInt(relativeX / gridManager.cellSize);
        
        // 确保列在有效范围内
        nearestCol = Mathf.Clamp(nearestCol, limitcol.Key, limitcol.Value - currentDraggingPiece.mCount + 1);
        
        // 计算到最近格子中心的距离
        float nearestColCenterX = PiecesManager.getPosition(currentDraggingPiece.mRow, nearestCol, gridManager.cellSize, gridManager.startOffset).x;
        float distance = Mathf.Abs(newx - nearestColCenterX);
        
        // 计算拖动方向 (-1左, 1右)
        dragDirection = (int)Mathf.Sign(newx - nearestColCenterX);
        if (dragDirection == 0) dragDirection = 1; // 默认方向，避免为0
        
        // 离散式吸附/拔出逻辑
        if (!isSnapped)
        {
            // 未处于吸附状态，检查是否需要吸附
            if (distance < snapThreshold)
            {
                // 直接设置为格子中心位置（瞬间吸附）
                newx = nearestColCenterX;
                isSnapped = true;
                snappedColumn = nearestCol;
                
                // 添加反馈
                #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
                #endif
                
                if (Debug.isDebugBuild)
                {
                    Debug.Log($"吸附到列 {nearestCol}");
                }
            }
        }
        else
        {
            // 已处于吸附状态，检查是否需要拔出
            if (distance > snapThreshold)
            {
                // 触发拔出效果，添加额外位移
                newx = nearestColCenterX + (dragDirection * (snapThreshold + pullOutDistance));
                isSnapped = false;
                
                // 添加反馈
                #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
                #endif
                
                if (Debug.isDebugBuild)
                {
                    Debug.Log($"从吸附状态拔出，方向: {dragDirection}");
                }
            }
            else
            {
                // 仍在吸附阈值内，保持吸附
                newx = nearestColCenterX;
            }
        }
    }

    /// <summary>
    /// 结束拖动时的处理，确定最终位置并更新网格数据
    /// </summary>
    public void TouchEnd()
    {
        // 获取结束拖动时的位置
        var endpos = GetTouchPosition();
        // 计算水平移动总距离
        var diffx = endpos.x - touchStartPosition.x;
        
        // 记录拖动前的列位置，用于后续比较
        int originalColumn = currentDraggingPiece.mCol;
        
        // 如果在吸附状态，优先使用吸附的列
        int diffcol;
        if (isSnapped && snappedColumn >= 0)
        {
            diffcol = snappedColumn;
            if (Debug.isDebugBuild)
            {
                Debug.Log($"使用吸附位置: 列 {diffcol}");
            }
        }
        else
        {
            // 根据移动距离计算目标列位置
            diffcol = getNowCol(currentDraggingPiece, diffx);
        }
        
        // 检查方块是否真的移动了位置
        bool hasMoved = diffcol != originalColumn;
        
        if (Debug.isDebugBuild)
        {
            Debug.Log($"方块从列 {originalColumn} 移动到列 {diffcol}，移动状态: {(hasMoved ? "已移动" : "未移动")}");
        }
        
        // 委托给GridManager更新网格数据和方块位置
        gridManager.movePiece(currentDraggingPiece, diffcol);
        
        // 计算新位置的世界坐标并更新方块显示位置
        var newpos = PiecesManager.getPosition(currentDraggingPiece.mRow, diffcol, gridManager.cellSize, gridManager.startOffset);
        currentDraggingPiece.transform.position = new Vector3(newpos.x, newpos.y, newpos.z);
        
        // 销毁拖动效果
        DestroyDragEffects();
        
        // 重置速度和吸附状态
        dragSpeed = 0f;
        isSnapped = false;
        snappedColumn = -1;
        
        // 只有当方块真的移动了位置时，才触发后续处理
        if (hasMoved)
        {
            // 查找GameFlowManager处理游戏流程
            GameFlowManager flowManager = FindObjectOfType<GameFlowManager>();
            if (flowManager != null)
            {
                // 使用新的流程管理器处理后续流程
                flowManager.StartGameFlow();
            }
            else
            {
                // 向下兼容：如果没有GameFlowManager，使用原有方法
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("未找到GameFlowManager，回退到原有的TriggerDropsNextFrame");
                }
                StartCoroutine(TriggerDropsNextFrame());
            }
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("方块未移动，不触发下落和生成新行");
        }
    }
    
    /// <summary>
    /// 在下一帧触发方块下落处理，确保拖动结束后完成
    /// </summary>
    private IEnumerator TriggerDropsNextFrame()
    {
        // 等待一帧，确保拖动相关操作全部完成
        yield return null;
        
        // 获取GridManager引用
        GridManager gridManager = FindObjectOfType<GridManager>();
        if(gridManager == null)
        {
            Debug.LogError("找不到GridManager组件！");
            yield break;
        }
        
        // 锁定游戏状态
        GameProcessController.IsProcessingActions = true;
        
        try
        {
            // 查找DropAnimator组件
            DropAnimator dropAnimator = FindObjectOfType<DropAnimator>();
            if (dropAnimator != null)
            {
                // 触发下落和消除链处理
                dropAnimator.DropAllPossiblePieces();
                
                // 等待动画完成
                while(dropAnimator.IsAnimating)
                {
                    yield return null;
                }
                
                if (Debug.isDebugBuild)
                {
                    Debug.Log("拖动结束后触发了下落和消除检查");
                }
            }
            else
            {
                // 如果没有DropAnimator，尝试直接触发消除检查
                LineClearAnimator lineClearAnimator = FindObjectOfType<LineClearAnimator>();
                if (lineClearAnimator != null)
                {
                    // 先处理数据层下落
                    var drops = gridManager.ProcessDrops();
                    
                    // 如果有下落，更新方块视觉位置
                    foreach (var kvp in drops)
                    {
                        PiecesManager piece = kvp.Key;
                        int targetRow = kvp.Value;
                        
                        // 更新方块位置
                        var newPosition = PiecesManager.getPosition(targetRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                        piece.transform.position = newPosition;
                    }
                    
                    // 检查消除
                    lineClearAnimator.CheckAndClearLines();
                    
                    // 等待消除动画完成
                    while(lineClearAnimator.IsAnimating)
                    {
                        yield return null;
                    }
                }
                else if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("无法找到DropAnimator或LineClearAnimator组件，下落和消除动画将不会执行");
                    
                    // 直接进行数据层处理
                    gridManager.ProcessDrops();
                    gridManager.ProcessLineClear();
                }
            }
            
            // 短暂延迟以便观察
            yield return new WaitForSeconds(0.2f);
            
            // 无论如何，都生成新行
            Debug.Log("开始生成新行...");
            bool success = gridManager.GenerateNewRow();
            Debug.Log($"新行生成{(success ? "成功" : "失败")}");
            
            if (success)
            {
                // 查找RowGeneratorAnimator组件处理动画
                RowGeneratorAnimator rowGeneratorAnimator = FindObjectOfType<RowGeneratorAnimator>();
                if (rowGeneratorAnimator != null)
                {
                    // 获取新生成的方块（底部行的方块）
                    List<PiecesManager> newPieces = gridManager.GetPiecesInRow(0);
                    
                    // 获取其他所有方块（非底部行的方块）
                    List<PiecesManager> existingPieces = gridManager.GetAllPieces()
                        .Where(p => p.mRow > 0).ToList();
                    
                    // 触发动画
                    rowGeneratorAnimator.AnimateNewRowGeneration(newPieces, existingPieces);
                    
                    // 等待动画完成
                    while (rowGeneratorAnimator.IsAnimating)
                    {
                        yield return null;
                    }
                    
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("新行生成动画已完成");
                    }
                }
                else
                {
                    // 如果没有动画组件，直接更新位置
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("未找到RowGeneratorAnimator组件，将直接更新方块位置");
                    }
                    
                    // 更新方块视觉位置以匹配数据层
                    foreach (var piece in gridManager.GetAllPieces())
                    {
                        Vector3 correctPosition = PiecesManager.getPosition(
                            piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                        piece.transform.position = correctPosition;
                    }
                }
            }
            else
            {
                // 更新方块视觉位置以匹配数据层
                foreach (var piece in gridManager.GetAllPieces())
                {
                    Vector3 correctPosition = PiecesManager.getPosition(
                        piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                    piece.transform.position = correctPosition;
                }
            }
        }
        finally
        {
            // 确保最终解锁游戏状态
            GameProcessController.IsProcessingActions = false;
        }
    }

    /// <summary>
    /// 获取鼠标/触摸在世界坐标系中的位置
    /// </summary>
    /// <returns>鼠标/触摸的世界坐标</returns>
    public Vector2 GetTouchPosition()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePosition = Input.mousePosition;
        //Debug.Log($"鼠标点击位置: {mousePosition}, 检测对象: {gameObject.name}");
            
        // 将屏幕坐标转换为世界坐标
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        //Debug.Log($"worldPoint: {worldPoint}, 检测对象: {gameObject.name}");
        return worldPoint;
    }

    /// <summary>
    /// 根据拖动距离计算方块应该位于哪一列
    /// </summary>
    /// <param name="piecesManager">方块管理器</param>
    /// <param name="totleDragX">总拖动距离X</param>
    /// <returns>目标列索引</returns>
    public int getNowCol(PiecesManager piecesManager, float totleDragX)
    {
        // 计算拖动引起的列偏移量，添加了一个修正系数以处理边界情况
        int diffcol = (int)((totleDragX + (totleDragX > 0 ? 0.725f: -0.725f)/2)/0.725f);
        // 计算新列位置
        int nowcol = piecesManager.mCol + diffcol;
        
        // 确保结果在有效范围内
        nowcol = math.max(limitcol.Key, nowcol); // 不小于最小列
        nowcol = math.min(limitcol.Value - piecesManager.mCount + 1, nowcol); // 不大于最大列 - 方块宽度 + 1
        
        return nowcol;
    }
    
    /// <summary>
    /// 创建拖动效果
    /// </summary>
    private void CreateDragEffects()
    {
        // 如果已存在拖动效果，先销毁
        DestroyDragEffects();
        
        // 创建容器对象
        dragEffectContainer = new GameObject("DragEffects");
        
        // 如果没有当前拖动的方块，直接返回
        if (currentDraggingPiece == null) return;
        
        // 获取方块占据的列范围
        int startCol = currentDraggingPiece.mCol;
        int endCol = startCol + currentDraggingPiece.mCount - 1;
        
        // 为每一列创建整列效果
        for (int col = startCol; col <= endCol; col++)
        {
            // 创建一个表示整列的效果对象
            GameObject colEffect = new GameObject($"Col_{col}_Effect");
            colEffect.transform.SetParent(dragEffectContainer.transform);
            
            // 获取该列的X坐标位置
            float colX = PiecesManager.getPosition(0, col, gridManager.cellSize, gridManager.startOffset).x;
            
            // 创建覆盖整列的效果
            GameObject columnHighlight = new GameObject($"ColumnHighlight_{col}");
            columnHighlight.transform.SetParent(colEffect.transform);
            
            // 添加精灵渲染器
            SpriteRenderer columnRenderer = columnHighlight.AddComponent<SpriteRenderer>();
            
            // 创建一个纯色精灵
            Texture2D texture = new Texture2D(1, 1);
            
            // 使用自定义颜色和透明度
            Color finalColor = effectColor;
            finalColor.a = dragEffectAlpha; // 应用透明度设置
            texture.SetPixel(0, 0, finalColor);
            
            texture.Apply();
            Sprite columnSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            
            columnRenderer.sprite = columnSprite;
            
            // 计算列的高度和位置，使用用户设置的高度比例
            float columnHeight = gridManager.rows * gridManager.cellSize;
            
            // 根据高度比例调整Y位置，使其垂直居中
            float columnY;
            if (columnHeightScale >= 1.0f)
            {
                // 当高度比例>=1时，保持中心对齐
                columnY = gridManager.startOffset.y + columnHeight / 2 - gridManager.cellSize / 2;
            }
            else
            {
                // 当高度比例<1时，调整以保持顶部对齐
                float scaledHeight = columnHeight * columnHeightScale;
                columnY = gridManager.startOffset.y + scaledHeight / 2;
            }
            
            // 设置精灵大小和位置，使用用户设置的宽度比例
            columnRenderer.transform.position = new Vector3(colX, columnY, 0);
            columnRenderer.transform.localScale = new Vector3(gridManager.cellSize * columnWidthScale*110, columnHeight * columnHeightScale*100, 1);
            
            // 设置排序顺序，使用用户设置的值确保显示在正确层级
            columnRenderer.sortingOrder = effectSortingOrder;
        }
        
        if (Debug.isDebugBuild)
        {
            Debug.Log($"创建了拖动效果，宽度比例:{columnWidthScale}，高度比例:{columnHeightScale}，层级:{effectSortingOrder}");        }
    }
    
    /// <summary>
    /// 更新拖动效果位置
    /// </summary>
    private void UpdateDragEffects(float newX, float newY)
    {
        if (dragEffectContainer == null || currentDraggingPiece == null) return;
        
        // 计算方块当前位置与目标位置的X偏移
        float offsetX = newX - PiecesManager.getPosition(currentDraggingPiece.mRow, currentDraggingPiece.mCol, gridManager.cellSize, gridManager.startOffset).x;
        
        // 获取方块占据的列数
        int pieceWidth = currentDraggingPiece.mCount;
        
        // 更新每列效果的位置
        for (int i = 0; i < pieceWidth; i++)
        {
            Transform colEffect = dragEffectContainer.transform.Find($"Col_{currentDraggingPiece.mCol + i}_Effect");
            if (colEffect != null)
            {
                // 获取基准X坐标位置（原始列位置）
                float baseX = PiecesManager.getPosition(0, currentDraggingPiece.mCol + i, gridManager.cellSize, gridManager.startOffset).x;
                
                // 更新该列效果的所有子对象位置
                foreach (Transform child in colEffect)
                {
                    Vector3 pos = child.position;
                    child.position = new Vector3(baseX + offsetX, pos.y, pos.z);
                }
            }
        }
    }
    
    /// <summary>
    /// 销毁拖动效果
    /// </summary>
    private void DestroyDragEffects()
    {
        if (dragEffectContainer != null)
        {
            Destroy(dragEffectContainer);
            dragEffectContainer = null;
        }
    }
}
