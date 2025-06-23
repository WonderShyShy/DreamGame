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
        Instantiate(Tetrominoes[Random.Range(0, Tetrominoes.Length)], transform.position, Quaternion.identity);
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
