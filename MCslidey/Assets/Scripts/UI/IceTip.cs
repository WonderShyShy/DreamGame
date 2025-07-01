using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class IceTip : MonoBehaviour
    {
        public GameObject iceFrame;
        public GameObject arrow;
        public GameObject tip;

        public void UpdateTip(GameObject blockItem)
        {
            var itemSize = blockItem.GetComponent<RectTransform>().sizeDelta;
            var pos = blockItem.transform.localPosition + new Vector3(itemSize.x / 2f, itemSize.y / 2f, 0);
            iceFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSize.x + 10, itemSize.y + 10);
            iceFrame.transform.localPosition = pos;
            iceFrame.transform.localScale = Vector2.one;

            var arrowScaleX = arrow.transform.localScale.x;
            var arrowScaleY = arrow.transform.localScale.y;
            var arrowPosY = arrow.transform.localPosition.y;
            var tipPosY = tip.transform.localPosition.y;

            if (iceFrame.transform.localPosition.x < 0)
            {
                arrowScaleX = -arrowScaleX;
            }

            if (iceFrame.transform.localPosition.y < -100)
            {
                arrowPosY = -arrowPosY;
                arrowScaleY = -arrowScaleY;
                tipPosY = pos.y + Math.Abs(tipPosY);
            }
            else
            {
                tipPosY = pos.y - Math.Abs(tipPosY);
            }

            arrow.transform.localScale = new Vector2(arrowScaleX, arrowScaleY);
            arrow.transform.localPosition = new Vector2(0, arrowPosY);
            tip.transform.localPosition = new Vector2(0, tipPosY);
        }

        public void UpdateText(string text)
        {
            tip.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void OnMaskClk()
        {
            Destroy(gameObject);
        }
    }
}
