using DG.Tweening;
using Manager;
using Models;
using Other;
using UnityEngine;

namespace UI
{
    public class B43221Dialog : MonoBehaviour
    {
        public GameObject warningImg;
        
        private int _startSlideNumber = -1;
        
        void Start()
        {
            _startSlideNumber = Constant.GameStatusData.SlideNumber;
            warningImg.GetComponent<CanvasGroup>().alpha = 100 / 255f;

            transform.localPosition = new Vector2(-Constant.ScreenWidth, gameObject.transform.localPosition.y);
            transform.DOLocalMoveX(0, 0.5f).OnComplete(() =>
            {
                var seq = DOTween.Sequence();
                seq.Append(warningImg.GetComponent<CanvasGroup>().DOFade(1, 1));
                seq.Append(warningImg.GetComponent<CanvasGroup>().DOFade(100 / 255f, 1));
                seq.SetLoops(-1);
                seq.SetUpdate(true);
            });

            transform.localScale = new Vector2(2, 2);
        }

        public void CheckRemove()
        {
            if ((Constant.GameStatusData.SlideNumber - _startSlideNumber) >= Constant.B43221RemoveSlideNumber)
            {
                OnBtnClk("close");
            }
        }

        public void OnBtnClk(string btnType)
        {
            DebugEx.Log(btnType);
            switch (btnType)
            {
                case "ad":
                    if (!Player.UserCanMove) return;
                    
                    Constant.GameStatusData.DWAD_CLICK = true;
                    Constant.GameStatusData.DWAD_CLICK_SLIDENUMBER =
                        Constant.GameStatusData.SlideNumber - _startSlideNumber;
                    Player.SaveGameStatusData();
                    ManagerLocalData.SaveData();
                    
                    Player.UserCanMove = false;
                    OnBtnClk("close");
                    ManagerAd.PlayRewardAd((int)ManagerAd.RewardVideoPlayType.B43221);
                    
                    break;
                case "close":
                    ManagerDialog.DestroyDialog("B43221Dialog");
                    
                    // todo txy 引导完成
                    break;
            }
        }
    }
}
