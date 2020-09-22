using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBlenderGeneratorChild : hkbBindable
    {
        public hkbGenerator m_generator;
        public hkbBoneWeightArray m_boneWeights;
        public float m_weight;
        public float m_worldFromModelWeight;
    }
}
