using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEvaluateExpressionModifierInternalState : hkReferencedObject
    {
        public List<hkbEvaluateExpressionModifierInternalExpressionData> m_internalExpressionsData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_internalExpressionsData = des.ReadClassArray<hkbEvaluateExpressionModifierInternalExpressionData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
