using System.Collections;
using Models;

namespace Manager
{
    public static class ManagerAd
    {
        public enum RewardVideoPlayType
        {
            SecondChance = 1,
            B421,
            B43221,
            B43221And1Hang,
            Prop1,
            Prop2,
            Prop3,
            Prop4,
            SpecialGold,
        }
        
        public enum InterstitialPlayType
        {
            Default = 1,
            SpecialGold,
            Restart
        }
        
        private static int _rewardVideoPlayType = 0;
        private static int _interstitialPlayType = 0;
        
        public static void InitData()
        {
            
        }

        public static bool HaveRewardAd()
        {
            return true;
        }

        public static void PlayRewardAd(int playType = (int)RewardVideoPlayType.SecondChance)
        {
            ManagerAudio.PauseBgm();
            _rewardVideoPlayType = playType;
        }

        public static void OnRewardAdClose(string result = "success")
        {
            switch (result)
            {
                case "success":
                    if (_rewardVideoPlayType == (int) RewardVideoPlayType.SecondChance)
                    {
                        Constant.GamePlayScript.SecondChanceResult(new Hashtable(){{"success", true}});
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.B421)
                    {
                        Constant.GamePlayScript.B421Result(new Hashtable(){{"success", true}});
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.B43221)
                    {
                        Constant.GamePlayScript.B43221Result(new Hashtable(){{"success", true}});
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.B43221And1Hang)
                    {
                        Constant.GamePlayScript.B43221And1HangResult(new Hashtable(){{"success", true}});
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.Prop1)
                    {
                        Player.UpdatePropNumById(Constant.Prop1, Player.GetPropNumById(Constant.Prop1) + 1);
                        Constant.GamePlayScript.UpdatePropInfo();
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.Prop2)
                    {
                        Player.UpdatePropNumById(Constant.Prop2, Player.GetPropNumById(Constant.Prop2) + 1);
                        Constant.GamePlayScript.UpdatePropInfo();
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.Prop3)
                    {
                        Player.UpdatePropNumById(Constant.Prop3, Player.GetPropNumById(Constant.Prop3) + 1);
                        Constant.GamePlayScript.UpdatePropInfo();
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.Prop4)
                    {
                        Player.UpdatePropNumById(Constant.Prop4, Player.GetPropNumById(Constant.Prop4) + 1);
                        Constant.GamePlayScript.UpdatePropInfo();
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.SpecialGold)
                    {
                        Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks, true);
                    }
                    break;
                case "fail":
                    if (_rewardVideoPlayType == (int) RewardVideoPlayType.SecondChance || _rewardVideoPlayType == (int) RewardVideoPlayType.B43221And1Hang)
                    {
                        Constant.GamePlayScript.SecondChanceResult();
                    } else if (_rewardVideoPlayType == (int) RewardVideoPlayType.SpecialGold)
                    {
                        Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks);
                    }
                    break;
            }
            
            ManagerAudio.ResumeBgm();

        }

        public static bool HaveInterstitialAd(bool isRestart = false)
        {
            return false;
        }

        public static void PlayInterstitialAd(int playType = (int) InterstitialPlayType.Default)
        {
            ManagerAudio.PauseBgm();
        }
        
        public static void OnInterstitialAdClose()
        {
            ManagerAudio.ResumeBgm();
            
            switch (_interstitialPlayType)
            {
                case (int) InterstitialPlayType.Default:
                    Constant.GamePlayScript.ShowGameOverDialog(false);
                    break;
                case (int) InterstitialPlayType.SpecialGold:
                    Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks, true);
                    break;
            }
        }

        public static void OnMainSceneRestart()
        {
        }

        public static void OnAdClk(int adType = 0)
        {
        }
    }
}
