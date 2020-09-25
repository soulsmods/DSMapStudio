using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventsFromRangeModifierInternalState : hkReferencedObject
    {
        public List<bool> m_wasActiveInPreviousFrame;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wasActiveInPreviousFrame = des.ReadBooleanArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
