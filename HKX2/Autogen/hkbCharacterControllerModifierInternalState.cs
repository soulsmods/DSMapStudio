using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterControllerModifierInternalState : hkReferencedObject
    {
        public Vector4 m_gravity;
        public bool m_isInitialVelocityAdded;
        public bool m_isTouchingGround;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_gravity = des.ReadVector4(br);
            m_isInitialVelocityAdded = br.ReadBoolean();
            m_isTouchingGround = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_isInitialVelocityAdded);
            bw.WriteBoolean(m_isTouchingGround);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
