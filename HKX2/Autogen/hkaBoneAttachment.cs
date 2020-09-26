using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaBoneAttachment : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public Matrix4x4 m_boneFromAttachment;
        public hkReferencedObject m_attachment;
        public string m_name;
        public short m_boneIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_originalSkeletonName = des.ReadStringPointer(br);
            br.ReadUInt64();
            m_boneFromAttachment = des.ReadMatrix4(br);
            m_attachment = des.ReadClassPointer<hkReferencedObject>(br);
            m_name = des.ReadStringPointer(br);
            m_boneIndex = br.ReadInt16();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
            bw.WriteInt16(m_boneIndex);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
