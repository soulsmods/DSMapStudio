using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpShapeMassProperties : hkReferencedObject
    {
        public hkCompressedMassProperties m_compressedMassProperties;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_compressedMassProperties = new hkCompressedMassProperties();
            m_compressedMassProperties.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_compressedMassProperties.Write(bw);
        }
    }
}
