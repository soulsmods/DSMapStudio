using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum SimulationType
    {
        SIMULATION_TYPE_SINGLE_THREADED = 0,
        SIMULATION_TYPE_MULTI_THREADED = 1,
    }
    
    public enum SolverType
    {
        SOLVER_TYPE_INVALID = 0,
        SOLVER_TYPE_2ITERS_SOFT = 1,
        SOLVER_TYPE_2ITERS_MEDIUM = 2,
        SOLVER_TYPE_2ITERS_HARD = 3,
        SOLVER_TYPE_4ITERS_SOFT = 4,
        SOLVER_TYPE_4ITERS_MEDIUM = 5,
        SOLVER_TYPE_4ITERS_HARD = 6,
        SOLVER_TYPE_8ITERS_SOFT = 7,
        SOLVER_TYPE_8ITERS_MEDIUM = 8,
        SOLVER_TYPE_8ITERS_HARD = 9,
        SOLVER_TYPE_MAX = 10,
    }
    
    public enum LeavingBroadPhaseBehavior
    {
        ON_LEAVING_BROAD_PHASE_DO_NOTHING = 0,
        ON_LEAVING_BROAD_PHASE_REMOVE_BODY = 1,
        ON_LEAVING_BROAD_PHASE_FREEZE_BODY = 2,
    }
    
    public class hknpWorldCinfo
    {
        public int m_bodyBufferCapacity;
        public int m_motionBufferCapacity;
        public int m_constraintBufferCapacity;
        public hknpMaterialLibrary m_materialLibrary;
        public hknpMotionPropertiesLibrary m_motionPropertiesLibrary;
        public hknpBodyQualityLibrary m_qualityLibrary;
        public SimulationType m_simulationType;
        public int m_numSplitterCells;
        public Vector4 m_gravity;
        public bool m_enableContactCaching;
        public bool m_mergeEventsBeforeDispatch;
        public LeavingBroadPhaseBehavior m_leavingBroadPhaseBehavior;
        public hkAabb m_broadPhaseAabb;
        public hknpBroadPhaseConfig m_broadPhaseConfig;
        public hknpCollisionFilter m_collisionFilter;
        public hknpShapeTagCodec m_shapeTagCodec;
        public float m_collisionTolerance;
        public float m_relativeCollisionAccuracy;
        public bool m_enableWeldingForDefaultObjects;
        public bool m_enableWeldingForCriticalObjects;
        public float m_solverTau;
        public float m_solverDamp;
        public int m_solverIterations;
        public int m_solverMicrosteps;
        public float m_defaultSolverTimestep;
        public float m_maxApproachSpeedForHighQualitySolver;
        public bool m_enableDeactivation;
        public bool m_deleteCachesOnDeactivation;
        public int m_largeIslandSize;
        public bool m_enableSolverDynamicScheduling;
        public int m_contactSolverType;
        public float m_unitScale;
        public bool m_applyUnitScaleToStaticConstants;
    }
}
