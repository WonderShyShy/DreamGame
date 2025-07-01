using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    [Header("Development Settings")]
    public bool enableAds = false;  // 广告系统总开关
    
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

    [HideInInspector] public BannerView AdBanner;
    [HideInInspector] public InterstitialAd AdInterstitial;
    [HideInInspector] public RewardedAd AdReward;

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
    }

    private void OnDestroy()
    {
        DestroyBannerAd();
        DestroyInterstitialAd();
    }

    public void Destroy() => Destroy(gameObject);

    public bool IsRewardAdLoaded()
    {
        if (!enableAds) return true;  // 广告关闭时模拟已加载
        
        if (rewardedAdEnabled && AdReward != null && AdReward.IsLoaded())
            return true;
        else
            return false;
    }
    
    AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
           .TagForChildDirectedTreatment(false)
           .AddExtra("npa", PlayerPrefs.GetInt("npa", 1).ToString())
           .Build();
    }

    #region Banner Ad ------------------------------------------------------------------------------
    public void ShowBanner()
    {
        if (!enableAds) 
        {
            Debug.Log("广告系统已禁用 - 跳过横幅广告");
            return;
        }
        
        if (!bannerAdEnabled) return;

        DestroyBannerAd();

        AdBanner = new BannerView(idBanner, AdSize.Banner, AdPosition.Bottom);

        AdBanner.LoadAd(CreateAdRequest());
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
        if (AdBanner != null)
            AdBanner.Destroy();
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
        
        AdInterstitial = new InterstitialAd(idInterstitial);

        AdInterstitial.OnAdClosed += HandleInterstitialAdClosed;

        AdInterstitial.LoadAd(CreateAdRequest());
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
        
        if (!interstitialAdEnabled) return;

        if (AdInterstitial != null && AdInterstitial.IsLoaded())
        {
            AdInterstitial.Show();
        }
    }
    
    public bool IsInterstitialAdLoad()
    {
        if (!enableAds) return true;  // 广告关闭时模拟已加载
        
        if (interstitialAdEnabled && AdInterstitial !=null && AdInterstitial.IsLoaded())
            return true;
        else
            return false;
    }

    public void DestroyInterstitialAd()
    {
        if (AdInterstitial != null)
            AdInterstitial.Destroy();
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
        
        AdReward = new RewardedAd(idReward);

        AdReward.OnAdClosed += HandleOnRewardedAdClosed;
        AdReward.OnUserEarnedReward += HandleOnRewardedAdWatched;

        AdReward.LoadAd(CreateAdRequest());
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
        
        if (!rewardedAdEnabled) return;

        if (AdReward.IsLoaded())
            AdReward.Show();
        else
        {
            RequestRewardAd();
            ShowToast("Reward based video ad is not ready yet");
        }
    } 
    

    public bool IsCanShowRewardAD()
    {
        if (!enableAds) return true;  // 广告关闭时模拟可以显示
        
        if (AdReward.IsLoaded())
        {
            return true;
        }

        return false;
    }   
    
    
    #endregion

    #region Event Handler
    
    public Action InteralADAction = null;
    
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

    public Action RewardAction = null;
    
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
    #endregion
}
