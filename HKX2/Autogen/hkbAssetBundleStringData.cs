using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbAssetBundleStringData : IHavokObject
    {
        public string m_bundleName;
        public List<string> m_assetNames;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_bundleName = des.ReadStringPointer(br);
            m_assetNames = des.ReadStringPointerArray(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
