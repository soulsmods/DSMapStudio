using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudioCore.Editor
{
    public class TaskManager
    {
        private static volatile ConcurrentDictionary<string, (bool, Task)> _liveTasks = new ConcurrentDictionary<string, (bool, Task)>();
        private static int _anonIndex = 0;

        public static bool Run(string taskId, bool wait, bool canRequeue, System.Action action)
        {
            bool add = AddTask(taskId, action);

            if (!add)
            {
                if (wait)
                {
                    (bool, Task) t;
                    if (_liveTasks.TryGetValue(taskId, out t))
                    {
                        t.Item2.Wait();
                        return AddTask(taskId, action);                        
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
        private static bool AddTask(string taskId, System.Action action)
        {
            if (taskId == null)
            {
                _anonIndex++;
                taskId = Thread.CurrentThread.Name+":"+_anonIndex;
            }

            Task t = new Task(() => {
                //try
                //{
                    action.Invoke();
                //}
                //catch (Exception e)
                //{
                //    MessageBox.Show(("An error has occurred in task "+taskId+":\n"+e.Message+"\n\n"+e.StackTrace).Replace("\0", "\\0"), "Unhandled Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                (bool, Task) old;
                _liveTasks.TryRemove(taskId, out old);
                if (old.Item1 == true)
                    AddTask(taskId, action);
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
    }
}
