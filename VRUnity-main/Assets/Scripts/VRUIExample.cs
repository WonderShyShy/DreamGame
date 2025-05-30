using UnityEngine;
using UnityEngine.UI;

public class VRUIExample : MonoBehaviour
{
    [Header("UI组件引用")]
    public VRUIManager uiManager;
    public VRUIPanelAnimator mainPanel;
    public VRUIPanelAnimator settingsPanel;
    
    [Header("示例UI元素")]
    public VRUIButton[] testButtons;
    public VRUISlider[] testSliders;
    public Text statusText;
    
    [Header("测试设置")]
    public KeyCode testKey = KeyCode.Space;
    public bool enableKeyboardTesting = true;
    
    void Start()
    {
        SetupExample();
    }
    
    void SetupExample()
    {
        // 设置状态文本
        if (statusText != null)
        {
            statusText.text = "VR UI 系统已就绪\n按空格键测试UI面板";
        }
        
        // 设置按钮事件
        SetupButtons();
        
        // 设置滑块事件
        SetupSliders();
        
        Debug.Log("VR UI Example 已初始化");
    }
    
    void SetupButtons()
    {
        if (testButtons != null)
        {
            for (int i = 0; i < testButtons.Length; i++)
            {
                int buttonIndex = i; // 避免闭包问题
                if (testButtons[i] != null)
                {
                    Button button = testButtons[i].GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnTestButtonClicked(buttonIndex));
                    }
                }
            }
        }
    }
    
    void SetupSliders()
    {
        if (testSliders != null)
        {
            for (int i = 0; i < testSliders.Length; i++)
            {
                int sliderIndex = i; // 避免闭包问题
                if (testSliders[i] != null)
                {
                    testSliders[i].OnValueChanged.AddListener((value) => OnTestSliderChanged(sliderIndex, value));
                }
            }
        }
    }
    
    void Update()
    {
        // 键盘测试
        if (enableKeyboardTesting)
        {
            HandleKeyboardInput();
        }
    }
    
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(testKey))
        {
            ToggleMainPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowMainPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowSettingsPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAllPanels();
        }
    }
    
    public void ToggleMainPanel()
    {
        if (mainPanel != null)
        {
            mainPanel.Toggle();
            UpdateStatusText($"主面板 {(mainPanel.IsVisible() ? "显示" : "隐藏")}");
        }
    }
    
    public void ShowMainPanel()
    {
        if (mainPanel != null)
        {
            mainPanel.Show();
            UpdateStatusText("显示主面板");
        }
    }
    
    public void ShowSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.Show();
            UpdateStatusText("显示设置面板");
        }
    }
    
    public void HideAllPanels()
    {
        if (mainPanel != null && mainPanel.IsVisible())
        {
            mainPanel.Hide();
        }
        
        if (settingsPanel != null && settingsPanel.IsVisible())
        {
            settingsPanel.Hide();
        }
        
        UpdateStatusText("隐藏所有面板");
    }
    
    void OnTestButtonClicked(int buttonIndex)
    {
        string message = $"按钮 {buttonIndex + 1} 被点击";
        UpdateStatusText(message);
        Debug.Log(message);
        
        // 示例：根据按钮索引执行不同操作
        switch (buttonIndex)
        {
            case 0:
                // 第一个按钮 - 启动问卷
                if (uiManager != null)
                {
                    uiManager.StartQuestionnaire();
                }
                break;
                
            case 1:
                // 第二个按钮 - 显示设置
                ShowSettingsPanel();
                break;
                
            case 2:
                // 第三个按钮 - 返回主菜单
                if (uiManager != null)
                {
                    uiManager.BackToMainMenu();
                }
                break;
                
            default:
                UpdateStatusText($"按钮 {buttonIndex + 1} 功能待实现");
                break;
        }
    }
    
    void OnTestSliderChanged(int sliderIndex, float value)
    {
        string message = $"滑块 {sliderIndex + 1}: {value:F1}";
        UpdateStatusText(message);
        Debug.Log(message);
        
        // 示例：根据滑块值调整某些设置
        switch (sliderIndex)
        {
            case 0:
                // 第一个滑块控制音量
                AudioListener.volume = value / 10f; // 假设滑块范围是0-10
                break;
                
            case 1:
                // 第二个滑块控制UI透明度
                if (mainPanel != null && mainPanel.canvasGroup != null)
                {
                    mainPanel.canvasGroup.alpha = value / 10f;
                }
                break;
                
            default:
                break;
        }
    }
    
    void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = $"状态: {message}\n时间: {System.DateTime.Now:HH:mm:ss}";
        }
    }
    
    // 公开方法供其他脚本调用
    public void SetButtonInteractable(int buttonIndex, bool interactable)
    {
        if (testButtons != null && buttonIndex >= 0 && buttonIndex < testButtons.Length)
        {
            if (testButtons[buttonIndex] != null)
            {
                testButtons[buttonIndex].SetInteractable(interactable);
            }
        }
    }
    
    public void SetSliderValue(int sliderIndex, float value)
    {
        if (testSliders != null && sliderIndex >= 0 && sliderIndex < testSliders.Length)
        {
            if (testSliders[sliderIndex] != null)
            {
                testSliders[sliderIndex].SetValue(value);
            }
        }
    }
    
    public void SetSliderRange(int sliderIndex, float min, float max)
    {
        if (testSliders != null && sliderIndex >= 0 && sliderIndex < testSliders.Length)
        {
            if (testSliders[sliderIndex] != null)
            {
                testSliders[sliderIndex].SetValueRange(min, max);
            }
        }
    }
    
    // 演示方法：创建一个简单的心理健康评估流程
    public void StartMentalHealthAssessment()
    {
        UpdateStatusText("开始心理健康评估");
        
        // 隐藏主面板
        if (mainPanel != null)
        {
            mainPanel.Hide();
        }
        
        // 启动问卷系统
        if (uiManager != null)
        {
            uiManager.StartQuestionnaire();
        }
    }
    
    public void ShowAssessmentResults(int score, string assessment)
    {
        string resultMessage = $"评估完成\n得分: {score}\n评估: {assessment}";
        UpdateStatusText(resultMessage);
        
        // 可以在这里添加更多的结果显示逻辑
        Debug.Log($"心理健康评估结果 - 得分: {score}, 评估: {assessment}");
    }
} 