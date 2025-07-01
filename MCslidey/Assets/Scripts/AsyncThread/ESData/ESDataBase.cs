//using System;
//using System.Collections;
//using System.Runtime.InteropServices;
//using Models;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using UnityEngine;
//
//namespace BFF
//{
//    public class ESDataBase
//    {
//        private static AndroidJavaClass _androidNativePlugin = new AndroidJavaClass("h5adapter.CCNativeAPIProxy");
//        private static JObject _appsFlyerConversionData;
//        private static JObject _nativeData;
//        private static bool _wifiEnabled = false;
//        private static string _adId = "";
//        private static string _ip = "";
//        private static long _nowTimeStamp = 0;
//        private static string _networkStatus = "";
//        private static int _loginDay = 0;
//        private static DateTime _time1970 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
//        
//#if !UNITY_EDITOR && UNITY_ANDROID
//        private static string _appPlatform = "android";
//#elif !UNITY_EDITOR && UNITY_IOS
//        private static string _appPlatform = "iOS";
//#else
//        private static string _appPlatform = "";
//#endif
//
//#if !UNITY_EDITOR && UNITY_IOS
//        [DllImport("__Internal")]
//        private static extern string mGetNativeData();
//        
//        [DllImport("__Internal")]
//        private static extern string mGetAdId();
//#endif
//        
//        private static void GetNativeData()
//        {
//            if (_nativeData == null)
//            {
//#if !UNITY_EDITOR && UNITY_ANDROID
//                var nativeDataStr = _androidNativePlugin.CallStatic<string>("getNativeData");
//                _nativeData = JObject.Parse(nativeDataStr);
//#elif !UNITY_EDITOR && UNITY_IOS
//                _nativeData = JObject.Parse(mGetNativeData());
//#endif
//            }
//        }
//
//        //目前只有Android发送080广告事件
//        private static void GetNativeTimeStamp()
//        {
//#if !UNITY_EDITOR && UNITY_ANDROID
//            _nowTimeStamp = _androidNativePlugin.CallStatic<long>("getTimeStamp");
//#endif
//        }
//
//        private static void GetAdId()
//        {
//            if (_adId != "") return;
//#if !UNITY_EDITOR && UNITY_ANDROID
//            _adId = _androidNativePlugin.CallStatic<string>("getAdId");
//#elif !UNITY_EDITOR && UNITY_IOS
//            _adId = mGetAdId();
//#endif
//        }
//
//        public static string GetUUID()
//        {
//            GetAdId();
//            return _adId;
//        }
//        
//        private static void GetIp()
//        {
//            if (_ip != "") return;
//#if !UNITY_EDITOR && UNITY_ANDROID
//            _ip = _androidNativePlugin.CallStatic<string>("getIp");
//#endif
//        }
//
//        public static void SetAppsFlyerData(string conversionData)
//        {
//            GetAppsFlyerData();
//            if (_appsFlyerConversionData == null)
//            {
//                PlayerPrefs.SetString(ESDataConfig.AppsFlyerConversionDataKey, conversionData);
//                PlayerPrefs.Save();
//                _appsFlyerConversionData = JObject.Parse(conversionData);
//            }
//            else
//            {
//                var newData = JObject.Parse(conversionData);
//                var appsFlyerDataUpdate = false;
//                foreach (var data in newData)
//                {
//                    if (data.Value != null && (string) data.Value != "" && !data.Value.Equals(_appsFlyerConversionData[data.Key]))
//                    {
//                        appsFlyerDataUpdate = true;
//                        _appsFlyerConversionData[data.Key] = data.Value;
//                    }
//                }
//
//                if (appsFlyerDataUpdate)
//                {
//                    PlayerPrefs.SetString(ESDataConfig.AppsFlyerConversionDataKey,
//                        JsonConvert.SerializeObject(_appsFlyerConversionData));
//                    PlayerPrefs.Save();   
//                }
//            }
//        }
//
//        private static void GetAppsFlyerData()
//        {
//            if (_appsFlyerConversionData == null)
//            {
//                if (PlayerPrefs.HasKey(ESDataConfig.AppsFlyerConversionDataKey))
//                {
//                    _appsFlyerConversionData = JObject.Parse(PlayerPrefs.GetString(ESDataConfig.AppsFlyerConversionDataKey));
//                }
//            }
//        }
//
//        private static string GetAppsFlyerDataByKey(string k)
//        {
//            if (_appsFlyerConversionData != null && _appsFlyerConversionData.ContainsKey(k))
//            {
//                if (!string.IsNullOrEmpty((string) _appsFlyerConversionData[k]))
//                {
//                    return (string) _appsFlyerConversionData[k];
//                }
//            }
//
//            return _nativeData.ContainsKey(k) ? (string) _nativeData[k] : "";
//        }
//
//        private static void GetWifiEnabled()
//        {
//            _wifiEnabled = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
//        }
//
//        private static void GetNetworkStatus()
//        {
//            _networkStatus = Application.internetReachability == NetworkReachability.NotReachable
//                ? "no_netwrok"
//                : "connected";
//#if !UNITY_EDITOR && UNITY_ANDROID
//            _networkStatus = _androidNativePlugin.CallStatic<string>("getNetworkStatus");
//#endif
//        }
//
//        private static void GetLoginDay()
//        {
//            Player.AddLoginDay();
//            _loginDay = Player.GetLoginDay();
//        }
//
//        public static void UpdateBaseData()
//        {
//            GetNativeData();
//            GetNativeTimeStamp();
//            GetIp();
//            GetAdId();
//            GetAppsFlyerData();
//            GetWifiEnabled();
//            GetNetworkStatus();
//            GetLoginDay();
//        }
//        
//        public static Hashtable GenerateData(string h5Action, Hashtable content)
//        {
//            var cha = DateTime.Now - _time1970;
//            var curTimeStamp = (long) cha.TotalMilliseconds;
//            
//            var device = new Hashtable
//            {
//                {"wifi", _wifiEnabled},
//                {"language", _nativeData["language"]},
//                {"time-zone", DateTime.Now.ToString("%z")},
//                {"timestamp", curTimeStamp},
//                {"country", _nativeData["country"]},
//                {"os_version", _nativeData["sdk_int"]},
//            };
//            
//            var user = new Hashtable();
//            user.Add("media_source", GetAppsFlyerDataByKey("media_source"));
//            user.Add("campaign_id", GetAppsFlyerDataByKey("campaign_id"));
//            user.Add("campaign", GetAppsFlyerDataByKey("campaign"));
//            user.Add("agency", GetAppsFlyerDataByKey("agency"));
//            user.Add("adset", GetAppsFlyerDataByKey("af_adset"));
//            user.Add("adsetId", GetAppsFlyerDataByKey("adset_id"));
//            user.Add("site_id", GetAppsFlyerDataByKey("af_siteid"));
//            user.Add("af_channel", GetAppsFlyerDataByKey("af_channel"));
//            
//            user.Add("config_version", _nativeData["config_version"]);
//#if !UNITY_EDITOR && UNITY_ANDROID
//            user.Add("debug", _nativeData["debug"]);
//#endif
//            user.Add("segment", _nativeData["segment"]);
//            user.Add("user_id", _nativeData["user_id"]);
//            user.Add("install_type", GetAppsFlyerDataByKey("af_status"));
//            user.Add("ad_id", _adId);
//            user.Add("day", _loginDay);
//
//            var app = new Hashtable();
//            app.Add("network", _networkStatus);
//            app.Add("name", _nativeData["name"]);
//            app.Add("version", _nativeData["version"]);
//            app.Add("first_version", _nativeData["first_version"]);
//            app.Add("time", _nativeData["time"]);
//#if !UNITY_EDITOR && UNITY_ANDROID
//            app.Add("first_code", _nativeData["first_code"]);
//            app.Add("code", _nativeData["code"]);
//#endif
//            app.Add("type", _appPlatform);
//
//            var data = new Hashtable
//            {
//                {"device", device},
//                {"app", app},
//                {"user", user},
//                {"h5Action", h5Action}
//            };
//
//            if (content != null)
//            {
//                data.Add("h5Para", content);
//            }
//            
//            return data;
//        }
//
//        public static Hashtable GenerateAdData(string jsonData, string adChance, bool isBannerReturn)
//        {
//            var data = JsonConvert.DeserializeObject<Hashtable>(jsonData);
//
//            if (isBannerReturn)
//            {
//                long duration = 0;
//
//                data["event_name"] = "ad_return";
//                data["event_type"] = 4;
//                if (data.ContainsKey("event_time"))
//                {
//                    var nowTime = _nowTimeStamp;
//                    duration = nowTime - (long) data["event_time"];
//                    data["event_time"] = nowTime;
//                }
//                
//                if (data.ContainsKey("event_meta"))
//                {
//                    ((JObject) data["event_meta"])["duration"] = duration;
//                }
//            }
//
//            data["gdpr_status"] = _nativeData["gdpr_status"];
//            data["uuid"] = _nativeData["uuid"];
//            data["ip"] = _ip;
//            data["gaid"] = _adId;
//            data["customer_user_id"] = _adId;
//            data["network_type"] = _networkStatus;
//            data["time_zone"] = _nativeData["time_zone"];
//            data["app_bundle"] = _nativeData["app_bundle"];
//            data["app_version_name"] = _nativeData["version"];
//            data["app_version_code"] = _nativeData["code"];
//            data["country"] = _nativeData["country"];
//            data["platform"] = _appPlatform;
//            data["apiy"] = 1;
//            data["install_time"] = _nativeData["install_time"];
//            
//            if (data.ContainsKey("event_meta"))
//            {
//                ((JObject) data["event_meta"])["ad_chance_name"] = adChance;
//            }
//
//            return data;
//        }
//
//        public static Hashtable GenerateBaseData()
//        {
//            var baseData = new Hashtable();
//            baseData["gdpr_status"] = _nativeData["gdpr_status"];
//            baseData["gaid"] = _adId;
//            baseData["customer_user_id"] = _adId;
//            baseData["network_type"] = _networkStatus;
//            baseData["app_bundle"] = _nativeData["app_bundle"];
//            baseData["app_version_name"] = _nativeData["version"];
//            baseData["app_version_code"] = _nativeData["code"];
//            baseData["fingerprint"] = _nativeData["fingerprint"];
//            baseData["os_version"] = _nativeData["os_version"];
//            baseData["country"] = _nativeData["country"];
//            baseData["country_source"] = _nativeData["country_source"];
//            baseData["platform"] = _appPlatform;
//            baseData["device_band"] = _nativeData["device_band"];
//            baseData["device_model"] = _nativeData["device_model"];
//            baseData["language"] = _nativeData["language"];
//            baseData["sdk_int"] = _nativeData["sdk_int"];
//            baseData["uuid"] = _nativeData["uuid"];
//            baseData["time_zone"] = _nativeData["time_zone"];
//            baseData["event_time"] = _nowTimeStamp;
//            baseData["ip"] = _ip;
//            baseData["apiy"] = 1;
//            baseData["install_time"] = _nativeData["install_time"];
//            return baseData;
//        }
//    }
//}
