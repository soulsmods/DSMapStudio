using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class CustomParamStorageMeshShape : hkpStorageMeshShape
    {
        public List<CustomMeshParameter> m_materialArray;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_materialArray = des.ReadClassPointerArray<CustomMeshParameter>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
