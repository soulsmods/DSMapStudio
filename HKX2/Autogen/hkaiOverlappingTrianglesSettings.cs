using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiOverlappingTrianglesSettings : IHavokObject
    {
        public float m_coplanarityTolerance;
        public float m_raycastLengthMultiplier;
        public WalkableTriangleSettings m_walkableTriangleSettings;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_coplanarityTolerance = br.ReadSingle();
            m_raycastLengthMultiplier = br.ReadSingle();
            m_walkableTriangleSettings = (WalkableTriangleSettings)br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_coplanarityTolerance);
            bw.WriteSingle(m_raycastLengthMultiplier);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
