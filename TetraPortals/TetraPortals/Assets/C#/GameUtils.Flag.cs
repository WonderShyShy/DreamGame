using System;
using System.Collections;
using System.Collections.Generic;

public static partial class GameUtils 
{
    public static class BitFlag
    {
        //设置Int的二进制位【nIndex第几位】 = 【value: 10】

        
        public static int IntSetFlag(int data, int nIndex, bool value = true)
        {
            if (nIndex < 0 || nIndex >= 32)
            {
                return data;
            }
            if (value)
            {
                data |= 1 << nIndex;
            }
            else
            {
                data &= ~(1 << nIndex);
            }
            return data;
        }
        
        /// <summary>
        /// data的低bit位是不是1
        /// </summary>
        /// <param name="data">要取数的数据</param>
        /// <param name="bit">低几位</param>
        /// <returns></returns>
        public static bool GetLow(int data, int bit)
        {
            return Convert.ToBoolean((data >> bit) & 1);
        }
        
        /// <summary>
        /// 获得两个int的“与”操作的结果
        /// </summary>
        /// <param name="data1">参数1</param>
        /// <param name="data2">参数2</param>
        /// <returns></returns>
        public static int GetAnd(int data1, int data2)
        {
            return data1 & data2;
        }
        
        
        //获得int32位的分别第几位是1
        public static List<int> GetBitList(int value)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if (GetLow(value, i))
                {
                    list.Add(i);
                }
            }
            return list;
        }
    }
    
}
