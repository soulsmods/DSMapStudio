using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MotionType
    {
        MOTION_INVALID = 0,
        MOTION_DYNAMIC = 1,
        MOTION_SPHERE_INERTIA = 2,
        MOTION_BOX_INERTIA = 3,
        MOTION_KEYFRAMED = 4,
        MOTION_FIXED = 5,
        MOTION_THIN_BOX_INERTIA = 6,
        MOTION_CHARACTER = 7,
        MOTION_MAX_ID = 8,
    }
    
    public class hkpMotion : hkReferencedObject
    {
        public MotionType m_type;
        public byte m_deactivationIntegrateCounter;
        public ushort m_deactivationNumInactiveFrames;
        public hkMotionState m_motionState;
        public Vector4 m_inertiaAndMassInv;
        public Vector4 m_linearVelocity;
        public Vector4 m_angularVelocity;
        public Vector4 m_deactivationRefPosition;
        public uint m_deactivationRefOrientation;
        public hkpMaxSizeMotion m_savedMotion;
        public ushort m_savedQualityTypeIndex;
        public short m_gravityFactor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (MotionType)br.ReadByte();
            m_deactivationIntegrateCounter = br.ReadByte();
            m_deactivationNumInactiveFrames = br.ReadUInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_motionState = new hkMotionState();
            m_motionState.Read(des, br);
            m_inertiaAndMassInv = des.ReadVector4(br);
            m_linearVelocity = des.ReadVector4(br);
            m_angularVelocity = des.ReadVector4(br);
            m_deactivationRefPosition = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_deactivationRefOrientation = br.ReadUInt32();
            br.AssertUInt32(0);
            m_savedMotion = des.ReadClassPointer<hkpMaxSizeMotion>(br);
            m_savedQualityTypeIndex = br.ReadUInt16();
            m_gravityFactor = br.ReadInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_deactivationIntegrateCounter);
            bw.WriteUInt16(m_deactivationNumInactiveFrames);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_motionState.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_deactivationRefOrientation);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt16(m_savedQualityTypeIndex);
            bw.WriteInt16(m_gravityFactor);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
