//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Manager;
//using Newtonsoft.Json.Linq;
//using Platform;
//using UnityEngine;
//
//
//namespace Plugins
//{
//    public class AppLovinMediationBanner
//    {
//        
//        private static readonly string TAG = "AppLovinMediationBanner-------";
//        private static readonly object padlock = new object();
//        
//
//        private readonly string[] ids = { "1a6223d5c709b7d3" };
//
//
//        private string[] configIds = { };
//
//        /// <summary>
//        /// 记录多个banner，bool = true 那个就是现在展示的
//        /// </summary>
//        private readonly Dictionary<string, bool> _shown = new Dictionary<string, bool>();
//        /// <summary>
//        /// 加载成功的banner
//        /// </summary>
//        private readonly Dictionary<string, bool> _loaded = new Dictionary<string, bool>();
//        /// <summary>
//        /// 加载中的Banner
//        /// </summary>
//        private readonly Dictionary<string, bool> _loading = new Dictionary<string, bool>();
//        /// <summary>
//        /// 加载失败次数
//        /// </summary>
//        private readonly Dictionary<string, int> _failedCount = new Dictionary<string, int>();
//
//        private bool _hasInitialed;
//        private bool _guideFinish = false;
//        private bool _splashFinish = false;
//
//        private bool _succeed = false;
//
//
//        private bool _firstLoaded = true;
//        private DateTime _lastShownTime = DateTime.Now;
//
//        private string _lastBanner = "";
//        private CancellationTokenSource _cancellationTokenSource;
//
//
//
//        public AppLovinMediationBanner()
//        {
//
//            Helper.Log(TAG + "AppLovinMediation - Banner");
//
//            
//            // MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoadedEvent;
//            // MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdFailedEvent;
//            // MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClickedEvent;
//            // MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
//            //
//        }
//
//        public void OnSdkInitializedEvent()
//        {
//            _hasInitialed = true;
////            LoadBannerPlugins();
//            TestLoadBannerPlugins();
//        }
//
//        private void TestLoadBannerPlugins()
//        {
////             if (!_hasInitialed)
////             {
////                 Helper.Log(TAG + "loadBannerPlugins not enable by no inittiallized");
////                 return;
////             }
////             Helper.Log(TAG + "loadBannerPlugins");
////             string original = PlatformBridge.getConfigList("Application.AppLovinUnit.Banner");
////             Helper.Log(TAG + "original string: " + original);
////             if (!string.IsNullOrEmpty(original))
////             {
////                 string[] oriID = JArray.Parse(original).ToObject<string[]>();
////                 if (oriID != null && oriID.Length > 0)
////                 {
////                     Helper.Log(TAG + "parse original config id: " + oriID.Length);
////                     configIds = oriID;
////                 }
////
////                 foreach (var adUnit in configIds)
////                 {
////                     Helper.Log(TAG + "parse config id: " + adUnit);
////                 }
////             }
////             
////             if (configIds == null || configIds.Length <= 0)
////             {
////                 Helper.Log(TAG + "use default ids");
////                 configIds = ids;
////             }
////
////             foreach (var adUnit in configIds)
////             {
////                 Helper.Log(TAG + "AdUnit is : " + adUnit);
////                 
////                 MaxSdk.CreateBanner(adUnit, MaxSdkBase.BannerPosition.BottomCenter);
////
////                 // Set background or background color for banners to be fully functional
//// //                MaxSdk.SetBannerBackgroundColor(adUnit);
////             }
//        }
//        
//        
//        private void LoadBannerPlugins()
//        {
//            if (!_hasInitialed)
//            {
//                Helper.Log(TAG + "loadBannerPlugins not enable by no inittiallized");
//                return;
//            }
//            Helper.Log(TAG + "loadBannerPlugins");
//
//            string original = PlatformBridge.getConfigList("Application.AppLovinUnit.Banner");
//            Helper.Log(TAG + "original string: " + original);
//            if (!string.IsNullOrEmpty(original))
//            {
//                string[] oriID = JArray.Parse(original).ToObject<string[]>();
//                if (oriID != null && oriID.Length > 0)
//                {
//                    Helper.Log(TAG + "parse original config id: " + oriID.Length);
//                    configIds = oriID;
//                }
//
//                foreach (var adUnit in configIds)
//                {
//                    Helper.Log(TAG + "parse config id: " + adUnit);
//                }
//            }
//
//            if (configIds == null || configIds.Length <= 0)
//            {
//                Helper.Log(TAG + "use default ids");
//                configIds = ids;
//            }
//            
//            foreach (var adUnit in configIds)
//            {
//                Helper.Log(TAG + "AdUnit is : " + adUnit);
//                _failedCount[adUnit] = 0;
//                _shown[adUnit] = false;
//                _loaded[adUnit] = false;
//
//                LoadBanner(adUnit);
//            }
//        }
//
//        private void LoadBanner(string adUnit)
//        {
//            return;
//            
//            
////             
////             if (!PlatformBridge.getConfigBoolean("Application.Banner.Enable", false))
////             {
////                 Helper.Log(TAG + "LoadBanner not enable by Config");
////                 return;
////             }
////
////             if (!_hasInitialed)
////             {
////                 Helper.Log(TAG + "LoadBanner not enable by no inittiallized");
////                 return;
////             }
////
////             if (!_guideFinish)
////             {
////                 Helper.Log(TAG + "LoadBanner not enable by guide");
////                 return;
////             }
////
////             if (!_splashFinish)
////             {
////                 Helper.Log(TAG + "LoadBanner not enable by splash");
////                 return;
////             }
////
////             if (_loading.ContainsKey(adUnit) && _loading[adUnit])
////             {
////                 Helper.Log(TAG + "Banner adUnitId: " + adUnit + " is loading now");
////                 return;
////             }
////
////             if (_loaded.ContainsKey(adUnit) && _loaded[adUnit])
////             {
////                 Helper.Log(TAG + "Banner adUnitId: " + adUnit + " is loaded");
////                 return;
////             }
////             
////             // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
////             // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
////             MaxSdk.CreateBanner(adUnit, MaxSdkBase.BannerPosition.BottomCenter);
////             // Set background or background color for banners to be fully functional
////             MaxSdk.SetBannerBackgroundColor(adUnit, Color.black);
//// #if UNITY_IOS && !UNITY_EDITOR
////
//// #elif UNITY_ANDROID && !UNITY_EDITOR
////         
////         if (PlatformBridge.getConfigBoolean("Application.Banner.RequestLog", false))
////         {
////             PlatformBridge.logESEvent("New_Banner_Request");
////         }           
//// #else
////
//// #endif
////             _loading[adUnit] = true;
////             Helper.Log(TAG + "LoadBanner with adUnitId:" + adUnit);
//        }
//
//        public void LoadBanner()
//        {
//            Helper.Log(TAG + "Load all banners");
//            foreach (var adUnit in configIds)
//            {
//                LoadBanner(adUnit);
//            }
//        }
//
//        private void LoadBannerWithoutId(string id)
//        {
//
//            Helper.Log(TAG + "Load all banners without id " + id);
//            foreach (var adUnit in configIds)
//            {
//                if (!adUnit.Equals(id))
//                {
//                    LoadBanner(adUnit);
//                }
//            }
//        }
//
//        private void InvalidLastAdunit(string currentId)
//        {
//            if (!currentId.Equals(_lastBanner))
//            {
//                Helper.Log(TAG + "Invalid Last Adunit; current id: " + currentId + ", last id is: " + _lastBanner);
//
//                if (_lastBanner.Length > 0)
//                {
//                    Helper.Log(TAG + "Hide and load adunit id: " + _lastBanner);
//
//                    _loaded[_lastBanner] = false;
//                    _shown[_lastBanner] = false;
////                    MaxSdk.HideBanner(_lastBanner);
//
//                    LoadBanner(_lastBanner);
//                }
//                _lastBanner = currentId;
//            } else
//            {
//                LoadBannerWithoutId(currentId);
//            }
//        }
//
//        private void RealShowBanner(string adUnit)
//        {
//            InvalidLastAdunit(adUnit);
//
//            // MaxSdk.ShowBanner(adUnit);
//            _shown[adUnit] = true;
//            _lastShownTime = DateTime.Now;
//            Helper.Log(TAG + "Real Show Banner ad to the user: " + adUnit +  " time: " + DateTime.Now);
//
//            if (PlatformBridge.getConfigBoolean("Application.Banner.NewESLog", false))
//            {
//                PlatformBridge.logESEvent("New_Banner_Shown");
//            }
//
////            if (_cancellationTokenSource != null)
////            {
////                _cancellationTokenSource.Cancel();
////            }
////
////            _cancellationTokenSource = new CancellationTokenSource();
////            Task.Delay(PlatformBridge.getConfigInt("Application.Banner.ShowInterval", 25000), _cancellationTokenSource.Token)
////                .ContinueWith(t => ShowH5Banner(), TaskScheduler.FromCurrentSynchronizationContext());
//        }
//
//        private void ShowH5Banner()
//        {
//            Helper.Log(TAG + "Try Show Banner ad to the user " +  " time:" + DateTime.Now);
//
//            string _nextShow  = "";
//            string _current = "";
//            foreach (var adUnit in configIds)
//            {
//                if (_nextShow.Equals("") 
//                        && _loaded[adUnit] 
//                            && !_shown[adUnit]) {
//
//                    _nextShow = adUnit;
//                } else if (_current.Equals("") && 
//                        _loaded[adUnit] 
//                            && _shown[adUnit])
//                {
//                    _current = adUnit;
//                }
//            }
//
//            if (!_nextShow.Equals(""))
//            {
//                RealShowBanner(_nextShow);
//            } else if (!_current.Equals(""))
//            {
//                if (configIds.Length <= 1)
//                {
//                    Helper.Log(TAG + "Load all banners as only one id");
//                    _loaded[_current] = false;
//
//                    LoadBanner();
//                    return;
//                }
//
//                LoadBannerWithoutId(_current);
//            }
//            else
//            {
//                LoadBanner();
//            }
//
//        }
//
//        public void DestroyBanner()
//        {
////             foreach (var id in configIds)
////             {
////                 MaxSdk.DestroyBanner(id);
////
////                 if (_shown.ContainsKey(id))
////                 {
////                     _shown[id] = false;
////                 }
////
////                 if (_loaded.ContainsKey(id))
////                 {
////                     _loaded[id] = false;
////                 }
////                 if (_loading.ContainsKey(id))
////                 {
////                     _loading[id] = false;
////                 }
////
////                 if (_failedCount.ContainsKey(id))
////                 {
////                     _failedCount[id] = 0;
////                 }
////             }
////
////             _firstLoaded = true;
////             _lastShownTime = DateTime.Now;
////
//// //            if (_cancellationTokenSource != null)
//// //            {
//// //                _cancellationTokenSource.Cancel();
//// //            }
//        }
//
//
////         public void OnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             
////             MaxSdk.ShowBanner(adUnitId);
////            
////             Helper.Log(TAG + "Real Show Banner ad to the user: " + adUnitId +  " time: " + DateTime.Now);
////
////             if (PlatformBridge.getConfigBoolean("Application.Banner.NewESLog", false))
////             {
////                 PlatformBridge.logESEvent("New_Banner_Shown");
////             }
////             return;
////             Helper.Log(TAG + "Load Banner Successfully adUnitId:" + adUnitId);
////             
////             MaxSdk.HideBanner(adUnitId);
////             
////             _loading[adUnitId] = false;
////             _loaded[adUnitId] = true;
////             
////             lock (padlock)
////             {
////                 if (_firstLoaded 
////                     || ((DateTime.Now.Subtract(_lastShownTime).TotalMilliseconds)) 
////                     >= PlatformBridge.getConfigInt("Application.Banner.ShowInterval", 25000))
////                 {
////                     Helper.Log(TAG + "Load Banner Successfully and Real Show banner " + adUnitId);
////                     RealShowBanner(adUnitId);
////                 }
////                 else
////                 {
////
////                 }
////                 
////                 _firstLoaded = false;
////                 _succeed = true;
////                 _failedCount[adUnitId] = 0;
////             }
////         }
////
////         public void OnAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
////         {
////             
////             /// 多条Banner ID
////             return;
////             Helper.Log(TAG + "Load Banner Failed adUnitId:" + adUnitId + ", coount: " + (_failedCount[adUnitId] + 1) + ", errorMessage: " + errorInfo.Message + " ,ErrorCode: " + errorInfo.Code);
////             int MaxFail = PlatformBridge.getConfigInt("Application.Banner.FailCount", 5);
////             if (++_failedCount[adUnitId] >= MaxFail)
////             {
////                 Helper.Log(TAG + "Failed greater than " + MaxFail + " times, reset _succeed to false, " + adUnitId);
////                 _succeed = false;
////                 _failedCount[adUnitId] = 0;
////             }
////
////             bool _tmpLoaded = false;
////             if (!_succeed)
////             {
////
////                 bool _kept = PlatformBridge.getConfigBoolean("Application.Banner.Kept", false);
////                 if (!_kept || !_loaded[adUnitId] || !_shown[adUnitId])
////                 {
////                     Helper.Log(TAG + "TRY 10 secs later: " + adUnitId);
//// //                    Task.Delay(PlatformBridge.getConfigInt("Application.Banner.RequestInterval", 10000))
//// //                        .ContinueWith(t => LoadBanner(adUnitId),
//// //                                TaskScheduler.FromCurrentSynchronizationContext());
////                 }
////
////                 _tmpLoaded = _kept && _loaded[adUnitId] && _shown[adUnitId];
////             }
////
////             _loading[adUnitId] = false;
////             _loaded[adUnitId] = _tmpLoaded;
////         }
////
////         private void OnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             ManagerAd.OnAdClk();
////         }
////         
////         private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             // Banner ad revenue paid. Use this callback to track user revenue.
////             Helper.Log(TAG + "Banner ad revenue paid");
////
////             // Ad revenue
////             double revenue = adInfo.Revenue;
////
////             // Miscellaneous data
////             string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
////             string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
////             string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
////             string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
////
////             AppLovinMediationAdapter.Instance.OnImpressionTrackedEvent(adUnitId, adInfo);
////         }
//
//        public void OnSplashFinish()
//        {
//            _splashFinish = true;
//
//            Helper.Log(TAG + "OnSplashFinish");
//            LoadBanner();
//        }
//
//        public void OnGuideComplete()
//        {
//            _guideFinish = true;
//
//            Helper.Log(TAG + "OnGuideComplete");
//            LoadBanner();
//        }
//
//        public bool IsBannerId(string id)
//        {
//            foreach (string adUnit in configIds)
//            {
//                if (adUnit.Equals(id))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//
//        public void OnMainSceneOnRestart()
//        {
//            Helper.Log(TAG + " try to reload banner on restart");
//            LoadBanner();
//        }
//   
//    }
//
//}
//
