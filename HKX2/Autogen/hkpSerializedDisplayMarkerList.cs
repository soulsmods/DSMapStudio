using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSerializedDisplayMarkerList : hkReferencedObject
    {
        public List<hkpSerializedDisplayMarker> m_markers;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_markers = des.ReadClassPointerArray<hkpSerializedDisplayMarker>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
