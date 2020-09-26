using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBreakableBodyController : hkReferencedObject
    {
        public float m_breakingImpulse;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_breakingImpulse = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_breakingImpulse);
            bw.WriteUInt32(0);
        }
    }
}
