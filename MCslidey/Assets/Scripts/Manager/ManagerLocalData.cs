using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Models;
//using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Other;
using UnityEngine;

namespace Manager
{
    public static class ManagerLocalData
    {
        public static void InitData()
        {
//            //判断数据升级是否完成
//            if (HaveData(COCOS_2_UNITY) && (GetStringData(COCOS_2_UNITY) == "done" || GetStringData(COCOS_2_UNITY) == "err"))
//            {
//                return;
//            }
//            
//            var conn = "";
//            //数据升级
//                
//            //Editor平台
//            if (File.Exists("/Users/yongfu.su/Workspace/jsb.sqlite"))
//            {
//                DebugEx.Log("Editor平台");
//                conn = "URI=file:/Users/yongfu.su/Workspace/jsb.sqlite";
//            }
//            
//            //Android平台
//            if (File.Exists("/data/user/0/com.sportbrain.jewelpuzzle/databases/jsb.sqlite")){
//                DebugEx.Log("Android平台");
//                conn = "URI=file:/data/user/0/com.sportbrain.jewelpuzzle/databases/jsb.sqlite";
//            }
//            
//            //iOS平台
//            if (File.Exists(Application.persistentDataPath + "/" + "jsb.sqlite"))
//            {
//                DebugEx.Log("iOS平台");
//                conn = "data source=" + Application.persistentDataPath + "/" + "jsb.sqlite";
//            }
//            
//            if (conn == "") return;
//
//            //如果数据升级失败，则只保留最高分和当前分
//            var bestScore = 0;
//            
//            try
//            {
//                var dbconn = new SqliteConnection(conn);
//                dbconn.Open();
//                
//                //优先获取最高分
//                var cmd1 = dbconn.CreateCommand();
//                cmd1.CommandText = "SELECT key,value FROM data";
//                var reader1 = cmd1.ExecuteReader();
//                while (reader1.Read())
//                {
//                    var key = reader1.GetString(0);
//                    var value = reader1.GetString(1);
//                    if (key == "JEWEL_BLAST_SCORE_BEST")
//                    {
//                        DebugEx.Log("先保证获取最高分");
//                        value = Encoding.UTF8.GetString(Convert.FromBase64String(value.Replace(".", "=")));
//                        bestScore = int.Parse(value);
//                    }
//                }
//                reader1.Close();
//                reader1 = null;
//                cmd1.Dispose();
//                cmd1 = null;
//
//                var dbcmd = dbconn.CreateCommand();
//                dbcmd.CommandText = "SELECT key,value FROM data";
//                var reader = dbcmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    var key = reader.GetString(0);
//                    var value = reader.GetString(1);
//                    if (key != "HotUpdateSearchPaths")
//                    {
//                        value = Encoding.UTF8.GetString(Convert.FromBase64String(value.Replace(".", "=")));
//                        DebugEx.Log(key, value);
//                    }
//                    switch (key)
//                    {
//                        case "JEWEL_BLAST_BLOCKS_DATA":
//                            value = value.Replace("null", "[0, 0, 0, 0, 0, 0]");
//                            var tmpBlocksData = JsonConvert.DeserializeObject<int[][]>(value);
//                            var blocksData = new int[Constant.Lie * Constant.Hang][];
//                            for (var i = 0; i < blocksData.Length; ++i)
//                            {
//                                if (i < tmpBlocksData.Length)
//                                {
//                                    var data = tmpBlocksData[i];
//                                    if (data[(int) Blocks.Key.Length] != 0)
//                                    {
//                                        var newData = new int[Enum.GetNames(typeof(Blocks.Key)).Length];
//                                        for (var j = 0; j < data.Length; ++j)
//                                        {
//                                            newData[j] = data[j];
//                                        }
//                                        blocksData[i] = newData;
//                                    }
//                                    else
//                                    {
//                                        blocksData[i] = new int[Enum.GetNames(typeof(Blocks.Key)).Length];
//                                    }
//                                }
//                                else
//                                {
//                                    blocksData[i] = new int[Enum.GetNames(typeof(Blocks.Key)).Length];
//                                }
//                            }
//                            SetTableData(BLOCKS_DATA, blocksData);
//                            break;
//                        case "JEWEL_BLAST_BLOCKS_GUIDE"://新手引导数据逻辑优化，不再需要这项目数据
//                            break;
//                        case "JEWEL_BLAST_AF":
//                            SetStringData(AF, value);
//                            break;
//                        case "AF_PLAY_ROUND":
//                            SetStringData(AF_PLAY_ROUND, value);
//                            break;
//                        case "AF_ROUND_OVER_SCORE":
//                            SetStringData(AF_ROUND_OVER_SCORE, value);
//                            break;
//                        case "AF_PLAY_DAY":
//                            SetStringData(AF_PLAY_DAY, value);
//                            break;
//                        case "JEWEL_BLAST_ES":
//                            SetStringData(GAME_STATUS, value);
//                            break;
//                        case "JEWEL_BLAST_GUIDE":
//                            SetStringData(GUIDE, value);
//                            break;
//                        case "JEWEL_BLAST_LOGIN":
//                            SetStringData(LOGIN, value);
//                            break;
//                        case "JEWEL_BLAST_MUSIC_ON_OFF":
//                            SetStringData(MUSIC_ON_OFF, value);
//                            break;
//                        case "JEWEL_BLAST_MUSIC_VOLUME":
//                            SetFloatData(MUSIC_VOLUME, float.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_SOUND_ON_OFF":
//                            SetStringData(SOUND_ON_OFF, value);
//                            break;
//                        case "JEWEL_BLAST_SOUND_VOLUME":
//                            SetFloatData(SOUND_VOLUME, float.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_NEW_BEST_STATUS":
//                            SetStringData(NEW_BEST_STATUS, value);
//                            break;
//                        case "JEWEL_BLAST_NO_NEW_BEST_ROUND":
//                            SetIntData(NO_NEW_BEST_ROUND, int.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_RATE":
//                            SetStringData(RATE, value);
//                            break;
//                        case "JEWEL_BLAST_SCORE_CUR":
//                            SetIntData(SCORE_CUR, int.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_SCORE_BEST":
//                            SetIntData(SCORE_BEST, int.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_TOTAL_SCORE":
//                            SetIntData(TOTAL_SCORE, int.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_TOTAL_ROUND":
//                            SetIntData(TOTAL_ROUND, int.Parse(value));
//                            break;
//                        case "JEWEL_BLAST_SECOND_CHANCE":
//                            SetStringData(SECONDCHANCE_USED, value);
//                            break;
//                        case "JEWEL_BLAST_ICE_TIP_GUIDE":
//                            SetIntData(ICE_TIP_GUIDE, int.Parse(value));
//                            break;
//                    }
//                }
//                
//                reader.Close();
//                reader = null;
//                dbcmd.Dispose();
//                dbcmd = null;
//                dbconn.Close();
//                dbconn = null;
//             
//                //数据升级完成，标记done
//                SetStringData(COCOS_2_UNITY, "done");
//                SaveData();
//            }
//            catch (Exception e)
//            {
//                DeleteAll();
//                //数据升级失败，标记err
//                SetStringData(COCOS_2_UNITY, "err");
//                //保留当前分
//                SetIntData(SCORE_CUR, 0);
//                //保留最高分
//                SetIntData(SCORE_BEST, bestScore);
//                //跳过新手引导
//                var guideData = new GuideData {step = 4, end = true};
//                SetTableData(GUIDE, guideData);
//                SaveData();
//            }
//            
//            try
//            {
//                //Editor平台
//                if (File.Exists("/Users/yongfu.su/Workspace/jsb.sqlite"))
//                {
//                    File.Delete("/Users/yongfu.su/Workspace/jsb.sqlite");
//                }
//                    
//                //Android平台
//                if (File.Exists("/data/user/0/com.sportbrain.jewelpuzzle/databases/jsb.sqlite"))
//                {
//                    File.Delete("/data/user/0/com.sportbrain.jewelpuzzle/databases/jsb.sqlite");
//                }
//                    
//                //iOS平台
//                if (File.Exists(Application.persistentDataPath + "/" + "jsb.sqlite"))
//                {
//                    File.Delete(Application.persistentDataPath + "/" + "jsb.sqlite");
//                }
//            }
//            catch (Exception e)
//            {
//                //文件删除失败
//            }
        }
        
        public static void SetIntData(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetIntData(string key)
        {
            return !PlayerPrefs.HasKey(key) ? 0 : PlayerPrefs.GetInt(key);
        }

        public static void SetFloatData(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloatData(string key)
        {
            return !PlayerPrefs.HasKey(key) ? 0.0f : PlayerPrefs.GetFloat(key);
        }

        public static void SetStringData(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static string GetStringData(string key)
        {
            return !PlayerPrefs.HasKey(key) ? "" : PlayerPrefs.GetString(key);
        }

        public static void SetTableData<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetTableData<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));
        }

        public static void ClearData(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return;
            PlayerPrefs.DeleteKey(key);
        }

        public static bool HaveData(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void SaveData()
        {
            PlayerPrefs.Save();
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        //数据key
        public const string BLOCKS_DATA = "BLOCKS_DATA";
        public const string SCORE_CUR = "SCORE_CUR";
        public const string SCORE_BEST = "SCORE_BEST";
        public const string LEFT_INNING_TIME = "LEFT_INNING_TIME";
        public const string TOTAL_SCORE = "TOTAL_SCORE";
        public const string TOTAL_ROUND = "TOTAL_ROUND";
        public const string MUSIC_ON_OFF = "MUSIC_ON_OFF";
        public const string MUSIC_VOLUME = "MUSIC_VOLUME";
        public const string SOUND_ON_OFF = "SOUND_ON_OFF";
        public const string SOUND_VOLUME = "SOUND_VOLUME";
        public const string NEW_BEST_STATUS = "NEW_BEST_STATUS";
        public const string NO_NEW_BEST_ROUND = "NO_NEW_BEST_ROUND";
        public const string NEW_BEST_ROUND = "NEW_BEST_ROUND";
        public const string RATE = "RATE";
        public const string LOGIN = "LOGIN";
        public const string SECONDCHANCE_USED = "SECONDCHANCE_USED";
        public const string SHOW_REWARD_VIDEO_SUCCESS = "SHOW_REWARD_VIDEO_SUCCESS";
        public const string GUIDE = "GUIDE";
        public const string AF = "AF";
        public const string AF_PLAY_ROUND = "AF_PLAY_ROUND";
        public const string AF_ROUND_OVER_SCORE = "AF_ROUND_OVER_SCORE";
        public const string AF_PLAY_DAY = "AF_PLAY_DAY";
        public const string GAME_STATUS = "GAME_STATUS";
        public const string ICE_TIP_GUIDE = "ICE_TIP_GUIDE";
        private const string COCOS_2_UNITY = "COCOS_2_UNITY";
        
        //道具相关
        public const string GOLD = "GOLD";
        public const string PROP_DATA = "PROP_DATA";
        
        //震动相关
        public const string VIBRATE_SWITCH = "VIBRATE_SWITCH";
        
        //石头引导
        public const string STONE_TIP_GUIDE = "STONE_TIP_GUIDE";

        public const string ADREVENUE = "ADREVENUE";
    }
    
    //存储数据定义如下
    public class GameStatus
    {
        public int SlideNumber { get; set; } = 0;
        public int RemoveNumber { get; set; } = 0;
        public int ComboNumber { get; set; } = 0;
        public int HighComboNumber { get; set; } = 0;
        public int GuideTime { get; set; } = 0;
        public int ContinueSlide { get; set; } = 0;
        public int ContinueRemove { get; set; } = 0;
        public bool IsRestart { get; set; } = false;

        public bool videoReady { get; set; } = false;
        public bool videoShow { get; set; } = false;
        public string videoReward { get; set; } = "null";

        public bool interstitialReady { get; set; } = false;
        public bool interstitialShow { get; set; } = false;
        public bool interstitialShould { get; set; } = false;

        public bool continueShow { get; set; } = false;
        public bool continueClk { get; set; } = false;
        public bool continueShould { get; set; } = false;

        public int CurrentLevel { get; set; } = 0;
        public int RemoveFrozen { get; set; } = 0;
        
        public float InningTime { get; set; } = 0.0f;
        public int clearTypeA { get; set; } = 0;
        public int clearTypeA2 { get; set; } = 0;
        public int clearTypeB { get; set; } = 0;
        public int clearTypeB2 { get; set; } = 0;
        public int clearTypeC { get; set; } = 0;
        
        public bool DWAD_CLICK { get; set; } = false;
        public bool DWAD_USE { get; set; } = false;
        public int DWAD_CLICK_SLIDENUMBER { get; set; } = 0;
        public int DWAD_USE_SLIDENUMBER { get; set; } = 0;
        public int DWAD_REMAINING_STEP { get; set; } = 0;

        public bool b421_Clk { get; set; } = false;
        public bool b421_Use { get; set; } = false;
        public int b421_Count { get; set; } = 0;
        public int b421_SlideNumber { get; set; } = 0;

        public int DieToLifeCount { get; set; } = 0;
        public bool Combo2Tip { get; set; } = false;
        public bool Combo4Tip { get; set; } = false;
        public bool Combo7Tip { get; set; } = false;
        public int StepScore { get; set; } = 0;
        public int AllClearCount { get; set; } = 0;

        public int TotalBlockCount { get; set; } = 0;
        public int LastBronze { get; set; } = 0;
        public int LastGold { get; set; } = 0;

        public int SpecialBronze { get; set; } = 0;
        public int SpecialGold { get; set; } = 0;
        public int SpecialGoldDialog { get; set; } = 0;
        public int SpecialGoldVideo { get; set; } = 0;
        public int SpecialGoldVideoShow { get; set; } = 0;
        public int SpecialGoldInsShow { get; set; } = 0;
        public int ScoreWhenSpecialGoldClear { get; set; } = 0;

        public int StoneCountGenerate { get; set; } = 0;
        public int StoneCountClear { get; set; } = 0;

        public int RemoveShake { get; set; } = 0;
        public int ComboShake { get; set; } = 0;
        public int SpecialGoldShake { get; set; } = 0;
        public int SpecialBlueSelectShake { get; set; } = 0;
        public int SpecialBlueSplitShake { get; set; } = 0;
        public bool IsFirstGetBestScorePreGame { get; set; } = true;
    }

    public class AFData
    {
        public bool guide_end { get; set; } = false;
        public bool level_2 { get; set; } = false;
        public bool level_3 { get; set; } = false;
        public bool level_4 { get; set; } = false;
        public bool level_5 { get; set; } = false;

        public bool level_6 { get; set; } = false;
        
        public bool level_8 { get; set; } = false;

        public bool rate_4_over { get; set; } = false;
        public bool video_show { get; set; } = false;
        public bool video_reward { get; set; } = false;
        public bool round_over_score_2000 { get; set; } = false;

        public bool inningtime_lt_600 { get; set; } = false;
        
        public bool inningtime_lt_1800 { get; set; } = false;

        public bool inningtime_lt_3000 { get; set; } = false;

        public bool round_lt_1 { get; set; } = false;

        public bool round_lt_5 { get; set; } = false;

        public bool round_lt_10 { get; set; } = false;

        public bool round_lt_15 { get; set; } = false;

        public bool round_lt_20 { get; set; } = false;

        public bool bestscore_lt_1 { get; set; } = false;

        public bool bestscore_lt_2 { get; set; } = false;

        public bool bestscore_lt_5 { get; set; } = false;

        public bool bestscore_lt_10 { get; set; } = false;

        public bool setting_click { get; set; } = false;

        public bool setting_sound { get; set; } = false;

        public bool setting_replay_click { get; set; } = false;

        public bool day1_retention_24h { get; set; } = false;

        public bool day3_retention_72h { get; set; } = false;

        public bool day1_retention { get; set; } = false;

        public bool day3_retention { get; set; } = false;

    }
    
    public class LoginData
    {
        //第一次登陆的时间
        public long _firstDateTime { get; set; } = 0;
        public int dayNum { get; set; } = -1;
        public long dayTimeStamp { get; set; } = 0;
        public int dayBestScore { get; set; } = 0;
        public int dayRound { get; set; } = 0;
        public List<int> dayStepArr = new List<int>();
    }

    public class GuideData
    {
        public int step { get; set; } = 0;
        public bool end { get; set; } = false;
    }

    public class RateData
    {
        public bool rate { get; set; } = false;
        public int count { get; set; } = 0;
        public List<int> showRound = new List<int>();
    }
}
