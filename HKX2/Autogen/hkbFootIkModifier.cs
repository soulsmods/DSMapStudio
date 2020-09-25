using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum AlignMode
    {
        ALIGN_MODE_FORWARD_RIGHT = 0,
        ALIGN_MODE_FORWARD = 1,
    }
    
    public class hkbFootIkModifier : hkbModifier
    {
        public hkbFootIkGains m_gains;
        public List<hkbFootIkModifierLeg> m_legs;
        public float m_raycastDistanceUp;
        public float m_raycastDistanceDown;
        public float m_originalGroundHeightMS;
        public float m_errorOut;
        public float m_verticalOffset;
        public uint m_collisionFilterInfo;
        public float m_forwardAlignFraction;
        public float m_sidewaysAlignFraction;
        public float m_sidewaysSampleWidth;
        public bool m_useTrackData;
        public bool m_lockFeetWhenPlanted;
        public bool m_useCharacterUpVector;
        public bool m_keepSourceFootEndAboveGround;
        public AlignMode m_alignMode;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_gains = new hkbFootIkGains();
            m_gains.Read(des, br);
            m_legs = des.ReadClassArray<hkbFootIkModifierLeg>(br);
            m_raycastDistanceUp = br.ReadSingle();
            m_raycastDistanceDown = br.ReadSingle();
            m_originalGroundHeightMS = br.ReadSingle();
            m_errorOut = br.ReadSingle();
            m_verticalOffset = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            m_forwardAlignFraction = br.ReadSingle();
            m_sidewaysAlignFraction = br.ReadSingle();
            m_sidewaysSampleWidth = br.ReadSingle();
            m_useTrackData = br.ReadBoolean();
            m_lockFeetWhenPlanted = br.ReadBoolean();
            m_useCharacterUpVector = br.ReadBoolean();
            m_keepSourceFootEndAboveGround = br.ReadBoolean();
            m_alignMode = (AlignMode)br.ReadSByte();
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
            m_gains.Write(bw);
            bw.WriteSingle(m_raycastDistanceUp);
            bw.WriteSingle(m_raycastDistanceDown);
            bw.WriteSingle(m_originalGroundHeightMS);
            bw.WriteSingle(m_errorOut);
            bw.WriteSingle(m_verticalOffset);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteSingle(m_forwardAlignFraction);
            bw.WriteSingle(m_sidewaysAlignFraction);
            bw.WriteSingle(m_sidewaysSampleWidth);
            bw.WriteBoolean(m_useTrackData);
            bw.WriteBoolean(m_lockFeetWhenPlanted);
            bw.WriteBoolean(m_useCharacterUpVector);
            bw.WriteBoolean(m_keepSourceFootEndAboveGround);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
