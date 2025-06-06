//namespace Plugins
//{
//    public class AppLovinMediationAdapter
//    {
//        private static AppLovinMediationAdapter instance = null;
//        private static readonly object padlock = new object();
//
//        AppLovinMediationAdapter()
//        {
//
//        }
//
//        public static AppLovinMediationAdapter Instance
//        {
//            get
//            {
//                lock (padlock)
//                {
//                    if (instance == null)
//                    {
//                        instance = new AppLovinMediationAdapter();
//                    }
//                    return instance;
//                }
//            }
//
//        }
//
//        private void onSdkInitialized()
//        {
//        }
//        
//        
//         //Banner
//        public void ShowBanner()
//        {
//        }
//        
//
//        public void LoadInterstitial(string placementName = "", int loadCount = 1) {
//        }
//
//        public void ShowInterstitialAd(bool restart = false) {
//        }
//
//        public void Show5sInterstitialAd(bool restart = false)
//        {
//        }
//        
//        public int InterstitialAdAvailableCount()
//        {
//            return 1;
//        }
//
//        public int FiveSecInterstitialCount()
//        {
//            return 1;
//        }
//        
//
//        public void LoadRewardVideo(string placementName = "", int loadCount = 1) {
//        }
//
//        public void ShowRewardVideo() {
//        }
//        
//
//        public int RewardVideoAvailableCount() {
//            return 1;
//        }
//
//        public int RewardVideoAvailableCount(string placementName) {
//            return 1;
//        }
//
//        public void OnMainSceneOnRestart()
//        {
//        }
//
//        public void OnSplashFinish()
//        {
//        }
//
//        public void OnGuideComplete()
//        {
//        }
//
//        public void DestroyBanner()
//        {
//        }
//        
//    }
//
//}
//
