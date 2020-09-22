using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbScriptGenerator : hkbGenerator
    {
        public hkbGenerator m_child;
        public string m_onActivateScript;
        public string m_onPreUpdateScript;
        public string m_onGenerateScript;
        public string m_onHandleEventScript;
        public string m_onDeactivateScript;
    }
}
