using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSequence : hkbModifier
    {
        public List<hkbEventSequencedData> m_eventSequencedData;
        public List<hkbRealVariableSequencedData> m_realVariableSequencedData;
        public List<hkbBoolVariableSequencedData> m_boolVariableSequencedData;
        public List<hkbIntVariableSequencedData> m_intVariableSequencedData;
        public int m_enableEventId;
        public int m_disableEventId;
        public hkbSequenceStringData m_stringData;
    }
}
