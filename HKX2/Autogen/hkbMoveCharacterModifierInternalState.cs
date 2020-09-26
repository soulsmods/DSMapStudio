using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbMoveCharacterModifierInternalState : hkReferencedObject
    {
        public float m_timeSinceLastModify;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_timeSinceLastModify = br.ReadSingle();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_timeSinceLastModify);
            bw.WriteUInt32(0);
        }
    }
}
