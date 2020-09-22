using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclVolumeConstraintMx : hclConstraintSet
    {
        public List<hclVolumeConstraintMxFrameBatchData> m_frameBatchDatas;
        public List<hclVolumeConstraintMxFrameSingleData> m_frameSingleDatas;
        public List<hclVolumeConstraintMxApplyBatchData> m_applyBatchDatas;
        public List<hclVolumeConstraintMxApplySingleData> m_applySingleDatas;
    }
}
