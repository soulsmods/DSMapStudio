using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpDynamicCompoundShapeData : hkReferencedObject
    {
        public hknpDynamicCompoundShapeTree m_aabbTree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabbTree = new hknpDynamicCompoundShapeTree();
            m_aabbTree.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabbTree.Write(bw);
        }
    }
}
