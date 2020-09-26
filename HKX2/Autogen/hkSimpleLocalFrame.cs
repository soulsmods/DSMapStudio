using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSimpleLocalFrame : hkLocalFrame
    {
        public Matrix4x4 m_transform;
        public List<hkLocalFrame> m_children;
        public hkLocalFrame m_parentFrame;
        public hkLocalFrameGroup m_group;
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transform = des.ReadTransform(br);
            m_children = des.ReadClassPointerArray<hkLocalFrame>(br);
            m_parentFrame = des.ReadClassPointer<hkLocalFrame>(br);
            m_group = des.ReadClassPointer<hkLocalFrameGroup>(br);
            m_name = des.ReadStringPointer(br);
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
