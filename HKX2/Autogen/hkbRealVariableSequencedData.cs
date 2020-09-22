using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRealVariableSequencedData : hkbSequencedData
    {
        public List<hkbRealVariableSequencedDataSample> m_samples;
        public int m_variableIndex;
    }
}
