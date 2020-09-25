using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConstraintChainInstanceAction : hkpAction
    {
        public hkpConstraintChainInstance m_constraintInstance;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_constraintInstance = des.ReadClassPointer<hkpConstraintChainInstance>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
