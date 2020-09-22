using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimpleMeshBoneDeformOperator : hclOperator
    {
        public uint m_inputBufferIdx;
        public uint m_outputTransformSetIdx;
        public List<hclSimpleMeshBoneDeformOperatorTriangleBonePair> m_triangleBonePairs;
        public List<Matrix4x4> m_localBoneTransforms;
    }
}
