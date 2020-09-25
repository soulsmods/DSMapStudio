using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxNode : hkxAttributeHolder
    {
        public string m_name;
        public hkReferencedObject m_object;
        public List<Matrix4x4> m_keyFrames;
        public List<hkxNode> m_children;
        public List<hkxNodeAnnotationData> m_annotations;
        public List<float> m_linearKeyFrameHints;
        public string m_userProperties;
        public bool m_selected;
        public bool m_bone;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            m_object = des.ReadClassPointer<hkReferencedObject>(br);
            m_keyFrames = des.ReadMatrix4Array(br);
            m_children = des.ReadClassPointerArray<hkxNode>(br);
            m_annotations = des.ReadClassArray<hkxNodeAnnotationData>(br);
            m_linearKeyFrameHints = des.ReadSingleArray(br);
            m_userProperties = des.ReadStringPointer(br);
            m_selected = br.ReadBoolean();
            m_bone = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteBoolean(m_selected);
            bw.WriteBoolean(m_bone);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
