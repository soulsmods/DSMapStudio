using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCharacterProxyCinfo : hkReferencedObject
    {
        public Vector4 m_position;
        public Quaternion m_orientation;
        public Vector4 m_velocity;
        public float m_dynamicFriction;
        public float m_staticFriction;
        public float m_keepContactTolerance;
        public Vector4 m_up;
        public hknpShape m_shape;
        public ulong m_userData;
        public uint m_collisionFilterInfo;
        public float m_keepDistance;
        public float m_contactAngleSensitivity;
        public uint m_userPlanes;
        public float m_maxCharacterSpeedForSolver;
        public float m_characterStrength;
        public float m_characterMass;
        public float m_maxSlope;
        public float m_penetrationRecoverySpeed;
        public int m_maxCastIterations;
        public bool m_refreshManifoldInCheckSupport;
        public bool m_presenceInWorld;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_position = des.ReadVector4(br);
            m_orientation = des.ReadQuaternion(br);
            m_velocity = des.ReadVector4(br);
            m_dynamicFriction = br.ReadSingle();
            m_staticFriction = br.ReadSingle();
            m_keepContactTolerance = br.ReadSingle();
            br.AssertUInt32(0);
            m_up = des.ReadVector4(br);
            m_shape = des.ReadClassPointer<hknpShape>(br);
            m_userData = br.ReadUInt64();
            br.AssertUInt64(0);
            m_collisionFilterInfo = br.ReadUInt32();
            m_keepDistance = br.ReadSingle();
            m_contactAngleSensitivity = br.ReadSingle();
            m_userPlanes = br.ReadUInt32();
            m_maxCharacterSpeedForSolver = br.ReadSingle();
            m_characterStrength = br.ReadSingle();
            m_characterMass = br.ReadSingle();
            m_maxSlope = br.ReadSingle();
            m_penetrationRecoverySpeed = br.ReadSingle();
            m_maxCastIterations = br.ReadInt32();
            m_refreshManifoldInCheckSupport = br.ReadBoolean();
            m_presenceInWorld = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_dynamicFriction);
            bw.WriteSingle(m_staticFriction);
            bw.WriteSingle(m_keepContactTolerance);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(m_userData);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteSingle(m_keepDistance);
            bw.WriteSingle(m_contactAngleSensitivity);
            bw.WriteUInt32(m_userPlanes);
            bw.WriteSingle(m_maxCharacterSpeedForSolver);
            bw.WriteSingle(m_characterStrength);
            bw.WriteSingle(m_characterMass);
            bw.WriteSingle(m_maxSlope);
            bw.WriteSingle(m_penetrationRecoverySpeed);
            bw.WriteInt32(m_maxCastIterations);
            bw.WriteBoolean(m_refreshManifoldInCheckSupport);
            bw.WriteBoolean(m_presenceInWorld);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
