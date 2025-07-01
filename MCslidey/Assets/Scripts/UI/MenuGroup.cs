using System.Collections;
using DG.Tweening;
using Manager;
using Models;
using Other;
using UnityEngine;

namespace UI
{
    public class MenuGroup : MonoBehaviour
    {
        public GameObject btnSettingOpen;
        public GameObject btnSettingClose;
        public GameObject btnMusicGroup;
        public GameObject btnMusicOpen;
        public GameObject btnMusicClose;
        public GameObject btnSoundGroup;
        public GameObject btnSoundOpen;
        public GameObject btnSoundClose;
        public GameObject btnVibrateGroup;
        public GameObject btnVibrateOpen;
        public GameObject btnVibrateClose;
        public GameObject btnRestart;
        public GameObject groupMenu;
        public GameObject groupScore;
        public GameObject btnPrivacy;

        private bool _isAnimating;
        private float _originalX;
        private float _moveOffsetX;
        private float _originalXGroupScore = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH,"dontShow");
            Debug.Log("SettingDialog" + ManagerLocalData.GetStringData(ManagerLocalData.VIBRATE_SWITCH));
            
            //震动默认开关
            if (!ManagerLocalData.HaveData(ManagerLocalData.VIBRATE_SWITCH))
            {
                ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "on");
            }

            if (Constant.SceneVersion != "3")
            {
                if (!Constant.VibratorSwitch)
                {
                    btnVibrateGroup.SetActive(false);
                }
            
                _originalX = btnSettingOpen.transform.localPosition.x;
                _moveOffsetX = _originalX * 2;

                _originalXGroupScore = groupScore.transform.localPosition.x;
            
                ResetBtnStatus();
                HideMenuGroup();
            }
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

        private void sendClickSettingAF()
        {
            if (Constant.AFData.setting_click)
            {
                return;
            }

            
            Constant.AFData.setting_click = true;
            ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);

        }
        

        /// <summary>
        /// SettingsDialog 中也有一个方法
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

        public void OnBtnClk(string btnType)
        {
            if (Player.IsInGuide())
            {
                return;
            }
            
            DebugEx.Log(btnType);
            switch (btnType)
            {
                case "setting_open":
                    
                    sendClickSettingAF();
                    
                    if (Constant.SceneVersion == "3")
                    {
                        if (!Player.UserCanMove) return;
                        Constant.GamePlayScript.ShowSettingsDialog();
                    }
                    else
                    {
                        ShowMenuGroup();
                    }
                    
//                    OnBtnClk("delAllData");
                    break;
                case "setting_close":
                    HideMenuGroup();
                    break;
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
                case "restart":
                    if (!Player.UserCanMove) return;
                    Constant.GamePlayScript.RestartGame(true);
                    AutoCloseMenuGroup();
                    sendReplayClickAF();
                  
                    break;
                case "vibrate_open":
                    ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "on");
                    ResetBtnStatus();
                    break;
                case "vibrate_close":
                    ManagerLocalData.SetStringData(ManagerLocalData.VIBRATE_SWITCH, "off");
                    ResetBtnStatus();
                    break;
                case "privacy":
                    break;
                case "delAllData":
                    ManagerLocalData.DeleteAll();
                    ManagerLocalData.SaveData();
                    break;
                case "addScore":
                    Player.SetCurScore(100000);
                    break;
            }
            
            ManagerAudio.PlaySound("sound_ButtonDown");
        }

        public void AutoCloseMenuGroup()
        {
            if (Constant.SceneVersion == "3") return;
            
            if (!groupMenu.activeInHierarchy)
            {
                return;
            }
            HideMenuGroup();
        }

        void ShowMenuGroup()
        {
            if (_isAnimating)
            {
                return;
            }

            _isAnimating = true;
            
            groupMenu.SetActive(true);
            groupMenu.GetComponent<CanvasGroup>().DOFade(1, 0.15f);

            if (Constant.VibratorSwitch)
            {
                btnPrivacy.transform.DOLocalMoveX(_originalX - _moveOffsetX, 0.3f).SetEase(Ease.OutCubic);
                btnMusicGroup.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 5, 0.3f).SetEase(Ease.OutCubic);
                btnSoundGroup.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 5 * 2, 0.3f).SetEase(Ease.OutCubic);
                btnVibrateGroup.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 5 * 3, 0.3f).SetEase(Ease.OutCubic);
                btnRestart.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 5 * 4, 0.3f).SetEase(Ease.OutCubic).OnComplete(
                    () =>
                    {
                        _isAnimating = false;
                    });
            }
            else
            {
                btnPrivacy.transform.DOLocalMoveX(_originalX - _moveOffsetX, 0.3f).SetEase(Ease.OutCubic);
                btnMusicGroup.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 4, 0.3f).SetEase(Ease.OutCubic);
                btnSoundGroup.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 4 * 2, 0.3f).SetEase(Ease.OutCubic);
                btnRestart.transform.DOLocalMoveX(_originalX - _moveOffsetX + _moveOffsetX / 4 * 3, 0.3f).SetEase(Ease.OutCubic).OnComplete(
                    () =>
                    {
                        _isAnimating = false;
                    });
            }
            
            btnSettingOpen.SetActive(false);
            btnSettingClose.SetActive(true);

            groupScore.transform.DOLocalMoveX(-Constant.ScreenWidth * 1.5f, 0.2f);
        }

        void HideMenuGroup()
        {
            if (_isAnimating)
            {
                return;
            }
            
            _isAnimating = true;

            btnPrivacy.transform.DOLocalMoveX(_originalX, 0.2f).SetEase(Ease.OutCubic);
            btnMusicGroup.transform.DOLocalMoveX(_originalX, 0.2f).SetEase(Ease.OutCubic);
            btnSoundGroup.transform.DOLocalMoveX(_originalX, 0.2f).SetEase(Ease.OutCubic);
            btnRestart.transform.DOLocalMoveX(_originalX, 0.2f).SetEase(Ease.OutCubic);

            if (Constant.VibratorSwitch)
            {
                btnVibrateGroup.transform.DOLocalMoveX(_originalX, 0.2f).SetEase(Ease.OutCubic);
            }

            groupMenu.GetComponent<CanvasGroup>().DOFade(0, 0.21f).OnComplete(() =>
            {
                groupMenu.SetActive(false);
                _isAnimating = false;
            });
            
            btnSettingOpen.SetActive(true);
            btnSettingClose.SetActive(false);

            groupScore.transform.DOLocalMoveX(_originalXGroupScore, 0.2f);
        }
    }
}
