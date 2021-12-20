using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace StudioCore.Editor
{
    public static class EditorCommandQueue
    {
        private static ConcurrentQueue<string> QueuedCommands = new ConcurrentQueue<string>();

        public static void AddCommand(string cmd)
        {
            QueuedCommands.Enqueue(cmd);
        }

        public static string GetNextCommand()
        {
            string cmd = null;
            var res = QueuedCommands.TryDequeue(out cmd);
            if (res)
            {
                return cmd;
            }
            return null;
        }
    }
}
