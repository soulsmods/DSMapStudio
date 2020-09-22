using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateMachineStateInfo : hkbBindable
    {
        public List<hkbStateListener> m_listeners;
        public hkbStateMachineEventPropertyArray m_enterNotifyEvents;
        public hkbStateMachineEventPropertyArray m_exitNotifyEvents;
        public hkbStateMachineTransitionInfoArray m_transitions;
        public hkbGenerator m_generator;
        public string m_name;
        public int m_stateId;
        public float m_probability;
        public bool m_enable;
    }
}
