using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclClothStateTransformSetAccess : IHavokObject
    {
        public uint m_transformSetIndex;
        public hclTransformSetUsage m_transformSetUsage;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transformSetIndex = br.ReadUInt32();
            br.AssertUInt32(0);
            m_transformSetUsage = new hclTransformSetUsage();
            m_transformSetUsage.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_transformSetIndex);
            bw.WriteUInt32(0);
            m_transformSetUsage.Write(bw);
        }
    }
}
