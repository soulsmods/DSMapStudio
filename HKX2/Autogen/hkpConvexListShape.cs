using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConvexListShape : hkpConvexShape
    {
        public float m_minDistanceToUseConvexHullForGetClosestPoints;
        public Vector4 m_aabbHalfExtents;
        public Vector4 m_aabbCenter;
        public bool m_useCachedAabb;
        public List<hkpConvexShape> m_childShapes;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_minDistanceToUseConvexHullForGetClosestPoints = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_aabbHalfExtents = des.ReadVector4(br);
            m_aabbCenter = des.ReadVector4(br);
            m_useCachedAabb = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_childShapes = des.ReadClassPointerArray<hkpConvexShape>(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_minDistanceToUseConvexHullForGetClosestPoints);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_useCachedAabb);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteUInt64(0);
        }
    }
}
