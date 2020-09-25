using SoulsFormats;
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
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_frameBatchDatas = des.ReadClassArray<hclVolumeConstraintMxFrameBatchData>(br);
            m_frameSingleDatas = des.ReadClassArray<hclVolumeConstraintMxFrameSingleData>(br);
            m_applyBatchDatas = des.ReadClassArray<hclVolumeConstraintMxApplyBatchData>(br);
            m_applySingleDatas = des.ReadClassArray<hclVolumeConstraintMxApplySingleData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
