using DG.Tweening;
using Models;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpecialEffGoldCountDown : MonoBehaviour
    {
        public Image num1;
        public Image num2;

        public Sprite[] numSpriteFrames;
        
        private int _remainingTime;

        public void StartCountDown()
        {
            _remainingTime = Constant.SpecialGoldCountDownNum;
            UpdateTimeText();
            StartCoroutine(Delay.Run(CountDown, Constant.UpAnimTime));
        }

        private void UpdateTimeText()
        {
            if (_remainingTime >= 10)
            {
                num1.gameObject.SetActive(true);
                num2.transform.localPosition = new Vector2(18, 0);

                num1.sprite = numSpriteFrames[_remainingTime / 10];
                num2.sprite = numSpriteFrames[_remainingTime % 10];
                
                num1.SetNativeSize();
                num2.SetNativeSize();
            }
            else
            {
                num1.gameObject.SetActive(false);
                num2.transform.localPosition = Vector2.zero;
                num2.sprite = numSpriteFrames[_remainingTime % 10];
                
                num2.SetNativeSize();
            }
        }

        private void CountDown()
        {
            UpdateTimeText();
            
            GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(() =>
            {
                GetComponent<CanvasGroup>().DOFade(1, 0.5f).OnComplete(() =>
                {
                    if (_remainingTime <= 0)
                    {
                        Destroy(gameObject);
                        Constant.GamePlayScript.SpecialGoldEffCountDownEnd(true);
                    }
                    else
                    {
                        --_remainingTime;
                        CountDown();
                    }
                });
            });
        }
    }
}
