using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDetectCloseToGroundModifier : hkbModifier
    {
        public hkbEventProperty m_closeToGroundEvent;
        public float m_closeToGroundHeight;
        public float m_raycastDistanceDown;
        public uint m_collisionFilterInfo;
        public short m_boneIndex;
        public short m_animBoneIndex;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_closeToGroundEvent = new hkbEventProperty();
            m_closeToGroundEvent.Read(des, br);
            m_closeToGroundHeight = br.ReadSingle();
            m_raycastDistanceDown = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            m_boneIndex = br.ReadInt16();
            m_animBoneIndex = br.ReadInt16();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_closeToGroundEvent.Write(bw);
            bw.WriteSingle(m_closeToGroundHeight);
            bw.WriteSingle(m_raycastDistanceDown);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteInt16(m_boneIndex);
            bw.WriteInt16(m_animBoneIndex);
            bw.WriteUInt64(0);
        }
    }
}
