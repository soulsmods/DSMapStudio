using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStorageMeshShape : hkpMeshShape
    {
        public List<hkpStorageMeshShapeSubpartStorage> m_storage;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadClassPointerArray<hkpStorageMeshShapeSubpartStorage>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
