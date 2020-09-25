using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpPhysicsSceneData : hkReferencedObject
    {
        public List<hknpPhysicsSystemData> m_systemDatas;
        public hknpRefWorldCinfo m_worldCinfo;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_systemDatas = des.ReadClassPointerArray<hknpPhysicsSystemData>(br);
            m_worldCinfo = des.ReadClassPointer<hknpRefWorldCinfo>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
