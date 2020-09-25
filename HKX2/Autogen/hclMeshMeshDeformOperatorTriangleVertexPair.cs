using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshMeshDeformOperatorTriangleVertexPair : IHavokObject
    {
        public Vector4 m_localPosition;
        public Vector4 m_localNormal;
        public ushort m_triangleIndex;
        public float m_weight;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_localPosition = des.ReadVector4(br);
            m_localNormal = des.ReadVector4(br);
            m_triangleIndex = br.ReadUInt16();
            br.AssertUInt16(0);
            m_weight = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_triangleIndex);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_weight);
            bw.WriteUInt64(0);
        }
    }
}
