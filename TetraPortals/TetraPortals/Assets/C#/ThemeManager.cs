using UnityEngine;

/// <summary>
/// 主题管理器 - 负责管理方块主题和颜色应用
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }
    
    [Header("主题配置")]
    [Tooltip("当前使用的方块主题")]
    public BlockTheme currentTheme;
    
    [Header("调试功能")]
    [Tooltip("是否启用调试模式")]
    public bool debugMode = true;
    
    [Tooltip("按此键刷新所有方块主题")]
    public KeyCode refreshKey = KeyCode.R;
    
    void Awake()
    {
        // 单例模式设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // 验证主题配置
        ValidateTheme();
    }
    
    void Update()
    {
        // 调试功能：按R键刷新所有方块主题
        if (debugMode && Input.GetKeyDown(refreshKey))
        {
            RefreshAllBlocks();
        }
    }
    
    /// <summary>
    /// 为SpriteRenderer应用对应宽度的主题贴图
    /// </summary>
    /// <param name="renderer">要应用主题的SpriteRenderer</param>
    /// <param name="blockWidth">方块宽度 (1-4)</param>
    public void ApplyTheme(SpriteRenderer renderer, int blockWidth)
    {
        if (currentTheme == null)
        {
            if (debugMode)
                Debug.LogWarning("ThemeManager: 当前主题为空，无法应用主题");
            return;
        }
        
        if (renderer == null)
        {
            if (debugMode)
                Debug.LogWarning("ThemeManager: SpriteRenderer为空，无法应用主题");
            return;
        }
        
        // 获取对应宽度的贴图
        Sprite targetSprite = currentTheme.GetSpriteByWidth(blockWidth);
        
        if (targetSprite != null)
        {
            renderer.sprite = targetSprite;
            
            if (debugMode)
                Debug.Log($"ThemeManager: 为{blockWidth}格方块应用主题贴图: {targetSprite.name}");
        }
        else
        {
            if (debugMode)
                Debug.LogWarning($"ThemeManager: 无法找到{blockWidth}格方块对应的贴图");
        }
    }
    
    /// <summary>
    /// 批量更新现有所有方块的主题
    /// </summary>
    public void RefreshAllBlocks()
    {
        if (debugMode)
            Debug.Log("ThemeManager: 开始刷新所有方块主题");
        
        var allPieces = FindObjectsOfType<PiecesManager>();
        int refreshedCount = 0;
        
        foreach(var piece in allPieces)
        {
            if (RefreshPieceTheme(piece))
                refreshedCount++;
        }
        
        if (debugMode)
            Debug.Log($"ThemeManager: 刷新完成，共更新了 {refreshedCount} 个方块");
    }
    
    /// <summary>
    /// 刷新单个方块的主题
    /// </summary>
    /// <param name="piece">要刷新的方块</param>
    /// <returns>是否成功刷新</returns>
    private bool RefreshPieceTheme(PiecesManager piece)
    {
        if (piece == null)
            return false;
        
        if (piece.useOverlay && piece.overlayRenderer != null)
        {
            // 刷新覆盖层的主题
            ApplyTheme(piece.overlayRenderer, piece.mCount);
            
            if (debugMode)
                Debug.Log($"刷新覆盖层主题: {piece.ToString()}");
            
            return true;
        }
        else if (piece.mCells != null && piece.mCells.Count > 0)
        {
            // 如果没有覆盖层，刷新底层单元格主题
            foreach(var cell in piece.mCells)
            {
                if (cell != null)
                {
                    ApplyTheme(cell, piece.mCount);
                }
            }
            
            if (debugMode)
                Debug.Log($"刷新底层单元格主题: {piece.ToString()}");
            
            return true;
        }
        else
        {
            if (debugMode)
                Debug.LogWarning($"方块 {piece.name} 没有可用的渲染器组件");
            
            return false;
        }
    }
    
    /// <summary>
    /// 切换到新主题
    /// </summary>
    /// <param name="newTheme">新的主题配置</param>
    public void SwitchTheme(BlockTheme newTheme)
    {
        if (newTheme == null)
        {
            Debug.LogWarning("ThemeManager: 尝试切换到空主题");
            return;
        }
        
        currentTheme = newTheme;
        
        if (debugMode)
            Debug.Log($"ThemeManager: 切换到新主题: {newTheme.name}");
        
        // 立即更新所有现有方块
        RefreshAllBlocks();
    }
    
    /// <summary>
    /// 验证当前主题配置
    /// </summary>
    private void ValidateTheme()
    {
        if (currentTheme == null)
        {
            Debug.LogError("ThemeManager: 未设置主题！请在Inspector中指定BlockTheme资源");
            return;
        }
        
        if (!currentTheme.IsThemeComplete())
        {
            Debug.LogWarning($"ThemeManager: 主题配置不完整，缺失: {currentTheme.GetMissingSprites()}");
        }
        else
        {
            if (debugMode)
                Debug.Log($"ThemeManager: 主题 '{currentTheme.name}' 配置完整，可以正常使用");
        }
    }
    
    /// <summary>
    /// 获取当前主题状态信息
    /// </summary>
    /// <returns>主题状态描述</returns>
    public string GetThemeStatus()
    {
        if (currentTheme == null)
            return "未设置主题";
        
        if (currentTheme.IsThemeComplete())
            return $"主题 '{currentTheme.name}' - 配置完整";
        else
            return $"主题 '{currentTheme.name}' - 缺失: {currentTheme.GetMissingSprites()}";
    }
    
    // *** 编辑器调试功能 ***
    #if UNITY_EDITOR
    [Header("编辑器调试")]
    [Space(10)]
    [UnityEngine.Serialization.FormerlySerializedAs("testButton")]
    [Tooltip("点击此按钮立即刷新所有方块主题（仅编辑器）")]
    public bool forceRefreshInEditor = false;
    
    void OnValidate()
    {
        // 当在编辑器中修改参数时触发
        if (forceRefreshInEditor && Application.isPlaying)
        {
            forceRefreshInEditor = false;
            RefreshAllBlocks();
        }
    }
    #endif
} 