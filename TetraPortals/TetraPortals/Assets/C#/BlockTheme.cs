using UnityEngine;

/// <summary>
/// 方块主题配置 - 拉长方块版本
/// 管理不同宽度方块对应的长条纹理资源
/// </summary>
[CreateAssetMenu(fileName = "BlockTheme", menuName = "Game/Block Theme")]
public class BlockTheme : ScriptableObject
{
    [Header("拉长方块贴图 - 按宽度分类")]
    [Tooltip("1格宽方块贴图 (蓝色正方形)")]
    public Sprite oneBlockSprite;   // Blue.png - 1x1 正方形
    
    [Tooltip("2格宽方块贴图 (绿色长条)")]
    public Sprite twoBlockSprite;   // Green.png - 2x1 长条  
    
    [Tooltip("3格宽方块贴图 (红色长条)")]
    public Sprite threeBlockSprite; // Red.png - 3x1 长条
    
    [Tooltip("4格宽方块贴图 (黄色长条)")]
    public Sprite fourBlockSprite;  // Yellow.png - 4x1 长条
    
    [Header("方块尺寸配置")]
    [Tooltip("单格的基础尺寸")]
    public float baseCellSize = 0.725f;
    
    /// <summary>
    /// 根据方块宽度获取对应的长条贴图
    /// </summary>
    /// <param name="width">方块宽度 (1-4)</param>
    /// <returns>对应的Sprite，如果宽度无效则返回1格贴图</returns>
    public Sprite GetSpriteByWidth(int width)
    {
        switch(width)
        {
            case 1: return oneBlockSprite;
            case 2: return twoBlockSprite; 
            case 3: return threeBlockSprite;
            case 4: return fourBlockSprite;
            default: 
                Debug.LogWarning($"无效的方块宽度: {width}，返回默认贴图");
                return oneBlockSprite; // 默认返回蓝色
        }
    }
    
    /// <summary>
    /// 获取方块的显示尺寸
    /// </summary>
    /// <param name="width">方块宽度</param>
    /// <returns>方块的显示尺寸 (宽度, 高度)</returns>
    public Vector2 GetBlockSize(int width)
    {
        return new Vector2(baseCellSize * width, baseCellSize);
    }
    
    /// <summary>
    /// 获取方块中心点偏移
    /// 用于正确定位拉长方块的中心
    /// </summary>
    /// <param name="width">方块宽度</param>
    /// <returns>相对于左上角的中心偏移</returns>
    public Vector2 GetCenterOffset(int width)
    {
        return new Vector2((width - 1) * baseCellSize * 0.5f, 0);
    }
    
    /// <summary>
    /// 检查主题配置是否完整
    /// </summary>
    /// <returns>如果所有贴图都已设置则返回true</returns>
    public bool IsThemeComplete()
    {
        return oneBlockSprite != null && 
               twoBlockSprite != null && 
               threeBlockSprite != null && 
               fourBlockSprite != null;
    }
    
    /// <summary>
    /// 获取缺失的贴图信息
    /// </summary>
    /// <returns>缺失贴图的描述字符串</returns>
    public string GetMissingSprites()
    {
        var missing = new System.Collections.Generic.List<string>();
        
        if (oneBlockSprite == null) missing.Add("1格贴图(蓝色)");
        if (twoBlockSprite == null) missing.Add("2格贴图(绿色)");
        if (threeBlockSprite == null) missing.Add("3格贴图(红色)");
        if (fourBlockSprite == null) missing.Add("4格贴图(黄色)");
        
        return missing.Count > 0 ? string.Join(", ", missing) : "无";
    }
    
    /// <summary>
    /// 获取主题信息摘要
    /// </summary>
    /// <returns>主题的详细信息</returns>
    public string GetThemeInfo()
    {
        return $"拉长方块主题 - 基础尺寸: {baseCellSize}, 配置完整度: {(IsThemeComplete() ? "完整" : "不完整")}";
    }
} 