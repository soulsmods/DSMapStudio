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
        public hclRuntimeConversionInfoSlotConversion m_slotConversions_0;
        public hclRuntimeConversionInfoSlotConversion m_slotConversions_1;
        public hclRuntimeConversionInfoSlotConversion m_slotConversions_2;
        public hclRuntimeConversionInfoSlotConversion m_slotConversions_3;
        public hclRuntimeConversionInfoElementConversion m_elementConversions_0;
        public hclRuntimeConversionInfoElementConversion m_elementConversions_1;
        public hclRuntimeConversionInfoElementConversion m_elementConversions_2;
        public hclRuntimeConversionInfoElementConversion m_elementConversions_3;
        public byte m_numSlotsConverted;
        public byte m_numElementsConverted;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_slotConversions_0 = new hclRuntimeConversionInfoSlotConversion();
            m_slotConversions_0.Read(des, br);
            m_slotConversions_1 = new hclRuntimeConversionInfoSlotConversion();
            m_slotConversions_1.Read(des, br);
            m_slotConversions_2 = new hclRuntimeConversionInfoSlotConversion();
            m_slotConversions_2.Read(des, br);
            m_slotConversions_3 = new hclRuntimeConversionInfoSlotConversion();
            m_slotConversions_3.Read(des, br);
            m_elementConversions_0 = new hclRuntimeConversionInfoElementConversion();
            m_elementConversions_0.Read(des, br);
            m_elementConversions_1 = new hclRuntimeConversionInfoElementConversion();
            m_elementConversions_1.Read(des, br);
            m_elementConversions_2 = new hclRuntimeConversionInfoElementConversion();
            m_elementConversions_2.Read(des, br);
            m_elementConversions_3 = new hclRuntimeConversionInfoElementConversion();
            m_elementConversions_3.Read(des, br);
            m_numSlotsConverted = br.ReadByte();
            m_numElementsConverted = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_slotConversions_0.Write(bw);
            m_slotConversions_1.Write(bw);
            m_slotConversions_2.Write(bw);
            m_slotConversions_3.Write(bw);
            m_elementConversions_0.Write(bw);
            m_elementConversions_1.Write(bw);
            m_elementConversions_2.Write(bw);
            m_elementConversions_3.Write(bw);
            bw.WriteByte(m_numSlotsConverted);
            bw.WriteByte(m_numElementsConverted);
        }
    }
}
