using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBinaryAction : hkpAction
    {
        public hkpEntity m_entityA;
        public hkpEntity m_entityB;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_entityA = des.ReadClassPointer<hkpEntity>(br);
            m_entityB = des.ReadClassPointer<hkpEntity>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
