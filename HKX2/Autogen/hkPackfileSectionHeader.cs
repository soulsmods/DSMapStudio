using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkPackfileSectionHeader
    {
        public char m_sectionTag;
        public char m_nullByte;
        public int m_absoluteDataStart;
        public int m_localFixupsOffset;
        public int m_globalFixupsOffset;
        public int m_virtualFixupsOffset;
        public int m_exportsOffset;
        public int m_importsOffset;
        public int m_endOffset;
        public int m_pad;
    }
}
