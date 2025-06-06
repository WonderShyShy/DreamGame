using System;
using System.Collections;

namespace BFF
{
    public class AsyncThread
    {
        private const string TAG = "AsyncThread----------";
        private static FileRecordQueue _recFile;

        //存储游戏数据
        public static void AddTask(Action act)
        {
            GameDataLoopThread.Instance.AddTask(act);
        }
    }
}
