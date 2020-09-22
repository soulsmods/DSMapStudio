using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCompiledExpressionSet : hkReferencedObject
    {
        public List<hkbCompiledExpressionSetToken> m_rpn;
        public List<int> m_expressionToRpnIndex;
        public char m_numExpressions;
    }
}
