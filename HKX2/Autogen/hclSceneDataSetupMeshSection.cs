using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSceneDataSetupMeshSection : hkReferencedObject
    {
        public hclSceneDataSetupMesh m_setupMesh;
        public hkxMeshSection m_meshSection;
        public bool m_skinnedSection;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_setupMesh = des.ReadClassPointer<hclSceneDataSetupMesh>(br);
            m_meshSection = des.ReadClassPointer<hkxMeshSection>(br);
            m_skinnedSection = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
            // Implement Write
            bw.WriteBoolean(m_skinnedSection);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
