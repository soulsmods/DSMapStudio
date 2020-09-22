using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum TriggerType
    {
        TRIGGER_TYPE_NONE = 0,
        TRIGGER_TYPE_BROAD_PHASE = 1,
        TRIGGER_TYPE_NARROW_PHASE = 2,
        TRIGGER_TYPE_CONTACT_SOLVER = 3,
    }
    
    public enum CombinePolicy
    {
        COMBINE_AVG = 0,
        COMBINE_MIN = 1,
        COMBINE_MAX = 2,
    }
    
    public enum MassChangerCategory
    {
        MASS_CHANGER_IGNORE = 0,
        MASS_CHANGER_DEBRIS = 1,
        MASS_CHANGER_HEAVY = 2,
    }
    
    public class hknpMaterial
    {
        public string m_name;
        public uint m_isExclusive;
        public int m_flags;
        public TriggerType m_triggerType;
        public hkUFloat8 m_triggerManifoldTolerance;
        public ushort m_dynamicFriction;
        public ushort m_staticFriction;
        public ushort m_restitution;
        public CombinePolicy m_frictionCombinePolicy;
        public CombinePolicy m_restitutionCombinePolicy;
        public ushort m_weldingTolerance;
        public float m_maxContactImpulse;
        public float m_fractionOfClippedImpulseToApply;
        public MassChangerCategory m_massChangerCategory;
        public ushort m_massChangerHeavyObjectFactor;
        public ushort m_softContactForceFactor;
        public ushort m_softContactDampFactor;
        public hkUFloat8 m_softContactSeperationVelocity;
        public hknpSurfaceVelocity m_surfaceVelocity;
        public ushort m_disablingCollisionsBetweenCvxCvxDynamicObjectsDistance;
        public ulong m_userData;
        public bool m_isShared;
    }
}
