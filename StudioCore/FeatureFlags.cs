using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace StudioCore
{
    /// <summary>
    /// Class of feature flags, which can be used to enable/disable experimental
    /// or in development features. Flags should generally be removed as features mature.
    /// </summary>
    public static class FeatureFlags
    {
        public static bool LoadNavmeshes = true;
        public static bool LoadDS3Navmeshes = true;

        public static bool EnableNavmeshBuilder = false;
        
        public static bool StrictResourceChecking = true;

        public static bool MBSE_Test = false; //MBSE read/write test
    }
}
