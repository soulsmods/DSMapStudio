using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavVolumePathSearchParameters : IHavokObject
    {
        public enum LineOfSightFlags
        {
            NO_LINE_OF_SIGHT_CHECK = 0,
            EARLY_OUT_IF_NO_COST_MODIFIER = 1,
            EARLY_OUT_ALWAYS = 4,
        }
        
        public Vector4 m_up;
        public byte m_lineOfSightFlags;
        public float m_heuristicWeight;
        public float m_maximumPathLength;
        public hkaiSearchParametersBufferSizes m_bufferSizes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_up = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_lineOfSightFlags = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_heuristicWeight = br.ReadSingle();
            m_maximumPathLength = br.ReadSingle();
            m_bufferSizes = new hkaiSearchParametersBufferSizes();
            m_bufferSizes.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_heuristicWeight);
            bw.WriteSingle(m_maximumPathLength);
            m_bufferSizes.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
