using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责处理方块下落的动画效果
/// 作为表现层，与GridManager的数据层分离
/// </summary>
public class DropAnimator : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private float dropDuration = 0.3f; // 下落动画持续时间
    [SerializeField] private float layerDelay = 0.1f; // 不同层之间的延迟时间
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 下落动画曲线

    [Header("调试选项")]
    [SerializeField] private bool debugMode = true; // 是否开启调试日志
    [SerializeField] private bool autoCheckDrops = true; // 是否自动检查下落
    [SerializeField] private float autoCheckInterval = 0.5f; // 自动检查间隔

    // 引用
    private GridManager gridManager;
    private LineClearAnimator lineClearAnimator; // 行消除动画器引用

    // 状态控制
    private bool isAnimating = false; // 是否正在播放动画
    public bool IsAnimating => isAnimating; // 公开动画状态只读属性

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

        // 获取LineClearAnimator引用
        lineClearAnimator = FindObjectOfType<LineClearAnimator>();
        if (lineClearAnimator == null)
        {
            Debug.LogWarning("场景中没有LineClearAnimator组件，将无法执行消除动画！");
        }
        else
        {
            // 订阅消除动画完成事件
            lineClearAnimator.OnClearAnimationComplete += OnClearAnimationComplete;
        }

        // 启动自动检查协程
        if (autoCheckDrops)
        {
            StartCoroutine(AutoCheckDropsRoutine());
        }

        if (debugMode)
        {
            Debug.Log("DropAnimator初始化完成，准备处理下落动画");
        }
    }

    /// <summary>
    /// 组件销毁时取消事件订阅
    /// </summary>
    private void OnDestroy()
    {
        if (lineClearAnimator != null)
        {
            lineClearAnimator.OnClearAnimationComplete -= OnClearAnimationComplete;
        }
    }

    /// <summary>
    /// 消除动画完成事件处理方法
    /// </summary>
    private void OnClearAnimationComplete(List<int> clearedRows)
    {
        if (debugMode)
        {
            Debug.Log($"消除动画完成，触发下落检查");
        }

        // 触发下落检查
        TriggerDropCheck();
    }

    /// <summary>
    /// 定期自动检查是否有方块可以下落
    /// </summary>
    private IEnumerator AutoCheckDropsRoutine()
    {
        // 等待一段时间确保游戏完全初始化
        yield return new WaitForSeconds(0.3f);

        while (autoCheckDrops)
        {
            // 如果没有正在进行的动画且游戏没有锁定，则触发下落检查
            if (!isAnimating && !GameProcessController.IsProcessingActions)
            {
                TriggerDropCheck();
            }

            // 等待指定间隔
            yield return new WaitForSeconds(autoCheckInterval);
        }
    }

    /// <summary>
    /// 触发下落检查，如果有方块可以下落则开始动画
    /// </summary>
    public void TriggerDropCheck()
    {
        // 如果已经在动画中或游戏被锁定，则跳过
        if (isAnimating || GameProcessController.IsProcessingActions || gridManager == null)
        {
            if (debugMode)
            {
                Debug.Log($"跳过下落检查：isAnimating={isAnimating}, IsProcessingActions={GameProcessController.IsProcessingActions}");
            }
            return;
        }

        // 获取所有可下落的方块
        var dropsData = gridManager.CheckDown();

        if (dropsData.Count > 0)
        {
            if (debugMode)
            {
                Debug.Log($"检测到{dropsData.Count}个方块可以下落，开始动画");
            }

            // 开始下落动画
            StartCoroutine(AnimateLayeredDrop(dropsData));
        }
        else if (debugMode)
        {
            Debug.Log("没有方块可以下落，检查行消除");

            // 如果没有方块可以下落，检查行消除
            CheckLineClear();
        }
    }

    /// <summary>
    /// 检查行消除
    /// </summary>
    private void CheckLineClear()
    {
        // 如果没有行消除动画器，直接返回
        if (lineClearAnimator == null) return;

        // 触发行消除检查
        lineClearAnimator.CheckAndClearLines();
    }

    /// <summary>
    /// 执行所有可能的方块下落
    /// </summary>
    public void DropAllPossiblePieces()
    {
        if (isAnimating || GameProcessController.IsProcessingActions)
        {
            return;
        }

        StartCoroutine(ProcessDropsAndClear());
    }

    /// <summary>
    /// 处理下落和消除循环
    /// </summary>
    private IEnumerator ProcessDropsAndClear()
    {
        // 标记为正在处理
        GameProcessController.IsProcessingActions = true;

        try
        {
            // 检查并处理下落
            var dropsData = gridManager.CheckDown();
            if (dropsData.Count > 0)
            {
                // 执行下落动画
                yield return AnimateLayeredDrop(dropsData);

                // 短暂延迟以便观察
                yield return new WaitForSeconds(0.1f);

                // 下落完成后，检查行消除
                CheckLineClear();
            }
            else
            {
                // 如果没有方块可以下落，直接检查行消除
                CheckLineClear();

                // 完成处理
                GameProcessController.IsProcessingActions = false;
            }
        }
        finally
        {
            // 注意：不在这里解除锁定，因为后续可能还有消除动画
            // 消除动画完成后会再次触发下落检查，形成循环
            // 最终会在没有更多变化时解除锁定
            if (!isAnimating && (lineClearAnimator == null || !lineClearAnimator.IsAnimating))
            {
                GameProcessController.IsProcessingActions = false;
            }
        }
    }

    /// <summary>
    /// 按层执行下落动画
    /// </summary>
    private IEnumerator AnimateLayeredDrop(Dictionary<PiecesManager, int> dropsData)
    {
        if (dropsData.Count == 0) yield break;

        // 标记为正在动画中
        isAnimating = true;

        try
        {
            // 记录原始位置和目标位置
            Dictionary<PiecesManager, (int originalRow, int targetRow)> dropInfo =
                new Dictionary<PiecesManager, (int originalRow, int targetRow)>();

            foreach (var kvp in dropsData)
            {
                dropInfo[kvp.Key] = (kvp.Key.mRow, kvp.Value);
            }

            // 按行分组方块（从高到低）
            Dictionary<int, List<PiecesManager>> piecesByRow = new Dictionary<int, List<PiecesManager>>();

            foreach (var kvp in dropInfo)
            {
                int originalRow = kvp.Value.originalRow;

                if (!piecesByRow.ContainsKey(originalRow))
                {
                    piecesByRow[originalRow] = new List<PiecesManager>();
                }

                piecesByRow[originalRow].Add(kvp.Key);
            }

            // 从低到高处理每一行
            List<int> rows = new List<int>(piecesByRow.Keys);
            rows.Sort(); // 升序排列

            foreach (int row in rows)
            {
                // 获取当前行的所有方块
                List<PiecesManager> piecesInRow = piecesByRow[row];

                if (debugMode)
                {
                    Debug.Log($"开始处理第{row}行，共{piecesInRow.Count}个方块");
                }

                // 更新网格数据
                foreach (var piece in piecesInRow)
                {
                    int targetRow = dropInfo[piece].targetRow;

                    // 创建可能的批量更新
                    List<(int row, int col, int value)> updates = new List<(int row, int col, int value)>();

                    // 清除原位置
                    for (int c = piece.mCol; c < piece.mCol + piece.mCount; c++)
                    {
                        updates.Add((row, c, 0)); // 原位置设为0
                    }

                    // 更新到新位置
                    for (int c = piece.mCol; c < piece.mCol + piece.mCount; c++)
                    {
                        updates.Add((targetRow, c, 1)); // 新位置设为1
                    }

                    // 批量更新网格数据
                    gridManager.BatchUpdateCells(updates);

                    // 更新方块数据
                    piece.mRow = targetRow;
                }

                // 开始同时动画处理当前行的所有方块
                List<Coroutine> animations = new List<Coroutine>();

                foreach (var piece in piecesInRow)
                {
                    // 添加每个方块的动画
                    animations.Add(StartCoroutine(AnimateSinglePieceDrop(piece, row, dropInfo[piece].targetRow)));
                }

                // 等待这一行的所有动画完成
                foreach (var animation in animations)
                {
                    yield return animation;
                }

                // 在处理下一行之前添加延迟
                yield return new WaitForSeconds(layerDelay);
            }

            // 所有下落完成
            if (debugMode)
            {
                Debug.Log($"所有方块下落动画完成，共{dropsData.Count}个方块");
            }

            // 下落完成后检查行消除
            CheckLineClear();
        }
        finally
        {
            // 标记动画结束
            isAnimating = false;
        }
    }

    /// <summary>
    /// 执行单个方块的下落动画
    /// </summary>
    private IEnumerator AnimateSinglePieceDrop(PiecesManager piece, int startRow, int targetRow)
    {
        if (piece == null)
        {
            Debug.LogWarning("尝试对空方块进行动画处理");
            yield break;
        }

        // 记录起始位置和目标位置
        Vector3 startPosition = piece.transform.position;
        Vector3 targetPosition = PiecesManager.getPosition(targetRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);

        float elapsed = 0f;

        // 执行下落动画
        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / dropDuration);
            float curveValue = dropCurve.Evaluate(normalizedTime);

            // 更新位置
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        // 确保最终位置精确
        piece.transform.position = targetPosition;
    }
}