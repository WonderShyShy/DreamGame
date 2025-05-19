using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理单个方块对象的属性和行为
/// 负责创建、管理和显示方块的各个单元格
/// </summary>
public class PiecesManager : MonoBehaviour
{
    public int mData;       // 方块数据，用二进制位表示方块形状（1=单格，3=两格，7=三格，15=四格）
    public int mCount;      // 方块宽度（占用的格子数量）
    public int mRow;        // 方块所在行
    public int mCol;        // 方块所在列（最左侧格子的列位置）
    public List<SpriteRenderer> mCells = new List<SpriteRenderer>(); // 存储方块的所有单元格的精灵渲染器

    //public void Start()
    //{
    //    CreateCells();
    //}

    /// <summary>
    /// 组件初始化
    /// </summary>
    public void Start()
    {
       //CreateCells();
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
        }
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
}
