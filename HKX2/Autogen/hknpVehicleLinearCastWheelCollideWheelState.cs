using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleLinearCastWheelCollideWheelState : IHavokObject
    {
        public hkAabb m_aabb;
        public hknpShape m_shape;
        public Matrix4x4 m_transform;
        public Vector4 m_to;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_shape = des.ReadClassPointer<hknpShape>(br);
            br.AssertUInt64(0);
            m_transform = des.ReadTransform(br);
            m_to = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
