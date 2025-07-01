//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Manager;
//using Newtonsoft.Json;
//
//namespace Models
//{
//    public static class Statistics
//    {
//        public static void InitData()
//        {
//            
//        }
//        
//        public static void Send(string eventName, Hashtable data = null)
//        {
//            return;
//        }
//    
//        public static void SendAF(string eventName, Hashtable data = null)
//        {
//            return;
//        }
//
//        public static void SendES(string eventName, Hashtable data = null)
//        {
//            return;
//        }
//
//        public static void SendFirebase(string eventName)
//        {
//            return;
//        }
//        
//        public static void LogFirebaseEvent(string eventName, Dictionary<string, string> paras = null)
//        {
//            return;
//        
//        }
//        
//
//        public static void SendGameOverData()
//        {
//            return;
//        }
//
//
//        private static void SendLtInningTime()
//        {
//            return;
//        }
//
//        public static void SendRetention()
//        {
//            return;
//          
//        }
//
//        private static void SendLtRoundCount()
//        {
//            return;
//           
//            
//        }
//
//        private static void SendLtBesetScore()
//        {
//            return;
//            // bool status = Player.IsNewBestStatus();
//            // if (!status)
//            // {
//            //     return;
//            // }
//            // int bestRoundCount = ManagerLocalData.GetIntData(ManagerLocalData.NEW_BEST_ROUND);
//            // if (bestRoundCount == 1 && !Constant.AFData.bestscore_lt_1)
//            // {
//            //     SendAF("roundover_bestscore_1th");
//            //     SendFirebase("roundover_bestscore_1th");
//            //     Constant.AFData.bestscore_lt_1 = true;
//            //     ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
//            // }
//            // else if (bestRoundCount == 2 && !Constant.AFData.bestscore_lt_2)
//            // {
//            //     SendAF("roundover_bestscore_2th");
//            //     SendFirebase("roundover_bestscore_2th");
//            //     Constant.AFData.bestscore_lt_2 = true;
//            //     ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
//            // }
//            // else if (bestRoundCount == 5 && !Constant.AFData.bestscore_lt_5)
//            // {
//            //     SendAF("roundover_bestscore_5th");
//            //     SendFirebase("roundover_bestscore_5th");
//            //     Constant.AFData.bestscore_lt_5 = true;
//            //     ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
//            // }
//            // else if (bestRoundCount == 10 && !Constant.AFData.bestscore_lt_10)
//            // {
//            //     SendAF("roundover_bestscore_10th");
//            //     SendFirebase("roundover_bestscore_10th");
//            //     Constant.AFData.bestscore_lt_10 = true;
//            //     ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
//            // }
//            //
//            //
//            
//        }
//    }
//}
