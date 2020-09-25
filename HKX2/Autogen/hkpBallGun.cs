using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBallGun : hkpFirstPersonGun
    {
        public float m_bulletRadius;
        public float m_bulletVelocity;
        public float m_bulletMass;
        public float m_damageMultiplier;
        public int m_maxBulletsInWorld;
        public Vector4 m_bulletOffsetFromCenter;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bulletRadius = br.ReadSingle();
            m_bulletVelocity = br.ReadSingle();
            m_bulletMass = br.ReadSingle();
            m_damageMultiplier = br.ReadSingle();
            m_maxBulletsInWorld = br.ReadInt32();
            br.AssertUInt32(0);
            m_bulletOffsetFromCenter = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_bulletRadius);
            bw.WriteSingle(m_bulletVelocity);
            bw.WriteSingle(m_bulletMass);
            bw.WriteSingle(m_damageMultiplier);
            bw.WriteInt32(m_maxBulletsInWorld);
            bw.WriteUInt32(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
