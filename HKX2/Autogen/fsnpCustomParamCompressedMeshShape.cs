using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class fsnpCustomParamCompressedMeshShape : hknpCompressedMeshShape
    {
        public fsnpCustomMeshParameter m_pParam;
        public List<uint> m_triangleIndexToShapeKey;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pParam = des.ReadClassPointer<fsnpCustomMeshParameter>(br);
            m_triangleIndexToShapeKey = des.ReadUInt32Array(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
