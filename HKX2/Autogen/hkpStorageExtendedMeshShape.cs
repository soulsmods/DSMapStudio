using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStorageExtendedMeshShape : hkpExtendedMeshShape
    {
        public List<hkpStorageExtendedMeshShapeMeshSubpartStorage> m_meshstorage;
        public List<hkpStorageExtendedMeshShapeShapeSubpartStorage> m_shapestorage;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_meshstorage = des.ReadClassPointerArray<hkpStorageExtendedMeshShapeMeshSubpartStorage>(br);
            m_shapestorage = des.ReadClassPointerArray<hkpStorageExtendedMeshShapeShapeSubpartStorage>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
