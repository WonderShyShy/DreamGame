using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Adaptation : MonoBehaviour
{
    [SerializeField]
    private GameObject topUI;

    private void Awake()
    {
        //暂时用Screen.safeArea直接获取，之后可能改成从json读取，先注释掉
        //SafeArea safe = new SafeArea();
        //safe.x = Screen.safeArea.x;
        //safe.y = Screen.safeArea.y;
        //safe.width = Screen.safeArea.width;
        //safe.height = Screen.safeArea.height;
        //string json = JsonConvert.SerializeObject(safe);
        //SafeArea safeArea = JsonConvert.DeserializeObject<SafeArea>(json);

        Debug.Log("adaptation: " + Screen.safeArea.top);
        Vector3 temVector = new Vector3();
        temVector = topUI.transform.localPosition;
        temVector.y -= Screen.safeArea.top;
        topUI.transform.localPosition = temVector;
    }
}

public class SafeArea
{
    public float x;
    public float y;
    public float width;
    public float height;
}