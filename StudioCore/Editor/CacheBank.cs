using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace StudioCore.Editor
{
    public class CacheBank
    {
        private static Dictionary<(EditorScreen, object, string), object> caches = new Dictionary<(EditorScreen, object, string), object>();
        
        /// <summary>
        /// Gets/Sets a cache. The cached data is intended to have a lifetime until the contextual object is modified, or the UIScreen object is refreshed.
        /// </summary>
        public static T GetCached<T>(EditorScreen UIScreen, object context, Func<T> getValue)
        {
            return GetCached<T>(UIScreen, context, "", getValue);
        }

        /// <summary>
        /// Gets/Sets a cache with a specific key, avoiding any case where there would be conflict over the context-giving object
        /// </summary>
        public static T GetCached<T>(EditorScreen UIScreen, object context, string key, Func<T> getValue)
        {
            var trueKey = (UIScreen, context, key);
            if (!caches.ContainsKey(trueKey))
            {
                caches[trueKey] = getValue();
            }
            return (T)caches[trueKey];
        }

        /// <summary>
        /// Removes cached data related to the context object
        /// </summary>
        public static void RemoveCache(EditorScreen UIScreen, object context)
        {
            var toRemove = caches.Where((keypair) => keypair.Key.Item1 == UIScreen && keypair.Key.Item2 == context);
            foreach (var kp in toRemove)
                caches.Remove(kp.Key);
        }
        
        /// <summary>
        /// Removes cached data within the UIScreen's domain
        /// </summary>
        public static void RemoveCache(EditorScreen UIScreen)
        {
            var toRemove = caches.Where((keypair) => keypair.Key.Item1 == UIScreen);
            foreach (var kp in toRemove)
                caches.Remove(kp.Key);
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
