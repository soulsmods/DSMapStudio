using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshBoneDeformOperatorTriangleBonePair
    {
        public Matrix4x4 m_localBoneTransform;
        public float m_weight;
        public ushort m_triangleIndex;
    }
}
