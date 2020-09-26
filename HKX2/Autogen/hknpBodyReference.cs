using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBodyReference : hkReferencedObject
    {
        public uint m_id;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_id = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_id);
            bw.WriteUInt32(0);
        }
    }
}
