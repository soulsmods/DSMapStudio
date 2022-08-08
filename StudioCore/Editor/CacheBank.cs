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

        public static T GetCached<T>(object UIScreen, object key, Func<T> getValue)
        {
            var trueKey = (UIScreen, key);
            if (!caches.ContainsKey(trueKey))
            {
                caches[trueKey] = getValue();
            }
            return (T)caches[trueKey];
        }
        public static void ClearCaches()
        {
            caches.Clear();
        }
    }
}
