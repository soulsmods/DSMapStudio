using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SpuFlagsEnum
    {
        FORCE_NARROW_PHASE_PPU = 1,
    }
    
    public class hknpBody
    {
        public enum Flags
        {
            IS_STATIC = 1,
            IS_DYNAMIC = 2,
            IS_KEYFRAMED = 4,
            IS_ACTIVE = 8,
        }
        
        public Matrix4x4 m_transform;
        public int m_flags;
        public uint m_collisionFilterInfo;
        public hknpShape m_shape;
        public hkAabb16 m_aabb;
        public uint m_id;
        public uint m_nextAttachedBodyId;
        public uint m_motionId;
        public uint m_broadPhaseId;
        public ushort m_materialId;
        public byte m_qualityId;
        public byte m_timAngle;
        public ushort m_maxTimDistance;
        public ushort m_maxContactDistance;
        public uint m_indexIntoActiveListOrDeactivatedIslandId;
        public ushort m_radiusOfComCenteredBoundingSphere;
        public uint m_spuFlags;
        public byte m_shapeSizeDiv16;
        public short m_motionToBodyRotation;
        public ulong m_userData;
    }
}
