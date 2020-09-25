using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCameraVariablesChangedCommand : hkReferencedObject
    {
        public List<string> m_cameraVariableFloatNames;
        public List<float> m_cameraFloatValues;
        public List<string> m_cameraVariableVectorNames;
        public List<Vector4> m_cameraVectorValues;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_cameraVariableFloatNames = des.ReadStringPointerArray(br);
            m_cameraFloatValues = des.ReadSingleArray(br);
            m_cameraVariableVectorNames = des.ReadStringPointerArray(br);
            m_cameraVectorValues = des.ReadVector4Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
