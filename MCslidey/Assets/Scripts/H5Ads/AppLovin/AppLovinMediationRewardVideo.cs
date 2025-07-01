//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Manager;
//using Models;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Platform;
//using UnityEngine;
//
//namespace Plugins
//{
//    public class AppLovinMediationRewardVideo
//    {
//        private static readonly string TAG = "AppLovinMediationRewardVideo-------";
//
//        [HideInInspector]
//        public string rewardType = "";
//
//        private readonly Dictionary<string, bool> _loading = new Dictionary<string, bool>();
//        private readonly Dictionary<string, bool> _loaded = new Dictionary<string, bool>();
//        private readonly Dictionary<string, int> _failCountByAdUnit = new Dictionary<string, int>();
//
//        private bool _hasInitialed;
//        private bool _adShowing = false;
//        private bool _isRewarded = false;
//        
//        private readonly string[] ids = { "23fb70ff7cadeec7" };
//
//        private string[] configIds = { };
////        private RewardVideoInfo _info;
//       
//
//        public AppLovinMediationRewardVideo() {
//
//            Helper.Log(TAG + "AppLovinMediation - RewardVideo");
//            
////             MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
////             MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedVideoFailedEvent;
////             MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedVideoShownEvent;
////             MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedVideoClickedEvent;
////             MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
////             MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedVideoClosedEvent;
////             MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedVideoFailedToPlayEvent;
////             MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
////             
//// //            _info = new RewardVideoInfo();
//
//        }
//
//        public void OnSdkInitializedEvent() {
//            _hasInitialed = true;
//
//            LoadRewardedVideoPlugins();
//        }
//
//        private void LoadRewardedVideoPlugins() {
//            if (!_hasInitialed) {
//                return;
//            }
//
//            Helper.Log(TAG + "loadRewardedVideoPlugins");
//            string original = PlatformBridge.getConfigList("Application.AppLovinUnit.Reward");
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
//            foreach (var adUnit in configIds) {
//                Helper.Log(TAG + "AdUnit:" + adUnit);
//            }
//
//
//            Task.Delay(1200).ContinueWith(t => LoadRewardVideo(), TaskScheduler.FromCurrentSynchronizationContext());
//        }
//
//        public void LoadRewardVideo() {
//
//            Helper.Log(TAG + "LoadRewardVideo");
//            foreach (var rewardedVideoAdUnit in configIds) {
//                CheckToLoadAds(rewardedVideoAdUnit, "", true);
//            }
//        }
//
//
////         private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
////         {
////             _loaded[adUnitId] = true;
////             _loading[adUnitId] = false;
////             _failCountByAdUnit[adUnitId] = 0;
////
////             if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Reward_Video_Ad_Load_Finish", true,
////                     JsonConvert.SerializeObject(new Dictionary<string, string> {{"result", "true"}},
////                         Formatting.Indented));
////             }
////             
////             Helper.Log(TAG + "OnRewardedVideoLoadedEvent adUnitId:" + adUnitId);
////
//// //            _info.OnRewardedAdLoadedEvent(adUnitId, adInfo);
////
////         }
////
////         private void OnRewardedVideoFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
////             _loaded[adUnitId] = false;
////             _loading[adUnitId] = false;
////
////             var failCount = _failCountByAdUnit.ContainsKey(adUnitId) ? _failCountByAdUnit[adUnitId] : 0;
////             failCount++;
////             _failCountByAdUnit[adUnitId] = failCount;
////
////             Helper.Log(TAG + "OnRewardedVideoFailedEvent adUnitId:" + adUnitId + " errorMsg:" + errorInfo.Message);
////             if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Reward_Video_Ad_Load_Finish", true,
////                     JsonConvert.SerializeObject(new Dictionary<string, string> {{"result", "false"}},
////                         Formatting.Indented));
////             }
////
////             CheckToLoadAds(adUnitId, "onAdLoadFail", false);
////         }
////
////         /// <summary>
////         /// 追踪广告收入 
////         /// </summary>
////         /// <param name="adUnitId"></param>
////         /// <param name="adInfo"></param>
////         private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
////             _loaded[adUnitId] = false;
////             Helper.Log(TAG + "OnRewardedAdRevenuePaidEvent adUnitId:" + adUnitId);
////
////             // Rewarded ad revenue paid. Use this callback to track user revenue.
////             Debug.Log("Rewarded ad revenue paid");
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
////
////             AppLovinMediationAdapter.Instance.OnImpressionTrackedEvent(adUnitId, adInfo);
////         }
////
////         public void ShowRewardVideo() {
////             foreach (string rewardedVideoAdUnit in configIds) {
////                 if (!_loaded.ContainsKey(rewardedVideoAdUnit) || !_loaded[rewardedVideoAdUnit]) {
////                     Helper.Log(TAG + "ShowRewardVideo adUnitId: no reward" + rewardedVideoAdUnit);
////                     continue;
////                 }
////
////                 _adShowing = true;
////                 _isRewarded = false;
////                 MaxSdk.ShowInterstitial(rewardedVideoAdUnit);
////
////                 if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLogShow", false))
////                 {
////                     PlatformBridge.logESEvent("Show_Reward_Video", true);
////                 }
////
////                 Helper.Log(TAG + "ShowRewardVideo adUnitId:" + rewardedVideoAdUnit);
////                 break;
////             }
////         }
////
////         private void OnRewardedVideoShownEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
////             Helper.Log(TAG + "OnRewardedVideoShownEvent adUnitId:" + adUnitId);
////             _adShowing = true;
////             _isRewarded = false;
//// //            AudioManager.Instance.pauseBgMusic();
////             ManagerAd.OnRewardAdShown();
////             
//// //            _info.OnRewardedVideoShownEvent(adUnitId, adInfo);
////         }
////
////         private void OnRewardedVideoFailedToPlayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) {
////             _loaded[adUnitId] = false;
////             _adShowing = false;
////             OnRewarded(false);
////             Helper.Log(TAG + "OnRewardedVideoFailedToPlayEvent adUnitId:" + adUnitId + " errorMsg:" + errorInfo.Message + " errorCode: " + errorInfo.Code);
////         }
////
////         private void OnRewardedVideoClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
////             Helper.Log(TAG +"OnRewardedVideoClickedEvent adUnitId:" + adUnitId);
////             
////             if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Reward_Video_Clicked", true);
////             }
////
//// //            _info.OnRewardedVideoClickedEvent(adUnitId, adInfo);
////         }
////
////         private void OnRewardedVideoClosedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
////             Helper.Log(TAG + "OnRewardedVideoClosedEvent adUnitId:" + adUnitId);
////
////             _loaded[adUnitId] = false;
////
//// //            AudioManager.Instance.resumeBgMusic();
////
////             Task.Delay(150).ContinueWith(t => OnRewarded(false), TaskScheduler.FromCurrentSynchronizationContext());
////
////             if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
////             {
////                 PlatformBridge.logESEvent("Reward_Video_Closed", true);
////             }
////
////             CheckToLoadAds(adUnitId, "onAdClosed", false);
////             _adShowing = false;
////         }
////
////         private void OnRewardedVideoReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) {
////             Helper.Log(TAG + "OnRewardedVideoReceivedRewardEvent adUnitId:" + adUnitId + " reward:" + reward.Label + " Revenue:" + adInfo.Revenue);
////
//// //            amount = 1;
////             OnRewarded(true);
////
//// //            if (amount > 0)
//// //            {
////                 if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
////                 {
////                     PlatformBridge.logESEvent("Reward_Video_Rewarded", true);
////                 }
//// //            }
////         }
//
//        private void sendRewardVideoAF()
//        {
//            return;
//            int showRewardVideoCount = ManagerLocalData.GetIntData(ManagerLocalData.SHOW_REWARD_VIDEO_SUCCESS);
//            showRewardVideoCount++;
//            ManagerLocalData.SetIntData(ManagerLocalData.SHOW_REWARD_VIDEO_SUCCESS, showRewardVideoCount);
//            if (showRewardVideoCount == 1)
//            {
//                Statistics.SendAF("rewardvideo_1th");
//                Statistics.SendFirebase("rewardvideo_1th");
//            }
//            else if (showRewardVideoCount == 3)
//            {
//                Statistics.SendAF("rewardvideo_3th");
//                Statistics.SendFirebase("rewardvideo_3th");
//            }
//            else if (showRewardVideoCount == 5)
//            {
//                Statistics.SendAF("rewardvideo_5th");
//                Statistics.SendFirebase("rewardvideo_5th");
//            }
//            else if (showRewardVideoCount == 10 )
//            {
//                Statistics.SendAF("rewardvideo_10th");
//                Statistics.SendFirebase("rewardvideo_10th");
//            }
//        }
//        
//
//        private void OnRewarded(bool reward)
//        {
//
//            Helper.Log(TAG + "IsRewarded: " + _isRewarded + " reward: " + reward);
//
//
//            if (!_isRewarded) {
//                _isRewarded = true;
//                Helper.Log(TAG + "OnRewarded:" + reward);
////                switch (rewardType)
////                {
////                    case "revoke":
//////                        Revoke.Instance.revoke(reward);
////                        break;
////                    case "secondChance":
//////                        SecondChance.Instance.isContinue(reward);
////                        break;
////                }
//
//                if (reward)
//                {
//                    ManagerAd.OnRewardAdClose();
//                    sendRewardVideoAF();
//                }
//                else
//                {
//                    ManagerAd.OnRewardAdClose("fail");
//                }
//            }
//
//            
//        }
//
//        public int RewardVideoAvailableCount() {
//            int count = 0;
//            foreach (string rewardedVideoAdUnit in configIds) {
//                if (_loaded.ContainsKey(rewardedVideoAdUnit) && _loaded[rewardedVideoAdUnit]) {
//                    count += 1;
//                } else {
//                    CheckToLoadAds(rewardedVideoAdUnit, "onGetAvailableAdsCount", false);
//                }
//            }
//
//            return count;
//        }
//
//        private void CheckToLoadAds(string adUnit, string loadReason, bool loadImmediately) {
//            if (_loading.ContainsKey(adUnit) && _loading[adUnit]) {
//                Helper.Log(TAG + "adUnit:" + adUnit + " already loading.");
//                return;
//            }
//
//            if (_loaded.ContainsKey(adUnit) && _loaded[adUnit]) {
//                Helper.Log(TAG + "adUnit:" + adUnit + " already loaded.");
//                return;
//            }
//
//            if (!loadImmediately) {
//                var failCount = _failCountByAdUnit.ContainsKey(adUnit) ? _failCountByAdUnit[adUnit] : 0;
//                if (failCount == 0) {
//                    Helper.Log(TAG + "adUnit:" + adUnit + " try to load ads.");
//                    LoadRewardVideo(adUnit, loadReason);
//                } else {
//                    if (failCount > 30) {
//                        failCount = 0;
//                        _failCountByAdUnit[adUnit] = failCount;
//                    }
//
//                    long loadDelaySeconds = 10 * (failCount / 3 + 1);
//                    Helper.Log(TAG + "adUnit:" + adUnit + " failCount:" + failCount + " loadDelaySeconds:" + loadDelaySeconds);
//                    LoadRewardVideoWithDelay(adUnit, loadDelaySeconds, loadReason);
//                }
//            } else {
//                Helper.Log(TAG + "adUnit:" + adUnit + " try to load ads immediately.");
//                LoadRewardVideo(adUnit, loadReason);
//            }
//        }
//
//        private void LoadRewardVideoWithDelay(string adUnit, long delaySeconds, string loadReason) {
//            Helper.Log(TAG + "Setting timer for , delay: " + delaySeconds + " loadReason:" + loadReason);
//
//            Task.Delay((int)delaySeconds * 1000).ContinueWith(t => CheckToLoadAds(adUnit, loadReason, true), 
//            TaskScheduler.FromCurrentSynchronizationContext());
//
//           
//        }
//
//        private void LoadRewardVideo(string rewardedVideoAdUnit, string loadReason) {
//            // if (!_hasInitialed) {
//            //     return;
//            // }
//            //
//            // if (_loaded.ContainsKey(rewardedVideoAdUnit) && _loaded[rewardedVideoAdUnit]) {
//            //     Helper.Log(TAG + "LoadRewardVideo. adUnitId:" + rewardedVideoAdUnit + " has loaded");
//            //     return;
//            // }
//            //
//            // if (_loading.ContainsKey(rewardedVideoAdUnit) && _loading[rewardedVideoAdUnit]) {
//            //     Helper.Log(TAG + "LoadRewardVideo. adUnitId:" + rewardedVideoAdUnit + " is loading");
//            //     return;
//            // }
//            //
//            //
//            // _loading[rewardedVideoAdUnit] = true;
//            //
//            // MaxSdk.LoadInterstitial(rewardedVideoAdUnit);
//            // Helper.Log(TAG + "LoadRewardVideo adUnitId:" + rewardedVideoAdUnit + " loadReason:" + loadReason);
//            //
//            // if (PlatformBridge.getConfigBoolean("Application.RewardVideo.ESLog", false))
//            // {
//            //     PlatformBridge.logESEvent("Load_Reward_Video", true);
//            // }
//        }
//
//        public void OnMainSceneOnRestart()
//        {
//            Helper.Log(TAG + "onMainSceneOnRestart");
//            if (_adShowing)
//            {
//                _adShowing = false;
////                OnRewarded(false);
//            }
//        }
//
//        public void AssignRewardType(string type)
//        {
//            rewardType = type;
//        }
//
//        public bool IsRewardId(string id)
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
//    }
//
//}
//
//
