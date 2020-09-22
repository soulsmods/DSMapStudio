using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBodyCinfo
    {
        public hknpShape m_shape;
        public uint m_reservedBodyId;
        public uint m_motionId;
        public byte m_qualityId;
        public ushort m_materialId;
        public uint m_collisionFilterInfo;
        public int m_flags;
        public float m_collisionLookAheadDistance;
        public string m_name;
        public ulong m_userData;
        public Vector4 m_position;
        public Quaternion m_orientation;
        public uint m_spuFlags;
        public hkLocalFrame m_localFrame;
    }
}
