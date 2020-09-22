using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclMeshBoneDeformOperator : hclOperator
    {
        public uint m_inputBufferIdx;
        public uint m_outputTransformSetIdx;
        public List<hclMeshBoneDeformOperatorTriangleBonePair> m_triangleBonePairs;
        public List<ushort> m_triangleBoneStartForBone;
    }
}
