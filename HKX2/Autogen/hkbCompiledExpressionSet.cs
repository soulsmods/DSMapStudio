using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCompiledExpressionSet : hkReferencedObject
    {
        public List<hkbCompiledExpressionSetToken> m_rpn;
        public List<int> m_expressionToRpnIndex;
        public sbyte m_numExpressions;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rpn = des.ReadClassArray<hkbCompiledExpressionSetToken>(br);
            m_expressionToRpnIndex = des.ReadInt32Array(br);
            m_numExpressions = br.ReadSByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSByte(m_numExpressions);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
