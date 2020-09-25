using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackfileHeader : IHavokObject
    {
        public int m_magic;
        public int m_userTag;
        public int m_fileVersion;
        public byte m_layoutRules;
        public int m_numSections;
        public int m_contentsSectionIndex;
        public int m_contentsSectionOffset;
        public int m_contentsClassNameSectionIndex;
        public int m_contentsClassNameSectionOffset;
        public sbyte m_contentsVersion;
        public int m_flags;
        public ushort m_maxpredicate;
        public ushort m_predicateArraySizePlusPadding;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_magic = br.ReadInt32();
            br.AssertUInt32(0);
            m_userTag = br.ReadInt32();
            m_fileVersion = br.ReadInt32();
            m_layoutRules = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_numSections = br.ReadInt32();
            m_contentsSectionIndex = br.ReadInt32();
            m_contentsSectionOffset = br.ReadInt32();
            m_contentsClassNameSectionIndex = br.ReadInt32();
            m_contentsClassNameSectionOffset = br.ReadInt32();
            m_contentsVersion = br.ReadSByte();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_flags = br.ReadInt32();
            m_maxpredicate = br.ReadUInt16();
            m_predicateArraySizePlusPadding = br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_magic);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_userTag);
            bw.WriteInt32(m_fileVersion);
            bw.WriteByte(m_layoutRules);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteInt32(m_numSections);
            bw.WriteInt32(m_contentsSectionIndex);
            bw.WriteInt32(m_contentsSectionOffset);
            bw.WriteInt32(m_contentsClassNameSectionIndex);
            bw.WriteInt32(m_contentsClassNameSectionOffset);
            bw.WriteSByte(m_contentsVersion);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteInt32(m_flags);
            bw.WriteUInt16(m_maxpredicate);
            bw.WriteUInt16(m_predicateArraySizePlusPadding);
        }
    }
}
