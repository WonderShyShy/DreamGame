using System.Collections;
using Manager;
using Models;
using Other;
using UnityEngine;

namespace UI
{
    public class SettingsDialog : MonoBehaviour
    {
        public GameObject btnMusicOpen;
        public GameObject btnMusicClose;
        public GameObject btnSoundOpen;
        public GameObject btnSoundClose;
        public GameObject btnVibrateOpen;
        public GameObject btnVibrateClose;
        public GameObject vibratorLine;
        public GameObject btnReplay;
        public GameObject btnPrivacy;
        public GameObject bG;
        
        private void OnDestroy()
        {
//            var topMask = GameObject.Find("topMask");
//            if (topMask != null)
//            {
//                topMask.GetComponent<TopMask>().ShowLight();
//            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
//            GameObject.Find("topMask").GetComponent<TopMask>().HideLight();
            
            ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH,"dontShow");
            Debug.Log("SettingDialog" + ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH));

            if (ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH) == "dontShow")
            {
                Debug.Log("SettingDialog1 " + ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH));
                vibratorLine.SetActive(false);
                Vector3 tem = new Vector3();
                tem = btnReplay.transform.localPosition;
                tem.y = -41.2f;
                btnReplay.transform.localPosition = tem;
                tem = btnPrivacy.transform.localPosition;
                tem.y = -203.2f;
                btnPrivacy.transform.localPosition = tem;
                bG.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1000);
            }
            else
            {
                Debug.Log("SettingDialog1 " + ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH));
                vibratorLine.SetActive(true);
                Vector3 tem = new Vector3();
                tem = btnReplay.transform.localPosition;
                tem.y = -162.2f;
                btnReplay.transform.localPosition = tem;
                tem = btnPrivacy.transform.localPosition;
                tem.y = -307.9f;
                btnPrivacy.transform.localPosition = tem;
                bG.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1099);
            }
            //震动默认开关
            if (!ManagerLocalData.HaveData(ManagerLocalData.VIBRATE_SWITCH))
            {
                ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "on");
            }
            ResetBtnStatus();
        }

        private void ResetBtnStatus()
        {
            btnMusicOpen.SetActive(!ManagerAudio.GetMusicEnabled());
            btnMusicClose.SetActive(ManagerAudio.GetMusicEnabled());
            
            btnSoundOpen.SetActive(!ManagerAudio.GetSoundEnabled());
            btnSoundClose.SetActive(ManagerAudio.GetSoundEnabled());
            
            btnVibrateOpen.SetActive(ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH) == "off");
            btnVibrateClose.SetActive(ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH) == "on");
        }
        
        /// <summary>
        /// MenuGroup 中也有一个方法
        /// </summary>
        private void sendReplayClickAF()
        {
            if (Constant.AFData.setting_replay_click)
            {
                return;
            }
            
            Constant.AFData.setting_replay_click = true;
            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
        }
        

        public void OnBtnClk(string param)
        {
            DebugEx.Log(param);
            switch (param)
            {
                case "music_open":
                    ManagerAudio.SetMusicOn();
                    ResetBtnStatus();
                    break;
                case "music_close":
                    ManagerAudio.SetMusicOff();
                    ResetBtnStatus();
                    break;
                case "sound_open":
                    ManagerAudio.SetSoundOn();
                    ResetBtnStatus();
                    break;
                case "sound_close":
                    ManagerAudio.SetSoundOff();
                    ResetBtnStatus();
                    break;
                case "vibrate_open":
                    ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "on");
                    ResetBtnStatus();
                    break;
                case "vibrate_close":
                    ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "off");
                    ResetBtnStatus();
                    break;
                case "restart":
                    if (!Player.UserCanMove) return;
                    Constant.GamePlayScript.RestartGame(true);
                    sendReplayClickAF();
                    OnBtnClk("close");
                    break;
                case "privacy":
                    Application.OpenURL("http://polarisgamestudio.epizy.com/policy.html");
                    break;
                case "close":
                    ManagerDialog.DestroyDialog("SettingsDialog");
                    break;
            }
        }
    }
}
