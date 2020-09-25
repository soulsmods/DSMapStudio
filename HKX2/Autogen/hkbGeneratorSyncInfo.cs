using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorSyncInfo : IHavokObject
    {
        public hkbGeneratorSyncInfoSyncPoint m_syncPoints;
        public float m_duration;
        public float m_localTime;
        public float m_playbackSpeed;
        public sbyte m_numSyncPoints;
        public bool m_isCyclic;
        public bool m_isMirrored;
        public bool m_isAdditive;
        public hkbGeneratorSyncInfoActiveInterval m_activeInterval;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_syncPoints = new hkbGeneratorSyncInfoSyncPoint();
            m_syncPoints.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_duration = br.ReadSingle();
            m_localTime = br.ReadSingle();
            m_playbackSpeed = br.ReadSingle();
            m_numSyncPoints = br.ReadSByte();
            m_isCyclic = br.ReadBoolean();
            m_isMirrored = br.ReadBoolean();
            m_isAdditive = br.ReadBoolean();
            m_activeInterval = new hkbGeneratorSyncInfoActiveInterval();
            m_activeInterval.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_syncPoints.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_duration);
            bw.WriteSingle(m_localTime);
            bw.WriteSingle(m_playbackSpeed);
            bw.WriteSByte(m_numSyncPoints);
            bw.WriteBoolean(m_isCyclic);
            bw.WriteBoolean(m_isMirrored);
            bw.WriteBoolean(m_isAdditive);
            m_activeInterval.Write(bw);
        }
    }
}
