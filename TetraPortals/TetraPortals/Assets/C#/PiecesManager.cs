using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理单个方块对象的属性和行为 - 覆盖式拉长版本
/// 保持原有多单元格结构，添加拉长贴图覆盖层
/// </summary>
public class PiecesManager : MonoBehaviour
{
    [Header("方块数据")]
    public int mData;       // 方块数据，用二进制位表示方块形状（1=单格，3=两格，7=三格，15=四格）
    public int mCount;      // 方块宽度（占用的格子数量）
    public int mRow;        // 方块所在行
    public int mCol;        // 方块所在列（最左侧格子的列位置）
    
    [Header("渲染组件")]
    public List<SpriteRenderer> mCells = new List<SpriteRenderer>(); // 存储方块的所有单元格的精灵渲染器
    public SpriteRenderer overlayRenderer; // 覆盖层渲染器（显示拉长贴图）
    
    [Header("覆盖层设置")]
    [Tooltip("是否启用拉长贴图覆盖层")]
    public bool useOverlay = true;
    
    [Tooltip("是否隐藏底层的小方块")]
    public bool hideBaseCells = true;

    /// <summary>
    /// 组件初始化
    /// </summary>
    public void Start()
    {
        // 保持原有逻辑
    }

    /// <summary>
    /// 创建方块的所有单元格并配置它们的位置
    /// </summary>
    /// <param name="row">方块所在行</param>
    /// <param name="col">方块所在列</param>
    /// <param name="pieceWidth">方块宽度（1-4）</param>
    public void CreateCells(int row, int col, int pieceWidth)
    {
        mRow = row;
        mCol = col;
        mCount = pieceWidth;
        
        // 根据方块宽度设置数据值
        // 数据值是一个二进制表示，表示方块的形状
        // 例如: 0001(1)=单格, 0011(3)=两格, 0111(7)=三格, 1111(15)=四格
        switch (pieceWidth)
        {
            case 1:
                mData = 1; // 单个格子 (0001 二进制)
                break;
            case 2:
                mData = 3; // 两格横向 (0011 二进制)
                break;
            case 3:
                mData = 7; // 三格横向 (0111 二进制)
                break;
            case 4:
                mData = 15; // 四格横向 (1111 二进制)
                break;
        }
        
        // 获取方块形状中有效格子的位置
        var list = GameUtils.BitFlag.GetBitList(mData);
        var first = mCells[0]; // 使用第一个单元格作为模板
        
        // 创建或重用所有需要的单元格
        for (int i = 0; i < list.Count; i++)
        {
            int cellId = list[i];
            SpriteRenderer cell;
            
            // 如果需要更多单元格，就克隆第一个
            if (i >= mCells.Count)
            {
                cell = GameObject.Instantiate<SpriteRenderer>(first, first.transform.parent);
                mCells.Add(cell);
            }
            else
            {
                cell = mCells[i];
            }
            
            // 获取单元格在方块内的相对位置
            var rc = GetRowColumn(cellId);
            // 设置单元格的位置（在本地坐标系中）
            cell.transform.localPosition = new Vector3(rc.Value * 0.725f, rc.Key * 0.725f);
            
            // 如果设置隐藏底层方块，则隐藏单元格
            if (hideBaseCells && useOverlay)
            {
                cell.color = new Color(1, 1, 1, 0); // 设为透明，保持碰撞但不显示
            }
        }
        
        // 创建并设置覆盖层
        if (useOverlay)
        {
            CreateOverlay();
        }
    }
    
    /// <summary>
    /// 创建拉长贴图覆盖层
    /// </summary>
    private void CreateOverlay()
    {
        // 如果覆盖层不存在，创建一个
        if (overlayRenderer == null)
        {
            GameObject overlayObj = new GameObject("StretchOverlay");
            overlayObj.transform.SetParent(transform);
            overlayRenderer = overlayObj.AddComponent<SpriteRenderer>();
        }
        
        // 应用拉长主题贴图
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.ApplyTheme(overlayRenderer, mCount);
        }
        
        // 设置覆盖层位置和尺寸
        SetupOverlayPosition();
        
        // 设置渲染顺序，确保覆盖层在最上面
        overlayRenderer.sortingOrder = 10;
    }
    
    /// <summary>
    /// 设置覆盖层的位置和尺寸
    /// </summary>
    private void SetupOverlayPosition()
    {
        if (overlayRenderer == null) return;
        
        // 计算覆盖层的中心位置
        float centerOffsetX = (mCount - 1) * 0.725f * 0.5f;
        
        // 设置覆盖层位置（相对于方块中心）
        overlayRenderer.transform.localPosition = new Vector3(centerOffsetX, 0, 0);
        
        // 设置覆盖层尺寸（拉伸到覆盖所有单元格）
        overlayRenderer.transform.localScale = new Vector3(mCount, 1, 1);
        
        // 确保覆盖层在最前面
        overlayRenderer.sortingOrder = 10;
    }

    /// <summary>
    /// 将格子ID转换为行列位置
    /// </summary>
    /// <param name="id">格子ID（0-15）</param>
    /// <returns>格子的行列坐标</returns>
    private KeyValuePair<int, int> GetRowColumn(int id)
    {
        int row = id / 4; // 行号是ID除以4的商
        int col = id % 4; // 列号是ID除以4的余数
        
        return new KeyValuePair<int, int>(row, col);
    }

    /// <summary>
    /// 根据行列索引计算世界坐标位置
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <param name="cellSize">格子大小</param>
    /// <param name="startOffset">网格起始偏移</param>
    /// <returns>世界坐标中的位置</returns>
    public static Vector3 getPosition(int row, int col, float cellSize, Vector2 startOffset)
    {
        // 将行列坐标转换为世界坐标位置
        Vector3 position = new Vector3(
            startOffset.x + col * cellSize, // X坐标根据列和起始偏移计算
            startOffset.y + row * cellSize, // Y坐标根据行和起始偏移计算
            0 // Z坐标为0（2D游戏）
        );
        return position;
    }

    /// <summary>
    /// 根据世界坐标计算行列索引
    /// </summary>
    /// <param name="y">Y坐标（垂直位置）</param>
    /// <param name="x">X坐标（水平位置）</param>
    /// <param name="cellSize">格子大小</param>
    /// <param name="startOffset">网格起始偏移</param>
    /// <returns>对应的行列坐标</returns>
    public static KeyValuePair<int, int> getRowCol(float y, float x, float cellSize, Vector2 startOffset)
    {
        // 考虑到格子中心点偏移，添加半个格子大小的偏移
        float rowf = (y - startOffset.y + cellSize/2) / cellSize;
        float colf = (x - startOffset.x + cellSize/2) / cellSize;
        
        // 转换为整数（向下取整）
        int col = (int)(colf);
        int row = (int)(rowf);
        
        return new KeyValuePair<int, int>(row, col);
    }
    
    /// <summary>
    /// 刷新方块的主题（用于运行时主题切换）
    /// </summary>
    public void RefreshTheme()
    {
        if (useOverlay && overlayRenderer != null && ThemeManager.Instance != null)
        {
            ThemeManager.Instance.ApplyTheme(overlayRenderer, mCount);
        }
    }
    
    /// <summary>
    /// 切换覆盖层显示模式
    /// </summary>
    /// <param name="enabled">是否启用覆盖层</param>
    public void SetOverlayEnabled(bool enabled)
    {
        useOverlay = enabled;
        
        if (overlayRenderer != null)
        {
            overlayRenderer.gameObject.SetActive(enabled);
        }
        
        // 根据覆盖层状态调整底层方块的可见性
        foreach (var cell in mCells)
        {
            if (cell != null)
            {
                cell.color = (enabled && hideBaseCells) ? 
                    new Color(1, 1, 1, 0) : // 透明
                    new Color(1, 1, 1, 1);  // 不透明
            }
        }
    }
    
    /// <summary>
    /// 获取方块信息的字符串描述
    /// </summary>
    /// <returns>方块的详细信息</returns>
    public override string ToString()
    {
        return $"覆盖式方块: 位置({mRow},{mCol}), 宽度{mCount}, 覆盖层{(useOverlay ? "启用" : "禁用")}";
    }
}
