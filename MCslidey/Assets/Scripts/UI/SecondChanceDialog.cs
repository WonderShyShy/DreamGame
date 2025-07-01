using System.Collections;
using Manager;
using Models;
using Other;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SecondChanceDialog : MonoBehaviour
    {
        public GameObject content;
        public GameObject btnNoThx;
        public Sprite[] countDownSpriteFrames;
        public TextMeshProUGUI curScore;
        
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

        private void PlayCountDownSound()
        {
            ManagerAudio.PlaySound("countDown"); 
            StartCoroutine(Delay.Run(() =>
                {
                    PlayCountDownSound();
                }, 80 / 60f));
        }

        // Start is called before the first frame update
        void Start()
        {
            curScore.text = Player.GetCurScore().ToString();
            Constant.GameStatusData.continueShow = true;
            
            if (Constant.SceneVersion == "3")
            {
//                GameObject.Find("topMask").GetComponent<TopMask>().HideLight();
                
                var scoreGroup = curScore.transform.parent;
                var oriPos = scoreGroup.transform.localPosition;
                scoreGroup.transform.localPosition =
                    new Vector2(Player.GetCurScore().ToString().Length * -129 / 6f, oriPos.y);

                StartCoroutine(Delay.Run(() =>
                {
                    var countDownObj = content.transform.Find("countDownShadow2");
                    countDownObj.gameObject.SetActive(true);
                    countDownObj.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry entry)
                    {
                        if (entry.ToString() == "animation_2")
                        {
                            OnBtnClk("noThx");
                        }
                    };
                    countDownObj.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation_2", false);
                    StartCoroutine(Delay.Run(() => { PlayCountDownSound(); }, 25 / 60f));
                    
//                    StartCountDown();
                }, 1.2f));
            }
        }

        public void OnBtnClk(string btnType)
        {
            DebugEx.Log(btnType);
            switch (btnType)
            {
                case "playOn":
                    Constant.GameStatusData.continueClk = true;
                    Constant.GameStatusData.videoShow = true;
                    Constant.RewardVideoType = "secondChance";

                    AdManager.Instance.RewardAction = () =>
                    {
                        Player.UseSecondChance();
                        var result = new Hashtable();
                        result.Add("success", true);
                        Constant.GamePlayScript.SecondChanceResult(result);
                        ManagerDialog.DestroyDialog("SecondChanceDialog");
                    };
                    AdManager.Instance.ShowRewardAd();
                  
                    break;
                case "noThx":
                    ManagerDialog.DestroyDialog("SecondChanceDialog");
                    Constant.GamePlayScript.ShowGameOverEff();
                    break;
            }
        }
    }
}
