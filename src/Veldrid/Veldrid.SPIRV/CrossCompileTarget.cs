﻿namespace Veldrid.SPIRV
{
    /// <summary>
    /// Identifies a particular shading language.
    /// </summary>
    public enum CrossCompileTarget : uint
    {
        /// <summary>
        /// HLSL Shader Model 5
        /// </summary>
        HLSL,
        /// <summary>
        /// OpenGL-style GLSL, Version 330 or 430
        /// </summary>
        GLSL,
        /// <summary>
        /// OpenGL ES-style GLSL, version 300 es or 320 es.
        /// </summary>
        ESSL,
        /// <summary>
        /// Metal Shading Language
        /// </summary>
        MSL,
    };
}
