using UnityEngine;

/// <summary>
/// 方块主题配置
/// 管理不同宽度方块对应的贴图资源
/// </summary>
[CreateAssetMenu(fileName = "BlockTheme", menuName = "Game/Block Theme")]
public class BlockTheme : ScriptableObject
{
    [Header("方块贴图 - 按宽度分类")]
    [Tooltip("1格宽方块贴图 (蓝色)")]
    public Sprite oneBlockSprite;   // Blue.png - 1格
    
    [Tooltip("2格宽方块贴图 (绿色)")]
    public Sprite twoBlockSprite;   // Green.png - 2格  
    
    [Tooltip("3格宽方块贴图 (红色)")]
    public Sprite threeBlockSprite; // Red.png - 3格
    
    [Tooltip("4格宽方块贴图 (黄色)")]
    public Sprite fourBlockSprite;  // Yellow.png - 4格
    
    /// <summary>
    /// 根据方块宽度获取对应的贴图
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
} 