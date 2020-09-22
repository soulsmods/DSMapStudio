using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum HideCriteria
    {
        NONE = 0,
        MODELER_IS_MAX = 1,
        MODELER_IS_MAYA = 2,
        UI_SCHEME_IS_DESTRUCTION = 4,
        UI_SCHEME_IS_DESTRUCTION_2012 = 8,
    }
    
    public class hkUiAttribute
    {
        public bool m_visible;
        public bool m_editable;
        public HideCriteria m_hideCriteria;
        public string m_label;
        public string m_group;
        public string m_hideBaseClassMembers;
        public bool m_endGroup;
        public bool m_endGroup2;
        public bool m_advanced;
    }
}
