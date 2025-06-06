//using BFF;
//using Models;
//using Platform;
//using UnityEngine;
//
//
//namespace H5Tracker
//{
//    public class H5TrackManager
//    {
//        private static readonly string TAG = "H5TrackManager-------";
//
//        private static H5TrackManager instance = null;
//        private static readonly object padlock = new object();
//
//        private bool _permissionGarented = false;
//        public bool AppsFlyerInit = false;
//
//        H5TrackManager()
//        {
//        }
//
//        public static H5TrackManager Instance
//        {
//            get
//            {
//                lock (padlock)
//                {
//                    if (instance == null)
//                    {
//                        instance = new H5TrackManager();
//                    }
//                    return instance;
//                }
//            }
//
//        }
//
//        public void InitAppsFlyer()
//        {
////             /* Mandatory - set your AppsFlyer’s Developer key. */
////             string appsflyerKey = PlatformBridge.getConfigString("libCommons.Analytics.FlyerKey", "");
////             if (string.IsNullOrEmpty(appsflyerKey)) 
////             {
////                 Debug.LogError("FlyerKey 读取失败，需要在yaml中进行配置");
////             }
////             else
////             {
////                 Debug.Log("SharpAppsflyer: -----------------> appsflyerApiKey: " + appsflyerKey);
////
////                 var AppID = PlatformBridge.getConfigString("libCommons.Analytics.AppleAppID", "");
//// #if UNITY_ANDROID
////                 AppID = PlatformBridge.getPackageName();
//// #endif
////                 
////
////                 GameObject consoleGO = GameObject.Find("BFAppsFlyer");
////                 if (consoleGO == null)
////                 {
////                     consoleGO = new GameObject("af_container");
////                 }
////
////                 BFAppsFlyer callback = consoleGO.GetComponent<BFAppsFlyer>();
////                 if (callback == null)
////                 {
////                     callback = consoleGO.AddComponent<BFAppsFlyer>();
////                 }
////                 
////                 
////                 
//// #if UNITY_IOS
////
//// //                StartCoroutine(RequestAuthorization());
//// //                Screen.orientation = ScreenOrientation.Portrait;
////                 if (PlatformBridge.trackingTransparencyStatus() == 2)
////                 {
////                     var time = PlatformBridge.getConfigInt("Application.ATTGuideAlert.AFWaitATTTime",60);
////                     if (time > 0)
////                     {
////                         AppsFlyeriOS.waitForATTUserAuthorizationWithTimeoutInterval(time);
////                     }
////
////                 }
//// #endif
////
////                 
////
////
//// #if LOG
////                 AppsFlyer.setIsDebug(true);
//// #endif
////                 
////                 AppsFlyer.initSDK(appsflyerKey, AppID, callback);
////                 AppsFlyer.setCustomerUserId(ESDataBase.GetUUID());
////                 AppsFlyer.startSDK();
////
////                 AppsFlyerInit = true;
////                 
////                 
////
////             
////   
////                 Helper.Log(TAG + "InitAppsFlyer: " + appsflyerKey);
////             }
//
//
//
//        }
//
//        
//        public void InitFlurry()
//        {
//            // string FLURRY_API_KEY = PlatformBridge.getConfigString("libCommons.Analytics.FlurryKey", "");
//            // new Flurry.Builder().WithCrashReporting(false)
//            //     .WithLogEnabled(true)
//            //     .WithLogLevel(Flurry.LogLevel.WARN)
//            //     .WithMessaging(true)
//            //     .Build(FLURRY_API_KEY);
//            //
//            // Helper.Log(TAG + "InitFlurry: " + FLURRY_API_KEY);
//            //
//            // _permissionGarented = true;
//        }
//
//        public bool IsGranted()
//        {
//            return _permissionGarented;
//        }
//
//    }
//}
