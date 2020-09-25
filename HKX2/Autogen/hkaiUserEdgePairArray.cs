using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiUserEdgePairArray : hkReferencedObject
    {
        public List<hkaiUserEdgeUtilsUserEdgePair> m_edgePairs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_edgePairs = des.ReadClassArray<hkaiUserEdgeUtilsUserEdgePair>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
