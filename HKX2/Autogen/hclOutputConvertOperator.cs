using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclOutputConvertOperator : hclOperator
    {
        public uint m_userBufferIndex;
        public uint m_shadowBufferIndex;
        public hclRuntimeConversionInfo m_conversionInfo;
    }
}
