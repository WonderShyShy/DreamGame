using Manager;
using Models;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SceneInit : MonoBehaviour
    {
        //出现空格权重
        public int emptyBlock1 = 6;
        public int emptyBlock2 = 3;
        public int emptyBlock3 = 2;
        
        //场景版本
        public string sceneVersion;
        
        // Start is called before the first frame update
        void Awake()
        {
            Application.targetFrameRate = Constant.FrameRate;
            Constant.InitData();
            Constant.SceneVersion = sceneVersion;
            Constant.EffCtrlScript.LoadResAsync_manRes();
            
            if (!Blocks.IsTesting())
            {
                UpdateServerData();
            }
            
            ManagerLocalData.InitData();
            Player.InitData();
            ManagerAudio.InitData(Constant.GamePlayScript.gameObject);
            ManagerDialog.InitData(Constant.SceneVersion == "3" ? GameObject.Find("CanvasDialogGroup") : gameObject);
            ManagerAd.InitData();
            
            SetBlockGroup();
            UpdateDragDelta();
            SetDifficulty();
            SetLevelText();

            if (Constant.SceneVersion == "3")
            {
                Constant.AchievementSwitch = false;
                Constant.SpecialRainbowSwitch = false;
                Constant.StoneSwitch = false;
                Constant.LevelUpEffVersion = "3";
                Constant.SpecialGoldClearTime = 0f;
            } else if (Constant.SceneVersion == "2")
            {
                Constant.AchievementSwitch = true;
                Constant.LevelUpEffVersion = "1";
            }

//#if UNITY_EDITOR
//            Constant.IceBlockSwitch = true;
//            Constant.SpecialGoldSwitch = true;
//            Constant.SpecialBronzeSwitch = true;
//            Constant.SpecialGoldEffWeight = "1:0:0";
//            Constant.SecondChanceEnabled = true;
//            Constant.LevelProgressVersion = "1";
//            Constant.ShowRemoveAd = true;
//            Constant.ShowSpecialGoldDialogAnim = true;
//#endif
        }

        void SetLevelText()
        {
            if (Constant.LevelToStageSwitch)
            {
                Constant.LevelText = "Stage ";
                Constant.LevelUpText = "STAGE ";
            }
        }

        void SetBlockGroup()
        {
            if (Constant.SceneVersion == "1" || Constant.SceneVersion == "2" || Constant.SceneVersion == "3")
            {
                Constant.UpdateBlockSize(116);

                if (!Constant.LevelToStageSwitch)
                {
                    Constant.LevelText = "Lv.";

                    if (Constant.SceneVersion == "3")
                    {
                        Constant.LevelText = "Lv. ";
                    }
                }
            }
        }

        void SetDifficulty()
        {
            //空格概率初始化
            var sum = emptyBlock1 + emptyBlock2 + emptyBlock3;
            Constant.EmptyBlockRate[0] = new[] {0, Tools.ChinaRound(emptyBlock1 * 1.0f / sum * 100)};
            Constant.EmptyBlockRate[1] = new[] {Tools.ChinaRound(emptyBlock1 * 1.0f / sum * 100), Tools.ChinaRound((emptyBlock1 + emptyBlock2) * 1.0f / sum * 100)};
            Constant.EmptyBlockRate[2] = new[] {Tools.ChinaRound((emptyBlock1 + emptyBlock2) * 1.0f / sum * 100), 100};
        }

        void UpdateDragDelta()
        {
            var designWidth = (int) GetComponent<CanvasScaler>().referenceResolution.x;
            var designHeight = (int) GetComponent<CanvasScaler>().referenceResolution.y;

            //修改分辨率
            var canvasScaler = GetComponent<CanvasScaler>();
            if (Screen.width * 1f / Screen.height > designWidth * 1f / designHeight)
            {
                DebugEx.Log("修改适配方案，改为高适配！");
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                var design = designHeight * 1f / designWidth;
                var fact = Screen.height * 1f / Screen.width;
                if (fact > (design + 0.1f))
                {
                    Constant.IsDeviceSoHeight = true;
                }
            }
            
            //宽适配
            var matchMode = canvasScaler.matchWidthOrHeight;
            if (matchMode < 0.5f)
            {
                Constant.DragDelta = designWidth * 1.0f / Screen.width;
            }
            //高适配
            if (matchMode >= 0.5f)
            {
                Constant.DragDelta = designHeight * 1.0f / Screen.height;
            }

            Constant.ScreenWidth = Screen.width * Constant.DragDelta;
            Constant.ScreenHeight = Screen.height * Constant.DragDelta;
        }

        void UpdateServerData()
        {

        }
    }
}
