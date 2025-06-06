//using System;
//using UnityEngine;
//using UnityEngine.UI;
//
//public  class Upgrade : MonoBehaviour
//{
//    public  Button OKbutton1;
//    public  GameObject upgradewindow1;
//    public  Button OKbutton2;
//    public  GameObject upgradewindow2;
//    public  Button cancelbutton2;
//
//    private void OnEnable()
//    {
//        this.upgradewindow1.SetActive(false);
//        this.upgradewindow2.SetActive(false);
//    }
//
//    private void Start()
//    {
//        this.UpgradeVersion();
//    }
//
//
//    private bool UpgradeVersion()
//    {
//        if (IsUpgrade())
//        {
//            ShowPopup();
//            return false;
//        }
//        else
//        {
//            return true;
//        }
//    }
//
//    private void ShowPopup()
//    {
//        if (UpgradeNow())
//        {
//            mustupgrade();
//        }
//        else
//        {
//            recommendupgrade();
//        }
//    }
//    
//    //比较版本，当前版本低就提示更新
//    private  bool IsUpgrade()
//    {
//        
//        string specifiedVersion = PlatformBridge.getConfigString("Application.CheckVersion.Version", "1.0.0");
//
//        string currentVersion =  Application.version;
//        
//        if (string.Compare(currentVersion, specifiedVersion) < 0)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//    //比较时间，当前时间晚于规定时间就必须更新
//    private  bool UpgradeNow()
//    {
//        string specifiedTime = PlatformBridge.getConfigString("Application.CheckVersion.EndTime", "2023-2-2");
//        string currentTime = DateTime.Now.ToString("yyyy-M-d");
//        if (string.Compare(currentTime, specifiedTime) < 0)
//        {
//            return false;
//        }
//        else
//        {
//            return true;
//        }
//    }
//    private  void mustupgrade()
//    {
//        //弹出只有确定按钮的窗口
//        upgradewindow1.SetActive(true);
//        OKbutton1.onClick.AddListener(() =>
//        {
//            startupgrade();
//            //upgradewindow1.SetActive(false);
//            //开始更新游戏
//        });
//    }
//    private void recommendupgrade()
//    {
//        //弹出两个按钮的窗口
//        upgradewindow2.SetActive(true);
//        OKbutton2.onClick.AddListener(() =>
//        {
//            //upgradewindow2.SetActive(false);
//            startupgrade();
//            //开始更新游戏
//        });
//        cancelbutton2.onClick.AddListener(() =>
//        {
//            upgradewindow2.SetActive(false);
////                FindObjectOfType<UILoading>().ContinueGame();
//            //返回loading场景
//        });
//    }
//    private void startupgrade()
//    {
//    }
//}
