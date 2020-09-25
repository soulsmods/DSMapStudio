using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxBlobMeshShape : hkMeshShape
    {
        public hkxBlob m_blob;
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_blob = new hkxBlob();
            m_blob.Read(des, br);
            m_name = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_blob.Write(bw);
        }
    }
}
