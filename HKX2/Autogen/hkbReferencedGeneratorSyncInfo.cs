using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbReferencedGeneratorSyncInfo : hkReferencedObject
    {
        public hkbGeneratorSyncInfo m_syncInfo;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_syncInfo = new hkbGeneratorSyncInfo();
            m_syncInfo.Read(des, br);
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_syncInfo.Write(bw);
            bw.WriteUInt32(0);
        }
    }
}
