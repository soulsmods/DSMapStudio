using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiLineOfSightUtilLineOfSightInput : hkaiLineOfSightUtilInputBase
    {
        public Vector4 m_goalPoint;
        public uint m_goalFaceKey;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_goalPoint = des.ReadVector4(br);
            m_goalFaceKey = br.ReadUInt32();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_goalFaceKey);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
