using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiStreamingCollection : hkReferencedObject
    {
        public bool m_isTemporary;
        public hkcdDynamicAabbTree m_tree;
        public List<hkaiStreamingCollectionInstanceInfo> m_instances;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isTemporary = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
            m_tree = des.ReadClassPointer<hkcdDynamicAabbTree>(br);
            m_instances = des.ReadClassArray<hkaiStreamingCollectionInstanceInfo>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_isTemporary);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            // Implement Write
        }
    }
}
