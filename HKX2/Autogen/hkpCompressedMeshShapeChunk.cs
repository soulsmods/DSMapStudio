using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCompressedMeshShapeChunk : IHavokObject
    {
        public Vector4 m_offset;
        public List<ushort> m_vertices;
        public List<ushort> m_indices;
        public List<ushort> m_stripLengths;
        public List<ushort> m_weldingInfo;
        public uint m_materialInfo;
        public ushort m_reference;
        public ushort m_transformIndex;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = des.ReadVector4(br);
            m_vertices = des.ReadUInt16Array(br);
            m_indices = des.ReadUInt16Array(br);
            m_stripLengths = des.ReadUInt16Array(br);
            m_weldingInfo = des.ReadUInt16Array(br);
            m_materialInfo = br.ReadUInt32();
            m_reference = br.ReadUInt16();
            m_transformIndex = br.ReadUInt16();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_materialInfo);
            bw.WriteUInt16(m_reference);
            bw.WriteUInt16(m_transformIndex);
            bw.WriteUInt64(0);
        }
    }
}
