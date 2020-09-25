using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbModifierGenerator : hkbGenerator
    {
        public hkbModifier m_modifier;
        public hkbGenerator m_generator;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modifier = des.ReadClassPointer<hkbModifier>(br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
