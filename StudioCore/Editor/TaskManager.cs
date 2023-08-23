using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace StudioCore.Editor
{
    public class TaskManager
    {
        /// <summary>
        /// Behavior of a LiveTask added when a task with the same task ID is already running.
        /// </summary>
        public enum RequeueTypeEnum
        {
            // Don't requeue.
            None,

            // Wait for first task to finish, then run.
            WaitThenRequeue,

            // Tell first task to run again after finishing.
            Repeat
        }

        private static volatile ConcurrentDictionary<string, LiveTask> _liveTasks = new();

        /// <summary>
        /// Number of non-passive tasks that are currently running.
        /// </summary>
        public static int ActiveTaskNum { get; private set; } = 0;

        public class LiveTask
        {
            /// <summary>
            /// Unique identifier for task.
            /// If more than one LiveTask with the same ID is run, RequeueType is checked.
            /// </summary>
            public readonly string TaskId;

            /// <summary>
            /// Behavior of a LiveTask added when a task with the same task ID is already running.
            /// </summary>
            public readonly RequeueTypeEnum RequeueType;

            public readonly TaskLogs.LogPriority LogPriority;
            public readonly Action TaskAction;

            /// <summary>
            /// If true, exceptions will be suppressed and logged.
            /// </summary>
            public readonly bool SilentFail;

            public Task Task { get; private set; } = null;

            /// <summary>
            /// If true, task will run again after finishing.
            /// </summary>
            public bool HasScheduledRequeue = false;

            /// <summary>
            /// True for tasks that are intended to be running as long as DSMS is running.
            /// </summary>
            public bool PassiveTask = false;

            public LiveTask() { }

            public LiveTask(string taskId, RequeueTypeEnum requeueType, bool silentFail, Action act)
            {
                TaskId = taskId;
                RequeueType = requeueType;
                SilentFail = silentFail;
                LogPriority = TaskLogs.LogPriority.Normal;
                TaskAction = act;
            }

            public LiveTask(string taskId, RequeueTypeEnum requeueType, bool silentFail, TaskLogs.LogPriority logPriority, Action act)
            {
                TaskId = taskId;
                RequeueType = requeueType;
                SilentFail = silentFail;
                LogPriority = logPriority;
                TaskAction = act;
            }

            public LiveTask(string taskId, bool silentFail, Action act)
            {
                TaskId = taskId;
                RequeueType = RequeueTypeEnum.None;
                SilentFail = silentFail;
                LogPriority = TaskLogs.LogPriority.Normal;
                TaskAction = act;
            }

            public LiveTask(string taskId, bool silentFail, TaskLogs.LogPriority logPriority, Action act)
            {
                TaskId = taskId;
                RequeueType = RequeueTypeEnum.None;
                SilentFail = silentFail;
                LogPriority = logPriority;
                TaskAction = act;
            }

            public void Run()
            {
                if (_liveTasks.TryGetValue(TaskId, out var oldLiveTask))
                {
                    if (oldLiveTask.RequeueType == RequeueTypeEnum.WaitThenRequeue)
                    {
                        oldLiveTask.Task.Wait();
                    }
                    else if (oldLiveTask.RequeueType == RequeueTypeEnum.Repeat)
                    {
                        oldLiveTask.HasScheduledRequeue = true;
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                CreateTask();

                if (!PassiveTask)
                    ActiveTaskNum++;
                _liveTasks[TaskId] = this;
                Task.Start();
            }

            private void CreateTask()
            {
                Task = new(() =>
                {
                    if (PassiveTask)
                    {
                        TaskLogs.AddLog($"Running passive task: {TaskId}",
                            Microsoft.Extensions.Logging.LogLevel.Information,
                            LogPriority);
                    }

                    try
                    {
                        TaskAction.Invoke();
                        TaskLogs.AddLog($"Task Completed: {TaskId}",
                            Microsoft.Extensions.Logging.LogLevel.Information,
                            LogPriority);
                    }
                    catch (Exception e)
                    {
                        if (SilentFail)
                        {
                            TaskLogs.AddLog($"Task Failed: {TaskId}",
                                Microsoft.Extensions.Logging.LogLevel.Error,
                                LogPriority);
                            TaskLogs.AddLog($"   {e.Message}",
                                Microsoft.Extensions.Logging.LogLevel.Error,
                                TaskLogs.LogPriority.Low);
                            TaskLogs.AddLog(e.StackTrace,
                                Microsoft.Extensions.Logging.LogLevel.Error,
                                TaskLogs.LogPriority.Low);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    if (HasScheduledRequeue)
                    {
                        HasScheduledRequeue = false;
                        Run();
                    }
                    else
                    {
                        if (!PassiveTask)
                            ActiveTaskNum--;
                        _liveTasks.TryRemove(TaskId, out _);
                    }
                });
            }
        }

        public static void Run(LiveTask liveTask)
        {
            liveTask.Run();
        }

        public static void WaitAll()
        {
            while (GetActiveTasks().Any())
            {
                var e = GetActiveTasks().GetEnumerator();
                e.MoveNext();
                e.Current.Task.Wait();
            }
        }

        public static IEnumerable<LiveTask> GetActiveTasks()
        {
            return _liveTasks.Values.ToList().Where(t => !t.PassiveTask);
        }

        /// <summary>
        /// Number of active tasks. Ignores passive tasks.
        /// </summary>
        /// <returns></returns>
        public static bool AnyActiveTasks() => ActiveTaskNum > 0;

        public static List<string> GetLiveThreads()
        {
            return new List<string>(_liveTasks.Keys);
        }

        public static void ThrowTaskExceptions()
        {
            // Allows exceptions in tasks to be caught by crash handler.
            foreach (var task in _liveTasks)
            {
                var ex = task.Value.Task.Exception;
                if (ex != null)
                {
                    throw ex;
                }
            }
        }
    }
}
