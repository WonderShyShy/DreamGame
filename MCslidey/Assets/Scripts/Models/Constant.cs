using Manager;
using UI;
using UnityEngine;

namespace Models
{
    public static class Constant
    {
        //屏幕滑动偏移系数
        public static float DragDelta = 1f;
        public static float ScreenWidth = Screen.width;
        public static float ScreenHeight = Screen.height;
        
        //不可变参数
        public const int FrameRate = 60;
        public const float FrameTime = 1.0f / 60;
        
        public const int Hang = 10;
        public const int Lie = 8;

        public static int BlockWidth = 120;
        public static int BlockHeight = 120;

        public static float WRatio = 1f;
        public static float HRatio = 1f;

        public static int BlockGroupEdgeLeft = -Lie * BlockWidth / 2;
        public static int BlockGroupEdgeBottom = -Hang * BlockHeight / 2;
        
        //道具ID
        public const string Prop1 = "prop1";//炸弹：目标块相邻所有块
        public const string Prop2 = "prop2";//火箭：某颜色所有块
        public const string Prop3 = "prop3";//锤子：4裂为4个1
        public const string Prop4 = "prop4";//魔法棒：4变成1个1
        
        //彩虹块出现概率
        public static bool SpecialRainbowSwitch = false;//彩色开关
        public static int BlockSpecialRate = 20;
        public static int BlockSpecialHangCountMin = 5;
        public static int BlockSpecialHangCountMax = 8;
        public static float SecondChanceClearEffTime = 0.7f;
        
        //其他特殊块出现规则
        public static bool SpecialBronzeSwitch = true;//青铜开关
        public static bool SpecialGoldSwitch = true;//金色开关
        public static string SpecialGoldEffWeight = "1:0:0";//金色块的三个效果随机权重（消除一种颜色，4变4个1，不生成新方块）
        public static int SpecialGoldEffNoNewBlocksTime = 10;//不生成新方块等待时间
        public static string SpecialBronzeRandom = "50,100,50";
        public static string SpecialGoldRandom = "100,150,200";
        public static string PreviousBlocks = "";
        public static bool SpecialGoldAdInterstitialSwitch = false;//消除金色块后展示插屏广告
        public static bool SpecialGoldAdRvAndInterSwitch = false;//消除金色块后，如果没有激励视频就用插屏
        public static bool SpecialGoldCountDownSwitch = false;//金色块倒计时开关
        public static int SpecialGoldCountDownNum = 15;//金色块倒计时时间
        public static bool SpecialGoldAdClear = false;//出发金块效果需要看视频开关
        
        //石头块相关参数
        public static bool StoneSwitch = false;//石头块功能开关
        public static int StoneScoreTimes = 4;//分数基础倍数
        public static int StoneScoreGenerate = 6000;//石头开始出现的分数
        public static int StoneGenerateHang = 10;//几行出一个石头
        public static int StoneMaxCount = 2;//场上最多的石头数

        //震动参数
        public static bool VibratorSwitch = false;//震动开关
        public static int VibratorTime = 300;//震动时间，ms
        public static int VibratorAmplitude = 200;//震动幅度，范围0-255
        
        //冰块相关
        public static bool IceBlockSwitch = true;//冰块开关
        
        //level相关
        public static bool LevelToStageSwitch = false;//level文案改为stage开关
        public static string LevelText = "Level ";//主界面level文案
        public static string LevelUpText = "LEVEL UP ";//升级特效level文案
        public static bool LevelRewardSwitch = false;//升级奖励开关
        public static bool LevelUpOtherEffSwitch = false;//升级时的其他特效

        public static string SceneVersion = "3";//场景版本
        public static string LevelUpEffVersion = "";//levelUp特效版本
        public static string LevelProgressVersion = "1";//levelProgress显示版本
        
        //是否已经超过设计长宽比
        public static bool IsDeviceSoHeight = false;

        //可配置参数
        public static float UpAnimTime = 0.2f;
        public static float DownAnimTime = 0.16f;
        public static float BlockRemoveTime = 0.3f;
        public static float SpecialClearTime = 0.7f;
        public static float SpecialGoldClearTime = 0.3f;
        public static float SpecialGoldClearIntervalTime = 0.1f;
        public static float SpecialEdgeClearTime = 0.5f;
        public static float ClearWaitTime = 0.25f;
        public static float IceTime = 0.2f;
        public static float ScoreUpdateDelayTime = 0.5f;
        public static float ScoreEffDelayTime = 0.6f;
        public static float ClearEffDelayTime = 0.12f;
        public static float ClearTipTimeMax = 7.0f;
        public static float GuideOneBlockTime = 0.75f;
        public static float GuideEdgeWaitTime = 0.1f;
        public static float DownEndWaitTime = 0.02f;
        public static float UpEndWaitTime = 0.03f;
        public static string GuideVersion = "";//新手引导版本
        public static bool B421Switch = false;
        public static int B421NotAddBlockStep = 2;
        public static bool B43221Switch = false;
        public static bool B43221And1HangSwitch = false;
        public static int B43221NotAddBlockStep = 2;
        public static int B43221RemoveSlideNumber = 3;//出现该功能后，3步之内不使用，则移除
        public static string B43221BlockPro = "2:1";//切割方块比例
        public static bool SecondChanceEnabled = true;//激励视频开关
        public static bool AchievementSwitch = false;//成就系统开关
        public static string ScoreToStar = "0,4000,12000";//分数评星
        public static int SecondChanceScore = 0;//二次机会出现分数条件

        public static bool ESLogUseNative = false;//是否从native层发送es数据

        public static bool ShowRemoveAd = false;//结算界面是否展示去掉广告按钮

        public static bool ShowSpecialGoldDialogAnim = true;//金色块弹窗里展示动态效果
        
        //adChance
        public static string RewardVideoType = "";
        //bannerClicked
        public static string BannerClickedData = "";
        
        public static int DifficultyLevel = 6;//游戏难度
        
        public static bool PropSwitch = false;//道具开关
        public static int PropUsedCount = 1;//单局中道具使用次数

        //AF数据
        public static int AF_PLAY_ROUND = 8;
        public static int AF_PLAY_DAY = 2;
        public static int AF_ROUND_OVER_SCORE = 10000;
        public static AFData AFData;

        //二次机会消除起始行和行数
        public static int SecondChanceHangStart = 3;
        public static int SecondChanceHangNum = 4;
        
        //空格概率
        public static readonly int[][] EmptyBlockRate = new int[3][];
        
        //游戏状态数据
        public static GameStatus GameStatusData = null;
        
        //RateDialog配置参数
        public static int RateRound = 5;
        public static int RateDeltaRound = 10;
        public static int RateMaxCount = 3;

        //脚本全局变量
        public static GamePlayDialog GamePlayScript;
        public static EffectController EffCtrlScript;
        public static AchievementTips AchievementScript;
        
        //topicID
        public static string TopicID = "topic-8a21ixizu";
        
        public static void InitData()
        {
            AFData = ManagerLocalData.GetTableData<AFData>(ManagerLocalData.AF) ?? new AFData();
            GameStatusData = ManagerLocalData.GetTableData<GameStatus>(ManagerLocalData.GAME_STATUS) ?? new GameStatus();

            var gamePlayObj = GameObject.Find("GamePlayDialog");
            GamePlayScript = gamePlayObj.GetComponent<GamePlayDialog>();
            EffCtrlScript = gamePlayObj.GetComponent<EffectController>();
            AchievementScript = gamePlayObj.GetComponent<AchievementTips>();
        }

        public static void UpdateBlockSize(int sizeWidth, int sizeHeight = 0)
        {
            if (sizeHeight == 0)
            {
                sizeHeight = sizeWidth;
            }

            WRatio = sizeWidth * 1f / BlockWidth;
            HRatio = sizeHeight * 1f / BlockHeight;
            
            BlockWidth = sizeWidth;
            BlockHeight = sizeHeight;
            BlockGroupEdgeLeft = -Lie * BlockWidth / 2;
            BlockGroupEdgeBottom = -Hang * BlockHeight / 2;
        }
    }
}
