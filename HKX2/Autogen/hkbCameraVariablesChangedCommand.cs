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
    }
}
