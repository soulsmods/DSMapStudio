using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiUserEdgeSetupArray : hkReferencedObject
    {
        public List<hkaiUserEdgeUtilsUserEdgeSetup> m_edgeSetups;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_edgeSetups = des.ReadClassArray<hkaiUserEdgeUtilsUserEdgeSetup>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
