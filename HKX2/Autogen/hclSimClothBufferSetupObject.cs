using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothBufferSetupObject : hclBufferSetupObject
    {
        public enum Type
        {
            SIM_CLOTH_MESH_CURRENT_POSITIONS = 0,
            SIM_CLOTH_MESH_PREVIOUS_POSITIONS = 1,
            SIM_CLOTH_MESH_ORIGINAL_POSE = 2,
        }
        
        public Type m_type;
        public string m_name;
        public hclSimClothSetupObject m_simClothSetupObject;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (Type)br.ReadUInt32();
            br.ReadUInt32();
            m_name = des.ReadStringPointer(br);
            m_simClothSetupObject = des.ReadClassPointer<hclSimClothSetupObject>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(0);
            // Implement Write
        }
    }
}
