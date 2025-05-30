using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRUISlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("滑块设置")]
    public Slider slider;
    public float sensitivity = 1f;
    public bool snapToIntegers = true;
    public bool showValueText = true;
    
    [Header("视觉反馈")]
    public Transform handle;
    public float handleHoverScale = 1.2f;
    public Color normalColor = Color.white;
    public Color activeColor = Color.cyan;
    public Color fillColor = Color.green;
    
    [Header("数值显示")]
    public Text valueText;
    public string valueFormat = "F0";
    public string valuePrefix = "";
    public string valueSuffix = "";
    
    [Header("分段设置")]
    public bool useSegments = false;
    public int segmentCount = 5;
    public Transform segmentContainer;
    public GameObject segmentPrefab;
    
    [Header("音效")]
    public AudioClip valueChangeSound;
    public AudioClip snapSound;
    public AudioSource audioSource;
    
    [Header("事件")]
    public UnityEvent<float> OnValueChanged = new UnityEvent<float>();
    public UnityEvent<int> OnIntegerValueChanged = new UnityEvent<int>();
    
    private bool isDragging = false;
    private Vector3 originalHandleScale;
    private Image handleImage;
    private Image fillImage;
    private float lastValue;
    private GameObject[] segmentObjects;
    
    void Start()
    {
        InitializeSlider();
        SetupVisuals();
        CreateSegments();
        UpdateDisplay();
    }
    
    void InitializeSlider()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            lastValue = slider.value;
        }
        
        if (handle != null)
        {
            originalHandleScale = handle.localScale;
            handleImage = handle.GetComponent<Image>();
        }
        
        if (slider != null && slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
        }
        
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }
    }
    
    void SetupVisuals()
    {
        if (handleImage != null)
        {
            handleImage.color = normalColor;
        }
        
        if (fillImage != null)
        {
            fillImage.color = fillColor;
        }
    }
    
    void CreateSegments()
    {
        if (!useSegments || segmentContainer == null || segmentPrefab == null)
            return;
        
        // 清理现有分段
        if (segmentObjects != null)
        {
            foreach (var segment in segmentObjects)
            {
                if (segment != null)
                    DestroyImmediate(segment);
            }
        }
        
        segmentObjects = new GameObject[segmentCount];
        
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, segmentContainer);
            segmentObjects[i] = segment;
            
            // 设置分段位置
            RectTransform rectTransform = segment.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float normalizedPosition = (float)i / (segmentCount - 1);
                rectTransform.anchorMin = new Vector2(normalizedPosition, 0.5f);
                rectTransform.anchorMax = new Vector2(normalizedPosition, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
            
            // 设置分段标签
            Text segmentText = segment.GetComponentInChildren<Text>();
            if (segmentText != null && slider != null)
            {
                float segmentValue = Mathf.Lerp(slider.minValue, slider.maxValue, (float)i / (segmentCount - 1));
                segmentText.text = segmentValue.ToString(valueFormat);
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        AnimateHandle(true);
        PlayValueChangeSound();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        AnimateHandle(false);
        
        // 如果启用整数捕捉
        if (snapToIntegers && slider != null)
        {
            float roundedValue = Mathf.Round(slider.value);
            if (Mathf.Abs(slider.value - roundedValue) > 0.01f)
            {
                slider.value = roundedValue;
                PlaySnapSound();
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && slider != null)
        {
            // 基于拖拽计算新值
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                slider.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out localPoint);
            
            // 计算滑块的新值
            Rect rect = slider.GetComponent<RectTransform>().rect;
            float normalizedValue = Mathf.Clamp01((localPoint.x + rect.width * 0.5f) / rect.width);
            float newValue = Mathf.Lerp(slider.minValue, slider.maxValue, normalizedValue);
            
            slider.value = newValue;
        }
    }
    
    void OnSliderValueChanged(float value)
    {
        // 检查值是否真的改变了
        if (Mathf.Abs(value - lastValue) > 0.01f)
        {
            PlayValueChangeSound();
            lastValue = value;
        }
        
        UpdateDisplay();
        UpdateSegmentHighlight();
        
        // 触发事件
        OnValueChanged.Invoke(value);
        
        if (snapToIntegers)
        {
            OnIntegerValueChanged.Invoke(Mathf.RoundToInt(value));
        }
    }
    
    void UpdateDisplay()
    {
        if (showValueText && valueText != null && slider != null)
        {
            string formattedValue = slider.value.ToString(valueFormat);
            valueText.text = valuePrefix + formattedValue + valueSuffix;
        }
        
        // 更新填充颜色透明度
        if (fillImage != null && slider != null)
        {
            float alpha = Mathf.Lerp(0.3f, 1f, slider.normalizedValue);
            Color currentColor = fillImage.color;
            fillImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }
    
    void UpdateSegmentHighlight()
    {
        if (!useSegments || segmentObjects == null || slider == null)
            return;
        
        int activeSegment = Mathf.RoundToInt(slider.normalizedValue * (segmentCount - 1));
        
        for (int i = 0; i < segmentObjects.Length; i++)
        {
            if (segmentObjects[i] != null)
            {
                Image segmentImage = segmentObjects[i].GetComponent<Image>();
                if (segmentImage != null)
                {
                    segmentImage.color = i <= activeSegment ? activeColor : normalColor;
                }
                
                // 缩放效果
                float scale = i == activeSegment ? 1.2f : 1f;
                segmentObjects[i].transform.localScale = Vector3.one * scale;
            }
        }
    }
    
    void AnimateHandle(bool isActive)
    {
        if (handle != null)
        {
            StartCoroutine(AnimateHandleScale(isActive));
            StartCoroutine(AnimateHandleColor(isActive));
        }
    }
    
    IEnumerator AnimateHandleScale(bool isActive)
    {
        Vector3 targetScale = isActive ? originalHandleScale * handleHoverScale : originalHandleScale;
        Vector3 startScale = handle.localScale;
        
        float duration = 0.2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            handle.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        
        handle.localScale = targetScale;
    }
    
    IEnumerator AnimateHandleColor(bool isActive)
    {
        if (handleImage == null) yield break;
        
        Color targetColor = isActive ? activeColor : normalColor;
        Color startColor = handleImage.color;
        
        float duration = 0.2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            handleImage.color = Color.Lerp(startColor, targetColor, progress);
            yield return null;
        }
        
        handleImage.color = targetColor;
    }
    
    void PlayValueChangeSound()
    {
        if (audioSource != null && valueChangeSound != null)
        {
            audioSource.PlayOneShot(valueChangeSound, 0.3f);
        }
    }
    
    void PlaySnapSound()
    {
        if (audioSource != null && snapSound != null)
        {
            audioSource.PlayOneShot(snapSound, 0.5f);
        }
    }
    
    public void SetValue(float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }
    
    public void SetValueRange(float min, float max)
    {
        if (slider != null)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }
        
        if (useSegments)
        {
            CreateSegments();
        }
    }
    
    public float GetValue()
    {
        return slider != null ? slider.value : 0f;
    }
    
    public int GetIntegerValue()
    {
        return slider != null ? Mathf.RoundToInt(slider.value) : 0;
    }
    
    public void SetInteractable(bool interactable)
    {
        if (slider != null)
        {
            slider.interactable = interactable;
        }
        
        // 更新视觉状态
        Color targetColor = interactable ? normalColor : Color.gray;
        if (handleImage != null)
        {
            handleImage.color = targetColor;
        }
    }
} 