using System;
using System.Collections.Generic;
using System.Threading;
using Other;
using UnityEngine;

namespace BFF.Lib
{

    public class BFFLoop
    {
        private List<Action> _messageQueues;
        private ManualResetEvent _semEvent;

        private static BFFLoop _mainLoop;
        public static BFFLoop GetMainLoop()
        {
            if (_mainLoop == null)
            {
                _mainLoop = new BFFLoop();
            }
            return _mainLoop;
        }

        public BFFLoop()
        {
            _semEvent = new ManualResetEvent(false);
            _messageQueues = new List<Action>();
        }

        public void AddAction(Action action)
        {
            lock (_messageQueues)
            {
                _messageQueues.Add(action);
            }
            this._semEvent.Set();
        }

        public void prepare()
        {
            _semEvent.WaitOne();
            _semEvent.Reset();
        }

        public bool Execute()
        {
            Action callback;
            lock (_messageQueues)
            {

                if (_messageQueues.Count == 0)
                {
                    return false;
                }
                callback = _messageQueues[0];
                _messageQueues.RemoveAt(0);
            }

            try
            {
                callback();
            }
            catch (Exception e)
            {
                DebugEx.LogError($"temp_error LoopThread Execute Failed : {e.Message}");
                DebugEx.LogError(e.Message + "\n" + e.StackTrace);
            }
            
            return true;
        }
    }

    public class BFFLoopThread
    {
        public int ThreadId;
        
        private bool _needExit;
        public BFFHandler Handler;
        private BFFLoop _loop;
        private Thread _thread;
        
        public void Start(string threadName = null)
        {
            _needExit = false;
            _loop = new BFFLoop();
            Handler = new BFFHandler(_loop);
            _thread = new Thread(Run);
            if (!string.IsNullOrEmpty(threadName))
            {
                _thread.Name = threadName;
            }
            _thread.Start();
        }

        public void Exit()
        {
            _needExit = true;
            if (_thread != null)
            {
                _thread.Abort();
            }
        }
        

        void Run()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            while (!_needExit)
            {
                _loop.prepare();
                while (true)
                {
                    try
                    {
                        if (!_loop.Execute())
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        DebugEx.LogError($"temp_error Looper里产生了未捕获异常，但我们仍需继续Loop : {e.Message}");
                        DebugEx.LogError("Looper里产生了未捕获异常，但我们仍需继续Loop" + e.Message + "\n" + e.StackTrace);
                    }
                }
            }
        }
    }
    
    public class BFFHandler
    {
        private BFFLoop _loop;

        public BFFHandler(BFFLoop mnLoop)
        {
            _loop = mnLoop;
        }

        public void Post(Action action)
        {
           _loop.AddAction(action);
        }
    }
}