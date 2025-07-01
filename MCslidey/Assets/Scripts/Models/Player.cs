using System;
using System.Collections.Generic;
using Manager;
using Other;


namespace Models
{
    public static class Player
    {
        public static bool IsBlockMoving { get; set; } = false;
        public static bool UserCanMove { get; set; } = false;

        private static int _curScore = 0;
        private static int _bestScore = 0;
        private static string _newBestStatus = "no";
        private static int _totalRound = 0;
        private static int _totalScore = 0;
        private static string _secondChanceUsed = "no";

        //道具相关
        private static int _gold;
        private static Dictionary<string, int> _propsData = null;

        private static RateData _rateData = null;
        private static LoginData _loginData = null;
        private static GuideData _guideData = null;
        private static int _guideDataNextHangIndex = 0;
        private static int[][] _guideStepData = null;
        private static int _iceTipData = 0;
        private static int _stoneTipData = 0;
        private static float _afAdRevenue = 0;

        public static void InitData()
        {
            if (ManagerLocalData.HaveData(ManagerLocalData.SCORE_CUR))
            {
                _curScore = ManagerLocalData.GetIntData(ManagerLocalData.SCORE_CUR);
                _bestScore = ManagerLocalData.GetIntData(ManagerLocalData.SCORE_BEST);
                _newBestStatus = ManagerLocalData.GetStringData(ManagerLocalData.NEW_BEST_STATUS);
                _totalRound = ManagerLocalData.GetIntData(ManagerLocalData.TOTAL_ROUND);
                _totalScore = ManagerLocalData.GetIntData(ManagerLocalData.TOTAL_SCORE);
                _secondChanceUsed = ManagerLocalData.GetStringData(ManagerLocalData.SECONDCHANCE_USED);
                _iceTipData = ManagerLocalData.GetIntData(ManagerLocalData.ICE_TIP_GUIDE);
                _stoneTipData = ManagerLocalData.GetIntData(ManagerLocalData.STONE_TIP_GUIDE);
            }

            _afAdRevenue = ManagerLocalData.GetFloatData(ManagerLocalData.ADREVENUE);

            _rateData = ManagerLocalData.GetTableData<RateData>(ManagerLocalData.RATE) ?? new RateData();
            _loginData = ManagerLocalData.GetTableData<LoginData>(ManagerLocalData.LOGIN) ?? new LoginData();
            _guideData = ManagerLocalData.GetTableData<GuideData>(ManagerLocalData.GUIDE) ?? new GuideData();
            _guideDataNextHangIndex = GetGuideStepHangStartIndex(GetGuideStep());
            if (_rateData.showRound == null)
            {
                _rateData.showRound = new List<int>();
            }

            if (Constant.PropSwitch)
            {
                _gold = ManagerLocalData.GetIntData(ManagerLocalData.GOLD);
                _propsData = ManagerLocalData.HaveData(ManagerLocalData.PROP_DATA) ? ManagerLocalData.GetTableData<Dictionary<string, int>>(ManagerLocalData.PROP_DATA) : new Dictionary<string, int>();
            }
        }

        private static void CheckGuideData()
        {
            if (_guideData == null)
            {
                _guideData = ManagerLocalData.GetTableData<GuideData>(ManagerLocalData.GUIDE) ?? new GuideData();
            }
        }

        private static void CheckLoginData()
        {
            if (_loginData == null)
            {
                _loginData = ManagerLocalData.GetTableData<LoginData>(ManagerLocalData.LOGIN) ?? new LoginData();
            }

            if (_loginData.dayStepArr == null)
            {
                _loginData.dayStepArr = new List<int>();
            }
        }

        private static void CheckRateData()
        {
            if (_rateData == null)
            {
                _rateData = ManagerLocalData.GetTableData<RateData>(ManagerLocalData.RATE) ?? new RateData();
            }

            if (_rateData.showRound == null)
            {
                _rateData.showRound = new List<int>();
            }
        }

        public static void SetGold(int gold)
        {
            _gold = gold;
            ManagerLocalData.SetIntData(ManagerLocalData.GOLD, _gold);
        }

        public static int GetGold()
        {
            return _gold;
        }




        public static void UpdatePropNumById(string propId, int num)
        {
            if (_propsData.ContainsKey(propId))
            {
                _propsData[propId] = num;
            }
            else
            {
                _propsData.Add(propId, num);
            }

            ManagerLocalData.SetTableData(ManagerLocalData.PROP_DATA, _propsData);
            ManagerLocalData.SaveData();
        }

        public static void UpdateADRevenue(float revenue)
        {
        }

        public static int GetPropNumById(string propId)
        {
            return _propsData.ContainsKey(propId) ? _propsData[propId] : 0;
        }

        public static void SetCurScore(int score)
        {
            _curScore = score;
            ManagerLocalData.SetIntData(ManagerLocalData.SCORE_CUR, _curScore);
        }

        public static int GetCurScore()
        {
            return _curScore;
        }

        public static void SetBestScore(int score)
        {
            if (_bestScore < score)
            {
                _bestScore = score;
                ManagerLocalData.SetIntData(ManagerLocalData.SCORE_BEST, _bestScore);
            }
        }

        public static int GetBestScore()
        {
            return _bestScore;
        }

        public static bool IsNewBestStatus()
        {
            return _newBestStatus == "yes";
        }

        public static void NewBestStatus()
        {
            _newBestStatus = "yes";
            ManagerLocalData.SetStringData(ManagerLocalData.NEW_BEST_STATUS, _newBestStatus);
        }

        public static void ResetNewBestStatus()
        {
            _newBestStatus = "no";
            ManagerLocalData.SetStringData(ManagerLocalData.NEW_BEST_STATUS, _newBestStatus);
        }

        public static bool ShouldShowRate()
        {
            CheckRateData();
            if (_rateData.rate)
            {
                return false;
            }

            if (_rateData.count >= Constant.RateMaxCount)
            {
                return false;
            }

            if (_rateData.count > 0 && GetTotalRound() == _rateData.showRound[_rateData.showRound.Count - 1])
            {
                return false;
            }

            return GetTotalRound() > 0 && (GetTotalRound() - Constant.RateRound) >= 0 &&
                   (GetTotalRound() - Constant.RateRound) % Constant.RateDeltaRound == 0;
        }

        public static void AddShowRateCount()
        {
            CheckRateData();
            if (_rateData.count < Constant.RateMaxCount)
            {
                _rateData.showRound.Add(GetTotalRound());
                ++_rateData.count;
                ManagerLocalData.SetTableData(ManagerLocalData.RATE, _rateData);
            }
        }

        public static int GetShowRateCount()
        {
            CheckRateData();
            return _rateData.count;
        }

        public static void SetAlreadyRate()
        {
            CheckRateData();
            _rateData.rate = true;
            ManagerLocalData.SetTableData(ManagerLocalData.RATE, _rateData);
        }

        public static void AddTotalScore()
        {
            _totalScore += _curScore;
            ManagerLocalData.SetIntData(ManagerLocalData.TOTAL_SCORE, _totalScore);
        }

        public static int GetTotalScore()
        {
            return _totalScore;
        }

        public static void AddTotalRound()
        {
            ++_totalRound;
            ManagerLocalData.SetIntData(ManagerLocalData.TOTAL_ROUND, _totalRound);
        }

        public static int GetTotalRound()
        {
            return _totalRound;
        }

        public static bool IsSecondChanceUsed()
        {
            return _secondChanceUsed == "yes";
        }

        public static void ResetSecondChanceUsed()
        {
//            _secondChanceUsed = "no";
//            ManagerLocalData.SetStringData(ManagerLocalData.SECONDCHANCE_USED, _secondChanceUsed);
        }

        public static void UseSecondChance()
        {
            _secondChanceUsed = "yes";
            ManagerLocalData.SetStringData(ManagerLocalData.SECONDCHANCE_USED, _secondChanceUsed);
        }

        public static void AddDayRound()
        {
            CheckLoginData();
            ++_loginData.dayRound;
            ManagerLocalData.SetTableData(ManagerLocalData.LOGIN, _loginData);
        }

        public static int GetDayRound()
        {
            CheckLoginData();
            return _loginData.dayRound;
        }

        public static void AddDayStep(int step)
        {
            CheckLoginData();
            _loginData.dayStepArr.Add(step);
            ManagerLocalData.SetTableData(ManagerLocalData.LOGIN, _loginData);
        }

        public static int GetStepAverage()
        {
            CheckLoginData();
            var sum = 0;
            foreach (var stepNum in _loginData.dayStepArr)
            {
                sum += stepNum;
            }
            return Tools.ChinaRound(sum * 1.0f / _loginData.dayStepArr.Count);
        }

        public static void UpdateDayBestScore(int score)
        {
            CheckLoginData();
            if (score > _loginData.dayBestScore)
            {
                _loginData.dayBestScore = score;
                ManagerLocalData.SetTableData(ManagerLocalData.LOGIN, _loginData);
            }
        }

        public static void UpdateLeftInningTime(float inningTime)
        {
            float lfInningTime = ManagerLocalData.GetFloatData(ManagerLocalData.LEFT_INNING_TIME);
            lfInningTime += inningTime;
            ManagerLocalData.SetFloatData(ManagerLocalData.LEFT_INNING_TIME, lfInningTime);
        }

        public static void UpNewBsetRound()
        {
            bool bsetState = Player.IsNewBestStatus();
            if (!bsetState)
            {
                return;
            }

            int bestRoundCount = ManagerLocalData.GetIntData(ManagerLocalData.NEW_BEST_ROUND);
            bestRoundCount += 1;
            ManagerLocalData.SetIntData(ManagerLocalData.NEW_BEST_ROUND, bestRoundCount);
        }

        public static int GetDayBestScore()
        {
            CheckLoginData();
            return _loginData.dayBestScore;
        }

        public static void AddLoginDay()
        {
            CheckLoginData();
            if (_loginData.dayNum == -1)
            {
                var beginTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                _loginData._firstDateTime = (long)(DateTime.Now - beginTime).TotalMilliseconds;
                _loginData.dayNum = 0;
            }
            if (Tools.IsToday(_loginData.dayTimeStamp))
            {
                return;
            }

            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            _loginData.dayTimeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds;

            _loginData.dayNum = Convert.ToInt32((_loginData.dayTimeStamp - _loginData._firstDateTime) / 1000 / 3600 / 24);

            _loginData.dayRound = 0;
            _loginData.dayStepArr = new List<int>();
            _loginData.dayBestScore = 0;

            ManagerLocalData.SetTableData(ManagerLocalData.LOGIN, _loginData);
        }

        public static int GetLoginDay()
        {
            CheckLoginData();
            return _loginData.dayNum;
        }

        public static bool IsInGuide()
        {
            CheckGuideData();
            return !_guideData.end;
        }

        public static int GetGuideStep()
        {
            CheckGuideData();
            return _guideData.step;
        }

        public static void GuideEnd()
        {
            CheckGuideData();
            if (_guideStepData != null && _guideData.step >= _guideStepData.Length)
            {
                _guideData.end = true;
                ManagerLocalData.SetTableData(ManagerLocalData.GUIDE, _guideData);
            }
        }

        public static void CompleteGuide()
        {
            CheckGuideData();
            ++_guideData.step;
            ManagerLocalData.SetTableData(ManagerLocalData.GUIDE, _guideData);
        }

        public static void AddGuideStepNextHangIndex()
        {
            ++_guideDataNextHangIndex;
        }

        public static int GetGuideNextHangIndex()
        {
            return _guideDataNextHangIndex;
        }

        private static int GetGuideStepHangStartIndex(int step)
        {
            switch (step)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 7;
                default:
                    return 100;
            }
        }

        public static int[] GetGuideStepData(int step)
        {
            switch (Constant.GuideVersion)
            {
                case "":
                    //起始位置，目标位置
                    if (_guideStepData == null)
                    {
                        _guideStepData = new[]
                        {
                            new []{11, 5},
                            new []{8, 2},
                            new []{36, 27},
                            new []{12, 5}
                        };
                    }

                    if (step < _guideStepData.Length)
                    {
                        return _guideStepData[step];
                    }

                    return null;
                case "new":
                    if (_guideStepData == null)
                    {
                        _guideStepData = new[]
                        {
                            new []{11, 5},
                            new []{0, 2},
                            new []{27, 30}
                        };
                    }

                    if (step < _guideStepData.Length)
                    {
                        return _guideStepData[step];
                    }

                    return null;
                default:
                    return null;
            }
        }

        public static void SaveGameStatusData()
        {
            ManagerLocalData.SetTableData(ManagerLocalData.GAME_STATUS, Constant.GameStatusData);
        }

        public static void SetAlreadyShowIceTip()
        {
            _iceTipData = 1;
            ManagerLocalData.SetIntData(ManagerLocalData.ICE_TIP_GUIDE, _iceTipData);
        }

        public static bool ShouldShowIceTip()
        {
            return _iceTipData != 1;
        }

        public static int ScoreToStar(int score)
        {
            var scoreToStarArr = Constant.ScoreToStar.Split(',');
            for (var i = scoreToStarArr.Length - 1; i >= 0; --i)
            {
                if (score >= int.Parse(scoreToStarArr[i]))
                {
                    return (i + 1);
                }
            }
            return 3;
        }

        public static void SetAlreadyStoneTip()
        {
            _stoneTipData = 1;
            ManagerLocalData.SetIntData(ManagerLocalData.STONE_TIP_GUIDE, _stoneTipData);
        }

        public static bool ShouldShowStoneTip()
        {
            return _stoneTipData != 1;
        }

        public static long GetFirstOpenAppTime()
        {
            return _loginData._firstDateTime;
        }

    }
}