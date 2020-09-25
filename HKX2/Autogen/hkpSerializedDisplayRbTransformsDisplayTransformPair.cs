using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSerializedDisplayRbTransformsDisplayTransformPair : IHavokObject
    {
        public hkpRigidBody m_rb;
        public Matrix4x4 m_localToDisplay;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rb = des.ReadClassPointer<hkpRigidBody>(br);
            br.AssertUInt64(0);
            m_localToDisplay = des.ReadTransform(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
