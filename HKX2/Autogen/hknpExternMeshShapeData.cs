using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpExternMeshShapeData : hkReferencedObject
    {
        public hkcdStaticTreeDefaultTreeStorage6 m_aabbTree;
        public hkcdSimdTree m_simdTree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabbTree = new hkcdStaticTreeDefaultTreeStorage6();
            m_aabbTree.Read(des, br);
            m_simdTree = new hkcdSimdTree();
            m_simdTree.Read(des, br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabbTree.Write(bw);
            m_simdTree.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
