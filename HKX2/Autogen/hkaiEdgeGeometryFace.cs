using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiEdgeGeometryFace : IHavokObject
    {
        public uint m_data;
        public uint m_faceIndex;
        public byte m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_data = br.ReadUInt32();
            m_faceIndex = br.ReadUInt32();
            m_flags = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_data);
            bw.WriteUInt32(m_faceIndex);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
