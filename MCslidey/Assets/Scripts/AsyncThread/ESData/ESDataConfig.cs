//namespace BFF
//{
//    public class ESDataConfig
//    {
//        public static string esTypeGame = "game";
//        public static string esTypeAd = "ad";
//        private static string _sigKey = "K1267937";
//        private static string _hashKey = "-:7kf]<*zU`=zYyw,=uh4<hfPOcPC^(x";
//        private static string _esUri = "https://h5game.api-alliance.com/client_report";
//        
//        //game事件
//        private static int _esRecordSize = 2048 * 10;
//        private static int _esRecordMaxCount = 100;
//        private static int _esMaxSendCount = 20;
//        private static int _esMinSendCount = 3;
//        
//        //ad事件
//        private static int _esAdRecordSize = 2048 * 10;
//        private static int _esAdRecordMaxCount = 100;
//        private static int _esAdMaxSendCount = 20;
//        private static int _esAdMinSendCount = 10;
//
//        public static string AppsFlyerConversionDataKey = "AppsFlyerConversionData";
//        
//        #region esLogConfig
//        
//        public static string GetESSigKey()
//        {
//            return _sigKey;
//        }
//
//        public static string GetESHashKey()
//        {
//            return _hashKey;
//        }
//
//        public static string GetESUri()
//        {
//            return _esUri;
//        }
//
//        public static string GetESFileName(string esType)
//        {
//            if (esType == esTypeAd)
//            {
//                return "es_ad.dat";
//            }
//            return "es.dat";
//        }
//
//        public static int GetESRecordSize(string esType)
//        {
//            if (esType == esTypeAd)
//            {
//                return _esAdRecordSize;
//            }
//            return _esRecordSize;
//        }
//
//        public static int GetESRecordMaxCount(string esType)
//        {
//            if (esType == esTypeAd)
//            {
//                return _esAdRecordMaxCount;
//            }
//            return _esRecordMaxCount;
//        }
//
//        public static int GetESMaxSendCount(string esType)
//        {
//            if (esType == esTypeAd)
//            {
//                return _esAdMaxSendCount;
//            }
//            return _esMaxSendCount;
//        }
//
//        public static int GetESMinSendCount(string esType)
//        {
//            if (esType == esTypeAd)
//            {
//                return _esAdMinSendCount;
//            }
//            return _esMinSendCount;
//        }
//        
//        #endregion
//    }
//}
//
