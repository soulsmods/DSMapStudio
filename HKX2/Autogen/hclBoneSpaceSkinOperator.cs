using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceSkinOperator : hclOperator
    {
        public List<ushort> m_transformSubset;
        public uint m_outputBufferIndex;
        public uint m_transformSetIndex;
        public hclBoneSpaceDeformer m_boneSpaceDeformer;
    }
}
