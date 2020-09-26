using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoseStoringGeneratorOutputListenerStoredPose : hkReferencedObject
    {
        public hkbNode m_node;
        public List<Matrix4x4> m_pose;
        public Matrix4x4 m_worldFromModel;
        public bool m_isPoseValid;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_node = des.ReadClassPointer<hkbNode>(br);
            m_pose = des.ReadQSTransformArray(br);
            br.ReadUInt64();
            m_worldFromModel = des.ReadQSTransform(br);
            m_isPoseValid = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteBoolean(m_isPoseValid);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
