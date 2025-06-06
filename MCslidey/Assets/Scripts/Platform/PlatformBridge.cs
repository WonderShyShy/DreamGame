//using System;
//using System.Runtime.InteropServices;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using BFF;
//using Mono.Data.Sqlite;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Manager;
//using Models;
//using Other;
//using Plugins;
//using UnityEngine;
//
//namespace Platform
//{
//    public static class PlatformBridge
//    {
//
//        static AppLovinMediationAdapter adapter = null;
//
//        public static string notifyInited()
//        {
//
//            return "";
//        }
//
//        public static void InitAppLovinAdapter()
//        {
//
//            if (adapter == null)
//            {
//                adapter = AppLovinMediationAdapter.Instance;
//            }
//
//        }
//
//        public static void OnMainSceneOnRestart()
//        {
//            adapter?.OnMainSceneOnRestart();
//        }
//
//        public static string getCustomUserID()
//        {
//            return "";
//        }
//
//        public static string getDeviceId()
//        {
//            return "";
//        }
//
//        public static void gotoMarket()
//        {
//            return;
//        }
//
//        public static void onFinish()
//        {
//            
//        }
//
//        public static void showToast(string toast)
//        {
//
//        }
//
//        public static void isBGMMuted(bool mute)
//        {
//
//        }
//
//        public static void shareOverFacebook()
//        {
//
//        }
//
//        //config
//
//        public static string getConfigList(string keyPath)
//        {
//            return "";
//        }
//
//        public static string getConfigMap(string keyPath)
//        {
//            return "";
//        }
//
//        public static int getConfigInt(string keyPath, int defaultValue)
//        {
//            return defaultValue;
//        }
//
//        public static float getConfigFloat(string keyPath, float defaultValue)
//        {
//            return defaultValue;
//        }
//
//        public static string getConfigString(string keyPath, string defaultValue)
//        {
//            return defaultValue;
//        }
//
//
//        public static bool getConfigBoolean(string keyPath, bool defaultValue)
//        {
//            return defaultValue;
//        }
//
//        public static string getPackageName(string name = "")
//        {
//            return name;
//        }
//
//        public static long GetEnterGameTime()
//        {
//            return 0;
//        }
//
//        public static int GetBannerHeight()
//        {
//            var result = 150;
//            return result;
//        }
//
//
//        public static void showInterstitialAd(bool restart = false)
//        {
//        }
//
//        public static void show5sInterstitialAd(bool restart = false)
//        {
//        }
//
//        public static void loadInterstitialAd()
//        {
//        }
//
//        public static bool FiveSecInterstitialCount()
//        {
//            if (adapter == null) return false;
//            return adapter.FiveSecInterstitialCount() > 0;
//        }
//
//        public static bool isInterstitialReady()
//        {
//            if (adapter == null) return false;
//            return adapter.InterstitialAdAvailableCount() > 0;
//        }
//
//        public static void showRewardVideoAd()
//        {
//            adapter?.ShowRewardVideo();
//        }
//
//        public static void loadRewardVideoAd()
//        {
//            adapter?.LoadRewardVideo();
//        }
//
//        public static bool isRewardVideoReady()
//        {
//            if (adapter == null) return false;
//            return adapter.RewardVideoAvailableCount() > 0;
//        }
//
//
//        //广告结束
//        public static void OnAppsFlyerTrackerCallback(string conversion)
//        {
//
//        }
//
//        //记录事件
//        public static void logEvent(string eventName, string arg1 = "")
//        {
//            return;
//        }
//
//        public static void logESEvent(string eventName, string arg1 = "")
//        {
//            return;
//        }
//
//        public static void logESEvent(string eventName, bool enableFabric, string arg1 = "")
//        {
//            return;
//        }
//
//        public static void logAFEvent(string eventName, string arg1 = "")
//        {
//            return;
//        }
//
//        //震动 时间：毫秒单位  ， 震动幅度 0 - 255
//        public static void doRibrator(int time, int amplitude)
//        {
//        }
//
//        public enum eVibratorMode
//        {
//            remove = 0,
//            combo = 1,
//            special_gold = 2,
//            special_blue_select = 3,
//            special_blue_split = 4,
//            bestscore_ingame = 5,
//            bestscore_roundover = 6,
//            
//        }
//
//        public static void vibratorStart(eVibratorMode mode)
//        {
//
//        }
//
//        public static void vibratorStop()
//        {
//
//        }
//
//        public static string getESIDandSwitchFlag()
//        {
//            return "|off";
//        }
//
//        public static string getSystemApiLevel()
//        {
//            return "";
//        }
//
//        public static string networkStatus()
//        {
//            return "";
//        }
//
//        public static bool isFirstDay()
//        {
//            return true;
//        }
//
//        public static int trackingTransparencyStatus()
//        {
//                return 0;
//        }
//
//        public static void submitAdData(string json, string adChance)
//        {
//
//        }
//
//        public static void submitBannerReturnData(string json, string adChance)
//        {
//
//        }
//
//        public static void submitBaseData()
//        {
//
//        }
//
//        //打开隐私政策网页
//        public static void ShowPrivacy()
//        {
//
//        }
//
//    }
//}
//
