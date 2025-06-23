using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
    
    // 鼠标拖动相关变量
    private bool isDragging = false;
    private int selectedRow = -1;
    private Vector3 dragStartPos;
    private Dictionary<Transform, Vector3> originalRowPositions;
    private Camera mainCamera;
    
    /// <summary>
    /// 静态方法：获取网格引用（供其他脚本使用）
    /// </summary>
    public static Transform GetGridReference(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return grid[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// 静态方法：更新网格引用（供其他脚本使用）
    /// </summary>
    public static void UpdateGridReference(int x, int y, Transform blockTransform)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            grid[x, y] = blockTransform;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        originalRowPositions = new Dictionary<Transform, Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        // 鼠标点击检测
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
        
        // 鼠标拖动处理
        if (isDragging && Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }
        
        // 鼠标释放处理
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            HandleMouseRelease();
        }
        
        // 保留旋转功能，改为更直观的键位
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            //rotate !
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }

        // 自动下落逻辑保持不变
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
    }
    
    void HandleMouseClick()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
        int clickedRow = Mathf.RoundToInt(mouseWorldPos.y);
        
        // 检查点击的行是否在有效范围内且有方块
        if (clickedRow >= 0 && clickedRow < height && HasBlocksInRow(clickedRow))
        {
            selectedRow = clickedRow;
            isDragging = true;
            dragStartPos = mouseWorldPos;
            
            // 保存这一行所有方块的原始位置
            SaveRowOriginalPositions(selectedRow);
            
            Debug.Log($"选中第{selectedRow}行进行拖动");
        }
    }
    
    void HandleMouseDrag()
    {
        Vector3 currentMousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
        float dragDistance = currentMousePos.x - dragStartPos.x;
        
        // 移动选中行的所有方块
        MoveRowByDistance(selectedRow, dragDistance);
    }
    
    void HandleMouseRelease()
    {
        if (selectedRow >= 0)
        {
            // 将拖动的行对齐到最近的格子位置
            SnapRowToGrid(selectedRow);
            
            // 检查最终位置是否有效
            if (!IsRowMoveValid(selectedRow))
            {
                // 回退到原始位置
                RevertRowPosition(selectedRow);
                Debug.Log("移动无效，已回退到原始位置");
            }
            else
            {
                Debug.Log($"第{selectedRow}行移动完成");
            }
        }
        
        isDragging = false;
        selectedRow = -1;
        originalRowPositions.Clear();
    }
    
    bool HasBlocksInRow(int row)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] != null)
                return true;
        }
        return false;
    }
    
    void SaveRowOriginalPositions(int row)
    {
        originalRowPositions.Clear();
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] != null)
            {
                originalRowPositions[grid[x, row]] = grid[x, row].position;
            }
        }
    }
    
    void MoveRowByDistance(int row, float distance)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] != null)
            {
                Vector3 originalPos = originalRowPositions[grid[x, row]];
                float newX = originalPos.x + distance;
                
                // 实时显示传送预览效果
                if (newX < -0.5f)
                {
                    // 如果拖动超过左边界一半，开始显示从右边的预览
                    newX = width + newX;
                }
                else if (newX >= width - 0.5f)
                {
                    // 如果拖动超过右边界一半，开始显示从左边的预览
                    newX = newX - width;
                }
                
                grid[x, row].position = new Vector3(newX, originalPos.y, originalPos.z);
            }
        }
    }
    
    void SnapRowToGrid(int row)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] != null)
            {
                Vector3 currentPos = grid[x, row].position;
                float snappedX = Mathf.Round(currentPos.x);
                
                // 实现边界循环传送
                if (snappedX < 0)
                {
                    snappedX = width + snappedX; // 从左边传送到右边
                    Debug.Log($"左边界传送：传送到位置 {snappedX}");
                }
                else if (snappedX >= width)
                {
                    snappedX = snappedX - width; // 从右边传送到左边
                    Debug.Log($"右边界传送：传送到位置 {snappedX}");
                }
                
                grid[x, row].position = new Vector3(snappedX, currentPos.y, currentPos.z);
            }
        }
    }
    
    bool IsRowMoveValid(int row)
    {
        // 临时清空当前行在网格中的引用
        Transform[] tempRow = new Transform[width];
        for (int x = 0; x < width; x++)
        {
            tempRow[x] = grid[x, row];
            grid[x, row] = null;
        }
        
        // 检查新位置是否有效
        bool isValid = true;
        for (int x = 0; x < width; x++)
        {
            if (tempRow[x] != null)
            {
                int newX = Mathf.RoundToInt(tempRow[x].position.x);
                int newY = Mathf.RoundToInt(tempRow[x].position.y);
                
                // 处理边界循环传送
                if (newX < 0)
                {
                    newX = width + newX; // 从左边传送到右边
                }
                else if (newX >= width)
                {
                    newX = newX - width; // 从右边传送到左边
                }
                
                // 检查Y轴边界（上下不循环）
                if (newY < 0 || newY >= height)
                {
                    isValid = false;
                    break;
                }
                
                // 确保传送后的X坐标在有效范围内
                if (newX < 0 || newX >= width)
                {
                    isValid = false;
                    break;
                }
                
                // 检查是否与其他方块重叠
                if (grid[newX, newY] != null)
                {
                    isValid = false;
                    break;
                }
            }
        }
        
        // 如果有效，更新网格；如果无效，恢复原来的网格状态
        if (isValid)
        {
            // 更新网格引用到新位置（考虑传送）
            for (int x = 0; x < width; x++)
            {
                if (tempRow[x] != null)
                {
                    int newX = Mathf.RoundToInt(tempRow[x].position.x);
                    
                    // 处理边界循环传送
                    if (newX < 0)
                    {
                        newX = width + newX;
                    }
                    else if (newX >= width)
                    {
                        newX = newX - width;
                    }
                    
                    grid[newX, row] = tempRow[x];
                }
            }
        }
        else
        {
            // 恢复原来的网格状态
            for (int x = 0; x < width; x++)
            {
                grid[x, row] = tempRow[x];
            }
        }
        
        return isValid;
    }
    
    void RevertRowPosition(int row)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, row] != null && originalRowPositions.ContainsKey(grid[x, row]))
            {
                grid[x, row].position = originalRowPositions[grid[x, row]];
            }
        }
    }

    void CheckForLines()
    {
        for (int i = height-1; i >= 0; i--)
        {
            if(HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for(int j = 0; j< width; j++)
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
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if(grid[j,y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
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

            grid[roundedX, roundedY] = children;
        }
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 ||roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }

        return true;
    }


}
