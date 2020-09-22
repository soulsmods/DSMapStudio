using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothData : hkReferencedObject
    {
        public hclSimClothDataOverridableSimulationInfo m_simulationInfo;
        public string m_name;
        public List<hclSimClothDataParticleData> m_particleDatas;
        public List<ushort> m_fixedParticles;
        public List<ushort> m_triangleIndices;
        public List<byte> m_triangleFlips;
        public float m_totalMass;
        public hclSimClothDataCollidableTransformMap m_collidableTransformMap;
        public List<hclCollidable> m_perInstanceCollidables;
        public List<hclConstraintSet> m_staticConstraintSets;
        public List<hclConstraintSet> m_antiPinchConstraintSets;
        public List<hclSimClothPose> m_simClothPoses;
        public List<hclAction> m_actions;
        public List<uint> m_staticCollisionMasks;
        public List<bool> m_perParticlePinchDetectionEnabledFlags;
        public List<hclSimClothDataCollidablePinchingData> m_collidablePinchingDatas;
        public ushort m_minPinchedParticleIndex;
        public ushort m_maxPinchedParticleIndex;
        public uint m_maxCollisionPairs;
        public float m_maxParticleRadius;
        public hclSimClothDataLandscapeCollisionData m_landscapeCollisionData;
        public uint m_numLandscapeCollidableParticles;
        public bool m_doNormals;
        public hclSimClothDataTransferMotionData m_transferMotionData;
    }
}
