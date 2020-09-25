using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpRefWorldCinfo : hkReferencedObject
    {
        public hknpWorldCinfo m_info;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_info = new hknpWorldCinfo();
            m_info.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_info.Write(bw);
        }
    }
}
