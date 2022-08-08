using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudioCore.Editor
{
    public class CacheBank
    {
        private static Dictionary<object, object> caches = new Dictionary<object, object>();

        public static T GetCached<T>(object key, Func<T> getValue)
        {
            if (!caches.ContainsKey(key))
            {
                Console.WriteLine("No Cache");
                caches[key] = getValue();
            }
            return (T)caches[key];
        }
        public static void ClearCaches()
        {
            caches.Clear();
        }
    }
}
