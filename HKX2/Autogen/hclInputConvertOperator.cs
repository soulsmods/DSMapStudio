using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclInputConvertOperator : hclOperator
    {
        public uint m_userBufferIndex;
        public uint m_shadowBufferIndex;
        public hclRuntimeConversionInfo m_conversionInfo;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_userBufferIndex = br.ReadUInt32();
            m_shadowBufferIndex = br.ReadUInt32();
            m_conversionInfo = new hclRuntimeConversionInfo();
            m_conversionInfo.Read(des, br);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_userBufferIndex);
            bw.WriteUInt32(m_shadowBufferIndex);
            m_conversionInfo.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
