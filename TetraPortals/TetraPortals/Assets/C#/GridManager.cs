using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;  // 添加这一行引入LINQ命名空间
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理棋盘网格和方块生成的主要类
/// 负责创建背景格子、生成和跟踪游戏方块、维护棋盘状态
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("棋盘设置")]
    private int[,] grid; // 8x10 的二维数组棋盘，0表示空，1表示被占用
    public int rows = 8; // 棋盘行数
    public int columns = 10; // 棋盘列数

    [SerializeField, Tooltip("棋盘的背景格预制体")]
    public GameObject backgroundGridPrefab;

    [SerializeField, Tooltip("格子的大小（单位）")]
    public float cellSize = 1.0f;

    [SerializeField, Tooltip("棋盘起始位置偏移")]
    public Vector2 startOffset = new Vector2(0, 0);

    [Header("方块生成")]
    [SerializeField, Tooltip("方块预制体")]
    public PiecesManager piecePrefab;

    [SerializeField, Tooltip("初始生成方块的数量范围")]
    public Vector2Int initialPieceCount = new Vector2Int(3, 5);

    [SerializeField, Tooltip("生成方块的间隔时间")]
    public float spawnInterval = 2.0f;

    [SerializeField, Tooltip("是否自动生成方块")]
    public bool autoSpawn = false;
    
    // 调试模式
    [SerializeField] private bool debugMode = true;
    
    

    // 引用缓存
    private List<GameObject> gridCells = new List<GameObject>(); // 存储所有生成的背景格子
    private List<PiecesManager> spawnedPieces = new List<PiecesManager>(); // 存储所有生成的方块
    private float spawnTimer = 0f; // 自动生成方块的计时器

    // 添加锁定状态变量，防止数据不一致
    private bool isLocked = false;

    /// <summary>
    /// 初始化棋盘和开始游戏
    /// </summary>
    void Start()
    {
        InitializeGrid();
        GenerateGridVisual();
        GenerateBottomRowPieces(); // 游戏开局时在底部两行生成方块
        Debug.Log($"棋盘初始化完成，尺寸：{rows}x{columns}");
        
        if (debugMode)
        {
            LogGridState("初始化完成后的棋盘状态");
        }
    }

    /// <summary>
    /// 初始化棋盘的数据结构
    /// </summary>
    private void InitializeGrid()
    {
        // 创建并初始化棋盘数组
        grid = new int[rows, columns];
        
        // 将所有格子初始化为0（空）
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                grid[row, col] = 0;
            }
        }
    }

    /// <summary>
    /// 生成棋盘的可视化背景格子
    /// </summary>
    void GenerateGridVisual()
    {
        // 检查预制体是否设置
        if (backgroundGridPrefab == null)
        {
            Debug.LogError("未设置背景格预制体！请在Inspector中设置。");
            return;
        }

        // 清除现有的格子
        ClearGridVisual();

        // 生成新的格子
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // 计算格子位置
                Vector3 position = PiecesManager.getPosition(row, col, cellSize, startOffset);

                // 实例化格子
                GameObject cell = Instantiate(backgroundGridPrefab, position, Quaternion.identity, transform);
                
                // 设置描述性名称
                cell.name = $"Grid_{row}_{col}";
                
                // 添加到列表中以便后续管理
                gridCells.Add(cell);
            }
        }
    }

    /// <summary>
    /// 清除现有的背景格子
    /// </summary>
    void ClearGridVisual()
    {
        foreach (GameObject cell in gridCells)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        gridCells.Clear();
    }

    /// <summary>
    /// 重新生成棋盘（可在运行时调用）
    /// </summary>
    public void RebuildGrid()
    {
        ClearGridVisual();
        GenerateGridVisual();
    }

    /// <summary>
    /// 获取特定位置的格子值
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>格子的值（0表示空，1表示被占用，-1表示无效位置）</returns>
    public int GetCellValue(int row, int col)
    {
        // 边界检查
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            return grid[row, col];
        }
        return -1; // 表示无效位置
    }

    /// <summary>
    /// 设置特定位置的格子值
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <param name="value">要设置的值（0表示空，1表示被占用）</param>
    public void SetCellValue(int row, int col, int value)
    {
        // 边界检查
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            if (debugMode && grid[row, col] != value)
            {
                Debug.LogWarning($"网格数据更新: ({row}, {col}) 从 {grid[row, col]} 变为 {value}");
            }
            grid[row, col] = value;
        }
        else
        {
            Debug.LogWarning($"尝试设置无效位置的格子值: ({row}, {col})");
        }
    }
    
    /// <summary>
    /// 批量更新多个格子的值
    /// </summary>
    /// <param name="updates">要更新的格子列表，每项包含行、列和新值</param>
    public void BatchUpdateCells(List<(int row, int col, int value)> updates)
    {
        if (updates == null || updates.Count == 0) return;
        
        foreach (var (row, col, value) in updates)
        {
            SetCellValue(row, col, value);
        }
            
        if (debugMode)
        {
            Debug.LogWarning($"批量更新了 {updates.Count} 个格子");
        }
    }
    
    /// <summary>
    /// 检查指定区域是否空闲
    /// </summary>
    /// <param name="row">起始行</param>
    /// <param name="col">起始列</param>
    /// <param name="width">宽度</param>
    /// <returns>如果区域空闲则返回true</returns>
    public bool CheckAreaFree(int row, int col, int width)
    {
        // 检查是否有足够的空间
        for (int j = 0; j < width; j++)
        {
            // 检查超出边界或已被占用
            if (col + j >= columns || grid[row, col + j] != 0)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 在棋盘上随机位置生成一个方块
    /// </summary>
    /// <returns>生成的方块对象，如果生成失败则返回null</returns>
    public PiecesManager SpawnRandomPiece()
    {
        // 检查预制体是否设置
        if (piecePrefab == null)
        {
            Debug.LogError("未设置方块预制体！请在Inspector中设置。");
            return null;
        }
        
        // 随机选择方块类型 (1-4格宽)
        int pieceType = UnityEngine.Random.Range(1, 5);
        int pieceWidth = pieceType; // 方块的宽度
        
        // 只在底部三行生成方块
        int[] bottomRows = { 0, 1, 2 }; // 底部三行的索引
        
        // 尝试找到一个合适的位置
        int maxAttempts = 50; // 最大尝试次数
        for (int i = 0; i < maxAttempts; i++)
        {
            // 随机选择底部三行之一
            int randomRow = bottomRows[UnityEngine.Random.Range(0, bottomRows.Length)];
            // 随机选择列，确保不会超出右边界
            int randomCol = UnityEngine.Random.Range(0, columns - pieceWidth + 1);
            
            // 检查该区域是否可用
            bool isAreaFree = CheckAreaFree(randomRow, randomCol, pieceWidth);
            
            if (isAreaFree)
            {
                // 创建并配置新方块
                PiecesManager piece = CreatePiece(randomRow, randomCol, pieceType, pieceWidth);
                if (piece != null)
                {
                    //Debug.Log($"在底部第{randomRow+1}行生成了{pieceType}格横向方块，位置({randomRow},{randomCol})");
                    return piece;
                }
            }
        }
        
        Debug.Log("无法在底部三行找到合适位置生成方块");
        return null;
    }

    /// <summary>
    /// 创建并配置方块
    /// </summary>
    /// <param name="row">方块所在行</param>
    /// <param name="col">方块所在列</param>
    /// <param name="pieceType">方块类型</param>
    /// <param name="pieceWidth">方块宽度</param>
    /// <returns>创建的方块对象</returns>
    private PiecesManager CreatePiece(int row, int col, int pieceType, int pieceWidth)
    {
        // 计算方块位置
        Vector3 position = PiecesManager.getPosition(row, col, cellSize, startOffset);
        
        // 实例化方块
        PiecesManager pieceManager = Instantiate(piecePrefab, position, Quaternion.identity);
        
        // 设置方块大小与格子匹配
        pieceManager.transform.localScale = new Vector3(
            cellSize / 0.725f,
            cellSize / 0.725f,
            1
        );
        
        // 创建方块单元格
        pieceManager.CreateCells(row, col, pieceType);

        // 在二维数组中标记该区域已被占用
        for (int j = 0; j < pieceWidth; j++)
        {
            grid[row, col + j] = 1; // 使用1表示占用状态
        }
        
        // 存储方块信息
        pieceManager.name = $"Piece_{row}_{col}_{pieceType}";
        spawnedPieces.Add(pieceManager);
        
        return pieceManager;
    }
    
    /// <summary>
    /// 生成指定数量的方块在底部
    /// </summary>
    /// <param name="count">要生成的方块数量</param>
    public void SpawnInitialPieces(int count)
    {
        // 清空现有的所有方块
        ClearAllPieces();
        
        // 生成指定数量的方块
        for (int i = 0; i < count; i++)
        {
            PiecesManager piece = SpawnRandomPiece();
            if (piece == null)
            {
                Debug.Log($"在生成第{i+1}个方块时失败，底部三行可能已满");
                break;
            }
        }
    }
    
    /// <summary>
    /// 清除所有生成的方块并重置棋盘状态
    /// </summary>
    public void ClearAllPieces()
    {
        // 销毁方块物体
        foreach (PiecesManager piece in spawnedPieces)
        {
            if (piece != null)
            {
                Destroy(piece.gameObject);
            }
        }
        spawnedPieces.Clear();
        
        // 重置棋盘状态
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                grid[row, col] = 0; // 0表示空
            }
        }
        
        if (debugMode)
        {
            Debug.Log("已清除所有方块并重置棋盘状态");
        }
    }

    /// <summary>
    /// 在底部生成随机数量的方块
    /// </summary>
    public void GenerateBottomRowPieces()
    {
        // 在底部三行生成随机数量的方块
        int randomCount = UnityEngine.Random.Range(initialPieceCount.x, initialPieceCount.y + 1);
        SpawnInitialPieces(randomCount);
        
        if (debugMode)
        {
            LogGridState("底部方块生成后的棋盘状态");
        }
    }
    
    /// <summary>
    /// 输出当前棋盘状态（调试用）
    /// </summary>
    /// <param name="title">日志标题</param>
    public void LogGridState(string title = "当前棋盘状态")
    {
        if (!debugMode) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<color=yellow>{title}</color>");
            
        // 从上到下打印棋盘
        for (int row = rows - 1; row >= 0; row--)
        {
            sb.Append($"行{row}: ");
            for (int col = 0; col < columns; col++)
            {
                sb.Append(grid[row, col] == 0 ? "□ " : "■ "); // 空格用□，占用格用■
            }
            sb.AppendLine();
        }
            
        Debug.Log(sb.ToString());
    }
    
    /// <summary>
    /// 输出指定区域的棋盘状态（调试用）
    /// </summary>
    /// <param name="startRow">起始行</param>
    /// <param name="startCol">起始列</param>
    /// <param name="width">区域宽度</param>
    /// <param name="height">区域高度</param>
    public void LogAreaState(int startRow, int startCol, int width, int height)
    {
        if (!debugMode) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<color=cyan>区域状态 ({startRow},{startCol}) 宽{width}x高{height}</color>");
            
        // 输出列标题
        sb.Append("    ");
        for (int col = startCol; col < startCol + width && col < columns; col++)
        {
            sb.Append($"{col} ");
        }
        sb.AppendLine();
            
        // 从上到下打印区域
        for (int row = startRow + height - 1; row >= startRow && row >= 0 && row < rows; row--)
        {
            sb.Append($"行{row}: ");
            for (int col = startCol; col < startCol + width && col < columns; col++)
            {
                // 检查是否在有效范围内
                if (col >= 0 && row >= 0 && col < columns && row < rows)
                {
                    sb.Append(grid[row, col] == 0 ? "□ " : "■ ");
                }
                else
                {
                    sb.Append("X ");  // 越界区域用X表示
                }
            }
            sb.AppendLine();
        }
            
        Debug.Log(sb.ToString());
    }
    
    /// <summary>
    /// 根据行列坐标查找方块
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>找到的方块对象，如果没找到则返回null</returns>
    public PiecesManager FindPiece(int row, int col)
    {
        foreach (var piece in spawnedPieces)
        {
            if (row == piece.mRow)
            {
                // 检查列位置是否在方块范围内
                if (col >= piece.mCol && col <= piece.mCol + piece.mCount - 1)
                {
                    return piece;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 检查方块可水平移动的范围
    /// </summary>
    /// <param name="pieceManager">要检查的方块</param>
    /// <returns>可移动的最小列和最大列</returns>
    public KeyValuePair<int,int> CheckPiece(PiecesManager pieceManager)
    {
        int row = pieceManager.mRow;
        int col = pieceManager.mCol;
        int width = pieceManager.mCount;
        int min = col;
        int max = col + width - 1;
        
        // 向左检查可移动空间
        int canmincol = min;
        for (int c = min - 1; c >= 0; c--)
        {
            if (grid[row, c] == 0)
            {
                canmincol = c; // 更新可移动的最左列
            }
            else
            {
                break; // 遇到障碍物停止
            }
        }
        
        // 向右检查可移动空间
        int canmaxcol = max;
        for (int c = max + 1; c < columns; c++)
        {
            if (grid[row, c] == 0)
            {
                canmaxcol = c; // 更新可移动的最右列
            }
            else
            {
                break; // 遇到障碍物停止
            }
        }
        
        // 返回可移动范围
        return new KeyValuePair<int,int>(canmincol, canmaxcol);     
    }
    
    /// <summary>
    /// 将方块移动到新的列位置并更新网格数据
    /// </summary>
    /// <param name="pieceManager">要移动的方块</param>
    /// <param name="col">目标列位置</param>
    public void movePiece(PiecesManager pieceManager, int col)
    {
        // 清除原位置的网格标记
        for (int i = 0; i < pieceManager.mCount; i++)
        {
            grid[pieceManager.mRow, pieceManager.mCol + i] = 0;
        }
        
        // 更新方块的列位置
        pieceManager.mCol = col;
        
        // 在新位置标记网格
        for (int i = 0; i < pieceManager.mCount; i++)
        {
            grid[pieceManager.mRow, pieceManager.mCol + i] = 1;
        }
    }

    /// <summary>
    /// 方块宽度与数据值的映射字典
    /// </summary>
    Dictionary<int,int> countvalues  = new Dictionary<int, int>()
    {
        [1] = 1,    // 宽度1 -> 数据值1 (二进制:0001)
        [2] = 3,    // 宽度2 -> 数据值3 (二进制:0011)
        [3] = 7,    // 宽度3 -> 数据值7 (二进制:0111)
        [4] = 15,   // 宽度4 -> 数据值15 (二进制:1111)
    };
    
    /// <summary>
    /// 数据值与方块宽度的映射字典
    /// </summary>
    Dictionary<int,int> valuecounts  = new Dictionary<int, int>()
    {
        [1] = 1,    // 数据值1 -> 宽度1
        [3] = 2,    // 数据值3 -> 宽度2
        [7] = 3,    // 数据值7 -> 宽度3
        [15] = 4,   // 数据值15 -> 宽度4
    };
    
    /// <summary>
    /// 检查棋盘上所有方块是否可以下落
    /// </summary>
    /// <returns>包含可下落方块及其目标行的字典</returns>
    public Dictionary<PiecesManager,int> CheckDown()
    {
        // 创建返回的结果字典
        Dictionary<PiecesManager,int> downs = new Dictionary<PiecesManager, int>();
        
        // 遍历所有方块
        foreach (var piece in spawnedPieces)
        {
            int row = piece.mRow;       // 当前行
            int col = piece.mCol;       // 左侧列位置
            int colmax = col + piece.mCount - 1;  // 右侧列位置
            int downrow = -1;           // 初始化下落目标行为-1
            
            // 从当前行向下检查空间
            for (int r = row-1; r >= 0; r--)
            {
                bool isall = true;  // 假设当前行的所有需要的格子都是空的
                
                // 检查方块宽度范围内的所有格子
                for (int c = col; c <= colmax; c++)
                {
                    if(grid[r,c] != 0)  // 如果格子被占用
                    {
                        isall = false;  // 标记为不可用
                        break;          // 跳出内层循环
                    }
                }

                if (isall)
                {
                    downrow = r;  // 如果当前行可用，更新目标行
                }
                else
                {
                    break;  // 遇到障碍物，停止向下检查
                }
            }

            // 如果找到可下落的位置，添加到结果字典
            if (downrow != -1)
            {
                downs[piece] = downrow;
            }
        }
        
        // 返回所有可下落的方块及其目标行
        return downs;
    }

    /// <summary>
    /// 处理所有方块的下落逻辑，从底层向上处理
    /// </summary>
    /// <returns>包含下落方块及其目标行的字典</returns>
    public Dictionary<PiecesManager, int> ProcessDrops()
    {
        // 创建结果字典，存储所有下落的方块及其目标行
        Dictionary<PiecesManager, int> results = new Dictionary<PiecesManager, int>();
        
        // 如果已经锁定，则返回空结果
        if (isLocked)
        {
            Debug.LogWarning("网格状态已锁定，无法处理下落");
            return results;
        }
        
        // 锁定网格状态，防止操作过程中被修改
        isLocked = true;
        
        // 按行从底到顶排序方块（升序）
        List<PiecesManager> sortedPieces = new List<PiecesManager>(spawnedPieces);
        sortedPieces.Sort((a, b) => a.mRow.CompareTo(b.mRow));
        
        // 处理每个方块的下落
        foreach (var piece in sortedPieces)
        {
            int row = piece.mRow;
            int col = piece.mCol;
            int width = piece.mCount;
            
            // 方块在最底行或已被处理，则跳过
            if (row <= 0) continue;
            
            // 计算可以下落到的最低行
            int targetRow = row;
            bool canFall = false;
            
            // 从当前行位置向下检查
            for (int r = row - 1; r >= 0; r--)
            {
                bool rowIsFree = true;
                
                // 检查当前行该方块宽度范围内是否都是空的
                for (int c = col; c < col + width; c++)
                {
                    // 如果超出边界或格子非空，则标记为不可用
                    if (c >= columns || grid[r, c] != 0)
                    {
                        rowIsFree = false;
                        break;
                    }
                }
                
                if (rowIsFree)
                {
                    targetRow = r;
                    canFall = true;
                }
                else
                {
                    // 找到障碍，停止向下检查
                    break;
                }
            }
            
            // 如果可以下落，执行下落操作
            if (canFall && targetRow < row)
            {
                // 准备批量更新的格子列表
                List<(int row, int col, int value)> updates = new List<(int row, int col, int value)>();
                
                // 清除原始位置
                for (int c = col; c < col + width; c++)
                {
                    updates.Add((row, c, 0));
                }
                
                // 更新到新位置
                for (int c = col; c < col + width; c++)
                {
                    updates.Add((targetRow, c, 1));
                }
                
                // 批量更新网格
                BatchUpdateCells(updates);
                
                // 更新方块对象的行位置
                piece.mRow = targetRow;
                
                // 记录这次移动
                results[piece] = targetRow;
                
                if (debugMode)
                {
                    Debug.Log($"方块从第{row}行下落到第{targetRow}行");
                }
            }
        }
        
        if (debugMode && results.Count > 0)
        {
            LogGridState($"下落完成后的棋盘状态（{results.Count}个方块下落）");
        }
        
        // 解锁网格状态
        isLocked = false;
        
        return results;
    }

    /// <summary>
    /// 检查并处理行消除
    /// </summary>
    /// <returns>被消除的行索引列表</returns>
    public List<int> ProcessLineClear()
    {
        // 创建结果列表
        List<int> clearedRows = new List<int>();
        
        // 如果已经锁定，则返回空结果
        if (isLocked)
        {
            Debug.LogWarning("网格状态已锁定，无法处理消除");
            return clearedRows;
        }
        
        // 锁定网格状态
        isLocked = true;
        
        // 检查每一行
        for (int row = 0; row < rows; row++)
        {
            bool isRowFull = true;
            
            // 检查当前行的每一列
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col] == 0)
                {
                    isRowFull = false;
                    break;
                }
            }
            
            // 如果满行，执行消除
            if (isRowFull)
            {
                ClearRow(row);
                clearedRows.Add(row);
                
                if (debugMode)
                {
                    Debug.Log($"消除第{row}行");
                }
            }
        }
        
        if (clearedRows.Count > 0 && debugMode)
        {
            LogGridState($"消除{clearedRows.Count}行后的棋盘状态");
        }
        
        // 操作完成，释放锁
        isLocked = false;
        
        return clearedRows;
    }
    
    /// <summary>
    /// 消除单行
    /// </summary>
    private void ClearRow(int row)
    {
        // 存储需要更新的格子
        List<(int row, int col, int value)> updates = new List<(int row, int col, int value)>();
        
        // 存储需要删除的方块
        List<PiecesManager> piecesToRemove = new List<PiecesManager>();
        
        // 清除该行网格数据
        for (int col = 0; col < columns; col++)
        {
            updates.Add((row, col, 0)); // 设置为空
        }
        
        // 查找并处理在该行上的方块
        foreach (var piece in spawnedPieces.ToList())
        {
            if (piece.mRow == row)
            {
                // 方块完全在这一行，标记为移除
                piecesToRemove.Add(piece);
            }
        }
        
        // 批量更新格子
        BatchUpdateCells(updates);
        
        // 移除方块
        foreach (var piece in piecesToRemove)
        {
            if (debugMode)
            {
                Debug.Log($"移除方块: {piece.name}");
            }
            spawnedPieces.Remove(piece);
            Destroy(piece.gameObject);
        }
    }
    
    /// <summary>
    /// 检查是否有可消除的行
    /// </summary>
    /// <returns>满足消除条件的行索引列表</returns>
    public List<int> CheckLineClear()
    {
        List<int> fullRows = new List<int>();
        
        // 检查每一行
        for (int row = 0; row < rows; row++)
        {
            bool isRowFull = true;
            
            // 检查当前行的每一列
            for (int col = 0; col < columns; col++)
            {
                if (grid[row, col] == 0)
                {
                    isRowFull = false;
                    break;
                }
            }
            
            // 如果整行都是1，加入到结果列表
            if (isRowFull)
            {
                fullRows.Add(row);
                if (debugMode)
                {
                    Debug.Log($"检测到第{row}行满足消除条件");
                }
            }
        }
        
        return fullRows;
    }

    /// <summary>
    /// 向上移动所有方块一行，并在底部生成新方块
    /// </summary>
    /// <returns>是否成功执行操作</returns>
    public bool GenerateNewRow()
    {
        // 检查是否满足生成条件
        if (isLocked)
        {
            Debug.LogWarning("网格状态已锁定，无法生成新行");
            return false;
        }
        
        isLocked = true;
        
        try
        {
            // 步骤1: 上移所有方块数据
            List<PiecesManager> movedPieces = ShiftRowsUp();
            
            // 步骤2: 清空底部行
            ClearBottomRow();
            
            // 步骤3: 在底部生成新方块
            List<PiecesManager> newPieces = GenerateBottomRowSingleLine();
            
            // 记录日志
            if (debugMode)
            {
                Debug.Log($"生成新行：上移{movedPieces.Count}个方块，生成{newPieces.Count}个新方块");
                LogGridState("生成新行后的棋盘状态");
            }
            
            return true;
        }
        finally
        {
            isLocked = false;
        }
    }
    
    /// <summary>
    /// 将所有方块向上移动一行
    /// </summary>
    /// <returns>被移动的方块列表</returns>
    private List<PiecesManager> ShiftRowsUp()
    {
        if (debugMode)
        {
            Debug.Log("开始向上移动所有方块");
            LogGridState("移动前的棋盘状态");
        }
        
        // 从上到下处理方块，防止数据覆盖
        List<PiecesManager> sortedPieces = new List<PiecesManager>(spawnedPieces);
        sortedPieces.Sort((a, b) => b.mRow.CompareTo(a.mRow)); // 从高到低排序
        
        // 收集需要删除的方块（超出顶部边界的）
        List<PiecesManager> piecesToRemove = new List<PiecesManager>();
        List<PiecesManager> movedPieces = new List<PiecesManager>();
        List<(int row, int col, int value)> updates = new List<(int row, int col, int value)>();
        
        // 更新网格数据 - 从上到下移动
        for (int row = rows - 1; row > 0; row--)
        {
            for (int col = 0; col < columns; col++)
            {
                // 将当前行的数据移动到上一行
                updates.Add((row, col, grid[row - 1, col]));
            }
        }
        
        // 更新方块位置
        foreach (var piece in sortedPieces)
        {
            int newRow = piece.mRow + 1;
            
            // 检查是否超出顶部边界
            if (newRow >= rows)
            {
                piecesToRemove.Add(piece);
                if (debugMode)
                {
                    Debug.Log($"方块 {piece.name} 因超出顶部边界而被移除");
                }
            }
            else
            {
                // 记录原位置
                int oldRow = piece.mRow;
                
                // 更新方块的行索引
                piece.mRow = newRow;
                movedPieces.Add(piece);
                
                if (debugMode)
                {
                    Debug.Log($"方块 {piece.name} 从第{oldRow}行移动到第{newRow}行");
                }
            }
        }
        
        // 应用所有更新
        BatchUpdateCells(updates);
        
        // 删除超出边界的方块
        foreach (var piece in piecesToRemove)
        {
            spawnedPieces.Remove(piece);
            Destroy(piece.gameObject);
        }
        
        if (debugMode && piecesToRemove.Count > 0)
        {
            Debug.Log($"移除了{piecesToRemove.Count}个超出边界的方块");
        }
        
        return movedPieces;
    }
    
    /// <summary>
    /// 清空底部行
    /// </summary>
    private void ClearBottomRow()
    {
        List<(int row, int col, int value)> updates = new List<(int row, int col, int value)>();
        
        // 清空第0行
        for (int col = 0; col < columns; col++)
        {
            updates.Add((0, col, 0));
        }
        
        BatchUpdateCells(updates);
        
        if (debugMode)
        {
            Debug.Log("底部行已清空");
        }
    }
    
    /// <summary>
    /// 在底部行生成一排方块
    /// </summary>
    /// <returns>生成的方块列表</returns>
    private List<PiecesManager> GenerateBottomRowSingleLine()
    {
        if (debugMode)
        {
            Debug.Log("开始在底部行生成新方块");
        }
        
        List<PiecesManager> newPieces = new List<PiecesManager>();
        int row = 0; // 底部行
        
        // 随机决定填充密度
        float fillDensity = UnityEngine.Random.Range(0.5f, 0.9f); // 50%-90%的填充率
        int targetFillAmount = Mathf.FloorToInt(columns * fillDensity);
        
        // 记录已使用的列
        bool[] usedColumns = new bool[columns];
        int filledAmount = 0;
        
        // 尝试固定次数生成方块
        int maxAttempts = 50; 
        int attempts = 0;
        
        while (filledAmount < targetFillAmount && attempts < maxAttempts)
        {
            attempts++;
            
            // 随机选择方块类型和宽度 (1-4格宽)
            int pieceType = UnityEngine.Random.Range(1, 5);
            int pieceWidth = pieceType;
            
            // 检查是否有足够的空间
            if (pieceWidth > columns - filledAmount)
            {
                // 剩余空间不足，调整方块宽度
                pieceWidth = UnityEngine.Random.Range(1, columns - filledAmount + 1);
                pieceType = pieceWidth;
            }
            
            // 随机选择起始列
            int startCol = UnityEngine.Random.Range(0, columns - pieceWidth + 1);
            
            // 检查该区域是否已被占用
            bool canPlace = true;
            for (int i = 0; i < pieceWidth; i++)
            {
                if (usedColumns[startCol + i])
                {
                    canPlace = false;
                    break;
                }
            }
            
            if (canPlace)
            {
                // 创建方块
                PiecesManager piece = CreatePiece(row, startCol, pieceType, pieceWidth);
                if (piece != null)
                {
                    newPieces.Add(piece);
                    
                    // 标记已使用的列
                    for (int i = 0; i < pieceWidth; i++)
                    {
                        usedColumns[startCol + i] = true;
                    }
                    
                    filledAmount += pieceWidth;
                    
                    if (debugMode)
                    {
                        Debug.Log($"在底部行位置({row},{startCol})生成了{pieceWidth}格宽的方块");
                    }
                }
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"底部行生成完成，创建了{newPieces.Count}个方块，填充了{filledAmount}/{columns}列");
            LogGridState("生成底部行后的棋盘状态");
        }
        
        return newPieces;
    }

    /// <summary>
    /// 获取所有当前生成的方块
    /// </summary>
    /// <returns>所有方块的列表</returns>
    public List<PiecesManager> GetAllPieces()
    {
        return new List<PiecesManager>(spawnedPieces);
    }

    /// <summary>
    /// 获取指定行中的所有方块
    /// </summary>
    /// <param name="row">行索引</param>
    /// <returns>该行中的所有方块列表</returns>
    public List<PiecesManager> GetPiecesInRow(int row)
    {
        List<PiecesManager> piecesInRow = new List<PiecesManager>();
        
        foreach (var piece in spawnedPieces)
        {
            if (piece.mRow == row)
            {
                piecesInRow.Add(piece);
            }
        }
        
        return piecesInRow;
    }
}