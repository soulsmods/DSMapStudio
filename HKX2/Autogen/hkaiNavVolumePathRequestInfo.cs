using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavVolumePathRequestInfo : hkReferencedObject
    {
        public hkaiVolumePathfindingUtilFindPathInput m_input;
        public hkaiVolumePathfindingUtilFindPathOutput m_output;
        public int m_priority;
        public bool m_markedForDeletion;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_input = des.ReadClassPointer<hkaiVolumePathfindingUtilFindPathInput>(br);
            m_output = des.ReadClassPointer<hkaiVolumePathfindingUtilFindPathOutput>(br);
            m_priority = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_markedForDeletion = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteInt32(m_priority);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_markedForDeletion);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
