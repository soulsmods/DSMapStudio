using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxMesh : hkReferencedObject
    {
        public List<hkxMeshSection> m_sections;
        public List<hkxMeshUserChannelInfo> m_userChannelInfos;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sections = des.ReadClassPointerArray<hkxMeshSection>(br);
            m_userChannelInfos = des.ReadClassPointerArray<hkxMeshUserChannelInfo>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
