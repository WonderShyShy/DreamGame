using UnityEngine;

public class VRObjectInteraction : MonoBehaviour
{
    [Header("VR设置")]
    public Transform vrCamera;
    public Transform leftController;
    public Transform rightController;
    
    [Header("射线设置")]
    public float rayDistance = 10f;
    public LineRenderer leftRayLine;
    public LineRenderer rightRayLine;
    public Material rayMaterial;
    
    [Header("交互设置")]
    public LayerMask interactableLayer = 1; // 可交互物体层级
    public KeyCode testKey = KeyCode.Space; // 测试键（编辑器用）
    
    [Header("视觉反馈")]
    public GameObject hitPointPrefab; // 射线击中点的视觉效果
    public Color normalRayColor = Color.blue;
    public Color hitRayColor = Color.green;
    
    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip interactionSound;
    
    private Camera vrCam;
    private GameObject currentHitPoint;
    private InteractableObject currentTarget;
    
    void Start()
    {
        SetupRayLines();
        
        if (vrCamera != null)
        {
            vrCam = vrCamera.GetComponent<Camera>();
        }
        
        Debug.Log("VR物体交互系统初始化完成");
    }
    
    void SetupRayLines()
    {
        // 设置左手射线
        if (leftController != null && leftRayLine == null)
        {
            GameObject leftRayGO = new GameObject("LeftControllerRay");
            leftRayGO.transform.SetParent(leftController);
            leftRayLine = leftRayGO.AddComponent<LineRenderer>();
            SetupLineRenderer(leftRayLine);
        }
        
        // 设置右手射线
        if (rightController != null && rightRayLine == null)
        {
            GameObject rightRayGO = new GameObject("RightControllerRay");
            rightRayGO.transform.SetParent(rightController);
            rightRayLine = rightRayGO.AddComponent<LineRenderer>();
            SetupLineRenderer(rightRayLine);
        }
    }
    
    void SetupLineRenderer(LineRenderer line)
    {
        if (line == null) return;
        
        line.material = rayMaterial != null ? rayMaterial : CreateDefaultRayMaterial();
        line.startColor = normalRayColor;
        line.endColor = normalRayColor;
        line.startWidth = 0.01f;
        line.endWidth = 0.005f;
        line.positionCount = 2;
        line.useWorldSpace = true;
    }
    
    Material CreateDefaultRayMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = normalRayColor;
        return mat;
    }
    
    void Update()
    {
        // 处理VR控制器交互
        HandleVRControllerInteraction();
        
        // 编辑器测试（用鼠标射线）
        HandleEditorTest();
    }
    
    void HandleVRControllerInteraction()
    {
        bool leftTrigger = false;
        bool rightTrigger = false;
        
        // 检测VR控制器输入
        // 注意：这里需要根据您使用的VR SDK调整输入检测
        // 例如：OVR、XR Interaction Toolkit、SteamVR等
        
        // XR Interaction Toolkit 示例（如果您使用的话）
        /*
        leftTrigger = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool leftPressed) && leftPressed;
        rightTrigger = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool rightPressed) && rightPressed;
        */
        
        // 临时使用键盘模拟（开发阶段）
        leftTrigger = Input.GetKeyDown(KeyCode.Q);
        rightTrigger = Input.GetKeyDown(KeyCode.E);
        
        // 处理左手控制器
        if (leftController != null)
        {
            HandleControllerRay(leftController, leftRayLine, leftTrigger, "左手控制器");
        }
        
        // 处理右手控制器
        if (rightController != null)
        {
            HandleControllerRay(rightController, rightRayLine, rightTrigger, "右手控制器");
        }
    }
    
    void HandleControllerRay(Transform controller, LineRenderer rayLine, bool triggerPressed, string controllerName)
    {
        if (controller == null) return;
        
        Vector3 rayOrigin = controller.position;
        Vector3 rayDirection = controller.forward;
        
        // 发射射线
        RaycastHit hit;
        bool hasHit = Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, interactableLayer);
        
        // 更新射线视觉效果
        if (rayLine != null)
        {
            Vector3 endPoint = hasHit ? hit.point : rayOrigin + rayDirection * rayDistance;
            rayLine.SetPosition(0, rayOrigin);
            rayLine.SetPosition(1, endPoint);
            
            // 修复：使用startColor和endColor而不是color
            Color currentColor = hasHit ? hitRayColor : normalRayColor;
            rayLine.startColor = currentColor;
            rayLine.endColor = currentColor;
        }
        
        // 处理击中的物体
        if (hasHit)
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            
            if (interactable != null)
            {
                // 更新当前目标
                if (currentTarget != interactable)
                {
                    if (currentTarget != null)
                        currentTarget.OnHoverExit();
                    
                    currentTarget = interactable;
                    currentTarget.OnHoverEnter();
                }
                
                // 显示击中点
                ShowHitPoint(hit.point);
                
                // 处理触发输入
                if (triggerPressed)
                {
                    interactable.OnInteract(controllerName);
                    PlayInteractionSound();
                    Debug.Log($"{controllerName}与{hit.collider.name}交互");
                }
            }
        }
        else
        {
            // 没有击中任何物体
            if (currentTarget != null)
            {
                currentTarget.OnHoverExit();
                currentTarget = null;
            }
            HideHitPoint();
        }
    }
    
    void HandleEditorTest()
    {
        // 编辑器中用鼠标测试
        if (Input.GetKeyDown(testKey) && vrCam != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = vrCam.ScreenPointToRay(mousePos);
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.OnInteract("鼠标测试");
                    PlayInteractionSound();
                    Debug.Log($"鼠标测试与{hit.collider.name}交互");
                }
            }
        }
    }
    
    void ShowHitPoint(Vector3 position)
    {
        if (hitPointPrefab != null)
        {
            if (currentHitPoint == null)
            {
                currentHitPoint = Instantiate(hitPointPrefab);
            }
            currentHitPoint.transform.position = position;
            currentHitPoint.SetActive(true);
        }
    }
    
    void HideHitPoint()
    {
        if (currentHitPoint != null)
        {
            currentHitPoint.SetActive(false);
        }
    }
    
    void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }
    }
    
    void OnDrawGizmos()
    {
        // 在Scene视图中显示射线
        if (leftController != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(leftController.position, leftController.forward * rayDistance);
        }
        
        if (rightController != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rightController.position, rightController.forward * rayDistance);
        }
    }
} 