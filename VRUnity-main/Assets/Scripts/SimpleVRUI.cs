using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleVRUI : MonoBehaviour
{
    [Header("UI设置")]
    public Canvas uiCanvas;
    public Transform vrCamera;
    public float canvasDistance = 2f;
    
    [Header("测试按钮")]
    public Button testButton1;
    public Button testButton2;
    public Button testButton3;
    
    [Header("显示文本")]
    public Text statusText;
    
    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    
    private int clickCount = 0;
    
    void Start()
    {
        SetupVRCanvas();
        SetupButtons();
        UpdateStatusText("VR UI 系统已准备就绪，请用手柄点击按钮测试");
    }
    
    void SetupVRCanvas()
    {
        if (uiCanvas == null)
        {
            Debug.LogError("请拖入Canvas到UI Canvas字段！");
            return;
        }
        
        // 配置Canvas为World Space
        uiCanvas.renderMode = RenderMode.WorldSpace;
        
        // 设置Canvas位置
        if (vrCamera != null)
        {
            Vector3 canvasPosition = vrCamera.position + vrCamera.forward * canvasDistance;
            canvasPosition.y = vrCamera.position.y;
            uiCanvas.transform.position = canvasPosition;
            uiCanvas.transform.LookAt(vrCamera);
            uiCanvas.transform.Rotate(0, 180, 0);
            
            // 设置适合VR的尺寸
            uiCanvas.transform.localScale = Vector3.one * 0.001f;
            
            // 设置摄像机引用
            uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
        }
        
        // 确保有必要的组件
        if (uiCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            uiCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }
        
        // 检查EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("自动创建了EventSystem");
        }
        
        Debug.Log("VR Canvas配置完成");
    }
    
    void SetupButtons()
    {
        // 设置按钮点击事件
        if (testButton1 != null)
        {
            testButton1.onClick.AddListener(() => OnButtonClicked("按钮1"));
        }
        
        if (testButton2 != null)
        {
            testButton2.onClick.AddListener(() => OnButtonClicked("按钮2"));
        }
        
        if (testButton3 != null)
        {
            testButton3.onClick.AddListener(() => OnButtonClicked("按钮3"));
        }
        
        Debug.Log("按钮事件配置完成");
    }
    
    public void OnButtonClicked(string buttonName)
    {
        clickCount++;
        
        // 播放音效
        PlayClickSound();
        
        // 更新状态文本
        UpdateStatusText($"您点击了{buttonName}！\n总点击次数: {clickCount}");
        
        // 控制台输出
        Debug.Log($"VR手柄点击了: {buttonName}, 总次数: {clickCount}");
        
        // 简单的视觉反馈
        StartCoroutine(ButtonFeedback());
    }
    
    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    
    void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
    
    System.Collections.IEnumerator ButtonFeedback()
    {
        // 简单的颜色闪烁反馈
        if (statusText != null)
        {
            Color originalColor = statusText.color;
            statusText.color = Color.green;
            yield return new WaitForSeconds(0.3f);
            statusText.color = originalColor;
        }
    }
    
    void Update()
    {
        // 让UI始终朝向用户（可选）
        if (uiCanvas != null && vrCamera != null)
        {
            Vector3 direction = vrCamera.position - uiCanvas.transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                uiCanvas.transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
        
        // 键盘测试（开发时使用）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnButtonClicked("按钮1（键盘测试）");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnButtonClicked("按钮2（键盘测试）");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnButtonClicked("按钮3（键盘测试）");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTest();
        }
    }
    
    public void ResetTest()
    {
        clickCount = 0;
        UpdateStatusText("测试已重置，请继续用手柄点击按钮");
        Debug.Log("VR UI测试已重置");
    }
    
    // 在Inspector中显示帮助信息
    [ContextMenu("创建简单VR UI")]
    void CreateSimpleUI()
    {
        Debug.Log("请按照以下步骤手动创建UI：");
        Debug.Log("1. 创建Canvas（World Space）");
        Debug.Log("2. 在Canvas下创建3个Button");
        Debug.Log("3. 创建一个Text作为状态显示");
        Debug.Log("4. 将组件拖入此脚本的对应字段");
        Debug.Log("5. 确保有VR摄像机引用");
    }
} 