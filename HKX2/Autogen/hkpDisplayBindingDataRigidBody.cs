using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDisplayBindingDataRigidBody : hkReferencedObject
    {
        public hkpRigidBody m_rigidBody;
        public hkReferencedObject m_displayObjectPtr;
        public Matrix4x4 m_rigidBodyFromDisplayObjectTransform;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBody = des.ReadClassPointer<hkpRigidBody>(br);
            m_displayObjectPtr = des.ReadClassPointer<hkReferencedObject>(br);
            m_rigidBodyFromDisplayObjectTransform = des.ReadMatrix4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
