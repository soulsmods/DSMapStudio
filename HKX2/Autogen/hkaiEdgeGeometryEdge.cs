using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiEdgeGeometryEdge : IHavokObject
    {
        public uint m_a;
        public uint m_b;
        public uint m_face;
        public uint m_data;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_a = br.ReadUInt32();
            m_b = br.ReadUInt32();
            m_face = br.ReadUInt32();
            m_data = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_a);
            bw.WriteUInt32(m_b);
            bw.WriteUInt32(m_face);
            bw.WriteUInt32(m_data);
        }
    }
}
