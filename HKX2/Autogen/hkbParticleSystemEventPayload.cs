using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SystemType
    {
        DEBRIS = 0,
        DUST = 1,
        EXPLOSION = 2,
        SMOKE = 3,
        SPARKS = 4,
    }
    
    public class hkbParticleSystemEventPayload : hkbEventPayload
    {
        public SystemType m_type;
        public short m_emitBoneIndex;
        public Vector4 m_offset;
        public Vector4 m_direction;
        public int m_numParticles;
        public float m_speed;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (SystemType)br.ReadByte();
            br.AssertByte(0);
            m_emitBoneIndex = br.ReadInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_offset = des.ReadVector4(br);
            m_direction = des.ReadVector4(br);
            m_numParticles = br.ReadInt32();
            m_speed = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(0);
            bw.WriteInt16(m_emitBoneIndex);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_numParticles);
            bw.WriteSingle(m_speed);
            bw.WriteUInt64(0);
        }
    }
}
