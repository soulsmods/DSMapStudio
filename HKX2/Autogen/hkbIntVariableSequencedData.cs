using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbIntVariableSequencedData : hkbSequencedData
    {
        public List<hkbIntVariableSequencedDataSample> m_samples;
        public int m_variableIndex;
    }
}
