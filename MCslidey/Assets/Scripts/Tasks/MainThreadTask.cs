/********************************************************************************************
    BlowFire Framework
    Module: Core/Tasks
    Author: HU QIWEI
********************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


namespace BFF.Tasks
{
    [DefaultExecutionOrder(-100)]
    internal class MainThreadExecutorComponent: MonoBehaviour
    {
        static MainThreadExecutorComponent instance = null;
        List<ActionRunner> taskList = new List<ActionRunner>();
        List<ActionRunner> lateTaskList = new List<ActionRunner>();

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        private void Update()
        {
            lock(taskList)
            {
                while (taskList.Count > 0)
                {
                    ActionRunner runner = taskList[0];
                    taskList.RemoveAt(0);

                    try
                    {
                        runner.Run();
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            lock (lateTaskList)
            {
                while (lateTaskList.Count > 0)
                {
                    ActionRunner runner = lateTaskList[0];
                    lateTaskList.RemoveAt(0);

                    try
                    {
                        runner.Run();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        public void AddTask(ActionRunner runner, bool ignoreIfExists)
        {
            lock(taskList)
            {
                if (ignoreIfExists)
                {
                    for (int i = 0; i < taskList.Count; i++)
                        if (taskList[i].action == runner.action && taskList[i].runnable == runner.runnable)
                            return;
                }
                taskList.Add(runner);
            }
        }

        public void AddTaskToLateUpdate(ActionRunner runner, bool ignoreIfExists)
        {
            lock (lateTaskList)
            {
                if (ignoreIfExists)
                {
                    for (int i = 0; i < lateTaskList.Count; i++)
                        if (lateTaskList[i].action == runner.action && lateTaskList[i].runnable == runner.runnable)
                            return;
                }
                lateTaskList.Add(runner);
            }
        }
    }

    /// <summary>
    /// schedule a task on main thread
    /// </summary>
    public static class MainThreadTask
	{
        static GameObject mainThreadRunnerObject;
        static MainThreadExecutorComponent executor;

        private static Thread mainThread;

        static MainThreadTask()
        {
            Thread currentThread = Thread.CurrentThread;
            if (currentThread.ManagedThreadId > 1 || currentThread.IsThreadPoolThread)
                throw new Exception("Initialize the class on the main thread, please!");
            mainThread = currentThread;

            mainThreadRunnerObject = Utils.UtilityGameObject();
            executor = mainThreadRunnerObject.AddComponent<MainThreadExecutorComponent>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (mainThread == null)
                Debug.Log("Init MainThreadTask");   // this won't happen
        }

        /// <summary>
        /// check if current thread is main thread
        /// </summary>
        /// <returns>true if we are in main thread</returns>
        public static bool IsMainThread() { return Thread.CurrentThread == mainThread; }

        /// <summary>
        /// run a task on main thread and wait for return
        /// </summary>
        /// <param name="action">action</param>
        public static void RunTask(System.Action action)
        {
            if (IsMainThread())
                action.Invoke();
            else
            {
                ActionRunner runner = new ActionRunner(action, null, true);
                executor.AddTask(runner, false);
                runner.Wait();
            }            
        }

        /// <summary>
        /// add a task to run on main thread (Update)
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTask(System.Action action, bool ignoreIfExists = false)
        {
            executor.AddTask(new ActionRunner(action, null), ignoreIfExists);
        }

        /// <summary>
        /// add a task to run on main thread (LateUpdate)
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTaskToLateUpdate(System.Action action, bool ignoreIfExists = false)
        {
            executor.AddTaskToLateUpdate(new ActionRunner(action, null), ignoreIfExists);
        }

        /// <summary>
        /// Run a task on main thread and wait for return
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        public static void RunTask(IRunnable runnable)
        {
            if (IsMainThread())
                runnable.Run();
            else
            {
                ActionRunner runner = new ActionRunner(null, runnable, true);
                executor.AddTask(runner, false);
                runner.Wait();
            }
        }

        /// <summary>
        /// Add a task on main thread (Update)
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTask(IRunnable runnable, bool ignoreIfExists = false)
        {
            executor.AddTask(new ActionRunner(null, runnable), ignoreIfExists);
        }

        /// <summary>
        /// Add a task on main thread (LateUpdate)
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTaskToLateUpdate(IRunnable runnable, bool ignoreIfExists = false)
        {
            executor.AddTaskToLateUpdate(new ActionRunner(null, runnable), ignoreIfExists);
        }
    }
}
