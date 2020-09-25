using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbStateChooserWrapper : hkbCustomIdSelector
    {
        public hkbStateChooser m_wrappedChooser;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wrappedChooser = des.ReadClassPointer<hkbStateChooser>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
