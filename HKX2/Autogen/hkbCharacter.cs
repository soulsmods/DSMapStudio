using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CapabilityTypes
    {
        FULL_POSE = 1,
        MIRRORING = 2,
        DOCKING = 4,
        HAND_IK = 8,
        CHARACTER_CONTROL = 16,
        RAGDOLL = 32,
        FOOT_IK = 64,
        AI_CONTROL = 128,
        BASIC_CAPABILITIES = 7,
        PHYSICS_CAPABILITIES = 120,
        STANDARD_CAPABILITIES = 127,
    }
    
    public class hkbCharacter : hkReferencedObject
    {
        public List<hkbCharacter> m_nearbyCharacters;
        public ulong m_userData;
        public short m_currentLod;
        public string m_name;
        public hkbCharacterSetup m_setup;
        public hkbBehaviorGraph m_behaviorGraph;
        public hkbProjectData m_projectData;
        public int m_capabilities;
        public int m_effectiveCapabilities;
    }
}
