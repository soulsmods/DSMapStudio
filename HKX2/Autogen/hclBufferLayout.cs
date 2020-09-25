using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SlotFlags
    {
        SF_NO_ALIGNED_START = 0,
        SF_16BYTE_ALIGNED_START = 1,
        SF_64BYTE_ALIGNED_START = 3,
    }
    
    public enum TriangleFormat
    {
        TF_THREE_INT32S = 0,
        TF_THREE_INT16S = 1,
        TF_OTHER = 2,
    }
    
    public class hclBufferLayout : IHavokObject
    {
        public hclBufferLayoutBufferElement m_elementsLayout;
        public hclBufferLayoutSlot m_slots;
        public byte m_numSlots;
        public TriangleFormat m_triangleFormat;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elementsLayout = new hclBufferLayoutBufferElement();
            m_elementsLayout.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_slots = new hclBufferLayoutSlot();
            m_slots.Read(des, br);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_numSlots = br.ReadByte();
            m_triangleFormat = (TriangleFormat)br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_elementsLayout.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_slots.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(m_numSlots);
        }
    }
}
