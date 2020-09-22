using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BONE_GROUP_SIZE
    {
        GROUP_SIZE_1 = 1,
        GROUP_SIZE_4 = 4,
        GROUP_SIZE_8 = 8,
        GROUP_SIZE_16 = 16,
    }
    
    public class hclSkinOperator : hclOperator
    {
        public List<hclSkinOperatorBoneInfluence> m_boneInfluences;
        public List<ushort> m_boneInfluenceStartPerVertex;
        public List<Matrix4x4> m_boneFromSkinMeshTransforms;
        public List<ushort> m_usedBoneGroupIds;
        public bool m_skinPositions;
        public bool m_skinNormals;
        public bool m_skinTangents;
        public bool m_skinBiTangents;
        public uint m_inputBufferIndex;
        public uint m_outputBufferIndex;
        public uint m_transformSetIndex;
        public ushort m_startVertex;
        public ushort m_endVertex;
        public bool m_partialSkinning;
        public bool m_dualQuaternionSkinning;
        public byte m_boneGroupSize;
    }
}
