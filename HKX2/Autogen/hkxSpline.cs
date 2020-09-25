using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ControlType
    {
        BEZIER_SMOOTH = 0,
        BEZIER_CORNER = 1,
        LINEAR = 2,
        CUSTOM = 3,
    }
    
    public class hkxSpline : hkReferencedObject
    {
        public List<hkxSplineControlPoint> m_controlPoints;
        public bool m_isClosed;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controlPoints = des.ReadClassArray<hkxSplineControlPoint>(br);
            m_isClosed = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_isClosed);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
