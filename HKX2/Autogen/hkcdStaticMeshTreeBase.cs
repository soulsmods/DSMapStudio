using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CompressionMode
    {
        CM_GLOBAL = 0,
        CM_LOCAL_4 = 1,
        CM_LOCAL_2 = 2,
        CM_AUTO = 3,
    }
    
    public class hkcdStaticMeshTreeBase : hkcdStaticTreeTreehkcdStaticTreeDynamicStorage5
    {
        public int m_numPrimitiveKeys;
        public int m_bitsPerKey;
        public uint m_maxKeyValue;
        public List<hkcdStaticMeshTreeBaseSection> m_sections;
        public List<hkcdStaticMeshTreeBasePrimitive> m_primitives;
        public List<ushort> m_sharedVerticesIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numPrimitiveKeys = br.ReadInt32();
            m_bitsPerKey = br.ReadInt32();
            m_maxKeyValue = br.ReadUInt32();
            br.AssertUInt32(0);
            m_sections = des.ReadClassArray<hkcdStaticMeshTreeBaseSection>(br);
            m_primitives = des.ReadClassArray<hkcdStaticMeshTreeBasePrimitive>(br);
            m_sharedVerticesIndex = des.ReadUInt16Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_numPrimitiveKeys);
            bw.WriteInt32(m_bitsPerKey);
            bw.WriteUInt32(m_maxKeyValue);
            bw.WriteUInt32(0);
        }
    }
}
