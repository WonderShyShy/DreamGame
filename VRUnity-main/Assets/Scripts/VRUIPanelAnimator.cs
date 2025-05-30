using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VRUIPanelAnimator : MonoBehaviour
{
    [Header("动画设置")]
    public AnimationType animationType = AnimationType.Fade;
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("缩放动画")]
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;
    
    [Header("移动动画")]
    public Vector3 startPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero;
    public bool useLocalPosition = true;
    
    [Header("淡入淡出")]
    public bool animateCanvasGroup = true;
    public CanvasGroup canvasGroup;
    
    [Header("旋转动画")]
    public Vector3 startRotation = Vector3.zero;
    public Vector3 endRotation = Vector3.zero;
    
    [Header("弹性效果")]
    public bool useSpringEffect = false;
    public float springStrength = 10f;
    public float dampening = 5f;
    
    [Header("音效")]
    public AudioClip showSound;
    public AudioClip hideSound;
    public AudioSource audioSource;
    
    public enum AnimationType
    {
        Fade,
        Scale,
        SlideFromTop,
        SlideFromBottom,
        SlideFromLeft,
        SlideFromRight,
        Rotate,
        Custom
    }
    
    private bool isVisible = false;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private Coroutine currentAnimation;
    
    void Awake()
    {
        // 记录原始状态
        originalPosition = useLocalPosition ? transform.localPosition : transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.localEulerAngles;
        
        // 获取或创建CanvasGroup
        if (animateCanvasGroup && canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        // 查找音频源
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }
        
        // 设置预设动画
        SetupPresetAnimation();
        
        // 初始隐藏
        SetInitialHiddenState();
    }
    
    void SetupPresetAnimation()
    {
        switch (animationType)
        {
            case AnimationType.Fade:
                break; // 只使用CanvasGroup的alpha
                
            case AnimationType.Scale:
                startScale = Vector3.zero;
                endScale = originalScale;
                break;
                
            case AnimationType.SlideFromTop:
                startPosition = originalPosition + Vector3.up * 500f;
                endPosition = originalPosition;
                break;
                
            case AnimationType.SlideFromBottom:
                startPosition = originalPosition + Vector3.down * 500f;
                endPosition = originalPosition;
                break;
                
            case AnimationType.SlideFromLeft:
                startPosition = originalPosition + Vector3.left * 500f;
                endPosition = originalPosition;
                break;
                
            case AnimationType.SlideFromRight:
                startPosition = originalPosition + Vector3.right * 500f;
                endPosition = originalPosition;
                break;
                
            case AnimationType.Rotate:
                startRotation = originalRotation + new Vector3(0, 0, 90);
                endRotation = originalRotation;
                break;
        }
    }
    
    void SetInitialHiddenState()
    {
        // 设置隐藏状态
        if (animationType == AnimationType.Scale || animationType == AnimationType.Custom)
        {
            transform.localScale = startScale;
        }
        
        if (animationType == AnimationType.SlideFromTop || 
            animationType == AnimationType.SlideFromBottom ||
            animationType == AnimationType.SlideFromLeft || 
            animationType == AnimationType.SlideFromRight ||
            animationType == AnimationType.Custom)
        {
            if (useLocalPosition)
                transform.localPosition = startPosition;
            else
                transform.position = startPosition;
        }
        
        if (animationType == AnimationType.Rotate || animationType == AnimationType.Custom)
        {
            transform.localEulerAngles = startRotation;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        gameObject.SetActive(false);
    }
    
    public void Show()
    {
        if (isVisible) return;
        
        isVisible = true;
        gameObject.SetActive(true);
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateShow());
        PlayShowSound();
    }
    
    public void Hide()
    {
        if (!isVisible) return;
        
        isVisible = false;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateHide());
        PlayHideSound();
    }
    
    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }
    
    IEnumerator AnimateShow()
    {
        float elapsedTime = 0f;
        
        // 确保CanvasGroup状态正确
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(progress);
            
            // 应用弹性效果
            if (useSpringEffect && progress > 0.7f)
            {
                float springProgress = (progress - 0.7f) / 0.3f;
                float spring = Mathf.Sin(springProgress * Mathf.PI * springStrength) * 
                              Mathf.Exp(-springProgress * dampening);
                curveValue += spring * 0.1f;
            }
            
            ApplyAnimation(curveValue, true);
            yield return null;
        }
        
        // 确保最终状态正确
        ApplyAnimation(1f, true);
        currentAnimation = null;
    }
    
    IEnumerator AnimateHide()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(1f - progress);
            
            ApplyAnimation(curveValue, false);
            yield return null;
        }
        
        // 最终隐藏状态
        ApplyAnimation(0f, false);
        
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        gameObject.SetActive(false);
        currentAnimation = null;
    }
    
    void ApplyAnimation(float progress, bool isShowing)
    {
        // 淡入淡出
        if (canvasGroup != null && (animationType == AnimationType.Fade || animateCanvasGroup))
        {
            canvasGroup.alpha = progress;
        }
        
        // 缩放动画
        if (animationType == AnimationType.Scale || animationType == AnimationType.Custom)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);
        }
        
        // 位置动画
        if (animationType == AnimationType.SlideFromTop || 
            animationType == AnimationType.SlideFromBottom ||
            animationType == AnimationType.SlideFromLeft || 
            animationType == AnimationType.SlideFromRight ||
            animationType == AnimationType.Custom)
        {
            Vector3 targetPosition = Vector3.Lerp(startPosition, endPosition, progress);
            if (useLocalPosition)
                transform.localPosition = targetPosition;
            else
                transform.position = targetPosition;
        }
        
        // 旋转动画
        if (animationType == AnimationType.Rotate || animationType == AnimationType.Custom)
        {
            transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, progress);
        }
    }
    
    void PlayShowSound()
    {
        if (audioSource != null && showSound != null)
        {
            audioSource.PlayOneShot(showSound);
        }
    }
    
    void PlayHideSound()
    {
        if (audioSource != null && hideSound != null)
        {
            audioSource.PlayOneShot(hideSound);
        }
    }
    
    public void SetAnimationType(AnimationType newType)
    {
        animationType = newType;
        SetupPresetAnimation();
    }
    
    public void SetAnimationDuration(float duration)
    {
        animationDuration = duration;
    }
    
    public bool IsVisible()
    {
        return isVisible;
    }
    
    public void ShowImmediate()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        isVisible = true;
        gameObject.SetActive(true);
        ApplyAnimation(1f, true);
        
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    public void HideImmediate()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        isVisible = false;
        ApplyAnimation(0f, false);
        
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        gameObject.SetActive(false);
    }
    
    // 延迟显示/隐藏
    public void ShowWithDelay(float delay)
    {
        StartCoroutine(ShowAfterDelay(delay));
    }
    
    public void HideWithDelay(float delay)
    {
        StartCoroutine(HideAfterDelay(delay));
    }
    
    IEnumerator ShowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Show();
    }
    
    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
} 