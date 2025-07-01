using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Manager;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Other
{
    public static class Tools
    {
        private static readonly Random Ran = new Random();

        public static double OneDayMilliseconds = 24 * 3600 * 1000;

        /// <summary>
        /// 四舍五入, 在x.5时取离0远的那个数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int ChinaRound(float d)
        {
            return (int)Math.Round(d, MidpointRounding.AwayFromZero);
        }

        public static int GetNumFromRange(int min, int max)
        {
            return Ran.Next(min, max + 1);
        }


        public static long GetCurrentTimeMillis()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        public static List<T> RandomSortList<T>(List<T> listT)
        {
            var random = new Random();
            var newList = new List<T>();
            foreach (var item in listT)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }


        /// <summary>
        /// 相差天数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int DiffDays(DateTime startTime, DateTime endTime)
        {
            TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return (int)daysSpan.TotalDays;
        }


        public static bool IsToday(long timeStamp)
        {
            var today = DateTime.Now.ToString("yyyy/MM/dd");

            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区

            var dt = startTime.AddMilliseconds(timeStamp);
            var day = dt.ToString("yyyy/MM/dd");

            var todayArr = today.Split('/');
            var dayArr = day.Split('/');

            for (var i = 0; i < todayArr.Length; ++i)
            {
                if (todayArr[i] != dayArr[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static void NumChange(TextMeshProUGUI label, int endNum, float time = 0.5f, Action changeCallback = null, bool isToStandard = false, Action endCallback = null)
        {
            var startNum = int.Parse(label.text.Replace(",", ""));
            var num = endNum - startNum;

            if (Constant.LevelProgressVersion == "1")
                if (num < 0)
                {
                    num = endNum;
                }
            if (num != 0)
            {
                var seq = DOTween.Sequence();
                seq.Append(DOTween.To(delegate (float value)
                {
                    value = (float)Math.Floor(value);
                    if (isToStandard)
                    {
                        label.text = NumToStandard((int)value);
                    }
                    else
                    {
                        label.text = ((int)value).ToString();
                    }
                    changeCallback?.Invoke();
                }, startNum, endNum, time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    endCallback?.Invoke();
                }));
                seq.SetUpdate(true);
            }
        }

        public static string NumToStandard(int num)
        {
            return num.ToString("N0");
        }

        public static void DoVibrator(int time, int amplitude)
        {
           
        }

        public static void SetImgOpacity(Image img, int opacityNum = 0)
        {
            var tmpColor = img.color;
            tmpColor.a = opacityNum;
            img.color = tmpColor;
        }
        
//        public static async Task<T> LoadAssetAsync<T>(string assetPath)
//        {
//            var result = await Addressables.LoadAssetAsync<T>(assetPath).Task;
//            return result;
//        }

    }

    public static class Delay
    {
        public static IEnumerator Run(Action action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            action();
        }
    }
}
