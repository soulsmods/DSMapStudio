using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConvexVerticesShape : hkpConvexShape
    {
        public Vector4 m_aabbHalfExtents;
        public Vector4 m_aabbCenter;
        public List<Matrix4x4> m_rotatedVertices;
        public int m_numVertices;
        public List<Vector4> m_planeEquations;
        public hkpConvexVerticesConnectivity m_connectivity;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_aabbHalfExtents = des.ReadVector4(br);
            m_aabbCenter = des.ReadVector4(br);
            m_rotatedVertices = des.ReadMatrix3Array(br);
            m_numVertices = br.ReadInt32();
            br.AssertUInt32(0);
            m_planeEquations = des.ReadVector4Array(br);
            m_connectivity = des.ReadClassPointer<hkpConvexVerticesConnectivity>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_numVertices);
            bw.WriteUInt32(0);
            // Implement Write
        }
    }
}
