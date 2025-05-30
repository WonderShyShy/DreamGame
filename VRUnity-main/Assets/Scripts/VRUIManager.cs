using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRUIManager : MonoBehaviour
{
    [Header("UI Canvas设置")]
    public Canvas uiCanvas;
    public float canvasDistance = 2f;
    public Transform vrCamera;
    
    [Header("UI面板")]
    public GameObject mainMenuPanel;
    public GameObject questionPanel;
    public GameObject resultPanel;
    
    [Header("问题系统")]
    public Text questionText;
    public Button[] answerButtons;
    public Text progressText;
    public Slider progressSlider;
    
    [Header("结果显示")]
    public Text resultText;
    public Image resultIcon;
    
    [Header("交互设置")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    
    // 问卷系统变量
    private int currentQuestionIndex = 0;
    private List<QuestionData> questions = new List<QuestionData>();
    private List<int> userAnswers = new List<int>();
    
    [System.Serializable]
    public class QuestionData
    {
        public string questionText;
        public string[] answers;
        public int[] scores; // 每个答案对应的分数
    }
    
    void Start()
    {
        SetupVRUI();
        InitializeQuestions();
        ShowMainMenu();
    }
    
    void SetupVRUI()
    {
        // 设置Canvas为World Space
        if (uiCanvas != null)
        {
            uiCanvas.renderMode = RenderMode.WorldSpace;
            
            if (vrCamera != null)
            {
                uiCanvas.worldCamera = vrCamera.GetComponent<Camera>();
                
                // 位置UI在用户前方
                Vector3 canvasPosition = vrCamera.position + vrCamera.forward * canvasDistance;
                canvasPosition.y = vrCamera.position.y; // 保持相同高度
                uiCanvas.transform.position = canvasPosition;
                uiCanvas.transform.LookAt(vrCamera);
                uiCanvas.transform.Rotate(0, 180, 0); // 翻转面向用户
                
                // 设置适合VR的Canvas缩放
                uiCanvas.transform.localScale = Vector3.one * 0.001f; // 适合VR的尺寸
            }
        }
        
        // 设置按钮点击事件
        SetupButtons();
    }
    
    void SetupButtons()
    {
        // 为答案按钮添加点击事件
        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int buttonIndex = i; // 避免闭包问题
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
            }
        }
    }
    
    void InitializeQuestions()
    {
        // 示例问题数据 - 您可以根据心理健康评估需求修改
        questions.Add(new QuestionData
        {
            questionText = "在过去的一周里，您感到心情如何？",
            answers = new string[] { "非常好", "较好", "一般", "较差", "非常差" },
            scores = new int[] { 5, 4, 3, 2, 1 }
        });
        
        questions.Add(new QuestionData
        {
            questionText = "您最近的睡眠质量如何？",
            answers = new string[] { "很好", "较好", "一般", "较差", "很差" },
            scores = new int[] { 5, 4, 3, 2, 1 }
        });
        
        questions.Add(new QuestionData
        {
            questionText = "您对当前生活的满意度如何？",
            answers = new string[] { "非常满意", "比较满意", "一般", "不太满意", "很不满意" },
            scores = new int[] { 5, 4, 3, 2, 1 }
        });
        
        questions.Add(new QuestionData
        {
            questionText = "您最近是否经常感到焦虑？",
            answers = new string[] { "从不", "偶尔", "有时", "经常", "总是" },
            scores = new int[] { 5, 4, 3, 2, 1 }
        });
        
        questions.Add(new QuestionData
        {
            questionText = "您对未来的看法如何？",
            answers = new string[] { "非常乐观", "比较乐观", "一般", "比较悲观", "很悲观" },
            scores = new int[] { 5, 4, 3, 2, 1 }
        });
    }
    
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (questionPanel != null) questionPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }
    
    public void StartQuestionnaire()
    {
        PlayButtonSound();
        currentQuestionIndex = 0;
        userAnswers.Clear();
        
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (questionPanel != null) questionPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);
        
        DisplayCurrentQuestion();
    }
    
    void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            QuestionData currentQuestion = questions[currentQuestionIndex];
            
            // 更新问题文本
            if (questionText != null)
            {
                questionText.text = currentQuestion.questionText;
            }
            
            // 更新进度
            if (progressText != null)
            {
                progressText.text = $"问题 {currentQuestionIndex + 1} / {questions.Count}";
            }
            
            if (progressSlider != null)
            {
                progressSlider.value = (float)(currentQuestionIndex) / questions.Count;
            }
            
            // 更新答案按钮
            if (answerButtons != null)
            {
                for (int i = 0; i < answerButtons.Length; i++)
                {
                    if (i < currentQuestion.answers.Length)
                    {
                        answerButtons[i].gameObject.SetActive(true);
                        Text buttonText = answerButtons[i].GetComponentInChildren<Text>();
                        if (buttonText != null)
                        {
                            buttonText.text = currentQuestion.answers[i];
                        }
                    }
                    else
                    {
                        answerButtons[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    public void OnAnswerSelected(int answerIndex)
    {
        PlayButtonSound();
        
        if (currentQuestionIndex < questions.Count)
        {
            // 记录用户选择
            userAnswers.Add(answerIndex);
            
            // 移到下一题或显示结果
            currentQuestionIndex++;
            
            if (currentQuestionIndex < questions.Count)
            {
                DisplayCurrentQuestion();
            }
            else
            {
                ShowResults();
            }
        }
    }
    
    void ShowResults()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);
        
        // 计算总分
        int totalScore = 0;
        for (int i = 0; i < userAnswers.Count; i++)
        {
            totalScore += questions[i].scores[userAnswers[i]];
        }
        
        // 生成评估结果
        string assessment = GenerateAssessment(totalScore);
        
        if (resultText != null)
        {
            resultText.text = $"您的评估结果：\n\n{assessment}\n\n总分：{totalScore}/{questions.Count * 5}";
        }
        
        // 更新结果图标颜色
        UpdateResultIcon(totalScore);
    }
    
    string GenerateAssessment(int score)
    {
        float percentage = (float)score / (questions.Count * 5) * 100;
        
        if (percentage >= 80)
        {
            return "您的心理健康状态良好！\n继续保持积极的生活态度。";
        }
        else if (percentage >= 60)
        {
            return "您的心理健康状态基本正常。\n建议适当关注自己的情绪变化。";
        }
        else if (percentage >= 40)
        {
            return "您可能存在一些心理压力。\n建议寻求专业帮助或尝试放松技巧。";
        }
        else
        {
            return "建议您重视心理健康状况。\n强烈建议咨询专业心理健康医生。";
        }
    }
    
    void UpdateResultIcon(int score)
    {
        if (resultIcon != null)
        {
            float percentage = (float)score / (questions.Count * 5);
            
            if (percentage >= 0.8f)
            {
                resultIcon.color = Color.green;
            }
            else if (percentage >= 0.6f)
            {
                resultIcon.color = Color.yellow;
            }
            else if (percentage >= 0.4f)
            {
                resultIcon.color = new Color(1f, 0.5f, 0f, 1f); // 橙色 (255, 128, 0)
            }
            else
            {
                resultIcon.color = Color.red;
            }
        }
    }
    
    public void RestartQuestionnaire()
    {
        PlayButtonSound();
        StartQuestionnaire();
    }
    
    public void BackToMainMenu()
    {
        PlayButtonSound();
        ShowMainMenu();
    }
    
    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    void Update()
    {
        // 保持UI朝向用户（可选）
        if (uiCanvas != null && vrCamera != null)
        {
            Vector3 direction = vrCamera.position - uiCanvas.transform.position;
            direction.y = 0; // 只在水平面旋转
            if (direction != Vector3.zero)
            {
                uiCanvas.transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }
} 