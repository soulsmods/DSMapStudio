using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavMeshPathRequestInfo : hkReferencedObject
    {
        public hkaiPathfindingUtilFindPathInput m_input;
        public hkaiPathfindingUtilFindPathOutput m_output;
        public int m_priority;
        public bool m_markedForDeletion;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_input = des.ReadClassPointer<hkaiPathfindingUtilFindPathInput>(br);
            m_output = des.ReadClassPointer<hkaiPathfindingUtilFindPathOutput>(br);
            m_priority = br.ReadInt32();
            br.ReadUInt64();
            br.ReadUInt32();
            m_markedForDeletion = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
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
