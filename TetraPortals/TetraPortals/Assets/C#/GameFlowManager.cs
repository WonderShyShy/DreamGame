using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 游戏流程管理器
/// 负责控制整个游戏逻辑流程，确保正确的顺序执行：移动→下落→消除→下落→消除→...→稳定→生成新行
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    [Header("调试选项")]
    [SerializeField] private bool debugMode = true; // 是否开启调试日志

    // 引用
    private GridManager gridManager;
    private DropAnimator dropAnimator;
    private LineClearAnimator lineClearAnimator;
    private RowGeneratorAnimator rowGeneratorAnimator;

    // 状态控制
    private bool isProcessing = false; // 是否正在处理游戏流程

    private void Start()
    {
        // 获取必要组件
        gridManager = FindObjectOfType<GridManager>();
        dropAnimator = FindObjectOfType<DropAnimator>();
        lineClearAnimator = FindObjectOfType<LineClearAnimator>();
        rowGeneratorAnimator = FindObjectOfType<RowGeneratorAnimator>();

        if (gridManager == null)
        {
            Debug.LogError("未找到GridManager组件！");
            enabled = false;
            return;
        }

        if (debugMode)
        {
            Debug.Log("GameFlowManager初始化完成");

            if (dropAnimator == null)
                Debug.LogWarning("未找到DropAnimator组件，将使用数据层下落处理");

            if (lineClearAnimator == null)
                Debug.LogWarning("未找到LineClearAnimator组件，将使用数据层消除处理");

            if (rowGeneratorAnimator == null)
                Debug.LogWarning("未找到RowGeneratorAnimator组件，将使用直接位置更新");
        }
    }

    /// <summary>
    /// 玩家移动方块后调用此方法启动游戏流程
    /// </summary>
    public void StartGameFlow()
    {
        if (isProcessing)
        {
            if (debugMode)
                Debug.Log("已有游戏流程正在处理中，忽略此次调用");
            return;
        }

        isProcessing = true;
        StartCoroutine(GameFlowRoutine());
    }

    /// <summary>
    /// 游戏流程协程
    /// </summary>
    private IEnumerator GameFlowRoutine()
    {
        if (debugMode)
            Debug.Log("开始游戏流程");

        // 锁定游戏状态
        GameProcessController.IsProcessingActions = true;

        try
        {
            // 反复执行下落和消除，直到棋盘稳定
            bool hasChanges;
            do
            {
                hasChanges = false;

                // 1. 执行下落
                bool hasDrops = false;
                yield return ExecuteDrops((result) => hasDrops = result);

                // 2. 执行消除
                bool hasClears = false;
                yield return ExecuteClears((result) => hasClears = result);

                // 如果有下落或消除，标记有变化
                hasChanges = hasDrops || hasClears;

                if (debugMode && hasChanges)
                    Debug.Log($"循环检查：有{(hasDrops ? "下落" : "")}{(hasDrops && hasClears ? "和" : "")}{(hasClears ? "消除" : "")}，继续循环");

            } while (hasChanges);

            if (debugMode)
                Debug.Log("棋盘已稳定，开始生成新行");

            // 3. 生成新行
            yield return ExecuteNewRowGeneration();

            // 4. 再次检查下落和消除（确保生成的新行产生的连锁反应）
            do
            {
                hasChanges = false;

                // 执行下落
                bool hasDrops = false;
                yield return ExecuteDrops((result) => hasDrops = result);

                // 执行消除
                bool hasClears = false;
                yield return ExecuteClears((result) => hasClears = result);

                // 如果有下落或消除，标记有变化
                hasChanges = hasDrops || hasClears;

                if (debugMode && hasChanges)
                    Debug.Log($"新行后检查：有{(hasDrops ? "下落" : "")}{(hasDrops && hasClears ? "和" : "")}{(hasClears ? "消除" : "")}，继续循环");

            } while (hasChanges);

            if (debugMode)
                Debug.Log("游戏流程结束，棋盘已稳定");
        }
        finally
        {
            // 解锁游戏状态
            GameProcessController.IsProcessingActions = false;
            isProcessing = false;
        }
    }

    /// <summary>
    /// 执行下落逻辑
    /// </summary>
    /// <param name="resultCallback">回调函数，返回是否有方块下落</param>
    private IEnumerator ExecuteDrops(System.Action<bool> resultCallback)
    {
        bool hasDrops = false;

        if (debugMode)
            Debug.Log("开始执行下落检查");

        //// 优先使用DropAnimator
        //if (dropAnimator != null)
        //{
        //    // 记录动画前的状态
        //    bool wasAnimating = dropAnimator.IsAnimating;

        //    // 触发下落处理
        //    dropAnimator.DropAllPossiblePieces();

        //    // 如果触发后开始动画，说明有下落
        //    hasDrops = dropAnimator.IsAnimating || wasAnimating != dropAnimator.IsAnimating;

        //    if (hasDrops)
        //    {
        //        if (debugMode)
        //            Debug.Log("等待下落动画完成");

        //        // 等待动画完成
        //        while (dropAnimator.IsAnimating)
        //            yield return null;
        //    }
        //}
        //else
        //{
        // 备用：直接使用数据层处理
        var drops = gridManager.ProcessDrops();
        hasDrops = drops.Count > 0;

        if (hasDrops)
        {
            if (debugMode)
                Debug.Log($"数据层下落处理：{drops.Count}个方块下落");

            // 更新方块视觉位置
            foreach (var kvp in drops)
            {
                PiecesManager piece = kvp.Key;
                int targetRow = kvp.Value;

                var newPosition = PiecesManager.getPosition(
                    targetRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                //piece.transform.position = newPosition;
                AnimateToPosition(piece.transform, newPosition);
            }

            // 添加短暂延迟，让玩家能看到变化
            yield return new WaitForSeconds(0.5f);
        }
        //}

        //if (debugMode)
        Debug.Log($"下落检查完成：{(hasDrops ? "有" : "无")}下落");


        resultCallback(hasDrops);
    }

    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private float bounceAmplitude = 0.05f;


    public void AnimateToPosition(Transform Stransform , Vector3 targetPosition)
    {
        // 记录初始位置
        Vector3 startPos = Stransform.position;

        // 创建并保存Tween引用
        Tween moveTween = Stransform.DOMove(targetPosition, animationDuration)
            .SetEase(Ease.OutQuart); // 先快后慢的缓动效果

        // 添加回落效果（使用独立的Update回调）
        DOTween.To(() => 0f, x => {
            float progress = x;

            // 添加回落效果
            if (progress > 0.8f)
            {
                float bounceProgress = (progress - 0.8f) / 0.2f;
                float bounceEffect = Mathf.Sin(bounceProgress * Mathf.PI) * bounceAmplitude;
                Vector3 currentPos = Stransform.position;
                currentPos.y = targetPosition.y - bounceEffect;
                Stransform.position = currentPos;
            }
        }, 1f, animationDuration)
        .SetEase(Ease.Linear);

        // 确保最终位置精确
        moveTween.OnComplete(() => {
            Stransform.position = targetPosition;
        });
    }

    /// <summary>
    /// 执行消除逻辑
    /// </summary>
    /// <param name="resultCallback">回调函数，返回是否有行被消除</param>
    private IEnumerator ExecuteClears(System.Action<bool> resultCallback)
    {
        bool hasClears = false;

        if (debugMode)
            Debug.Log("开始执行消除检查");

        // 优先使用LineClearAnimator
        if (lineClearAnimator != null)
        {
            hasClears = lineClearAnimator.CheckAndClearLines();

            if (hasClears)
            {
                if (debugMode)
                    Debug.Log("等待消除动画完成");

                // 等待动画完成
                while (lineClearAnimator.IsAnimating)
                    yield return null;
            }
        }
        else
        {
            // 备用：直接使用数据层处理
            var clearedRows = gridManager.ProcessLineClear();
            hasClears = clearedRows.Count > 0;

            if (hasClears)
            {
                if (debugMode)
                    Debug.Log($"数据层消除处理：消除了{clearedRows.Count}行");

                // 添加短暂延迟，让玩家能看到变化
                yield return new WaitForSeconds(0.2f);
            }
        }

        if (debugMode)
            Debug.Log($"消除检查完成：{(hasClears ? "有" : "无")}消除");

        resultCallback(hasClears);
    }

    /// <summary>
    /// 执行新行生成
    /// </summary>
    private IEnumerator ExecuteNewRowGeneration()
    {
        if (debugMode)
            Debug.Log("开始生成新行");

        // 生成新行
        bool success = gridManager.GenerateNewRow();

        if (success)
        {
            if (debugMode)
                Debug.Log("新行生成成功");

            // 使用RowGeneratorAnimator处理动画
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
                    yield return null;

                if (debugMode)
                    Debug.Log("新行生成动画完成");
            }
            else
            {
                // 直接更新所有方块位置
                foreach (var piece in gridManager.GetAllPieces())
                {
                    Vector3 correctPosition = PiecesManager.getPosition(
                        piece.mRow, piece.mCol, gridManager.cellSize, gridManager.startOffset);
                    piece.transform.position = correctPosition;
                }

                // 添加短暂延迟，让玩家能看到变化
                yield return new WaitForSeconds(0.2f);

                //if (debugMode)
                Debug.Log("新行生成：直接更新了方块位置");
            }
        }
        else
        {
            if (debugMode)
                Debug.LogWarning("新行生成失败");
        }
    }
}