using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConvexVerticesConnectivity : hkReferencedObject
    {
        public List<ushort> m_vertexIndices;
        public List<byte> m_numVerticesPerFace;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexIndices = des.ReadUInt16Array(br);
            m_numVerticesPerFace = des.ReadByteArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
