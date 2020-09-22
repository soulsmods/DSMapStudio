using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticPvs
    {
        public hkcdStaticTreeTreehkcdStaticTreeDynamicStorage6 m_cells;
        public int m_bytesPerCells;
        public int m_cellsPerBlock;
        public List<byte> m_pvs;
        public List<ushort> m_map;
        public List<hkcdStaticPvsBlockHeader> m_blocks;
    }
}
