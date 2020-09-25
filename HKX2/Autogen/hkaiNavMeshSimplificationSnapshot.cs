using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavMeshSimplificationSnapshot : IHavokObject
    {
        public hkGeometry m_geometry;
        public List<hkaiCarver> m_carvers;
        public hkBitField m_cuttingTriangles;
        public hkaiNavMeshGenerationSettings m_settings;
        public hkaiNavMesh m_unsimplifiedNavMesh;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_geometry = des.ReadClassPointer<hkGeometry>(br);
            m_carvers = des.ReadClassPointerArray<hkaiCarver>(br);
            m_cuttingTriangles = new hkBitField();
            m_cuttingTriangles.Read(des, br);
            m_settings = new hkaiNavMeshGenerationSettings();
            m_settings.Read(des, br);
            m_unsimplifiedNavMesh = des.ReadClassPointer<hkaiNavMesh>(br);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            m_cuttingTriangles.Write(bw);
            m_settings.Write(bw);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
