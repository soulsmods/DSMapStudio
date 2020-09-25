using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterSkinInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public List<ulong> m_deformableSkins;
        public List<ulong> m_rigidSkins;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_deformableSkins = des.ReadUInt64Array(br);
            m_rigidSkins = des.ReadUInt64Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
        }
    }
}
