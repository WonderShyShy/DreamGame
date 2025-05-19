using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClearAnimator : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private float clearDuration = 1f; // 消除动画持续时间
    [SerializeField] private int flashCount = 3; // 闪烁次数
    [SerializeField] private Color clearColor = Color.yellow; // 消除高亮颜色

    [Header("调试选项")]
    [SerializeField] private bool debugMode = true; // 是否开启调试日志

    // 引用
    private GridManager gridManager;

    // 状态控制
    private bool isAnimating = false; // 是否正在播放动画
    public bool IsAnimating => isAnimating; // 公开动画状态只读属性

    // 事件
    public event Action<List<int>> OnClearAnimationComplete;

    /// <summary>
    /// 初始化组件，获取必要引用
    /// </summary>
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("场景中没有GridManager组件！");
            enabled = false; // 禁用组件
            return;
        }

        if (debugMode)
        {
            Debug.Log("LineClearAnimator初始化完成，准备处理消除动画");
        }
    }

    /// <summary>
    /// 检查并执行行消除动画
    /// </summary>
    public bool CheckAndClearLines()
    {
        List<int> clearedRows = gridManager.ProcessLineClear();
        bool hasClears = clearedRows.Count > 0;

        if (hasClears)
        {
            if (debugMode)
            {
                Debug.Log($"开始消除动画，共{clearedRows.Count}行");
            }

            StartCoroutine(AnimateLineClear(clearedRows));
        }

        return hasClears;
    }

    /// <summary>
    /// 执行行消除动画
    /// </summary>
    private IEnumerator AnimateLineClear(List<int> clearedRows)
    {
        isAnimating = true;

        // 创建行高亮效果
        List<GameObject> rowHighlights = new List<GameObject>();

        // 为每一个被消除的行创建高亮效果
        foreach (int row in clearedRows)
        {
            // 创建效果对象
            GameObject highlight = new GameObject($"ClearHighlight_Row{row}");
            highlight.transform.SetParent(transform);

            // 计算行的位置和大小
            float rowY = gridManager.startOffset.y + row * gridManager.cellSize;
            float rowWidth = gridManager.columns * gridManager.cellSize;

            // 添加精灵渲染器
            SpriteRenderer renderer = highlight.AddComponent<SpriteRenderer>();

            // 创建白色纹理
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            renderer.sprite = sprite;

            // 设置位置和大小
            highlight.transform.position = new Vector3(
                gridManager.startOffset.x + rowWidth / 2,
                rowY + gridManager.cellSize / 2,
                -1);

            highlight.transform.localScale = new Vector3(rowWidth, gridManager.cellSize, 1);

            // 设置初始透明度为0
            renderer.color = new Color(clearColor.r, clearColor.g, clearColor.b, 0);

            rowHighlights.Add(highlight);
        }

        // 执行闪烁动画
        float elapsed = 0f;
        while (elapsed < clearDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / clearDuration;

            // 根据闪烁次数计算闪烁状态
            float flashValue = Mathf.Sin(progress * Mathf.PI * 2 * flashCount);
            float alpha = Mathf.Abs(flashValue) * clearColor.a;

            // 更新所有高亮的透明度
            foreach (var highlight in rowHighlights)
            {
                SpriteRenderer renderer = highlight.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = new Color(clearColor.r, clearColor.g, clearColor.b, alpha);
                }
            }

            yield return null;
        }

        // 清理高亮效果
        foreach (var highlight in rowHighlights)
        {
            Destroy(highlight);
        }

        isAnimating = false;

        // 触发消除动画完成事件
        OnClearAnimationComplete?.Invoke(clearedRows);
    }
}