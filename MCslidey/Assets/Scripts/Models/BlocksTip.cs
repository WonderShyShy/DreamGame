using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Other;

namespace Models
{
    public class BlocksTip
    {
        private string _oriBlocksData;
        private int[][] _blocksData;
        private int[] _blocksMap;

        public int[] GetClearTip(int[][] blocksData)
        {
            _oriBlocksData = JsonConvert.SerializeObject(blocksData);
            SetBlocksData(_oriBlocksData);

            for (var posIndex = 0; posIndex < _blocksData.Length; ++posIndex)
            {
                if (HaveBlock(posIndex) && !HaveStone(posIndex))
                {
                    var data = _blocksData[posIndex];
                    var edgeIndex = GetEdgeIndex(data);
                    var startDeltaX = edgeIndex[0] - GetLieByPos(posIndex);
                    var endDeltaX = edgeIndex[1] - GetLieByPos(posIndex);

                    int[] moveData;
                    for (var i = -1; i >= startDeltaX; --i)
                    {
                        moveData = new int[] {i, 0, posIndex};
                        MoveBlock(moveData);
                        var downBlocks = DownBlocks();
                        if (downBlocks != null && downBlocks.Count > 0)
                        {
                            var canClear = ClearBlocks(out var clearHang, out var clearSpecialBlocks);
                            if (canClear)
                            {
                                return moveData;
                            }
                        }
                        SetBlocksData(_oriBlocksData);
                    }

                    for (var i = 1; i <= endDeltaX; ++i)
                    {
                        moveData = new int[] {i, 0, posIndex};
                        MoveBlock(moveData);
                        var downBlocks = DownBlocks();
                        if (downBlocks != null && downBlocks.Count > 0)
                        {
                            var canClear = ClearBlocks(out var clearHang, out var clearSpecialBlocks);
                            if (canClear)
                            {
                                return moveData;
                            }
                        }
                        SetBlocksData(_oriBlocksData);
                    }
                }
            }
            return null;
        }

        public int[] GetClearTips(int[][] blocksData)
        {
            _oriBlocksData = JsonConvert.SerializeObject(blocksData);
            SetBlocksData(_oriBlocksData);

            var clearTipsA = new List<int[]>();
            var clearTipsB = new List<int[]>();
            var clearTipsC = new List<int[]>();
            for (var posIndex = 0; posIndex < _blocksData.Length; ++posIndex)
            {
                if (HaveBlock(posIndex) && !HaveStone(posIndex))
                {
                    var data = _blocksData[posIndex];
                    var edgeIndex = GetEdgeIndex(data);
                    var startDeltaX = edgeIndex[0] - GetLieByPos(posIndex);
                    var endDeltaX = edgeIndex[1] - GetLieByPos(posIndex);

                    int[] moveData;
                    for (var i = -1; i >= startDeltaX; --i)
                    {
                        moveData = new int[] {i, 0, posIndex};
                        MoveBlock(moveData);
                        var downBlocks = DownBlocks();
                        if (downBlocks != null && downBlocks.Count > 0)
                        {
                            var canClear = ClearBlocks(out var clearHang, out var clearSpecialBlocks);
                            if (canClear)
                            {
                                var curHang = GetHangByPos(moveData[2]);
                                foreach (var hang in clearHang)
                                {
                                    if (curHang - 1 == hang)
                                    {
                                        clearTipsA.Add(moveData);
                                    } 
                                    else if (curHang == hang)
                                    {
                                        clearTipsB.Add(moveData);
                                    } 
                                    else
                                    {
                                        clearTipsC.Add(moveData);
                                    }
                                }
                            }
                        }
                        SetBlocksData(_oriBlocksData);
                    }

                    for (var i = 1; i <= endDeltaX; ++i)
                    {
                        moveData = new int[] {i, 0, posIndex};
                        MoveBlock(moveData);
                        var downBlocks = DownBlocks();
                        if (downBlocks != null && downBlocks.Count > 0)
                        {
                            var canClear = ClearBlocks(out var clearHang, out var clearSpecialBlocks);
                            if (canClear)
                            {
                                var curHang = GetHangByPos(moveData[2]);
                                foreach (var hang in clearHang)
                                {
                                    if (curHang - 1 == hang)
                                    {
                                        clearTipsA.Add(moveData);
                                    } 
                                    else if (curHang == hang)
                                    {
                                        clearTipsB.Add(moveData);
                                    } 
                                    else
                                    {
                                        clearTipsC.Add(moveData);
                                    }
                                }
                            }
                        }
                        SetBlocksData(_oriBlocksData);
                    }
                }
            }

            DebugEx.Log("可消除列表");
            DebugEx.LogObject(clearTipsA);
            DebugEx.LogObject(clearTipsB);
            DebugEx.LogObject(clearTipsC);
            if (clearTipsC.Count > 0)
            {
                DebugEx.Log("消除提示C");
                return clearTipsC[0];
            }
            
            if (clearTipsB.Count > 0)
            {
                DebugEx.Log("消除提示B");
                return clearTipsB[0];
            }
            
            if (clearTipsA.Count > 0)
            {
                DebugEx.Log("消除提示A");
                return clearTipsA[0];
            }
            return null;
        }

        public int CheckGoldWillClear(int[][] blocksData, int[] moveData)
        {
            _oriBlocksData = JsonConvert.SerializeObject(blocksData);
            SetBlocksData(_oriBlocksData);

            var oriGoldPosIndex = -1;
            for (var i = 0; i < Constant.Lie * Constant.Hang; ++i)
            {
                if (_blocksData[i][(int) Blocks.Key.Special] == (int) Blocks.Special.Gold)
                {
                    oriGoldPosIndex = _blocksData[i][(int) Blocks.Key.Pos];
                }
            }            
            
            MoveBlock(moveData);
            var downBlocks = DownBlocks();
            if (downBlocks != null && downBlocks.Count > 0)
            {
                var canClear = ClearBlocks(out var clearHang, out var clearSpecialBlocks);
                if (canClear)
                {
                    for (var i = 0; i < clearSpecialBlocks.Count; i++)
                    {
                        var data = clearSpecialBlocks[i];
                        if (data[(int)Blocks.Key.Special] == (int) Blocks.Special.Gold && oriGoldPosIndex != -1)
                        {
                            return oriGoldPosIndex;
                        }
                    }
                }
            }
            
            return -1;
        }
        
        private void SetBlocksData(string oriBlocksData)
        {
            _blocksData = JsonConvert.DeserializeObject<int[][]>(oriBlocksData);
            UpdateMap();
        }

        //跟Blocks里方法相同，需要直接复制粘贴
        private void UpdateMap()
        {
            _blocksMap = new int[Constant.Lie * Constant.Hang];
            foreach (var data in _blocksData)
            {
                if (data != null && data[(int) Blocks.Key.Length] > 0)
                {
                    for (var i = 0; i < data[(int) Blocks.Key.Length]; ++i)
                    {
                        _blocksMap[i + data[(int) Blocks.Key.Pos]] = 1;
                    }
                }
            }
        }
        
        private bool IsEmpty(int index)
        {
            if (index >= _blocksMap.Length || index < 0)
            {
                return false;
            }
            return _blocksMap[index] == 0;
        }
        
        private bool HaveBlock(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int) Blocks.Key.Length] != 0;
        }
        
        private int[] GetEdgeIndex(int[] blockData)
        {
            var curHang = GetHangByPos(blockData[(int) Blocks.Key.Pos]);
            var curLie = GetLieByPos(blockData[(int) Blocks.Key.Pos]);
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
            for (var i = curLie + blockData[(int) Blocks.Key.Length]; i < Constant.Lie; ++i)
            {
                if (IsEmpty(curHang * Constant.Lie + i))
                {
                    max = i - blockData[(int) Blocks.Key.Length] + 1;
                }
                else
                {
                    break;
                }
            }
            return new[] {min, max};
        }
        
        private int GetHangByPos(int posIndex)
        {
            return (int)Math.Floor((float) posIndex / Constant.Lie);
        }
        
        private static int GetLieByPos(int posIndex)
        {
            return posIndex % Constant.Lie;
        }
        
        public void MoveBlock(int[] moveData)
        {
            var offsetX = moveData[0];
            var offsetY = moveData[1];
            var posIndex = moveData[2];
            if (offsetX != 0)
            {
                _blocksData[posIndex][(int) Blocks.Key.Pos] += offsetX;
                SetBlockDataByIndex(_blocksData[posIndex][(int)Blocks.Key.Pos], _blocksData[posIndex]);
                ClearBlockDataByIndex(_blocksData[posIndex][(int)Blocks.Key.Pos] - offsetX);
            }

            if (offsetY != 0)
            {
                _blocksData[posIndex][(int) Blocks.Key.Pos] += Constant.Lie * offsetY;
                SetBlockDataByIndex(_blocksData[posIndex][(int) Blocks.Key.Pos], _blocksData[posIndex]);
                ClearBlockDataByIndex(_blocksData[posIndex][(int)Blocks.Key.Pos] - Constant.Lie * offsetY);
            }
            UpdateMap();
        }
        
        private void SetBlockDataByIndex(int index, int[] data)
        {
            _blocksData[index] = data;
        }
    
        private void ClearBlockDataByIndex(int index)
        {
            _blocksData[index] = new int[Enum.GetNames(typeof(Blocks.Key)).Length];
        }
        
        private List<int[]> DownBlocks()
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
                        var endIndex = startIndex + _blocksData[i][(int) Blocks.Key.Length];
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
                        downList.Add(new int[] {deltaY, i});
                        MoveBlock(new int[] {0, deltaY, i});
                    }
                }
            }
            return downList;
        }

        private bool ClearBlocks(out List<int> clearHang, out List<int[]> clearSpecialBlocks)
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
                            if (HaveSpecial(posIndex))
                            {
                                clearSpecialBlocks.Add(_blocksData[posIndex]);
                            }

                            if (HaveIce(posIndex))
                            {
                                --_blocksData[posIndex][(int) Blocks.Key.Ice];
                            }
                            else
                            {
                                ClearBlockDataByIndex(posIndex);
                            }
                        }
                    }
                }
            }
            UpdateMap();
            return clearHang.Count > 0;
        }
        
        private bool HaveSpecial(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int) Blocks.Key.Special] != 0;
        }

        private bool HaveIce(int posIndex)
        {
            if (posIndex >= _blocksData.Length || posIndex < 0)
            {
                return false;
            }
            return _blocksData[posIndex] != null && _blocksData[posIndex][(int) Blocks.Key.Ice] >= 2;
        }

        private bool HaveStone(int posIndex)
        {
            return HaveBlock(posIndex) &&
                    _blocksData[posIndex][(int) Blocks.Key.Special] == (int) Blocks.Special.Stone;
        }
    }
}
