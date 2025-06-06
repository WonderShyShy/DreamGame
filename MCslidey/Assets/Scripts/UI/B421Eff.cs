using Manager;
using Models;
using Other;
using UnityEngine;

namespace UI
{
    public class B421Eff : MonoBehaviour
    {
        public void OnBtnClk(string btnType)
        {
            DebugEx.Log(btnType);

            if (Player.IsBlockMoving)
            {
                return;
            }
            
            switch (btnType)
            {
                case "ad":
                    if (!Player.UserCanMove) return;
                    
                    Constant.GameStatusData.b421_Clk = true;
                    Player.UserCanMove = false;
                    Constant.GamePlayScript.ResetClearTipTime();
                    Constant.EffCtrlScript.RemoveB421Eff();
                    ManagerAd.PlayRewardAd((int)ManagerAd.RewardVideoPlayType.B421);
                    Player.SaveGameStatusData();
                    ManagerLocalData.SaveData();
                    
                    //test
//                    Constant.GamePlayScript.B421Result(new Hashtable(){{"success", true}});
                    break;
            }
        }
    }
}
