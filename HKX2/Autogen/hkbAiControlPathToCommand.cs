using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAiControlPathToCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public Vector4 m_goalPoint;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            br.AssertUInt64(0);
            m_goalPoint = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            bw.WriteUInt64(0);
        }
    }
}
