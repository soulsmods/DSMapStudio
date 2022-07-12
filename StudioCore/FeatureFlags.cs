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
        public static bool LoadDS3Navmeshes = false; //Breaks rendering for other games

        public static bool EnableNavmeshBuilder = false;

        public static bool MBSE_Test = false; //MBSE read/write test
    }
}
