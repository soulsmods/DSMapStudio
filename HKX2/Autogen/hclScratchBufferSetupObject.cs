using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclScratchBufferSetupObject : hclBufferSetupObject
    {
        public string m_name;
        public hclSetupMesh m_setupMesh;
        public bool m_storeNormals;
        public bool m_storeTangentsAndBiTangents;
        public bool m_storeTriangles;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_setupMesh = des.ReadClassPointer<hclSetupMesh>(br);
            m_storeNormals = br.ReadBoolean();
            m_storeTangentsAndBiTangents = br.ReadBoolean();
            m_storeTriangles = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteBoolean(m_storeNormals);
            bw.WriteBoolean(m_storeTangentsAndBiTangents);
            bw.WriteBoolean(m_storeTriangles);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
