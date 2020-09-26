using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiMaterialPainter : hkReferencedObject
    {
        public int m_material;
        public hkaiVolume m_volume;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_material = br.ReadInt32();
            br.ReadUInt32();
            m_volume = des.ReadClassPointer<hkaiVolume>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_material);
            bw.WriteUInt32(0);
            // Implement Write
        }
    }
}
