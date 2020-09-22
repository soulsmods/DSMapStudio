using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MultiRotationAxisType
    {
        AxisXY = 0,
        AxisYX = 1,
    }
    
    public class CustomLookAtTwistModifier : hkbModifier
    {
        public enum SetAngleMethod
        {
            LINEAR = 0,
            RAMPED = 1,
        }
        
        public enum RotationAxisCoordinates
        {
            ROTATION_AXIS_IN_MODEL_COORDINATES = 0,
            ROTATION_AXIS_IN_LOCAL_COORDINATES = 1,
        }
        
        public int m_ModifierID;
        public MultiRotationAxisType m_rotationAxisType;
        public int m_SensingDummyPoly;
        public List<CustomLookAtTwistModifierTwistParam> m_twistParam;
        public float m_UpLimitAngle;
        public float m_DownLimitAngle;
        public float m_RightLimitAngle;
        public float m_LeftLimitAngle;
        public float m_UpMinimumAngle;
        public float m_DownMinimumAngle;
        public float m_RightMinimumAngle;
        public float m_LeftMinimumAngle;
        public short m_SensingAngle;
        public SetAngleMethod m_setAngleMethod;
        public bool m_isAdditive;
    }
}
