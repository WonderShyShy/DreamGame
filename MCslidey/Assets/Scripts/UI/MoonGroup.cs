using DG.Tweening;
using Models;
using UnityEngine;

namespace UI
{
    public class MoonGroup : MonoBehaviour
    {
        public GameObject moon;
    
        // Start is called before the first frame update
        void Start()
        {
            var t = 240;
            if (Constant.SceneVersion == "3")
            {
                t = 40;
            }
            moon.transform.DORotate(new Vector3(0, 0, -360), t).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}
