using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPathPathPoint : IHavokObject
    {
        public Vector4 m_position;
        public Vector4 m_normal;
        public uint m_userEdgeData;
        public int m_sectionId;
        public byte m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_position = des.ReadVector4(br);
            m_normal = des.ReadVector4(br);
            m_userEdgeData = br.ReadUInt32();
            m_sectionId = br.ReadInt32();
            m_flags = br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_userEdgeData);
            bw.WriteInt32(m_sectionId);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
