using Other;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace UI
{
    public class Man : MonoBehaviour
    {
        private int _daijiCount;
        private bool _isDoing;
        
        // Start is called before the first frame update
        public void StartShow()
        {
            GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate(TrackEntry entry)
            {
                if (entry.ToString() != "")
                {
                    if (entry.ToString() == "xinfen1")
                    {
                        _isDoing = false;
                    }
                    
                    ShowDaiji();
                }
            };
            
            ShowDaiji();
        }

        public void ShowKun()
        {
            ShowAnimByName("kun");
        }
        
        public void ShowNewBest()
        {
            ShowAnimByName("newbest");
        }

        public void ShowJuSang()
        {
            ShowAnimByName("jusang");
        }

        public void ShowXingFen()
        {
//            ShowAnimByName(Tools.GetNumFromRange(1, 2) == 1 ? "xingfen" : "xinfen1");
            ShowAnimByName("xinfen1");
            _isDoing = true;
        }
        
        private void ShowDaiji()
        {
            ++_daijiCount;
            var daijiIndex = Tools.GetNumFromRange(2, 3);
            if (_daijiCount == 10)
            {
                daijiIndex = 4;
            }
            ShowAnimByName("daiji" + daijiIndex);
        }

        private void ShowAnimByName(string animName)
        {
            if (_isDoing)
            {
                return;
            }

            if (GetComponent<SkeletonGraphic>() != null && GetComponent<SkeletonGraphic>().AnimationState != null)
            {
                GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, animName, false);
            }
            else
            {
                if (animName == "xinfen1")
                {
                    _isDoing = false;
                }
            }
        }
    }
}
