using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 层级调试工具
/// 用于检查和显示当前场景中所有渲染器的层级设置
/// </summary>
public class LayerDebugger : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool autoUpdate = true;
    [SerializeField] private float updateInterval = 2f;
    [SerializeField] private bool showInConsole = true;
    [SerializeField] private bool showOnGUI = false;
    
    [Header("过滤设置")]
    [SerializeField] private bool ignoreInactiveObjects = true;
    [SerializeField] private bool groupBySortingOrder = true;
    
    private string debugInfo = "";
    private float lastUpdateTime = 0f;
    
    void Start()
    {
        UpdateLayerInfo();
    }
    
    void Update()
    {
        if (autoUpdate && Time.time - lastUpdateTime > updateInterval)
        {
            UpdateLayerInfo();
            lastUpdateTime = Time.time;
        }
    }
    
    void OnGUI()
    {
        if (showOnGUI && !string.IsNullOrEmpty(debugInfo))
        {
            GUI.Box(new Rect(10, 10, 400, 300), debugInfo);
        }
    }
    
    /// <summary>
    /// 更新层级信息
    /// </summary>
    [ContextMenu("更新层级信息")]
    public void UpdateLayerInfo()
    {
        var allRenderers = FindObjectsOfType<SpriteRenderer>();
        
        if (ignoreInactiveObjects)
        {
            allRenderers = allRenderers.Where(r => r.gameObject.activeInHierarchy).ToArray();
        }
        
        var layerInfo = AnalyzeLayers(allRenderers);
        debugInfo = GenerateDebugReport(layerInfo);
        
        if (showInConsole)
        {
            Debug.Log("=== 🎨 层级调试报告 ===\n" + debugInfo);
        }
    }
    
    /// <summary>
    /// 分析层级信息
    /// </summary>
    private Dictionary<int, List<SpriteRenderer>> AnalyzeLayers(SpriteRenderer[] renderers)
    {
        var layerGroups = new Dictionary<int, List<SpriteRenderer>>();
        
        foreach (var renderer in renderers)
        {
            int sortingOrder = renderer.sortingOrder;
            
            if (!layerGroups.ContainsKey(sortingOrder))
            {
                layerGroups[sortingOrder] = new List<SpriteRenderer>();
            }
            
            layerGroups[sortingOrder].Add(renderer);
        }
        
        return layerGroups;
    }
    
    /// <summary>
    /// 生成调试报告
    /// </summary>
    private string GenerateDebugReport(Dictionary<int, List<SpriteRenderer>> layerInfo)
    {
        var report = new System.Text.StringBuilder();
        
        report.AppendLine("📊 当前场景层级使用情况：\n");
        
        // 按层级排序
        var sortedLayers = layerInfo.Keys.OrderBy(k => k).ToList();
        
        foreach (int layer in sortedLayers)
        {
            var renderers = layerInfo[layer];
            string layerName = GetLayerName(layer);
            
            report.AppendLine($"🔹 层级 {layer} {layerName} ({renderers.Count} 个对象)");
            
            // 显示对象详情
            foreach (var renderer in renderers.Take(5)) // 最多显示5个
            {
                string objName = renderer.gameObject.name;
                if (objName.Length > 20) objName = objName.Substring(0, 17) + "...";
                report.AppendLine($"   • {objName}");
            }
            
            if (renderers.Count > 5)
            {
                report.AppendLine($"   ... 还有 {renderers.Count - 5} 个对象");
            }
            
            report.AppendLine();
        }
        
        // 检查层级冲突
        var conflicts = DetectLayerConflicts(layerInfo);
        if (conflicts.Count > 0)
        {
            report.AppendLine("⚠️ 发现层级冲突：");
            foreach (var conflict in conflicts)
            {
                report.AppendLine($"   • {conflict}");
            }
            report.AppendLine();
        }
        
        // 显示推荐配置
        report.AppendLine("📋 推荐层级配置：");
        report.AppendLine(LayerConstants.GetLayerInfo());
        
        return report.ToString();
    }
    
    /// <summary>
    /// 获取层级名称
    /// </summary>
    private string GetLayerName(int layer)
    {
        if (layer == LayerConstants.GHOST_EFFECT) return "(残影)";
        if (layer == LayerConstants.BASE_PIECES) return "(基础方块)";
        if (layer == LayerConstants.DRAG_EFFECTS) return "(拖拽特效)";
        if (layer == LayerConstants.BLOCK_THEMES) return "(方块主题)";
        if (layer == LayerConstants.ANIMATION_EFFECTS) return "(动画特效)";
        if (layer == LayerConstants.GAME_UI) return "(游戏UI)";
        if (layer == LayerConstants.DEBUG_OVERLAY) return "(调试层)";
        
        if (layer < 10) return "(背景层)";
        if (layer < 100) return "(游戏层)";
        if (layer < 200) return "(UI层)";
        return "(其他)";
    }
    
    /// <summary>
    /// 检测层级冲突
    /// </summary>
    private List<string> DetectLayerConflicts(Dictionary<int, List<SpriteRenderer>> layerInfo)
    {
        var conflicts = new List<string>();
        
        // 检查是否有非推荐层级的使用
        foreach (var kvp in layerInfo)
        {
            int layer = kvp.Key;
            int count = kvp.Value.Count;
            
            // 检查是否使用了推荐的层级值
            bool isRecommended = layer == LayerConstants.GHOST_EFFECT ||
                               layer == LayerConstants.BASE_PIECES ||
                               layer == LayerConstants.DRAG_EFFECTS ||
                               layer == LayerConstants.BLOCK_THEMES ||
                               layer == LayerConstants.ANIMATION_EFFECTS ||
                               layer == LayerConstants.GAME_UI ||
                               layer == LayerConstants.DEBUG_OVERLAY;
            
            if (!isRecommended && count > 1)
            {
                conflicts.Add($"层级 {layer} 有 {count} 个对象，但不在推荐配置中");
            }
        }
        
        // 检查层级密度
        if (layerInfo.ContainsKey(LayerConstants.DRAG_EFFECTS) && 
            layerInfo.ContainsKey(LayerConstants.BLOCK_THEMES))
        {
            int dragCount = layerInfo[LayerConstants.DRAG_EFFECTS].Count;
            int themeCount = layerInfo[LayerConstants.BLOCK_THEMES].Count;
            
            if (dragCount > 0 && themeCount > 0)
            {
                conflicts.Add($"拖拽特效({dragCount})和方块主题({themeCount})同时存在，检查是否正常");
            }
        }
        
        return conflicts;
    }
    
    /// <summary>
    /// 自动修复层级设置
    /// </summary>
    [ContextMenu("自动修复层级")]
    public void AutoFixLayers()
    {
        var allRenderers = FindObjectsOfType<SpriteRenderer>();
        int fixedCount = 0;
        
        foreach (var renderer in allRenderers)
        {
            string objName = renderer.gameObject.name.ToLower();
            int newLayer = -1;
            
            // 根据对象名称推断正确的层级
            if (objName.Contains("ghost"))
                newLayer = LayerConstants.GHOST_EFFECT;
            else if (objName.Contains("drag") || objName.Contains("effect"))
                newLayer = LayerConstants.DRAG_EFFECTS;
            else if (objName.Contains("overlay") || objName.Contains("theme"))
                newLayer = LayerConstants.BLOCK_THEMES;
            else if (objName.Contains("piece") || objName.Contains("cell"))
                newLayer = LayerConstants.BASE_PIECES;
            
            if (newLayer != -1 && renderer.sortingOrder != newLayer)
            {
                renderer.sortingOrder = newLayer;
                fixedCount++;
                Debug.Log($"修复 {renderer.gameObject.name} 的层级: {renderer.sortingOrder} → {newLayer}");
            }
        }
        
        Debug.Log($"自动修复完成，共修复 {fixedCount} 个对象的层级设置");
        UpdateLayerInfo();
    }
} 