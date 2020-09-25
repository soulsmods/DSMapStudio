using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompressedMeshShapeData : hkReferencedObject
    {
        public hknpCompressedMeshShapeTree m_meshTree;
        public hkcdSimdTree m_simdTree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_meshTree = new hknpCompressedMeshShapeTree();
            m_meshTree.Read(des, br);
            m_simdTree = new hkcdSimdTree();
            m_simdTree.Read(des, br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_meshTree.Write(bw);
            m_simdTree.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
