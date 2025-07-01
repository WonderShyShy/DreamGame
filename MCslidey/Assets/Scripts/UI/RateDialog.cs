// using System;
// using System.Collections;
// using DG.Tweening;
// using Manager;
// using Models;
// using Other;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace UI
// {
//     public class RateDialog : MonoBehaviour
//     {
//         public GameObject[] stars;
//         public Image emoji;
//         public Sprite[] emojiSpriteFrames;
//         public GameObject hand;
//         
//         private int _starNum = 0;
//         // Start is called before the first frame update
//         void Start()
//         {
//             HandRunAnimation();
//             
//             Player.AddShowRateCount();
//         }
//         
//         void HandRunAnimation()
//         {
//             var originalHandX = hand.transform.localPosition.x;
//             var offsetX = Math.Abs(stars[0].transform.localPosition.x) * 2;
//             hand.transform.DOLocalMoveX(originalHandX + offsetX, 1.3f).OnComplete(() =>
//             {
//                 hand.transform.localPosition = new Vector2(originalHandX, hand.transform.localPosition.y);
//             }).SetLoops(-1);
//         }
//
//         
//         void CheckStar(float posX)
//         {
//             _starNum = 1;
//             for (var i = 1; i < stars.Length; ++i)
//             {
//                 if (posX >= (stars[i].transform.localPosition.x -
//                              stars[i].GetComponent<RectTransform>().sizeDelta.x / 2 - 20))
//                 {
//                     stars[i].SetActive(true);
//                     _starNum = i + 1;
//                 }
//                 else
//                 {
//                     stars[i].SetActive(false);
//                 }
//             }
//
//             emoji.sprite = emojiSpriteFrames[_starNum - 1];
//             emoji.SetNativeSize();
//         }
//
//         public void OnBtnClk(string btnType)
//         {
//             DebugEx.Log(btnType);
//             switch (btnType)
//             {
//                 case "rate":
//                     if (hand.activeInHierarchy)
//                     {
//                         Debug.LogError("Please complete the rating!");
//                         return;
//                     }
//
//                     if (!Constant.AFData.rate_4_over && _starNum >= 4)
//                     {
//                         Constant.AFData.rate_4_over = true;
//                         ManagerLocalData.SetTableData(ManagerLocalData.AF, Constant.AFData);
//                         ManagerLocalData.SaveData();
//                     }
//
//                     Player.SetAlreadyRate();
//                     if (_starNum >= 5)
//                     {
//                     }
//                     else
//                     {
//                     }
//
//                     ManagerDialog.DestroyDialog("RateDialog");
//                     break;
//                 case "noThx":
//                     
//                     ManagerDialog.DestroyDialog("RateDialog");
//                     break;
//                 case "1":
//                 case "2":
//                 case "3":
//                 case "4":
//                 case "5":
//                     hand.SetActive(false);
//                     CheckStar(stars[int.Parse(btnType) - 1].transform.localPosition.x);
//                     break;
//
//             }
//         }
//         
//         private void Update()
//         {
//             if (hand.activeInHierarchy)
//             {
//                 CheckStar(hand.transform.localPosition.x);
//             }
//         }
//
//     }
// }
