using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxMaterialTextureStage : IHavokObject
    {
        public hkReferencedObject m_texture;
        public TextureType m_usageHint;
        public int m_tcoordChannel;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_texture = des.ReadClassPointer<hkReferencedObject>(br);
            m_usageHint = (TextureType)br.ReadInt32();
            m_tcoordChannel = br.ReadInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteInt32(m_tcoordChannel);
        }
    }
}
