using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavVolumeInstance : hkReferencedObject
    {
        public hkaiNavVolume m_originalVolume;
        public List<int> m_cellMap;
        public List<hkaiNavVolumeInstanceCellInstance> m_instancedCells;
        public List<hkaiNavVolumeEdge> m_ownedEdges;
        public uint m_sectionUid;
        public int m_runtimeId;
        public uint m_layer;
        public Vector4 m_translation;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_originalVolume = des.ReadClassPointer<hkaiNavVolume>(br);
            m_cellMap = des.ReadInt32Array(br);
            m_instancedCells = des.ReadClassArray<hkaiNavVolumeInstanceCellInstance>(br);
            m_ownedEdges = des.ReadClassArray<hkaiNavVolumeEdge>(br);
            m_sectionUid = br.ReadUInt32();
            m_runtimeId = br.ReadInt32();
            m_layer = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_translation = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            // Implement Write
            bw.WriteUInt32(m_sectionUid);
            bw.WriteInt32(m_runtimeId);
            bw.WriteUInt32(m_layer);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
