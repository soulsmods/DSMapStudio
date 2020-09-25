using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VectorConversion
    {
        VC_FLOAT4 = 0,
        VC_FLOAT3 = 1,
        VC_BYTE4 = 2,
        VC_SHORT3 = 3,
        VC_HFLOAT3 = 4,
        VC_CUSTOM_A = 20,
        VC_CUSTOM_B = 21,
        VC_CUSTOM_C = 22,
        VC_CUSTOM_D = 23,
        VC_CUSTOM_E = 24,
        VC_NONE = 250,
    }
    
    public class hclRuntimeConversionInfo : IHavokObject
    {
        public hclRuntimeConversionInfoSlotConversion m_slotConversions;
        public hclRuntimeConversionInfoElementConversion m_elementConversions;
        public byte m_numSlotsConverted;
        public byte m_numElementsConverted;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_slotConversions = new hclRuntimeConversionInfoSlotConversion();
            m_slotConversions.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
            m_elementConversions = new hclRuntimeConversionInfoElementConversion();
            m_elementConversions.Read(des, br);
            br.AssertUInt64(0);
            br.AssertByte(0);
            m_numSlotsConverted = br.ReadByte();
            m_numElementsConverted = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_slotConversions.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
            m_elementConversions.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteByte(0);
            bw.WriteByte(m_numSlotsConverted);
            bw.WriteByte(m_numElementsConverted);
        }
    }
}
