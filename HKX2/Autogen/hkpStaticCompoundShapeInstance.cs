using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStaticCompoundShapeInstance : IHavokObject
    {
        public Matrix4x4 m_transform;
        public hkpShape m_shape;
        public uint m_filterInfo;
        public uint m_childFilterInfoMask;
        public ulong m_userData;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transform = des.ReadQSTransform(br);
            m_shape = des.ReadClassPointer<hkpShape>(br);
            m_filterInfo = br.ReadUInt32();
            m_childFilterInfoMask = br.ReadUInt32();
            m_userData = br.ReadUInt64();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt32(m_filterInfo);
            bw.WriteUInt32(m_childFilterInfoMask);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt64(0);
        }
    }
}
