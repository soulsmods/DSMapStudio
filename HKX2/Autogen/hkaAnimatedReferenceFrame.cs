using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum hkaReferenceFrameTypeEnum
    {
        REFERENCE_FRAME_UNKNOWN = 0,
        REFERENCE_FRAME_DEFAULT = 1,
        REFERENCE_FRAME_PARAMETRIC = 2,
    }
    
    public class hkaAnimatedReferenceFrame : hkReferencedObject
    {
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
