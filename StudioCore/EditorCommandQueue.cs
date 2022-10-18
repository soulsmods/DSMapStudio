using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;

namespace StudioCore
{
    public static class EditorCommandQueue
    {
        private static ConcurrentQueue<string[]> QueuedCommands = new ConcurrentQueue<string[]>();

        public static void AddCommand(string cmd)
        {
            QueuedCommands.Enqueue(cmd.Split("/"));
        }

        public static void AddCommand(IEnumerable<string> cmd)
        {
            QueuedCommands.Enqueue(cmd.ToArray());
        }

        public static string[] GetNextCommand()
        {
            QueuedCommands.TryDequeue(out string[] cmd);
            return cmd;
        }
    }
}
