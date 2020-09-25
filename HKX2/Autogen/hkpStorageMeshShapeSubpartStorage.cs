using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStorageMeshShapeSubpartStorage : hkReferencedObject
    {
        public List<float> m_vertices;
        public List<ushort> m_indices16;
        public List<uint> m_indices32;
        public List<byte> m_materialIndices;
        public List<uint> m_materials;
        public List<ushort> m_materialIndices16;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertices = des.ReadSingleArray(br);
            m_indices16 = des.ReadUInt16Array(br);
            m_indices32 = des.ReadUInt32Array(br);
            m_materialIndices = des.ReadByteArray(br);
            m_materials = des.ReadUInt32Array(br);
            m_materialIndices16 = des.ReadUInt16Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
