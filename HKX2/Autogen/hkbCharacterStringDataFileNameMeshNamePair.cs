using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterStringDataFileNameMeshNamePair : IHavokObject
    {
        public string m_fileName;
        public string m_meshName;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_fileName = des.ReadStringPointer(br);
            m_meshName = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
