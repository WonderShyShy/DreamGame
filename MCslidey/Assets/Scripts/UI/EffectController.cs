using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Manager;
using Models;
using Other;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EffectController : MonoBehaviour
    {
        public GameObject effGroup;

        public GameObject specialEffBronze;
        public Sprite[] specialEffBronzeSpriteFrames;
        public GameObject specialEffGold;
        public Sprite[] specialEffGoldSpriteFrames;
        public GameObject specialEffStone;
        public Sprite[] specialEffStoneSpriteFrames;
        public GameObject[] toStoneEff;

        public GameObject clearSpecialEffBronze;
        public GameObject clearSpecialEffGold;
        public GameObject clearSpecialEffGold2;
        public GameObject clearSpecialEffShanBao;
        
        public GameObject specialEff;
        public Sprite[] specialEffFrame;
        public GameObject clearSpecialEff;
        public GameObject clearSpecialEdgeEff;
        public GameObject deadWarningEff;
        public GameObject blockItemGrayEff;
        public GameObject clearHangLightEff;
        public GameObject comboEff;
        public Sprite[] comboEffFrame;
        public Sprite[] comboEffFrame1;
        public Sprite[] comboEffFrame3;
        public GameObject scoreEff;
        public Sprite[] scoreEffSpriteFrames;
        public GameObject scoreIceEff;
        public GameObject guideEff;

        public GameObject clearTipEff;
        public GameObject clearTipEff2;
        public Sprite[] clearTipEffFrame;

        public GameObject iceEff; 
        public Sprite[] iceEffFrame;

        public GameObject levelUpEffPrefab;
        public GameObject levelUpEff1Prefab;
        public Sprite[] levelUpEffNumFrames;

        public GameObject clearBlockEff;
        public Texture2D[] clearBlockEffSpriteFramesBig;
        public Texture2D[] clearBlockEffSpriteFramesLittle;
        public Texture2D clearBlockEffIceBig;
        public Texture2D clearBlockEffIceLittle;
        public Texture2D clearBlockEffStoneBig;
        public Texture2D clearBlockEffStoneLittle;

        public Texture2D[] clearBlockEffSpriteFramesYuan;
        public Texture2D[] clearBlockEffSpriteFramesShu;
        public Texture2D[] clearBlockEffSpriteFramesStar;
        public Texture2D[] clearBlockEffSpriteFramesFour;

        public GameObject newBestEff;

        //开场动画
        public GameObject topUI;
        public GameObject blockGroupBg;
        public GameObject readyGroup;
        public SkeletonGraphic boardEff;
        public GameObject boardEffItem;
        public Sprite[] boardEffItemImgs;

        public GameObject windEff;
        public GameObject toIceEff;

        public GameObject b421Eff;
        public Sprite[] b421EffImgs;

        public GameObject readyBgIceEff;
        public GameObject blackMaskEff;

        public GameObject iceTipEff;

        public GameObject readyBgIceStartEff;
        public GameObject readyBgIceClearEff;
        public GameObject readyBgCountDownEff;
        public GameObject readyBgIceGenSuiEff;

        public GameObject specialEffGoldCountDown;

        public GameObject boardFrameEff;
        public GameObject clearToLevelEff;
        public GameObject clearToLevelEffEnd;
        public GameObject levelTextEff;
        public GameObject textLevel;

        public GameObject levelToClearEff;
        public GameObject levelToClearEffEnd;
        
        //青铜块相关特效
        private GameObject _clearSpecialEffBronzeLine;
        private ObjectPool _clearSpecialEffBronzeLinePool;
        private ObjectPool _clearSpecialEffBronzePool;
        
        //金块特效相关
        private Material[] _specialEffGoldMaterials;
        private GameObject _clearSpecialEffGoldLineEff;
        
        private ObjectPool _clearSpecialEdgeEffPool;
        private ObjectPool _clearSpecialEffPool;
        private ObjectPool _specialEffPool;
        private ObjectPool _specialEffBronzePool;
        
        private ObjectPool _specialEffGoldPool;
        private ObjectPool _specialEffStonePool;
        private ObjectPool _clearSpecialEffShanBaoPool;
        private ObjectPool _iceEffPool;
        private ObjectPool _blockItemGrayEffPool;
        private ObjectPool _comboEffPool;
        private ObjectPool _scoreEffPool;
        private ObjectPool _scoreIceEffPool;
        private ObjectPool _clearBlockEffPool;
        private ObjectPool _boardEffItemPool;
        private ObjectPool _windEffPool;
        private ObjectPool _toIceEffPool;
        private ObjectPool[] _toIceEffPoolArr;
        private ObjectPool _readyBgCountDownEffPool;
        private ObjectPool _clearToLevelEffPool;
        private ObjectPool _levelToClearEffPool;
        private ObjectPool _blockScoreEffPool;
        private ObjectPool _blockScoreEff1Pool;
        
        private List<GameObject> _boardEffItemArr;
        
        private GameObject _levelUpEff = null;
        private GameObject _deadWarningEff = null;
        private GameObject _clearTipEff = null;
        private GameObject _guideEff = null;
        private GameObject _newBestEff = null;
        private Action _startAnimCompleteAct = null;
        private Color[] _clearBlockEffColors = null;
        private GameObject _b421Eff = null;
        private GameObject _readyBgIceEff = null;
        private GameObject _blackMaskEff = null;
        private GameObject _boardFrameEff = null;

        private List<GameObject> _fadianEffList = null;
        private List<GameObject> _xianEffList = null;

        private Texture[] _clearIceEffSpriteFrames;
        
        // Start is called before the first frame update
        void Start()
        {
            if (Constant.SpecialGoldCountDownSwitch)
            {
//                clearSpecialEffGold = clearSpecialEffGold2;
            }
            
            clearTipEff2.SetActive(false);

            if (boardEff != null)
            {
                boardEff.AnimationState.Complete += delegate(TrackEntry entry)
                {
                    if (entry.ToString() == "newAnimation")
                    {
                        _startAnimCompleteAct?.Invoke();
                        _startAnimCompleteAct = null;
                        boardEff.gameObject.SetActive(false);
                        blockGroupBg.SetActive(true);

                        foreach (var boardItem in _boardEffItemArr)
                        {
                            _boardEffItemPool.Put(boardItem);
                        }
                    }
                };
                boardEff.gameObject.SetActive(false);
            }

            if (clearToLevelEffEnd != null)
            {
                clearToLevelEffEnd.SetActive(false);
            }

            if (levelTextEff != null)
            {
                levelTextEff.SetActive(false);
            }
            
            if (_clearBlockEffPool == null)
            {
                _clearBlockEffPool = new ObjectPool();
                //                if (Constant.SceneVersion == "3")
                //                {
                //                    var tmpPrefab = await Tools.LoadAssetAsync<GameObject>("Prefabs_Scene3/ClearBlockEff");
                //                    _clearBlockEffPool.CreateObject(tmpPrefab, 24);
                //                }
                //                else
                //                {
                _clearBlockEffPool.CreateObject(clearBlockEff, 24);
                //                }
            }
        }

        public async void ShowStartAnim(Action act = null)
        {
            if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
            {
                act?.Invoke();
                return;
            }

            var oriTopUIPosY = topUI.transform.localPosition.y;
            var oriReadyGroupPosY = readyGroup.transform.localPosition.y;
            blockGroupBg.SetActive(false);
            readyGroup.transform.localPosition = new Vector2(0, oriReadyGroupPosY - Constant.ScreenHeight / 2f);

            await LoadResAsync_boardEffItemPool();
            
            //坐下角起步
            var fadeInTime = 0.15f;
            var waitDeltaTime = 0.07f;
            if (Constant.SceneVersion == "3")
            {
                fadeInTime = 0.7f;
                waitDeltaTime = 0.08f;
            }
            for (var i = 0; i < Constant.Lie; ++i)
            {
                var cx = i;
                var cy = 0;
//                var img = cx % 2 == 0 ? boardEffItemImgs[1] : boardEffItemImgs[0];
//                if (Constant.SceneVersion == "3")
//                {
//                    img = null;
//                }

                while (cx >= 0)
                {
                    var eff = _boardEffItemPool.Get();
                    eff.transform.SetParent(effGroup.transform);
                    eff.transform.localScale = Vector2.one;
//                    if (img != null)
//                    {
//                        eff.GetComponent<Image>().sprite = img;
//                    }
                    eff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth, Constant.BlockHeight);
                    eff.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + cx * Constant.BlockWidth, Constant.BlockGroupEdgeBottom + cy * Constant.BlockHeight);
                    eff.GetComponent<CanvasGroup>().alpha = 0;
                    eff.GetComponent<CanvasGroup>().DOFade(1, fadeInTime).SetDelay(waitDeltaTime * i + waitDeltaTime);
                    _boardEffItemArr.Add(eff);

                    --cx;
                    ++cy;
                }
            }

            //右上角起步
            for (var i = Constant.Lie - 1; i >= 0; --i)
            {
                var cx = i;
                var cy = Constant.Hang - 1;
//                var img = cx % 2 == 0 ? boardEffItemImgs[0] : boardEffItemImgs[1];
//                if (Constant.SceneVersion == "3")
//                {
//                    img = null;
//                }
                while (cx <= Constant.Lie - 1)
                {
                    var eff = _boardEffItemPool.Get();
                    eff.transform.SetParent(effGroup.transform);
                    eff.transform.localScale = Vector2.one;
//                    if (img != null)
//                    {
//                        eff.GetComponent<Image>().sprite = img;
//                    }
                    eff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth, Constant.BlockHeight);
                    eff.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + cx * Constant.BlockWidth, Constant.BlockGroupEdgeBottom + cy * Constant.BlockHeight);
                    eff.GetComponent<CanvasGroup>().alpha = 0;
                    eff.GetComponent<CanvasGroup>().DOFade(1, fadeInTime).SetDelay(waitDeltaTime * (Constant.Lie - 1 - i));
                    _boardEffItemArr.Add(eff);
                    
                    ++cx;
                    --cy;
                }
            }

            //中间特殊
            var cyy = Constant.Hang - 2;
            var cxx = 0;
            while (cxx <= Constant.Lie - 1)
            {
                var eff = _boardEffItemPool.Get();
                eff.transform.SetParent(effGroup.transform);
                eff.transform.localScale = Vector2.one;
//                if (Constant.SceneVersion != "3")
//                {
//                    eff.GetComponent<Image>().sprite = boardEffItemImgs[1];
//                }
                eff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth, Constant.BlockHeight);
                eff.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + cxx * Constant.BlockWidth, Constant.BlockGroupEdgeBottom + cyy * Constant.BlockHeight);
                eff.GetComponent<CanvasGroup>().alpha = 0;
                eff.GetComponent<CanvasGroup>().DOFade(1, fadeInTime).SetDelay(waitDeltaTime * (Constant.Lie));
                _boardEffItemArr.Add(eff);

                ++cxx;
                --cyy;
            }
            
            if (boardEff != null)
            {
                boardEff.gameObject.SetActive(true);
            }

            _startAnimCompleteAct = act;
            if (Constant.SceneVersion == "3")
            {
//                topUI.GetComponent<Animator>().Play("topUI", 0, 0);
                _startAnimCompleteAct = act;
                StartCoroutine(Delay.Run(() =>
                {
                    _startAnimCompleteAct?.Invoke();
                    _startAnimCompleteAct = null;
                    blockGroupBg.SetActive(true);

                    foreach (var boardItem in _boardEffItemArr)
                    {
                        _boardEffItemPool.Put(boardItem);
                    }
                }, 1.3f));
            }
            else
            {
                topUI.transform.localPosition = new Vector2(0, oriTopUIPosY + Constant.ScreenHeight / 4f);
                topUI.transform.DOLocalMoveY(oriTopUIPosY, 0.3f);
                boardEff.AnimationState.SetAnimation(0, "newAnimation", false);
            }
            
            readyGroup.SetActive(true);
            readyGroup.transform.DOLocalMoveY(oriReadyGroupPosY, 0.15f).SetDelay(0.3f);
        }

        #region LoadResAsync

        private async Task<bool> LoadResAsync_comboEffPool()
        {
            if (_comboEffPool == null)
            {
                _comboEffPool = new ObjectPool();
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
             
                } else if (Constant.SceneVersion == "3")
                {
                    comboEffFrame3 = new Sprite[10];
                    for (var i = 0; i < 10; ++i)
                    {
                        comboEffFrame3[i] = Resources.Load<Sprite>("Images3/Eff/ComboScore/"+i);
                    }
                    
                    var comboEffPrefab = MaterPool.Instace.ComboEff;
                    _comboEffPool.CreateObject(comboEffPrefab, 1);
                }
                else
                {
                    _comboEffPool.CreateObject(comboEff, 1);
                }
            }

            return true;
        }

        private async Task<bool> LoadResAsync_scoreEffPool()
        {
            if (_scoreEffPool == null)
            {
                _scoreEffPool = new ObjectPool();
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                   
                } else if (Constant.SceneVersion == "3")
                {
                    scoreEffSpriteFrames = new Sprite[11];
                    for (var i = 0; i < 10; ++i)
                    {
                        scoreEffSpriteFrames[i] = Resources.Load<Sprite>("Images3/Eff/ComboScore/"+i + "_1");
                    }

                    scoreEffSpriteFrames[scoreEffSpriteFrames.Length - 1] = Resources.Load<Sprite>("Images3/Eff/ComboScore/x_1");
                    
                    var scoreEffPrefab = MaterPool.Instace.ScoreEff;
                    _scoreEffPool.CreateObject(scoreEffPrefab);
                }
                else
                {
                    _scoreEffPool.CreateObject(scoreEff);
                }
            }
            
            if (_scoreIceEffPool == null)
            {
                _scoreIceEffPool = new ObjectPool();
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                  
                } else if (Constant.SceneVersion == "3")
                {
                    if (_blockScoreEffPool == null)
                    {
                        _blockScoreEffPool = new ObjectPool();
                        var tmpPrefab1 = MaterPool.Instace.BlockScoreEff;
                        _blockScoreEffPool.CreateObject(tmpPrefab1);
                    }

                    if (_blockScoreEff1Pool == null)
                    {
                        _blockScoreEff1Pool = new ObjectPool();
                        var tmpPrefab2 = MaterPool.Instace.BlockScoreEff1;
                        _blockScoreEff1Pool.CreateObject(tmpPrefab2);
                    }
                }
            }

            return true;
        }

        private async Task<bool> LoadResAsync_boardEffItemPool()
        {
            if (_boardEffItemPool == null)
            {
                _boardEffItemPool = new ObjectPool();
                if (Constant.SceneVersion == "3")
                {
                    var tmpPrefab = MaterPool.Instace.BoardEffItem;
                    _boardEffItemPool.CreateObject(tmpPrefab, 20);
                }
                else if (Constant.SceneVersion == "")
                {
                    _boardEffItemPool.CreateObject(boardEffItem, 20);
                }
            }

            if (_boardEffItemArr == null)
            {
                _boardEffItemArr = new List<GameObject>();
            }

            return true;
        }

        public async void LoadResAsync_manRes()
        {
            if (Constant.SceneVersion == "3")
            {
//                var manMat = await Tools.LoadAssetAsync<Material>("man_mat");
//                var manSkeletonData = await Tools.LoadAssetAsync<SkeletonDataAsset>("man_sket");
//                var manRes = await Tools.LoadAssetAsync<GameObject>("man");
//                var manObj = Instantiate(manRes,
//                    topUI.transform.Find("levelGroup").transform.Find("manBg").transform, false);
//                manObj.transform.SetParent(topUI.transform.Find("levelGroup").transform.Find("manBg").transform,
//                    false);
//                manObj.transform.localPosition = new Vector2(0, 5000);
//                var manSkeletonGraphicComponent = manObj.AddComponent<SkeletonGraphicExt>();
//                manSkeletonGraphicComponent.skeletonDataAsset = manSkeletonData;
//                manSkeletonGraphicComponent.material = manMat;
//                await manSkeletonGraphicComponent.InitializeAsync();
//                manObj.name = "man";
//                manObj.transform.localPosition = new Vector2(0, -125);
//                manObj.GetComponent<Man>().StartShow();
//                
//                Destroy(topUI.transform.Find("levelGroup").transform.Find("manBg").transform.Find("manDefault").gameObject);
            }
        }

        private async Task<bool> LoadResAsync_effBronze()
        {
            if (_specialEffBronzePool == null)
            {
                _specialEffBronzePool = new ObjectPool();
                
                if (Constant.SceneVersion == "3")
                {
                    specialEffBronzeSpriteFrames = new Sprite[4];
                    for (var i = 0; i < 4; ++i)
                    {
                        specialEffBronzeSpriteFrames[i] =
                            Resources.Load<Sprite>("Images3/Eff/BlockSpecial/block_bronze_" + (i + 1));
                    }

                    clearSpecialEffBronze = MaterPool.Instace.ClearSpecialEffBronze;
                    _clearSpecialEffBronzeLine = MaterPool.Instace.ClearSpecialEffBronzeLine;

                    specialEffBronze = MaterPool.Instace.SpecialEffBronze;
                    _specialEffBronzePool.CreateObject(specialEffBronze, 1);
                }
                else
                {
                    _specialEffBronzePool.CreateObject(specialEffBronze, 1);
                }
            }

            return true;
        }

        private async Task<bool> LoadResAsync_effGold()
        {
            if (_specialEffGoldPool == null)
            {
                _specialEffGoldPool = new ObjectPool();
                if (Constant.SceneVersion == "3")
                {
                    specialEffGoldSpriteFrames = new Sprite[4];
                    for (var i = 0; i < 4; ++i)
                    {
                        specialEffGoldSpriteFrames[i] = Resources.Load<Sprite>("Images3/Eff/BlockSpecial/block_gold_" + (i + 1));
                    }
            
                    _specialEffGoldMaterials = new Material[4];
                    for (var i = 0; i < 4; ++i)
                    {
                        _specialEffGoldMaterials[i] = Resources.Load<Material>("Images3/Eff/BlockSpecialMat/zhezhao" + (i + 1));
                    }

                    clearSpecialEffGold = MaterPool.Instace.ClearSpecialEffGold;
                    _clearSpecialEffGoldLineEff = MaterPool.Instace.ClearSpecialEffGoldLine;

                    specialEffGold = MaterPool.Instace.SpecialEffGold;
                    _specialEffGoldPool.CreateObject(specialEffGold, 1);

                    //实例化一次金块消除时的特效，使得特效创建时速度加快
                    var a = Instantiate(clearSpecialEffGold);
                    var b = Instantiate(_clearSpecialEffGoldLineEff);
                    a.name = "ClearSpecial_a";
                    b.name = "ClearSpecial_b";
                    a.SetActive(false);
                    b.SetActive(false);
                    Destroy(a);
                    Destroy(b);
                }
                else
                {
                    _specialEffGoldPool.CreateObject(specialEffGold);
                }
            }

            return true;
        }

        private async Task<bool> LoadResAsync_iceEff()
        {
            if (_iceEffPool == null)
            {
                _iceEffPool = new ObjectPool();
                if (Constant.SceneVersion == "3")
                {
                    if (_clearIceEffSpriteFrames == null)
                    {
                        _clearIceEffSpriteFrames = new Texture[4];
                        _clearIceEffSpriteFrames[0] = Resources.Load<Texture>("Images3/Eff/ParticleImg/Ice_K");
                        _clearIceEffSpriteFrames[1] = Resources.Load<Texture>("Images3/Eff/ParticleImg/Ice_K2");
                        _clearIceEffSpriteFrames[2] = Resources.Load<Texture>("Images3/Eff/ParticleImg/Ice_K3");
                        _clearIceEffSpriteFrames[3] = Resources.Load<Texture>("Images3/Eff/ParticleImg/Ice_ster");
                    }
                    
                    iceEffFrame = new Sprite[4];
                    for (var i = 0; i < 4; ++i)
                    {
                        iceEffFrame[i] =
                            Resources.Load<Sprite>("Images3/Eff/BlockIce/ice_" + (i + 1));
                    }

                    iceEff = MaterPool.Instace.IceEff;
                    _iceEffPool.CreateObject(iceEff);
                }
                else
                {
                    _iceEffPool.CreateObject(iceEff);
                }
            }

            return true;
        }

        #endregion

        public void LoadResAsync_AfterLoadScene()
        {
            if (Constant.IceBlockSwitch)
            {
                LoadResAsync_iceEff();
            }

            if (Constant.SpecialGoldSwitch)
            {
                LoadResAsync_effGold();
            }

            if (Constant.SpecialBronzeSwitch)
            {
                LoadResAsync_effBronze();
            }
        }

        public async void AddSpecialEff(GameObject item, int specialType = (int)Blocks.Special.Rainbow, bool specialGoldSuccess = false)
        {
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            switch (specialType)
            {
                case (int)Blocks.Special.Rainbow:
                    if (_specialEffPool == null)
                    {
                        _specialEffPool = new ObjectPool();
                        _specialEffPool.CreateObject(specialEff, 2);
                    }
                    
                    var eff = _specialEffPool.Get();
                    eff.transform.SetParent(item.transform);
                    eff.transform.localScale = Vector2.one;
                    eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
                    eff.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    eff.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;
                    eff.transform.Find("frame").GetComponent<Image>().sprite = specialEffFrame[itemLength - 1];
                    eff.transform.Find("mask").transform.Find("specialEff").GetComponent<Animator>().Play("specialEff");
                    break;
                case (int)Blocks.Special.Bronze:
                    await LoadResAsync_effBronze();
                    
                    if (_specialEffBronzePool == null)
                    {
                        return;
                    } else if (!_specialEffBronzePool.HaveObjectBase())
                    {
                        while (!_specialEffBronzePool.HaveObjectBase())
                        {
                            await Task.Delay(100);
                        }
                    }
                    
                    var effBronze = _specialEffBronzePool.Get();
                    effBronze.transform.SetParent(item.transform);
                    effBronze.GetComponent<Image>().sprite = specialEffBronzeSpriteFrames[itemLength - 1];

                    if (Constant.SceneVersion == "3")
                    {
                        effBronze.transform.Find("eff").GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,
                            itemLength.ToString(), true);

                        effBronze.transform.Find("par").transform.Find("mask").transform.localScale =
                            new Vector2((itemLength * Constant.BlockWidth - 25) / 455f * 100, 100);
                    }
                    
                    effBronze.transform.localScale = Vector2.one;
                    effBronze.transform.localPosition = item.transform.Find("img").transform.localPosition;
                    effBronze.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    effBronze.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;
                    break;
                case (int)Blocks.Special.Gold:
                    await LoadResAsync_effGold();

                    if (_specialEffGoldPool == null)
                    {
                        return;
                    } else if (!_specialEffGoldPool.HaveObjectBase())
                    {
                        while (!_specialEffGoldPool.HaveObjectBase())
                        {
                            await Task.Delay(100);
                        }
                    }
                        
                    var effGold = _specialEffGoldPool.Get();
                    effGold.transform.SetParent(item.transform);
                    effGold.GetComponent<Image>().sprite = specialEffGoldSpriteFrames[itemLength - 1];
                    effGold.transform.localScale = Vector2.one;
                    effGold.transform.localPosition = item.transform.Find("img").transform.localPosition;
                    effGold.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    effGold.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;

                    if (Constant.SceneVersion == "3")
                    {
                        effGold.transform.Find("huangjin").transform.localScale = new Vector2(
                            (itemLength * Constant.BlockWidth - 25 * Constant.WRatio) / 455f * 100, 100 * Constant.HRatio);

                        effGold.transform.Find("eff").GetComponent<Image>().material =
                            _specialEffGoldMaterials[itemLength - 1];
                        
                        effGold.transform.Find("eff").GetComponent<RectTransform>().sizeDelta = new Vector2(itemLength * Constant.BlockWidth - 25 * Constant.WRatio, 95 * Constant.HRatio);

                        var originalSize = effGold.transform.Find("frame").GetComponent<RectTransform>().sizeDelta;
                        effGold.transform.Find("frame").GetComponent<RectTransform>().sizeDelta = new Vector2(40 * Constant.WRatio + itemLength * Constant.BlockWidth, originalSize.y);
                        
                        effGold.transform.Find("fangdianEff").gameObject.SetActive(false);
                        
                        effGold.transform.Find("eff").gameObject.SetActive(false);
                        effGold.transform.Find("frame").gameObject.SetActive(false);
                        StartCoroutine(Delay.Run(() =>
                        {
                            effGold.transform.Find("eff").gameObject.SetActive(true);
                            effGold.transform.Find("frame").gameObject.SetActive(true);
                            effGold.GetComponent<Animator>().Play("SpecialEffGold");
                        }, Constant.UpAnimTime));
                    }
                    
                    ShowSpecialGoldOtherEff(item, "daiji");
                    if (!specialGoldSuccess)
                    {
                        AddSpecialEffGoldCountDown(item);
                    }
                    break;
                case (int) Blocks.Special.Stone:
                    if (_specialEffStonePool == null)
                    {
                        _specialEffStonePool = new ObjectPool();
                        _specialEffStonePool.CreateObject(specialEffStone, 1);
                    }
                    
                    var effStone = _specialEffStonePool.Get();
                    effStone.transform.SetParent(item.transform);
                    effStone.GetComponent<Image>().sprite = specialEffStoneSpriteFrames[itemLength - 1];
                    effStone.transform.localScale = Vector2.one;
                    effStone.transform.localPosition = item.transform.Find("img").transform.localPosition;
                    effStone.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    effStone.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;
                    break;
            }
        }

        public void AddSpecialEffStone(GameObject item)
        {
            if (item == null) return;
            
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            var eff = Instantiate(toStoneEff[itemLength - 1], item.transform, false);
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;

            StartCoroutine(Delay.Run(() => { AddSpecialEff(item, (int) Blocks.Special.Stone); }, 0.8f));

            StartCoroutine(Delay.Run(() =>
            {
                Destroy(eff);
            }, 3f));
        }

        private void AddSpecialEffGoldCountDown(GameObject item)
        {
            if (Constant.SpecialGoldCountDownSwitch)
            {
                var goldEff = item.transform.Find("SpecialEffGold(Clone)");
                if (goldEff != null)
                {
                    var eff = Instantiate(specialEffGoldCountDown, goldEff.transform, false);
                    eff.transform.localScale = new Vector2(0.9f, 0.9f);
                    eff.transform.localPosition = Vector2.zero;
                    eff.transform.SetAsFirstSibling();
                    eff.GetComponent<SpecialEffGoldCountDown>().StartCountDown();
                }
            }
        }

        public void RemoveSpecialEffGoldCountDown(GameObject item)
        {
            var goldItem = item.transform.Find("SpecialEffGold(Clone)");
            if (goldItem != null)
            {
                if (goldItem.transform.Find("SpecialEffGoldCountDown(Clone)") != null)
                {
                    Destroy(goldItem.transform.Find("SpecialEffGoldCountDown(Clone)").gameObject);
                }
            }
        }

        public bool IsSpecialEffGoldCountDowning(GameObject item)
        {
            var goldItem = item.transform.Find("SpecialEffGold(Clone)");
            if (goldItem != null)
            {
                return goldItem.transform.Find("SpecialEffGoldCountDown(Clone)") != null;
            }

            return false;
        }

        public void RemoveSpecialEff(GameObject item)
        {
            if (item.transform.Find("SpecialEff(Clone)") != null)
            {
                _specialEffPool.Put(item.transform.Find("SpecialEff(Clone)").gameObject);
            }

            if (item.transform.Find("SpecialEffBronze(Clone)") != null)
            {
                _specialEffBronzePool.Put(item.transform.Find("SpecialEffBronze(Clone)").gameObject);
            }

            RemoveSpecialEffGoldCountDown(item);
            
            if (item.transform.Find("SpecialEffGold(Clone)") != null)
            {
                _specialEffGoldPool.Put(item.transform.Find("SpecialEffGold(Clone)").gameObject);
            }
            
            if (item.transform.Find("ClearSpecialEff(Clone)") != null)
            {
                RemoveClearSpecialEff(item);
            }

            if (item.transform.Find("SpecialEffStone(Clone)") != null)
            {
                _specialEffStonePool.Put(item.transform.Find("SpecialEffStone(Clone)").gameObject);
            }

            if (item.transform.Find("ToStone1(Clone)") != null)
            {
                Destroy(item.transform.Find("ToStone1(Clone)").gameObject);
            }
            
            if (item.transform.Find("ToStone2(Clone)") != null)
            {
                Destroy(item.transform.Find("ToStone2(Clone)").gameObject);
            }
        }

        //daiji_1，tishi_1
        public void ShowSpecialGoldOtherEff(GameObject item, string otherEffType)
        {
            if (Constant.SceneVersion == "3") return;
            
            var goldEff = item.transform.Find("SpecialEffGold(Clone)");
            if (goldEff != null)
            {
                var childCount = goldEff.transform.childCount;
                for (var i = 0; i < childCount ; i++)
                {
                    if (goldEff.transform.GetChild(i).gameObject.name != "SpecialEffGoldCountDown(Clone)")
                    {
                        Destroy(goldEff.transform.GetChild(i).gameObject);
                    }
                }
                
                var itemScript = item.GetComponent<BlockItem>();
                var eff = Instantiate(clearSpecialEffGold, goldEff.transform, false);
                eff.transform.localScale = Vector3.one;
                eff.GetComponent<SkeletonGraphic>().AnimationState
                    .SetAnimation(0, otherEffType + "_" + itemScript.GetLength(), true);
            }
        }

        public async void AddIceEff(GameObject item, bool showAnim = true)
        {
            await LoadResAsync_iceEff();

            if (_iceEffPool == null)
            {
                return;
            } else if (!_iceEffPool.HaveObjectBase())
            {
                while (!_iceEffPool.HaveObjectBase())
                {
                    await Task.Delay(100);
                }
            }
            
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            var eff = _iceEffPool.Get();
            eff.transform.SetParent(item.transform);
            eff.transform.localScale = Vector2.one;
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            eff.transform.localRotation = Quaternion.Euler(0, 0, 0);
            eff.GetComponent<Image>().sprite = iceEffFrame[itemLength - 1];
            eff.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;

            if (showAnim)
            {
                eff.GetComponent<CanvasGroup>().alpha = 0;
                eff.GetComponent<CanvasGroup>().DOFade(Constant.SceneVersion == "3" ? 3 : 1, Constant.IceTime);

                if (Constant.SceneVersion == "2")
                {
                    if (_toIceEffPool == null)
                    {
                        _toIceEffPool = new ObjectPool();
                        _toIceEffPool.CreateObject(toIceEff, 2);
                    }
                    
                    var toIce = _toIceEffPool.Get();
                    toIce.transform.SetParent(item.transform);
                    toIce.transform.localScale = Vector2.one;
                    toIce.transform.localPosition = item.transform.Find("img").transform.localPosition;

                    StartCoroutine(Delay.Run(() =>
                    {
                        _toIceEffPool.Put(toIce);
                    }, 2f));
                }
                else if (Constant.SceneVersion == "3")
                {
                    if (_toIceEffPoolArr == null)
                    {
                        _toIceEffPoolArr = new ObjectPool[4];
                        for (var i = 0; i < 4; ++i)
                        {
                            var iceNum = i;
                            _toIceEffPoolArr[iceNum] = new ObjectPool();
                            GameObject tmpPrefab = null;
                            if (iceNum == 0)
                            {
                                tmpPrefab = MaterPool.Instace.ToIceEff_0;

                            }else if (iceNum == 1)
                            {
                                tmpPrefab = MaterPool.Instace.ToIceEff_1;
                            }else if (iceNum == 2)
                            {
                                tmpPrefab = MaterPool.Instace.ToIceEff_2;
                            }else if (iceNum == 3)
                            {
                                tmpPrefab = MaterPool.Instace.ToIceEff_3;
                            }
                            _toIceEffPoolArr[iceNum].CreateObject(tmpPrefab);
                        }
                    }

                    while (_toIceEffPoolArr[itemLength - 1] == null || (_toIceEffPoolArr[itemLength - 1] != null && !_toIceEffPoolArr[itemLength - 1].HaveObjectBase()))
                    {
                        await Task.Delay(100);
                    }
                    
                    var toIce = _toIceEffPoolArr[itemLength - 1].Get();
                    toIce.transform.SetParent(item.transform);
                    toIce.transform.localScale = new Vector2(Constant.WRatio, Constant.HRatio);
                    toIce.transform.localPosition = item.transform.Find("img").transform.localPosition;

                    StartCoroutine(Delay.Run(() =>
                    {
                        _toIceEffPoolArr[itemLength - 1].Put(toIce);
                    }, 5f));
                }
            }
        }

        public void RemoveIceEff(GameObject item)
        {
            if (item.transform.Find("IceEff(Clone)") != null)
            {
                _iceEffPool.Put(item.transform.Find("IceEff(Clone)").gameObject);
            }
        }

        public async void ShowLevelUpEff(int level)
        {
            if (Blocks.IsTesting()) return;

            switch (Constant.SceneVersion)
            {
                case "1":
                    if (textLevel != null)
                    {
                        textLevel.transform.DOScale(1.5f, 0.3f);
                        StartCoroutine(Delay.Run(() =>
                        {
                            textLevel.transform.DOScale(1, 0.3f);
                        }, 1.5f));
                    }
                    break;
                case "2":
//                    if (levelTextEff != null)
//                    {
//                        levelTextEff.SetActive(true);
//                        levelTextEff.GetComponent<SkeletonGraphic>().AnimationState
//                            .SetAnimation(0, "2", false);
//
//                        StartCoroutine(Delay.Run(() => { levelTextEff.SetActive(false); }, 1));
//                    }

                    if (textLevel != null)
                    {
                        var lightEff = textLevel.transform.Find("light");
                        lightEff.gameObject.SetActive(true);
                        lightEff.GetComponent<CanvasGroup>().alpha = 0;
                        lightEff.GetComponent<TextMeshProUGUI>().text = textLevel.GetComponent<TextMeshProUGUI>().text;
                        lightEff.GetComponent<CanvasGroup>().DOFade(1, 0.2f).OnComplete(() =>
                        {
                            lightEff.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(0.3f).OnComplete(() =>
                            {
                                lightEff.gameObject.SetActive(false);
                            });
                        });
                    }
                    break;
            }
            
            if (Constant.LevelUpEffVersion == "1" || Constant.LevelUpEffVersion == "2")
            {
               
            }else if (Constant.LevelUpEffVersion == "3")
            {
                //unity动画
                if (_levelUpEff == null)
                {
                    levelUpEffNumFrames = new Sprite[10];
                    for (var i = 0; i < 10; i++)
                    {
                        levelUpEffNumFrames[i] =
                            Resources.Load<Sprite>("Images3/Eff/LevelUp/" + i);
                    }

                    var tmpPrefab = MaterPool.Instace.LevelUpEff;
                    _levelUpEff = Instantiate(tmpPrefab, effGroup.transform, false);
                    _levelUpEff.transform.localScale = Vector2.one;
                    _levelUpEff.transform.localPosition = new Vector2(0, 450);
                    _levelUpEff.SetActive(false);
                }
                
                if (level >= 10)
                {
                    _levelUpEff.transform.Find("eff").transform.Find("num1").gameObject.SetActive(true);
                    _levelUpEff.transform.Find("eff").transform.Find("num").GetComponent<Image>().sprite =
                        levelUpEffNumFrames[level / 10];
                    _levelUpEff.transform.Find("eff").transform.Find("num1").GetComponent<Image>().sprite =
                        levelUpEffNumFrames[level % 10];
                }
                else
                {
                    _levelUpEff.transform.Find("eff").transform.Find("num1").gameObject.SetActive(false);
                    _levelUpEff.transform.Find("eff").transform.Find("num").GetComponent<Image>().sprite =
                        levelUpEffNumFrames[level];
                }
                
                _levelUpEff.SetActive(true);
                _levelUpEff.GetComponent<Animator>().Play("levelUp", 0, 0);
                
                StartCoroutine(Delay.Run(() =>
                {
                    ManagerAudio.PlaySound("newBest");
                }, 0.4f));
                
                StartCoroutine(Delay.Run(() =>
                {
                    _levelUpEff.SetActive(false);
                }, 2.9f));
            }
            else
            {
                if (_levelUpEff == null)
                {
                    _levelUpEff = Instantiate(levelUpEffPrefab, effGroup.transform, false);
                    _levelUpEff.transform.localScale = Vector2.one;
                    _levelUpEff.transform.localPosition = new Vector2(0, 450);
                    _levelUpEff.SetActive(false);
                }
            
                StartCoroutine(Delay.Run(() =>
                {
                    if (!_levelUpEff.transform.Find("eff").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0)
                        .IsName("levelUpEff"))
                    {
                        _levelUpEff.SetActive(false);
                    }
                }, 1.7f));
            
                _levelUpEff.SetActive(true);
                _levelUpEff.transform.Find("eff").GetComponent<Animator>().Play("levelUpEff", 0, 0);
                _levelUpEff.transform.Find("eff").GetComponent<TextMeshProUGUI>().text = Constant.LevelUpText + level;
                
                StartCoroutine(Delay.Run(() =>
                {
                    ManagerAudio.PlaySound("newBest");
                }, 0.4f));
            }

            if (Constant.LevelUpOtherEffSwitch)
            {
                ShowBoardFrameEff();
                Constant.GamePlayScript.bgGroup.GetComponent<BgGroupEff>().UpdateEffByLevel(Blocks.CurLevel - 1);
            }
        }
        
        public async void ShowNewBestEff()
        {
            if (Blocks.IsTesting()) return;
            
            if (_newBestEff == null)
            {
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    
                }
                else
                {
                    _newBestEff = Instantiate(newBestEff, effGroup.transform, false);
                }
                
                _newBestEff.transform.localScale = Vector2.one;
                _newBestEff.transform.localPosition = new Vector2(0, 350);
                _newBestEff.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry state)
                {
                    _newBestEff.SetActive(false);
                };
                _newBestEff.SetActive(false);
            }
            
            _newBestEff.SetActive(true);
            _newBestEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "new_best_score", false);

            StartCoroutine(Delay.Run(() =>
            {
                ManagerAudio.PlaySound("newBest");
            }, 0.4f));
        }

        public void ShowSecondChanceClearHangEff(Action callback = null, int hang = 100)
        {
            var eff = Instantiate(clearHangLightEff, effGroup.transform, false);
            eff.transform.localScale = Vector2.one;

            if (hang == 100)
            {
                var tmpY = Constant.BlockGroupEdgeBottom + Constant.SecondChanceHangStart * Constant.BlockHeight +
                           Constant.BlockGroupEdgeBottom +
                            (Constant.SecondChanceHangStart + Constant.SecondChanceHangNum) * Constant.BlockHeight;
                tmpY /= 2;
                eff.transform.localPosition = new Vector2(0, tmpY);
                eff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.Lie * Constant.BlockWidth + 80,  Constant.SecondChanceHangNum * Constant.BlockHeight + 80);
            }
            else
            {
                eff.transform.localPosition = new Vector2(0, Constant.BlockGroupEdgeBottom + hang * Constant.BlockHeight + Constant.BlockHeight / 2);
                eff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.Lie * Constant.BlockWidth + 80,  Constant.BlockHeight + 80);
            }
            
            var effImg = eff.GetComponent<Image>();
            var tmpColor = effImg.color;
            tmpColor.a = 0;
            effImg.color = tmpColor;
            effImg.DOFade(1, 0.5f).OnComplete(() =>
            {
                StartCoroutine(Delay.Run(() =>
                {
                    callback?.Invoke();
                    effImg.DOFade(0, Constant.BlockRemoveTime).OnComplete(() =>
                    {
                        Destroy(eff);
                    });
                }, Constant.SecondChanceClearEffTime));
            });
        }
        
        public void ShowClearSpecialEdgeEff(GameObject item, float delayTime = 0.02f, float opacity = 0.5f, float fadeTime = 0.2f)
        {
            if (item == null) return;

            if (_clearSpecialEdgeEffPool == null)
            {
                _clearSpecialEdgeEffPool = new ObjectPool();
                _clearSpecialEdgeEffPool.CreateObject(clearSpecialEdgeEff, 3);
            }
            
            var eff = _clearSpecialEdgeEffPool.Get();
            eff.transform.SetParent(item.transform);
            eff.transform.localScale = Vector2.one;
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            eff.GetComponent<RectTransform>().sizeDelta =
                item.GetComponent<RectTransform>().sizeDelta;
            
            var effImg = eff.GetComponent<Image>();
            effImg.sprite = item.GetComponent<BlockItem>().GetSprite();
            var tmpColor = eff.GetComponent<Image>().color;
            tmpColor.a = 0;
            effImg.color = tmpColor;
            
            effImg.DOFade(opacity, fadeTime).OnComplete(() =>
            {
                effImg.DOFade(0, fadeTime).SetDelay(delayTime).OnComplete(() =>
                {
                    _clearSpecialEdgeEffPool.Put(eff);
                });
            });
        }
        
        public void ShowClearSpecialEff(GameObject item)
        {
            if (item == null) return;
            
            ShowClearSpecialEdgeEff(item);

            if (_clearSpecialEffPool == null)
            {
                _clearSpecialEffPool = new ObjectPool();
                _clearSpecialEffPool.CreateObject(clearSpecialEff, 2);
            }
            
            var eff = _clearSpecialEffPool.Get();
            eff.transform.SetParent(item.transform);
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            eff.transform.localScale = new Vector2(item.GetComponent<BlockItem>().GetLength() * Constant.BlockWidth * 1.0f / 240.0f, Constant.BlockHeight * 1.0f / 120.0f);
            eff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "newAnimation", false);
        }

        private void RemoveClearSpecialEff(GameObject item)
        {
            _clearSpecialEffPool.Put(item.transform.Find("ClearSpecialEff(Clone)").gameObject);
        }

        public async void ShowClearSpecialEffBronze(int bronzePosIndex, List<int[]> splitInfo)
        {
//            Time.timeScale = 0.1f;
//            StartCoroutine(Delay.Run(() =>
//            {
//                Time.timeScale = 1;
//            }, 1.5f));
            
            var bronzeItem = Constant.GamePlayScript.GetBlockItemByPosIndex(bronzePosIndex);
            if (bronzeItem != null)
            {
                if (Constant.SceneVersion == "2")
                {
                    var eff1 = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                    eff1.transform.localScale = Vector2.one;
                    eff1.transform.localPosition = bronzeItem.transform.localPosition +
                                                   bronzeItem.transform.Find("img").transform.localPosition;
                    eff1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dabao", false);
                    StartCoroutine(Delay.Run(() =>
                    {
                        eff1.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                        Destroy(eff1);
                    }, 0.5f));
                    
                    foreach (var splitData in splitInfo)
                    {
                        var eff = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                        eff.transform.localScale = Vector2.one;
                        eff.transform.localPosition = eff1.transform.localPosition;
                        eff.transform.Find("sankai").gameObject.SetActive(true);
    
                        var splitItem = Constant.GamePlayScript.GetBlockItemByPosIndex(splitData[0]);
                        if (splitItem == null)
                        {
                            Destroy(eff);
                            continue;
                        }
                        
                        var splitItemLength = splitItem.GetComponent<BlockItem>().GetLength();
                        var dstPos = Blocks.GetLocalPosByPosIndex(splitData[1]) + new Vector3(0, Constant.BlockHeight / 2f, 0);
                        eff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dfly_2", true);
                        
                        eff.transform.DOLocalMove(dstPos, 0.6f).OnComplete(() =>
                        {
                            var animName = "";
                            if (splitItemLength == 3)
                            {
                                if (splitData[1] - splitData[0] == 1)
                                {
                                    animName = "beiji_3_zuo";
                                }else if (splitData[1] - splitData[0] == 2)
                                {
                                    animName = "beiji_3_you";
                                }
                            }
                            else
                            {
                                animName = "beiji_" + splitItemLength;
                            }

                            var splitEff = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                            splitEff.transform.localScale = Vector2.one;
                            splitEff.transform.localPosition = dstPos;
                            splitEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, animName, false);
                            StartCoroutine(Delay.Run(() =>
                            {
                                splitEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                Destroy(splitEff);
                            }, 0.5f));

                            StartCoroutine(Delay.Run(() =>
                            {
                                eff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                Destroy(eff);
                            }, 0.3f));
                                                       
                            var item = Constant.GamePlayScript.GetBlockItemByPosIndex(splitData[0]);
                            if (item.GetComponent<BlockItem>().HaveIce())
                            {
                                ShowClearIceEff(item);
                            }
                            else
                            {
                                ShowClearBlockEff(item, false);
                            }
                        });
                    }
                } else if (Constant.SceneVersion == "3")
                {
                    if (clearSpecialEffBronze == null || _clearSpecialEffBronzeLine == null)
                    {
                        return;
                    }
                    
                    if (_clearSpecialEffPool == null)
                    {
                        _clearSpecialEffBronzePool = new ObjectPool();
                        _clearSpecialEffBronzePool.CreateObject(clearSpecialEffBronze);
                
                        _clearSpecialEffBronzeLinePool = new ObjectPool();
                        _clearSpecialEffBronzeLinePool.CreateObject(_clearSpecialEffBronzeLine);
                    }
            
                    foreach (var splitData in splitInfo)
                    {
                        var splitItem = Constant.GamePlayScript.GetBlockItemByPosIndex(splitData[0]);
                        if (splitItem == null)
                        {
                            continue;
                        }

                        if (!Blocks.HaveBlock(splitData[0]) && !Blocks.HaveBlock(splitData[1]))
                        {
                            continue;
                        }
                        
                        var line = _clearSpecialEffBronzeLinePool.Get();
                        if (line != null)
                        {
                            line.transform.SetParent(effGroup.transform, false);
                            line.transform.localPosition = bronzeItem.transform.localPosition +
                                                           bronzeItem.transform.Find("img").transform.localPosition;
                            
                            var splitItemLength = splitItem.GetComponent<BlockItem>().GetLength();
                            var splitItemColor = splitItem.GetComponent<BlockItem>().GetColor();
                            var dstPos = Blocks.GetLocalPosByPosIndex(splitData[1]) + new Vector3(0, Constant.BlockHeight / 2f, 0);
                            line.transform.DOLocalMove(dstPos, 0.2f).OnComplete(() =>
                            {
                                if (splitItem.gameObject.activeInHierarchy)
                                {
                                    ShowClearSpecialEdgeEff(splitItem, 0, 1f, 0.2f);
                                }
                                
                                Constant.GameStatusData.SpecialBlueSelectShake++;
                                
                                StartCoroutine(Delay.Run(() =>
                                {
                                    _clearSpecialEffBronzeLinePool.Put(line);

                                    if (splitItem.activeInHierarchy)
                                    {
                                        splitItem.SetActive(false);
                                    }
                                    
                                    var animName = "qiege" + splitItemLength + "_" + splitItemColor;
                                    if (splitItemLength == 3)
                                    {
                                        if (splitData[1] - splitData[0] == 1)
                                        {
                                            animName += "Y";
                                        }else if (splitData[1] - splitData[0] == 2)
                                        {
                                            animName += "Z";
                                        }
                                    }

                                    if (animName == "") return;
                                    
                                    Constant.GameStatusData.SpecialBlueSplitShake++;
                                    var splitEff = _clearSpecialEffBronzePool.Get();
                                    splitEff.transform.SetParent(effGroup.transform, false);
                                    splitEff.transform.localScale = new Vector2(Constant.WRatio, Constant.HRatio);
                                    splitEff.transform.localPosition = dstPos;
                                    splitEff.transform.Find("eff").GetComponent<SkeletonGraphic>().AnimationState
                                        .SetAnimation(0, animName, false);
                                    StartCoroutine(Delay.Run(() => { _clearSpecialEffBronzePool.Put(splitEff); }, 1.5f));
                                }, 0.3f));
                            });
                        }
                    }
                }
            }
        }

        public void ShowClearSpecialEffGoldSplit(int goldPosIndex, List<int[]> edgeToNew)
        {
            var goldItem = Constant.GamePlayScript.GetBlockItemByPosIndex(goldPosIndex);
            if (goldItem != null)
            {
                if (Constant.SceneVersion == "2")
                {
                    var eff1 = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                    eff1.transform.localScale = Vector2.one;
                    eff1.transform.localPosition = goldItem.transform.localPosition +
                                                   goldItem.transform.Find("img").transform.localPosition;
                    eff1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dabao", false);
                    StartCoroutine(Delay.Run(() =>
                    {
                        eff1.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                        Destroy(eff1);
                    }, 0.5f));
                    
                    foreach (var splitData in edgeToNew)
                    {
                        for (var i = 1; i < splitData.Length; ++i)
                        {
                            var eff = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                            eff.transform.localScale = Vector2.one;
                            eff.transform.localPosition = eff1.transform.localPosition;
                            eff.transform.Find("sankai").gameObject.SetActive(true);
        
                            var splitItem = Constant.GamePlayScript.GetBlockItemByPosIndex(splitData[0]);
                            if (splitItem == null)
                            {
                                Destroy(eff);
                                continue;
                            }
                        
                            var dstPos = Blocks.GetLocalPosByPosIndex(splitData[i]) + new Vector3(0, Constant.BlockHeight / 2f, 0);
                            eff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dfly_2", true);
    
                            var i1 = i;
                            eff.transform.DOLocalMove(dstPos, 0.6f).OnComplete(() =>
                            {
                                var animName = "beiji_2";
                                var splitEff = Instantiate(clearSpecialEffBronze, effGroup.transform, false);
                                splitEff.transform.localScale = Vector2.one;
                                splitEff.transform.localPosition = dstPos;
                                splitEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, animName, false);
                                StartCoroutine(Delay.Run(() =>
                                {
                                    splitEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                    Destroy(splitEff);
                                }, 0.5f));
    
                                StartCoroutine(Delay.Run(() =>
                                {
                                    eff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                    Destroy(eff);
                                }, 0.3f));
    
                                if (i1 == splitData.Length - 1)
                                {
                                    var item = Constant.GamePlayScript.GetBlockItemByPosIndex(splitData[0]);
                                    if (item.GetComponent<BlockItem>().HaveIce())
                                    {
                                        ShowClearIceEff(item);
                                    }
                                    else
                                    {
                                        ShowClearBlockEff(item, false);
                                    }
                                }
                            });
                        }
                    }
                } else if (Constant.SceneVersion == "3")
                {
                    
                }
            }
        }

        public void ShowClearSpecialEffGold(int goldPosIndex, List<int> edgeBlocks)
        {
//            Time.timeScale = 0.1f;
//            StartCoroutine(Delay.Run(() =>
//            {
//                Time.timeScale = 1;
//            }, 2.5f));
            
            var goldItem = Constant.GamePlayScript.GetBlockItemByPosIndex(goldPosIndex);
            if (goldItem != null)
            {
                if (Constant.SceneVersion == "2")
                {
                    var eff1 = Instantiate(clearSpecialEffGold, effGroup.transform, false);
                    eff1.transform.localScale = Vector2.one;
                    eff1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    eff1.transform.localPosition = goldItem.transform.localPosition +
                                                   goldItem.transform.Find("img").transform.localPosition;
                    eff1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "y_fadian_" + goldItem.GetComponent<BlockItem>().GetLength(), true);
                    StartCoroutine(Delay.Run(() =>
                    {
                        eff1.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                        Destroy(eff1);
                    }, 0.5f + Constant.ClearWaitTime + Constant.SpecialGoldClearTime + edgeBlocks.Count * Constant.SpecialGoldClearIntervalTime));
    
                    if (edgeBlocks != null && edgeBlocks.Count > 0)
                    {
                        var shanBaoEffScale = new Vector2(0.6f, 0.6f);
                        var count = 0;
                        foreach (var posIndex in edgeBlocks)
                        {
                            ++count;
                            
                            var edgeItem = Constant.GamePlayScript.GetBlockItemByPosIndex(posIndex);
                            if (edgeItem == null) continue;
                            var edgeItemLength = edgeItem.GetComponent<BlockItem>().GetLength();
                            var goldItemPos = eff1.transform.localPosition;
                            var dstPos = edgeItem.transform.localPosition + edgeItem.transform.Find("img").transform.localPosition;
                            var dis = Vector2.Distance(goldItemPos, dstPos);
                            
                            var angle = Vector2.Angle(Vector2.up, new Vector2(dstPos.x - goldItemPos.x, dstPos.y - goldItemPos.y));
                            if (goldItemPos.x < dstPos.x)
                            {
                                angle = -angle;
                            }
    
                            var xianEff = Instantiate(clearSpecialEffGold, effGroup.transform, false);
                            xianEff.transform.localScale = new Vector2(1, dis / 600);
                            xianEff.transform.localPosition = eff1.transform.localPosition;
                            xianEff.transform.localRotation = Quaternion.Euler(0, 0, angle);
                            
                            xianEff.SetActive(false);
                            StartCoroutine(Delay.Run(() =>
                            {
                                xianEff.SetActive(true);
                                xianEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "xian", true);
    
                                StartCoroutine(Delay.Run(() =>
                                {
                                    xianEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                    Destroy(xianEff);
                                }, 0.8f));
                                
                                var jizhongEff = Instantiate(clearSpecialEffGold, effGroup.transform, false);
                                jizhongEff.transform.localScale = Vector2.one;
                                jizhongEff.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                jizhongEff.transform.localPosition = dstPos;
                                jizhongEff.SetActive(false);
                                
                                var fadianEff = Instantiate(clearSpecialEffGold, effGroup.transform, false);
                                fadianEff.transform.localScale = Vector2.one;
                                fadianEff.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                fadianEff.transform.localPosition = dstPos;
                                fadianEff.SetActive(false);
                                
                                StartCoroutine(Delay.Run(() =>
                                {
                                    jizhongEff.SetActive(true);
                                    jizhongEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "jizhong_" + edgeItemLength, false);
                                    StartCoroutine(Delay.Run(() =>
                                        {
                                            jizhongEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                            Destroy(jizhongEff);
                                        }, 13 / 60f));
                                    
                                    
                                    fadianEff.SetActive(true);
                                    fadianEff.GetComponent<SkeletonGraphic>().AnimationState
                                        .AddAnimation(0, "y_fadian_" + edgeItemLength, true, 0f);
                                    StartCoroutine(Delay.Run(() =>
                                    {
                                        fadianEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                        Destroy(fadianEff);
                                    }, 1.2f));
                                }, 1 / 60f));
                            }, count * Constant.SpecialGoldClearIntervalTime));
    
                            if (_clearSpecialEffShanBaoPool == null)
                            {
                                _clearSpecialEffShanBaoPool = new ObjectPool();
                                _clearSpecialEffShanBaoPool.CreateObject(clearSpecialEffShanBao, 5);
                            }
                            var shanBaoEff = _clearSpecialEffShanBaoPool.Get();
                            shanBaoEff.transform.SetParent(effGroup.transform);
                            shanBaoEff.transform.localScale = shanBaoEffScale;
                            shanBaoEff.transform.localPosition = dstPos;
                            shanBaoEff.SetActive(false);
                            StartCoroutine(Delay.Run(() =>
                            {
                                shanBaoEff.SetActive(true);
                                shanBaoEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "shanbao", false);
                                StartCoroutine(Delay.Run(() =>
                                {
                                    shanBaoEff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                                    _clearSpecialEffShanBaoPool.Put(shanBaoEff);
                                }, 0.5f));
                            }, 0.5f + Constant.ClearWaitTime + Constant.SpecialGoldClearTime + edgeBlocks.Count * Constant.SpecialGoldClearIntervalTime));
                        }
                    }
                } else if (Constant.SceneVersion == "3")
                {
                    if (clearSpecialEffGold == null) return;
                    if (_clearSpecialEffGoldLineEff == null) return;
                    
                    if (goldItem == null || !Application.IsPlaying(goldItem) || !goldItem.activeInHierarchy) return;
                    
                    var goldItemLength = goldItem.GetComponent<BlockItem>().GetLength();
                    var eff1 = Instantiate(clearSpecialEffGold, goldItem.transform, false);
                    eff1.transform.localScale = Vector2.one;
                    eff1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    eff1.transform.localPosition = goldItem.transform.Find("img").transform.localPosition;
                    eff1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, goldItemLength.ToString(), true);

                    if (goldItem != null && goldItem.transform != null && goldItem.transform.Find("SpecialEffGold(Clone)") != null && goldItem.activeInHierarchy)
                    {
                        goldItem.transform.Find("SpecialEffGold(Clone)").transform.Find("fangdianEff").gameObject.SetActive(true);
                    }
                    
                    StartCoroutine(Delay.Run(() =>
                    {
                        eff1.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                        Destroy(eff1);
                    }, 0.5f + Constant.ClearWaitTime + Constant.SpecialGoldClearTime + edgeBlocks.Count * Constant.SpecialGoldClearIntervalTime));

                    if (goldItem == null || !Application.IsPlaying(goldItem) || !goldItem.activeInHierarchy) return;
                    
                    var goldItemPos = goldItem.transform.localPosition + goldItem.transform.Find("img").transform.localPosition;
                    if (edgeBlocks != null && edgeBlocks.Count > 0)
                    {
                        var count = 0;
                        foreach (var posIndex in edgeBlocks)
                        {
                            var edgeItem = Constant.GamePlayScript.GetBlockItemByPosIndex(posIndex);
                            if (edgeItem == null || !Application.IsPlaying(edgeItem) || !edgeItem.activeInHierarchy) continue;
                            var edgeItemLength = edgeItem.GetComponent<BlockItem>().GetLength();
                            var dstPos = edgeItem.transform.localPosition + edgeItem.transform.Find("img").transform.localPosition;
                            var dis = Vector2.Distance(goldItemPos, dstPos);
                            
                            var angle = Vector2.Angle(Vector2.up, new Vector2(dstPos.x - goldItemPos.x, dstPos.y - goldItemPos.y));
                            if (goldItemPos.x < dstPos.x)
                            {
                                angle = -angle;
                            }

                            angle += 90;

                            var xianEff = Instantiate(_clearSpecialEffGoldLineEff, effGroup.transform, false);
                            xianEff.transform.localScale = Vector2.one;
                            
                            xianEff.transform.localPosition = goldItemPos;
                            var line = xianEff.transform.Find("lineEff").transform;
                            line.localRotation = Quaternion.Euler(0, 0, angle);
                            line.localScale = new Vector2(dis / 540f, (dis / 540f > 1 ? dis / 540f : 1) * 0.8f);
                            line.localPosition = Vector2.zero;

                            xianEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.ScreenWidth * 2, dstPos.y - goldItemPos.y + 100);
                            
                            if (dstPos.y < goldItemPos.y)
                            {
                                xianEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.ScreenWidth * 2, goldItemPos.y - dstPos.y + 100);
                                xianEff.transform.localPosition = new Vector2(goldItemPos.x, goldItemPos.y - xianEff.GetComponent<RectTransform>().sizeDelta.y);
                                line.localPosition = new Vector2(0, xianEff.GetComponent<RectTransform>().sizeDelta.y);
                            }
                            
                            xianEff.SetActive(false);
                            if (_xianEffList == null)
                            {
                                _xianEffList = new List<GameObject>();
                            }
                            _xianEffList.Add(xianEff);

                            StartCoroutine(Delay.Run(() =>
                                {
                                    if (xianEff == null || !Application.IsPlaying(xianEff))
                                    {
                                        return;
                                    }
                                    
                                    xianEff.SetActive(true);
                                    line.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "shandian2", false);
                                    StartCoroutine(Delay.Run(() =>
                                        {
                                            if (edgeItem != null && Application.IsPlaying(edgeItem) && edgeItem.activeInHierarchy)
                                            {
                                                var tmpFaDianEff = Instantiate(clearSpecialEffGold, edgeItem.transform, false);
                                                tmpFaDianEff.transform.localScale = Vector2.one;
                                                tmpFaDianEff.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                                tmpFaDianEff.transform.localPosition = edgeItem.transform.Find("img").transform.localPosition;
                                                tmpFaDianEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, edgeItemLength.ToString(), true);
                                                tmpFaDianEff.transform.Find("fangdianEff").gameObject.SetActive(true);

                                                if (_fadianEffList == null)
                                                {
                                                    _fadianEffList = new List<GameObject>();
                                                }
                                                _fadianEffList.Add(tmpFaDianEff);

                                                edgeItem.transform.DOScale(new Vector3(1.05f, 1.05f, 1), 0.2f);
                                                edgeItem.transform.SetAsLastSibling();
                                                ShowBlockItemShakeEff(edgeItem);
                                            }
                                        }, 1 / 3f));
                                }, count * Constant.SpecialGoldClearIntervalTime));
                            
//                            StartCoroutine(Delay.Run(() =>
//                            {
//                                Destroy(xianEff);
//                                if (tmpFaDianEff != null)
//                                {
//                                    tmpFaDianEff.transform.Find("fangdianEff").gameObject.SetActive(false);
//                                }
//                            }, Constant.SpecialGoldClearTime + Constant.ClearWaitTime + edgeBlocks.Count * Constant.SpecialGoldClearIntervalTime));
                            
                            ++count;
                        }
                    }
                }
            }
        }

        private void ShowBlockItemShakeEff(GameObject gObj)
        {
            if (gObj == null || !gObj.activeInHierarchy) return;
            var oriPos = gObj.transform.localPosition;
            gObj.transform.DOBlendableLocalMoveBy(new Vector3(Tools.GetNumFromRange(-10, 10),
                Tools.GetNumFromRange(-10, 10), 0), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gObj.transform.DOLocalMove(oriPos, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    ShowBlockItemShakeEff(gObj);
                });
            });
        }

        public void RemoveClearSpecialEffGoldFaDianEff()
        {
            if (_fadianEffList != null && _fadianEffList.Count > 0)
            {
                foreach (var fadianEff in _fadianEffList)
                {
                    Destroy(fadianEff);
                }
                _fadianEffList.Clear();
            }

            if (_xianEffList != null && _xianEffList.Count > 0)
            {
                foreach (var xianEff in _xianEffList)
                {
                    Destroy(xianEff);
                }
                _xianEffList.Clear();
            }
        }

        public void ShowClearSpecialEffGoldOnly(GameObject goldItem)
        {
            var eff = Instantiate(clearSpecialEffGold, effGroup.transform, false);
            eff.transform.localScale = Vector2.one;
            eff.transform.localRotation = Quaternion.Euler(0, 0, 0);
            eff.transform.localPosition = goldItem.transform.localPosition +
                                           goldItem.transform.Find("img").transform.localPosition;
            eff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "y_fadian_" + goldItem.GetComponent<BlockItem>().GetLength(), true);
            StartCoroutine(Delay.Run(() =>
            {
                eff.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                Destroy(eff);
            }, 0.6f));

            var goldItemPos = goldItem.transform.localPosition;
            var gensuiEff = Instantiate(readyBgIceGenSuiEff, effGroup.transform, false);
            gensuiEff.transform.localPosition = goldItemPos + goldItem.transform.Find("img").transform.localPosition;
            gensuiEff.SetActive(false);
            
            StartCoroutine(Delay.Run(() =>
            {
                gensuiEff.SetActive(true);
                
                var offsetX = 600;
                var offsetY = (readyGroup.transform.localPosition.y - gensuiEff.transform.localPosition.y) / 3f;
                Vector3[] points;
                if (gensuiEff.transform.localPosition.x <= 0)
                {
                    points = new[]
                    {
                        new Vector3(0, readyGroup.transform.localPosition.y, 0),
                        new Vector3(goldItemPos.x + offsetX, goldItemPos.y + offsetY, 0),
                        new Vector3(goldItemPos.x + offsetX, goldItemPos.y + 2 * offsetY, 0),
                    };
                }
                else
                {
                    points = new[]
                    {
                        new Vector3(0, readyGroup.transform.localPosition.y, 0),
                        new Vector3(goldItemPos.x - offsetX, goldItemPos.y + offsetY, 0),
                        new Vector3(goldItemPos.x - offsetX, goldItemPos.y + 2 * offsetY, 0),
                    };
                }

                gensuiEff.transform.DOLocalPath(points, 0.6f, PathType.CubicBezier).OnComplete(() =>
                {
                    StartCoroutine(Delay.Run(() => { Destroy(gensuiEff); }, 1f));

                    if (Constant.GamePlayScript.IsSpecialGoldEffNoNewBlocks())
                    {
                        AddReadyIceEff();
                    }
                });
            }, 0.8f));
        }
        
        public async void ShowWindEff()
        {
            if (_windEffPool == null)
            {
                _windEffPool = new ObjectPool();
                var tmpEff = MaterPool.Instace.WindEff;
                _windEffPool.CreateObject(tmpEff);
            }
            
            var eff = _windEffPool.Get();
            eff.transform.SetParent(effGroup.transform);
            eff.transform.localScale = Vector2.one;
            eff.transform.localPosition = Vector2.zero;

            ManagerAudio.PlaySound("wind");
            StartCoroutine(Delay.Run(() =>
            {
                _windEffPool.Put(eff);
            }, 3.0f));
        }

        public async void ShowComboEff(int comboNum)
        {            
            await LoadResAsync_comboEffPool();
            
            StartCoroutine(Delay.Run(async () =>
            {
                if (_comboEffPool == null)
                {
                    return;
                }
                else if (!_comboEffPool.HaveObjectBase())
                {
                    while (!_comboEffPool.HaveObjectBase())
                    {
                        await Task.Delay(100);
                    }
                }
                
                var frames = comboEffFrame;
                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    frames = comboEffFrame1;
                } else if (Constant.SceneVersion == "3")
                {
                    frames = comboEffFrame3;
                }

                var eff = _comboEffPool.Get();
                eff.transform.SetParent(effGroup.transform);
                eff.transform.localScale = Vector2.one;
                eff.transform.localPosition = Constant.SceneVersion == "3" ? new Vector2(5, 100) : new Vector2(0, 100);
                
                if (Constant.SceneVersion == "3")
                {
                    eff.transform.GetComponent<Animator>().Play("comboEff1", 0, 0);

                    var num1 = eff.transform.Find("text").transform.Find("num1");
                    var num2 = eff.transform.Find("text").transform.Find("num2");
                    var bg = eff.transform.Find("text").transform.Find("bg");
                    
                    if (comboNum >= 10)
                    {
                        num2.gameObject.SetActive(true);
                        num1.GetComponent<Image>().sprite = frames[comboNum / 10];
                        num2.GetComponent<Image>().sprite = frames[comboNum % 10];

                        bg.transform.localPosition = new Vector2(25, 0);
                        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(640, 200);
                    }
                    else
                    {
                        num2.gameObject.SetActive(false);
                        num1.GetComponent<Image>().sprite = frames[comboNum];
                        
                        bg.transform.localPosition = Vector2.zero;
                        bg.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 200);
                    }
                    
                    StartCoroutine(Delay.Run(() => { _comboEffPool.Put(eff); }, 1.5f));
                }
                else
                {
                    eff.transform.localScale = Vector2.zero;
                    eff.GetComponent<CanvasGroup>().alpha = 1;
                    if (comboNum >= 10)
                    {
                        eff.transform.Find("label_num2").gameObject.SetActive(true);
                        eff.transform.Find("label_num").GetComponent<Image>().sprite = frames[comboNum / 10];
                        eff.transform.Find("label_num2").GetComponent<Image>().sprite = frames[comboNum % 10];
                    }
                    else
                    {
                        eff.transform.Find("label_num2").gameObject.SetActive(false);
                        eff.transform.Find("label_num").GetComponent<Image>().sprite = frames[comboNum];
                    }

                    eff.transform.DOScale(Vector2.one, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        eff.transform.GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetDelay(0.5f);
                        eff.transform.DOScale(new Vector2(1.5f, 1.5f), 0.31f).SetDelay(0.5f).OnComplete(() =>
                        {
                            _comboEffPool.Put(eff);
                        });
                    });
                }

                if (Constant.VibratorSwitch && comboNum >= 2)
                {
                    Tools.DoVibrator(Constant.VibratorTime, Constant.VibratorAmplitude);
                }
            }, 0.5f));
        }
        
        public async void ShowScoreEff(int score, int iceBlockNum = 0)
        {
            await LoadResAsync_scoreEffPool();
            
            StartCoroutine(Delay.Run(async () =>
            {
                if (iceBlockNum > 0 && Constant.SceneVersion == "2")
                {
                    if (_scoreIceEffPool == null)
                    {
                        return;
                    } else if (!_scoreIceEffPool.HaveObjectBase())
                    {
                        while (!_scoreIceEffPool.HaveObjectBase())
                        {
                            await Task.Delay(100);
                        }
                    }
                    
                    var eff = _scoreIceEffPool.Get();
                    eff.transform.SetParent(effGroup.transform);
                    eff.transform.localScale = Vector2.zero;
                    eff.transform.localPosition = Vector2.zero;
                    eff.GetComponent<CanvasGroup>().alpha = 1;
                    eff.GetComponent<TextMeshProUGUI>().text = score + " x " + (int)Math.Pow(2, iceBlockNum);
    
                    eff.transform.DOScale(Vector2.one, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        eff.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(0.2f).OnComplete(() =>
                        {
                            _scoreIceEffPool.Put(eff);
                        });
                    }).SetDelay(Constant.ScoreEffDelayTime);
                }
                else
                {
                    var numWidth = 94;
                    var numHeight = 110;
                    var lightWidthOffset = 100;
                    var lightHeightOffset = 130;
                    
                    if (_scoreEffPool == null)
                    {
                        return;
                    } else if (!_scoreEffPool.HaveObjectBase())
                    {
                        while (!_scoreEffPool.HaveObjectBase())
                        {
                            await Task.Delay(100);
                        }
                    }
                    
                    var eff = _scoreEffPool.Get();
                    eff.transform.SetParent(effGroup.transform);
                    eff.transform.localScale = new Vector2(0.8f, 0.8f);
                    eff.transform.localPosition = Vector2.zero;
    
                    if (Constant.SceneVersion == "3")
                    {
                        var numBg = eff.transform.Find("text");
                        var numPrefab = numBg.transform.Find("num");
                        var numGroup = numBg.transform.Find("numGroup");
                        var childCount = numGroup.transform.childCount;
                        for (var i = childCount - 1; i >= 0; i--)
                        {
                            Destroy(numGroup.transform.GetChild(i).gameObject);
                        }

                        var scoreStr = score.ToString();
                        numBg.GetComponent<RectTransform>().sizeDelta = new Vector2(numWidth * scoreStr.Length + lightWidthOffset, numHeight + lightHeightOffset);
                        numGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(numWidth * scoreStr.Length, numHeight + lightHeightOffset);
                        foreach (var t in scoreStr)
                        {
                            var numObj = Instantiate(numPrefab, numGroup.transform, false);
                            numObj.gameObject.SetActive(true);
                            numObj.transform.localScale = Vector2.one;
                            numObj.transform.localPosition = Vector2.zero;
                            numObj.GetComponent<Image>().sprite = scoreEffSpriteFrames[int.Parse(t.ToString())];
                        }

                        if (Constant.SceneVersion == "3" && iceBlockNum > 0)
                        {
                            var spaceLen = 20;
                            var xNumStr = ((int) Math.Pow(2, iceBlockNum)).ToString();
                            
                            numBg.GetComponent<RectTransform>().sizeDelta = new Vector2(numWidth * (scoreStr.Length + xNumStr.Length) + spaceLen * 2 + lightWidthOffset, numHeight + lightHeightOffset);
                            numGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(numWidth * (scoreStr.Length + xNumStr.Length + 1) + spaceLen * 2, numHeight + lightHeightOffset);
                            
                            for (var i = 0; i < 3; ++i)
                            {
                                var xObj = Instantiate(numPrefab, numGroup.transform, false);
                                xObj.gameObject.SetActive(true);
                                xObj.transform.localScale = Vector2.one;
                                xObj.transform.localPosition = Vector2.zero;
                                
                                if (i != 1)
                                {
                                    xObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                                    xObj.GetComponent<RectTransform>().sizeDelta = new Vector2(spaceLen, 100);
                                }
                                else
                                {
                                    xObj.GetComponent<Image>().sprite = scoreEffSpriteFrames[scoreEffSpriteFrames.Length - 1];
                                    xObj.GetComponent<Image>().SetNativeSize();
                                }
                            }

                            foreach (var t in xNumStr)
                            {
                                var numObj = Instantiate(numPrefab, numGroup.transform, false);
                                numObj.gameObject.SetActive(true);
                                numObj.transform.localScale = Vector2.one;
                                numObj.transform.localPosition = Vector2.zero;
                                numObj.GetComponent<Image>().sprite = scoreEffSpriteFrames[int.Parse(t.ToString())];
                            }
                        }
                        
                        numPrefab.gameObject.SetActive(false);

                        eff.transform.GetComponent<Animator>().Play("scoreEff1", 0, 0);
                        StartCoroutine(Delay.Run(() =>
                        {
                            _scoreEffPool.Put(eff);
                        }, 1.5f));
                    }
                    else
                    {
                        eff.transform.localScale = Vector2.zero;
                        eff.GetComponent<CanvasGroup>().alpha = 1;
                        eff.GetComponent<TextMeshProUGUI>().text = score.ToString();
    
                        eff.transform.DOScale(Vector2.one, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
                        {
                            eff.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(0.2f).OnComplete(() =>
                            {
                                _scoreEffPool.Put(eff);
                            });
                        }).SetDelay(Constant.ScoreEffDelayTime);
                    }
                }
            }, 0.3f));
        }

        public async void ShowDeadWarning()
        {
            if (Blocks.IsTesting()) return;
            
            if (_deadWarningEff != null && _deadWarningEff.activeInHierarchy) return;
            if (_deadWarningEff == null)
            {
                if (Constant.SceneVersion == "3")
                {
                    var tmpPrefab = MaterPool.Instace.DeadWarning;
                    _deadWarningEff = Instantiate(tmpPrefab, effGroup.transform, false);
                    _deadWarningEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * Constant.Lie + 37, Constant.BlockHeight * Constant.Hang + 37);
                }
                else
                {
                    _deadWarningEff = Instantiate(deadWarningEff, effGroup.transform, true);
                    _deadWarningEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * Constant.Lie + 5, Constant.BlockHeight * Constant.Hang + 5);
                }
                
                _deadWarningEff.transform.localScale = Vector2.one;
                _deadWarningEff.transform.localPosition = new Vector2(0, 10);
            }
            
            _deadWarningEff.SetActive(true);
            var deadWarningEffImg = _deadWarningEff.GetComponent<Image>();
            var tmpColor = deadWarningEffImg.color;
            tmpColor.a = 0;
            deadWarningEffImg.color = tmpColor;
            deadWarningEffImg.DOFade(1, 1).OnComplete(() =>
            {
                var seq = DOTween.Sequence();
                seq.Append(deadWarningEffImg.DOFade(0.5f, 1));
                seq.Append(deadWarningEffImg.DOFade(1, 1));
                seq.SetLoops(-1);
                seq.SetId("deadWarning");
                seq.SetUpdate(true);
            });
        }

        public void RemoveDeadWarning()
        {
            if (_deadWarningEff == null || !_deadWarningEff.activeInHierarchy) return;
            DOTween.Kill("deadWarning");
            _deadWarningEff.SetActive(false);
        }

        public bool IsShowingDeadWarning()
        {
            return _deadWarningEff != null && _deadWarningEff.activeInHierarchy;
        }

        public void ShowBlockItemGrayEff(GameObject item)
        {
            if (_blockItemGrayEffPool == null)
            {
                _blockItemGrayEffPool = new ObjectPool();
                _blockItemGrayEffPool.CreateObject(blockItemGrayEff, 20);
            }
            
            var eff = _blockItemGrayEffPool.Get();
            eff.transform.SetParent(item.transform);
            eff.transform.localScale = Vector2.one;
            
            if (Constant.SceneVersion == "2")
            {
                eff.transform.GetComponent<Image>().sprite = item.GetComponent<BlockItem>().GetSprite();
                
            } else if (Constant.SceneVersion == "3")
            {
                eff.transform.GetComponent<Image>().sprite = specialEffStoneSpriteFrames[item.GetComponent<BlockItem>().GetLength() - 1];
            }
            
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            eff.GetComponent<RectTransform>().sizeDelta =
                item.GetComponent<BlockItem>().GetComponent<RectTransform>().sizeDelta;

            var effImg = eff.GetComponent<Image>();
            var tmpColor = effImg.color;
            tmpColor.a = 0;
            effImg.color = tmpColor;
            effImg.DOFade(1, 0.3f);
        }

        public void RemoveBlockItemGrayEff(GameObject item)
        {
            if (item.transform.Find("BlockItemGrayEff(Clone)") != null)
            {
                _blockItemGrayEffPool.Put(item.transform.Find("BlockItemGrayEff(Clone)").gameObject);
            }
        }

        public void ShowClearTip(GameObject item, int[] tipData)
        {
            if (item == null) return;
            
            if (_clearTipEff == null)
            {
                _clearTipEff = Instantiate(clearTipEff, item.transform, false);
            }
            else
            {
                _clearTipEff.transform.SetParent(item.transform);
                _clearTipEff.SetActive(true);
            }

            _clearTipEff.transform.localScale = new Vector2(tipData[0] > 0 ? 1 : -1, 1);
            _clearTipEff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            _clearTipEff.GetComponent<Image>().sprite = item.GetComponent<BlockItem>().IsSpecial() ? clearTipEffFrame[0] : clearTipEffFrame[item.GetComponent<BlockItem>().GetColor()];

            clearTipEff2.SetActive(true);
            var itemSize = item.GetComponent<RectTransform>().sizeDelta;
            clearTipEff2.transform.localScale = Vector2.one;
            clearTipEff2.transform.localPosition = item.transform.localPosition + new Vector3(itemSize.x / 2, itemSize.y / 2, 0) + new Vector3(tipData[0] * Constant.BlockWidth, 0, 0);
            clearTipEff2.GetComponent<RectTransform>().sizeDelta = itemSize;
            
            var oriX = item.transform.localPosition.x;
            var dstX = oriX + Math.Abs(tipData[0]) / tipData[0] * 20;
            var seq1 = DOTween.Sequence();
            var seq2 = DOTween.Sequence();
            seq1.Append(item.transform.DOLocalMoveX(dstX, 0.2f).SetDelay(0.05f));
            seq1.Append(item.transform.DOLocalMoveX(oriX, 0.2f).SetDelay(0.05f));
            seq1.SetLoops(3);
            seq2.Append(seq1);
            seq2.Append(item.transform.DOLocalMoveX(oriX, 1.5f));
            seq2.SetLoops(-1);
            seq2.SetId("clearTipEff");
            seq1.SetUpdate(true);
            seq2.SetUpdate(true);
        }

        public void HideClearTip()
        {
            if (!IsShowingClearTip()) return;
            DOTween.Kill("clearTipEff");

            var blockItem = _clearTipEff.transform.parent;
            if (blockItem != null)
            {
                var posIndex = blockItem.GetComponent<BlockItem>().GetPosIndex();
                var localPositionY = blockItem.transform.localPosition.y;
                blockItem.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + Blocks.GetLieByPos(posIndex) * Constant.BlockWidth, localPositionY); 
            }
            
            _clearTipEff.SetActive(false);
            _clearTipEff.transform.SetParent(null);
            
            clearTipEff2.SetActive(false);
        }

        public bool IsShowingClearTip()
        {
            return (_clearTipEff != null && _clearTipEff.activeInHierarchy) || (clearTipEff2 != null && clearTipEff2.activeInHierarchy);
        }

        public void ShowGuideStepEff()
        {
            var guideStepData = Player.GetGuideStepData(Player.GetGuideStep());
            if (guideStepData != null)
            {
                if (_guideEff == null)
                {
                    _guideEff = Instantiate(guideEff, effGroup.transform, false);
                }
                _guideEff.SetActive(true);
                _guideEff.GetComponent<GuideEff>().UpdateGuidePos(guideStepData);
            }
        }

        public void RemoveGuideStepEff(bool destroy = false)
        {
            if (_guideEff != null)
            {
                if (_guideEff.activeInHierarchy)
                {
                    _guideEff.SetActive(false);
                }

                if (destroy)
                {
                    Destroy(_guideEff);
                }
            }
        }

        public async void ShowClearBlockEff(GameObject item, bool showScoreEff = true)
        {
            await LoadResAsync_scoreEffPool();
            
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            var itemColor = item.GetComponent<BlockItem>().GetColor();
            var itemIsSpecial = item.GetComponent<BlockItem>().IsSpecial();
            var itemSpecialType = item.GetComponent<BlockItem>().GetSpecial();
            var itemPos = item.transform.localPosition;
            var itemCenterPos = itemPos + item.transform.Find("img").transform.localPosition;

            if (itemIsSpecial && item.GetComponent<BlockItem>().GetSpecial() == (int) Blocks.Special.Stone)
            {
                ++Constant.GameStatusData.StoneCountClear;
            }
            
            StartCoroutine(Delay.Run(async () =>
            {
                for (var i = 0; i < itemLength; ++i)
                {
                    var eff = _clearBlockEffPool.Get();
                    eff.transform.SetParent(effGroup.transform, false);
                    eff.transform.localPosition = itemPos + new Vector3((i + 0.5f) * Constant.BlockWidth, Constant.BlockHeight / 2, 0);

                    if (Constant.SceneVersion == "3")
                    {
                        eff.transform.Find("xx").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffSpriteFramesStar[itemColor - 1];
                        eff.transform.Find("k").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffSpriteFramesYuan[itemColor - 1];
                        eff.transform.Find("k2").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffSpriteFramesShu[itemColor - 1];
                        eff.transform.Find("boom").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffSpriteFramesFour[itemColor - 1];
                        eff.transform.Find("boom1").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffSpriteFramesFour[itemColor - 1];
                    }
                    else
                    {
                        if (_clearBlockEffColors == null)
                        {
                            _clearBlockEffColors = new Color[Enum.GetNames(typeof(Blocks.Color)).Length + 1];
                            _clearBlockEffColors[0] = new Color(1, 1, 1, 100 / 255f);
                            _clearBlockEffColors[1] = new Color(59 / 255f, 75 / 255f, 255 / 255f, 100 / 255f);
                            _clearBlockEffColors[2] = new Color(145 / 255f, 252 / 255f, 131 / 255f, 100 / 255f);
                            _clearBlockEffColors[3] = new Color(216 / 255f, 61 / 255f, 255 / 255f, 100 / 255f);
                            _clearBlockEffColors[4] = new Color(253 / 255f, 49 / 255f, 49 / 255f, 100 / 255f);
                            _clearBlockEffColors[5] = new Color(255 / 255f, 206 / 255f, 78 / 255f, 100 / 255f);
                        }
                        
                        if (itemIsSpecial)
                        {
                            if (itemSpecialType == (int) Blocks.Special.Stone)
                            {
                                eff.transform.Find("eff1").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffStoneBig;
                                eff.transform.Find("eff2").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffStoneLittle;
                                eff.transform.Find("eff3").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[0]; 
                                eff.transform.Find("eff4").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[0]; 
                            }
                            else
                            {
                                eff.transform.Find("eff1").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffSpriteFramesBig[0];
                                eff.transform.Find("eff2").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffSpriteFramesLittle[0];
                                eff.transform.Find("eff3").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[0]; 
                                eff.transform.Find("eff4").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[0]; 
                            }   
                        }
                        else
                        {
                            eff.transform.Find("eff1").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffSpriteFramesBig[itemColor];
                            eff.transform.Find("eff2").GetComponent<ParticleSystemRenderer>().material.mainTexture = clearBlockEffSpriteFramesLittle[itemColor];
                            eff.transform.Find("eff3").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[itemColor]; 
                            eff.transform.Find("eff4").GetComponent<ParticleSystem>().startColor = _clearBlockEffColors[itemColor]; 
                        }
                    }
                
                    StartCoroutine(Delay.Run(() =>
                    {
                        if (eff != null)
                        {
                            _clearBlockEffPool.Put(eff);
                        }
                    }, 5));
                }

                if (Constant.SceneVersion != "3" && showScoreEff)
                {
                    GameObject sEff;
                    if (Constant.SceneVersion == "3")
                    {
                        sEff = itemLength > 2 ? _blockScoreEff1Pool.Get() : _blockScoreEffPool.Get();
                    }
                    else
                    {
                        if (_scoreEffPool == null)
                        {
                            return;
                        } else if (!_scoreEffPool.HaveObjectBase())
                        {
                            while (!_scoreEffPool.HaveObjectBase())
                            {
                                await Task.Delay(100);
                            }
                        }

                        if (_scoreIceEffPool == null)
                        {
                            return;
                        } else if (!_scoreIceEffPool.HaveObjectBase())
                        {
                            while (!_scoreIceEffPool.HaveObjectBase())
                            {
                                await Task.Delay(100);
                            }
                        }
                        
                        sEff = itemLength > 2 ? _scoreIceEffPool.Get() : _scoreEffPool.Get();
                    }
                    var score = itemLength * Constant.GamePlayScript.GetUnitBlockScore();
                    if (itemIsSpecial)
                    {
                        score *= 2;
                    }
                    
                    sEff.transform.SetParent(effGroup.transform);
                    sEff.transform.localScale = Vector2.zero;
                    sEff.transform.localPosition = itemCenterPos;
                    sEff.GetComponent<CanvasGroup>().alpha = 1;
                    sEff.GetComponent<TextMeshProUGUI>().text = "+" + score;

                    if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                    {
                        sEff.GetComponent<TextMeshProUGUI>().text = score.ToString();
                    }

                    sEff.transform.DOScale(new Vector2(0.4f, 0.4f), 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        sEff.GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetDelay(0.2f).OnComplete(() =>
                        {
                            if (sEff != null)
                            {
                                if (Constant.SceneVersion == "3")
                                {
                                    if (itemLength > 2)
                                    {
                                        _blockScoreEff1Pool.Put(sEff);
                                    }
                                    else
                                    {
                                        _blockScoreEffPool.Put(sEff);
                                    }
                                }
                                else
                                {
                                    if (itemLength > 2)
                                    {
                                        _scoreIceEffPool.Put(sEff);
                                    }
                                    else
                                    {
                                        _scoreEffPool.Put(sEff);
                                    }
                                }
                            }
                        });
                    });
                }
            }, Constant.ClearEffDelayTime));
        }

        public void ShowClearIceEff(GameObject item)
        {            
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            if (Constant.SceneVersion == "2")
            {
                StartCoroutine(Delay.Run(() =>
                {
                    for (var i = 0; i < itemLength; ++i)
                    {
                        var eff = _clearBlockEffPool.Get();
                        eff.transform.SetParent(effGroup.transform);
                        eff.transform.localPosition = item.transform.localPosition + new Vector3((i + 0.5f) * Constant.BlockWidth, Constant.BlockHeight / 2, 0);

                        eff.transform.Find("eff1").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffIceBig;
                        eff.transform.Find("eff2").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffIceLittle;
                
                        StartCoroutine(Delay.Run(() =>
                        {
                            _clearBlockEffPool.Put(eff);
                        }, 1));
                    } 
                }, Constant.ClearEffDelayTime));
            } else if (Constant.SceneVersion == "3")
            {
                item.transform.DOKill();
                var posIndex = item.GetComponent<BlockItem>().GetPosIndex();
                item.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + Constant.BlockWidth * Blocks.GetLieByPos(posIndex), Constant.BlockGroupEdgeBottom + Constant.BlockHeight * Blocks.GetHangByPos(posIndex));
                item.transform.localScale = Vector2.one;
                
                StartCoroutine(Delay.Run(() =>
                {
                    for (var i = 0; i < itemLength; ++i)
                    {
                        var eff = _clearBlockEffPool.Get();
                        eff.transform.SetParent(effGroup.transform, false);
                        eff.transform.localPosition = item.transform.localPosition + new Vector3((i + 0.5f) * Constant.BlockWidth, Constant.BlockHeight / 2, 0);
                        
                        eff.transform.Find("xx").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            _clearIceEffSpriteFrames[3];
                        eff.transform.Find("k").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            _clearIceEffSpriteFrames[1];
                        eff.transform.Find("k2").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            _clearIceEffSpriteFrames[2];
                        eff.transform.Find("boom").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            _clearIceEffSpriteFrames[0];
                        eff.transform.Find("boom1").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            _clearIceEffSpriteFrames[0];
                
                        StartCoroutine(Delay.Run(() =>
                        {
                            _clearBlockEffPool.Put(eff);
                        }, 1));
                    } 
                }, Constant.ClearEffDelayTime));
            }
        }

        public void AddB421Eff(GameObject item)
        {
            if (_b421Eff != null && _b421Eff.activeInHierarchy) return;
            if (_b421Eff == null)
            {
                _b421Eff = Instantiate(b421Eff, null, false);
            }
            
            var itemLength = item.GetComponent<BlockItem>().GetLength();
            var itemColor = item.GetComponent<BlockItem>().GetColor();
            var isSpecial = item.GetComponent<BlockItem>().IsSpecial();
            _b421Eff.SetActive(true);
            _b421Eff.transform.SetParent(item.transform);
            _b421Eff.transform.localScale = Vector2.one;
            _b421Eff.transform.localPosition = new Vector2(itemLength / 2f * Constant.BlockWidth, Constant.BlockHeight / 2f);
            
            _b421Eff.GetComponent<Image>().sprite = isSpecial ? b421EffImgs[0] : b421EffImgs[itemColor];

            var oriX = _b421Eff.transform.localPosition.x;
            var dstX = oriX + 10;
            var seq1 = DOTween.Sequence();
            var seq2 = DOTween.Sequence();
            seq1.Append(_b421Eff.transform.DOLocalMoveX(dstX, 0.2f).SetDelay(0.05f));
            seq1.Append(_b421Eff.transform.DOLocalMoveX(oriX, 0.2f).SetDelay(0.05f));
            seq1.SetLoops(3);
            seq2.Append(seq1);
            seq2.Append(_b421Eff.transform.DOLocalMoveX(oriX, 1.5f));
            seq2.SetLoops(-1);
            seq2.SetId("b421Eff");
            seq1.SetUpdate(true);
            seq2.SetUpdate(true);
        }

        public void RemoveB421Eff()
        {
            if (IsB421EffShowing())
            {
                DOTween.Kill("b421Eff");
                _b421Eff.transform.SetParent(null);
                _b421Eff.SetActive(false);
            }
        }

        public bool IsB421EffShowing()
        {
            return _b421Eff != null && _b421Eff.activeInHierarchy;
        }

        public void AddReadyBgIceEff(GameObject readyBg, bool showAnim = true)
        {
            if (_readyBgIceEff == null)
            {
                _readyBgIceEff = Instantiate(readyBgIceEff, readyBg.transform, false);
            }
            
            _readyBgIceEff.SetActive(true);
            _readyBgIceEff.GetComponent<CanvasGroup>().alpha = 0;
            _readyBgIceEff.GetComponent<CanvasGroup>().DOFade(1, Constant.IceTime);
            
            if (showAnim)
            {
                if (_toIceEffPool == null)
                {
                    _toIceEffPool = new ObjectPool();
                    _toIceEffPool.CreateObject(toIceEff, 2);
                }
                
                ManagerAudio.PlaySound("frozen");
                for (var i = 0; i < 4; ++i)
                {
                    var eff = _toIceEffPool.Get();
                    eff.transform.SetParent(readyBg.transform);
                    eff.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + ((i + 1) * 2 - 1) * Constant.BlockWidth, 0);
                    StartCoroutine(Delay.Run(() =>
                    {
                        _toIceEffPool.Put(eff);
                    }, 2f));
                }
            }
        }

        public void RemoveReadyBgIceEff(GameObject readyBg, bool showAnim = true)
        {
            if (_readyBgIceEff != null && _readyBgIceEff.activeInHierarchy)
            {
                _readyBgIceEff.GetComponent<CanvasGroup>().DOFade(0, Constant.BlockRemoveTime).OnComplete(() =>
                {
                    _readyBgIceEff.SetActive(false);
                });

                if (showAnim)
                {                    
                    for (var i = 0; i < Constant.Lie; ++i)
                    {
                        var eff = _clearBlockEffPool.Get();
                        eff.transform.SetParent(readyBg.transform);
                        eff.transform.localPosition = new Vector2(Constant.BlockGroupEdgeLeft + (i + 0.5f) * Constant.BlockWidth, 0);

                        eff.transform.Find("eff1").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffIceBig;
                        eff.transform.Find("eff2").GetComponent<ParticleSystemRenderer>().material.mainTexture =
                            clearBlockEffIceLittle;
                
                        StartCoroutine(Delay.Run(() =>
                        {
                            _clearBlockEffPool.Put(eff);
                        }, 1));
                    
                    }
                }
            }
        }

        public void AddBlackMaskEff()
        {
            if (_blackMaskEff == null)
            {
                _blackMaskEff = Instantiate(blackMaskEff, effGroup.transform, false);
                _blackMaskEff.transform.localScale = Vector2.one;
                _blackMaskEff.transform.localPosition = Vector2.zero;
                _blackMaskEff.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.ScreenWidth, Constant.ScreenHeight);
            }

            _blackMaskEff.SetActive(true);
            _blackMaskEff.GetComponent<CanvasGroup>().alpha = 0;
            _blackMaskEff.GetComponent<CanvasGroup>().DOFade(1, 0.1f);
        }

        public void RemoveBlackMaskEff()
        {
            if (_blackMaskEff != null && _blackMaskEff.activeInHierarchy)
            {
                _blackMaskEff.SetActive(false);
            }
        }

        public void AddHighLightEff(GameObject item)
        {
            if (_clearSpecialEdgeEffPool == null)
            {
                _clearSpecialEdgeEffPool = new ObjectPool();
                _clearSpecialEdgeEffPool.CreateObject(clearSpecialEdgeEff, 3);
            }
            
            var eff = _clearSpecialEdgeEffPool.Get();
            eff.transform.SetParent(item.transform);
            eff.transform.localScale = Vector2.one;
            eff.transform.localPosition = item.transform.Find("img").transform.localPosition;
            eff.GetComponent<RectTransform>().sizeDelta =
                item.GetComponent<RectTransform>().sizeDelta;
            
            var effImg = eff.GetComponent<Image>();
            effImg.sprite = item.GetComponent<BlockItem>().GetSprite();
            var tmpColor = eff.GetComponent<Image>().color;
            tmpColor.a = 0;
            effImg.color = tmpColor;

            effImg.DOFade(0.5f, 0.2f);
        }

        public void RemoveHighLightEff(GameObject item)
        {
            if (item.transform.Find("ClearSpecialEdgeEff(Clone)") != null)
            {
                _clearSpecialEdgeEffPool.Put(item.transform.Find("ClearSpecialEdgeEff(Clone)").gameObject);
            }
        }

        public void ShowShakeEff()
        {
            var shakeTime = 1 / 30f;
            var seq = DOTween.Sequence();
            seq.Append(gameObject.transform.DOLocalRotate(new Vector3(0, 0, -1), shakeTime / 2));
            seq.Append(gameObject.transform.DOLocalRotate(new Vector3(0, 0, 1), shakeTime));
            seq.Append(gameObject.transform.DOLocalRotate(new Vector3(0, 0, -1), shakeTime));
            seq.Append(gameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), shakeTime / 2));
            seq.SetUpdate(true);
        }

        public async void ShowIceTipEff(GameObject item)
        {
            var tmpIceTipEff = MaterPool.Instace.IceTip;
            var tip = Instantiate(tmpIceTipEff, effGroup.transform, false);
            tip.GetComponent<IceTip>().UpdateTip(item);
        }

        public void ShowStoneTipEff(GameObject item)
        {
            var tip = Instantiate(iceTipEff, effGroup.transform, false);
            tip.GetComponent<IceTip>().UpdateTip(item);
            tip.GetComponent<IceTip>().UpdateText("The stone is too heavy to move, let it fall to clear");
        }

        public void ShowReadyCountDownEff(int num)
        {
            if (_readyBgCountDownEffPool == null)
            {
                _readyBgCountDownEffPool = new ObjectPool();
                _readyBgCountDownEffPool.CreateObject(readyBgCountDownEff, 2);
            }
            
            var eff = _readyBgCountDownEffPool.Get();
            eff.transform.SetParent(readyGroup.transform);
            eff.GetComponent<TextMeshProUGUI>().text = num.ToString();
            eff.transform.localPosition = new Vector2(0, -4);
            eff.transform.localScale = new Vector2(2, 2);
            eff.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            eff.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                eff.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetDelay(0.5f).OnComplete(() =>
                {
                    _readyBgCountDownEffPool.Put(eff);
                });
            });
        }

        public void AddReadyIceEff()
        {
//            Time.timeScale = 0.1f;
            
            var eff = Instantiate(readyBgIceStartEff, readyGroup.transform, false);
            eff.transform.localPosition = new Vector3(0, -4, 0);
            eff.name = "readyIceStartEff";
            
            var ice = eff.transform.Find("ice");
            ice.GetComponent<CanvasGroup>().DOFade(1, 2);
            
            ManagerAudio.PlaySound("frozen");
        }

        public void RemoveReadyIceEff()
        {
            if (readyGroup.transform.Find("readyIceStartEff") != null)
            {
                Destroy(readyGroup.transform.Find("readyIceStartEff").gameObject);
            }

            var eff = Instantiate(readyBgIceClearEff, readyGroup.transform, false);
            eff.transform.localPosition = new Vector3(0, -4, 0);
            
            var ice = eff.transform.Find("ice");
            ice.GetComponent<CanvasGroup>().DOFade(0, 0.2f);
            
            var iceLight = eff.transform.Find("iceLight");
            iceLight.GetComponent<CanvasGroup>().DOFade(0, 0.2f);

            StartCoroutine(Delay.Run(() => { Destroy(eff); }, 2));
            
            ManagerAudio.PlaySound("clear_1");
        }

        public void ShowBlockShakeEff(GameObject item)
        {
            var tweenId = "itemShake" + item.GetComponent<BlockItem>().GetPosIndex();
            
            if (DOTween.TweensById(tweenId) != null && DOTween.TweensById(tweenId).Count > 0) return;
            
            item.transform.SetAsLastSibling();
            var time = 0.04f;
            var offsetX = 10;
            var repeatCount = 2;
            var originalX = item.transform.localPosition.x;
            var seq = DOTween.Sequence();
            seq.Append(item.transform.DOLocalMoveX(originalX - offsetX / 2f, time));

            for (int i = 0; i < repeatCount; i++)
            {
                seq.Append(item.transform.DOLocalMoveX(originalX + offsetX, time));
                seq.Append(item.transform.DOLocalMoveX(originalX- offsetX, time));
            }

            seq.Append(item.transform.DOLocalMoveX(originalX, time));
            seq.SetId(tweenId);
            seq.SetUpdate(true);
        }

        private void ShowBoardFrameEff()
        {
            if (_boardFrameEff == null)
            {
                _boardFrameEff = Instantiate(boardFrameEff, effGroup.transform, false);
                _boardFrameEff.transform.localScale = Vector2.one;

                if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
                {
                    _boardFrameEff.transform.localScale = new Vector2(115 / 120f, 113 / 120f);
                }
                
                _boardFrameEff.SetActive(false);
            }

            if (_boardFrameEff.activeInHierarchy) return;

            _boardFrameEff.SetActive(true);
            _boardFrameEff.transform.SetAsLastSibling();
            
            _boardFrameEff.transform.localPosition = new Vector2(0, 22);
            _boardFrameEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "newAnimation", false);
            StartCoroutine(Delay.Run(() =>
            {
                _boardFrameEff.transform.Find("zhashan1").gameObject.SetActive(true);

                StartCoroutine(Delay.Run(() =>
                {
                    _boardFrameEff.transform.Find("zhashan1").gameObject.SetActive(false);
                    _boardFrameEff.SetActive(false);
                }, 1));
            }, 0.2f));
        }

        public void ShowClearToLevelEff(int hangNum)
        {
//            if (_clearToLevelEffPool == null)
//            {
//                _clearToLevelEffPool = new ObjectPool();
//                _clearToLevelEffPool.CreateObject(clearToLevelEff, 1);
//            }
//            
//            var eff = _clearToLevelEffPool.Get();
//            eff.transform.SetParent(effGroup.transform, false);
//            eff.transform.localScale = Vector2.one;
//            eff.transform.localPosition = new Vector2(0, Constant.BlockGroupEdgeBottom + Constant.BlockHeight * (0.5f + hangNum));
//
//            var startPos = eff.transform.localPosition;
//            var levelBarPos = Constant.GamePlayScript.scoreGroup.GetComponent<ScoreGroup>().barProgress.transform
//                .InverseTransformPoint(Vector2.zero) * -1;
//            var levelBarWidth = Constant.GamePlayScript.scoreGroup.GetComponent<ScoreGroup>().barProgress
//                .GetComponent<RectTransform>().sizeDelta.x;
//
//            var x = Tools.GetNumFromRange(500, 800);
//            var y = Tools.GetNumFromRange(3, 4);
//            var offsetX = Tools.GetNumFromRange(0, 1) == 0 ? -x : x;
//            var offsetY = (levelBarPos.y + startPos.y) / (y * 1f);
//            
//            var points = new[]
//            {
//                new Vector3(levelBarPos.x + levelBarWidth, levelBarPos.y, 0),
//                new Vector3(offsetX, offsetY * (y - 2), 0),
//                new Vector3(0, offsetY * (y - 1), 0),
//            };
//
//            eff.transform.DOLocalPath(points, 0.6f, PathType.CubicBezier).OnComplete(() =>
//                {
//                    StartCoroutine(Delay.Run(() => { _clearToLevelEffPool.Put(eff); }, 0.3f));
//                    
//                    if (clearToLevelEffEnd != null)
//                    {
//                        if (!clearToLevelEffEnd.activeInHierarchy)
//                        {
//                            var animName = "3";
//                            if (Constant.SceneVersion == "2")
//                            {
//                                animName = "1";
//                            }
//                            
//                            clearToLevelEffEnd.SetActive(true);
//                            clearToLevelEffEnd.GetComponent<SkeletonGraphic>().AnimationState
//                                .SetAnimation(0, animName, false);
//                            StartCoroutine(Delay.Run(() => { clearToLevelEffEnd.SetActive(false); }, 1));
//                        }
//                    }
//                });

            StartCoroutine(Delay.Run(() =>
            {
                if (clearToLevelEffEnd != null)
                {
                    if (!clearToLevelEffEnd.activeInHierarchy)
                    {
                        var animName = "3";
                        if (Constant.SceneVersion == "2")
                        {
                            animName = "1";
                        }

                        clearToLevelEffEnd.SetActive(true);
                        clearToLevelEffEnd.GetComponent<SkeletonGraphic>().AnimationState
                            .SetAnimation(0, animName, false);
                        StartCoroutine(Delay.Run(() => { clearToLevelEffEnd.SetActive(false); }, 1));
                    }
                }
            }, 0.6f));
        }

        public void ShowLevelUpRewardEff(List<int> rewardBlocks, int goldPosIndex, Action callback)
        {
            var posY = 450;
            var blockItems = Constant.GamePlayScript.GetBlockItems();
            StartCoroutine(Delay.Run(() =>
            {
                var flyTime = 0.3f;
                var lightDelayTime = 1f;
                if (rewardBlocks != null && rewardBlocks.Count > 0)
                {
                    foreach (var t in rewardBlocks)
                    {
                        var item = blockItems[t];
                        if (item != null)
                        {
                            var endPos = item.transform.localPosition + item.transform.Find("img").transform.localPosition;
                        
                            if (levelToClearEff != null)
                            {
                                if (_levelToClearEffPool == null)
                                {
                                    _levelToClearEffPool = new ObjectPool();
                                    _levelToClearEffPool.CreateObject(levelToClearEff, 1);
                                }
                            }
                            
                            var eff = _levelToClearEffPool.Get();
                            eff.transform.SetParent(effGroup.transform, false);
                            eff.transform.localScale = Vector2.one;
                            eff.transform.localPosition = new Vector2(0, posY);

                            eff.transform.DOLocalMove(endPos, flyTime).OnComplete(
                                () =>
                                {
                                    if (levelToClearEffEnd != null)
                                    {
                                        var lightEff = Instantiate(levelToClearEffEnd, effGroup.transform, false);
                                        lightEff.transform.localScale = Vector2.one;
                                        lightEff.transform.localPosition = endPos;
                                        lightEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,
                                            "G" + item.GetComponent<BlockItem>().GetLength(), false);
                                        StartCoroutine(Delay.Run(() => { Destroy(lightEff); }, lightDelayTime));
                                    }
                                
                                    StartCoroutine(Delay.Run(() => { _levelToClearEffPool.Put(eff); }, 0.3f));
                                });
                        }
                    }
                }

                if (goldPosIndex >= 0)
                {
                    var goldItem = blockItems[goldPosIndex];
                    if (goldItem != null)
                    {
                        var endPos = goldItem.transform.localPosition + goldItem.transform.Find("img").transform.localPosition;
                        
                        if (levelToClearEff != null)
                        {
                            if (_levelToClearEffPool == null)
                            {
                                _levelToClearEffPool = new ObjectPool();
                                _levelToClearEffPool.CreateObject(levelToClearEff, 1);
                            }
                        }
                        
                        var eff = _levelToClearEffPool.Get();
                        eff.transform.SetParent(effGroup.transform, false);
                        eff.transform.localScale = Vector2.one;
                        eff.transform.localPosition = new Vector2(0, posY);

                        eff.transform.DOLocalMove(
                            goldItem.transform.localPosition + goldItem.transform.Find("img").transform.localPosition, flyTime).OnComplete(
                            () =>
                            {
                                if (levelToClearEffEnd != null)
                                {
                                    var lightEff = Instantiate(levelToClearEffEnd, effGroup.transform, false);
                                    lightEff.transform.localScale = Vector2.one;
                                    lightEff.transform.localPosition = endPos;
                                    lightEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,
                                        "G" + goldItem.GetComponent<BlockItem>().GetLength(), false);
                                    StartCoroutine(Delay.Run(() => { Destroy(lightEff); }, lightDelayTime));
                                }

                                StartCoroutine(Delay.Run(() => { _levelToClearEffPool.Put(eff); }, 0.3f));
                            });
                    }
                }

                StartCoroutine(Delay.Run(() =>
                {
                    callback?.Invoke();
                    return;
                    
                    var curPosArr = new List<int>();
                    for (var i = 0; i < blockItems.Length; ++i)
                    {
                        if (blockItems[i] != null)
                        {
                            curPosArr.Add(i);
                        }
                    }
        
                    var randomArr = new List<int>();
                    for (var i = 0; i < 16; ++i)
                    {
                        var nextPos = curPosArr[Tools.GetNumFromRange(0, curPosArr.Count - 1)];
                        if (randomArr.Count > 0)
                        {
                            while (randomArr[randomArr.Count - 1] == nextPos)
                            {
                                nextPos = curPosArr[Tools.GetNumFromRange(0, curPosArr.Count - 1)];
                            }
                        }
                        randomArr.Add(nextPos);
                    }

                    if (rewardBlocks != null && rewardBlocks.Count > 0)
                    {
                        randomArr.Add(rewardBlocks[0]);
                    } else if (goldPosIndex >= 0)
                    {
                        randomArr.Add(goldPosIndex);
                    }
        
                    var lightEff = Instantiate(levelToClearEffEnd, effGroup.transform, false);
                    lightEff.transform.localScale = Vector2.one;
        
                    for (var i = 0; i < randomArr.Count; ++i)
                    {
                        var curIndex = i;
                        StartCoroutine(Delay.Run(() =>
                            {
                                var item = blockItems[randomArr[curIndex]];
                                if (item != null)
                                {
                                    var itemLength = item.GetComponent<BlockItem>().GetLength();
                                    lightEff.GetComponent<SkeletonGraphic>().AnimationState
                                        .SetAnimation(0, "L" + itemLength, false);
                                    lightEff.transform.localPosition = item.transform.localPosition + item.transform.Find("img").transform.localPosition;
                                }
                            }, i * 0.1f));
        
                        if (i == randomArr.Count - 1)
                        {
                            StartCoroutine(Delay.Run(() =>
                                {
                                    Destroy(lightEff);
                                    if (rewardBlocks != null && rewardBlocks.Count > 0)
                                    {
                                        foreach (var t in rewardBlocks)
                                        {
                                            ShowClearSpecialEdgeEff(blockItems[t]);
                                        }
                                    }
                                    
                                    StartCoroutine(Delay.Run(() =>
                                        {
                                            callback?.Invoke();
                                        }, 0.3f));
                                }, i * 0.1f));
                        }
                    }
                }, flyTime + lightDelayTime + 0.05f));
            }, 1.2f));
        }
        
        public void ShowManEffByName(string animName)
        {
            if (Blocks.IsTesting()) return;
            
            if (Constant.SceneVersion == "3")
            {
                var manObj = topUI.transform.Find("levelGroup").transform.Find("manBg").transform.Find("man");
                if (manObj != null)
                {
                    switch (animName)
                    {
                        case "jusang":
                            manObj.GetComponent<Man>().ShowJuSang();
                            break;
                        case "xingfen":
                            manObj.GetComponent<Man>().ShowXingFen();
                            StartCoroutine(Delay.Run(() => { ManagerAudio.PlaySound("laugh", 0.4f); }, 0.2f));
                            break;
                        case "newbest":
                            manObj.GetComponent<Man>().ShowNewBest();
                            break;
                        case "kun":
                            manObj.GetComponent<Man>().ShowKun();
                            break;
                    }   
                }
            }
        }
    }
}
