using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkFreeListArrayhknpShapeInstancehkHandleshort32767hknpShapeInstanceIdDiscriminant8hknpShapeInstance : IHavokObject
    {
        public List<hknpShapeInstance> m_elements;
        public int m_firstFree;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elements = des.ReadClassArray<hknpShapeInstance>(br);
            m_firstFree = br.ReadInt32();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_firstFree);
            bw.WriteUInt32(0);
        }
    }
}
