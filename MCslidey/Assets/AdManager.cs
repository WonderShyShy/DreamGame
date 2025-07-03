using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if ADMOB_ENABLED
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    [Header("Development Settings")]
    public bool enableAds = false;  // 广告系统总开关 - 已禁用倒计时看广告功能
    
    [Header("Admob Ad Units :")]
    string idBanner = "ca-app-pub-8405254493226727/5337408778";
    string idInterstitial = "ca-app-pub-8405254493226727/6110314837";
    string idReward = "ca-app-pub-8405254493226727/9857988159";

    
    AndroidJavaObject currentActivity;
    AndroidJavaClass UnityPlayer;
    AndroidJavaObject context;
    AndroidJavaObject toast;
    
    [Header("Toggle Admob Ads :")]
   private bool bannerAdEnabled = true;
   private bool interstitialAdEnabled = true;
   private bool rewardedAdEnabled = true;

#if ADMOB_ENABLED
    [HideInInspector] public BannerView AdBanner;
    [HideInInspector] public InterstitialAd AdInterstitial;
    [HideInInspector] public RewardedAd AdReward;
#endif

    public GameObject GDPR;

    public static AdManager Instance;
    public bool _firstInit = true;

    protected  void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);

#if UNITY_ANDROID && !UNITY_EDITOR

        UnityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        currentActivity = UnityPlayer
            .GetStatic<AndroidJavaObject>("currentActivity");


        context = currentActivity
            .Call<AndroidJavaObject>("getApplicationContext");
#endif
            
            Instance = this;
            
            // show banner every scene loaded
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                if (PlayerPrefs.GetInt("npa", -1) == -1)
                {
                    if (!enableAds)
                    {
                        // 广告系统关闭时自动跳过GDPR
                        PlayerPrefs.SetInt("npa", 1);
                        Debug.Log("广告系统已禁用 - 自动跳过GDPR");
                        if (_firstInit) this.InitAd();
                    }
                    else
                    {
                        if (GDPR == null)
                        {
                            GameObject original = Resources.Load<GameObject>("CanvasGDPR");
                            GDPR = UnityEngine.Object.Instantiate<GameObject>(original);
                        }
                        GDPR.SetActive(true);
                        Time.timeScale = 0;
                    }
                }
                else
                {
                    if (_firstInit) this.InitAd();
                    else if (enableAds) ShowBanner();
                }
            };
            
        }
        else
        { 
            Destroy(this.gameObject);
        }
      
     
    }
    
    public void ShowToast(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#elif UNITY_ANDROID
            currentActivity.Call
                (
                    "runOnUiThread",
                    new AndroidJavaRunnable(() =>
                    {
                        AndroidJavaClass Toast
                        = new AndroidJavaClass("android.widget.Toast");
            
                        AndroidJavaObject javaString
                        = new AndroidJavaObject("java.lang.String", message);
            
                        toast = Toast.CallStatic<AndroidJavaObject>
                        (
                            "makeText",
                            context,
                            javaString,
                            Toast.GetStatic<int>("LENGTH_SHORT")
                        );
            
                        toast.Call("show");
                    })
                 );
#endif
    }
    
    public void OnUserClickAccept()
    {
        PlayerPrefs.SetInt("npa", 0);
        GDPR.SetActive(false);
        Time.timeScale = 1;
        if (_firstInit) this.InitAd();
        Destroy(GDPR);
    }
    
    
    public void OnUserClickCancel()
    {
        PlayerPrefs.SetInt("npa", 1);
        GDPR.SetActive(false);
        Time.timeScale = 1;
        if (_firstInit) this.InitAd();
        Destroy(GDPR);
    }
    
    public void OnUserClickPrivacyPolicy()
    {
        Application.OpenURL("http://polarisgamestudio.epizy.com/policy.html");
    }

    public void ClickAD()
    {
        PlayerPrefs.SetInt("npa", -1);
        DestroyBannerAd();
        DestroyInterstitialAd();
        if (PlayerPrefs.GetInt("npa", -1) == -1)
        {
            if (GDPR == null)
            {
                GameObject original = Resources.Load<GameObject>("CanvasGDPR");
                GDPR = UnityEngine.Object.Instantiate<GameObject>(original);
            }
            GDPR.SetActive(true);
            Time.timeScale = 0;
        }
        
        _firstInit = true;
    }
    
    public void InitAd()
    {
        if (!enableAds)
        {
            Debug.Log("广告系统已禁用 - 跳过初始化");
            _firstInit = false;
            return;
        }
        
#if ADMOB_ENABLED
        // 检查iOS AdMob配置
        #if UNITY_IOS && !UNITY_EDITOR
        if (string.IsNullOrEmpty(GoogleMobileAds.GoogleMobileAdsSettings.Instance.AdMobIOSAppId))
        {
            Debug.LogWarning("iOS AdMob应用ID未配置，广告功能可能无法正常工作。请在Google Mobile Ads设置中配置iOS应用ID。");
            _firstInit = false;
            return;
        }
        #endif
        
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .build();
        

        MobileAds.Initialize(initstatus => {
            MobileAdsEventExecutor.ExecuteInUpdate(() => {
                ShowBanner();
                RequestRewardAd();
                RequestInterstitialAd();
                _firstInit = false;
            });
        });
#else
        Debug.Log("AdMob已禁用 - 跳过广告初始化");
        _firstInit = false;
#endif
    }

    private void OnDestroy()
    {
#if ADMOB_ENABLED
        DestroyBannerAd();
        DestroyInterstitialAd();
#endif
    }

    public void Destroy() => Destroy(gameObject);

    public bool IsRewardAdLoaded()
    {
        if (!enableAds) return true;  // 广告关闭时模拟已加载
        
#if ADMOB_ENABLED
        if (rewardedAdEnabled && AdReward != null && AdReward.IsLoaded())
            return true;
        else
            return false;
#else
        return true;  // AdMob禁用时模拟已加载
#endif
    }
    
#if ADMOB_ENABLED
    AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
           .TagForChildDirectedTreatment(false)
           .AddExtra("npa", PlayerPrefs.GetInt("npa", 1).ToString())
           .Build();
    }
#endif

    #region Banner Ad ------------------------------------------------------------------------------
    public void ShowBanner()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 跳过横幅广告");
            return;
        }
        
#if ADMOB_ENABLED
        if (!bannerAdEnabled) return;

        DestroyBannerAd();

        AdBanner = new BannerView(idBanner, AdSize.Banner, AdPosition.Bottom);

        AdBanner.LoadAd(CreateAdRequest());
#else
        Debug.Log("AdMob已禁用 - 跳过横幅广告");
#endif
    }
    
    public void AdsButtonPressed()
    {
        PlayerPrefs.SetInt("npa", -1);

        //load gdpr scene
        LoadLevel(1);
    }
    
    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified");
        }
    }

    public void DestroyBannerAd()
    {
#if ADMOB_ENABLED
        if (AdBanner != null)
            AdBanner.Destroy();
#endif
    }
    #endregion

    #region Interstitial Ad ------------------------------------------------------------------------
    public void RequestInterstitialAd()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 跳过插屏广告加载");
            return;
        }
        
#if ADMOB_ENABLED
        AdInterstitial = new InterstitialAd(idInterstitial);

        AdInterstitial.OnAdClosed += HandleInterstitialAdClosed;

        AdInterstitial.LoadAd(CreateAdRequest());
#else
        Debug.Log("AdMob已禁用 - 跳过插屏广告加载");
#endif
    }

    public void ShowInterstitialAd()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 模拟插屏广告完成");
            // 直接触发插屏广告关闭回调
            if (InteralADAction != null)
            {
                InteralADAction.Invoke();
                InteralADAction = null;
            }
            return;
        }
        
#if ADMOB_ENABLED
        if (!interstitialAdEnabled) return;

        if (AdInterstitial != null && AdInterstitial.IsLoaded())
        {
            AdInterstitial.Show();
        }
#else
        Debug.Log("AdMob已禁用 - 模拟插屏广告完成");
        if (InteralADAction != null)
        {
            InteralADAction.Invoke();
            InteralADAction = null;
        }
#endif
    }
    
    public bool IsInterstitialAdLoad()
    {
        if (!enableAds) return true;  // 广告关闭时模拟已加载
        
#if ADMOB_ENABLED
        if (interstitialAdEnabled && AdInterstitial !=null && AdInterstitial.IsLoaded())
            return true;
        else
            return false;
#else
        return true;  // AdMob禁用时模拟已加载
#endif
    }

    public void DestroyInterstitialAd()
    {
#if ADMOB_ENABLED
        if (AdInterstitial != null)
            AdInterstitial.Destroy();
#endif
    }
    #endregion

    #region Rewarded Ad ----------------------------------------------------------------------------
    public void RequestRewardAd()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 跳过激励视频加载");
            return;
        }
        
#if ADMOB_ENABLED
        AdReward = new RewardedAd(idReward);

        AdReward.OnAdClosed += HandleOnRewardedAdClosed;
        AdReward.OnUserEarnedReward += HandleOnRewardedAdWatched;

        AdReward.LoadAd(CreateAdRequest());
#else
        Debug.Log("AdMob已禁用 - 跳过激励视频加载");
#endif
    }   
    
   
    public void ShowRewardAd()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 模拟激励视频观看完成，直接给予奖励");
            // 直接触发奖励回调，模拟用户观看完成
            if (RewardAction != null)
            {
                RewardAction.Invoke();
                RewardAction = null;
            }
            return;
        }
        
#if ADMOB_ENABLED
        if (!rewardedAdEnabled) return;

        if (AdReward.IsLoaded())
            AdReward.Show();
        else
        {
            RequestRewardAd();
            ShowToast("Reward based video ad is not ready yet");
        }
#else
        Debug.Log("AdMob已禁用 - 模拟激励视频观看完成，直接给予奖励");
        if (RewardAction != null)
        {
            RewardAction.Invoke();
            RewardAction = null;
        }
#endif
    } 
    

    public bool IsCanShowRewardAD()
    {
        if (!enableAds) return true;  // 广告关闭时模拟可以显示
        
#if ADMOB_ENABLED
        if (AdReward.IsLoaded())
        {
            return true;
        }

        return false;
#else
        return true;  // AdMob禁用时模拟可以显示
#endif
    }   
    
    
    #endregion

    #region Event Handler
    
    public Action InteralADAction = null;
    
#if ADMOB_ENABLED
    private void HandleInterstitialAdClosed(object sender, EventArgs e)
    {
        if (InteralADAction != null)
        {
            InteralADAction.Invoke();
        }
        InteralADAction?.Invoke();
        DestroyInterstitialAd();
        RequestInterstitialAd();
    }
#endif

    public Action RewardAction = null;
    
#if ADMOB_ENABLED
    private void HandleOnRewardedAdClosed(object sender, EventArgs e)
    {
        RequestRewardAd();
    }

    private void HandleOnRewardedAdWatched(object sender, Reward e)
    {
        if (RewardAction != null)
        {
            RewardAction.Invoke();
        }

        RewardAction = null;
    }
#endif
    #endregion
}
