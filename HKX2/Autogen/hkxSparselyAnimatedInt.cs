using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxSparselyAnimatedInt : hkReferencedObject
    {
        public List<int> m_ints;
        public List<float> m_times;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_ints = des.ReadInt32Array(br);
            m_times = des.ReadSingleArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
