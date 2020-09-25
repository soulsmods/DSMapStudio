using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiTraversalAnalysisInputSection : IHavokObject
    {
        public hkaiNavMeshInstance m_navMeshInstance;
        public hkGeometry m_geometry;
        public hkBitField m_walkableBitfield;
        public hkBitField m_cuttingBitfield;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_navMeshInstance = des.ReadClassPointer<hkaiNavMeshInstance>(br);
            m_geometry = des.ReadClassPointer<hkGeometry>(br);
            m_walkableBitfield = new hkBitField();
            m_walkableBitfield.Read(des, br);
            m_cuttingBitfield = new hkBitField();
            m_cuttingBitfield.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            // Implement Write
            m_walkableBitfield.Write(bw);
            m_cuttingBitfield.Write(bw);
        }
    }
}
