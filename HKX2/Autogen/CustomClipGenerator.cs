using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class CustomClipGenerator : hkbGenerator
    {
        public enum PlaybackMode
        {
            MODE_SINGLE_PLAY = 0,
            MODE_LOOPING = 1,
        }
        
        public enum ClipFlags
        {
            FLAG_CONTINUE_MOTION_AT_END = 1,
            FLAG_SYNC_HALF_CYCLE_IN_PING_PONG_MODE = 2,
            FLAG_MIRROR = 4,
            FLAG_FORCE_DENSE_POSE = 8,
            FLAG_DONT_CONVERT_ANNOTATIONS_TO_TRIGGERS = 16,
            FLAG_IGNORE_MOTION = 32,
        }
        
        public enum AnimeEndEventType
        {
            FireNextStateEvent = 0,
            FireStateEndEvent = 1,
            FireIdleEvent = 2,
        }
        
        public float m_playbackSpeed;
        public PlaybackMode m_mode;
        public int m_animId;
        public AnimeEndEventType m_animeEndEventType;
        public hkbEvent m_endEvent;
        public float m_cropStartAmountLocalTime;
        public float m_cropEndAmountLocalTime;
        public float m_startTime;
        public float m_enforcedDuration;
        public sbyte m_flags;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_playbackSpeed = br.ReadSingle();
            m_mode = (PlaybackMode)br.ReadSByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_animId = br.ReadInt32();
            m_animeEndEventType = (AnimeEndEventType)br.ReadInt32();
            m_endEvent = new hkbEvent();
            m_endEvent.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_cropStartAmountLocalTime = br.ReadSingle();
            m_cropEndAmountLocalTime = br.ReadSingle();
            m_startTime = br.ReadSingle();
            m_enforcedDuration = br.ReadSingle();
            m_flags = br.ReadSByte();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_playbackSpeed);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteInt32(m_animId);
            m_endEvent.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_cropStartAmountLocalTime);
            bw.WriteSingle(m_cropEndAmountLocalTime);
            bw.WriteSingle(m_startTime);
            bw.WriteSingle(m_enforcedDuration);
            bw.WriteSByte(m_flags);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
