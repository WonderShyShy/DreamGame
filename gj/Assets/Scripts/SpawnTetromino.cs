using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{
    public GameObject[] Tetrominoes;
    
    [Header("初始方块设置")]
    [Tooltip("初始生成方块的行数")]
    public int initialRows = 3;
    [Tooltip("每行方块的密度(0-1)")]
    [Range(0f, 1f)]
    public float blockDensity = 0.6f;
    [Tooltip("是否启用初始方块生成")]
    public bool enableInitialBlocks = true;
    
    [Header("随机旋转设置")]
    [Tooltip("是否启用方块生成时的随机旋转")]
    public bool enableRandomRotation = true;
    [Tooltip("随机旋转的概率(0-1)")]
    [Range(0f, 1f)]
    public float randomRotationChance = 0.8f;
    [Tooltip("是否对O形方块应用旋转（视觉上无差异）")]
    public bool rotateOTetromino = false;
    [Tooltip("是否限制I形方块只旋转0°或90°")]
    public bool limitITetrominoRotation = true;

    // Start is called before the first frame update
    void Start()
    {
        if (enableInitialBlocks)
        {
            GenerateInitialBlocks();
        }
        NewTetromino();
    }

    public void NewTetromino()
    {
        // 随机选择一个方块类型
        int tetrominoIndex = Random.Range(0, Tetrominoes.Length);
        GameObject newTetromino = Instantiate(Tetrominoes[tetrominoIndex], transform.position, Quaternion.identity);
        
        // 应用随机旋转
        if (enableRandomRotation && Random.Range(0f, 1f) < randomRotationChance)
        {
            ApplyRandomRotation(newTetromino, tetrominoIndex);
        }
    }
    
    /// <summary>
    /// 为新生成的方块应用随机旋转
    /// </summary>
    void ApplyRandomRotation(GameObject tetromino, int tetrominoIndex)
    {
        TetrisBlock tetrominoScript = tetromino.GetComponent<TetrisBlock>();
        if (tetrominoScript == null)
        {
            Debug.LogWarning("方块缺少TetrisBlock组件，跳过旋转");
            return;
        }
        
        // 获取方块名称来判断类型
        string tetrominoName = Tetrominoes[tetrominoIndex].name;
        int maxRotations = GetMaxRotationsForTetromino(tetrominoName);
        
        if (maxRotations <= 1)
        {
            return; // 不需要旋转或只有一种状态
        }
        
        // 随机选择旋转次数
        int randomRotations = Random.Range(0, maxRotations);
        
        // 应用旋转
        Vector3 rotationPoint = tetrominoScript.rotationPoint;
        int successfulRotations = 0;
        
        for (int i = 0; i < randomRotations; i++)
        {
            // 执行旋转
            tetromino.transform.RotateAround(tetromino.transform.TransformPoint(rotationPoint), Vector3.forward, 90f);
            
            // 验证旋转是否有效
            if (tetrominoScript.ValidMove())
            {
                successfulRotations++;
            }
            else
            {
                // 如果旋转无效，回退并停止进一步旋转
                tetromino.transform.RotateAround(tetromino.transform.TransformPoint(rotationPoint), Vector3.forward, -90f);
                Debug.Log($"方块 {tetrominoName} 在第 {i + 1} 次旋转时遇到阻碍，停止旋转");
                break;
            }
        }
        
        if (successfulRotations > 0)
        {
            Debug.Log($"方块 {tetrominoName} 成功旋转了 {successfulRotations} 次 ({successfulRotations * 90}°)");
        }
    }
    
    /// <summary>
    /// 根据方块类型获取最大旋转次数
    /// </summary>
    int GetMaxRotationsForTetromino(string tetrominoName)
    {
        // 转换为大写进行比较，提高兼容性
        string name = tetrominoName.ToUpper();
        
        if (name.Contains("O") && !rotateOTetromino)
        {
            return 1; // O形方块4个方向看起来一样，除非强制启用
        }
        else if (name.Contains("I") && limitITetrominoRotation)
        {
            return 2; // I形方块只有水平和垂直两种有效状态
        }
        else if (name.Contains("S") || name.Contains("Z"))
        {
            return 2; // S和Z形方块旋转180°后与原状态相同
        }
        else
        {
            return 4; // T、L、J形方块有4个不同的旋转状态
        }
    }
    
    /// <summary>
    /// 生成初始方块，填充底部几行
    /// </summary>
    void GenerateInitialBlocks()
    {
        int width = TetrisBlock.width;
        int height = TetrisBlock.height;
        
        // 获取用于创建单个方块的贴图
        GameObject sampleTetromino = Tetrominoes[0];
        
        for (int row = 0; row < initialRows; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // 根据密度随机决定是否在此位置生成方块
                if (Random.Range(0f, 1f) < blockDensity)
                {
                    CreateSingleBlock(col, row);
                }
            }
            
            // 确保每行至少有一个空位，避免初始时就有满行
            if (IsRowFull(row))
            {
                int randomCol = Random.Range(0, width);
                DestroyBlockAt(randomCol, row);
            }
        }
        
        Debug.Log($"初始方块生成完成：{initialRows}行，密度{blockDensity}");
    }
    
    /// <summary>
    /// 在指定位置创建单个方块
    /// </summary>
    void CreateSingleBlock(int x, int y)
    {
        // 随机选择一个俄罗斯方块预制件来获取样式
        GameObject randomTetromino = Tetrominoes[Random.Range(0, Tetrominoes.Length)];
        
        // 获取第一个子对象作为单个方块的模板
        Transform firstChild = randomTetromino.transform.GetChild(0);
        
        // 创建单个方块
        GameObject singleBlock = new GameObject("InitialBlock");
        singleBlock.transform.position = new Vector3(x, y, 0);
        
        // 复制SpriteRenderer组件
        SpriteRenderer originalRenderer = firstChild.GetComponent<SpriteRenderer>();
        SpriteRenderer newRenderer = singleBlock.AddComponent<SpriteRenderer>();
        
        if (originalRenderer != null)
        {
            newRenderer.sprite = originalRenderer.sprite;
            newRenderer.color = originalRenderer.color;
            newRenderer.sortingOrder = originalRenderer.sortingOrder;
            newRenderer.sortingLayerID = originalRenderer.sortingLayerID;
        }
        
        // 设置缩放
        singleBlock.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        
        // 直接更新网格引用
        TetrisBlock.UpdateGridReference(x, y, singleBlock.transform);
    }
    
    /// <summary>
    /// 检查指定行是否已满
    /// </summary>
    bool IsRowFull(int row)
    {
        int width = TetrisBlock.width;
        for (int x = 0; x < width; x++)
        {
            if (TetrisBlock.GetGridReference(x, row) == null)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 销毁指定位置的方块
    /// </summary>
    void DestroyBlockAt(int x, int y)
    {
        Transform blockTransform = TetrisBlock.GetGridReference(x, y);
        if (blockTransform != null)
        {
            Destroy(blockTransform.gameObject);
            TetrisBlock.UpdateGridReference(x, y, null);
        }
    }
}
