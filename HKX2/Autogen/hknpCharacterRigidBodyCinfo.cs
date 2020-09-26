using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCharacterRigidBodyCinfo : hkReferencedObject
    {
        public uint m_collisionFilterInfo;
        public hknpShape m_shape;
        public Vector4 m_position;
        public Quaternion m_orientation;
        public float m_mass;
        public float m_dynamicFriction;
        public float m_staticFriction;
        public float m_weldingTolerance;
        public uint m_reservedBodyId;
        public byte m_additionMode;
        public byte m_additionFlags;
        public Vector4 m_up;
        public float m_maxSlope;
        public float m_maxForce;
        public float m_maxSpeedForSimplexSolver;
        public float m_supportDistance;
        public float m_hardSupportDistance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collisionFilterInfo = br.ReadUInt32();
            br.ReadUInt32();
            m_shape = des.ReadClassPointer<hknpShape>(br);
            br.ReadUInt64();
            br.ReadUInt64();
            m_position = des.ReadVector4(br);
            m_orientation = des.ReadQuaternion(br);
            m_mass = br.ReadSingle();
            m_dynamicFriction = br.ReadSingle();
            m_staticFriction = br.ReadSingle();
            m_weldingTolerance = br.ReadSingle();
            m_reservedBodyId = br.ReadUInt32();
            m_additionMode = br.ReadByte();
            m_additionFlags = br.ReadByte();
            br.ReadUInt64();
            br.ReadUInt16();
            m_up = des.ReadVector4(br);
            m_maxSlope = br.ReadSingle();
            m_maxForce = br.ReadSingle();
            m_maxSpeedForSimplexSolver = br.ReadSingle();
            m_supportDistance = br.ReadSingle();
            m_hardSupportDistance = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_mass);
            bw.WriteSingle(m_dynamicFriction);
            bw.WriteSingle(m_staticFriction);
            bw.WriteSingle(m_weldingTolerance);
            bw.WriteUInt32(m_reservedBodyId);
            bw.WriteByte(m_additionMode);
            bw.WriteByte(m_additionFlags);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_maxSlope);
            bw.WriteSingle(m_maxForce);
            bw.WriteSingle(m_maxSpeedForSimplexSolver);
            bw.WriteSingle(m_supportDistance);
            bw.WriteSingle(m_hardSupportDistance);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
