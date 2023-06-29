using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StudioCore.Editor
{
    public class TaskManager
    {
        public static volatile ConcurrentDictionary<string, string> warningList = new ConcurrentDictionary<string, string>();
        private static volatile ConcurrentDictionary<string, (bool, Task)> _liveTasks = new ConcurrentDictionary<string, (bool, Task)>();
        private static int _anonIndex = 0;

        public static bool Run(string taskId, bool wait, bool canRequeue, bool silentFail, System.Action action)
        {
            bool add = AddTask(taskId, silentFail, action);

            if (!add)
            {
                if (wait)
                {
                    (bool, Task) t;
                    if (_liveTasks.TryGetValue(taskId, out t))
                    {
                        t.Item2.Wait();
                        return AddTask(taskId, silentFail, action);                        
                    }
                }
                if (canRequeue)
                {
                    (bool, Task) t;
                    if (_liveTasks.TryGetValue(taskId, out t))
                    {
                        if (t.Item1 == false)
                            _liveTasks[taskId] = (true, t.Item2);
                    }
                    return true;
                }
            }

            return true;
        }
        private static bool AddTask(string taskId, bool silentFail, System.Action action)
        {
            if (taskId == null)
            {
                _anonIndex++;
                taskId = Thread.CurrentThread.Name+":"+_anonIndex;
            }

            Task t = new Task(() => {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    if (silentFail)
                    {
                        warningList.TryAdd(taskId, ("An error has occurred in task " + taskId + ":\n" + e.Message).Replace("\0", "\\0"));
                    }
                    else
                    {
                        throw;
                    }
                }
                (bool, Task) old;
                _liveTasks.TryRemove(taskId, out old);
                if (old.Item1 == true)
                    AddTask(taskId, silentFail, action);
            });
            bool add = _liveTasks.TryAdd(taskId, (false, t));
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
