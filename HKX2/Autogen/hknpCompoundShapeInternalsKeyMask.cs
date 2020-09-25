using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCompoundShapeInternalsKeyMask : hknpCompoundShapeKeyMask
    {
        public hknpCompoundShape m_shape;
        public List<hknpShapeKeyMask> m_instanceMasks;
        public List<uint> m_filter;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_shape = des.ReadClassPointer<hknpCompoundShape>(br);
            m_instanceMasks = des.ReadClassPointerArray<hknpShapeKeyMask>(br);
            m_filter = des.ReadUInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
