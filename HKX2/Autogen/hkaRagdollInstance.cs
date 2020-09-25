using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaRagdollInstance : hkReferencedObject
    {
        public List<hkpRigidBody> m_rigidBodies;
        public List<hkpConstraintInstance> m_constraints;
        public List<int> m_boneToRigidBodyMap;
        public hkaSkeleton m_skeleton;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBodies = des.ReadClassPointerArray<hkpRigidBody>(br);
            m_constraints = des.ReadClassPointerArray<hkpConstraintInstance>(br);
            m_boneToRigidBodyMap = des.ReadInt32Array(br);
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
