using DG.Tweening;
using Manager;
using Models;
using Other;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpecialGoldDialog : MonoBehaviour
    {
        public GameObject content;
        
        public void StartShow(Vector2 pos)
        {
            if (Constant.SceneVersion == "3" && Constant.ShowSpecialGoldDialogAnim)
            {
                content.transform.Find("bg").gameObject.SetActive(false);
                content.transform.Find("animContent").gameObject.SetActive(true);
            }
            else
            {
                content.transform.Find("bg").gameObject.SetActive(true);
                content.transform.Find("animContent").gameObject.SetActive(false);
            }
            
            var localPosition = content.transform.localPosition;
            var originalPos = localPosition;
            content.transform.localScale = Vector2.zero;
            content.transform.localPosition = pos;
            content.transform.DOLocalMove(new Vector3(originalPos.x, originalPos.y, 0), 0.5f).SetEase(Ease.OutBack);
            content.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(ShowClearGoldAnim);
            
            ++Constant.GameStatusData.SpecialGoldDialog;
        }
        
        public void OnBtnClk(string btnType)
        {
            DebugEx.Log(btnType);
            switch (btnType)
            {
                case "playOn":
                    ++Constant.GameStatusData.SpecialGoldVideo;
                    Constant.RewardVideoType = "gold";

#if UNITY_EDITOR
                    //test start 
                    Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks, true);
                    ManagerDialog.DestroyDialog("SpecialGoldDialog");
                    return;
                    //test end
#endif
                    
                    var showRewardVideo = true;
                    if (Constant.SpecialGoldAdRvAndInterSwitch)
                    {
                        if (!ManagerAd.HaveRewardAd())
                        {
                            if (ManagerAd.HaveInterstitialAd())
                            {
                                showRewardVideo = false;
                            }
                        }
                    }

                    if (Constant.SpecialGoldAdInterstitialSwitch)
                    {
                        showRewardVideo = false;
                    }
                    
                    if (!showRewardVideo)
                    {
                        if (ManagerAd.HaveInterstitialAd())
                        {
                            ++Constant.GameStatusData.SpecialGoldInsShow;
                            ManagerAd.PlayInterstitialAd((int)ManagerAd.InterstitialPlayType.SpecialGold);
                        }
                        else
                        {
                            Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks);
                        }
                    }
                    else
                    {
                        if (ManagerAd.HaveRewardAd())
                        {
                            ++Constant.GameStatusData.SpecialGoldVideoShow;
                            ManagerAd.PlayRewardAd((int) ManagerAd.RewardVideoPlayType.SpecialGold);
                        }
                        else
                        {
                            Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks);
                        }
                    }
                    
                    ManagerDialog.DestroyDialog("SpecialGoldDialog");
                    break;
                case "close":
                    ManagerDialog.DestroyDialog("SpecialGoldDialog");
                    Constant.GamePlayScript.MoveEnd(null, Constant.PreviousBlocks);
                    break;
            }
        }

        private void ShowClearGoldAnim()
        {
            if (Constant.SceneVersion != "3") return;
            if (!Constant.ShowSpecialGoldDialogAnim) return;
            
            var edgeItemSpineAnimNames = new []
            {
                "2",
                "1",
                "1",
                "3",
                "3"
            };

            var edgeItemLengths = new[]
            {
                2, 
                1, 
                1, 
                3, 
                3
            };
            
            var animContent = content.transform.Find("animContent");
            var goldItem = animContent.transform.Find("block_gold").gameObject;
            var goldItemSize = goldItem.GetComponent<RectTransform>().sizeDelta;
            var goldItemEff = animContent.transform.Find("ClearSpecialEffGold");
            
            goldItemEff.gameObject.SetActive(true);
            goldItemEff.transform.localScale = new Vector2(goldItemSize.x / 348f, goldItemSize.y / 116f);
            goldItemEff.transform.localRotation = Quaternion.Euler(0, 0, 0);
            goldItemEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "3", true);

            goldItem.SetActive(true);
            for (var i = 0; i < 5; ++i)
            {
                animContent.transform.Find("block_" + i).GetComponent<Image>().color = Color.white;
            }

            var xianEffList = new GameObject[5];
            StartCoroutine(Delay.Run(() =>
            {
                goldItemEff.transform.Find("fangdianEff").gameObject.SetActive(true);
    
                var goldItemPos = goldItem.transform.localPosition;

                var tmpXianEff = animContent.transform.Find("ClearSpecialEffGoldLine");
                for (var i = 0; i < 5; ++i)
                {
                    var edgeItem = animContent.transform.Find("block_" + i).gameObject;
                    var xianEff = Instantiate(tmpXianEff, animContent.transform, false);
                    var lineEff = xianEff.transform.Find("lineEff").gameObject;
                    var dstPos = edgeItem.transform.localPosition;
                    var dis = Vector2.Distance(goldItemPos, dstPos);

                    xianEffList[i] = xianEff.gameObject;
                    
                    var angle = Vector2.Angle(Vector2.up,
                        new Vector2(dstPos.x - goldItemPos.x, dstPos.y - goldItemPos.y));
                    if (goldItemPos.x < dstPos.x)
                    {
                        angle = -angle;
                    }
    
                    angle += 90;

                    lineEff.transform.localRotation = Quaternion.Euler(0, 0, angle);
                    lineEff.transform.localScale = new Vector2(dis / 540f, (dis / 540f > 1 ? dis / 540f : 1) * 0.8f);
    
                    var itemIndex = i;
                    StartCoroutine(Delay.Run(() =>
                        {
                            xianEff.gameObject.SetActive(true);
                            lineEff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "shandian2", false);
    
                            StartCoroutine(Delay.Run(() =>
                                {
                                    edgeItem.transform.Find("ClearSpecialEffGold").gameObject.SetActive(true);
                                    edgeItem.transform.Find("ClearSpecialEffGold").GetComponent<SkeletonGraphic>()
                                        .AnimationState
                                        .SetAnimation(0, edgeItemSpineAnimNames[itemIndex], true);
    
                                    edgeItem.transform.Find("ClearSpecialEffGold").transform.Find("fangdianEff")
                                        .gameObject
                                        .SetActive(true);
                                }, 1 / 4f));
                        }, i * 0.1f));
                }
    
                StartCoroutine(Delay.Run(() =>
                {
                    goldItemEff.transform.Find("fangdianEff").gameObject.SetActive(false);
                    goldItemEff.gameObject.SetActive(false);
                    goldItem.SetActive(false);
                
                    var clearEff = animContent.transform.Find("ClearBlockEff").gameObject;
                    for (var i = 0; i < 5; ++i)
                    {
                        Destroy(xianEffList[i]);
                        
                        var edgeItem = animContent.transform.Find("block_" + i).gameObject;
                        var edgeItemSize = edgeItem.GetComponent<RectTransform>().sizeDelta;
                        var edgeItemLength = edgeItemLengths[i];
                        var posBaseX = edgeItemSize.x / (edgeItemLength * 2f);
                        edgeItem.GetComponent<Image>().DOFade(0, 0.2f);
    
                        edgeItem.transform.Find("ClearSpecialEffGold").transform.Find("fangdianEff").gameObject
                            .SetActive(false);
                        edgeItem.transform.Find("ClearSpecialEffGold").gameObject.SetActive(false);
    
                        for (var j = 0; j < edgeItemLength; ++j)
                        {
                            var eff = Instantiate(clearEff, animContent.transform, false);
                            eff.transform.localPosition = edgeItem.transform.localPosition - new Vector3(edgeItemSize.x, 0, 0) +
                                                          new Vector3((2 * j + 1) * posBaseX, 0, 0);
                            eff.SetActive(true);
    
                            StartCoroutine(Delay.Run(() =>
                            {
                                Destroy(eff);
                            }, 2f));
                        }
                    }
    
                    StartCoroutine(Delay.Run(ShowClearGoldAnim, 2));
                }, 1.5f));
            }, 0.5f));
        }
    }
}
