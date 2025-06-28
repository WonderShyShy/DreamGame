using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
   
    [Header("UI 元素")]
    public Slider volumeSlider;
    public Button muteButton;
    public Image muteIcon;

    [Header("图标设置")]
    public Sprite mutedSprite;
    public Sprite unmutedSprite;

    [Header("音频设置")]
    [Range(0f, 1f)] public float defaultVolume = 0.8f;
    
    // 存储所有需要控制的音频源
    private List<AudioSource> audioSources = new List<AudioSource>();
    private bool isMuted = false;
    private float currentVolume;
    public GameData Data;
    

    void Awake()
    {
        Data = GameData.Instance;   
        // 初始化音量
        currentVolume = Data.volume;
        isMuted = Data.isMuted;
        
        // 自动收集场景中的所有AudioSource
        FindAllAudioSources();
        
        // 修复：正确注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // 修复：在对象销毁时取消注册事件，防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAllAudioSources();  // 重新收集新场景的音频源
        UpdateAudioSources();   // 立即应用当前设置
    }
    void Start()
    {
        FindAllAudioSources();
        UpdateAudioSources();
        // 设置UI初始状态
        if (volumeSlider != null)
        {
            volumeSlider.value = currentVolume;
            volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        }
        
        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
        }
        
        UpdateAudioSources();
        UpdateMuteIcon();
    }
    
    // 自动收集场景中的AudioSource
    public void FindAllAudioSources()
    {
        audioSources.Clear();
        AudioSource[] sources = FindObjectsOfType<AudioSource>(true); // 包含未激活对象
        foreach (AudioSource source in sources)
        {
            if (!source.CompareTag("Sound"))
            {
                audioSources.Add(source);
                ApplyVolumeSettings(source); // 立即应用设置
            }
        }
        Debug.Log($"找到 {audioSources.Count} 个音频源");
    }
    
    // 手动添加AudioSource（用于动态生成的音频源）
    public void AddAudioSource(AudioSource source)
    {
        if (!audioSources.Contains(source))
        {
            audioSources.Add(source);
            ApplyVolumeSettings(source);
        }
    }
    
    // 设置全局音量
    public void SetGlobalVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);
        isMuted = false; // 修复：移除注释中的"da"
        Data.volume = currentVolume;
        
        PlayerPrefs.SetFloat("MasterVolume", currentVolume);
        PlayerPrefs.SetInt("IsMuted", 0);
        PlayerPrefs.Save();
        
        UpdateAudioSources();
        UpdateMuteIcon();
    }
    
    // 切换静音状态
    public void ToggleMute()
    {
        isMuted = !isMuted;
        Data.isMuted = isMuted;
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateAudioSources();
        UpdateMuteIcon();
    }
    
    // 更新所有音频源的音量
    private void UpdateAudioSources()
    {
        foreach (AudioSource source in audioSources)
        {
            ApplyVolumeSettings(source);
        }
    }
    
    // 应用音量设置到单个音频源
    private void ApplyVolumeSettings(AudioSource source)
    {
        if (source == null) return;
        
        if (isMuted)
        {
            source.volume = 0f;
        }
        else
        {
            source.volume = currentVolume;
        }
    }
    
    // 更新静音图标
    private void UpdateMuteIcon()
    {
        if (muteIcon != null)
        {
            muteIcon.sprite = isMuted ? mutedSprite : unmutedSprite;
        }
    }
}