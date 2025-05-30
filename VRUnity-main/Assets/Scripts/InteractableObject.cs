using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("交互设置")]
    public string objectName = "可交互物体";
    public string interactionText = "点击交互";
    
    [Header("视觉反馈")]
    public bool enableHoverEffect = true;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color interactColor = Color.green;
    public float glowIntensity = 1.5f;
    
    [Header("音效")]
    public AudioClip hoverSound;
    public AudioClip interactSound;
    
    [Header("事件")]
    public UnityEvent OnInteractionEvent;
    public UnityEvent OnHoverEnterEvent;
    public UnityEvent OnHoverExitEvent;
    
    [Header("交互类型")]
    public InteractionType interactionType = InteractionType.OneTime;
    public float cooldownTime = 1f;
    
    public enum InteractionType
    {
        OneTime,      // 只能交互一次
        Repeatable,   // 可重复交互
        Toggle,       // 开关状态
        Cooldown      // 冷却时间
    }
    
    private Renderer objectRenderer;
    private Material originalMaterial;
    private Material glowMaterial;
    private AudioSource audioSource;
    private bool isHovered = false;
    private bool hasInteracted = false;
    private bool toggleState = false;
    private float lastInteractionTime = 0f;
    
    void Start()
    {
        SetupComponents();
        SetupMaterials();
    }
    
    void SetupComponents()
    {
        // 获取渲染器
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
        
        // 添加音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D音效
        }
        
        // 确保有碰撞器
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"{gameObject.name} 没有碰撞器，添加了BoxCollider");
            gameObject.AddComponent<BoxCollider>();
        }
        
        Debug.Log($"可交互物体 {objectName} 初始化完成");
    }
    
    void SetupMaterials()
    {
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
            
            // 创建发光材质（如果启用悬停效果）
            if (enableHoverEffect)
            {
                glowMaterial = new Material(originalMaterial);
                glowMaterial.EnableKeyword("_EMISSION");
                glowMaterial.SetColor("_EmissionColor", hoverColor * glowIntensity);
            }
        }
    }
    
    public void OnHoverEnter()
    {
        if (!isHovered)
        {
            isHovered = true;
            
            // 视觉反馈
            if (enableHoverEffect && objectRenderer != null && glowMaterial != null)
            {
                objectRenderer.material = glowMaterial;
            }
            
            // 音效反馈
            if (hoverSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
            
            // 触发事件
            OnHoverEnterEvent?.Invoke();
            
            Debug.Log($"悬停进入: {objectName}");
        }
    }
    
    public void OnHoverExit()
    {
        if (isHovered)
        {
            isHovered = false;
            
            // 恢复原始材质
            if (enableHoverEffect && objectRenderer != null && originalMaterial != null)
            {
                objectRenderer.material = originalMaterial;
            }
            
            // 触发事件
            OnHoverExitEvent?.Invoke();
            
            Debug.Log($"悬停退出: {objectName}");
        }
    }
    
    public void OnInteract(string interactorName)
    {
        // 检查交互条件
        if (!CanInteract())
        {
            Debug.Log($"{objectName} 当前无法交互");
            return;
        }
        
        // 记录交互时间
        lastInteractionTime = Time.time;
        
        // 根据交互类型处理
        switch (interactionType)
        {
            case InteractionType.OneTime:
                if (!hasInteracted)
                {
                    hasInteracted = true;
                    PerformInteraction(interactorName);
                }
                break;
                
            case InteractionType.Repeatable:
                PerformInteraction(interactorName);
                break;
                
            case InteractionType.Toggle:
                toggleState = !toggleState;
                PerformToggleInteraction(interactorName, toggleState);
                break;
                
            case InteractionType.Cooldown:
                PerformInteraction(interactorName);
                break;
        }
    }
    
    bool CanInteract()
    {
        switch (interactionType)
        {
            case InteractionType.OneTime:
                return !hasInteracted;
                
            case InteractionType.Repeatable:
                return true;
                
            case InteractionType.Toggle:
                return true;
                
            case InteractionType.Cooldown:
                return Time.time - lastInteractionTime >= cooldownTime;
                
            default:
                return true;
        }
    }
    
    void PerformInteraction(string interactorName)
    {
        // 视觉反馈
        StartCoroutine(InteractionFlash());
        
        // 音效反馈
        if (interactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactSound);
        }
        
        // 触发事件
        OnInteractionEvent?.Invoke();
        
        // 自定义交互逻辑
        HandleInteraction(interactorName);
        
        Debug.Log($"{interactorName} 与 {objectName} 交互成功！");
    }
    
    void PerformToggleInteraction(string interactorName, bool newState)
    {
        PerformInteraction(interactorName);
        
        // 切换状态的视觉反馈
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newState ? interactColor : normalColor;
        }
        
        Debug.Log($"{objectName} 切换状态: {(newState ? "开启" : "关闭")}");
    }
    
    System.Collections.IEnumerator InteractionFlash()
    {
        if (objectRenderer != null)
        {
            Material flashMaterial = new Material(originalMaterial);
            flashMaterial.color = interactColor;
            
            objectRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.2f);
            
            if (isHovered && glowMaterial != null)
            {
                objectRenderer.material = glowMaterial;
            }
            else
            {
                objectRenderer.material = originalMaterial;
            }
        }
    }
    
    // 虚函数，子类可以重写实现具体的交互逻辑
    protected virtual void HandleInteraction(string interactorName)
    {
        // 基础交互逻辑
        // 子类可以重写此方法实现具体功能
    }
    
    // 公共方法，供外部调用
    public void ResetInteraction()
    {
        hasInteracted = false;
        toggleState = false;
        lastInteractionTime = 0f;
        
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
        }
        
        Debug.Log($"{objectName} 交互状态已重置");
    }
    
    public bool IsInteracted()
    {
        return hasInteracted;
    }
    
    public bool GetToggleState()
    {
        return toggleState;
    }
    
    // 在Inspector中显示信息
    void OnValidate()
    {
        if (string.IsNullOrEmpty(objectName))
        {
            objectName = gameObject.name;
        }
    }
    
    void OnDrawGizmos()
    {
        // 在Scene视图中显示交互范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
} 