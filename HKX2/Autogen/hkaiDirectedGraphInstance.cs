using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiDirectedGraphInstance : hkReferencedObject
    {
        public uint m_sectionUid;
        public int m_runtimeId;
        public hkaiDirectedGraphExplicitCost m_originalGraph;
        public List<int> m_nodeMap;
        public List<hkaiDirectedGraphExplicitCostNode> m_instancedNodes;
        public List<hkaiDirectedGraphExplicitCostEdge> m_ownedEdges;
        public List<uint> m_ownedEdgeData;
        public List<int> m_userEdgeCount;
        public List<hkaiDirectedGraphInstanceFreeBlockList> m_freeEdgeBlocks;
        public Matrix4x4 m_transform;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_sectionUid = br.ReadUInt32();
            m_runtimeId = br.ReadInt32();
            br.AssertUInt32(0);
            m_originalGraph = des.ReadClassPointer<hkaiDirectedGraphExplicitCost>(br);
            m_nodeMap = des.ReadInt32Array(br);
            m_instancedNodes = des.ReadClassArray<hkaiDirectedGraphExplicitCostNode>(br);
            m_ownedEdges = des.ReadClassArray<hkaiDirectedGraphExplicitCostEdge>(br);
            m_ownedEdgeData = des.ReadUInt32Array(br);
            m_userEdgeCount = des.ReadInt32Array(br);
            m_freeEdgeBlocks = des.ReadClassArray<hkaiDirectedGraphInstanceFreeBlockList>(br);
            br.AssertUInt64(0);
            m_transform = des.ReadTransform(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_sectionUid);
            bw.WriteInt32(m_runtimeId);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
