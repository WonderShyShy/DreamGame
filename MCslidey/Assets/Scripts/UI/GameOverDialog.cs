using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using Models;
using Other;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameOverDialog : MonoBehaviour
    {
        public TextMeshProUGUI curScore;
        public TextMeshProUGUI bestScore;
        public TextMeshProUGUI newBestScore;
        public GameObject btnAgain;

        public GameObject groupObj;
        public GameObject dogGroup;

        private void OnDestroy()
        {
            if (Constant.SceneVersion == "3")
            {
//                var topMask = GameObject.Find("topMask");
//                if (topMask != null)
//                {
//                    topMask.GetComponent<TopMask>().ShowLight();
//                }
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            ManagerAudio.PauseBgm();
            btnAgain.SetActive(false);
            bestScore.text = Player.GetBestScore().ToString();
            ResetBestScorePos();
            
            if (Constant.SceneVersion == "3")
            {
                
                if (Player.IsNewBestStatus())
                {
                    transform.Find("group").gameObject.SetActive(false);
                }
                else
                {
                    transform.Find("groupNewBest").gameObject.SetActive(false);
                }
            }

            var delayTime = 0f;
            if (Constant.SceneVersion == "3")
            {
                StartCoroutine(Delay.Run(() =>
                {
                    var man = groupObj.transform.Find("man");
                    man.gameObject.SetActive(false);
                    if (man != null && man.GetComponent<SkeletonGraphic>() != null && man.GetComponent<SkeletonGraphic>().AnimationState != null)
                    {
                        man.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry entry)
                        {
                            if (entry.ToString() == "JS_1")
                            {
                                man.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "JS_daiji1", false);
                            } else if (entry.ToString() == "JS_daiji1")
                            {
                                man.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "JS_daiji", true);
                                StartCoroutine(Delay.Run(() =>
                                {
                                    man.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "JS_daiji1", false);
                                }, 8));
                            }
                        };
                    
                        man.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "JS_1", false);
                        man.gameObject.SetActive(true);
                    }
                }, 0));
                
                delayTime = 0.25f;
            }

            StartCoroutine(Delay.Run(() =>
            {
                if (Constant.SceneVersion == "3")
                {
//                    if (GameObject.Find("topMask") != null)
//                    {
//                        GameObject.Find("topMask").GetComponent<TopMask>().HideLight();
//                    }

                    if (Player.IsNewBestStatus())
                    {
//                        var HG1 = transform.Find("groupNewBest").transform.Find("HG1");
//                        HG1.gameObject.SetActive(true);
//                        HG1.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry entry)
//                        {
//                            if (entry.ToString() == "animation6")
//                            {
//                                HG1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation4", true);
//                            }
//                        };
//                        HG1.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation6", false);
                        
//                        var HG2 = transform.Find("groupNewBest").transform.Find("HG2");
//                        HG2.gameObject.SetActive(true);
//                        HG2.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry entry)
//                        {
//                            if (entry.ToString() == "animation7")
//                            {
//                                HG2.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation3", true);
//                            }
//                        };
//                        HG2.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation7", false);
                        
                        btnAgain.transform.localPosition = new Vector2(0, -560);
                    }
                    
                    var scoreChangeDelayTime = 0f;
                    if (Player.IsNewBestStatus())
                    {
                        scoreChangeDelayTime = 0.7f;
                    }
                    
                    var curScoreNum = Player.GetCurScore();
                    var textArr = new List<TextMeshProUGUI>();
                    var scoreBlack = transform.Find("group").transform.Find("title_score").transform.Find("scoreBlack");
                    if (Player.IsNewBestStatus())
                    {
                        scoreBlack = transform.Find("groupNewBest").transform.Find("title_score").transform.Find("scoreBlack");
                        textArr.Add(scoreBlack.transform.Find("scoreWhite").GetComponent<TextMeshProUGUI>());
                    }
                    
                    textArr.Add(scoreBlack.GetComponent<TextMeshProUGUI>());
                    textArr.Add(scoreBlack.transform.Find("score").GetComponent<TextMeshProUGUI>());

                    var textSize = 220;
                    if (curScoreNum <= 99999)
                    {
                        textSize = 220;
                    } else if (curScoreNum >= 100000 && curScoreNum <= 999999)
                    {
                        textSize = 200;
                    } else if (curScoreNum >= 1000000)
                    {
                        textSize = 170;
                    }

                    foreach (var textItem in textArr)
                    {
                        textItem.fontSize = textSize;
                    }
                    
                    if (curScoreNum > 0)
                    {
                        StartCoroutine(Delay.Run(() => { GetComponent<Animator>().Play("GameOverScore", 0, 0); },
                            scoreChangeDelayTime));
                        
                        var seq = DOTween.Sequence();
                        seq.Append(DOTween.To(delegate(float value)
                        {
                            value = (float) Math.Floor(value);
                            var newScore = (int) value;

                            foreach (var textItem in textArr)
                            {
                                textItem.text = newScore.ToString();
                            }

                            ManagerAudio.PlaySound("numChange");
                        }, 0, curScoreNum, 1).SetEase(Ease.Linear).SetDelay(scoreChangeDelayTime));

                        seq.Append(scoreBlack.transform.transform.DOScale(new Vector3(1, 1, 1), 0.01f).SetEase(Ease.Linear).SetDelay(0.3f).OnComplete(
                            () =>
                            {
                                if (Player.IsNewBestStatus())
                                {
                                    if (curScoreNum <= 9999)
                                    {
                                        transform.Find("groupNewBest").transform.Find("title_score").transform.Find("xing_3").gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        transform.Find("groupNewBest").transform.Find("title_score").transform.Find("xing_5").gameObject.SetActive(true);
                                    }
                                }
                                
                                btnAgain.gameObject.SetActive(true);
                                btnAgain.GetComponent<Animator>().Play("btnPlayAgain", 0, 0);
                                StartCoroutine(Delay.Run(
                                    () => { btnAgain.GetComponent<Animator>().Play("btnPlayAgainBreath", 0, 0); },
                                    2.25f));
                            }));
                        seq.SetUpdate(true);
                    }
                    else
                    {
                        btnAgain.gameObject.SetActive(true);
                        btnAgain.GetComponent<Animator>().Play("btnPlayAgain", 0, 0);
                        StartCoroutine(Delay.Run(
                            () => { btnAgain.GetComponent<Animator>().Play("btnPlayAgainBreath", 0, 0); },
                            2.25f));
                    }
                }
                else
                {
                    var dogGroupObj = Instantiate(dogGroup, groupObj.transform, false);
                    dogGroupObj.transform.localPosition = new Vector2(0, 300);
                    dogGroupObj.transform.localScale = new Vector2(2, 2);
                    dogGroupObj.GetComponent<GameOverDogGroup>()
                        .SetText(Player.IsNewBestStatus() ? "newBest" : "normal");
                    dogGroupObj.GetComponent<GameOverDogGroup>().ShowStarEff(Player.ScoreToStar(Player.GetCurScore()));
                    dogGroupObj.GetComponent<GameOverDogGroup>().SetEndCallback(() =>
                    {
                        StartCoroutine(Delay.Run(() =>
                        {
                            btnAgain.SetActive(true);
                            btnAgain.GetComponent<CanvasGroup>().alpha = 0;
                            btnAgain.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                        }, 0.5f));
                    });
                    
                    curScore.text = "0";
                    newBestScore.text = "0";

                    if (Player.IsNewBestStatus())
                    {
                        curScore.gameObject.SetActive(false);
                    }
                    else
                    {
                        newBestScore.gameObject.SetActive(false);
                    }

                    if (Player.IsNewBestStatus())
                    {
                        Tools.NumChange(newBestScore, Player.GetCurScore(), 1,
                            () => { ManagerAudio.PlaySound("numChange"); });
                    }
                    else
                    {
                        Tools.NumChange(curScore, Player.GetCurScore(), 1, () => { ManagerAudio.PlaySound("numChange"); });
                    }
                }
                
                Constant.GamePlayScript.ClearBlocksData();
                
                if (Player.IsNewBestStatus())
                {
                    ManagerAudio.PlaySound("newRecord");
                    Tools.NumChange(bestScore, Player.GetBestScore(), 1,
                        () =>
                        {
                            ManagerAudio.PlaySound("numChange"); 
                            ResetBestScorePos();
                        });
                }
                ResetBestScorePos();
                ManagerLocalData.SaveData();
            }, delayTime));
        }

        public void OnBtnClk(string btnType)
        {
            DebugEx.Log(btnType);
            switch (btnType)
            {
                case "again":
                    ManagerDialog.DestroyDialog("GameOverDialog");
                    Constant.GamePlayScript.RestartGame();
                    ManagerAudio.ResumeBgm();
                    break;
            }
            
            ManagerAudio.PlaySound("sound_ButtonDown");
        }

        private void ResetBestScorePos()
        {
            if (Constant.SceneVersion == "3")
            {
                var parentTransform = bestScore.transform.parent.transform;
                var oriPos = parentTransform.localPosition;
                parentTransform.localPosition = new Vector3(-25 * bestScore.text.Length, oriPos.y, oriPos.z);
            }
        }
    }
}
