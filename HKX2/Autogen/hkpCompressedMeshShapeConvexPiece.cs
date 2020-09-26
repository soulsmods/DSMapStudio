using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCompressedMeshShapeConvexPiece : IHavokObject
    {
        public Vector4 m_offset;
        public List<ushort> m_vertices;
        public ushort m_reference;
        public ushort m_transformIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = des.ReadVector4(br);
            m_vertices = des.ReadUInt16Array(br);
            m_reference = br.ReadUInt16();
            m_transformIndex = br.ReadUInt16();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_reference);
            bw.WriteUInt16(m_transformIndex);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
