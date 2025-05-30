using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("视觉效果设置")]
    public float hoverScale = 1.1f;
    public float pressScale = 0.95f;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    public Color pressColor = Color.yellow;
    public Color disabledColor = Color.gray;
    
    [Header("动画设置")]
    public float animationDuration = 0.2f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("音效设置")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;
    
    [Header("触觉反馈")]
    public bool enableHapticFeedback = true;
    public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;
    
    [Header("发光效果")]
    public bool enableGlowEffect = true;
    public Material glowMaterial;
    public float glowIntensity = 2f;
    
    private Button button;
    private Image buttonImage;
    private Text buttonText;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isHovered = false;
    private bool isPressed = false;
    private bool isInteractable = true;
    
    // 发光效果组件
    private GameObject glowObject;
    private Image glowImage;
    
    void Start()
    {
        // 获取组件引用
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();
        
        // 如果没有AudioSource，尝试找到一个
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }
        
        // 记录原始状态
        originalScale = transform.localScale;
        originalColor = buttonImage ? buttonImage.color : Color.white;
        
        // 创建发光效果
        if (enableGlowEffect)
        {
            CreateGlowEffect();
        }
        
        // 监听按钮状态变化
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    void CreateGlowEffect()
    {
        // 创建发光对象
        glowObject = new GameObject("ButtonGlow");
        glowObject.transform.SetParent(transform, false);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localScale = Vector3.one * 1.2f;
        
        // 添加Image组件
        glowImage = glowObject.AddComponent<Image>();
        glowImage.sprite = buttonImage ? buttonImage.sprite : null;
        glowImage.material = glowMaterial;
        glowImage.color = new Color(hoverColor.r, hoverColor.g, hoverColor.b, 0f);
        
        // 确保发光在按钮后面
        glowObject.transform.SetSiblingIndex(0);
        glowObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        isHovered = true;
        PlayHoverSound();
        StartCoroutine(AnimateToHover());
        
        // 启用发光效果
        if (enableGlowEffect && glowObject != null)
        {
            glowObject.SetActive(true);
            StartCoroutine(AnimateGlow(true));
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        isHovered = false;
        if (!isPressed)
        {
            StartCoroutine(AnimateToNormal());
        }
        
        // 禁用发光效果
        if (enableGlowEffect && glowObject != null)
        {
            StartCoroutine(AnimateGlow(false));
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        isPressed = true;
        StartCoroutine(AnimateToPressed());
        
        // 触觉反馈
        if (enableHapticFeedback)
        {
            TriggerHapticFeedback();
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        isPressed = false;
        if (isHovered)
        {
            StartCoroutine(AnimateToHover());
        }
        else
        {
            StartCoroutine(AnimateToNormal());
        }
    }
    
    void OnButtonClick()
    {
        PlayClickSound();
    }
    
    IEnumerator AnimateToHover()
    {
        yield return StartCoroutine(AnimateScale(originalScale * hoverScale));
        yield return StartCoroutine(AnimateColor(hoverColor));
    }
    
    IEnumerator AnimateToPressed()
    {
        yield return StartCoroutine(AnimateScale(originalScale * pressScale));
        yield return StartCoroutine(AnimateColor(pressColor));
    }
    
    IEnumerator AnimateToNormal()
    {
        yield return StartCoroutine(AnimateScale(originalScale));
        yield return StartCoroutine(AnimateColor(normalColor));
    }
    
    IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(progress);
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    IEnumerator AnimateColor(Color targetColor)
    {
        if (buttonImage == null) yield break;
        
        Color startColor = buttonImage.color;
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            buttonImage.color = Color.Lerp(startColor, targetColor, progress);
            yield return null;
        }
        
        buttonImage.color = targetColor;
    }
    
    IEnumerator AnimateGlow(bool fadeIn)
    {
        if (glowImage == null) yield break;
        
        Color startColor = glowImage.color;
        Color targetColor = fadeIn ? 
            new Color(hoverColor.r, hoverColor.g, hoverColor.b, 0.5f) : 
            new Color(hoverColor.r, hoverColor.g, hoverColor.b, 0f);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            glowImage.color = Color.Lerp(startColor, targetColor, progress);
            yield return null;
        }
        
        glowImage.color = targetColor;
        
        if (!fadeIn)
        {
            glowObject.SetActive(false);
        }
    }
    
    void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, 0.5f);
        }
    }
    
    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    
    void TriggerHapticFeedback()
    {
        // 基础触觉反馈实现 - 可以在安装XR Toolkit后扩展
        Debug.Log("Haptic feedback triggered!");
    }
    
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        
        if (button != null)
        {
            button.interactable = interactable;
        }
        
        if (buttonImage != null)
        {
            buttonImage.color = interactable ? normalColor : disabledColor;
        }
        
        // 禁用时隐藏发光效果
        if (!interactable && glowObject != null)
        {
            glowObject.SetActive(false);
        }
    }
    
    public void SetColors(Color normal, Color hover, Color press)
    {
        normalColor = normal;
        hoverColor = hover;
        pressColor = press;
        originalColor = normal;
        
        if (buttonImage != null && !isHovered && !isPressed)
        {
            buttonImage.color = normalColor;
        }
    }
    
    void OnDestroy()
    {
        if (glowObject != null)
        {
            DestroyImmediate(glowObject);
        }
    }
} 