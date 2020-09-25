using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticMeshTreeBaseSection : hkcdStaticTreeTreehkcdStaticTreeDynamicStorage4
    {
        public enum Flags
        {
            SF_REQUIRE_TREE = 1,
        }
        
        public float m_codecParms;
        public uint m_firstPackedVertex;
        public hkcdStaticMeshTreeBaseSectionSharedVertices m_sharedVertices;
        public hkcdStaticMeshTreeBaseSectionPrimitives m_primitives;
        public hkcdStaticMeshTreeBaseSectionDataRuns m_dataRuns;
        public byte m_numPackedVertices;
        public byte m_numSharedIndices;
        public ushort m_leafIndex;
        public byte m_page;
        public byte m_flags;
        public byte m_layerData;
        public byte m_unusedData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_codecParms = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_firstPackedVertex = br.ReadUInt32();
            m_sharedVertices = new hkcdStaticMeshTreeBaseSectionSharedVertices();
            m_sharedVertices.Read(des, br);
            m_primitives = new hkcdStaticMeshTreeBaseSectionPrimitives();
            m_primitives.Read(des, br);
            m_dataRuns = new hkcdStaticMeshTreeBaseSectionDataRuns();
            m_dataRuns.Read(des, br);
            m_numPackedVertices = br.ReadByte();
            m_numSharedIndices = br.ReadByte();
            m_leafIndex = br.ReadUInt16();
            m_page = br.ReadByte();
            m_flags = br.ReadByte();
            m_layerData = br.ReadByte();
            m_unusedData = br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_codecParms);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_firstPackedVertex);
            m_sharedVertices.Write(bw);
            m_primitives.Write(bw);
            m_dataRuns.Write(bw);
            bw.WriteByte(m_numPackedVertices);
            bw.WriteByte(m_numSharedIndices);
            bw.WriteUInt16(m_leafIndex);
            bw.WriteByte(m_page);
            bw.WriteByte(m_flags);
            bw.WriteByte(m_layerData);
            bw.WriteByte(m_unusedData);
        }
    }
}
