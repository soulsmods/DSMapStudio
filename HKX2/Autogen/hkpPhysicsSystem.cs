using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CloneConstraintMode
    {
        CLONE_SHALLOW_IF_NOT_CONSTRAINED_TO_WORLD = 0,
        CLONE_DEEP_WITH_MOTORS = 1,
        CLONE_FORCE_SHALLOW = 2,
        CLONE_DEFAULT = 0,
    }
    
    public class hkpPhysicsSystem : hkReferencedObject
    {
        public List<hkpRigidBody> m_rigidBodies;
        public List<hkpConstraintInstance> m_constraints;
        public List<hkpAction> m_actions;
        public List<hkpPhantom> m_phantoms;
        public string m_name;
        public ulong m_userData;
        public bool m_active;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rigidBodies = des.ReadClassPointerArray<hkpRigidBody>(br);
            m_constraints = des.ReadClassPointerArray<hkpConstraintInstance>(br);
            m_actions = des.ReadClassPointerArray<hkpAction>(br);
            m_phantoms = des.ReadClassPointerArray<hkpPhantom>(br);
            m_name = des.ReadStringPointer(br);
            m_userData = br.ReadUInt64();
            m_active = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_userData);
            bw.WriteBoolean(m_active);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
