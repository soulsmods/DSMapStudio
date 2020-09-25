using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSweptTransformf : IHavokObject
    {
        public Vector4 m_centerOfMass0;
        public Vector4 m_centerOfMass1;
        public Quaternion m_rotation0;
        public Quaternion m_rotation1;
        public Vector4 m_centerOfMassLocal;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_centerOfMass0 = des.ReadVector4(br);
            m_centerOfMass1 = des.ReadVector4(br);
            m_rotation0 = des.ReadQuaternion(br);
            m_rotation1 = des.ReadQuaternion(br);
            m_centerOfMassLocal = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
