using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxSparselyAnimatedString : hkReferencedObject
    {
        public List<string> m_strings;
        public List<float> m_times;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_strings = des.ReadStringPointerArray(br);
            m_times = des.ReadSingleArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
