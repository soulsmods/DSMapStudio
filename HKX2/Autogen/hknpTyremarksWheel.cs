using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpTyremarksWheel : hkReferencedObject
    {
        public int m_currentPosition;
        public int m_numPoints;
        public List<hknpTyremarkPoint> m_tyremarkPoints;
    }
}
