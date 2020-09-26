using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaSplineCompressedAnimationAnimationCompressionParams : IHavokObject
    {
        public ushort m_maxFramesPerBlock;
        public bool m_enableSampleSingleTracks;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_maxFramesPerBlock = br.ReadUInt16();
            m_enableSampleSingleTracks = br.ReadBoolean();
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_maxFramesPerBlock);
            bw.WriteBoolean(m_enableSampleSingleTracks);
            bw.WriteByte(0);
        }
    }
}
