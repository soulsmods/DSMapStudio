using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ListShapeFlags
    {
        ALL_FLAGS_CLEAR = 0,
        DISABLE_SPU_CACHE_FOR_LIST_CHILD_INFO = 1,
    }
    
    public class hkpListShape : hkpShapeCollection
    {
        public List<hkpListShapeChildInfo> m_childInfo;
        public ushort m_flags;
        public ushort m_numDisabledChildren;
        public Vector4 m_aabbHalfExtents;
        public Vector4 m_aabbCenter;
        public uint m_enabledChildren;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childInfo = des.ReadClassArray<hkpListShapeChildInfo>(br);
            m_flags = br.ReadUInt16();
            m_numDisabledChildren = br.ReadUInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_aabbHalfExtents = des.ReadVector4(br);
            m_aabbCenter = des.ReadVector4(br);
            m_enabledChildren = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(m_flags);
            bw.WriteUInt16(m_numDisabledChildren);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_enabledChildren);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
