using DG.Tweening;
using Models;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MeteorGroup3 : MonoBehaviour
    {
        public GameObject meteorEff;

        // Start is called before the first frame update
        void Start()
        {
            ShowMeteorEff();
        }

        private void ShowMeteorEff()
        {
            var meteorScale = Tools.GetNumFromRange(1, 100) / 100f * 0.8f + 0.2f;
            meteorEff.transform.localScale = new Vector3(meteorScale, meteorScale, meteorScale);

            var tmpColor = meteorEff.GetComponent<Image>().color;
            if (meteorScale < 0.6f)
            {
                tmpColor.a = Tools.GetNumFromRange(20, 50) / 100f;
            }
            else
            {
                tmpColor.a = Tools.GetNumFromRange(50, 80) / 100f;
            }
            meteorEff.GetComponent<Image>().color = tmpColor;

            var otherOffset = 100;
            var startX = 0f;
            var startY = 0f;
            var isTop = Tools.GetNumFromRange(1, 2) == 1;
            if (isTop)
            {
                startX = Tools.GetNumFromRange(-(int) Constant.ScreenWidth / 4, (int) Constant.ScreenWidth / 2) + otherOffset; 
                startY = Constant.ScreenHeight / 2f + otherOffset;
            }
            else
            {
                startX = Constant.ScreenWidth / 2f + otherOffset;
                startY = Tools.GetNumFromRange(-(int) Constant.ScreenHeight / 4, (int) Constant.ScreenHeight / 2) + otherOffset;
            }

            var offsetX = startX - -Constant.ScreenWidth / 2 + otherOffset;
            var offsetY = offsetX;

            var destX = startX - offsetX;
            var destY = startY - offsetY;

            var flyTime = Mathf.Abs(offsetX) / Tools.GetNumFromRange(100, 300);
            
            meteorEff.transform.localPosition = new Vector2(startX , startY);
            meteorEff.transform.DOLocalMove(new Vector3(destX, destY, 0), flyTime).SetEase(Ease.Linear).OnComplete(ShowMeteorEff);
        }
    }
}
