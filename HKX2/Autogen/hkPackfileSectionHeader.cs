using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackfileSectionHeader : IHavokObject
    {
        public sbyte m_sectionTag;
        public sbyte m_nullByte;
        public int m_absoluteDataStart;
        public int m_localFixupsOffset;
        public int m_globalFixupsOffset;
        public int m_virtualFixupsOffset;
        public int m_exportsOffset;
        public int m_importsOffset;
        public int m_endOffset;
        public int m_pad;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_sectionTag = br.ReadSByte();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            m_nullByte = br.ReadSByte();
            m_absoluteDataStart = br.ReadInt32();
            m_localFixupsOffset = br.ReadInt32();
            m_globalFixupsOffset = br.ReadInt32();
            m_virtualFixupsOffset = br.ReadInt32();
            m_exportsOffset = br.ReadInt32();
            m_importsOffset = br.ReadInt32();
            m_endOffset = br.ReadInt32();
            m_pad = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSByte(m_sectionTag);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteSByte(m_nullByte);
            bw.WriteInt32(m_absoluteDataStart);
            bw.WriteInt32(m_localFixupsOffset);
            bw.WriteInt32(m_globalFixupsOffset);
            bw.WriteInt32(m_virtualFixupsOffset);
            bw.WriteInt32(m_exportsOffset);
            bw.WriteInt32(m_importsOffset);
            bw.WriteInt32(m_endOffset);
            bw.WriteInt32(m_pad);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
