//using System;
//using Manager;
//using Other;
//using Platform;
//using UnityEngine;
//
//namespace H5Ads
//{
//    public class RestartAd : MonoBehaviour
//    {
//        private const string Tag = "RestartAd ";
//        private bool _isSceneFirstStart = true;
//        private bool _onAdClosed = true;
//        private DateTime _onAdClosedTime = DateTime.Now;
//        private bool _restartAdSwitch;
//        private int _restartAdInterval;
//        private bool _isAdClk;
//        private bool _isReady;
//
//        private void Start()
//        {
//            // _restartAdSwitch = PlatformBridge.getConfigBoolean("Application.RestartAd.Enable", false);
//            // _restartAdInterval = PlatformBridge.getConfigInt("Application.RestartAd.Interval", 3000);
//
//            _restartAdSwitch = true;
//            _restartAdInterval = 3000;
//            
//            _isReady = true;
//        }
//
//        //热启动时的广告
//        public void OnMainSceneRestart()
//        {
//            if (!_isReady)
//            {
//                return;
//            }
//            
//            //游戏第一次启动时不生效
//            if (_isSceneFirstStart)
//            {
//                DebugEx.Log(Tag + "is scene first start");
//                _isSceneFirstStart = false;
//                return;
//            }
//
//            if (_isAdClk)
//            {
//                DebugEx.Log(Tag + "is ad clk");
//                _isAdClk = false;
//                return;
//            }
//            
//            if (_restartAdSwitch)
//            {
//                DebugEx.Log(Tag + "Restarted");
//                CheckShowRestartAd();
//            }
//        }
//
//        public void OnAdClk()
//        {
//            DebugEx.Log(Tag + "OnAdClk");
//            _isAdClk = true;
//        }
//
//        public void OnAdShown()
//        {
//            DebugEx.Log(Tag + "OnAdShown");
//            _onAdClosed = false;
//        }
//
//        public void OnAdClosed()
//        {
//            DebugEx.Log(Tag + "OnAdClosed");
//            _onAdClosedTime = DateTime.Now;
//            _onAdClosed = true;
//        }
//
//        private void CheckShowRestartAd()
//        {
//            if (_onAdClosed)
//            {
//                if (DateTime.Now.Subtract(_onAdClosedTime).TotalMilliseconds >= _restartAdInterval)
//                {
//                    DebugEx.Log(Tag + "Show RestartAd");
//                    if (ManagerAd.Have5sInterstitialAd(true))
//                    {
//                        ManagerAd.PlayRestartInterstitialAd();
//                    }
//                }
//            }
//        }
//    }
//}
