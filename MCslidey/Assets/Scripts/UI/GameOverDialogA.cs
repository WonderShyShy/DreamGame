using BFF;
using DG.Tweening;
using Manager;
using Models;
using Other;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameOverDialogA : MonoBehaviour
    {
        public TextMeshProUGUI curScore;
        public TextMeshProUGUI bestScore;
        public TextMeshProUGUI newBestScore;

        public GameObject achievementGroup;
        public GameObject achievementBoard;
        public GameObject achievementEff;

        public GameObject btnAgain;
        public GameObject btnRemoveAd;
        
        public GameObject groupObj;
        public GameObject dogGroup;

        private bool _starEffEnd = false;
        private bool _aEnd = false;
        private bool _removeAdClicked = false;

        // Start is called before the first frame update
        void Start()
        {
            if (Constant.ShowRemoveAd)
            {
                groupObj.transform.localScale = new Vector2(0.85f, 0.85f);
                groupObj.transform.localPosition = new Vector2(0, 100);
            }
            
            btnRemoveAd.SetActive(false);
            btnAgain.SetActive(false);
            
            var dogGroupObj = Instantiate(dogGroup, groupObj.transform, false);
            dogGroupObj.transform.localPosition = new Vector2(0, 550);
            dogGroupObj.transform.localScale = new Vector2(2, 2);
            dogGroupObj.GetComponent<GameOverDogGroup>().SetText(Player.IsNewBestStatus() ? "newBest" : "normal");
            dogGroupObj.GetComponent<GameOverDogGroup>().ShowStarEff(Player.ScoreToStar(Player.GetCurScore()));
            dogGroupObj.GetComponent<GameOverDogGroup>().SetEndCallback(() =>
            {
                StartCoroutine(Delay.Run(() =>
                {
                    _starEffEnd = true;
                    ShowPlayAgainBtn();
                }, 0.5f));
            });
            
            ManagerAudio.PauseBgm();
            Constant.GamePlayScript.ClearBlocksData();
            
            achievementBoard.transform.localPosition = new Vector2(0, Constant.ScreenHeight * 2);
            achievementEff.transform.localPosition = new Vector2(0, Constant.ScreenHeight * 2);
            
            curScore.text = "0";
            newBestScore.text = "0";
            bestScore.text = Player.GetBestScore().ToString();

            if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
            {
                var flag = bestScore.transform.Find("flag");
                flag.transform.localPosition = new Vector2(-30 - bestScore.text.Length * 20, -2);
            }
            
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
                ManagerAudio.PlaySound("newRecord");
                
                Tools.NumChange(newBestScore, Player.GetCurScore(), 1, () =>
                {
                    ManagerAudio.PlaySound("numChange");
                });
                Tools.NumChange(bestScore, Player.GetBestScore(), 1, () =>
                {
                    ManagerAudio.PlaySound("numChange");
                });
            }
            else
            {
                Tools.NumChange(curScore, Player.GetCurScore(), 1, () =>
                {
                    ManagerAudio.PlaySound("numChange");
                });
            }

            ManagerLocalData.SaveData();

            if (Constant.AchievementSwitch)
            {
                CreateAchievementBoard();
            }
            
            bestScore.gameObject.SetActive(false);
            curScore.transform.localPosition = new Vector2(0, 100);
            newBestScore.transform.localPosition = new Vector2(0, 100);
        }

        private async void CreateAchievementBoard()
        {
            await Achievement.LoadData();
            
            var titleArr = new[]
            {
                "LINES CLEARED",
                "FROZEN CLEARED",
//                "DYING RESCUE",
                "COMBO TIMES",
                "KEEP SCORING",
                "SKILLFUL MOVES",
//                "CLEAR ALL"
            };
            var numArr = new[]
            {
                Constant.GameStatusData.RemoveNumber,
                Constant.GameStatusData.RemoveFrozen,
//                Constant.GameStatusData.DieToLifeCount,
                Constant.GameStatusData.ComboNumber,
                Constant.GameStatusData.StepScore,
                Constant.GameStatusData.clearTypeC,
//                Constant.GameStatusData.AllClearCount
            };
            
            var percentArr = new[]
            {
                Achievement.GetPercentByRemoveNumber(Constant.GameStatusData.RemoveNumber),
                Achievement.GetPercentByIceNumber(Constant.GameStatusData.RemoveFrozen),
//                Achievement.GetPercentByDW(Constant.GameStatusData.DieToLifeCount),
                Achievement.GetPercentByComboNumber(Constant.GameStatusData.ComboNumber),
                Achievement.GetPercentByStepScore(Constant.GameStatusData.StepScore),
                Achievement.GetPercentBySkillMovesNumber(Constant.GameStatusData.clearTypeC),
//                99//清屏操作无论几次都是超越99%的玩家
            };

            GameObject[] boardObjArr = new GameObject[titleArr.Length];
            for (var i = 0; i < titleArr.Length; ++i)
            {
                //特殊成就没完成则不显示
//                if (i == titleArr.Length - 1)
//                {
//                    if (numArr[i] <= 0)
//                    {
//                        break;
//                    }
//                }
                
                var board = Instantiate(achievementBoard, achievementGroup.transform, false);
                var boardSize = board.GetComponent<RectTransform>().sizeDelta;
                board.transform.localScale = Vector2.one;
                board.transform.localPosition = new Vector2(-Constant.ScreenWidth, -boardSize.y / 2f - i * boardSize.y);
                boardObjArr[i] = board.gameObject;
                
                var textTitle = board.transform.Find("title").GetComponent<TextMeshProUGUI>();
                var textNum = board.transform.Find("num").GetComponent<TextMeshProUGUI>();
                var textPercent = board.transform.Find("percent").GetComponent<TextMeshProUGUI>();
                textPercent.gameObject.SetActive(false);

                textTitle.text = titleArr[i];
                textNum.text = numArr[i].ToString();
                textPercent.text = percentArr[i] + "";

                if (i == 0)
                {
                    board.transform.Find("line").gameObject.SetActive(false);
                }

                var i1 = i;
                board.transform.DOLocalMoveX(0, 0.3f).SetDelay(0.1f * i).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        DOTween.To(delegate(float value) { textNum.text = Tools.ChinaRound(value).ToString(); }, 0,
                            numArr[i1], 0.2f);
                    });
            }
            
            StartCoroutine(Delay.Run(() =>
            {
                var showEff = false;
                for (var j = 0; j < boardObjArr.Length; ++j)
                {
                    if (boardObjArr[j] != null && percentArr[j] > 0)
                    {
                        var eff = Instantiate(achievementEff, boardObjArr[j].transform, false);
                        eff.transform.localScale = new Vector2(1, 1.1f);
                        eff.transform.localPosition = Vector2.one;
                        eff.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "1", false);
                        eff.SetActive(true);
                        boardObjArr[j].transform.Find("percent").gameObject.SetActive(true);
                        
                        showEff = true;
                    }
                }

                if (showEff)
                {
                    StartCoroutine(Delay.Run(() =>
                    {
                        _aEnd = true;
                        ShowPlayAgainBtn();
                    }, 0.5f));
                }
                else
                {
                    _aEnd = true;
                    ShowPlayAgainBtn();
                }
            }, 1.1f));
        }

        private void ShowPlayAgainBtn()
        {
            if (_aEnd && _starEffEnd)
            {
                btnAgain.SetActive(true);
                btnAgain.GetComponent<CanvasGroup>().alpha = 0;
                btnAgain.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

                if (Constant.ShowRemoveAd)
                {
                    btnRemoveAd.SetActive(true);
                    btnRemoveAd.GetComponent<CanvasGroup>().alpha = 0;
                    btnRemoveAd.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                }
            }
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
                case "removeAd":
                    if (_removeAdClicked) return;
                    _removeAdClicked = true;
                    break;
            }
            
            ManagerAudio.PlaySound("sound_ButtonDown");
        }
    }
}
