using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiTraversalAnalysisSettings : hkReferencedObject
    {
        public Vector4 m_up;
        public float m_minEdgeLength;
        public float m_maxSectionDistance;
        public float m_characterHeight;
        public float m_characterRadius;
        public float m_raiseEdgeHeightLimit;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_up = des.ReadVector4(br);
            m_minEdgeLength = br.ReadSingle();
            m_maxSectionDistance = br.ReadSingle();
            m_characterHeight = br.ReadSingle();
            m_characterRadius = br.ReadSingle();
            m_raiseEdgeHeightLimit = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_minEdgeLength);
            bw.WriteSingle(m_maxSectionDistance);
            bw.WriteSingle(m_characterHeight);
            bw.WriteSingle(m_characterRadius);
            bw.WriteSingle(m_raiseEdgeHeightLimit);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
