using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclObjectSpaceSkinOperator : hclOperator
    {
        public List<Matrix4x4> m_boneFromSkinMeshTransforms;
        public List<ushort> m_transformSubset;
        public uint m_outputBufferIndex;
        public uint m_transformSetIndex;
        public hclObjectSpaceDeformer m_objectSpaceDeformer;
    }
}
