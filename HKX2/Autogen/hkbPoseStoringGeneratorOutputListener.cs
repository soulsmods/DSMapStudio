using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoseStoringGeneratorOutputListener : hkbGeneratorOutputListener
    {
        public List<hkbPoseStoringGeneratorOutputListenerStoredPose> m_storedPoses;
        public bool m_dirty;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storedPoses = des.ReadClassPointerArray<hkbPoseStoringGeneratorOutputListenerStoredPose>(br);
            m_dirty = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_dirty);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
