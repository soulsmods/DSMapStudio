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

        /// <summary>
        /// Gets/Sets a cache
        /// </summary>
        public static T GetCached<T>(object UIScreen, object key, Func<T> getValue)
        {
            var trueKey = (UIScreen, key);
            if (!caches.ContainsKey(trueKey))
            {
                caches[trueKey] = getValue();
            }
            return (T)caches[trueKey];
        }

        /// <summary>
        /// Removes a targeted cache
        /// </summary>
        public static void RemoveCache(object UIScreen, object key)
        {
            var trueKey = (UIScreen, key);
            caches.Remove(trueKey);
        }

        /// <summary>
        /// Clears all caches
        /// </summary>
        public static void ClearCaches()
        {
            caches.Clear();
        }
    }
}
