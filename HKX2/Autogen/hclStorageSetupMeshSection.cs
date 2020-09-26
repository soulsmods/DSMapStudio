using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclStorageSetupMeshSection : hkReferencedObject
    {
        public hclSetupMesh m_parentSetupMesh;
        public List<Vector4> m_vertices;
        public List<Vector4> m_normals;
        public List<Vector4> m_tangents;
        public List<Vector4> m_bitangents;
        public List<hclSetupMeshSectionTriangle> m_triangles;
        public List<hclStorageSetupMeshSectionSectionVertexChannel> m_sectionVertexChannels;
        public List<hclStorageSetupMeshSectionSectionEdgeSelectionChannel> m_sectionEdgeChannels;
        public List<hclStorageSetupMeshSectionSectionTriangleSelectionChannel> m_sectionTriangleChannels;
        public List<hclStorageSetupMeshSectionBoneInfluences> m_boneInfluences;
        public List<ushort> m_normalIDs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_parentSetupMesh = des.ReadClassPointer<hclSetupMesh>(br);
            m_vertices = des.ReadVector4Array(br);
            m_normals = des.ReadVector4Array(br);
            m_tangents = des.ReadVector4Array(br);
            m_bitangents = des.ReadVector4Array(br);
            m_triangles = des.ReadClassArray<hclSetupMeshSectionTriangle>(br);
            m_sectionVertexChannels = des.ReadClassPointerArray<hclStorageSetupMeshSectionSectionVertexChannel>(br);
            m_sectionEdgeChannels = des.ReadClassPointerArray<hclStorageSetupMeshSectionSectionEdgeSelectionChannel>(br);
            m_sectionTriangleChannels = des.ReadClassPointerArray<hclStorageSetupMeshSectionSectionTriangleSelectionChannel>(br);
            m_boneInfluences = des.ReadClassPointerArray<hclStorageSetupMeshSectionBoneInfluences>(br);
            m_normalIDs = des.ReadUInt16Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
