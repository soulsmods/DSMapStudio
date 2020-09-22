using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackfileHeader
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
        public char m_contentsVersion;
        public int m_flags;
        public ushort m_maxpredicate;
        public ushort m_predicateArraySizePlusPadding;
    }
}
