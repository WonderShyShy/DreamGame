using DG.Tweening;
using Other;
using UnityEngine;

namespace UI
{
    public class TopMask : MonoBehaviour
    {
        public GameObject lightEff;

        private Vector3 _originalLightEffPos;
        private float _intervalTime = 20;
        private float _moveTime = 3;
        private bool _isHiding;
        private float _showWaitTime = 5f;
        private bool _showWaitTimeOver = true;
        
        // Start is called before the first frame update
        public void StartShow()
        {
            _originalLightEffPos = lightEff.transform.position;
            ShowLightAnim();
        }

        private void ShowLightAnim()
        {
            if (_isHiding || !_showWaitTimeOver)
            {
                DebugEx.Log("不播放溜光");
            }
            else
            {
                lightEff.transform.position = _originalLightEffPos;

                lightEff.transform.GetComponent<SpriteRenderer>().DOFade(100 / 255f, 1);
                lightEff.transform.GetComponent<SpriteRenderer>().DOFade(0, 1).SetDelay(2);
        
                lightEff.transform.DOMoveX(-_originalLightEffPos.x, _moveTime).SetEase(Ease.Linear);
            }
            
            StartCoroutine(Delay.Run(ShowLightAnim, _intervalTime));
        }

        public void HideLight()
        {
            _isHiding = true;
            lightEff.SetActive(false);
        }

        public void ShowLight()
        {
            _isHiding = false;
            lightEff.SetActive(true);

            _showWaitTimeOver = false;
            StartCoroutine(Delay.Run(() => { _showWaitTimeOver = true; }, _showWaitTime));
        }
    }
}
