using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Manager;
using Other;
using UnityEngine;

namespace Models
{
    public static class Blocks
    {
        public enum Key
        {
            Length = 0,
            Color,
            Pos,
            Special,
            Ice,
            Prop
        }

        //道具功能宝石
        public enum Special
        {
            Rainbow = 1,
            Bronze,//青铜
            Gold,//金
            Stone//石头
        }

        public enum SpecialGoldEffType
        {
            Color = 0,
            Split43,
            NoNewBlocks,
        }

        public enum Color
        {
            Blue = 1,
            Green,
            Pink,
            Red,
            Yellow
        }

        //升级奖励类型
        public const string LEVEL_REWARD_TYPE_CLEAR_BLOCKS_1 = "1";
        public const string LEVEL_REWARD_TYPE_CLEAR_BLOCKS_2 = "2";
        public const string LEVEL_REWARD_TYPE_CLEAR_BLOCKS_3 = "3";
        public const string LEVEL_REWARD_TYPE_BLOCK_TO_GOLD = "g";

        public static int CurLevel { get; private set; } = 1;
        //存储方块数据(0-5表示不同含义 enum KEY)的数组
        private static int[][] _blocksData = new int[Constant.Lie * Constant.Hang][];
        //简化上方数组,表示所有位置的占位情况  0位空 1位占位
        private static int[] _blocksMap = new int[Constant.Lie * Constant.Hang];
        private static int _difficulty = 0;
        private static int _blockSpecialHangCount;
        private static int[][] _iceRandomRate = null;
        private static int _iceStep = 0;
        private static List<int[]>[] _guideBlocks = null;

        private static LengthRate _lengthRate = null;
        private static LengthRateItem _curLengthRate = null;
        private static Level _levelData = null;
        private static HangNumByScore _hangNumByScore;
        private static UnitScore _unitScore;
        private static SP _sp;
        private static SPC _spc;
        private static BlocksTip _blocksTip;

        private static int _specialBronzeRangeStart = 50;
        private static int _specialBronzeRangeEnd = 100;
        private static int _specialBronzeInterval = 100;

        private static int _specialGoldRangeStart = 100;
        private static int _specialGoldRangeEnd = 150;
        private static int _specialGoldInterval = 200;
        private static int[] _specialGoldEffRandomRate = null;
        private static bool _parsedSpecialBlockRandomString = false;
        private static BlocksTest _blocksTest = null;

        private static int _curStoneHang = 0;
        private static int _loadNum = 0;

        #region InitDataAsync

        private static void InitDataAsync_hangNumByScore()
        {
            // _hangNumByScore = JsonParse<HangNumByScore>.GetDataFromPath("data_hangNumByScore");
            // _hangNumByScore.data = new 
            ++_loadNum;
        }

        private static void InitDataAsync_unitScore()
        {
            // _unitScore = JsonParse<UnitScore>.GetDataFromPath("data_unitScore");
            ++_loadNum;
        }

        private static void InitDataAsync_spc()
        {
            // _spc = JsonParse<SPC>.GetDataFromPath("data_SPC_rate");
            ++_loadNum;
        }

        private static void InitDataAsyncUpdateDifficulty()
        {
            UpdateDifficulty(Constant.DifficultyLevel);
            ++_loadNum;
        }

        private static void InitDataAsyncUpdateLevelData()
        {
            UpdateLevelData(1);
            ++_loadNum;
        }

        private static void InitDataAsyncUpdateSpecialProtect()
        {
            UpdateSpecialProtect(1);
            ++_loadNum;
        }

        #endregion

        public static async Task<bool> InitData()
        {
            
            InitDataAsync_hangNumByScore();

            _hangNumByScore = new HangNumByScore();
            _hangNumByScore.data = new[]
            {
                new HangNumByScoreItem(){id = 1, score_min = 0, hangNum = 2},
                new HangNumByScoreItem(){id = 2, score_min = 80, hangNum = 3},
                new HangNumByScoreItem(){id = 3, score_min = 500, hangNum = 4},
                new HangNumByScoreItem(){id = 4, score_min = 1000, hangNum = 5},
                new HangNumByScoreItem(){id = 5, score_min = 2000, hangNum = 6},
            };
            
            InitDataAsync_unitScore();
            _unitScore = new UnitScore();
            _unitScore.data = new[]
            {
                new UnitScoreItem(){id = 1 , score_min = 0, score_unit = 1},
                new UnitScoreItem(){id = 2 , score_min = 80, score_unit = 4},
                new UnitScoreItem(){id = 3 , score_min = 500, score_unit = 9},
                new UnitScoreItem(){id = 4 , score_min = 1000, score_unit = 16},
                new UnitScoreItem(){id = 5 , score_min = 2000, score_unit = 25},
                new UnitScoreItem(){id = 6 , score_min = 5000, score_unit = 36},
            };
            
            
            InitDataAsync_spc();

            _spc = new SPC();
            _spc.data = new[]
            {
                new SPCItem(){id = 1, combo_num = 1, rate_die = 100, rate_notDie = 100},
                new SPCItem(){id = 2, combo_num = 2, rate_die = 20, rate_notDie = 5},
            };

            InitDataAsyncUpdateDifficulty();
            
            InitDataAsyncUpdateLevelData();
            InitDataAsyncUpdateSpecialProtect();

            while (_loadNum < 6)
            {
                await Task.Delay(1000 / 60);
            }
            //录视频广告
//                        _blocksTest = new BlocksTest();

            _blocksTip = new BlocksTip();
            ResetData();

            return true;
        }

        public static BlocksTest GetBlocksTest()
        {
            return _blocksTest;
        }

        public static bool IsTesting()
        {
            return _blocksTest != null;
        }

        public static void ResetData()
        {
            if (_blocksTest != null)
            {
                ManagerLocalData.ClearData(ManagerLocalData.BLOCKS_DATA);
                ManagerLocalData.SaveData();

                Player.SetCurScore(_blocksTest.GetCurrentScore());
                Constant.GameStatusData = new GameStatus();
            }

            _iceRandomRate = new int[3][];

            for (var i = 0; i < Constant.Lie * Constant.Hang; ++i)
            {
                _blocksData[i] = new int[Enum.GetNames(typeof(Key)).Length];
            }
            UpdateMap();
        }

        /// <summary>
        /// 更新当前地图方块占位情况
        /// </summary>
        public static void UpdateMap()
        {
            _blocksMap = new int[Constant.Lie * Constant.Hang];
            foreach (var data in _blocksData)
            {
                if (data != null && data[(int)Key.Length] > 0)
                {
                    for (var i = 0; i < data[(int)Key.Length]; ++i)
                    {
                        _blocksMap[i + data[(int)Key.Pos]] = 1;
                    }
                }
            }
        }

        public static void DrawMap()
        {
            string str = "\n";
            for (var i = Constant.Hang - 1; i >= 0; --i)
            {
                for (var j = 0; j < Constant.Lie; ++j)
                {
                    str += _blocksMap[i * Constant.Lie + j] + ",";
                }
                str += "\n";
            }
            DebugEx.Log(str);
        }

        /// <summary>
        /// 当前index位置是否为空
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool IsEmpty(int index)
        {
            if (index >= _blocksMap.Length || index < 0)
            {
                return false;
            }
            return _blocksMap[index] == 0;
        }

        private static void SetBlockDataByIndex(int index, int[] data)
        {
            if (index >= _blocksMap.Length || index < 0)
            {
                return;
            }
            _blocksData[index] = data;
        }

        public static int[] GetBlockDataByIndex(int index)
        {
            if (HaveBlock(index))
            {
                return _blocksData[index];
            }

            return null;
        }

        public static void ClearBlockDataByIndex(int index)
        {
            _blocksData[index] = new int[Enum.GetNames(typeof(Key)).Length];
        }

        public static int[][] GetBlocksData()
        {
            return _blocksData;
        }

        public static void SetBlocksData(int[][] data)
        {
            _blocksData = data;
            UpdateMap();
        }

        public static int[][] CheckSpecialBlockSwitch()
        {
            foreach (var data in _blocksData)
            {
                if (data == null) continue;
                var specialType = data[(int)Key.Special];

                if ((specialType == (int)Special.Rainbow && !Constant.SpecialRainbowSwitch) ||
                    (specialType == (int)Special.Bronze && !Constant.SpecialBronzeSwitch) ||
                    (specialType == (int)Special.Gold && !Constant.SpecialGoldSwitch) ||
                    (specialType == (int)Special.Stone && !Constant.StoneSwitch))
                {
                    data[(int)Key.Special] = 0;
                }

                //如果冰块开关没有开，则之前有冰块的方块也变为普通块
                if (!Constant.IceBlockSwitch && data[(int)Key.Ice] > 0)
                {
                    data[(int)Key.Ice] = 0;
                }
            }

            return _blocksData;
        }

        public static List<int[]> DownBlocks()
        {
            var downList = new List<int[]>();
            for (var i = Constant.Lie; i < Constant.Lie * Constant.Hang; ++i)
            {
                if (HaveBlock(i))
                {
                    var curHang = GetHangByPos(i);
                    var deltaY = 0;
                    for (var hang = 1; hang <= curHang; ++hang)
                    {

                        var startIndex = i - hang * Constant.Lie;
                        var endIndex = startIndex + _blocksData[i][(int)Key.Length];
                        var canDown = true;
                        for (var posIndex = startIndex; posIndex < endIndex; ++posIndex)
                        {
                            if (!IsEmpty(posIndex))
                            {
                                canDown = false;
                            }
                        }

                        if (canDown)
                        {
                            deltaY = -hang;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (deltaY != 0)
                    {
                        downList.Add(new int[] { deltaY, i });
                        MoveBlock(new int[] { 0, deltaY, i });
                    }
                }
            }
            return downList;
        }

        public static void UpBlocks()
        {
            if (_blocksData != null)
            {
                for (var i = Constant.Lie * Constant.Hang - 1; i >= 0; --i)
                {
                    if (HaveBlock(i))
                    {
                        MoveBlock(new int[] { 0, 1, i });
                    }
                }
            }
        }

        public static void AddBlocks(List<int[]> blocksData)
        {
            foreach (var blockData in blocksData)
            {
                _blocksData[blockData[(int)Key.Pos]] = blockData;
            }
            UpdateMap();
        }

        //0为移动x，1为移动y，2为posIndex
        public static void MoveBlock(int[] moveData)
        {
            var offsetX = moveData[0];
            var offsetY = moveData[1];
            var posIndex = moveData[2];
            if (offsetX != 0)
            {
                _blocksData[posIndex][(int)Key.Pos] += offsetX;
                //新增现在位置信息
                SetBlockDataByIndex(_blocksData[posIndex][(int)Key.Pos], _blocksData[posIndex]);
                //清楚原来位置信息
                ClearBlockDataByIndex(_blocksData[posIndex][(int)Key.Pos] - offsetX);
            }

            if (offsetY != 0)
            {
                _blocksData[posIndex][(int)Key.Pos] += Constant.Lie * offsetY;
                SetBlockDataByIndex(_blocksData[posIndex][(int)Key.Pos], _blocksData[posIndex]);
                ClearBlockDataByIndex(_blocksData[posIndex][(int)Key.Pos] - Constant.Lie * offsetY);
            }
            UpdateMap();
        }

        public static int GetHangNum()
        {
            for (var i = _blocksMap.Length - 1; i >= 0; --i)
            {
                if (!IsEmpty(i))
                {
                    return GetHangByPos(i) + 1;
                }
            }
            return 0;
        }

        public static bool ClearBlocks(out List<int> clearHang, out List<int[]> clearSpecialBlocks)
        {
            clearHang = new List<int>();
            clearSpecialBlocks = new List<int[]>();

            for (var hang = 0; hang < Constant.Hang; ++hang)
            {
                var canClear = true;
                for (var lie = 0; lie < Constant.Lie; ++lie)
                {
                    if (IsEmpty(hang * Constant.Lie + lie))
                    {
                        canClear = false;
                        break;
                    }
                }

                if (canClear)
                {
                    clearHang.Add(hang);
                    for (var posIndex = hang * Constant.Lie; posIndex < (hang + 1) * Constant.Lie; ++posIndex)
                    {
                        if (HaveBlock(posIndex))
                        {
                            if (HaveSpecial(posIndex) && _blocksData[posIndex][(int)Key.Special] != (int)Special.Stone)
                            {
                                clearSpecialBlocks.Add(_blocksData[posIndex]);
                            }

                            if (HaveIce(posIndex))
                            {
                                --_blocksData[posIndex][(int)Key.Ice];
                            }
                            else
                            {
                                ClearBlockDataByIndex(posIndex);
                            }
                        }
                    }

                    if (IsTesting() && GetBlocksTest().OnlyClearOne())
                    {
                        break;
                    }
                }
            }
            UpdateMap();
            return clearHang.Count > 0;
        }

        public static List<int> ClearSpecialEdgeBlocks(int[] specialData)
        {
            var specialClear = new List<int>();
            //右
            int upMapPos;
            int downMapPos;
            for (var i = specialData[(int)Key.Pos];
                i < specialData[(int)Key.Pos] + specialData[(int)Key.Length];
                ++i)
            {
                //上
                upMapPos = i + Constant.Lie;
                if (!IsEmpty(upMapPos) && HaveBlock(upMapPos))
                {
                    specialClear.Add(upMapPos);
                }

                //下
                downMapPos = i - Constant.Lie;
                if (!IsEmpty(downMapPos) && HaveBlock(downMapPos))
                {
                    specialClear.Add(downMapPos);
                }
            }

            //左上
            upMapPos = specialData[(int)Key.Pos] + Constant.Lie;
            if (!IsEmpty(upMapPos) && !HaveBlock(upMapPos))
            {
                for (var k = upMapPos - 1; k >= 0; --k)
                {
                    if (!IsEmpty(k) && HaveBlock(k))
                    {
                        specialClear.Add(k);
                        break;
                    }
                }
            }

            //左下
            downMapPos = specialData[(int)Key.Pos] - Constant.Lie;
            if (!IsEmpty(downMapPos) && !HaveBlock(downMapPos))
            {
                for (var k = downMapPos - 1; k >= 0; --k)
                {
                    if (!IsEmpty(k) && HaveBlock(k))
                    {
                        specialClear.Add(k);
                        break;
                    }
                }
            }

            foreach (var posIndex in specialClear)
            {
                if (HaveBlock(posIndex))
                {
                    if (HaveIce(posIndex))
                    {
                        --_blocksData[posIndex][(int)Key.Ice];
                    }
                    else
                    {
                        ClearBlockDataByIndex(posIndex);
                    }
                }
            }

            if (specialClear.Count > 0)
            {
                UpdateMap();
            }
            return specialClear;
        }

        public static List<int> SplitSpecialEdgeBlocks(int[] specialData, out List<int[]> newBlocks, out List<int[]> edgeToNew)
        {
            var specialSplit = new List<int>();
            //右
            int upMapPos;
            int downMapPos;
            for (var i = specialData[(int)Key.Pos];
                i < specialData[(int)Key.Pos] + specialData[(int)Key.Length];
                ++i)
            {
                //上
                upMapPos = i + Constant.Lie;
                if (!IsEmpty(upMapPos) && HaveBlock(upMapPos))
                {
                    specialSplit.Add(upMapPos);
                }

                //下
                downMapPos = i - Constant.Lie;
                if (!IsEmpty(downMapPos) && HaveBlock(downMapPos))
                {
                    specialSplit.Add(downMapPos);
                }
            }

            //左上
            upMapPos = specialData[(int)Key.Pos] + Constant.Lie;
            if (!IsEmpty(upMapPos) && !HaveBlock(upMapPos))
            {
                for (var k = upMapPos - 1; k >= 0; --k)
                {
                    if (!IsEmpty(k) && HaveBlock(k))
                    {
                        specialSplit.Add(k);
                        break;
                    }
                }
            }

            //左下
            downMapPos = specialData[(int)Key.Pos] - Constant.Lie;
            if (!IsEmpty(downMapPos) && !HaveBlock(downMapPos))
            {
                for (var k = downMapPos - 1; k >= 0; --k)
                {
                    if (!IsEmpty(k) && HaveBlock(k))
                    {
                        specialSplit.Add(k);
                        break;
                    }
                }
            }

            newBlocks = new List<int[]>();
            edgeToNew = new List<int[]>();
            var specialClear = new List<int>();
            foreach (var posIndex in specialSplit)
            {
                if (HaveBlock(posIndex))
                {
                    if (HaveSpecial(posIndex) && (_blocksData[posIndex][(int)Key.Special] == (int)Special.Gold || _blocksData[posIndex][(int)Key.Special] == (int)Special.Bronze))
                    {
                        //do nothing
                    }
                    else
                    {
                        if (_blocksData[posIndex][(int)Key.Length] > 1)
                        {
                            specialClear.Add(posIndex);

                            var tmpLength = _blocksData[posIndex][(int)Key.Length];
                            var tmpColor = _blocksData[posIndex][(int)Key.Color];
                            var tmpPosIndex = _blocksData[posIndex][(int)Key.Pos];
                            var tmpSpecial = _blocksData[posIndex][(int)Key.Special];
                            var tmpIce = _blocksData[posIndex][(int)Key.Ice];
                            var tmpProp = _blocksData[posIndex][(int)Key.Prop];

                            ClearBlockDataByIndex(posIndex);

                            int[] data1 = null;
                            int[] data2 = null;
                            switch (tmpLength)
                            {
                                case 4:
                                    data1 = new[] { 2, tmpColor, tmpPosIndex, tmpSpecial, tmpIce, tmpProp };
                                    data2 = new[] { 2, tmpColor, tmpPosIndex + 2, tmpSpecial, tmpIce, tmpProp };
                                    break;
                                case 3:
                                    if (Tools.GetNumFromRange(1, 10) <= 5)
                                    {
                                        data1 = new[] { 1, tmpColor, tmpPosIndex, tmpSpecial, tmpIce, tmpProp };
                                        data2 = new[] { 2, tmpColor, tmpPosIndex + 1, tmpSpecial, tmpIce, tmpProp };
                                    }
                                    else
                                    {
                                        data1 = new[] { 2, tmpColor, tmpPosIndex, tmpSpecial, tmpIce, tmpProp };
                                        data2 = new[] { 1, tmpColor, tmpPosIndex + 2, tmpSpecial, tmpIce, tmpProp };
                                    }

                                    break;
                                case 2:
                                    data1 = new[] { 1, tmpColor, tmpPosIndex, tmpSpecial, tmpIce, tmpProp };
                                    data2 = new[] { 1, tmpColor, tmpPosIndex + 1, tmpSpecial, tmpIce, tmpProp };
                                    break;
                            }

                            if (data1 != null)
                            {
                                SetBlockDataByIndex(tmpPosIndex, data1);
                                SetBlockDataByIndex(tmpPosIndex + data1[(int)Key.Length], data2);
                                newBlocks.Add(data1);
                                newBlocks.Add(data2);

                                //被分割的块posIndex，分割后第二块的posIndex
                                edgeToNew.Add(new[] { posIndex, data2[(int)Key.Pos] });
                            }
                        }
                    }
                }
            }

            if (specialClear.Count > 0 || (newBlocks != null && newBlocks.Count > 0))
            {
                UpdateMap();
            }
            return specialClear;
        }

        public static List<int[]> GetNextBlocksDataCanClear()
        {
            var firstHangBlocks = new List<int[]>();
            for (var i = 0; i < Constant.Lie; ++i)
            {
                if (HaveBlock(i) && _blocksData[i][(int)Key.Length] <= 3)
                {
                    firstHangBlocks.Add(_blocksData[i]);
                }
            }

            if (firstHangBlocks.Count <= 0)
            {
                return GetNextBlocksData();
            }

            var empty = firstHangBlocks[Tools.GetNumFromRange(0, firstHangBlocks.Count - 1)];
            var leftCount1 = empty[(int)Key.Pos];
            var leftCount2 = Constant.Lie - (empty[(int)Key.Pos] + empty[(int)Key.Length]);
            var blocksArr = new List<int[]>();
            var posIndex = 0;

            while (leftCount1 > 0)
            {
                var randomLength = GetRandomLength();
                if (randomLength > leftCount1)
                {
                    randomLength = leftCount1;
                }
                blocksArr.Add(new int[] { randomLength, Tools.GetNumFromRange(1, 5), posIndex, 0, 0, 0 });
                posIndex += randomLength;
                leftCount1 -= randomLength;
            }

            posIndex += empty[(int)Key.Length];
            while (leftCount2 > 0)
            {
                var randomLength = GetRandomLength();
                if (randomLength > leftCount2)
                {
                    randomLength = leftCount2;
                }
                blocksArr.Add(new[] { randomLength, Tools.GetNumFromRange(1, 5), posIndex, 0, 0, 0 });
                posIndex += randomLength;
                leftCount2 -= randomLength;
            }

            Constant.GameStatusData.TotalBlockCount += blocksArr.Count;
            return blocksArr;
        }

        private static void GenerateGuideBlocks()
        {
            if (_guideBlocks == null)
            {
                switch (Constant.GuideVersion)
                {
                    case "":
                        _guideBlocks = new List<int[]>[]
                        {
                            //第一步引导
                            new List<int[]>
                            {
                                new []{2, 1, 0, 0, 0, 0},
                                new []{3, 1, 3, 0, 0, 0}
                            },
                            
                            //第二步引导
                            new List<int[]>
                            {
                                new []{2, 2, 0, 0, 0, 0},
                                new []{3, 2, 2, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{2, 3, 0, 0, 0, 0},
                                new []{4, 3, 4, 0, 0, 0}
                            },
                            
                            //第三步引导
                            new List<int[]>
                            {
                                new []{2, 4, 0, 0, 0, 0},
                                new []{2, 5, 4, 0, 0, 0},
                                new []{2, 1, 6, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{3, 2, 0, 0, 0, 0},
                                new []{3, 3, 5, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{2, 4, 2, 0, 0, 0},
                                new []{4, 5, 4, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{4, 1, 0, 0, 0, 0},
                                new []{2, 2, 4, 0, 0, 0}
                            },
                            
                            //第四步引导
                            new List<int[]>
                            {
                                new []{2, 3, 0, 0, 0, 0},
                                new []{3, 4, 4, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{2, 5, 0, 0, 0, 0},
                                new []{3, 1, 2, 0, 0, 0}
                            },
                            
                            //引导完成后，玩家第一次移动时可消除
                            new List<int[]>
                            {
                                new []{1, 3, 0, 0, 0, 0},
                                new []{3, 4, 3, 0, 0, 0},
                                new []{2, 1, 6, 0, 0, 0}
                            },
                        };
                        break;
                    case "new":
                        _guideBlocks = new List<int[]>[]
                        {
                            //第一步引导
                            new List<int[]>
                            {
                                new []{2, 1, 0, 0, 0, 0},
                                new []{3, 1, 3, 0, 0, 0}
                            },
                            
                            //第二步引导
                            new List<int[]>
                            {
                                new []{2, 2, 0, 0, 0, 0},
                                new []{3, 2, 2, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{2, 3, 0, 0, 0, 0},
                                new []{4, 3, 4, 0, 0, 0}
                            },
                            
                            //第三步引导
                            new List<int[]>
                            {
                                new []{1, 4, 3, 0, 0, 0},
                            },

                            new List<int[]>
                            {
                                new []{2, 4, 0, 0, 0, 0},
                                new []{2, 1, 3, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{3, 2, 0, 0, 0, 0},
                                new []{4, 3, 4, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{3, 4, 0, 0, 0, 0},
                                new []{3, 5, 3, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{4, 1, 2, 0, 0, 0},
                                new []{2, 2, 6, 0, 0, 0}
                            },
                            
                            //引导完成后，玩家第一次移动时可消除
                            new List<int[]>
                            {
                                new []{2, 3, 0, 0, 0, 0},
                                new []{3, 4, 4, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{2, 5, 0, 0, 0, 0},
                                new []{3, 1, 2, 0, 0, 0}
                            },

                            new List<int[]>
                            {
                                new []{1, 3, 0, 0, 0, 0},
                                new []{3, 4, 3, 0, 0, 0},
                                new []{2, 1, 6, 0, 0, 0}
                            },
                        };
                        break;
                }
            }
        }

        public static List<int[]> GetNextBlocksData()
        {
            if (_blocksTest != null)
            {
                var data = _blocksTest.GetNextBlocksData();
                if (data != null)
                {
                    return data;
                }

                return new List<int[]>();
            }

            if (Player.IsInGuide())
            {
                GenerateGuideBlocks();
                var nextIndex = Player.GetGuideNextHangIndex();
                if (nextIndex < _guideBlocks.Length)
                {
                    var data = _guideBlocks[nextIndex];
                    Player.AddGuideStepNextHangIndex();
                    return data;
                }
            }

            //获得空格数
            var randomNum = Tools.GetNumFromRange(0, 100);
            var emptyCount = 0;
            for (var i = 0; i < Constant.EmptyBlockRate.Length; ++i)
            {
                if (randomNum >= Constant.EmptyBlockRate[i][0] && randomNum <= Constant.EmptyBlockRate[i][1])
                {
                    emptyCount = i + 1;
                    break;
                }
            }

            //获取方块长度
            var lengthArr = new List<int>();
            for (var i = 0; i < emptyCount; ++i)
            {
                lengthArr.Add(0);
            }

            var leftCount = Constant.Lie - emptyCount;
            while (leftCount > 0)
            {
                var randomLength = GetRandomLength();
                while (randomLength > leftCount)
                {
                    randomLength = GetRandomLength();
                }
                lengthArr.Add(randomLength);
                leftCount -= randomLength;
            }

            lengthArr = Tools.RandomSortList(lengthArr);
            var blocksArr = new List<int[]>();
            var posIndex = 0;
            foreach (var length in lengthArr)
            {
                if (length > 0)
                {
                    blocksArr.Add(new int[] { length, Tools.GetNumFromRange(1, 5), posIndex, 0, 0, 0 });
                    posIndex += length;
                }
                else
                {
                    ++posIndex;
                }
            }

            Constant.GameStatusData.TotalBlockCount += blocksArr.Count;

            //随机彩色块
            var randomSpecialRainbow = false;
            if (Constant.SpecialRainbowSwitch)
            {
                ++_blockSpecialHangCount;
                if (_blockSpecialHangCount >= Constant.BlockSpecialHangCountMin)
                {
                    if (Tools.GetNumFromRange(0, 100) <= Constant.BlockSpecialRate)
                    {
                        blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int)Key.Special] = (int)Special.Rainbow;
                        _blockSpecialHangCount = 0;

                        randomSpecialRainbow = true;
                    }
                }

                if (_blockSpecialHangCount >= Constant.BlockSpecialHangCountMax)
                {
                    blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int)Key.Special] = (int)Special.Rainbow;
                    _blockSpecialHangCount = 0;

                    randomSpecialRainbow = true;
                }
            }

            //青铜金色出现概率解析
            if (Constant.SpecialBronzeSwitch || Constant.SpecialGoldSwitch)
            {
                if (!_parsedSpecialBlockRandomString)
                {
                    _parsedSpecialBlockRandomString = true;

                    var tmpBronzeRandomArr = Constant.SpecialBronzeRandom.Split(',');
                    _specialBronzeRangeStart = int.Parse(tmpBronzeRandomArr[0]);
                    _specialBronzeRangeEnd = int.Parse(tmpBronzeRandomArr[1]);
                    _specialBronzeInterval = int.Parse(tmpBronzeRandomArr[2]);

                    var tmpGoldRandomArr = Constant.SpecialGoldRandom.Split(',');
                    _specialGoldRangeStart = int.Parse(tmpGoldRandomArr[0]);
                    _specialGoldRangeEnd = int.Parse(tmpGoldRandomArr[1]);
                    _specialGoldInterval = int.Parse(tmpGoldRandomArr[2]);
                }
            }

            //随机青铜
            var randomSpecialBronze = false;
            if (Constant.SpecialBronzeSwitch && !randomSpecialRainbow)
            {
                var tmpBronzeNum = ShouldSpecial(_specialBronzeRangeStart, _specialBronzeRangeEnd,
                    _specialBronzeInterval, Constant.GameStatusData.TotalBlockCount,
                    Constant.GameStatusData.LastBronze);
                if (tmpBronzeNum != 0)
                {
                    randomSpecialBronze = true;
                    Constant.GameStatusData.LastBronze = tmpBronzeNum;
                    blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int)Key.Special] = (int)Special.Bronze;
                }
            }

            //随机金
            if (Constant.SpecialGoldSwitch && !randomSpecialRainbow && !randomSpecialBronze)
            {
                if (!BlocksHaveSpecialGold())
                {
                    var tmpGoldNum = ShouldSpecial(_specialGoldRangeStart, _specialGoldRangeEnd, _specialGoldInterval,
                        Constant.GameStatusData.TotalBlockCount, Constant.GameStatusData.LastGold);
                    if (tmpGoldNum != 0)
                    {
                        Constant.GameStatusData.LastGold = tmpGoldNum;
                        blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int)Key.Special] = (int)Special.Gold;
                    }
                }
            }

            ++_curStoneHang;

////            test
//                        if (Constant.SpecialBronzeSwitch && Constant.GameStatusData.TotalBlockCount % 2 == 0)
//                        {
//                            blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int) Key.Special] = (int)Special.Bronze;
//                        }
//
////            test
                        // if (Constant.SpecialGoldSwitch && Constant.GameStatusData.TotalBlockCount % 2 == 0 && !BlocksHaveSpecialGold())
                        // {
                        //     blocksArr[Tools.GetNumFromRange(0, blocksArr.Count - 1)][(int) Key.Special] = (int)Special.Gold;
                        // }

            return blocksArr;
        }

        private static bool BlockHaveStone()
        {
            var curCount = 0;
            //当前棋盘
            for (var posIndex = 0; posIndex < Constant.Hang * Constant.Lie; ++posIndex)
            {
                if (HaveBlock(posIndex) && HaveSpecial(posIndex) &&
                    _blocksData[posIndex][(int)Key.Special] == (int)Special.Stone)
                {
                    ++curCount;
                }
            }

            if (curCount >= Constant.StoneMaxCount)
            {
                return true;
            }

            //readyBlocksData
            return Constant.GamePlayScript.ReadyGroupHaveBlockSpecialType((int)Special.Stone, Constant.StoneMaxCount);
        }

        private static bool BlocksHaveSpecialGold()
        {
            //当前棋盘
            for (var posIndex = 0; posIndex < Constant.Hang * Constant.Lie; ++posIndex)
            {
                if (HaveBlock(posIndex) && HaveSpecial(posIndex) &&
                    _blocksData[posIndex][(int)Key.Special] == (int)Special.Gold)
                {
                    return true;
                }
            }

            //readyBlocksData
            return Constant.GamePlayScript.ReadyGroupHaveBlockSpecialType((int)Special.Gold);
        }

        private static int ShouldSpecial(int rangeStart, int rangeEnd, int intervalNum, int curTotalCount, int lastNum)
        {
            //第一次随机范围
            if (lastNum <= rangeStart)
            {
                if (curTotalCount > rangeStart && curTotalCount <= rangeEnd)
                {
                    if (Tools.GetNumFromRange(rangeStart + 1, rangeEnd) == rangeEnd)
                    {
                        return curTotalCount;
                    }
                }
                else if (curTotalCount > rangeEnd)
                {
                    return rangeEnd - 1;
                }

                return 0;
            }

            //第二次或以上的随机范围
            var rangeStartNew = rangeEnd + ((lastNum + intervalNum - rangeEnd) / intervalNum) * intervalNum;
            var rangeEndNew = rangeStartNew + intervalNum;

            if (curTotalCount > rangeStartNew && curTotalCount <= rangeEndNew)
            {
                if (Tools.GetNumFromRange(rangeStartNew + 1, rangeEndNew) == rangeEndNew)
                {
                    return curTotalCount;
                }
            }
            else if (curTotalCount > rangeEndNew)
            {
                return rangeEndNew - 1;
            }

            return 0;
        }

        private static int GetRandomLength()
        {
            var randomNum = Tools.GetNumFromRange(0, 100);
            if (randomNum <= _curLengthRate.rate_1)
            {
                return 1;
            }

            if (randomNum > _curLengthRate.rate_1 && randomNum <= _curLengthRate.rate_2)
            {
                return 2;
            }

            if (randomNum > _curLengthRate.rate_2 && randomNum <= _curLengthRate.rate_3)
            {
                return 3;
            }

            if (randomNum > _curLengthRate.rate_3 && randomNum <= _curLengthRate.rate_4)
            {
                return 4;
            }

            return Tools.GetNumFromRange(1, 4);
        }

        public static int GetHangByPos(int posIndex)
        {
            return (int)Math.Floor((float)posIndex / Constant.Lie);
        }

        public static int GetLieByPos(int posIndex)
        {
            return posIndex % Constant.Lie;
        }

        /// <summary>
        /// 根据当前方块的属性获取可活动列范围:
        /// 列的最大取值范围为[0-7]
        /// </summary>
        /// <param name="blockData"></param>
        /// <returns>  可活动列的取值范围</returns>
        public static int[] GetEdgeIndex(int[] blockData)
        {
            var curHang = GetHangByPos(blockData[(int)Key.Pos]);
            var curLie = GetLieByPos(blockData[(int)Key.Pos]);
            var min = curLie;
            for (var i = curLie - 1; i >= 0; --i)
            {
                if (IsEmpty(curHang * Constant.Lie + i))
                {
                    min = i;
                }
                else
                {
                    break;
                }
            }

            var max = curLie;
            for (var i = curLie + blockData[(int)Key.Length]; i < Constant.Lie; ++i)
            {
                if (IsEmpty(curHang * Constant.Lie + i))
                {
                    max = i - blockData[(int)Key.Length] + 1;
                }
                else
                {
                    break;
                }
            }
            return new[] { min, max };
        }

        public static bool HaveBlock(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int)Key.Length] != 0;
        }

        public static bool HaveSpecial(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int)Key.Special] != 0;
        }

        public static bool HaveProp(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int)Key.Prop] != 0;
        }

        public static bool HaveIce(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int)Key.Ice] >= 2;
        }

        public static bool IsIce(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int)Key.Ice] > 0;
        }

        public static bool UpdateSpecialProtect(int specialProtectLevel)
        {
            // _sp = JsonParse<SP>.GetDataFromPath("data_SP_D_" + specialProtectLevel);
            _sp = new SP();
            _sp.data = new[]
            {
                new SPItem(){id=1,bestScore_min=0,bestScore_max=100,curScore_min_1=0,curScore_max_1=10,rate_die_1=80,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=70,rate_notDie_2=20,curScore_min_3=25,curScore_max_3=28,rate_die_3=60,rate_notDie_3=10,curScore_min_4=45,curScore_max_4=50,rate_die_4=60,rate_notDie_4=10,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=10},
                new SPItem(){id=2,bestScore_min=100,bestScore_max=1000,curScore_min_1=0,curScore_max_1=10,rate_die_1=80,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=70,rate_notDie_2=20,curScore_min_3=25,curScore_max_3=28,rate_die_3=60,rate_notDie_3=10,curScore_min_4=45,curScore_max_4=50,rate_die_4=60,rate_notDie_4=10,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=10},
                new SPItem(){id=3,bestScore_min=1000,bestScore_max=5000,curScore_min_1=0,curScore_max_1=10,rate_die_1=70,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=60,rate_notDie_2=30,curScore_min_3=25,curScore_max_3=28,rate_die_3=50,rate_notDie_3=10,curScore_min_4=45,curScore_max_4=50,rate_die_4=50,rate_notDie_4=10,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=10},
                new SPItem(){id=4,bestScore_min=5000,bestScore_max=10000,curScore_min_1=0,curScore_max_1=10,rate_die_1=60,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=50,rate_notDie_2=30,curScore_min_3=25,curScore_max_3=28,rate_die_3=50,rate_notDie_3=20,curScore_min_4=45,curScore_max_4=50,rate_die_4=50,rate_notDie_4=20,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=20},
                new SPItem(){id=5,bestScore_min=10000,bestScore_max=20000,curScore_min_1=0,curScore_max_1=10,rate_die_1=50,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=40,rate_notDie_2=40,curScore_min_3=25,curScore_max_3=28,rate_die_3=40,rate_notDie_3=20,curScore_min_4=45,curScore_max_4=50,rate_die_4=40,rate_notDie_4=20,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=20},
                new SPItem(){id=6,bestScore_min=20000,bestScore_max=50000,curScore_min_1=0,curScore_max_1=10,rate_die_1=50,rate_notDie_1=20,curScore_min_2=11,curScore_max_2=20,rate_die_2=40,rate_notDie_2=40,curScore_min_3=25,curScore_max_3=28,rate_die_3=40,rate_notDie_3=20,curScore_min_4=45,curScore_max_4=50,rate_die_4=40,rate_notDie_4=20,curScore_min_5=85,curScore_max_5=90,rate_die_5=50,rate_notDie_5=20},
            };
            return true;
        }

        public static int GetDifficulty()
        {
            return _difficulty;
        }

        public static bool UpdateDifficulty(int difficulty)
        {
            _difficulty = difficulty;
            // _lengthRate =
            //      JsonParse<LengthRate>.GetDataFromPath("data_length_" + _difficulty);
            ParseLengthRate();

            return true;
        }

        private static void ParseLengthRate()
        {
            _lengthRate = new LengthRate();
            _lengthRate.data = new[]
            {
                new LengthRateItem() { id = 1, score_min = 0, rate_1 = 100, rate_2 = 120, rate_3 = 100, rate_4 = 100 },
                new LengthRateItem() { id = 4, score_min = 10, rate_1 = 100, rate_2 = 120, rate_3 = 120, rate_4 = 120 },
                new LengthRateItem() { id = 6, score_min = 25, rate_1 = 100, rate_2 = 120, rate_3 = 100, rate_4 = 100 },
                new LengthRateItem() { id = 7, score_min = 40, rate_1 = 150, rate_2 = 150, rate_3 = 100, rate_4 = 100 },
                new LengthRateItem() { id = 8, score_min = 45, rate_1 = 100, rate_2 = 120, rate_3 = 150, rate_4 = 150 },
                new LengthRateItem() { id = 9, score_min = 65, rate_1 = 100, rate_2 = 120, rate_3 = 170, rate_4 = 170 },
                new LengthRateItem() { id = 10, score_min = 85, rate_1 = 150, rate_2 = 150, rate_3 = 100, rate_4 = 100 },
                new LengthRateItem() { id = 11, score_min = 90, rate_1 = 100, rate_2 = 120, rate_3 = 150, rate_4 = 150 },
                new LengthRateItem() { id = 12, score_min = 130, rate_1 = 100, rate_2 = 120, rate_3 = 170, rate_4 = 170 },
                new LengthRateItem() { id = 13, score_min = 150, rate_1 = 150, rate_2 = 150, rate_3 = 100, rate_4 = 100 },
                new LengthRateItem() { id = 14, score_min = 160, rate_1 = 150, rate_2 = 150, rate_3 = 250, rate_4 = 250 },
            };
            
            foreach (var data in _lengthRate.data)
            {
                var newRate = new int[4];
                var sum = data.rate_1 + data.rate_2 + data.rate_3 + data.rate_4;
                newRate[0] = Tools.ChinaRound(data.rate_1 * 1.0f / sum * 100);
                newRate[1] = Tools.ChinaRound((data.rate_1 + data.rate_2) * 1.0f / sum * 100);
                newRate[2] = Tools.ChinaRound((data.rate_1 + data.rate_2 + data.rate_3) * 1.0f / sum * 100);
                newRate[3] = 100;

                data.rate_1 = newRate[0];
                data.rate_2 = newRate[1];
                data.rate_3 = newRate[2];
                data.rate_4 = newRate[3];
            }
        }

        public static void UpdateLengthRate(int slideNumber)
        {
            for (var i = _lengthRate.data.Length - 1; i >= 0; --i)
            {
                if (slideNumber >= _lengthRate.data[i].score_min)
                {
                    _curLengthRate = _lengthRate.data[i];
                    break;
                }
            }
        }

        public static int GetMaxHangNumByScore(int score)
        {
            if (Player.IsInGuide())
            {
                if (Player.GetGuideStep() == 2)
                {
                    return 5;
                }

                return 2;
            }

            if (_blocksTest != null)
            {
                return _blocksTest.GetMaxHangNum();
            }

            for (var i = _hangNumByScore.data.Length - 1; i >= 0; --i)
            {
                if (score >= _hangNumByScore.data[i].score_min)
                {
                    return _hangNumByScore.data[i].hangNum;
                }
            }
            return _hangNumByScore.data[0].hangNum;
        }

        public static int GetUnitBlockScoreByCurScore(int score)
        {
            for (var i = _unitScore.data.Length - 1; i >= 0; --i)
            {
                if (score >= _unitScore.data[i].score_min)
                {
                    return _unitScore.data[i].score_unit;
                }
            }
            return _unitScore.data[0].score_unit;
        }

        public static bool UpdateLevelData(int level)
        {
            _levelData = new Level();
                _levelData.data = new[]
                {
                    new LevelItem(){level = 1, score = 0, level_reward = "0", score_ice = 0, ice_1 = 0, ice_2 = 0, ice_3 = 0, hang_num = 0, delta_step = 0, max_ice_num = 0},
                    new LevelItem(){level = 2, score = 200, level_reward = "1", score_ice = 80, ice_1 = 0, ice_2 = 0, ice_3 = 0, hang_num = 0, delta_step = 0, max_ice_num = 0},
                    new LevelItem(){level = 3, score = 1200, level_reward = "12", score_ice = 500, ice_1 = 0, ice_2 = 0, ice_3 = 0, hang_num = 0, delta_step = 0, max_ice_num = 0},
                    new LevelItem(){level = 4, score = 6000, level_reward = "12g", score_ice = 1000, ice_1 = 0, ice_2 = 0, ice_3 = 0, hang_num = 0, delta_step = 0, max_ice_num = 0},
                    new LevelItem(){level = 5, score = 18000, level_reward = "123", score_ice = 2000, ice_1 = 0, ice_2 = 0, ice_3 = 0, hang_num = 0, delta_step = 0, max_ice_num = 0},
                    new LevelItem(){level = 6, score = 30000, level_reward = "123g", score_ice = 4000, ice_1 = 100, ice_2 = 0, ice_3 = 0, hang_num = 1, delta_step = 8, max_ice_num = 1},
                    new LevelItem(){level = 7, score = 42000, level_reward = "123g", score_ice = 6000, ice_1 = 100, ice_2 = 0, ice_3 = 0, hang_num = 2, delta_step = 8, max_ice_num = 2},
                    new LevelItem(){level = 8, score = 58000, level_reward = "23", score_ice = 8000, ice_1 = 100, ice_2 = 0, ice_3 = 0, hang_num = 3, delta_step = 7, max_ice_num = 2},
                    new LevelItem(){level = 9, score = 78000, level_reward = "23g", score_ice = 10000, ice_1 = 100, ice_2 = 50, ice_3 = 0, hang_num = 4, delta_step = 6, max_ice_num = 3},
                    new LevelItem(){level = 10, score = 100000, level_reward = "23g", score_ice = 15000, ice_1 = 100, ice_2 = 50, ice_3 = 50, hang_num = 4, delta_step = 5, max_ice_num = 3},
                    new LevelItem(){level = 11, score = 130000, level_reward = "3g", score_ice = 20000, ice_1 = 100, ice_2 = 100, ice_3 = 50, hang_num = 4, delta_step = 5, max_ice_num = 4},
                    new LevelItem(){level = 12, score = 180000, level_reward = "3g", score_ice = 50000, ice_1 = 100, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 4},
                    new LevelItem(){level = 13, score = 260000, level_reward = "3g", score_ice = 80000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 14, score = 400000, level_reward = "3g", score_ice = 120000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 15, score = 600000, level_reward = "3g", score_ice = 200000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 16, score = 800000, level_reward = "3g", score_ice = 300000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 17, score = 1000000, level_reward = "3g", score_ice = 500000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 18, score = 1200000, level_reward = "3g", score_ice = 800000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 19, score = 1400000, level_reward = "3g", score_ice = 1100000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                    new LevelItem(){level = 20, score = 1600000, level_reward = "3g", score_ice = 1500000, ice_1 = 50, ice_2 = 100, ice_3 = 100, hang_num = 5, delta_step = 4, max_ice_num = 5},
                };
                
            return true;
        }

        public static string GetLevelUpReward(int level)
        {
            var levelReward = _levelData.data[level - 1].level_reward;
            var rewardArr = new List<string>();
            if (levelReward.Contains(LEVEL_REWARD_TYPE_BLOCK_TO_GOLD) && Constant.SpecialGoldSwitch && !BlocksHaveSpecialGold())
            {
                rewardArr.Add(LEVEL_REWARD_TYPE_BLOCK_TO_GOLD);
            }

            if (levelReward.Contains(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_1))
            {
                rewardArr.Add(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_1);
            }

            if (levelReward.Contains(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_2))
            {
                rewardArr.Add(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_2);
            }

            if (levelReward.Contains(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_3))
            {
                rewardArr.Add(LEVEL_REWARD_TYPE_CLEAR_BLOCKS_3);
            }

            return rewardArr.Count > 0 ? rewardArr[Tools.GetNumFromRange(0, rewardArr.Count - 1)] : "";
        }

        public static void UseLevelReward(string levelReward, out List<int> clearBlocks, out int goldPosIndex)
        {
            clearBlocks = null;
            goldPosIndex = -1;

            var clearBlockCount = 0;
            var blockToGold = false;
            switch (levelReward)
            {
                case LEVEL_REWARD_TYPE_BLOCK_TO_GOLD:
                    blockToGold = true;
                    break;
                case LEVEL_REWARD_TYPE_CLEAR_BLOCKS_1:
                    clearBlockCount = 1;
                    break;
                case LEVEL_REWARD_TYPE_CLEAR_BLOCKS_2:
                    clearBlockCount = 2;
                    break;
                case LEVEL_REWARD_TYPE_CLEAR_BLOCKS_3:
                    clearBlockCount = 3;
                    break;
            }

            var curBlocks = new List<int>();
            for (var i = 0; i < _blocksData.Length; ++i)
            {
                if (HaveBlock(i) && _blocksData[i][(int)Key.Length] >= 2 && !HaveSpecial(i) && !HaveIce(i))
                {
                    curBlocks.Add(_blocksData[i][(int)Key.Pos]);
                }
            }

            if (blockToGold)
            {
                goldPosIndex = curBlocks[Tools.GetNumFromRange(0, curBlocks.Count - 1)];
                _blocksData[goldPosIndex][(int)Key.Special] = (int)Special.Gold;
            }
            else if (clearBlockCount > 0)
            {
                clearBlocks = new List<int>();
                for (var i = 0; i < clearBlockCount; ++i)
                {
                    if (curBlocks.Count > 0)
                    {
                        var selectIndex = Tools.GetNumFromRange(0, curBlocks.Count - 1);
                        clearBlocks.Add(curBlocks[selectIndex]);
                        ClearBlockDataByIndex(curBlocks[selectIndex]);
                        curBlocks.RemoveAt(selectIndex);
                    }
                }

                UpdateMap();
            }
        }

        public static int UpdateLevelByScore(int score)
        {
            for (var i = _levelData.data.Length - 1; i >= 0; --i)
            {
                if (score >= _levelData.data[i].score)
                {
                    CurLevel = _levelData.data[i].level;
                    //                    if (_levelData.data[i].ice_1 != 0 || _levelData.data[i].ice_2 != 0 || _levelData.data[i].ice_3 != 0)
                    //                    {
                    //                        SetIceRandomRate(_levelData.data[i]);
                    //                    }
                    return CurLevel;
                }
            }

            return CurLevel;
        }

        public static int GetLevelScoreByLevel(int lvl)
        {
            for (var i = 0; i < _levelData.data.Length; i++)
            {
                if (_levelData.data[i].level == lvl)
                {
                    return _levelData.data[i].score;
                }
            }
            return 0;
        }

        public static int GetCurLevelScore()
        {
            for (var i = 0; i < _levelData.data.Length; i++)
            {
                if (_levelData.data[i].level == CurLevel)
                {
                    return _levelData.data[i].score;
                }
            }
            return 0;
        }

        public static int GetNextLevelScore()
        {
            var nextLevel = CurLevel + 1;
            foreach (var levelData in _levelData.data)
            {
                if (levelData.level == nextLevel)
                {
                    return levelData.score;
                }
            }

            return _levelData.data[_levelData.data.Length - 1].score;
        }

        private static void SetIceRandomRate(LevelItem levelData)
        {
            if (_iceRandomRate[0] == null)
            {
                _iceRandomRate[0] = new[] { 1, 0 };
                _iceRandomRate[1] = new[] { 1, 0 };
                _iceRandomRate[2] = new[] { 1, 0 };
            }
            var sum = levelData.ice_1 + levelData.ice_2 + levelData.ice_3;
            _iceRandomRate[0][1] = Tools.ChinaRound(levelData.ice_1 * 1.0f / sum * 100);
            _iceRandomRate[1][1] = Tools.ChinaRound((levelData.ice_1 + levelData.ice_2) * 1.0f / sum * 100);
            _iceRandomRate[2][1] = 100;
        }

        public static List<int> GetIceBlocks()
        {
            if (IsTesting() && _blocksTest.EnableIceBlock())
            {
                var tmpIceBlocks = _blocksTest.GetIceBlocks();
                if (tmpIceBlocks != null)
                {
                    foreach (var posIndex in tmpIceBlocks)
                    {
                        if (HaveBlock(posIndex))
                        {
                            _blocksData[posIndex][(int)Key.Ice] = 2;
                        }
                    }
                }
                return tmpIceBlocks;
            }

            if (!Constant.IceBlockSwitch)
            {
                return null;
            }

            if (IsTesting() && !_blocksTest.EnableIceBlock())
            {
                return null;
            }

            var levelData = _levelData.data[CurLevel - 1];
            var curScore = Player.GetCurScore();
            for (var i = _levelData.data.Length - 1; i >= 0; --i)
            {
                if (curScore >= _levelData.data[i].score_ice)
                {
                    levelData = _levelData.data[i];
                    SetIceRandomRate(levelData);
                    break;
                }
            }

            if (_iceRandomRate[0] != null)
            {
                ++_iceStep;
                if (_iceStep >= levelData.delta_step)
                {
                    DebugEx.Log("应该生成冰块了");

                    var curIceCount = GetCurIceCount();
                    var randomNum = Tools.GetNumFromRange(0, 100);
                    for (var i = 0; i < _iceRandomRate.Length; ++i)
                    {
                        if (randomNum <= _iceRandomRate[i][1])
                        {
                            var newIceCount = _iceRandomRate[i][0];
                            if (curIceCount + newIceCount > levelData.max_ice_num)
                            {
                                newIceCount = levelData.max_ice_num - curIceCount;
                            }
                            if (newIceCount > 0)
                            {
                                var curBlocks = new List<int>();
                                var curMaxHang = GetHangNum();
                                for (var hang = curMaxHang - 1; hang >= curMaxHang - levelData.hang_num; --hang)
                                {
                                    for (var lie = 0; lie < Constant.Lie; ++lie)
                                    {
                                        var posIndex = hang * Constant.Lie + lie;
                                        if (HaveBlock(posIndex) && !HaveSpecial(posIndex) && !HaveIce(posIndex))
                                        {
                                            curBlocks.Add(posIndex);
                                        }
                                    }
                                }

                                var iceBlocks = new List<int>();
                                while (newIceCount > 0 && curBlocks.Count > 0)
                                {
                                    var num = Tools.GetNumFromRange(0, curBlocks.Count - 1);
                                    iceBlocks.Add(curBlocks[num]);
                                    curBlocks.RemoveAt(num);
                                    --newIceCount;
                                }

                                foreach (var posIndex in iceBlocks)
                                {
                                    if (HaveBlock(posIndex))
                                    {
                                        _blocksData[posIndex][(int)Key.Ice] = 2;
                                    }
                                }

                                _iceStep = 0;
                                return iceBlocks;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static int GetCurIceCount()
        {
            var count = 0;
            if (_blocksData != null)
            {
                foreach (var blockData in _blocksData)
                {
                    if (blockData != null && HaveIce(blockData[(int)Key.Pos]))
                    {
                        ++count;
                    }
                }
            }

            return count;
        }

        public static void SaveBlocksData()
        {
            ManagerLocalData.SetTableData(ManagerLocalData.BLOCKS_DATA, _blocksData);
        }

        public static int[] GetClearTip()
        {
            //            return _blocksTip.GetClearTip(_blocksData);
            return _blocksTip.GetClearTips(_blocksData);
        }

        public static int CheckGoldWillClear(int[] moveData)
        {
            if (BlocksHaveSpecialGold())
            {
                return _blocksTip.CheckGoldWillClear(_blocksData, moveData);
            }
            return -1;
        }


        public static  int GetcurScore_min(SPItem item, int index)
        {
            if (index == 1)
            {
                return item.curScore_min_1;
            }
            if (index == 2)
            {
                return item.curScore_min_2;
            }
            if (index == 3)
            {
                return item.curScore_min_3;
            }
            if (index == 4)
            {
                return item.curScore_min_4;
            }
            if (index == 5)
            {
                return item.curScore_min_5;
            }

            return 0;
        }
        
        public static  int GetcurScore_max(SPItem item, int index)
        {
            if (index == 1)
            {
                return item.curScore_max_1;
            }
            if (index == 2)
            {
                return item.curScore_max_2;
            }
            if (index == 3)
            {
                return item.curScore_max_3;
            }
            if (index == 4)
            {
                return item.curScore_max_4;
            }
            if (index == 5)
            {
                return item.curScore_max_5;
            }

            return 0;
        }
        
        public static  int GetcurRatedie(SPItem item, int index)
        {
            if (index == 1)
            {
                return item.rate_die_1;
            }
            if (index == 2)
            {
                return item.rate_die_1;
            }
            if (index == 3)
            {
                return item.rate_die_1;
            }
            if (index == 4)
            {
                return item.rate_die_1;
            }
            if (index == 5)
            {
                return item.rate_die_1;
            }

            return 0;
        }
        
        public static  int GetcurNotRatedie(SPItem item, int index)
        {
            if (index == 1)
            {
                return item.rate_notDie_1;
            }
            if (index == 2)
            {
                return item.rate_notDie_2;
            }
            if (index == 3)
            {
                return item.rate_notDie_3;
            }
            if (index == 4)
            {
                return item.rate_notDie_4;
            }
            if (index == 5)
            {
                return item.rate_notDie_5;
            }

            return 0;
        }
        
        public static int CheckSpecialProtect()
        {
            if (_blocksTest != null)
            {
                return 0;
            }

            var bestScore = Player.GetBestScore();
            var slideNum = Constant.GameStatusData.SlideNumber;

            if (bestScore >= _sp.data[_sp.data.Length - 1].bestScore_max)
            {
                return 0;
            }


            var clearRate = 0;
            for (var i = 0; i < _sp.data.Length; ++i)
            {
                var data = _sp.data[i];
                if (bestScore >= data.bestScore_min && bestScore < data.bestScore_max)
                {
                    var j = 1;
                    while (j <= 5)
                    {
                        var min = GetcurScore_min(data, j);// (int)data.GetType().GetProperty("curScore_min_" + j).GetValue(data);
                        var max = GetcurScore_max(data, j);//(int)data.GetType().GetProperty("curScore_max_" + j).GetValue(data);

                        if (slideNum >= min && slideNum < max)
                        {
                            clearRate = IsDangerous()
                                ? GetcurRatedie(data,
                                    j) //(int)data.GetType().GetProperty("rate_die_" + j).GetValue(data)
                                : GetcurNotRatedie(data, j);//(int)data.GetType().GetProperty("rate_notDie_" + j).GetValue(data);
                            break;
                        }
                        ++j;
                    }
                    
                    // while (data.GetType().GetProperty("curScore_min_" + j) != null || data.GetType().GetProperty("curScore_max_" + j) != null)
                    // {
                    //     var min = (int)data.GetType().GetProperty("curScore_min_" + j).GetValue(data);
                    //     var max = (int)data.GetType().GetProperty("curScore_max_" + j).GetValue(data);
                    //
                    //     if (slideNum >= min && slideNum < max)
                    //     {
                    //         clearRate = IsDangerous()
                    //             ? (int)data.GetType().GetProperty("rate_die_" + j).GetValue(data)
                    //             : (int)data.GetType().GetProperty("rate_notDie_" + j).GetValue(data);
                    //         break;
                    //     }
                    //     ++j;
                    // }
                    
                }
            }

            if (clearRate > 0)
            {
                var randomClearRateNum = Tools.GetNumFromRange(0, 100);
                DebugEx.Log("新手保护消除概率1", clearRate, randomClearRateNum);
                if (randomClearRateNum <= clearRate)
                {
                    if (_spc.data[0].rate_die > 1 || _spc.data[0].rate_notDie > 1)
                    {
                        var sumDie = 0;
                        var sumNotDie = 0;
                        foreach (var data in _spc.data)
                        {
                            sumDie += data.rate_die;
                            sumNotDie += data.rate_notDie;
                        }

                        var curSumDie = 0;
                        var curSumNotDie = 0;
                        foreach (var data in _spc.data)
                        {
                            curSumDie += data.rate_die;
                            curSumNotDie += data.rate_notDie;
                            data.rate_die = Tools.ChinaRound(curSumDie * 1.0f / sumDie * 100);
                            data.rate_notDie = Tools.ChinaRound(curSumNotDie * 1.0f / sumNotDie * 100);
                        }
                    }


                    var comboRate = Tools.GetNumFromRange(0, 100);
                    for (var i = 0; i < _spc.data.Length; ++i)
                    {
                        if (IsDangerous())
                        {
                            if (comboRate <= _spc.data[i].rate_die)
                            {
                                DebugEx.Log("新手保护消除概率2", comboRate);
                                return _spc.data[i].combo_num;
                            }
                        }
                        else
                        {
                            if (comboRate <= _spc.data[i].rate_notDie)
                            {
                                DebugEx.Log("新手保护消除概率2", comboRate);
                                return _spc.data[i].combo_num;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public static bool IsDangerous()
        {
            return GetHangNum() >= Constant.Hang - 1;
        }

        public static int GetB421Block()
        {
            //优先选择下面为空的长度4方块
            for (var posIndex = _blocksData.Length - 1; posIndex >= 0; --posIndex)
            {
                if (HaveBlock(posIndex))
                {
                    var data = GetBlockDataByIndex(posIndex);
                    if (data[(int)Key.Length] >= 4 && data[(int)Key.Pos] >= Constant.Lie)
                    {
                        for (var posIndex2 = data[(int)Key.Pos]; posIndex2 < data[(int)Key.Pos] + 4; ++posIndex2)
                        {
                            if (IsEmpty(posIndex2 - Constant.Lie))
                            {
                                return posIndex;
                            }
                        }
                    }
                }
            }

            //如果没有上述的长度4方块，则找最上面的4方块
            for (var posIndex = _blocksData.Length - 1; posIndex >= 0; --posIndex)
            {
                if (HaveBlock(posIndex))
                {
                    var data = GetBlockDataByIndex(posIndex);
                    if (data[(int)Key.Length] >= 4 && data[(int)Key.Pos] >= Constant.Lie)
                    {
                        return posIndex;
                    }
                }
            }
            return -1;
        }

        public static int[][] ChangeB421(int posIndex)
        {
            var blocksArr = new int[4][];
            if (HaveBlock(posIndex))
            {
                var data = GetBlockDataByIndex(posIndex);
                for (var i = 0; i < data[(int)Key.Length]; ++i)
                {
                    blocksArr[i] = new int[] { 1, data[(int)Key.Color], posIndex + i, data[(int)Key.Special], data[(int)Key.Ice], data[(int)Key.Prop] };
                }

                ClearBlockDataByIndex(posIndex);
                foreach (var newData in blocksArr)
                {
                    SetBlockDataByIndex(newData[(int)Key.Pos], newData);
                }
                return blocksArr;
            }
            return null;
        }

        public static void ChangeB43221(out bool[] delBlocks, out int[][] newBlocks)
        {
            delBlocks = new bool[Constant.Hang * Constant.Lie];
            newBlocks = new int[Constant.Hang * Constant.Lie][];

            var tmpArr = Constant.B43221BlockPro.Split(':');
            var block1Pro = int.Parse(tmpArr[0]);
            var block2Pro = int.Parse(tmpArr[1]);

            var newBlocks1 = new List<int[]>();
            var newBlocks2 = new List<int[]>();
            for (var posIndex = 0; posIndex < Constant.Hang * Constant.Lie; ++posIndex)
            {
                if (HaveBlock(posIndex))
                {
                    var data = _blocksData[posIndex];
                    var dLength = data[(int)Key.Length];
                    var dColor = data[(int)Key.Color];
                    var dSpecial = data[(int)Key.Special];
                    var dIce = data[(int)Key.Ice];
                    var dProp = data[(int)Key.Prop];
                    int[] tmpLength = null;
                    if (dLength == 3)
                    {
                        delBlocks[posIndex] = true;

                        if (Tools.GetNumFromRange(1, 2) == 1)
                        {
                            tmpLength = new[] { 1, 2 };
                        }
                        else
                        {
                            tmpLength = new[] { 2, 1 };
                        }
                    }
                    else if (dLength == 4)
                    {
                        delBlocks[posIndex] = true;

                        if (newBlocks2.Count * 1f / newBlocks1.Count < block2Pro * 1f / block1Pro)
                        {
                            tmpLength = new[] { 2, 2 };
                        }
                        else
                        {
                            var randomNum = Tools.GetNumFromRange(1, 3);
                            if (randomNum == 1)
                            {
                                tmpLength = new[] { 2, 1, 1 };
                            }
                            else if (randomNum == 2)
                            {
                                tmpLength = new[] { 1, 2, 1 };
                            }
                            else
                            {
                                tmpLength = new[] { 1, 1, 2 };
                            }
                        }
                    }

                    if (tmpLength != null)
                    {
                        var tmpPosIndex = posIndex;
                        foreach (var blockLength in tmpLength)
                        {
                            if (blockLength == 1)
                            {
                                newBlocks1.Add(new[] { blockLength, dColor, tmpPosIndex, dSpecial, dIce, dProp });
                                tmpPosIndex += blockLength;
                            }
                            else if (blockLength == 2)
                            {
                                newBlocks2.Add(new[] { blockLength, dColor, tmpPosIndex, dSpecial, dIce, dProp });
                                tmpPosIndex += blockLength;
                            }
                        }

                        ClearBlockDataByIndex(posIndex);
                    }
                }
            }

            foreach (var data in newBlocks1)
            {
                newBlocks[data[(int)Key.Pos]] = data;
                SetBlockDataByIndex(data[(int)Key.Pos], data);
            }
            foreach (var data in newBlocks2)
            {
                newBlocks[data[(int)Key.Pos]] = data;
                SetBlockDataByIndex(data[(int)Key.Pos], data);
            }
        }

        public static List<int> UseProp(string propId, out List<int[]> newBlocks)
        {
            newBlocks = null;

            switch (propId)
            {
                case Constant.Prop1:
                    var blockList = new List<int>();
                    for (var i = 0; i < _blocksData.Length; ++i)
                    {
                        if (HaveBlock(i))
                        {
                            blockList.Add(i);
                        }
                    }

                    var randomPosIndex = blockList[Tools.GetNumFromRange(0, blockList.Count - 1)];
                    if (HaveBlock(randomPosIndex))
                    {
                        var data = _blocksData[randomPosIndex];

                        //上下边缘块
                        var clearBlocks = ClearSpecialEdgeBlocks(data);

                        var curHang = GetHangByPos(randomPosIndex);
                        //左
                        if (GetHangByPos(randomPosIndex - 1) == curHang && !IsEmpty(randomPosIndex - 1))
                        {
                            for (var i = randomPosIndex - 1; i >= curHang * Constant.Lie; --i)
                            {
                                if (HaveBlock(i))
                                {
                                    if (HaveIce(i))
                                    {
                                        --_blocksData[i][(int)Key.Ice];
                                    }
                                    else
                                    {
                                        ClearBlockDataByIndex(i);
                                    }
                                    clearBlocks.Add(i);
                                    break;
                                }
                            }
                        }

                        //右
                        var rightPosIndex = randomPosIndex + data[(int)Key.Length];
                        if (GetHangByPos(rightPosIndex) == curHang &&
                            !IsEmpty(rightPosIndex) && HaveBlock(rightPosIndex))
                        {
                            if (HaveIce(rightPosIndex))
                            {
                                --_blocksData[rightPosIndex][(int)Key.Ice];
                            }
                            else
                            {
                                ClearBlockDataByIndex(rightPosIndex);
                            }
                            clearBlocks.Add(rightPosIndex);
                        }

                        //自身
                        if (HaveIce(randomPosIndex))
                        {
                            --_blocksData[randomPosIndex][(int)Key.Ice];
                        }
                        else
                        {
                            ClearBlockDataByIndex(randomPosIndex);
                        }
                        clearBlocks.Add(randomPosIndex);

                        UpdateMap();
                        return clearBlocks;
                    }
                    break;
                case Constant.Prop2:
                    var colorBlocks = new List<int>();
                    while (colorBlocks.Count <= 0)
                    {
                        var randomColor = Tools.GetNumFromRange((int)Color.Blue, (int)Color.Yellow);
                        for (var i = 0; i < _blocksData.Length; ++i)
                        {
                            if (!HaveSpecial(i) && HaveBlock(i) && _blocksData[i][(int)Key.Color] == randomColor)
                            {
                                if (HaveIce(i))
                                {
                                    --_blocksData[i][(int)Key.Ice];
                                }
                                else
                                {
                                    ClearBlockDataByIndex(i);
                                }
                                colorBlocks.Add(i);
                            }
                        }
                    }
                    UpdateMap();
                    return colorBlocks;
                case Constant.Prop3:
                case Constant.Prop4:
                    newBlocks = new List<int[]>();
                    var length4Blocks = new List<int>();
                    for (var i = 0; i < _blocksData.Length; ++i)
                    {
                        if (HaveBlock(i) && _blocksData[i][(int)Key.Length] >= 4)
                        {
                            length4Blocks.Add(i);
                        }
                    }

                    var clear4Blocks = new List<int>();
                    if (length4Blocks.Count > 3)
                    {
                        clear4Blocks.Add(length4Blocks[Tools.GetNumFromRange(0, length4Blocks.Count - 1)]);
                        clear4Blocks.Add(length4Blocks[Tools.GetNumFromRange(0, length4Blocks.Count - 1)]);
                        clear4Blocks.Add(length4Blocks[Tools.GetNumFromRange(0, length4Blocks.Count - 1)]);
                        while (clear4Blocks[1] == clear4Blocks[0])
                        {
                            clear4Blocks[1] = length4Blocks[Tools.GetNumFromRange(0, length4Blocks.Count - 1)];
                        }
                        while (clear4Blocks[2] == clear4Blocks[1])
                        {
                            clear4Blocks[2] = length4Blocks[Tools.GetNumFromRange(0, length4Blocks.Count - 1)];
                        }
                    }
                    else
                    {
                        clear4Blocks = length4Blocks;
                    }

                    foreach (var i in clear4Blocks)
                    {
                        var tmpColor = _blocksData[i][(int)Key.Color];
                        var tmpSpecial = _blocksData[i][(int)Key.Special];
                        var tmpIce = _blocksData[i][(int)Key.Ice];
                        var tmpProp = _blocksData[i][(int)Key.Prop];
                        ClearBlockDataByIndex(i);

                        if (propId == Constant.Prop3)
                        {
                            for (var posIndex = i; posIndex < i + 4; ++posIndex)
                            {
                                var newData = new[]
                                {
                                    1,
                                    tmpColor,
                                    posIndex,
                                    tmpSpecial,
                                    tmpIce,
                                    tmpProp
                                };
                                newBlocks.Add(newData);
                                SetBlockDataByIndex(posIndex, newData);
                            }
                        }
                        else if (propId == Constant.Prop4)
                        {
                            var newData = new[]
                            {
                                1,
                                tmpColor,
                                i,
                                tmpSpecial,
                                tmpIce,
                                tmpProp
                            };
                            newBlocks.Add(newData);
                            SetBlockDataByIndex(i, newData);
                        }
                    }
                    UpdateMap();
                    return clear4Blocks;
            }

            return null;
        }

        public static List<int> ClearColorBlocks()
        {
            var colorBlocks = new Dictionary<int, List<int>>();
            for (var i = (int)Color.Blue; i <= (int)Color.Yellow; ++i)
            {
                colorBlocks.Add(i, new List<int>());
            }

            for (var i = 0; i < _blocksData.Length; ++i)
            {
                if (!HaveSpecial(i) && HaveBlock(i))
                {
                    colorBlocks[_blocksData[i][(int)Key.Color]].Add(i);
                }
            }

            List<int> colorBlock = null;
            foreach (var item in colorBlocks)
            {
                if (item.Value.Count > 0)
                {
                    if (colorBlock == null)
                    {
                        colorBlock = item.Value;
                    }
                    else
                    {
                        if (item.Value.Count > colorBlock.Count)
                        {
                            colorBlock = item.Value;
                        }
                        else if (item.Value.Count == colorBlock.Count)
                        {
                            if (Tools.GetNumFromRange(0, 1) == 1)
                            {
                                colorBlock = item.Value;
                            }
                        }
                    }
                }
            }

            if (colorBlock == null)
            {
                return null;
            }

            foreach (var posIndex in colorBlock)
            {
                if (HaveIce(posIndex))
                {
                    --_blocksData[posIndex][(int)Key.Ice];
                }
                else
                {
                    ClearBlockDataByIndex(posIndex);
                }
            }

            UpdateMap();
            return colorBlock;
        }

        public static Vector3 GetLocalPosByPosIndex(int posIndex)
        {
            var x = GetLieByPos(posIndex) * Constant.BlockWidth + Constant.BlockGroupEdgeLeft;
            var y = GetHangByPos(posIndex) * Constant.BlockHeight + Constant.BlockGroupEdgeBottom;
            return new Vector3(x, y, 0);
        }

        public static int ChangeGoldToBronze()
        {
            for (var i = 0; i < Constant.Lie * Constant.Hang; ++i)
            {
                if (HaveBlock(i) && HaveSpecial(i))
                {
                    if (_blocksData[i][(int)Key.Special] == (int)Special.Gold)
                    {
                        _blocksData[i][(int)Key.Special] = (int)Special.Bronze;
                        return _blocksData[i][(int)Key.Pos];
                    }
                }
            }

            return -1;
        }

        public static int ChangeGoldToCommon()
        {
            for (var i = 0; i < Constant.Lie * Constant.Hang; ++i)
            {
                if (HaveBlock(i) && HaveSpecial(i))
                {
                    if (_blocksData[i][(int)Key.Special] == (int)Special.Gold)
                    {
                        _blocksData[i][(int)Key.Special] = 0;
                        return _blocksData[i][(int)Key.Pos];
                    }
                }
            }

            return -1;
        }

        public static int GetSpecialGoldEffType()
        {
            if (_blocksTest != null)
            {
                return _blocksTest.GetGoldEffType();
            }

            if (_specialGoldEffRandomRate == null)
            {
                var tmpGoldEffRandomArr = Constant.SpecialGoldEffWeight.Split(':');
                var sum = 0f;
                foreach (var t in tmpGoldEffRandomArr)
                {
                    sum += int.Parse(t);
                }

                _specialGoldEffRandomRate = new[]
                {
                    (int) Math.Ceiling(int.Parse(tmpGoldEffRandomArr[0]) / sum * 100),
                    (int) Math.Ceiling((int.Parse(tmpGoldEffRandomArr[0]) + int.Parse(tmpGoldEffRandomArr[1])) / sum * 100),
                    100
                };
            }

            if (_specialGoldEffRandomRate != null && _specialGoldEffRandomRate.Length > 0)
            {
                var randomNum = Tools.GetNumFromRange(1, 100);
                for (var i = 0; i < _specialGoldEffRandomRate.Length; ++i)
                {
                    if (randomNum <= _specialGoldEffRandomRate[i])
                    {
                        return i;
                    }
                }
            }

            return (int)SpecialGoldEffType.Color;
        }

        //金色块功能
        public static void SplitB421(out List<int> clearBlocks, out List<int[]> newBlocks, out List<int[]> edgeToNew)
        {
            clearBlocks = new List<int>();
            newBlocks = new List<int[]>();
            edgeToNew = new List<int[]>();

            var b4Arr = new List<int>();
            var b3Arr = new List<int>();
            for (var i = 0; i < Constant.Lie * Constant.Hang; ++i)
            {
                if (!HaveBlock(i)) continue;
                if (HaveSpecial(i)) continue;
                switch (_blocksData[i][(int)Key.Length])
                {
                    case 4:
                        b4Arr.Add(i);
                        break;
                    case 3:
                        b3Arr.Add(i);
                        break;
                }
            }

            var splitArr = b4Arr;
            var splitLen = 4;
            if (splitArr.Count <= 0)
            {
                splitArr = b3Arr;
                splitLen = 3;
            }

            if (_blocksTest != null)
            {
                if (splitLen == 4)
                {
                    splitArr.AddRange(b3Arr);
                }
            }

            if (splitArr.Count > 0)
            {
                foreach (var posIndex in splitArr)
                {
                    var tmpData = _blocksData[posIndex];
                    var tmpColor = tmpData[(int)Key.Color];
                    var tmpSpecial = tmpData[(int)Key.Special];
                    var tmpIce = tmpData[(int)Key.Ice];
                    var tmpProp = tmpData[(int)Key.Prop];
                    var tmpLength = tmpData[(int)Key.Length];

                    ClearBlockDataByIndex(posIndex);
                    clearBlocks.Add(posIndex);

                    var tmpEdgeToNew = new int[tmpLength];
                    for (var i = 0; i < tmpLength; ++i)
                    {
                        newBlocks.Add(new[]
                        {
                            1,
                            tmpColor,
                            posIndex + i,
                            tmpSpecial,
                            tmpIce,
                            tmpProp
                        });
                        SetBlockDataByIndex(posIndex + i, newBlocks[newBlocks.Count - 1]);
                        tmpEdgeToNew[i] = posIndex + i;
                    }

                    edgeToNew.Add(tmpEdgeToNew);
                }
            }
        }

        public static int GetSpecialScoreTimesBySpecialType(int specialType)
        {
            if (specialType == (int)Special.Stone)
            {
                return Constant.StoneScoreTimes;
            }

            return 2;
        }

        //随机石头
        public static List<int> GetStoneBlocks()
        {
            if (Constant.StoneSwitch)
            {
                if (Player.GetCurScore() >= Constant.StoneScoreGenerate && !BlockHaveStone())
                {
                    if (_curStoneHang >= Constant.StoneGenerateHang)
                    {
                        var canBeStoneBlocks = new List<int>();
                        for (var posIndex = 0; posIndex < Constant.Lie * Constant.Hang; ++posIndex)
                        {
                            if (HaveBlock(posIndex) && !HaveSpecial(posIndex) && !HaveIce(posIndex) && _blocksData[posIndex][(int)Key.Length] <= 2)
                            {
                                canBeStoneBlocks.Add(posIndex);
                            }
                        }

                        if (canBeStoneBlocks.Count > 0)
                        {
                            List<int> stoneBlocks = null;

                            var stoneIndex = canBeStoneBlocks[Tools.GetNumFromRange(0, canBeStoneBlocks.Count - 1)];
                            stoneBlocks = new List<int> { stoneIndex };

                            foreach (var stonePosIndex in stoneBlocks)
                            {
                                _blocksData[stonePosIndex][(int)Key.Special] = (int)Special.Stone;
                            }

                            _curStoneHang = 0;
                            ++Constant.GameStatusData.StoneCountGenerate;

                            DebugEx.Log("生成石头了");
                            DebugEx.LogObject(stoneBlocks);
                            return stoneBlocks;
                        }
                    }
                }
            }

            return null;
        }
    }
}
