using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StudioCore.Editor
{
    public class TaskManager
    {
        private static volatile ConcurrentDictionary<string, (bool, Task)> _liveTasks = new ConcurrentDictionary<string, (bool, Task)>();
        private static int _anonIndex = 0;

        public class LiveTask
        {
            public string TaskId;
            public bool Wait;
            public bool CanRequeue;
            public bool SilentFail;
            public TaskLogs.LogPriority LogPriority;
            public Action TaskAction;

            public LiveTask() { }

            public LiveTask(string taskId, bool wait, bool canRequeue, bool silentFail, Action act)
            {
                TaskId = taskId;
                Wait = wait;
                CanRequeue = canRequeue;
                SilentFail = silentFail;
                LogPriority = TaskLogs.LogPriority.Normal;
                TaskAction = act;
            }

            public LiveTask(string taskId, bool wait, bool canRequeue, bool silentFail, TaskLogs.LogPriority logPriority, Action act)
            {
                TaskId = taskId;
                Wait = wait;
                CanRequeue = canRequeue;
                SilentFail = silentFail;
                LogPriority = logPriority;
                TaskAction = act;
            }
        }

        public static bool Run(LiveTask liveTask)
        {
            bool add = AddTask(liveTask);

            if (!add)
            {
                if (liveTask.Wait)
                {
                    (bool, Task) t;
                    if (_liveTasks.TryGetValue(liveTask.TaskId, out t))
                    {
                        t.Item2.Wait();
                        return AddTask(liveTask);                        
                    }
                }
                if (liveTask.CanRequeue)
                {
                    (bool, Task) t;
                    if (_liveTasks.TryGetValue(liveTask.TaskId, out t))
                    {
                        if (t.Item1 == false)
                            _liveTasks[liveTask.TaskId] = (true, t.Item2);
                    }
                    return true;
                }
            }

            return true;
        }

        private static bool AddTask(LiveTask liveTask)
        {
            if (liveTask.TaskId == null)
            {
                _anonIndex++;
                liveTask.TaskId = Thread.CurrentThread.Name+":"+_anonIndex;
            }

            Task t = new Task(() => {
                try
                {
                    liveTask.TaskAction.Invoke();
                    TaskLogs.AddLog($"Task Completed: {liveTask.TaskId}",
                        Microsoft.Extensions.Logging.LogLevel.Information,
                        liveTask.LogPriority);
                }
                catch (Exception e)
                {
                    if (liveTask.SilentFail)
                    {
                        TaskLogs.AddLog($"Task Failed: {liveTask.TaskId}",
                            Microsoft.Extensions.Logging.LogLevel.Error,
                            liveTask.LogPriority);
                    }
                    else
                    {
                        throw;
                    }
                }
                (bool, Task) old;
                _liveTasks.TryRemove(liveTask.TaskId, out old);
                if (old.Item1 == true)
                    AddTask(liveTask);
            });
            bool add = _liveTasks.TryAdd(liveTask.TaskId, (false, t));
            if (add)
                t.Start();
            return add;
        }

        public static void WaitAll()
        {
            while (_liveTasks.Count > 0)
            {
                var e = _liveTasks.GetEnumerator();
                e.MoveNext();
                e.Current.Value.Item2.Wait();
            }
        }

        public static List<string> GetLiveThreads()
        {
            return new List<string>(_liveTasks.Keys);
        }

        public static void ThrowTaskExceptions()
        {
            // Allows exceptions in tasks to be caught by crash handler.
            foreach (var task in _liveTasks)
            {
                var ex = task.Value.Item2.Exception;
                if (ex != null)
                {
                    throw ex;
                }
            }
        }
    }
}
