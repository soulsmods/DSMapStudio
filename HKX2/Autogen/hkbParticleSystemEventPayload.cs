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
    }
}
