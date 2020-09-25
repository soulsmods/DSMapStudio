using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclVolumeConstraint : hclConstraintSet
    {
        public List<hclVolumeConstraintFrameData> m_frameDatas;
        public List<hclVolumeConstraintApplyData> m_applyDatas;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_frameDatas = des.ReadClassArray<hclVolumeConstraintFrameData>(br);
            m_applyDatas = des.ReadClassArray<hclVolumeConstraintApplyData>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
