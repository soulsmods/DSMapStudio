using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterControllerSetup : IHavokObject
    {
        public hkbRigidBodySetup m_rigidBodySetup;
        public hkReferencedObject m_controllerCinfo;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rigidBodySetup = new hkbRigidBodySetup();
            m_rigidBodySetup.Read(des, br);
            m_controllerCinfo = des.ReadClassPointer<hkReferencedObject>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_rigidBodySetup.Write(bw);
            // Implement Write
        }
    }
}
