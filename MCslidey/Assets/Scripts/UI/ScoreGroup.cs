using DG.Tweening;
using Manager;
using Models;
using Other;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreGroup : MonoBehaviour
    {
        public GameObject groupNormal;
        public GameObject groupNewBest;
        public GameObject groupBest;

        public TextMeshProUGUI textNormal;
        public TextMeshProUGUI textNewBest;
        public TextMeshProUGUI textBest;

        public TextMeshProUGUI textLevel;
        public TextMeshProUGUI textProgress;
        public GameObject barProgress;
        private int _oldLevel = 0;
        
        private int _barProgressHeight = 22;

        void Start()
        {
            if (Constant.SceneVersion == "3")
            {
                _barProgressHeight = 40;
                groupNewBest.transform.Find("bgEff").gameObject.SetActive(false);
            }
        }
        
        public void ResetScoreUi()
        {
            textNormal.text = "0";
            textNewBest.text = "0";
            textBest.text = "0";
            textLevel.text = Constant.LevelText + "01";
            _oldLevel = 1;
            
            barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _barProgressHeight);
            textProgress.text = "0/0";
        }

        public void UpdateScoreUi(bool showEff = true)
        {
            var curScore = Player.GetCurScore();
            var bestScore = Player.GetBestScore();

            var shouldShowNewBestEff = false;
            if (curScore > bestScore)
            {
                bestScore = curScore;
                if (!Player.IsNewBestStatus() && curScore >= 100)
                {
                    Player.NewBestStatus();
                    if (Constant.SceneVersion != "3")
                    {
                        Constant.EffCtrlScript.ShowNewBestEff();
                    }
                    
                    shouldShowNewBestEff = true;
                }
            }
            
            groupNewBest.SetActive(Player.IsNewBestStatus());
            groupNormal.SetActive(!Player.IsNewBestStatus());
            groupBest.transform.Find("newBg").gameObject.SetActive(Player.IsNewBestStatus());
            
            //分数更新
            if (showEff)
            {
                var dstScale = 1.5f;
                if (Constant.SceneVersion == "1")
                {
                    dstScale = 1.2f;
                }
                
                if (Player.IsNewBestStatus())
                {
                    if (Constant.GameStatusData.IsFirstGetBestScorePreGame)
                    {
                        Constant.GameStatusData.IsFirstGetBestScorePreGame = false;
                    }
                    
                    Tools.NumChange(textNewBest, curScore);
                    Tools.NumChange(textBest, curScore, 0.5f, UpdateFlagPos);

                    if (Constant.SceneVersion == "3" && shouldShowNewBestEff)
                    {
                        var bgEff = groupNewBest.transform.Find("bgEff");
                        var light1 = bgEff.transform.Find("light1");
                        var light2 = bgEff.transform.Find("light2");
                        bgEff.gameObject.SetActive(true);
                        light1.transform.DORotate(new Vector3(0, 0, -360), 3).SetRelative(true).SetEase(Ease.Linear);
                        light2.transform.DORotate(new Vector3(0, 0, 360), 3).SetRelative(true).SetEase(Ease.Linear);
                        
                        StartCoroutine(Delay.Run(
                            () =>
                            {
                                bgEff.GetComponent<CanvasGroup>().DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() =>
                                {
                                    bgEff.gameObject.SetActive(false);
                                });
                            }, 2));
                        
                        textNewBest.transform.DOScale(new Vector2(dstScale, dstScale), 0.25f).OnComplete(() =>
                        {
                            textNewBest.transform.DOScale(new Vector2(1, 1), 0.25f);
                        });
                        
                        textNewBest.transform.Find("eff").gameObject.SetActive(true);
                        StartCoroutine(
                            Delay.Run(() => { textNewBest.transform.Find("eff").gameObject.SetActive(false); }, 3));
                        
                        StartCoroutine(Delay.Run(() =>
                        {
                            ManagerAudio.PlaySound("newBest");
                        }, 0.1f));

                        Constant.EffCtrlScript.ShowManEffByName("newbest");
                    }
                    else
                    {
                        if (Constant.SceneVersion != "3")
                        {
                            textNewBest.transform.DOScale(new Vector2(dstScale, dstScale), 0.25f).OnComplete(() =>
                            {
                                textNewBest.transform.DOScale(new Vector2(1, 1), 0.25f);
                            });
                        }
                    }
                }
                else
                {
                    Tools.NumChange(textNormal, curScore);

                    if (Constant.SceneVersion != "3")
                    {
                        textNormal.transform.DOScale(new Vector2(dstScale, dstScale), 0.25f).OnComplete(() =>
                        {
                            textNormal.transform.DOScale(new Vector2(1, 1), 0.25f);
                        });
                    }
                }
            }
            else
            {
                textNormal.text = curScore.ToString();
                textNewBest.text = curScore.ToString();
                textBest.text = bestScore.ToString();
                UpdateFlagPos();
            }

            //等级更新
            var newLevel = Blocks.UpdateLevelByScore(curScore);
            if (newLevel > _oldLevel)
            {
                if (newLevel >= 10)
                {
                    textLevel.text = Constant.LevelText + newLevel;
                }
                else
                {
                    textLevel.text = Constant.LevelText + "0" + newLevel;
                }
                
                _oldLevel = newLevel;

                Constant.GameStatusData.CurrentLevel = newLevel;
                
                if (showEff)
                {
//                    Constant.EffCtrlScript.ShowLevelUpEff(newLevel);
                    if (newLevel == 2)
                    {
                        if (!Constant.AFData.level_2)
                        {
                            Constant.AFData.level_2 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }else if (newLevel == 3)
                    {
                        if (!Constant.AFData.level_3)
                        {
                            Constant.AFData.level_3 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }else if (newLevel == 4)
                    {
                        if (!Constant.AFData.level_4)
                        {
                            Constant.AFData.level_4 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }
                    else if (newLevel == 5)
                    {
                        if (!Constant.AFData.level_5)
                        {
                            Constant.AFData.level_5 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }else if (newLevel == 6)
                    {
                        if (!Constant.AFData.level_6)
                        {
                            Constant.AFData.level_6 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }else if (newLevel == 8)
                    {
                        if (!Constant.AFData.level_8)
                        {
                            Constant.AFData.level_8 = true;
                            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
                        }
                    }
                }
            }

            if (barProgress)
            {
                var totalWidth = barProgress.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
                var nextLevelScore = Blocks.GetNextLevelScore();
                var curLevelScore = curScore;

                if (Constant.LevelProgressVersion == "1")
                {
                    nextLevelScore -= Blocks.GetCurLevelScore();
                    curLevelScore -= Blocks.GetCurLevelScore();
                }
                
                if (!showEff)
                {
                    if (curLevelScore > 0 && curLevelScore > nextLevelScore)
                    {
                        barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, _barProgressHeight);
                        textProgress.text = nextLevelScore + "/" + nextLevelScore;
                        CheckBarProgress();
                    }
                    else
                    {
                        barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(curLevelScore * 1.0f / nextLevelScore * totalWidth, _barProgressHeight);
                        textProgress.text = curLevelScore + "/" + nextLevelScore;
                        CheckBarProgress();
                    }
                }
                else
                {
                    if (curLevelScore > 0 && curLevelScore > nextLevelScore)
                    {
                        barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, _barProgressHeight);
                        textProgress.text = nextLevelScore + "/" + nextLevelScore;
                        CheckBarProgress();
                    }
                    else
                    {
                        var oldScore = int.Parse(textProgress.text.Split('/')[0]);
                        textProgress.text = oldScore.ToString();

                        if (oldScore > curLevelScore)
                        {
                            var midScore = Blocks.GetCurLevelScore() - Blocks.GetLevelScoreByLevel(Blocks.CurLevel - 1);
                            Tools.NumChange(textProgress, midScore, 0.25f, () =>
                            {
                                var curChange = int.Parse(textProgress.text);
                                textProgress.text = curChange + "/" + midScore;
                                barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(curChange * 1.0f / midScore * totalWidth, _barProgressHeight);
                                CheckBarProgress();
                            }, false, () =>
                            {
                                textProgress.text = "0/" + nextLevelScore;
                                var oldScore1 = int.Parse(textProgress.text.Split('/')[0]);
                                textProgress.text = oldScore1.ToString();
                                
                                Tools.NumChange(textProgress, curLevelScore, 0.25f, () =>
                                {
                                    var curChange = int.Parse(textProgress.text);
                                    textProgress.text = curChange + "/" + nextLevelScore;
                                    barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(curChange * 1.0f / nextLevelScore * totalWidth, _barProgressHeight);
                                    CheckBarProgress();
                                });
                            });
                        }
                        else
                        {
                            Tools.NumChange(textProgress, curLevelScore, 0.5f, () =>
                            {
                                var curChange = int.Parse(textProgress.text);
                                textProgress.text = curChange + "/" + nextLevelScore;
                                barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(curChange * 1.0f / nextLevelScore * totalWidth, _barProgressHeight);
                                CheckBarProgress();
                            });
                        }
                    }
                }
            }
        }

        private void CheckBarProgress()
        {
            if (Constant.SceneVersion == "3" && barProgress.GetComponent<RectTransform>().sizeDelta.x <= 10)
            {
                barProgress.GetComponent<RectTransform>().sizeDelta = new Vector2(10, _barProgressHeight);
            }
        }

        private void UpdateFlagPos()
        {
            if (Constant.SceneVersion == "1")
            {
                var flag = groupBest.transform.Find("flag");
                var newFlag = groupBest.transform.Find("newBg").transform.Find("newFlag");
                var newPosX = -10 - (textBest.text.Length - 1) * 20;
                flag.transform.localPosition = new Vector2(newPosX, 4);
                newFlag.transform.localPosition = new Vector2(newPosX, 6.2f);
            }
        }
    }
}
