using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Manager;
using Models;
using Newtonsoft.Json;
using Other;
using UnityEngine;

namespace UI
{
    public class GamePlayDialog : MonoBehaviour
    {
        public GameObject blockGroup;
        public GameObject readyGroup;

        //移动方块时背景提示
        public GameObject blockBgLightEff;
        public GameObject blockBgTip;
        public GameObject blockLightTip;

        public GameObject menuGroup;
        public GameObject scoreGroup;

        public GameObject bgGroup;

        private readonly List<List<int[]>> _readyBlocksData = new List<List<int[]>>();
        private readonly GameObject[] _itemList = new GameObject[Constant.Lie * Constant.Hang];
        private bool _userMove;
        private int _userMoveLevel = 100;
        private int _unitBlockScore;
        private int _clearCombo;
        private int _clearComboSound;
        private int _spComboNum;

        private bool _firstTimeSlide = false;
        private bool _firstTimeRemove = false;
        private bool _isRestart = false;

        private bool _downBlockItemNull = false;

        private float _clearTipTime;

        private ObjectPool _blockItemsPool;
        private GameObject _movedBlockItem = null;

        private GameObject _b421BlockItem = null;
        private int _stepScore = 0;

        private GameObject _specialGoldItem = null;
        private int _specialGoldEffNoNewBlocksTime = 0;
        private Ease _easeEff = Ease.OutSine;
        //        private Ease _easeEff = Ease.InOutCubic;

        void CreateBlockBgLightEff()
        {
            blockBgTip.transform.localScale = Vector2.one;
            blockBgTip.SetActive(false);

            blockBgLightEff.transform.localScale = Vector2.one;
            blockBgLightEff.SetActive(false);

            if (Constant.SceneVersion == "3")
            {
                blockLightTip.transform.localScale = Vector2.one;
                blockLightTip.SetActive(false);
            }
        }

        void RemoveAllBlockItems()
        {
            ResetClearTipTime();
            for (var i = 0; i < _itemList.Length; ++i)
            {
                if (_itemList[i] != null)
                {
                    Constant.EffCtrlScript.RemoveBlockItemGrayEff(_itemList[i]);
                    RemoveBlockItemByIndex(i, false);
                }
            }
        }

        void CreateBlocks(int[][] blocksData, bool specialGoldSuccess = false)
        {
            for (var i = blocksData.Length - 1; i >= 0; --i)
            {
                if (blocksData[i][(int)Blocks.Key.Length] != 0)
                {
                    CreateBlockItem(blocksData[i], specialGoldSuccess);
                }
            }
        }

        private async Task<bool> LoadResAsync_BlockItem()
        {
            //初始化方块对象池
            if (_blockItemsPool == null)
            {
                _blockItemsPool = new ObjectPool();
                var blockPrefab = MaterPool.Instace.BlockItem;
                _blockItemsPool.CreateObject(blockPrefab, 20);
            }

            return true;
        }

        async Task<bool> LoadRes()
        {
            await LoadResAsync_BlockItem();

            //初始化readyGroup
            readyGroup.GetComponent<ReadyGroup>().InitRes();

            if ((Constant.SceneVersion == "1" || Constant.SceneVersion == "2") && Constant.IsDeviceSoHeight)
            {
                var topUITransform = gameObject.transform.Find("topUI");
                topUITransform.localPosition = new Vector2(0, topUITransform.localPosition.y + 50);
            }

            return true;
        }

        // Start is called before the first frame update
        async void Start()
        {
            //创建移动方块时背景提示
            CreateBlockBgLightEff();

            //新一局开始，如果有开场特效，则隐藏一下棋盘
            if (Constant.SceneVersion == "3")
            {
                if (!ManagerLocalData.HaveData(ManagerLocalData.BLOCKS_DATA))
                {
                    Constant.EffCtrlScript.blockGroupBg.SetActive(false);
                    Constant.EffCtrlScript.readyGroup.SetActive(false);
                }
            }

            gameObject.AddComponent<InputModel>();
            await Blocks.InitData();
            UpdateScoreUi(false);

            await LoadRes();
            //继续游戏
            Blocks.UpdateLengthRate(Constant.GameStatusData.SlideNumber);

            if (Constant.GameStatusData.DWAD_REMAINING_STEP > 0)
            {
                ++Constant.GameStatusData.DWAD_REMAINING_STEP;
            }

            ResetData();

            if (Constant.GameStatusData.DWAD_REMAINING_STEP > 0)
            {
                Constant.EffCtrlScript.AddReadyBgIceEff(readyGroup, false);
            }

            StartCoroutine(Delay.Run(() => { Blocks.CheckSpecialProtect(); }, 2));

            if (!ManagerLocalData.HaveData(ManagerLocalData.AF_PLAY_DAY) &&
                Player.GetLoginDay() >= Constant.AF_PLAY_DAY)
            {
                ManagerLocalData.SetStringData(ManagerLocalData.AF_PLAY_DAY, "true");
            }

            //冰块提示，如果该用户已经超过4000分，则说明已经出现过冰块提示，不再需要了
            if (Player.ShouldShowIceTip() && (Player.GetBestScore() >= 4000 || Player.GetCurScore() >= 4000))
            {
                Player.SetAlreadyShowIceTip();
            }

            CheckShowTopUIMask(true);

            ManagerAudio.SetMusicVolume(0.2f);
            ManagerAudio.PlayMusic("bg");
            ManagerAudio.SetSoundVolume(1.0f);

            if (!Player.IsInGuide())
            {
                ShowBanner();
            }

            //
            SendEvent();

            var splashImg = GameObject.Find("SplashImg");
            if (splashImg != null)
            {
                Destroy(splashImg.gameObject);
            }

            Constant.EffCtrlScript.LoadResAsync_AfterLoadScene();
        }

        private void SendEvent()
        {

        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                Player.AddLoginDay();
            }
        }


        private void ShowBanner()
        {
//            if (Blocks.IsTesting()) return;
//            StartCoroutine(Delay.Run(() => { AppLovinMediationAdapter.Instance.OnGuideComplete(); }, 5f));
            
            // todo txy 
        }

        private void CheckShowTopUIMask(bool isStart = false)
        {
            return;
            //顶部流光特效
            if (Constant.SceneVersion == "3")
            {
                var topUIObj = gameObject.transform.Find("topUI").transform.Find("topMask").gameObject;

                if (Player.IsInGuide())
                {
                    topUIObj.SetActive(false);
                }
                else
                {
                    if (!topUIObj.activeInHierarchy)
                    {
                        topUIObj.SetActive(true);
                        topUIObj.GetComponent<TopMask>().StartShow();
                    }

                    if (isStart)
                    {
                        topUIObj.GetComponent<TopMask>().StartShow();
                    }
                }
            }
        }

        public void RestartGame(bool isRestart = false)
        {

            _isRestart = isRestart;
            if (_isRestart)
            {
            }

            ClearBlocksData();
            ResetData();
        }

        private void ResetData()
        {
            Constant.GameStatusData.IsFirstGetBestScorePreGame = true;
            
            Player.AddLoginDay();
            Player.UserCanMove = false;
            _unitBlockScore = 1;
            _clearCombo = -1;
            _clearComboSound = -1;

            //继续游戏
            if (ManagerLocalData.HaveData(ManagerLocalData.BLOCKS_DATA))
            {
                var blocksData = ManagerLocalData.GetTableData<int[][]>(ManagerLocalData.BLOCKS_DATA);
                if (!Player.IsInGuide())
                {
                    //设置方块数据
                    Blocks.SetBlocksData(blocksData);

                    //检测特殊块开关，如果开关关闭，但仍有特殊块，则把特殊块置为普通块
                    blocksData = Blocks.CheckSpecialBlockSwitch();

                    CreateBlocks(blocksData);
                }
                else
                {
                    Blocks.ResetData();
                }

                //新手引导第二部特殊处理
                if (Player.IsInGuide() && Player.GetGuideStep() == 1)
                {
                    var nextData = Blocks.GetNextBlocksData();
                    nextData.RemoveAt(1);
                    _readyBlocksData.Add(nextData);
                }
                else
                {
                    _readyBlocksData.Add(Blocks.GetNextBlocksData());
                }
                _readyBlocksData.Add(Blocks.GetNextBlocksData());

                AddReadyBlockItems();
                MoveEnd();
            }
            else
            {
                //新一局
                _stepScore = 0;
                Player.SetCurScore(0);
                Player.ResetNewBestStatus();
                Player.ResetSecondChanceUsed();
                Constant.GameStatusData = new GameStatus();
                Blocks.UpdateLengthRate(Constant.GameStatusData.SlideNumber);
                Blocks.ResetData();
                Blocks.UpdateLevelByScore(Player.GetCurScore());

                scoreGroup.GetComponent<ScoreGroup>().ResetScoreUi();
                ResetClearTipTime();
                Constant.EffCtrlScript.RemoveDeadWarning();
                Constant.EffCtrlScript.RemoveB421Eff();
                Constant.EffCtrlScript.RemoveReadyBgIceEff(readyGroup, false);
                Constant.GameStatusData.DWAD_REMAINING_STEP = 0;
                if (Constant.B43221Switch && ManagerDialog.IsExistDialog("B43221Dialog") != null)
                {
                    var dialog = ManagerDialog.IsExistDialog("B43221Dialog");
                    dialog.GetComponent<B43221Dialog>().OnBtnClk("close");
                }

                RemoveAllBlockItems();
                _readyBlocksData.Clear();
                _readyBlocksData.Add(Blocks.GetNextBlocksData());
                _readyBlocksData.Add(Blocks.GetNextBlocksData());
                readyGroup.GetComponent<ReadyGroup>().UpdateReadyGroup(_readyBlocksData[0]);

                Constant.EffCtrlScript.ShowStartAnim(() =>
                {
                    AddBlockItems();
                    StartCoroutine(Delay.Run(() =>
                        {
                            AddBlockItems();
                            StartCoroutine(Delay.Run(() => { MoveEnd(); }, Constant.UpAnimTime + 0.01f));
                        }, Constant.UpAnimTime + 0.01f));

                    Constant.GameStatusData.InningTime = 0.00f;

                    if (_isRestart)
                    {
                        _isRestart = false;
                        Constant.GameStatusData.IsRestart = true;
                    }
                });
            }

            UpdateScoreUi(false);

            if (Constant.LevelUpOtherEffSwitch)
            {
                bgGroup.GetComponent<BgGroupEff>().UpdateEffByLevel(Blocks.CurLevel - 1);
            }

            if (Constant.SceneVersion != "3" && Player.ShouldShowRate())
            {
                ShowRateDialog();
            }
        }

        public void MoveEnd(int[] moveData = null, string previousBlocks = "", bool specialGoldSuccess = false, bool autoClear = true)
        {
            if (Blocks.GetHangNum() <= 0 && Blocks.GetMaxHangNumByScore(Player.GetCurScore()) >= 3)
            {
                ++Constant.GameStatusData.AllClearCount;
                //                if (Constant.GameStatusData.AllClearCount == 1)
                //                {
                //                    Constant.AchievementScript.AddAchievementTip("CLEAR ALL", 1);
                //                }
            }

            ResetClearTipTime();
            Player.UserCanMove = false;

            if (_movedBlockItem != null)
            {
                _movedBlockItem.GetComponent<BlockItem>().IsMovedBlockItem = false;
                _movedBlockItem.GetComponent<BlockItem>().OriHang = 0;
                _movedBlockItem = null;
            }

            //用户操作移动
            if (moveData != null)
            {
                _movedBlockItem = _itemList[moveData[2]];

                //如果用户移动的方块出了bug，则重置所有方块，避免无法移动造成卡死
                if (_movedBlockItem == null)
                {
                    RemoveAllBlockItems();
                    CreateBlocks(Blocks.GetBlocksData());
                    MoveEnd();
                    return;
                }

                _movedBlockItem.GetComponent<BlockItem>().IsMovedBlockItem = true;
                _movedBlockItem.GetComponent<BlockItem>().OriHang =
                    Blocks.GetHangByPos(_movedBlockItem.GetComponent<BlockItem>().GetPosIndex());

                ManagerAudio.PlaySound("userMove");

                _userMove = true;

                //moveData[0]，x移动，moveData[1]，y移动，moveData[2]，需要移动的方块，
                Blocks.MoveBlock(moveData);
                _itemList[moveData[2] + moveData[0]] = _itemList[moveData[2]];
                _itemList[moveData[2]] = null;

                ++Constant.GameStatusData.SlideNumber;
                if (Player.IsSecondChanceUsed())
                {
                    ++Constant.GameStatusData.ContinueSlide;
                }

                Blocks.UpdateLengthRate(Constant.GameStatusData.SlideNumber);

                menuGroup.GetComponent<MenuGroup>().AutoCloseMenuGroup();

                if (!_firstTimeSlide)
                {
                    _firstTimeSlide = true;
                }

                var b43221Dialog = ManagerDialog.IsExistDialog("B43221Dialog");
                if (b43221Dialog != null)
                {
                    b43221Dialog.GetComponent<B43221Dialog>().CheckRemove();
                }

                _userMoveLevel = Blocks.CurLevel;
            }

            var goldToBronzePosIndex = -1;
            if (previousBlocks != "")
            {
                Blocks.SetBlocksData(JsonConvert.DeserializeObject<int[][]>(previousBlocks));

                //失败，则金变青铜
                if (!specialGoldSuccess)
                {
                    //                    goldToBronzePosIndex = Blocks.ChangeGoldToBronze();
                    Blocks.ChangeGoldToCommon();
                }
                else
                {
                    Constant.GameStatusData.ScoreWhenSpecialGoldClear = Player.GetCurScore();
                }

                RemoveAllBlockItems();
                CreateBlocks(Blocks.GetBlocksData(), specialGoldSuccess);
            }


            List<int[]> downBlocks = new List<int[]>();
            bool canClear = false;
            List<int> clearHang = new List<int>();
            List<int[]> clearSpecialBlocks = new List<int[]>();
            if (autoClear)
            {
                downBlocks = Blocks.DownBlocks();
                if (Constant.SpecialGoldSwitch)
                {
                    Constant.PreviousBlocks = JsonConvert.SerializeObject(Blocks.GetBlocksData());
                }
                canClear = Blocks.ClearBlocks(out clearHang, out clearSpecialBlocks);
            }

            if (downBlocks.Count > 0 || canClear)
            {
                DownBlockItemsAnim(downBlocks);
                StartCoroutine(Delay.Run(() =>
                    {
                        if (canClear)
                        {
                            if (!_firstTimeRemove)
                            {
                                _firstTimeRemove = true;
                            }
                            Constant.GameStatusData.RemoveNumber += clearHang.Count;
                            if (Player.IsSecondChanceUsed())
                            {
                                Constant.GameStatusData.ContinueRemove += clearHang.Count;
                            }

                            //彩色块边缘块
                            var clearSpecialEdgeBlocks = new List<int>();
                            //青铜块边缘和生成的新方块
                            List<int[]> newBlocks = null;
                            List<int> clearSpecialEdgeBlocksBronze = null;
                            Dictionary<int, List<int[]>> specialEdgeBronzeToNew = null;
                            //金色块标识
                            var isSpecialGold = false;

                            GameObject specialGoldItem = null;
                            bool shouldVibrator = true;

                            foreach (var specialData in clearSpecialBlocks)
                            {
                                switch (specialData[(int)Blocks.Key.Special])
                                {
                                    case (int)Blocks.Special.Rainbow:
                                        clearSpecialEdgeBlocks.AddRange(Blocks.ClearSpecialEdgeBlocks(specialData));
                                        break;
                                    case (int)Blocks.Special.Bronze:
                                        if (clearSpecialEdgeBlocksBronze == null)
                                        {
                                            clearSpecialEdgeBlocksBronze = new List<int>();
                                        }
                                        clearSpecialEdgeBlocksBronze.AddRange(Blocks.SplitSpecialEdgeBlocks(specialData, out var tmpNewBlocks, out var tmpEdgeToNew));

                                        if (tmpNewBlocks.Count > 0)
                                        {
                                            shouldVibrator = false;
                                        }
                                        
                                        if (newBlocks == null)
                                        {
                                            newBlocks = new List<int[]>();
                                        }
                                        newBlocks.AddRange(tmpNewBlocks);

                                        if (specialEdgeBronzeToNew == null)
                                        {
                                            specialEdgeBronzeToNew = new Dictionary<int, List<int[]>>();
                                        }
                                        specialEdgeBronzeToNew.Add(specialData[(int)Blocks.Key.Pos], tmpEdgeToNew);
                                        break;
                                    case (int)Blocks.Special.Gold:
                                        isSpecialGold = true;
                                        specialGoldItem = _itemList[specialData[(int) Blocks.Key.Pos]];
                                        shouldVibrator = false;
                                        break;
                                }
                            }

                            //连锁消除//只有彩块会触发其他特殊块连锁消除
                            if (clearSpecialEdgeBlocks.Count > 0)
                            {
                                var tmpSpecialEdge = new List<int>();
                                while (clearSpecialEdgeBlocks.Find(posIndex =>
                                    _itemList[posIndex] != null && _itemList[posIndex].GetComponent<BlockItem>().IsSpecial()) > 0)
                                {
                                    var clearSpecialEdge2 = new List<int>();
                                    for (var i = clearSpecialEdgeBlocks.Count - 1; i >= 0; --i)
                                    {
                                        if (_itemList[clearSpecialEdgeBlocks[i]] != null && _itemList[clearSpecialEdgeBlocks[i]].GetComponent<BlockItem>().IsSpecial())
                                        {
                                            var itemScript = _itemList[clearSpecialEdgeBlocks[i]]
                                                .GetComponent<BlockItem>();
                                            switch (itemScript.GetSpecial())
                                            {
                                                case (int)Blocks.Special.Rainbow:
                                                    clearSpecialEdge2.AddRange(Blocks.ClearSpecialEdgeBlocks(_itemList[clearSpecialEdgeBlocks[i]].GetComponent<BlockItem>().GetData()));
                                                    break;
                                                case (int)Blocks.Special.Bronze:
                                                    if (clearSpecialEdgeBlocksBronze == null)
                                                    {
                                                        clearSpecialEdgeBlocksBronze = new List<int>();
                                                    }
                                                    clearSpecialEdgeBlocksBronze.AddRange(Blocks.SplitSpecialEdgeBlocks(itemScript.GetData(), out var tmpNewBlocks, out var tmpEdgeToNew));

                                                    if (newBlocks == null)
                                                    {
                                                        newBlocks = new List<int[]>();
                                                    }
                                                    newBlocks.AddRange(tmpNewBlocks);

                                                    if (specialEdgeBronzeToNew == null)
                                                    {
                                                        specialEdgeBronzeToNew = new Dictionary<int, List<int[]>>();
                                                    }
                                                    specialEdgeBronzeToNew.Add(itemScript.GetPosIndex(), tmpEdgeToNew);
                                                    break;
                                                case (int)Blocks.Special.Gold:
                                                    isSpecialGold = true;
                                                    specialGoldItem = _itemList[clearSpecialEdgeBlocks[i]];
                                                    break;
                                            }

                                            tmpSpecialEdge.Add(clearSpecialEdgeBlocks[i]);
                                            clearSpecialEdgeBlocks.RemoveAt(i);
                                        }
                                    }

                                    clearSpecialEdgeBlocks.AddRange(clearSpecialEdge2);
                                }
                                clearSpecialEdgeBlocks.AddRange(tmpSpecialEdge);
                            }

                            //消除金色前播放视频                            
                            if (previousBlocks == "" && isSpecialGold)
                            {
                                if (!Constant.SpecialGoldAdClear || Blocks.IsTesting())
                                {
                                    MoveEnd(null, Constant.PreviousBlocks, true);
                                    return;
                                }

                                if (specialGoldItem != null)
                                {
                                    StartCoroutine(Delay.Run(() =>
                                    {
                                        Constant.EffCtrlScript.ShowClearSpecialEdgeEff(specialGoldItem);
                                        Constant.EffCtrlScript.RemoveSpecialEffGoldCountDown(specialGoldItem);
                                    },
                                    Constant.ClearWaitTime));
                                }

                                StartCoroutine(Delay.Run(() =>
                                {
                                    if (Blocks.IsTesting())
                                    {
                                        MoveEnd(null, Constant.PreviousBlocks, true);
                                    }
                                    else
                                    {
                                        var pos = specialGoldItem.transform.localPosition + specialGoldItem.transform.Find("img").transform.localPosition;
                                        ShowSpecialGoldDialog(pos);
                                    }
                                }, 1f));
                                return;
                            }

                            var clearWaitTime = Constant.ClearWaitTime;
                            var effWaitTime = 0f;

                            List<int> clearSpecialEdgeBlocksGold = null;
                            List<int> specialGoldEffSplitBlocks = null;
                            List<int[]> specialGoldEffSplitBlocksNew = null;
                            List<int[]> specialGoldEffSplitEdgeToNew = null;
                            if (previousBlocks != "" && isSpecialGold && specialGoldSuccess)
                            {
                                Constant.GameStatusData.SpecialGoldShake++;
                                var specialGoldEffType = Blocks.GetSpecialGoldEffType();
                                switch (specialGoldEffType)
                                {
                                    case (int)Blocks.SpecialGoldEffType.Color:
                                        clearSpecialEdgeBlocksGold = Blocks.ClearColorBlocks();
                                        break;
                                    case (int)Blocks.SpecialGoldEffType.Split43:
                                        Blocks.SplitB421(out specialGoldEffSplitBlocks, out specialGoldEffSplitBlocksNew, out specialGoldEffSplitEdgeToNew);
                                        break;
                                    case (int)Blocks.SpecialGoldEffType.NoNewBlocks:
                                        _specialGoldEffNoNewBlocksTime = Constant.SpecialGoldEffNoNewBlocksTime;
                                        break;
                                }

                                if (specialGoldItem != null && !Blocks.IsTesting())
                                {
                                    Constant.EffCtrlScript.ShowClearSpecialEdgeEff(specialGoldItem);
                                    if (Constant.SceneVersion == "2")
                                    {
                                        clearWaitTime += 0.5f;
                                        effWaitTime += 0.5f;
                                    }
                                }
                            }

                            if (clearSpecialBlocks.Count > 0)
                            {
                                if (Constant.SceneVersion == "2")
                                {
                                    if (clearSpecialEdgeBlocksGold != null && clearSpecialEdgeBlocksGold.Count > 0)
                                    {
                                        clearWaitTime += Constant.SpecialGoldClearTime +
                                                         clearSpecialEdgeBlocksGold.Count * Constant.SpecialGoldClearIntervalTime;
                                    }
                                    else if (clearSpecialEdgeBlocksBronze != null && clearSpecialEdgeBlocksBronze.Count > 0)
                                    {
                                        clearWaitTime += Constant.SpecialClearTime - 0.5f;
                                    }
                                }
                                else if (Constant.SceneVersion == "3")
                                {
                                    var bronzeTime = 0f;
                                    var goldTime = 0f;
                                    if (clearSpecialEdgeBlocksGold != null && clearSpecialEdgeBlocksGold.Count > 0)
                                    {
                                        goldTime += Constant.SpecialGoldClearTime +
                                                         clearSpecialEdgeBlocksGold.Count * Constant.SpecialGoldClearIntervalTime;
                                    }
                                    else if (clearSpecialEdgeBlocksBronze != null && clearSpecialEdgeBlocksBronze.Count > 0)
                                    {
                                        bronzeTime += Constant.SpecialClearTime + 0.2f;
                                    }

                                    clearWaitTime += Math.Max(goldTime, bronzeTime);
                                }

                                clearWaitTime += Constant.SpecialClearTime;

                                StartCoroutine(Delay.Run(() =>
                                {
                                    foreach (var specialData in clearSpecialBlocks)
                                    {
                                        if (specialData[(int)Blocks.Key.Special] == (int)Blocks.Special.Rainbow)
                                        {
                                            //彩色块消除时的特效
                                            Constant.EffCtrlScript.ShowClearSpecialEff(_itemList[specialData[(int)Blocks.Key.Pos]]);
                                        }
                                    }

                                    ManagerAudio.PlaySound("thunder");

                                    //彩色块边缘块特效
                                    if (clearSpecialEdgeBlocks.Count > 0)
                                    {
                                        foreach (var posIndex in clearSpecialEdgeBlocks)
                                        {
                                            if (_itemList[posIndex] != null)
                                            {
                                                if (_itemList[posIndex].GetComponent<BlockItem>().IsSpecial())
                                                {
                                                    if (_itemList[posIndex].GetComponent<BlockItem>().GetSpecial() ==
                                                        (int)Blocks.Special.Rainbow)
                                                    {
                                                        Constant.EffCtrlScript.ShowClearSpecialEff(_itemList[posIndex]);
                                                    }
                                                }
                                                else
                                                {
                                                    Constant.EffCtrlScript.ShowClearSpecialEdgeEff(_itemList[posIndex]);
                                                }
                                            }
                                        }
                                    }

                                    //青铜块消除和青铜块边缘块碎裂特效
                                    if (specialEdgeBronzeToNew != null)
                                    {
                                        foreach (var item in specialEdgeBronzeToNew)
                                        {
                                            //金色变成的青铜不计入青铜数
                                            if (goldToBronzePosIndex != -1 && goldToBronzePosIndex == item.Key)
                                            {
                                                //do nothing
                                            }
                                            else
                                            {
                                                ++Constant.GameStatusData.SpecialBronze;
                                            }
                                            Constant.EffCtrlScript.ShowClearSpecialEffBronze(item.Key, item.Value);
                                        }
                                    }

                                    //金色消除后冰冻readyGroup
                                    if (specialGoldItem != null && _specialGoldEffNoNewBlocksTime > 0)
                                    {
                                        ++Constant.GameStatusData.SpecialGold;
                                        if (specialGoldItem != null)
                                        {
                                            Constant.EffCtrlScript.ShowClearSpecialEffGoldOnly(specialGoldItem);

                                            if (Constant.VibratorSwitch)
                                            {
                                                Tools.DoVibrator(Constant.VibratorTime, Constant.VibratorAmplitude);
                                            }
                                        }
                                    }

                                    //金色切割边缘块特效
                                    if (specialGoldEffSplitEdgeToNew != null && specialGoldItem != null)
                                    {
                                        ++Constant.GameStatusData.SpecialGold;
                                        Constant.EffCtrlScript.ShowClearSpecialEffGoldSplit(specialGoldItem.GetComponent<BlockItem>().GetPosIndex(), specialGoldEffSplitEdgeToNew);

                                        if (Constant.VibratorSwitch)
                                        {
                                            Tools.DoVibrator(Constant.VibratorTime, Constant.VibratorAmplitude);
                                        }
                                    }

                                    //金色和边缘块特效（消除颜色块）
                                    if (clearSpecialEdgeBlocksGold != null && clearSpecialEdgeBlocksGold.Count > 0)
                                    {
                                        foreach (var specialData in clearSpecialBlocks)
                                        {
                                            if (specialData[(int)Blocks.Key.Special] == (int)Blocks.Special.Gold)
                                            {
                                                ++Constant.GameStatusData.SpecialGold;
                                                Constant.EffCtrlScript.ShowClearSpecialEffGold(
                                                    specialData[(int)Blocks.Key.Pos], clearSpecialEdgeBlocksGold);
                                                break;
                                            }
                                        }

                                        foreach (var posIndex in clearSpecialEdgeBlocks)
                                        {
                                            if (_itemList[posIndex] != null &&
                                                _itemList[posIndex].GetComponent<BlockItem>().GetSpecial() ==
                                                (int)Blocks.Special.Gold)
                                            {
                                                Constant.EffCtrlScript.ShowClearSpecialEffGold(posIndex, clearSpecialEdgeBlocksGold);
                                                break;
                                            }
                                        }

                                        if (Constant.VibratorSwitch)
                                        {
                                            Tools.DoVibrator(Constant.VibratorTime, Constant.VibratorAmplitude);
                                        }
                                    }
                                }, Constant.ClearWaitTime + effWaitTime));
                            }

                            if (Player.IsInGuide() || (Blocks.IsTesting() && Blocks.GetBlocksTest().ShouldShowClearHangEff()))
                            {
                                clearWaitTime += Constant.SecondChanceClearEffTime + 0.4f;

                                StartCoroutine(Delay.Run(() =>
                                {
                                    foreach (var hang in clearHang)
                                    {
                                        Constant.EffCtrlScript.ShowSecondChanceClearHangEff(null, hang);
                                        for (var i = hang * Constant.Lie; i < (hang + 1) * Constant.Lie; ++i)
                                        {
                                            if (_itemList[i] != null)
                                            {
                                                Constant.EffCtrlScript.ShowClearSpecialEdgeEff(_itemList[i]);
                                            }
                                        }
                                        Constant.GameStatusData.RemoveShake++;
                                        if (shouldVibrator)
                                        {
                                        }
                                    }

                                    if (clearHang.Count > 1)
                                    {
                                        Constant.GameStatusData.ComboShake++;
                                    }
                                }, 0.2f));
                            }

                            if (!Player.IsInGuide() && Constant.SceneVersion == "3")
                            {
                                foreach (var hang in clearHang)
                                {
                                    for (var i = hang * Constant.Lie; i < (hang + 1) * Constant.Lie; ++i)
                                    {
                                        if (_itemList[i] != null)
                                        {
                                            Constant.EffCtrlScript.ShowClearSpecialEdgeEff(_itemList[i], 0.02f, 70 / 255f);
                                        }
                                    }
                                    Constant.GameStatusData.RemoveShake++;
                                    if (shouldVibrator)
                                    {
                                    }
                                }
                                if (clearHang.Count > 1)
                                {
                                    Constant.GameStatusData.ComboShake++;
                                }
                            }

                            //青铜边缘块碎裂+生成新方块
                            if (Constant.SceneVersion == "3")
                            {
                                StartCoroutine(Delay.Run(() =>
                                {
                                    var isClearGold = clearSpecialEdgeBlocksGold != null &&
                                                      clearSpecialEdgeBlocksGold.Count > 0;

                                    if (clearSpecialEdgeBlocksBronze != null && clearSpecialEdgeBlocksBronze.Count > 0)
                                    {
                                        foreach (var i in clearSpecialEdgeBlocksBronze)
                                        {
                                            if (_itemList[i] != null)
                                            {
                                                var clearByGold = false;
                                                if (isClearGold)
                                                {
                                                    foreach (var t in clearSpecialEdgeBlocksGold)
                                                    {
                                                        if (t == i)
                                                        {
                                                            clearByGold = true;
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (!clearByGold)
                                                {
                                                    RemoveBlockItemByIndex(i, false);
                                                }
                                            }
                                        }

                                        if (newBlocks != null && newBlocks.Count > 0)
                                        {
                                            foreach (var newData in newBlocks)
                                            {
                                                if (!Blocks.HaveBlock(newData[(int)Blocks.Key.Pos]))
                                                {
                                                    continue;
                                                }

                                                //两个青铜同时碎裂一个长度为3以上的块时，会有两次分裂，所以会生成两次新的块，特殊处理一下
                                                if (_itemList[newData[(int)Blocks.Key.Pos]] != null)
                                                {
                                                    RemoveBlockItemByIndex(newData[(int)Blocks.Key.Pos], false);
                                                }

                                                var newItem = CreateBlockItem(newData);
                                                newItem.SetActive(false);
                                                StartCoroutine(Delay.Run(() =>
                                                {
                                                    newItem.SetActive(true);
                                                }, 0.5f));
                                            }
                                        }
                                    }
                                }, clearWaitTime - 0.3f));
                            }

                            StartCoroutine(Delay.Run(() =>
                            {
                                //行消除
                                var clearScore = 0;
                                var iceBlockNum = 0;
                                var doubleScore = 2;
                                var doubleCombo = 0;
                                foreach (var hang in clearHang)
                                {
                                    if (Constant.LevelUpOtherEffSwitch)
                                    {
                                        Constant.EffCtrlScript.ShowClearToLevelEff(hang);
                                    }

                                    var isClearTypeB = false;
                                    var isClearTypeB1 = false;
                                    var isClearTypeB2 = false;

                                    for (var i = hang * Constant.Lie; i < (hang + 1) * Constant.Lie; ++i)
                                    {
                                        if (_itemList[i] != null)
                                        {
                                            //数据统计
                                            if (_itemList[i].GetComponent<BlockItem>().IsMovedBlockItem)
                                            {
                                                var offsetHang =
                                                    Blocks.GetHangByPos(_itemList[i].GetComponent<BlockItem>()
                                                        .GetPosIndex()) - _itemList[i].GetComponent<BlockItem>()
                                                        .OriHang;
                                                if (offsetHang == -1)
                                                {
                                                    AddClearType("A1");
                                                }
                                                else if (offsetHang < -1)
                                                {
                                                    AddClearType("A2");
                                                }
                                                else if (offsetHang == 0)
                                                {
                                                    isClearTypeB = true;
                                                }

                                                _itemList[i].GetComponent<BlockItem>().IsMovedBlockItem = false;
                                                _itemList[i].GetComponent<BlockItem>().OriHang = 0;
                                                _itemList[i].GetComponent<BlockItem>().WillBeHangRemove = true;
                                            }
                                            else
                                            {
                                                if (_itemList[i].GetComponent<BlockItem>().LastDownOffsetY == -1)
                                                {
                                                    isClearTypeB1 = true;
                                                }
                                                else if (_itemList[i].GetComponent<BlockItem>().LastDownOffsetY < -1)
                                                {
                                                    isClearTypeB2 = true;
                                                }

                                            }

                                            var score = _itemList[i].GetComponent<BlockItem>().GetLength() *
                                                        _unitBlockScore;
                                            if (_itemList[i].GetComponent<BlockItem>().IsSpecial())
                                            {
                                                score *= Blocks.GetSpecialScoreTimesBySpecialType(_itemList[i].GetComponent<BlockItem>().GetSpecial());
                                            }

                                            score *= (int)Math.Pow(doubleScore, doubleCombo);
                                            clearScore += score;

                                            if (_itemList[i].GetComponent<BlockItem>().IsIce())
                                            {
                                                Constant.EffCtrlScript.ShowClearIceEff(_itemList[i]);
                                                Constant.EffCtrlScript.RemoveIceEff(_itemList[i]);
                                                ++iceBlockNum;
                                            }
                                            else
                                            {
                                                Constant.EffCtrlScript.ShowClearBlockEff(_itemList[i]);
                                                RemoveBlockItemByIndex(i);
                                            }
                                        }
                                    }

                                    if (isClearTypeB)
                                    {
                                        if (isClearTypeB1)
                                        {
                                            AddClearType("B1");
                                        }

                                        if (isClearTypeB2)
                                        {
                                            AddClearType("B2");
                                        }
                                    }

                                    ++_clearCombo;
                                    ++doubleCombo;
                                }

                                if (Constant.SceneVersion == "2")
                                {
                                    //青铜边缘块碎裂+生成新方块
                                    if (clearSpecialEdgeBlocksBronze != null && clearSpecialEdgeBlocksBronze.Count > 0)
                                    {
                                        foreach (var i in clearSpecialEdgeBlocksBronze)
                                        {
                                            if (_itemList[i] != null)
                                            {
                                                RemoveBlockItemByIndex(i, false);
                                            }
                                        }

                                        if (newBlocks != null && newBlocks.Count > 0)
                                        {
                                            foreach (var newData in newBlocks)
                                            {
                                                //两个青铜同时碎裂一个长度为3以上的块时，会有两次分裂，所以会生成两次新的块，特殊处理一下
                                                if (_itemList[newData[(int)Blocks.Key.Pos]] != null)
                                                {
                                                    RemoveBlockItemByIndex(newData[(int)Blocks.Key.Pos], false);
                                                }
                                                CreateBlockItem(newData);
                                            }
                                        }
                                    }
                                }

                                //金色分割4或3为1
                                if (specialGoldEffSplitBlocks != null && specialGoldEffSplitBlocks.Count > 0)
                                {
                                    foreach (var i in specialGoldEffSplitBlocks)
                                    {
                                        if (_itemList[i] != null)
                                        {
                                            RemoveBlockItemByIndex(i, false);
                                        }
                                    }

                                    if (specialGoldEffSplitBlocksNew != null)
                                    {
                                        foreach (var newData in specialGoldEffSplitBlocksNew)
                                        {
                                            CreateBlockItem(newData);
                                        }
                                    }
                                }

                                //彩色边缘块消除
                                if (clearSpecialEdgeBlocks.Count > 0)
                                {
                                    foreach (var i in clearSpecialEdgeBlocks)
                                    {
                                        if (_itemList[i] != null)
                                        {
                                            var score = _itemList[i].GetComponent<BlockItem>().GetLength() *
                                                        _unitBlockScore;
                                            if (_itemList[i].GetComponent<BlockItem>().IsSpecial())
                                            {
                                                score *= Blocks.GetSpecialScoreTimesBySpecialType(_itemList[i].GetComponent<BlockItem>().GetSpecial());
                                            }
                                            clearScore += score;

                                            if (_itemList[i].GetComponent<BlockItem>().IsIce())
                                            {
                                                Constant.EffCtrlScript.ShowClearIceEff(_itemList[i]);
                                                Constant.EffCtrlScript.RemoveIceEff(_itemList[i]);
                                                ++iceBlockNum;
                                            }
                                            else
                                            {
                                                Constant.EffCtrlScript.ShowClearBlockEff(_itemList[i]);
                                                RemoveBlockItemByIndex(i);
                                            }
                                        }
                                    }
                                }

                                //金色消除某同色宝石
                                if (clearSpecialEdgeBlocksGold != null && clearSpecialEdgeBlocksGold.Count > 0)
                                {
                                    foreach (var i in clearSpecialEdgeBlocksGold)
                                    {
                                        if (_itemList[i] != null)
                                        {
                                            var score = _itemList[i].GetComponent<BlockItem>().GetLength() *
                                                        _unitBlockScore;
                                            if (_itemList[i].GetComponent<BlockItem>().IsSpecial())
                                            {
                                                score *= Blocks.GetSpecialScoreTimesBySpecialType(_itemList[i].GetComponent<BlockItem>().GetSpecial());
                                            }
                                            clearScore += score;

                                            if (_itemList[i].GetComponent<BlockItem>().IsIce())
                                            {
                                                Constant.EffCtrlScript.ShowClearIceEff(_itemList[i]);
                                                Constant.EffCtrlScript.RemoveIceEff(_itemList[i]);
                                                ++iceBlockNum;
                                            }
                                            else
                                            {
                                                Constant.EffCtrlScript.ShowClearBlockEff(_itemList[i]);
                                                RemoveBlockItemByIndex(i);
                                            }
                                        }
                                    }

                                    Constant.EffCtrlScript.RemoveClearSpecialEffGoldFaDianEff();
                                }

                                ++_clearComboSound;
                                if (_clearComboSound + 1 >= 6)
                                {
                                    ManagerAudio.PlaySound("clear_6");
                                }
                                else
                                {
                                    ManagerAudio.PlaySound("clear_" + (_clearComboSound + 1));
                                }

                                if (clearScore > 0)
                                {
                                    if (_clearCombo > 0)
                                    {
                                        Constant.EffCtrlScript.ShowComboEff(_clearCombo);

                                        var shouldShowAchievementTip = false;
                                        if (_clearCombo == 2 && !Constant.GameStatusData.Combo2Tip && !Player.IsInGuide())
                                        {
                                            Constant.GameStatusData.Combo2Tip = true;
                                            shouldShowAchievementTip = true;
                                        }

                                        if (_clearCombo == 4 && !Constant.GameStatusData.Combo4Tip)
                                        {
                                            Constant.GameStatusData.Combo2Tip = true;
                                            Constant.GameStatusData.Combo4Tip = true;
                                            shouldShowAchievementTip = true;
                                        }

                                        if (_clearCombo == 7 && !Constant.GameStatusData.Combo7Tip)
                                        {
                                            Constant.GameStatusData.Combo2Tip = true;
                                            Constant.GameStatusData.Combo4Tip = true;
                                            Constant.GameStatusData.Combo7Tip = true;
                                            shouldShowAchievementTip = true;
                                        }

                                        if (shouldShowAchievementTip)
                                        {
                                            Constant.AchievementScript.AddAchievementTip("COMBO TIMES", _clearCombo);
                                        }
                                    }

                                    Constant.EffCtrlScript.ShowScoreEff(clearScore, iceBlockNum);
                                }

                                if (iceBlockNum > 0)
                                {
                                    clearScore *= (int)Math.Pow(2, iceBlockNum);
                                    Constant.GameStatusData.RemoveFrozen += iceBlockNum;
                                }
                                Player.SetCurScore(Player.GetCurScore() + clearScore);
                                StartCoroutine(Delay.Run(() =>
                                {
                                    UpdateScoreUi();
                                }, Constant.ScoreUpdateDelayTime));


                                if (_movedBlockItem != null)
                                {
                                    if (_clearCombo == 0)
                                    {
                                        if (!_movedBlockItem.GetComponent<BlockItem>().WillBeHangRemove)
                                        {
                                            AddClearType("C", 1);
                                        }
                                    }
                                    else if (_clearCombo > 0)
                                    {
                                        if (_movedBlockItem.GetComponent<BlockItem>().WillBeHangRemove)
                                        {
                                            AddClearType("C", _clearCombo);
                                        }
                                        else
                                        {
                                            AddClearType("C", _clearCombo + 1);
                                        }
                                    }
                                }

                                var nextMoveEndWaitTime = Constant.DownEndWaitTime;
                                if (clearSpecialEdgeBlocks.Count > 0 || (clearSpecialEdgeBlocksBronze != null && clearSpecialEdgeBlocksBronze.Count > 0) || isSpecialGold)
                                {
                                    nextMoveEndWaitTime += Constant.SpecialEdgeClearTime;
                                }
                                StartCoroutine(Delay.Run(() =>
                                {
                                    MoveEnd();
                                }, nextMoveEndWaitTime));
                            }, clearWaitTime));
                        }
                        else
                        {
                            StartCoroutine(Delay.Run(() => { MoveEnd(); }, 0.1f));
                        }
                    }, Constant.DownAnimTime + 0.01f));
            }
            else
            {
                if (Blocks.GetHangNum() >= Constant.Hang)
                {
                    Blocks.SaveBlocksData();
                    if (Player.GetCurScore() >= Constant.SecondChanceScore)
                    {
                        Constant.GameStatusData.continueShould = true;
//                        if (!Constant.GameStatusData.b421_Clk && Constant.SecondChanceEnabled && ManagerAd.HaveRewardAd())
                        if (Constant.SecondChanceEnabled && AdManager.Instance.IsRewardAdLoaded())
                        {
                            Constant.GameStatusData.videoReady = true;
                            ShowSecondChanceDialog();
                        }
                        else
                        {
                            ShowGameOverEff();
                        }
                    }
                    else
                    {
                        ShowGameOverEff();
                    }
                    Player.SaveGameStatusData();
                }
                else
                {
                    if (_specialGoldEffNoNewBlocksTime > 0f)
                    {
                        _userMove = false;
                    }

                    if (Constant.GameStatusData.DWAD_REMAINING_STEP > 0)
                    {
                        if (Blocks.GetHangNum() > 0)
                        {
                            --Constant.GameStatusData.DWAD_REMAINING_STEP;
                            _userMove = false;
                            if (Constant.GameStatusData.DWAD_REMAINING_STEP == 0)
                            {
                                Constant.EffCtrlScript.RemoveReadyBgIceEff(readyGroup);
                            }
                        }
                    }

                    if (Blocks.IsTesting())
                    {
                        if (!Blocks.GetBlocksTest().ShouldAddBlockItems())
                        {
                            _userMove = false;
                        }
                    }
                    if (_userMove)
                    {
                        _userMove = false;
                        AddBlockItems();
                        StartCoroutine(Delay.Run(() => { MoveEnd(); }, Constant.UpAnimTime + Constant.UpEndWaitTime));
                    }
                    else
                    {
                        //如果在倒计时时屏幕全清，则倒计时直接结束
                        if (_specialGoldEffNoNewBlocksTime > 0 && Blocks.GetHangNum() <= 1)
                        {
                            _specialGoldEffNoNewBlocksTime = 0;
                        }

                        if (_specialGoldEffNoNewBlocksTime <= 0 && Constant.GameStatusData.DWAD_REMAINING_STEP <= 0 && Blocks.GetHangNum() < Blocks.GetMaxHangNumByScore(Player.GetCurScore()))
                        {
                            if (Blocks.IsTesting() && !Blocks.GetBlocksTest().ShouldAddBlockItems())
                            {
                                return;
                            }

                            AddBlockItems();
                            var canAutoClear = !(Blocks.IsTesting() && !Blocks.GetBlocksTest().EnableAutoClear());
                            StartCoroutine(Delay.Run(() => { MoveEnd(null, "", false, canAutoClear); }, Constant.UpAnimTime + Constant.UpEndWaitTime));
                        }
                        else
                        {
                            if (_clearCombo > 0)
                            {
                                Constant.GameStatusData.ComboNumber += _clearCombo;
                                if (_clearCombo > Constant.GameStatusData.HighComboNumber)
                                {
                                    Constant.GameStatusData.HighComboNumber = _clearCombo;
                                }

                                if (_clearCombo >= 3)
                                {
                                    Constant.EffCtrlScript.ShowManEffByName("xingfen");
                                }
                            }

                            if (_clearCombo >= 0)
                            {
                                ++_stepScore;
                                if (_stepScore > Constant.GameStatusData.StepScore)
                                {
                                    Constant.GameStatusData.StepScore = _stepScore;

                                    if (_stepScore == 5 || _stepScore == 8 || _stepScore == 10)
                                    {
                                        Constant.AchievementScript.AddAchievementTip("KEEP SCORING", _stepScore);
                                    }
                                }
                            }
                            else
                            {
                                _stepScore = 0;
                            }

                            var userCanMoveTime = 0.0f;

                            if (Blocks.UpdateLevelByScore(Player.GetCurScore()) > _userMoveLevel)
                            {
                                _userMoveLevel = Blocks.CurLevel;

                                StartCoroutine(Delay.Run(() =>
                                {
                                    Constant.EffCtrlScript.ShowLevelUpEff(Blocks.CurLevel);
                                }, userCanMoveTime + 0.1f));
                            }

                            //生成冰块
                            var iceBlocks = Blocks.GetIceBlocks();
                            if (iceBlocks != null && iceBlocks.Count > 0)
                            {
                                GameObject iceTipBlockItem = null;

                                userCanMoveTime += Constant.IceTime + 0.01f;
                                Constant.EffCtrlScript.ShowWindEff();
                                ManagerAudio.PlaySound("frozen");
                                foreach (var posIndex in iceBlocks)
                                {
                                    if (_itemList[posIndex] != null)
                                    {
                                        Constant.EffCtrlScript.AddIceEff(_itemList[posIndex]);
                                        iceTipBlockItem = _itemList[posIndex];
                                    }
                                }

                                if (Player.ShouldShowIceTip())
                                {
                                    userCanMoveTime += 0.2f;
                                    StartCoroutine(Delay.Run(() =>
                                    {
                                        Player.SetAlreadyShowIceTip();
                                        Constant.EffCtrlScript.ShowIceTipEff(iceTipBlockItem);
                                    }, 0.2f));
                                }
                            }
                            else
                            {
                                //生成石块
                                var stoneBlocks = Blocks.GetStoneBlocks();
                                if (stoneBlocks != null && stoneBlocks.Count > 0)
                                {
                                    userCanMoveTime += Constant.IceTime + 0.01f;
                                    ManagerAudio.PlaySound("toStone");
                                    foreach (var stonePosIndex in stoneBlocks)
                                    {
                                        Constant.EffCtrlScript.AddSpecialEffStone(_itemList[stonePosIndex]);
                                    }

                                    if (Player.ShouldShowStoneTip())
                                    {
                                        userCanMoveTime += 0.2f;
                                        StartCoroutine(Delay.Run(() =>
                                        {
                                            var stoneItem = _itemList[stoneBlocks[0]];
                                            if (stoneItem != null)
                                            {
                                                Player.SetAlreadyStoneTip();
                                                Constant.EffCtrlScript.ShowStoneTipEff(stoneItem);
                                            }
                                        }, 0.2f));
                                    }
                                }
                            }

                            StartCoroutine(Delay.Run(() =>
                            {
                                if (Player.IsInGuide())
                                {
                                    Constant.EffCtrlScript.ShowGuideStepEff();
                                    Player.GuideEnd();

                                    //引导结束
                                    if (!Player.IsInGuide())
                                    {
                                        Constant.EffCtrlScript.RemoveGuideStepEff(true);
                                        if (!Constant.AFData.guide_end)
                                        {
                                            Constant.AFData.guide_end = true;
                                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                                        }

                                        ShowBanner();

                                        CheckShowTopUIMask();
                                    }
                                }
                                _clearCombo = -1;
                                _clearComboSound = -1;
                                Player.UserCanMove = true;
                                Player.SaveGameStatusData();
                                Blocks.SaveBlocksData();
                                ManagerLocalData.SaveData();
                                Blocks.DrawMap();

                                if (_downBlockItemNull)
                                {
                                    _downBlockItemNull = false;
                                    RemoveAllBlockItems();
                                    CreateBlocks(Blocks.GetBlocksData());
                                    MoveEnd();
                                }

                                if (_specialGoldEffNoNewBlocksTime == Constant.SpecialGoldEffNoNewBlocksTime)
                                {
                                    UpdateSpecialGoldNoNewBlocksTime();
                                }

                                SpecialGoldEffCountDownEnd();

                                Constant.EffCtrlScript.RemoveClearSpecialEffGoldFaDianEff();
                            }, userCanMoveTime));
                        }

                        CheckDeadWarning();
                    }
                }
            }
        }

        void AddBlockItems()
        {
            Blocks.UpBlocks();
            UpBlockItemsAnim();

            Blocks.AddBlocks(_readyBlocksData[0]);
            for (var i = 0; i < _readyBlocksData[0].Count; ++i)
            {
                var blockItem = CreateBlockItem(_readyBlocksData[0][i]);
                var localPosition = blockItem.transform.localPosition;
                localPosition = new Vector2(localPosition.x, Constant.BlockGroupEdgeBottom - Constant.BlockHeight);
                blockItem.transform.localPosition = localPosition;
                blockItem.transform.DOLocalMoveY(localPosition.y + Constant.BlockHeight, Constant.UpAnimTime).SetEase(_easeEff);
                blockItem.GetComponent<BlockItem>().LastDownOffsetY = 0;
            }
            _readyBlocksData.RemoveAt(0);
            AddReadyBlockItems();
        }

        void AddReadyBlockItems()
        {
            if (!Player.IsInGuide() && _spComboNum <= 0)
            {
                _spComboNum = Blocks.CheckSpecialProtect();
                if (_spComboNum > 0)
                {
                    if (Blocks.GetHangNum() <= Blocks.GetMaxHangNumByScore(Player.GetCurScore()) - 2)
                    {
                        _spComboNum = Tools.GetNumFromRange(0, 100) >= 50 ? 1 : 0;
                    }
                    DebugEx.Log("新手保护连消行数", _spComboNum);
                }
            }

            if (_spComboNum <= 0)
            {
                _readyBlocksData.Add(Blocks.GetNextBlocksData());
            }
            else
            {
                _readyBlocksData.Insert(0, Blocks.GetNextBlocksDataCanClear());
                --_spComboNum;
            }

            readyGroup.GetComponent<ReadyGroup>().UpdateReadyGroup(_readyBlocksData[0]);
        }

        GameObject CreateBlockItem(int[] data, bool specialGoldSuccess = false)
        {
            var item = _blockItemsPool.Get();
            item.transform.DOKill();
            item.transform.SetParent(blockGroup.transform);
            item.transform.localScale = Vector2.one;
            item.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + Constant.BlockWidth * Blocks.GetLieByPos(data[(int)Blocks.Key.Pos]), Constant.BlockGroupEdgeBottom + Constant.BlockHeight * Blocks.GetHangByPos(data[(int)Blocks.Key.Pos]));
            item.transform.localRotation = Quaternion.Euler(0, 0, 0);
            item.GetComponent<BlockItem>().UpdateUi(data, blockBgLightEff, blockBgTip, blockLightTip);
            item.GetComponent<CanvasGroup>().alpha = 1;

            if (item.GetComponent<BlockItem>().IsSpecial())
            {
                Constant.EffCtrlScript.AddSpecialEff(item, item.GetComponent<BlockItem>().GetSpecial(), specialGoldSuccess);
            }

            if (item.GetComponent<BlockItem>().HaveIce())
            {
                Constant.EffCtrlScript.AddIceEff(item, false);
            }

            _itemList[data[(int)Blocks.Key.Pos]] = item;
            return item;
        }

        void RemoveBlockItemByIndex(int index, bool showAnim = true)
        {
            if (_itemList[index] != null)
            {
                var item = _itemList[index];
                if (showAnim)
                {
                    item.GetComponent<CanvasGroup>().DOFade(0, Constant.BlockRemoveTime).OnComplete(() =>
                    {
                        RemoveBlockItemOtherEff(item);
                        _blockItemsPool.Put(item);
                    });
                }
                else
                {
                    RemoveBlockItemOtherEff(item);
                    _blockItemsPool.Put(item);
                }

                _itemList[index] = null;
            }
        }

        void RemoveBlockItemOtherEff(GameObject item)
        {
            if (item.GetComponent<BlockItem>().IsSpecial())
            {
                Constant.EffCtrlScript.RemoveSpecialEff(item);
            }

            if (item.transform.Find("IceEff(Clone)") != null)
            {
                Constant.EffCtrlScript.RemoveIceEff(item);
            }

            if (item.transform.Find("B421(Clone)") != null)
            {
                Constant.EffCtrlScript.RemoveB421Eff();
            }
        }

        void UpBlockItemsAnim()
        {
            for (var i = _itemList.Length - 1; i >= 0; --i)
            {
                if (_itemList[i] != null)
                {
                    //                    if (i > _itemList.Length - 1 - Constant.Lie)
                    //                    {
                    //                        RemoveBlockItemByIndex(i);
                    //                    }
                    //                    else
                    //                    {
                    _itemList[i + Constant.Lie] = _itemList[i];
                    _itemList[i] = null;
                    _itemList[i + Constant.Lie].transform.DOLocalMoveY(_itemList[i + Constant.Lie].transform.localPosition.y + Constant.BlockHeight, Constant.UpAnimTime).SetEase(_easeEff);

                    _itemList[i + Constant.Lie].GetComponent<BlockItem>().LastDownOffsetY = 0;
                    //                    }
                }
            }

            ManagerAudio.PlaySound("blockUp");
        }

        void DownBlockItemsAnim(List<int[]> downList)
        {
            foreach (var downData in downList)
            {
                if (_itemList[downData[1]] != null)
                {
                    //downData[0]下落高度，downData[1]下落方块
                    _itemList[downData[1] + Constant.Lie * downData[0]] = _itemList[downData[1]];
                    _itemList[downData[1]] = null;
                    _itemList[downData[1] + Constant.Lie * downData[0]].transform
                        .DOLocalMoveY(_itemList[downData[1] + Constant.Lie * downData[0]].transform.localPosition.y + Constant.BlockHeight * downData[0],
                            Constant.DownAnimTime).SetEase(_easeEff);

                    _itemList[downData[1] + Constant.Lie * downData[0]].GetComponent<BlockItem>().LastDownOffsetY =
                        downData[0];
                }
                else
                {
                    _downBlockItemNull = true;
                }
            }
        }

        void UpdateScoreUi(bool showEff = true)
        {
            scoreGroup.GetComponent<ScoreGroup>().UpdateScoreUi(showEff);
            _unitBlockScore = Blocks.GetUnitBlockScoreByCurScore(Player.GetCurScore());
        }

        public int GetUnitBlockScore()
        {
            return _unitBlockScore;
        }

        public void B421Result(Hashtable result = null)
        {
            if (result != null && (bool)result["success"])
            {
                ResetClearTipTime();

                Constant.GameStatusData.b421_Use = true;
                Constant.GameStatusData.b421_SlideNumber = Constant.GameStatusData.SlideNumber;

                Constant.EffCtrlScript.AddReadyBgIceEff(readyGroup);
                Constant.GameStatusData.DWAD_REMAINING_STEP = Constant.B421NotAddBlockStep + 1;

                var posIndex = Blocks.GetB421Block();
                if (posIndex >= 0 && _b421BlockItem != null)
                {
                    Constant.EffCtrlScript.AddBlackMaskEff();

                    _b421BlockItem.transform.SetParent(Constant.EffCtrlScript.effGroup.transform);
                    Constant.EffCtrlScript.AddHighLightEff(_b421BlockItem);

                    var seq = DOTween.Sequence();
                    seq.Append(_b421BlockItem.transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0.1f).SetDelay(0.1f));
                    seq.Append(_b421BlockItem.transform.DOScale(new Vector3(1, 1, 1), 0.15f).SetDelay(0.8f).OnComplete(
                        () =>
                        {
                            Constant.EffCtrlScript.RemoveHighLightEff(_b421BlockItem);
                            Constant.EffCtrlScript.ShowShakeEff();

                            Constant.EffCtrlScript.RemoveBlackMaskEff();
                            Constant.EffCtrlScript.ShowClearBlockEff(_b421BlockItem);

                            var removeBlockItemIndex = _b421BlockItem.GetComponent<BlockItem>().GetPosIndex();
                            var newBlocks = Blocks.ChangeB421(removeBlockItemIndex);

                            RemoveBlockItemByIndex(removeBlockItemIndex);

                            foreach (var blockData in newBlocks)
                            {
                                var item = CreateBlockItem(blockData);
                                _itemList[blockData[(int)Blocks.Key.Pos]] = item;
                                _b421BlockItem = null;
                            }

                            StartCoroutine(Delay.Run(() =>
                            {
                                MoveEnd();
                            }, 0.1f));
                        }));
                    seq.SetUpdate(true);
                }
            }
        }

        public void B43221Result(Hashtable result = null)
        {
            if (result != null && (bool)result["success"])
            {
                ResetClearTipTime();

                Constant.GameStatusData.DWAD_USE = true;
                Constant.GameStatusData.DWAD_USE_SLIDENUMBER = Constant.GameStatusData.SlideNumber;

                Constant.EffCtrlScript.AddReadyBgIceEff(readyGroup);
                Constant.GameStatusData.DWAD_REMAINING_STEP = Constant.B43221NotAddBlockStep + 1;

                Constant.EffCtrlScript.AddBlackMaskEff();

                Blocks.ChangeB43221(out var delBlocks, out var newBlocks);

                var lastPosIndex = -1;
                var curCount = 0;
                for (var hang = Constant.Hang - 2; hang >= 0; --hang)
                {
                    for (var posIndex = hang * Constant.Lie; posIndex < (hang + 1) * Constant.Lie; ++posIndex)
                    {
                        if (delBlocks[posIndex] && _itemList[posIndex] != null)
                        {
                            ++curCount;
                            lastPosIndex = posIndex;

                            var item = _itemList[posIndex];

                            item.transform.SetParent(Constant.EffCtrlScript.effGroup.transform);
                            Constant.EffCtrlScript.AddHighLightEff(item);

                            var localPosition = item.transform.localPosition;
                            var oriLocalPos = new Vector2(localPosition.x, localPosition.y);

                            var delayTime1 = 0.1f + curCount * 0.05f;
                            var delayTime2 = 0.7f - curCount * 0.06f;

                            item.transform.DOLocalMove(oriLocalPos * 1.3f, 0.1f).SetDelay(delayTime1).OnComplete(() =>
                            {
                                item.transform.DOLocalMove(oriLocalPos, 0.15f).SetDelay(delayTime2);
                            });

                            var tmpCurCount = curCount;
                            var seq = DOTween.Sequence();
                            seq.Append(item.transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0.1f).SetDelay(delayTime1));
                            seq.Append(item.transform.DOScale(new Vector3(1, 1, 1), 0.15f).SetDelay(delayTime2).OnComplete(
                                () =>
                                {
                                    Constant.EffCtrlScript.RemoveHighLightEff(item);
                                    if (tmpCurCount % 3 == 1)
                                    {
                                        Constant.EffCtrlScript.ShowShakeEff();
                                    }

                                    Constant.EffCtrlScript.RemoveBlackMaskEff();
                                    Constant.EffCtrlScript.ShowClearBlockEff(item);

                                    var curPosIndex = item.GetComponent<BlockItem>().GetPosIndex();
                                    var curLength = item.GetComponent<BlockItem>().GetLength();
                                    RemoveBlockItemByIndex(curPosIndex);
                                    for (var i = curPosIndex; i < curPosIndex + curLength; ++i)
                                    {
                                        if (newBlocks[i] != null)
                                        {
                                            CreateBlockItem(newBlocks[i]);
                                        }
                                    }

                                    if (curPosIndex == lastPosIndex)
                                    {
                                        StartCoroutine(Delay.Run(() => { MoveEnd(); }, 0.5f));
                                    }
                                }));
                            seq.SetUpdate(true);
                        }
                    }
                }
            }
        }

        public void B43221And1HangResult(Hashtable result = null)
        {
            if (result != null && (bool)result["success"])
            {
                ResetClearTipTime();

                Player.UseSecondChance();
                Constant.GameStatusData.videoReward = "success";

                Constant.SecondChanceHangNum = 1;
                Constant.SecondChanceHangStart = 9;

                Constant.EffCtrlScript.ShowSecondChanceClearHangEff(() =>
                {
                    for (var hang = Constant.SecondChanceHangStart;
                        hang < Constant.SecondChanceHangStart + Constant.SecondChanceHangNum;
                        ++hang)
                    {
                        for (var posIndex = hang * Constant.Lie; posIndex < (hang + 1) * Constant.Lie; ++posIndex)
                        {
                            if (_itemList[posIndex] != null)
                            {
                                Constant.EffCtrlScript.ShowClearBlockEff(_itemList[posIndex], false);
                                Blocks.ClearBlockDataByIndex(posIndex);
                                RemoveBlockItemByIndex(posIndex);
                            }
                        }
                    }

                    ManagerAudio.PlaySound("clear_1");
                    Blocks.UpdateMap();
                    StartCoroutine(Delay.Run(() => { B43221Result(result); }, 0.05f));
                });

                if (!Constant.AFData.video_reward)
                {
                    Constant.AFData.video_reward = true;
                    ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                }
            }
        }

        public void SecondChanceResult(Hashtable result = null)
        {
            if (result != null && (bool)result["success"])
            {
                ResetClearTipTime();

                Player.UseSecondChance();
                Constant.GameStatusData.videoReward = "success";

                Constant.EffCtrlScript.ShowSecondChanceClearHangEff(() =>
                {
                    for (var hang = Constant.SecondChanceHangStart;
                        hang < Constant.SecondChanceHangStart + Constant.SecondChanceHangNum;
                        ++hang)
                    {
                        for (var posIndex = hang * Constant.Lie; posIndex < (hang + 1) * Constant.Lie; ++posIndex)
                        {
                            if (_itemList[posIndex] != null)
                            {
                                Constant.EffCtrlScript.ShowClearBlockEff(_itemList[posIndex], false);
                                Blocks.ClearBlockDataByIndex(posIndex);
                                RemoveBlockItemByIndex(posIndex);
                            }
                        }
                    }

                    ManagerAudio.PlaySound("clear_1");
                    Blocks.UpdateMap();
                    StartCoroutine(Delay.Run(() => { MoveEnd(); }, 0.05f));
                });

                if (!Constant.AFData.video_reward)
                {
                    Constant.AFData.video_reward = true;
                    ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                }
            }
            else
            {
                Constant.GameStatusData.videoReward = "fail";
                ShowGameOverEff();
            }

            Player.SaveGameStatusData();
        }

        public void ShowGameOverEff()
        {
            for (var hang = Constant.Hang - 1; hang >= 0; --hang)
            {
                var hang1 = hang;
                StartCoroutine(Delay.Run(() =>
                    {
                        for (var posIndex = hang1 * Constant.Lie; posIndex < (hang1 + 1) * Constant.Lie; ++posIndex)
                        {
                            if (_itemList[posIndex] != null)
                            {
                                Constant.EffCtrlScript.ShowBlockItemGrayEff(_itemList[posIndex]);
                            }
                        }

                        if (hang1 == 0)
                        {
                            StartCoroutine(Delay.Run(() => { ShowGameOverDialog(); }, 0.3f));
                        }
                    }, 0.15f * (Constant.Hang - hang)));
            }

            ManagerAudio.PauseBgm();
            ManagerAudio.PlaySound("gameOver");

            Constant.EffCtrlScript.ShowManEffByName("jusang");
        }

        public void ShowGameOverDialog(bool showAd = true)
        {
//            if (!showAd)
//            {
//                ReallyShowGameOverDialog();
//                return;
//            }


            ReallyShowGameOverDialog();
            Player.SaveGameStatusData();

            if (AdManager.Instance.IsInterstitialAdLoad())
            {
                AdManager.Instance.ShowInterstitialAd();
            }
            
           
        }

        public void ClearBlocksData()
        {
            Player.SetBestScore(Player.GetCurScore());
            ManagerLocalData.ClearData(ManagerLocalData.BLOCKS_DATA);
        }

        void CheckDeadWarning()
        {
            if (Blocks.IsDangerous())
            {
                if (!Constant.EffCtrlScript.IsShowingDeadWarning())
                {
                    Constant.EffCtrlScript.ShowDeadWarning();

                    if (Constant.B421Switch)
                    {
                        if (!Constant.GameStatusData.b421_Clk && !Constant.GameStatusData.continueClk && !Constant.EffCtrlScript.IsB421EffShowing() && ManagerAd.HaveRewardAd())
                        {
                            var posIndex = Blocks.GetB421Block();
                            if (posIndex > -1 && _itemList[posIndex] != null)
                            {
                                _b421BlockItem = _itemList[posIndex];
                                Constant.EffCtrlScript.AddB421Eff(_itemList[posIndex]);

                                ++Constant.GameStatusData.b421_Count;
                            }
                        }
                    }

                    if (Constant.B43221Switch)
                    {
                        if (!Constant.GameStatusData.DWAD_CLICK && ManagerAd.HaveRewardAd() &&
                            ManagerDialog.IsExistDialog("B43221Dialog") == null)
                        {
                            ManagerDialog.CreateDialog("B43221Dialog");
                        }
                    }
                }
            }
            else
            {
                if (Constant.EffCtrlScript.IsShowingDeadWarning())
                {
                    ++Constant.GameStatusData.DieToLifeCount;
                }

                Constant.EffCtrlScript.RemoveDeadWarning();
                Constant.EffCtrlScript.RemoveB421Eff();
            }
        }

        public void ResetClearTipTime()
        {
            Constant.EffCtrlScript.HideClearTip();
            _clearTipTime = 0;

            if (_specialGoldItem != null)
            {
                var item = _specialGoldItem;
                if (item != null && item.GetComponent<BlockItem>().GetSpecial() == (int)Blocks.Special.Gold)
                {
                    Constant.EffCtrlScript.ShowSpecialGoldOtherEff(item, "daiji");
                }
                _specialGoldItem = null;
            }
        }

        void CheckClearTip()
        {
            if (Blocks.IsTesting()) return;

            if (!Player.IsInGuide() && _clearTipTime >= Constant.ClearTipTimeMax && !Constant.EffCtrlScript.IsShowingClearTip() && Player.UserCanMove && !Player.IsBlockMoving)
            {
                var clearTipData = Blocks.GetClearTip();
                if (clearTipData != null)
                {
                    DebugEx.Log("操作提示：", clearTipData[2], "移动", clearTipData[0]);
                    Constant.EffCtrlScript.ShowClearTip(_itemList[clearTipData[2]], clearTipData);

                    ++Constant.GameStatusData.GuideTime;

                    var goldPosIndex = Blocks.CheckGoldWillClear(clearTipData);
                    if (goldPosIndex != -1)
                    {
                        var item = _itemList[goldPosIndex];
                        if (item != null && item.GetComponent<BlockItem>().GetSpecial() == (int)Blocks.Special.Gold)
                        {
                            Constant.EffCtrlScript.ShowSpecialGoldOtherEff(item, "tishi");
                            _specialGoldItem = item;
                        }
                    }

                    Constant.EffCtrlScript.ShowManEffByName("kun");
                }
                else
                {
                    DebugEx.Log("未找到操作提示！");
                    ResetClearTipTime();
                }
            }
        }

        private void AddClearType(string clearType, int combo = 0)
        {
            switch (clearType)
            {
                case "A1":
                    ++Constant.GameStatusData.clearTypeA;
                    break;
                case "A2":
                    ++Constant.GameStatusData.clearTypeA2;
                    break;
                case "B1":
                    ++Constant.GameStatusData.clearTypeB;
                    break;
                case "B2":
                    ++Constant.GameStatusData.clearTypeB2;
                    break;
                case "C":
                    Constant.GameStatusData.clearTypeC += combo;

                    var num = Constant.GameStatusData.clearTypeC;
                    if (num == 5 || num == 10 || num == 20)
                    {
                        Constant.AchievementScript.AddAchievementTip("SKILLFUL MOVES", num);
                    }
                    break;
            }
        }

        public void ClearPropBlocks(List<int> clearBlocks, List<int[]> newBlocks)
        {
            if (clearBlocks == null)
            {
                Player.UserCanMove = true;
                return;
            }

            foreach (var posIndex in clearBlocks)
            {
                if (_itemList[posIndex] != null)
                {
                    if (_itemList[posIndex].GetComponent<BlockItem>().IsIce())
                    {
                        Constant.EffCtrlScript.ShowClearIceEff(_itemList[posIndex]);
                        Constant.EffCtrlScript.RemoveIceEff(_itemList[posIndex]);
                    }
                    else
                    {
                        Constant.EffCtrlScript.ShowClearBlockEff(_itemList[posIndex]);
                        RemoveBlockItemByIndex(posIndex);
                    }
                }
            }

            if (newBlocks != null && newBlocks.Count > 0)
            {
                foreach (var data in newBlocks)
                {
                    CreateBlockItem(data);
                }
            }

            StartCoroutine(Delay.Run(() =>
            {
                MoveEnd();
            }, 1));
        }

        public void UpdatePropInfo()
        {

        }

        public GameObject GetBlockItemByPosIndex(int posIndex)
        {
            return _itemList[posIndex] != null ? _itemList[posIndex] : null;
        }

        public GameObject[] GetBlockItems()
        {
            return _itemList;
        }

        public bool ReadyGroupHaveBlockSpecialType(int blockSpecialType, int count = 1)
        {
            var curCount = 0;
            if (_readyBlocksData != null && _readyBlocksData.Count > 0)
            {
                foreach (var readyData in _readyBlocksData)
                {
                    foreach (var blockData in readyData)
                    {
                        if (blockData[(int)Blocks.Key.Special] == blockSpecialType)
                        {
                            ++curCount;
                            if (curCount >= count)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool IsSpecialGoldEffNoNewBlocks()
        {
            return _specialGoldEffNoNewBlocksTime > 0;
        }

        private void UpdateSpecialGoldNoNewBlocksTime()
        {
            Constant.EffCtrlScript.ShowReadyCountDownEff(_specialGoldEffNoNewBlocksTime);
            StartCoroutine(Delay.Run(() =>
            {
                --_specialGoldEffNoNewBlocksTime;
                DebugEx.Log("剩余时间", _specialGoldEffNoNewBlocksTime);
                if (_specialGoldEffNoNewBlocksTime > 0)
                {
                    UpdateSpecialGoldNoNewBlocksTime();
                }
                else
                {
                    _specialGoldEffNoNewBlocksTime = 0;
                    Constant.EffCtrlScript.RemoveReadyIceEff();
                }
            }, 1f));
        }

        public void SpecialGoldEffCountDownEnd(bool isCountDownEnd = false)
        {
            if (Player.UserCanMove && Constant.SpecialGoldCountDownSwitch)
            {
                GameObject goldItem = null;
                for (var i = 0; i < _itemList.Length; ++i)
                {
                    if (_itemList[i] != null && _itemList[i].GetComponent<BlockItem>().IsSpecial() &&
                        _itemList[i].GetComponent<BlockItem>().GetSpecial() == (int)Blocks.Special.Gold)
                    {
                        goldItem = _itemList[i];
                        break;
                    }
                }

                if (isCountDownEnd)
                {
                    //nothing
                }
                else if (goldItem == null || Constant.EffCtrlScript.IsSpecialEffGoldCountDowning(goldItem))
                {
                    return;
                }

                if (goldItem == null)
                {
                    return;
                }

                RemoveBlockItemOtherEff(goldItem);
                var goldItemPosIndex = goldItem.GetComponent<BlockItem>().GetPosIndex();

                Blocks.ChangeGoldToCommon();
                RemoveAllBlockItems();
                CreateBlocks(Blocks.GetBlocksData());

                Constant.EffCtrlScript.ShowClearSpecialEdgeEff(_itemList[goldItemPosIndex]);

                Blocks.SaveBlocksData();
                ManagerLocalData.SaveData();
            }
        }

        private async void ShowSecondChanceDialog()
        {
            if (!ManagerDialog.IsExistDialogRes("SecondChanceDialog"))
            {
                var dialogAddressableName = "Prefabs_Scene2/SecondChanceDialog";
                if (Constant.SceneVersion == "3")
                {
                    dialogAddressableName = "Prefabs_Scene3/SecondChanceDialog";
                }

                var tmpPrefab = MaterPool.Instace.SecondChanceDialog;
                ManagerDialog.AddDialog("SecondChanceDialog", tmpPrefab);
            }
            ManagerDialog.CreateDialog("SecondChanceDialog");
        }

        private async void ReallyShowGameOverDialog()
        {
            if (!ManagerDialog.IsExistDialogRes("GameOverDialog"))
            {
                var dialogAddressableName = "Prefabs_Scene2/GameOverDialogA";
                if (Constant.SceneVersion == "3")
                {
                    dialogAddressableName = "Prefabs_Scene3/GameOverDialog";
                }

                var tmpPrefab = MaterPool.Instace.GameOverDialog;
                ManagerDialog.AddDialog("GameOverDialog", tmpPrefab);
            }
            ManagerDialog.CreateDialog("GameOverDialog");
        }

        //no
        private async void ShowRateDialog()
        {
//            if (!ManagerDialog.IsExistDialogRes("RateDialog"))
//            {
//                var dialogAddressableName = "Prefabs_Scene2/RateDialog";
//                var tmpPrefab = await Tools.LoadAssetAsync<GameObject>(dialogAddressableName);
//                ManagerDialog.AddDialog("RateDialog", tmpPrefab);
//            }
//            ManagerDialog.CreateDialog("RateDialog");
        }
//no
        private async void ShowSpecialGoldDialog(Vector2 pos)
        {
//            if (!ManagerDialog.IsExistDialogRes("SpecialGoldDialog"))
//            {
//                var dialogAddressableName = "Prefabs_Scene2/SpecialGoldDialog";
//                if (Constant.SceneVersion == "3")
//                {
//                    dialogAddressableName = "Prefabs_Scene3/SpecialGoldDialog";
//                }
//
//                var tmpPrefab = await Tools.LoadAssetAsync<GameObject>(dialogAddressableName);
//                ManagerDialog.AddDialog("SpecialGoldDialog", tmpPrefab);
//            }
//            var dialog = ManagerDialog.CreateDialog("SpecialGoldDialog");
//            dialog.GetComponent<SpecialGoldDialog>().StartShow(pos);
        }

        public async void ShowSettingsDialog()
        {
            if (!ManagerDialog.IsExistDialogRes("SettingsDialog"))
            {
                var dialogAddressableName = "Prefabs_Scene3/SettingsDialog";
                var tmpPrefab = MaterPool.Instace.SettingsDialog;
                ManagerDialog.AddDialog("SettingsDialog", tmpPrefab);
            }
            ManagerDialog.CreateDialog("SettingsDialog");
        }

        void Update()
        {
            if (Constant.GameStatusData != null)
            {
                Constant.GameStatusData.InningTime += Constant.FrameTime;
            }
            _clearTipTime += Constant.FrameTime;
            CheckClearTip();

        }
    }
}
