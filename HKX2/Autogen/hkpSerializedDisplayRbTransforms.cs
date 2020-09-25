using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSerializedDisplayRbTransforms : hkReferencedObject
    {
        public List<hkpSerializedDisplayRbTransformsDisplayTransformPair> m_transforms;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transforms = des.ReadClassArray<hkpSerializedDisplayRbTransformsDisplayTransformPair>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
