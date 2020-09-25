using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDisplayBindingData : hkReferencedObject
    {
        public List<hkpDisplayBindingDataRigidBody> m_rigidBodyBindings;
        public List<hkpDisplayBindingDataPhysicsSystem> m_physicsSystemBindings;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBodyBindings = des.ReadClassPointerArray<hkpDisplayBindingDataRigidBody>(br);
            m_physicsSystemBindings = des.ReadClassPointerArray<hkpDisplayBindingDataPhysicsSystem>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
