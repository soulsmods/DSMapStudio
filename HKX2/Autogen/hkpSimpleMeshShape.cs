using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSimpleMeshShape : hkpShapeCollection
    {
        public List<Vector4> m_vertices;
        public List<hkpSimpleMeshShapeTriangle> m_triangles;
        public List<byte> m_materialIndices;
        public float m_radius;
        public WeldingType m_weldingType;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertices = des.ReadVector4Array(br);
            m_triangles = des.ReadClassArray<hkpSimpleMeshShapeTriangle>(br);
            m_materialIndices = des.ReadByteArray(br);
            m_radius = br.ReadSingle();
            m_weldingType = (WeldingType)br.ReadByte();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_radius);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
