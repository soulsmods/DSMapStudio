using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBoolVariableSequencedData : hkbSequencedData
    {
        public List<hkbBoolVariableSequencedDataSample> m_samples;
        public int m_variableIndex;
    }
}
