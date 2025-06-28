using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
    public GameData Data;
    public float newRowInterval; // 新行生成间隔
    private float newRowTimer = 0f;
    public GameObject[] objs;
    private bool isGameOver = false;
    private bool isAnimating = false; // 新增：标记是否正在播放消行动画
    public GameEvent m_event;
    
    // Start is called before the first frame update
    void Start()
    { 
        Data = GameData.Instance;
        m_event = GameEvent.Instance;
        newRowInterval = Data.newRowInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // 如果正在播放消行动画，暂停所有游戏逻辑
        if (isAnimating) return;
        
        newRowTimer += Time.deltaTime;
        if (newRowTimer >= newRowInterval)
        {
            newRowTimer = 0f;
            StartCoroutine(CreatNewRow());
        }
        if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 恢复原来的玩法：交换当前高度层的方块
            for (int j = 0; j < width-1; j++)
            {
                SwapBlock(grid[j,Data.currentHeight],grid[j+1,Data.currentHeight]);
                (grid[j+1,Data.currentHeight], grid[j,Data.currentHeight ]) = (grid[j,Data.currentHeight], grid[j+1,Data.currentHeight]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 恢复原来的玩法：交换当前高度层的方块
            for (int j = width-1; j >=1; j--)
            {
                SwapBlock(grid[j-1,Data.currentHeight],grid[j,Data.currentHeight]);
                (grid[j-1,Data.currentHeight], grid[j,Data.currentHeight ]) = (grid[j,Data.currentHeight], grid[j-1,Data.currentHeight]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Data.currentHeight < height-1)
            {
                Data.currentHeight++;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            //rotate !
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }
        else if(Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Data.currentHeight > 0)
            {
                Data.currentHeight--;
            }
        }

        if (Time.time - previousTime > (Input.GetKey(KeyCode.LeftShift) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();

                this.enabled = false;
                FindObjectOfType<SpawnTetromino>().NewTetromino();

            }
            previousTime = Time.time;
        }

        GameOver();
    }

    protected virtual void CheckForLines()
    {
        if (isAnimating) return; // 如果正在播放动画，不检查消行
        
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                m_event.clearLine?.Invoke();
                StartCoroutine(DeleteLineWithAnimation(i));
                return;
            }
        }
    }

    protected virtual IEnumerator DeleteLineWithAnimation(int lineIndex)
    {
        isAnimating = true; // 开始动画，暂停其他游戏逻辑
        
        // 收集这一行的所有方块
        Transform[] blocksToDestroy = new Transform[width];
        SpriteRenderer[] spriteRenderers = new SpriteRenderer[width];
        Color[] originalColors = new Color[width];
        Vector3[] originalPositions = new Vector3[width];
        
        for (int j = 0; j < width; j++)
        {
            blocksToDestroy[j] = grid[j, lineIndex];
            if (blocksToDestroy[j] != null)
            {
                spriteRenderers[j] = blocksToDestroy[j].GetComponent<SpriteRenderer>();
                originalPositions[j] = blocksToDestroy[j].position;
                if (spriteRenderers[j] != null)
                {
                    originalColors[j] = spriteRenderers[j].color;
                }
            }
        }
        
        // 动画参数
        float animationDuration = 0.6f; // 稍微延长动画时间
        float elapsedTime = 0f;
        
        // 执行缩小动画
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            // 使用更平滑的缓出曲线
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            float scale = Mathf.Lerp(1f, 0f, easedProgress);
            
            // 旋转效果
            float rotation = progress * 180f; // 减少旋转角度让效果更柔和
            
            // 颜色变化：保持原色但逐渐变淡透明
            float alpha = Mathf.Lerp(1f, 0f, progress);
            
            for (int j = 0; j < width; j++)
            {
                if (blocksToDestroy[j] != null)
                {
                    // 缩放动画
                    blocksToDestroy[j].localScale = Vector3.one * scale;
                    
                    // 旋转动画
                    blocksToDestroy[j].rotation = Quaternion.Euler(0, 0, rotation);
                    
                    // 颜色和透明度动画 - 保持原色，只改变透明度让颜色变淡
                    if (spriteRenderers[j] != null)
                    {
                        Color fadeColor = originalColors[j];
                        fadeColor.a = alpha; // 只改变透明度，保持原始颜色
                        spriteRenderers[j].color = fadeColor;
                    }
                    
                    // 可选：添加轻微的上下浮动效果
                    float floatOffset = Mathf.Sin(progress * Mathf.PI * 2f) * 0.1f;
                    blocksToDestroy[j].position = originalPositions[j] + Vector3.up * floatOffset;
                }
            }
            
            yield return null;
        }
        
        // 动画完成后销毁方块并清理grid
        for (int j = 0; j < width; j++)
        {
            if (blocksToDestroy[j] != null)
            {
                Destroy(blocksToDestroy[j].gameObject);
                grid[j, lineIndex] = null;
            }
        }
        
        // 执行行下移
        RowDown(lineIndex);
        
        isAnimating = false; // 动画完成，恢复游戏逻辑
        
        // 继续检查是否还有其他满行需要消除
        CheckForLines();
    }

    bool HasLine(int i)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, i] == null)
                    return false;
            }

            return true;
        }

        void DeleteLine(int i)
        {
            for (int j = 0; j < width; j++)
            {
                Destroy(grid[j, i].gameObject);
                grid[j, i] = null;
            }
        }

        void RowDown(int i)
        {
            for (int y = i + 1; y < height; y++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (grid[j, y] != null)
                    {
                        grid[j, y - 1] = grid[j, y];
                        grid[j, y] = null;
                        grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                    }
                }
            }
        }

        void RowUP()
        {
            for (int y = height - 1; y > 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    if (grid[x, y - 1] != null)
                    {
                        grid[x, y] = grid[x, y - 1];
                        grid[x, y - 1] = null;
                        if (grid[x, y] != null)
                        {
                            grid[x, y].position += new Vector3(0, 1, 0);
                        }
                    }
                }
            }
        }



        void AddToGrid()
        {
            foreach (Transform children in transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                if (roundedX >= 0 && roundedX < width && roundedY >= 0 && roundedY < height)
                {
                    grid[roundedX, roundedY] = children;
                }
                else
                {
                    Debug.LogWarning($"尝试将方块添加到越界位置: ({roundedX}, {roundedY})");
                }
            }
        }

        bool ValidMove()
        {
            foreach (Transform children in transform)
            {
                int roundedX = Mathf.RoundToInt(children.transform.position.x);
                int roundedY = Mathf.RoundToInt(children.transform.position.y);

                if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
                {
                    return false;
                }

                if (grid[roundedX, roundedY] != null)
                    return false;
            }

            return true;
        }
    
    // 恢复原来的SwapBlock方法 - 这是原游戏玩法的核心
    protected virtual void SwapBlock(Transform block1, Transform block2)
    {
        if (block1 != null && block2 != null)
        {
            (block1.position, block2.position) = (block2.position, block1.position);
        }
        else if(block1==null&&block2!=null)
        {
            block2.position = block2.position - new Vector3(1, 0, 0);
        }
        else if(block1!=null&&block2==null)
        {
            block1.position = block1.position + new Vector3(1, 0, 0);
        }
    }

    protected virtual void GameOver()
    {
        for (int y = height - 2; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    if (!isGameOver)
                    {
                        isGameOver = true;
                        StartCoroutine(LoadGameOverScene());
                    }
                    return; 
                }
            }
        }
    }

    private IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSeconds(1f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    protected virtual IEnumerator CreatNewRow()
    {
        // 如果正在播放动画，跳过新行生成
        if (isAnimating) yield break;
        
        RowUP();
        int blockCount = Random.Range(1, 8);
        int beCreatCount = 0;

        while (beCreatCount < blockCount)
        {
            int transformX = Random.Range(0, width);
            if (grid[transformX, 0] == null)
            {
                int colorIndex = Random.Range(0, objs.Length);
                GameObject newBlock = Instantiate(objs[colorIndex],
                    new Vector3(transformX, 0, 0), Quaternion.identity);
                grid[transformX, 0] = newBlock.transform;
                beCreatCount++;
            }

            yield return null; 
        }
        GameOver();
    }

   public static void ResetGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].gameObject != null)
                    {
                        Destroy(grid[x, y].gameObject);
                    }
                    grid[x, y] = null;
                }
            }
        }
        
        grid = new Transform[width, height];
        
        // 重置所有TetrisBlock实例的动画状态
        TetrisBlock[] tetrisBlocks = FindObjectsOfType<TetrisBlock>();
        foreach (TetrisBlock block in tetrisBlocks)
        {
            block.isAnimating = false;
        }
    }
    
}

