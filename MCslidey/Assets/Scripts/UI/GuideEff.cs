using System;
using DG.Tweening;
using Models;
using UnityEngine;

namespace UI
{
    public class GuideEff : MonoBehaviour
    {
        public GameObject guideMask;
        public GameObject hand;
        public GameObject dstTip;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void UpdateGuidePos(int[] guideData)
        {
            DOTween.Kill("guideHand");

            var startIndex = guideData[0];
            var endIndex = guideData[1];
            
            var blockLength = Blocks.GetBlockDataByIndex(startIndex)[(int)Blocks.Key.Length];
            var moveBlockIndexHang = Blocks.GetHangByPos(startIndex);
            var handStartPosX = Constant.BlockGroupEdgeLeft + (Blocks.GetLieByPos(startIndex) + blockLength / 2f) * Constant.BlockWidth;
            var moveX = Blocks.GetLieByPos(endIndex) - Blocks.GetLieByPos(startIndex);
            var downY = Blocks.GetHangByPos(startIndex) - Blocks.GetHangByPos(endIndex);
            
            //mask设置
            guideMask.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * Constant.Lie, Constant.BlockHeight * Blocks.GetHangNum());
            guideMask.transform.localPosition = new Vector2(0, Constant.BlockGroupEdgeBottom + Constant.BlockHeight * Blocks.GetHangNum() / 2f);

            //手动画设置
            hand.transform.localPosition = new Vector2(handStartPosX, Constant.BlockGroupEdgeBottom + moveBlockIndexHang * Constant.BlockHeight + Constant.BlockHeight / 2);
            hand.transform.localScale = moveX < 0 ? new Vector2(-1, 1) : Vector2.one;
            
            var handEndPosX = handStartPosX + Constant.BlockWidth * moveX;
            var seq = DOTween.Sequence();
            seq.Append(hand.transform.DOLocalMoveX(handEndPosX, Constant.GuideOneBlockTime * Math.Abs(moveX))
                .SetDelay(Constant.GuideEdgeWaitTime));
            seq.Append(hand.transform.DOLocalMoveX(handStartPosX, Constant.GuideOneBlockTime * Math.Abs(moveX) / 2.0f)
                .SetDelay(Constant.GuideEdgeWaitTime));

            seq.SetLoops(-1);
            seq.SetId("guideHand");
            seq.SetUpdate(true);
            
            //目标位置设置
            dstTip.GetComponent<RectTransform>().sizeDelta = new Vector2(Constant.BlockWidth * blockLength, Constant.BlockHeight);
            dstTip.transform.localPosition = new Vector2(handEndPosX, hand.transform.localPosition.y - downY * Constant.BlockHeight);
        }
    }
}
