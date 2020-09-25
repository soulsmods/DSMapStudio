using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaFootstepAnalysisInfoContainer : hkReferencedObject
    {
        public List<hkaFootstepAnalysisInfo> m_previewInfo;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_previewInfo = des.ReadClassPointerArray<hkaFootstepAnalysisInfo>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
