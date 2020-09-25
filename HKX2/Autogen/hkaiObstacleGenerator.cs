using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiObstacleGenerator : hkReferencedObject
    {
        public bool m_useSpheres;
        public bool m_useBoundaries;
        public bool m_clipBoundaries;
        public Matrix4x4 m_transform;
        public List<hkaiAvoidanceSolverSphereObstacle> m_spheres;
        public List<hkaiAvoidanceSolverBoundaryObstacle> m_boundaries;
        public ulong m_userData;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_useSpheres = br.ReadBoolean();
            m_useBoundaries = br.ReadBoolean();
            m_clipBoundaries = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
            m_transform = des.ReadTransform(br);
            m_spheres = des.ReadClassArray<hkaiAvoidanceSolverSphereObstacle>(br);
            m_boundaries = des.ReadClassArray<hkaiAvoidanceSolverBoundaryObstacle>(br);
            m_userData = br.ReadUInt64();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_useSpheres);
            bw.WriteBoolean(m_useBoundaries);
            bw.WriteBoolean(m_clipBoundaries);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt64(0);
        }
    }
}
