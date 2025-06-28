using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{
    public GameObject[] Tetrominoes;
    public Transform tetrisContainer; 

    void Start()
    {
        // 确保容器存在
        if (tetrisContainer == null)
        {
            tetrisContainer = new GameObject("TetrisContainer").transform;
        }
        NewTetromino();
    }

    public void NewTetromino()
    {
        var creatTransform = new Vector3(Random.Range(1,8), transform.position.y, transform.position.z);
        // 实例化时设置父对象
        GameObject newTetromino = Instantiate(
            Tetrominoes[Random.Range(0, Tetrominoes.Length)], 
            creatTransform, 
            Quaternion.identity,
            tetrisContainer // 设置父对象
        );
    }
}
