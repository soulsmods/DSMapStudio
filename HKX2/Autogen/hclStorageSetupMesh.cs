using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStorageSetupMesh : hclSetupMesh
    {
        public string m_name;
        public Matrix4x4 m_worldFromMesh;
        public List<hclStorageSetupMeshSection> m_sections;
        public List<hclStorageSetupMeshVertexChannel> m_vertexChannels;
        public List<hclStorageSetupMeshEdgeChannel> m_edgeChannels;
        public List<hclStorageSetupMeshTriangleChannel> m_triangleChannels;
        public List<hclStorageSetupMeshBone> m_bones;
        public bool m_isSkinned;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            br.ReadUInt64();
            m_worldFromMesh = des.ReadMatrix4(br);
            m_sections = des.ReadClassPointerArray<hclStorageSetupMeshSection>(br);
            m_vertexChannels = des.ReadClassArray<hclStorageSetupMeshVertexChannel>(br);
            m_edgeChannels = des.ReadClassArray<hclStorageSetupMeshEdgeChannel>(br);
            m_triangleChannels = des.ReadClassArray<hclStorageSetupMeshTriangleChannel>(br);
            m_bones = des.ReadClassArray<hclStorageSetupMeshBone>(br);
            m_isSkinned = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteBoolean(m_isSkinned);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
