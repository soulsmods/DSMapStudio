using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMotionPropertiesLibrary : hkReferencedObject
    {
        public hkFreeListArrayhknpMotionPropertieshknpMotionPropertiesId8hknpMotionPropertiesFreeListArrayOperations m_entries;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            m_entries = new hkFreeListArrayhknpMotionPropertieshknpMotionPropertiesId8hknpMotionPropertiesFreeListArrayOperations();
            m_entries.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            m_entries.Write(bw);
        }
    }
}
