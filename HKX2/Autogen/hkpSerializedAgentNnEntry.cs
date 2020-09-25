using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SerializedAgentType
    {
        INVALID_AGENT_TYPE = 0,
        BOX_BOX_AGENT3 = 1,
        CAPSULE_TRIANGLE_AGENT3 = 2,
        PRED_GSK_AGENT3 = 3,
        PRED_GSK_CYLINDER_AGENT3 = 4,
        CONVEX_LIST_AGENT3 = 5,
        LIST_AGENT3 = 6,
        BV_TREE_AGENT3 = 7,
        COLLECTION_COLLECTION_AGENT3 = 8,
        COLLECTION_AGENT3 = 9,
    }
    
    public class hkpSerializedAgentNnEntry : hkReferencedObject
    {
        public hkpEntity m_bodyA;
        public hkpEntity m_bodyB;
        public ulong m_bodyAId;
        public ulong m_bodyBId;
        public bool m_useEntityIds;
        public SerializedAgentType m_agentType;
        public hkpSimpleContactConstraintAtom m_atom;
        public List<byte> m_propertiesStream;
        public List<hkContactPoint> m_contactPoints;
        public List<byte> m_cpIdMgr;
        public byte m_nnEntryData;
        public hkpSerializedTrack1nInfo m_trackInfo;
        public byte m_endianCheckBuffer;
        public uint m_version;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_bodyA = des.ReadClassPointer<hkpEntity>(br);
            m_bodyB = des.ReadClassPointer<hkpEntity>(br);
            m_bodyAId = br.ReadUInt64();
            m_bodyBId = br.ReadUInt64();
            m_useEntityIds = br.ReadBoolean();
            m_agentType = (SerializedAgentType)br.ReadSByte();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_atom = new hkpSimpleContactConstraintAtom();
            m_atom.Read(des, br);
            m_propertiesStream = des.ReadByteArray(br);
            m_contactPoints = des.ReadClassArray<hkContactPoint>(br);
            m_cpIdMgr = des.ReadByteArray(br);
            m_nnEntryData = br.ReadByte();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_trackInfo = new hkpSerializedTrack1nInfo();
            m_trackInfo.Read(des, br);
            m_endianCheckBuffer = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_version = br.ReadUInt32();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(m_bodyAId);
            bw.WriteUInt64(m_bodyBId);
            bw.WriteBoolean(m_useEntityIds);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            m_atom.Write(bw);
            bw.WriteByte(m_nnEntryData);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            m_trackInfo.Write(bw);
            bw.WriteByte(m_endianCheckBuffer);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteUInt32(m_version);
            bw.WriteUInt64(0);
        }
    }
}
