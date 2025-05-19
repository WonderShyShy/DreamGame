using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 新行生成动画控制器
/// 负责处理新行生成的视觉效果，从-1行生成并推动到0行
/// </summary>
public class RowGeneratorAnimator : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private float animationDuration = 0.5f; // 动画持续时间
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 动画曲线
    
    [Header("调试选项")]
    [SerializeField] private bool debugMode = true; // 是否开启调试日志
    
    // 引用
    private GridManager gridManager;
    
    // 状态控制
    private bool isAnimating = false; // 是否正在播放动画
    public bool IsAnimating => isAnimating; // 公开动画状态只读属性
    
    // 事件委托 - 当生成动画完成时触发
    public event System.Action OnAnimationComplete;
    
    /// <summary>
    /// 初始化组件，获取必要引用
    /// </summary>
    private void Start()
    {
        // 获取GridManager引用
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("场景中没有GridManager组件！");
            enabled = false; // 禁用组件
            return;
        }
        
        if (debugMode)
        {
            Debug.Log("RowGeneratorAnimator初始化完成，准备处理新行生成动画");
        }
    }
    
    /// <summary>
    /// 触发新行生成动画
    /// </summary>
    /// <param name="newPieces">新生成的方块（底部行的方块）</param>
    /// <param name="existingPieces">其他所有方块</param>
    public void AnimateNewRowGeneration(List<PiecesManager> newPieces, List<PiecesManager> existingPieces)
    {
        if (isAnimating)
        {
            if (debugMode) Debug.Log("已有动画正在进行，跳过本次动画");
            return;
        }
        
        if (debugMode)
        {
            Debug.Log($"开始新行生成动画，新方块数量：{newPieces.Count}，现有方块数量：{existingPieces.Count}");
        }
        
        StartCoroutine(AnimateRowGenerationRoutine(newPieces, existingPieces));
    }
    
    /// <summary>
    /// 执行新行生成动画
    /// </summary>
    private IEnumerator AnimateRowGenerationRoutine(List<PiecesManager> newPieces, List<PiecesManager> existingPieces)
    {
        isAnimating = true;
        
        // 标记为正在处理操作，防止其他交互
        GameProcessController.IsProcessingActions = true;
        
        try
        {
            // 1. 将新生成的方块放在-1行的位置
            foreach (var piece in newPieces)
            {
                Vector3 startPos = PiecesManager.getPosition(
                    -1, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                piece.transform.position = startPos;
                
                if (debugMode)
                {
                    Debug.Log($"方块起始位置设置在-1行：{piece.name}, 位置：{startPos}");
                }
            }
            
            // 2. 记录所有方块的起始位置和目标位置
            Dictionary<PiecesManager, Vector3> startPositions = new Dictionary<PiecesManager, Vector3>();
            Dictionary<PiecesManager, Vector3> targetPositions = new Dictionary<PiecesManager, Vector3>();
            
            // 新方块从-1行移动到0行
            foreach (var piece in newPieces)
            {
                startPositions[piece] = piece.transform.position;
                targetPositions[piece] = PiecesManager.getPosition(
                    piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
            }
            
            // 现有方块从当前位置移动到新位置
            foreach (var piece in existingPieces)
            {
                startPositions[piece] = piece.transform.position;
                targetPositions[piece] = PiecesManager.getPosition(
                    piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
            }
            
            if (debugMode)
            {
                Debug.Log($"开始执行移动动画，总共 {startPositions.Count} 个方块需要移动");
            }
            
            // 3. 执行动画
            float elapsed = 0;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / animationDuration);
                float curveValue = movementCurve.Evaluate(normalizedTime);
                
                // 更新所有方块位置
                foreach (var kvp in startPositions)
                {
                    PiecesManager piece = kvp.Key;
                    if (piece == null) continue; // 防止空引用
                    
                    Vector3 start = kvp.Value;
                    Vector3 target = targetPositions[piece];
                    
                    piece.transform.position = Vector3.Lerp(start, target, curveValue);
                }
                
                yield return null;
            }
            
            // 4. 确保所有方块到达最终位置
            foreach (var kvp in targetPositions)
            {
                if (kvp.Key != null) // 防止空引用
                {
                    kvp.Key.transform.position = kvp.Value;
                }
            }
            
            if (debugMode)
            {
                Debug.Log("新行生成动画完成");
            }
            
            // 触发动画完成事件
            OnAnimationComplete?.Invoke();
        }
        finally
        {
            // 确保动画状态被重置
            isAnimating = false;
            GameProcessController.IsProcessingActions = false;
        }
    }
    
    /// <summary>
    /// 强制执行新行生成
    /// </summary>
    /// <param name="immediate">是否立即完成（不播放动画）</param>
    public void ForceRowGeneration(bool immediate = false)
    {
        if (isAnimating)
        {
            // 停止所有协程
            StopAllCoroutines();
            isAnimating = false;
            GameProcessController.IsProcessingActions = false;
        }
        
        // 执行数据层生成
        bool success = gridManager.GenerateNewRow();
        
        if (success)
        {
            if (!immediate)
            {
                // 获取新生成的方块（底部行的方块）
                List<PiecesManager> newPieces = gridManager.GetPiecesInRow(0);
                
                // 获取其他所有方块（非底部行的方块）
                List<PiecesManager> existingPieces = gridManager.GetAllPieces()
                    .Where(p => p.mRow > 0).ToList();
                
                // 触发动画
                AnimateNewRowGeneration(newPieces, existingPieces);
            }
            else
            {
                // 立即更新所有方块位置
                foreach (var piece in gridManager.GetAllPieces())
                {
                    Vector3 correctPosition = PiecesManager.getPosition(
                        piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                    piece.transform.position = correctPosition;
                }
                
                // 触发动画完成事件
                OnAnimationComplete?.Invoke();
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("生成新行失败");
        }
    }
} 