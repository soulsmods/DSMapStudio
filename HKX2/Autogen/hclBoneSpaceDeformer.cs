using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceDeformer
    {
        public enum ControlByte
        {
            FOUR_BLEND = 0,
            THREE_BLEND = 1,
            TWO_BLEND = 2,
            ONE_BLEND = 3,
            NEXT_SPU_BATCH = 4,
        }
        
        public List<hclBoneSpaceDeformerFourBlendEntryBlock> m_fourBlendEntries;
        public List<hclBoneSpaceDeformerThreeBlendEntryBlock> m_threeBlendEntries;
        public List<hclBoneSpaceDeformerTwoBlendEntryBlock> m_twoBlendEntries;
        public List<hclBoneSpaceDeformerOneBlendEntryBlock> m_oneBlendEntries;
        public List<byte> m_controlBytes;
        public ushort m_startVertexIndex;
        public ushort m_endVertexIndex;
        public ushort m_batchSizeSpu;
        public bool m_partialWrite;
    }
}
