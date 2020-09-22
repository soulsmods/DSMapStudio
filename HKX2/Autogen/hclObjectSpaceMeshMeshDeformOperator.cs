using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceMeshMeshDeformOperator : hclOperator
    {
        public enum ScaleNormalBehaviour
        {
            SCALE_NORMAL_IGNORE = 0,
            SCALE_NORMAL_APPLY = 1,
            SCALE_NORMAL_INVERT = 2,
        }
        
        public uint m_inputBufferIdx;
        public uint m_outputBufferIdx;
        public ScaleNormalBehaviour m_scaleNormalBehaviour;
        public List<ushort> m_inputTrianglesSubset;
        public List<Matrix4x4> m_triangleFromMeshTransforms;
        public hclObjectSpaceDeformer m_objectSpaceDeformer;
    }
}
