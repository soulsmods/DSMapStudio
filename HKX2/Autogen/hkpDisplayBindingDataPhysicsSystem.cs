using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDisplayBindingDataPhysicsSystem : hkReferencedObject
    {
        public List<hkpDisplayBindingDataRigidBody> m_bindings;
        public hkpPhysicsSystem m_system;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bindings = des.ReadClassPointerArray<hkpDisplayBindingDataRigidBody>(br);
            m_system = des.ReadClassPointer<hkpPhysicsSystem>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
