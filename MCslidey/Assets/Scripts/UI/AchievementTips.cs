using System.Collections.Generic;
using DG.Tweening;
using Manager;
using Models;
using Other;
using TMPro;
using UnityEngine;

namespace UI
{
    public class AchievementTips : MonoBehaviour
    {
        public GameObject tipBoard;

        private ObjectPool _tipBoardPool;
        private List<string[]> _tipDataList;
        private bool _isShowingTip = false;
        
        // Start is called before the first frame update
        async void Start()
        {
            if (!Constant.AchievementSwitch) return;
            
            _tipBoardPool = new ObjectPool();

            if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2")
            {
                
            }
            else
            {
                if (tipBoard != null)
                {
                    _tipBoardPool.CreateObject(tipBoard);
                }
            }
            
            _tipDataList = new List<string[]>();

//            for (var i = 0; i < 10; ++i)
//            {
//                AddAchievementTip("DYING RESCUE", i);
//            }
        }

        public void AddAchievementTip(string title, int num)
        {
            if (Blocks.IsTesting()) return;
            
            if (!Constant.AchievementSwitch) return;

            if (_tipBoardPool != null && _tipBoardPool.HaveObjectBase())
            {
                _tipDataList.Add(new[]{title, num.ToString()});
                if (!_isShowingTip)
                {
                    ShowTip();
                }
            }
        }

        private void ShowTip()
        {
            _isShowingTip = true;
            if (_tipDataList.Count > 0)
            {
                var board = _tipBoardPool.Get();
                var data = _tipDataList[0];
                
                board.transform.Find("title").GetComponent<TextMeshProUGUI>().text = data[0];
                board.transform.Find("num").GetComponent<TextMeshProUGUI>().text = data[1];
                board.transform.SetParent(transform);
                
                var okImgRect = board.transform.Find("bg").transform.Find("ok").GetComponent<RectTransform>();
                var okImgOriSize = okImgRect.sizeDelta;
                okImgRect.sizeDelta = new Vector2(0, 0);
                
                var boardSize = board.GetComponent<RectTransform>().sizeDelta;
                board.GetComponent<CanvasGroup>().alpha = 1;
                board.transform.localScale = Vector2.one;
                board.transform.localPosition = new Vector2(-Constant.ScreenWidth, Constant.BlockHeight * 5 - boardSize.y / 2f);
                board.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    ManagerAudio.PlaySound("ok", 0.3f);
                    DOTween.To(delegate(float value) { okImgRect.sizeDelta = new Vector2(value, okImgOriSize.y); }, 0,
                        okImgOriSize.x, 0.5f);
                    board.GetComponent<CanvasGroup>().DOFade(0, 0.45f).SetDelay(1.5f);
                    board.transform.DOLocalMoveY(board.transform.localPosition.y + 200, 0.5f).SetDelay(1.5f).OnComplete(() =>
                        {
                            _tipDataList.RemoveAt(0);
                            _tipBoardPool.Put(board);
                            ShowTip();
                        });
                    });
            }
            else
            {
                _isShowingTip = false;
            }
        }
    }
}
