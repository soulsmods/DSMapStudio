using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxSkinBinding : hkReferencedObject
    {
        public List<string> m_nodeNames;
        public List<Matrix4x4> m_bindPose;
        public Matrix4x4 m_initSkinTransform;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_nodeNames = des.ReadStringPointerArray(br);
            m_bindPose = des.ReadMatrix4Array(br);
            br.AssertUInt64(0);
            m_initSkinTransform = des.ReadMatrix4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
