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
        private static List<(Func<bool>, Action)> cacheClears = new List<(Func<bool>, Action)>();

        public static void RegisterCache(Func<bool> isStillValid, Action clearCacheFunction)
        {
            cacheClears.Add((isStillValid, clearCacheFunction));
        }
        public static void ClearCaches()
        {
            for (int i=0; i<cacheClears.Count; i++)
            {
                (Func<bool> test, Action action) = cacheClears[i];
                if (!test())
                {
                    cacheClears.RemoveAt(i);
                    i--;
                }
                else
                {
                    action();
                }
            }
        }
    }
}
