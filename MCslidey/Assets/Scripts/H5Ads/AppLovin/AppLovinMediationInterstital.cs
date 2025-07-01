//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Manager;
//using Models;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Platform;
//using UnityEngine;
//
//
//namespace Plugins
//{
//    public class AppLovinMediationInterstital
//    {
//        
//        private static readonly string TAG = "AppLovinMediationInterstital-------";
//        
//        [HideInInspector]
//        public string interstitialType = "";
//        
//        private string[] configIds = { };
//        
//        private readonly Dictionary<string, bool> _loaded = new Dictionary<string, bool>();
//        private readonly Dictionary<string, bool> _loading = new Dictionary<string, bool>();
//        private readonly Dictionary<string, bool> _restartAds = new Dictionary<string, bool>();
//
//        private string _lastAdUnitId = "";
//        private bool _hasInitialed;
//        private bool _adShowing = false;
//        private string[] _fiveIDs = {  };
//        
//        private readonly string[] ids = { "a787a7aef35782c2" };
//
//
//        public AppLovinMediationInterstital()
//        {
//            Helper.Log(TAG + "AppLovinMediation - Interstitial");
//            
//            // MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
//            // MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
//
//            
//        }
//        
//        
//        public void OnSdkInitializedEvent()
//        {
//            _hasInitialed = true;
//            loadInterstitialPlugins();
//        }
//        
//         private void loadInterstitialPlugins()
//        {
//            if (!_hasInitialed)
//            {
//                return;
//            }
//            Helper.Log(TAG + "loadInterstitialPlugins");
//
//            string fiveS = PlatformBridge.getConfigList("Application.AppLovinUnit.Interstitial5s");
//            Helper.Log(TAG + "fiveS string: " + fiveS);
//            
//            if (!string.IsNullOrEmpty(fiveS))
//            {
//                string[] id5 = JArray.Parse(fiveS).ToObject<string[]>();
//                if (id5 != null && id5.Length > 0)
//                {
//                    _fiveIDs = id5;
//                }
//            }
//
//            string original = PlatformBridge.getConfigList("Application.AppLovinUnit.Interstitial");
//            Helper.Log(TAG + "original string: " + original);
//
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
//                Helper.Log(TAG + "AdUnit:" + adUnit);
//            }
//
//            string[] configIdAnd5s = configIds.Concat(_fiveIDs).ToArray();
//            foreach (var adUnit in configIdAnd5s)
//            {
//                Helper.Log(TAG + "configIdAnd5s:" + adUnit);
//            }
//
//            
//            Task.Delay(900).ContinueWith(t => LoadInterstitial(), TaskScheduler.FromCurrentSynchronizationContext());
////            Task.Delay(1000).ContinueWith(t => Load5Interstitial(), TaskScheduler.FromCurrentSynchronizationContext());
//        }
//         
//        
//        
//        public void LoadInterstitial()
//        {
//            LoadInterstitialInternal(configIds);
//        }
//        
//        public void Load5Interstitial()
//        {
//            LoadInterstitialInternal(_fiveIDs);
//        }
//
//        private void LoadInterstitialInternal(string[] IDs)
//        {
//            // if (!_hasInitialed)
//            // {
//            //     return;
//            // }
//            //
//            // foreach (var interstitialAdUnit in IDs)
//            // {
//            //     if (_loaded.ContainsKey(interstitialAdUnit) && _loaded[interstitialAdUnit])
//            //     {
//            //         Helper.Log(TAG + "LoadInterstitial. adUnitId:" + interstitialAdUnit + " has loaded");
//            //         continue;
//            //     }
//            //
//            //     if (_loading.ContainsKey(interstitialAdUnit) && _loading[interstitialAdUnit])
//            //     {
//            //         Helper.Log(TAG + "LoadInterstitial. adUnitId:" + interstitialAdUnit + " is loading");
//            //         continue;
//            //     }
//            //
//            //     MaxSdk.LoadInterstitial(interstitialAdUnit);
//            //     _loading[interstitialAdUnit] = true;
//            //
//            //     if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLog", false))
//            //     {
//            //         PlatformBridge.logESEvent("Load_Interstitial_Ad", true);
//            //     }
//            //
//            //     Helper.Log(TAG + "LoadInterstitial adUnitId:" + interstitialAdUnit);
//            // }
//        }
//
//        
//        private void ShowInterstitialAdInternal(bool restart, string[] IDs , bool fiveAd)
//        {
//            // foreach (string interstitialAdUnit in IDs)
//            // {
//            //     if (!_loaded.ContainsKey(interstitialAdUnit) || !_loaded[interstitialAdUnit])
//            //     {
//            //         Helper.Log(TAG + "ShowInterstitialAd interstitialAdUnit: no interstitial:" + interstitialAdUnit);
//            //         continue;
//            //     }
//            //
//            //     _lastAdUnitId = interstitialAdUnit;
//            //     _adShowing = true;
//            //     _restartAds[interstitialAdUnit] = restart;
//            //
//            //     if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLogShow", false))
//            //     {
//            //         if (restart)
//            //         {
//            //             if (fiveAd)
//            //             {
//            //                 PlatformBridge.logESEvent("Show_Restart5s_Ad", true);
//            //             }
//            //             else
//            //             {
//            //                 PlatformBridge.logESEvent("Show_Restart_Ad", true);
//            //             }
//            //         }
//            //         else
//            //         {
//            //             if (fiveAd)
//            //             {
//            //                 PlatformBridge.logESEvent("Show_Interstitial5s_Ad", true);
//            //             }
//            //             else
//            //             {
//            //                 PlatformBridge.logESEvent("Show_Interstitial_Ad", true);
//            //             }
//            //         }
//            //     }
//            //
//            //     Helper.Log(TAG + "ShowInterstitialAd interstitialAdUnit:" + interstitialAdUnit);
//            //
//            //     MaxSdk.ShowInterstitial(interstitialAdUnit);
//            //
//            //     if (!_restartAds[interstitialAdUnit])
//            //     {
//            //         Task.Delay(50).ContinueWith(t => {
//            //             Helper.Log(TAG + "OnInterstitialClose SHOW PLAY AGAIN");
//            //         }, TaskScheduler.FromCurrentSynchronizationContext());
//            //
//            //     }
//            //     break;
//            // }
//        }
//
//        public void Show5sInterstitialAd(bool restart)
//        {
//            ShowInterstitialAdInternal(restart, _fiveIDs,true);
//        }
//
//        public void ShowInterstitialAd(bool restart)
//        {
//            ShowInterstitialAdInternal(restart, configIds,false);
//        }
//        
//        //
//        // private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//        // {
//        //     // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
//        //
//        //     Helper.Log(TAG + "OnInterstitialLoadedEvent adUnitId:" + adUnitId);
//        //     _loaded[adUnitId] = true;
//        //     _loading[adUnitId] = false;
//        //
//        //     if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLog", false))
//        //     {
//        //         PlatformBridge.logESEvent("Interstitial_Ad_Load_Finish", true,
//        //             JsonConvert.SerializeObject(new Dictionary<string, string> {{"result", "true"}},
//        //                 Formatting.Indented));
//        //     }
//        // }
//        
////         private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
////         {
////             // Interstitial ad failed to load 
////             // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
////
////             Helper.Log(TAG + "OnInterstitialLoadFailedEvent adUnitId:" + adUnitId + " errorMessage: " + errorInfo.Message + " errorCode: " + errorInfo.Code);
////             _loaded[adUnitId] = false;
////             _loading[adUnitId] = false;
////
////             if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Interstitial_Ad_Load_Finish", true,
////                     JsonConvert.SerializeObject(new Dictionary<string, string> {{"result", "false"}},
////                         Formatting.Indented));
////             }
////             
////         }
////
////         private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             Helper.Log(TAG + "OnInterstitialDisplayedEvent adUnitId:" + adUnitId);
////
//// //            AudioManager.Instance.pauseBgMusic();
////             ManagerAd.OnInterstitialAdShown();
////         }
////
////
////         private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
////         {
////             
////             Helper.Log(TAG + "OnInterstitialAdFailedToDisplayEvent adUnitId:" + adUnitId);
////
////             _loaded[adUnitId] = false;
////             
////             // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
////             LoadInterstitial();
////         }
////         
////         private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             // Interstitial ad revenue paid. Use this callback to track user revenue.
////             Debug.Log("Interstitial revenue paid");
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
////            AppLovinMediationAdapter.Instance.OnImpressionTrackedEvent(adUnitId, adInfo);
////         }
////         
////
////         private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             Helper.Log(TAG + "OnInterstitialClickedEvent adUnitId:" + adUnitId);
////
////             if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Interstitial_Ad_Clicked", true);
////             }
////         }
////
////         private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
////         {
////             Helper.Log(TAG + "OnInterstitialHiddenEvent adUnitId:" + adUnitId);
////             OnInterstitialClose(adUnitId);
////         }
////         
//        
//        
//        private bool Is5SAd(string adunit)
//        {
//            foreach (string id in _fiveIDs)
//            {
//                if (id.Equals(adunit))
//                {
//                    return true;
//                }
//            }
//
//            return false;
//        }
//        
//        private void OnInterstitialClose(string adUnitId)
//        {
//            if (!_adShowing)
//            {
//                return;
//            }
//
//            _loaded[adUnitId] = false;
//            _adShowing = false;
//
////            AudioManager.Instance.resumeBgMusic();
//            LoadInterstitial();
//            if (Is5SAd(adUnitId))
//            {
//                Load5Interstitial();
//            }
//
//
//            if (_restartAds.ContainsKey(adUnitId) && _restartAds[adUnitId])
//            {
//                Helper.Log(TAG + " On RestartAd Close " + adUnitId);
//                _restartAds[adUnitId] = false;
//            }
//            ManagerAd.OnInterstitialAdClose();
//        }
//
//        
//        public int InterstitialAdAvailableCount()
//        {
//            int count = InterstitialCountIntertal(configIds);
//            if (count <= 0)
//            {
//                LoadInterstitial();
//            }
//
//            return count;
//        }
//        
//        private void OnShowInterstitialAdError(object message)
//        {
//            if (!_adShowing)
//            {
//                return;
//            }
//            Helper.Log(TAG + "OnShowInterstitialAdError adUnitId:" + _lastAdUnitId + " msg: " + message);
//
//            OnInterstitialClose(_lastAdUnitId);
//
//            if (PlatformBridge.getConfigBoolean("Application.Interstitial.ESLog", false))
//            {
//                PlatformBridge.logESEvent("Show_Interstitial_Ad_Failed", true);
//            }
//        }
//        
//        private int InterstitialCountIntertal(string[] IDs)
//        {
//            var count = 0;
//            foreach (var interstitialAdUnit in IDs)
//            {
//                if (_loaded.ContainsKey(interstitialAdUnit) && _loaded[interstitialAdUnit])
//                {
//                    count += 1;
//                }
//            }
//
//            Helper.Log(TAG + "InterstitialAdAvailableCount :" + count);
//
//            return count;
//        }
//        
//        public int FiveSecInterstitialCount()
//        {
//            int count = InterstitialCountIntertal(_fiveIDs);
//            if (count <= 0)
//            {
//                Load5Interstitial();
//            }
//            Helper.Log(TAG + "InterstitialAdAvailableCount 5s :" + count);
//            return count;
//        }
//        
//        public void OnMainSceneOnRestart()
//        {
//            Helper.Log(TAG + "onMainSceneOnRestart");
//            
//            ManagerAd.OnMainSceneRestart();
//            
//            Task.Delay(1500).ContinueWith(t => OnShowInterstitialAdError("onMainSceneOnRestart"), 
//                TaskScheduler.FromCurrentSynchronizationContext());
//
//            if (Constant.BannerClickedData != "")
//            {
//                PlatformBridge.submitBannerReturnData(Constant.BannerClickedData, "bottom");
//                Helper.Log(TAG + "submitBannerReturnData");
//                Constant.BannerClickedData = "";
//            }
//        }
//        
//        
//        public bool IsInterstitialId(string id)
//        {
//            foreach (string adUnit in configIds)
//            {
//                if (adUnit.Equals(id))
//                {
//                    return true;
//                }
//            }
//
//            foreach (string adUnit in _fiveIDs)
//            {
//                if (adUnit.Equals(id))
//                {
//                    return true;
//                }
//            }
//
//            return false;
//        }
//    }
//
//}
//
//
