using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclConvexPlanesShape : hclShape
    {
        public List<Vector4> m_planeEquations;
        public Matrix4x4 m_localFromWorld;
        public Matrix4x4 m_worldFromLocal;
        public hkAabb m_objAabb;
        public Vector4 m_geomCentroid;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_planeEquations = des.ReadVector4Array(br);
            br.AssertUInt64(0);
            m_localFromWorld = des.ReadTransform(br);
            m_worldFromLocal = des.ReadTransform(br);
            m_objAabb = new hkAabb();
            m_objAabb.Read(des, br);
            m_geomCentroid = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_objAabb.Write(bw);
        }
    }
}
