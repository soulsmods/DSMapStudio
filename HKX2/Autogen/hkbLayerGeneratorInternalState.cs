using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLayerGeneratorInternalState : hkReferencedObject
    {
        public int m_numActiveLayers;
        public List<hkbLayerGeneratorLayerInternalState> m_layerInternalStates;
        public bool m_initSync;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numActiveLayers = br.ReadInt32();
            br.ReadUInt32();
            m_layerInternalStates = des.ReadClassArray<hkbLayerGeneratorLayerInternalState>(br);
            m_initSync = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_numActiveLayers);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_initSync);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
