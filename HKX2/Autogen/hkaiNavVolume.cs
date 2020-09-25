using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CellEdgeFlagBits
    {
        EDGE_EXTERNAL_OPPOSITE = 64,
    }
    
    public class hkaiNavVolume : hkReferencedObject
    {
        public enum Constants
        {
            INVALID_CELL_INDEX = -1,
            INVALID_EDGE_INDEX = -1,
        }
        
        public List<hkaiNavVolumeCell> m_cells;
        public List<hkaiNavVolumeEdge> m_edges;
        public List<hkaiStreamingSet> m_streamingSets;
        public hkAabb m_aabb;
        public Vector4 m_quantizationScale;
        public Vector4 m_quantizationOffset;
        public int m_res;
        public ulong m_userData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_cells = des.ReadClassArray<hkaiNavVolumeCell>(br);
            m_edges = des.ReadClassArray<hkaiNavVolumeEdge>(br);
            m_streamingSets = des.ReadClassArray<hkaiStreamingSet>(br);
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_quantizationScale = des.ReadVector4(br);
            m_quantizationOffset = des.ReadVector4(br);
            m_res = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_userData = br.ReadUInt64();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabb.Write(bw);
            bw.WriteInt32(m_res);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt64(0);
        }
    }
}
