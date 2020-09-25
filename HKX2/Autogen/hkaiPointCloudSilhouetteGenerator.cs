using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum DetailLevel
    {
        DETAIL_INVALID = 0,
        DETAIL_FULL = 1,
        DETAIL_OBB = 2,
        DETAIL_CONVEX_HULL = 3,
    }
    
    public enum PointCloudFlagBits
    {
        LOCAL_POINTS_CHANGED = 1,
    }
    
    public class hkaiPointCloudSilhouetteGenerator : hkaiSilhouetteGenerator
    {
        public hkAabb m_localAabb;
        public List<Vector4> m_localPoints;
        public List<int> m_silhouetteSizes;
        public float m_weldTolerance;
        public DetailLevel m_silhouetteDetailLevel;
        public byte m_flags;
        public bool m_localPointsChanged;
        public bool m_isEnabled;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_localAabb = new hkAabb();
            m_localAabb.Read(des, br);
            m_localPoints = des.ReadVector4Array(br);
            m_silhouetteSizes = des.ReadInt32Array(br);
            m_weldTolerance = br.ReadSingle();
            m_silhouetteDetailLevel = (DetailLevel)br.ReadByte();
            m_flags = br.ReadByte();
            m_localPointsChanged = br.ReadBoolean();
            m_isEnabled = br.ReadBoolean();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_localAabb.Write(bw);
            bw.WriteSingle(m_weldTolerance);
            bw.WriteBoolean(m_localPointsChanged);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteUInt64(0);
        }
    }
}
