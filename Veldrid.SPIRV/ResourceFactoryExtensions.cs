using System.Text;

namespace Veldrid.SPIRV
{
    /// <summary>
    /// Contains extension methods for loading <see cref="Shader"/> modules from SPIR-V bytecode.
    /// </summary>
    public static class ResourceFactoryExtensions
    {
        /// <summary>
        /// Creates a vertex and fragment shader pair from the given <see cref="ShaderDescription"/> pair containing SPIR-V
        /// bytecode or GLSL source code.
        /// </summary>
        /// <param name="factory">The <see cref="ResourceFactory"/> used to compile the translated shader code.</param>
        /// <param name="vertexShaderDescription">The vertex shader's description. <see cref="ShaderDescription.ShaderBytes"/>
        /// should contain SPIR-V bytecode or Vulkan-style GLSL source code which can be compiled to SPIR-V.</param>
        /// <param name="fragmentShaderDescription">The fragment shader's description.
        /// <see cref="ShaderDescription.ShaderBytes"/> should contain SPIR-V bytecode or Vulkan-style GLSL source code which
        /// can be compiled to SPIR-V.</param>
        /// <returns>A two-element array, containing the vertex shader (element 0) and the fragment shader (element 1).</returns>
        public static Shader[] CreateFromSpirv(
            this ResourceFactory factory,
            ShaderDescription vertexShaderDescription,
            ShaderDescription fragmentShaderDescription)
        {
            return CreateFromSpirv(factory, vertexShaderDescription, fragmentShaderDescription, new CrossCompileOptions());
        }

        /// <summary>
        /// Creates a vertex and fragment shader pair from the given <see cref="ShaderDescription"/> pair containing SPIR-V
        /// bytecode or GLSL source code.
        /// </summary>
        /// <param name="factory">The <see cref="ResourceFactory"/> used to compile the translated shader code.</param>
        /// <param name="vertexShaderDescription">The vertex shader's description. <see cref="ShaderDescription.ShaderBytes"/>
        /// should contain SPIR-V bytecode or Vulkan-style GLSL source code which can be compiled to SPIR-V.</param>
        /// <param name="fragmentShaderDescription">The fragment shader's description.
        /// <see cref="ShaderDescription.ShaderBytes"/> should contain SPIR-V bytecode or Vulkan-style GLSL source code which
        /// can be compiled to SPIR-V.</param>
        /// <param name="options">The <see cref="CrossCompileOptions"/> which will control the parameters used to translate the
        /// shaders from SPIR-V to the target language.</param>
        /// <returns>A two-element array, containing the vertex shader (element 0) and the fragment shader (element 1).</returns>
        public static Shader[] CreateFromSpirv(
            this ResourceFactory factory,
            ShaderDescription vertexShaderDescription,
            ShaderDescription fragmentShaderDescription,
            CrossCompileOptions options)
        {
            GraphicsBackend backend = factory.BackendType;
            if (backend == GraphicsBackend.Vulkan)
            {
                vertexShaderDescription.ShaderBytes = EnsureSpirv(vertexShaderDescription);
                fragmentShaderDescription.ShaderBytes = EnsureSpirv(fragmentShaderDescription);

                return new Shader[]
                {
                    factory.CreateShader(ref vertexShaderDescription),
                    factory.CreateShader(ref fragmentShaderDescription)
                };
            }

            CrossCompileTarget target = GetCompilationTarget(factory.BackendType);
            VertexFragmentCompilationResult compilationResult = SpirvCompilation.CompileVertexFragment(
                vertexShaderDescription.ShaderBytes,
                fragmentShaderDescription.ShaderBytes,
                target,
                options);

            string vertexEntryPoint = (backend == GraphicsBackend.Metal && vertexShaderDescription.EntryPoint == "main")
                ? "main0"
                : vertexShaderDescription.EntryPoint;
            byte[] vertexBytes = GetBytes(backend, compilationResult.VertexShader);
            Shader vertexShader = factory.CreateShader(new ShaderDescription(
                vertexShaderDescription.Stage,
                vertexBytes,
                vertexEntryPoint));

            string fragmentEntryPoint = (backend == GraphicsBackend.Metal && fragmentShaderDescription.EntryPoint == "main")
                ? "main0"
                : fragmentShaderDescription.EntryPoint;
            byte[] fragmentBytes = GetBytes(backend, compilationResult.FragmentShader);
            Shader fragmentShader = factory.CreateShader(new ShaderDescription(
                fragmentShaderDescription.Stage,
                fragmentBytes,
                fragmentEntryPoint));

            return new Shader[] { vertexShader, fragmentShader };
        }

        /// <summary>
        /// Creates a compute shader from the given <see cref="ShaderDescription"/> containing SPIR-V bytecode or GLSL source
        /// code.
        /// </summary>
        /// <param name="factory">The <see cref="ResourceFactory"/> used to compile the translated shader code.</param>
        /// <param name="computeShaderDescription">The compute shader's description.
        /// <see cref="ShaderDescription.ShaderBytes"/> should contain SPIR-V bytecode or Vulkan-style GLSL source code which
        /// can be compiled to SPIR-V.</param>
        /// <returns>The compiled compute <see cref="Shader"/>.</returns>
        public static Shader CreateFromSpirv(
            this ResourceFactory factory,
            ShaderDescription computeShaderDescription)
        {
            return CreateFromSpirv(factory, computeShaderDescription, new CrossCompileOptions());
        }

        /// <summary>
        /// Creates a compute shader from the given <see cref="ShaderDescription"/> containing SPIR-V bytecode or GLSL source
        /// code.
        /// </summary>
        /// <param name="factory">The <see cref="ResourceFactory"/> used to compile the translated shader code.</param>
        /// <param name="computeShaderDescription">The compute shader's description.
        /// <see cref="ShaderDescription.ShaderBytes"/> should contain SPIR-V bytecode or Vulkan-style GLSL source code which
        /// can be compiled to SPIR-V.</param>
        /// <param name="options">The <see cref="CrossCompileOptions"/> which will control the parameters used to translate the
        /// shaders from SPIR-V to the target language.</param>
        /// <returns>The compiled compute <see cref="Shader"/>.</returns>
        public static Shader CreateFromSpirv(
            this ResourceFactory factory,
            ShaderDescription computeShaderDescription,
            CrossCompileOptions options)
        {
            GraphicsBackend backend = factory.BackendType;
            if (backend == GraphicsBackend.Vulkan)
            {
                computeShaderDescription.ShaderBytes = EnsureSpirv(computeShaderDescription);
                return factory.CreateShader(ref computeShaderDescription);
            }

            CrossCompileTarget target = GetCompilationTarget(factory.BackendType);
            ComputeCompilationResult compilationResult = SpirvCompilation.CompileCompute(
                computeShaderDescription.ShaderBytes,
                target,
                options);

            string computeEntryPoint = (backend == GraphicsBackend.Metal && computeShaderDescription.EntryPoint == "main")
                ? "main0"
                : computeShaderDescription.EntryPoint;
            byte[] computeBytes = GetBytes(backend, compilationResult.ComputeShader);
            return factory.CreateShader(new ShaderDescription(
                computeShaderDescription.Stage,
                computeBytes,
                computeEntryPoint));
        }

        private static unsafe byte[] EnsureSpirv(ShaderDescription description)
        {
            if (Util.HasSpirvHeader(description.ShaderBytes))
            {
                return description.ShaderBytes;
            }
            else
            {
                fixed (byte* sourceAsciiPtr = description.ShaderBytes)
                {
                    SpirvCompilationResult glslCompileResult = SpirvCompilation.CompileGlslToSpirv(
                        (uint)description.ShaderBytes.Length,
                        sourceAsciiPtr,
                        null,
                        description.Stage,
                        description.Debug,
                        0,
                        null);
                    return glslCompileResult.SpirvBytes;
                }
            }
        }

        private static byte[] GetBytes(GraphicsBackend backend, string code)
        {
            switch (backend)
            {
                case GraphicsBackend.Direct3D11:
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.OpenGLES:
                    return Encoding.ASCII.GetBytes(code);
                case GraphicsBackend.Metal:
                    return Encoding.UTF8.GetBytes(code);
                default:
                    throw new SpirvCompilationException($"Invalid GraphicsBackend: {backend}");
            }
        }

        private static CrossCompileTarget GetCompilationTarget(GraphicsBackend backend)
        {
            switch (backend)
            {
                case GraphicsBackend.Direct3D11:
                    return CrossCompileTarget.HLSL;
                case GraphicsBackend.OpenGL:
                    return CrossCompileTarget.GLSL;
                case GraphicsBackend.Metal:
                    return CrossCompileTarget.MSL;
                case GraphicsBackend.OpenGLES:
                    return CrossCompileTarget.ESSL;
                default:
                    throw new SpirvCompilationException($"Invalid GraphicsBackend: {backend}");
            }
        }
    }
}
