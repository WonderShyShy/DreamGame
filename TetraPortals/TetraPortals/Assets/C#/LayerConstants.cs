using UnityEngine;

/// <summary>
/// 游戏层级常量管理
/// 统一定义所有视觉元素的排序层级，确保正确的视觉层次
/// </summary>
public static class LayerConstants
{
    /// <summary>
    /// 🔧 层级配置说明
    /// 
    /// 数值越大，显示层级越高（越靠前）
    /// 建议在各层级之间留出足够的间隔，便于后续调整
    /// </summary>
    
    [Header("== 背景层 (0-9) ==")]
    public const int BACKGROUND = 0;           // 背景元素
    public const int GRID_LINES = 1;           // 网格线
    
    [Header("== 游戏元素层 (10-99) ==")]
    public const int GHOST_EFFECT = 10;        // 残影效果（最底层游戏元素）
    public const int BASE_PIECES = 20;         // 基础方块/单元格
    public const int DRAG_EFFECTS = 30;        // 拖拽特效（列高亮）
    public const int BLOCK_THEMES = 40;        // 方块主题覆盖层
    public const int ANIMATION_EFFECTS = 50;   // 动画特效（下落、消除等）
    
    [Header("== UI层 (100-199) ==")]
    public const int GAME_UI = 100;           // 游戏内UI
    public const int POPUP_UI = 150;          // 弹窗UI
    
    [Header("== 调试层 (200+) ==")]
    public const int DEBUG_OVERLAY = 200;     // 调试覆盖层
    
    /// <summary>
    /// 获取层级信息说明
    /// </summary>
    /// <returns>层级配置的详细说明</returns>
    public static string GetLayerInfo()
    {
        return @"
=== 🎨 TetraPortals 层级配置 ===

📋 层级顺序（从低到高）：
  
  🌄 背景层 (0-9)
    0  - 背景元素
    1  - 网格线
  
  🎮 游戏元素层 (10-99)
    10 - 残影效果（拖拽时的原位置提示）
    20 - 基础方块/单元格
    30 - 拖拽特效（列高亮效果）
    40 - 方块主题覆盖层（彩色长条）
    50 - 动画特效（下落、消除动画）
  
  🖥️ UI层 (100-199)
    100 - 游戏内UI
    150 - 弹窗UI
  
  🔧 调试层 (200+)
    200 - 调试覆盖层

💡 设计原则：
  • 残影在最底层，不遮挡任何游戏元素
  • 拖拽特效在方块下方，提供视觉引导但不干扰
  • 方块主题在最上层，确保清晰可见
  • 预留足够间隔，便于后续扩展

🎯 解决的问题：
  • 拖拽特效不再遮挡方块主题
  • 残影正确显示在所有元素下方
  • 动画效果有适当的显示层级
  • 整体视觉层次清晰有序
        ";
    }
    
    /// <summary>
    /// 验证层级配置是否正确
    /// </summary>
    /// <returns>验证结果说明</returns>
    public static string ValidateLayerSetup()
    {
        var issues = new System.Collections.Generic.List<string>();
        
        // 检查层级顺序
        if (GHOST_EFFECT >= DRAG_EFFECTS)
            issues.Add("残影层级不应高于拖拽特效");
            
        if (DRAG_EFFECTS >= BLOCK_THEMES)
            issues.Add("拖拽特效层级不应高于方块主题");
            
        if (BLOCK_THEMES >= ANIMATION_EFFECTS)
            issues.Add("方块主题层级不应高于动画特效");
        
        if (issues.Count == 0)
            return "✅ 层级配置正确！";
        else
            return "❌ 发现问题：\n" + string.Join("\n", issues);
    }
    
    /// <summary>
    /// 获取推荐的层级调整建议
    /// </summary>
    /// <param name="elementType">元素类型</param>
    /// <returns>推荐的层级值</returns>
    public static int GetRecommendedLayer(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Ghost:
                return GHOST_EFFECT;
            case ElementType.BasePiece:
                return BASE_PIECES;
            case ElementType.DragEffect:
                return DRAG_EFFECTS;
            case ElementType.BlockTheme:
                return BLOCK_THEMES;
            case ElementType.Animation:
                return ANIMATION_EFFECTS;
            case ElementType.UI:
                return GAME_UI;
            case ElementType.Debug:
                return DEBUG_OVERLAY;
            default:
                return BASE_PIECES;
        }
    }
}

/// <summary>
/// 游戏元素类型枚举
/// </summary>
public enum ElementType
{
    Ghost,      // 残影
    BasePiece,  // 基础方块
    DragEffect, // 拖拽特效
    BlockTheme, // 方块主题
    Animation,  // 动画效果
    UI,         // UI元素
    Debug       // 调试元素
} 