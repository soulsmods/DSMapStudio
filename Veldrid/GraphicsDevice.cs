using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.VulkanUtil;

namespace Veldrid
{
    /// <summary>
    /// Represents an abstract graphics device, capable of creating device resources and executing commands.
    /// </summary>
    public unsafe class GraphicsDevice : IDisposable
    {
        private static readonly FixedUtf8String s_name = "Veldrid-VkGraphicsDevice";
        private static readonly Lazy<bool> s_isSupported = new Lazy<bool>(CheckIsSupported, isThreadSafe: true);
        
        private readonly object _deferredDisposalLock = new object();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private Sampler _aniso4xSampler;
        
                private VkInstance _instance;
        private VkPhysicalDevice _physicalDevice;
        private VmaAllocator _vmaAllocator;
        private VkPhysicalDeviceProperties _physicalDeviceProperties;
        private VkPhysicalDeviceFeatures _physicalDeviceFeatures;
        private VkPhysicalDeviceVulkan11Features _physicalDeviceFeatures11;
        private VkPhysicalDeviceVulkan12Features _physicalDeviceFeatures12;
        private VkPhysicalDeviceVulkan13Features _physicalDeviceFeatures13;
        private VkPhysicalDeviceMemoryProperties _physicalDeviceMemProperties;
        private VkDevice _device;
        private uint _graphicsQueueIndex;
        private uint _transferQueueIndex;
        private uint _presentQueueIndex;
        private VkCommandPool _graphicsCommandPool;
        private readonly object _graphicsCommandPoolLock = new object();
        private VkCommandPool _transferCommandPool;
        private readonly object _transferCommandPoolLock = new object();
        private VkQueue _graphicsQueue;
        private VkQueue _transferQueue;
        private readonly object _graphicsQueueLock = new object();
        private readonly object _transferQueueLock = new object();
        private VkDebugReportCallbackEXT _debugCallbackHandle;
        private delegate uint DebugCallbackFunc(VkDebugReportFlagsEXT a, VkDebugReportObjectTypeEXT b, ulong @object, nuint location, int messageCode, sbyte* pLayerPrefix, sbyte* pMessage, void* pUserDat);
        private DebugCallbackFunc _debugCallbackFunc;
        private bool _debugMarkerEnabled;
        private vkDebugMarkerSetObjectNameEXT_t _setObjectNameDelegate;
        private vkCmdDebugMarkerBeginEXT_t _markerBegin;
        private vkCmdDebugMarkerEndEXT_t _markerEnd;
        private vkCmdDebugMarkerInsertEXT_t _markerInsert;
        private readonly ConcurrentDictionary<VkFormat, VkFilter> _filters = new ConcurrentDictionary<VkFormat, VkFilter>();
        private readonly BackendInfoVulkan _vulkanInfo;

        private const int SharedCommandPoolCount = 4;
        private ConcurrentStack<SharedCommandPool> _sharedGraphicsCommandPools = new ConcurrentStack<SharedCommandPool>();
        private VkDescriptorPoolManager _descriptorPoolManager;
        private bool _standardValidationSupported;
        private bool _standardClipYDirection;
        private vkGetBufferMemoryRequirements2_t _getBufferMemoryRequirements2;
        private vkGetImageMemoryRequirements2_t _getImageMemoryRequirements2;

        // Staging Resources
        private const uint MinStagingBufferSize = 64;
        private const uint MaxStagingBufferSize = 512;

        private readonly object _stagingResourcesLock = new object();
        private readonly List<Texture> _availableStagingTextures = new List<Texture>();
        private readonly List<DeviceBuffer> _availableStagingBuffers = new List<DeviceBuffer>();

        private readonly Dictionary<VkCommandBuffer, Texture> _submittedStagingTextures
            = new Dictionary<VkCommandBuffer, Texture>();
        private readonly Dictionary<VkCommandBuffer, DeviceBuffer> _submittedStagingBuffers
            = new Dictionary<VkCommandBuffer, DeviceBuffer>();
        private readonly Dictionary<VkCommandBuffer, SharedCommandPool> _submittedSharedCommandPools
            = new Dictionary<VkCommandBuffer, SharedCommandPool>();
        
        internal VkInstance Instance => _instance;
        internal VkDevice Device => _device;
        internal VkPhysicalDevice PhysicalDevice => _physicalDevice;
        internal VkPhysicalDeviceMemoryProperties PhysicalDeviceMemProperties => _physicalDeviceMemProperties;
        internal VkQueue GraphicsQueue => _graphicsQueue;
        internal uint GraphicsQueueIndex => _graphicsQueueIndex;
        internal uint TransferQueueIndex => _transferQueueIndex;
        internal uint PresentQueueIndex => _presentQueueIndex;
        internal VmaAllocator Allocator => _vmaAllocator;
        internal VkDescriptorPoolManager DescriptorPoolManager => _descriptorPoolManager;
        internal vkCmdDebugMarkerBeginEXT_t MarkerBegin => _markerBegin;
        internal vkCmdDebugMarkerEndEXT_t MarkerEnd => _markerEnd;
        internal vkCmdDebugMarkerInsertEXT_t MarkerInsert => _markerInsert;
        internal vkGetBufferMemoryRequirements2_t GetBufferMemoryRequirements2 => _getBufferMemoryRequirements2;
        internal vkGetImageMemoryRequirements2_t GetImageMemoryRequirements2 => _getImageMemoryRequirements2;

        private readonly object _submittedFencesLock = new object();
        private readonly ConcurrentQueue<Vortice.Vulkan.VkFence> _availableSubmissionFences = new ConcurrentQueue<Vortice.Vulkan.VkFence>();
        private readonly List<FenceSubmissionInfo> _submittedFences = new List<FenceSubmissionInfo>();
        private readonly Swapchain _mainSwapchain;

        private readonly List<FixedUtf8String> _surfaceExtensions = new List<FixedUtf8String>();
        
        internal GraphicsDevice(GraphicsDeviceOptions options, SwapchainDescription? scDesc)
            : this(options, scDesc, new VulkanDeviceOptions()) { }

        internal GraphicsDevice(GraphicsDeviceOptions options, SwapchainDescription? scDesc, VulkanDeviceOptions vkOptions)
        {
            CreateInstance(options.Debug, vkOptions);

            VkSurfaceKHR surface = VkSurfaceKHR.Null;
            if (scDesc != null)
            {
                surface = VkSurfaceUtil.CreateSurface(this, _instance, scDesc.Value.Source);
            }

            CreatePhysicalDevice();
            CreateLogicalDevice(surface, options.PreferStandardClipSpaceYDirection, vkOptions);

            var allocatorInfo = new VmaAllocatorCreateInfo
            {
                PhysicalDevice = _physicalDevice,
                Device = _device,
                Instance = _instance,
                VulkanApiVersion = Vortice.Vulkan.VkVersion.Version_1_3
            };
            var result = Vma.vmaCreateAllocator(&allocatorInfo, out _vmaAllocator);
            CheckResult(result);

            Features = new GraphicsDeviceFeatures(
                computeShader: true,
                geometryShader: _physicalDeviceFeatures.geometryShader,
                tessellationShaders: _physicalDeviceFeatures.tessellationShader,
                multipleViewports: _physicalDeviceFeatures.multiViewport,
                samplerLodBias: true,
                drawBaseVertex: true,
                drawBaseInstance: true,
                drawIndirect: true,
                drawIndirectBaseInstance: _physicalDeviceFeatures.drawIndirectFirstInstance,
                fillModeWireframe: _physicalDeviceFeatures.fillModeNonSolid,
                samplerAnisotropy: _physicalDeviceFeatures.samplerAnisotropy,
                depthClipDisable: _physicalDeviceFeatures.depthClamp,
                texture1D: true,
                independentBlend: _physicalDeviceFeatures.independentBlend,
                structuredBuffer: true,
                subsetTextureView: true,
                commandListDebugMarkers: _debugMarkerEnabled,
                bufferRangeBinding: true);

            ResourceFactory = new ResourceFactory(this);

            if (scDesc != null)
            {
                SwapchainDescription desc = scDesc.Value;
                _mainSwapchain = new Swapchain(this, ref desc, surface);
            }

            CreateDescriptorPool();
            CreateGraphicsCommandPool();
            CreateTransferCommandPool();
            for (int i = 0; i < SharedCommandPoolCount; i++)
            {
                _sharedGraphicsCommandPools.Push(new SharedCommandPool(this, true));
            }

            _vulkanInfo = new BackendInfoVulkan(this);

            PostDeviceCreated();
        }
        
        internal GraphicsDevice() { }

        /// <summary>
        /// Gets a value identifying whether texture coordinates begin in the top left corner of a Texture.
        /// If true, (0, 0) refers to the top-left texel of a Texture. If false, (0, 0) refers to the bottom-left 
        /// texel of a Texture. This property is useful for determining how the output of a Framebuffer should be sampled.
        /// </summary>
        public bool IsUvOriginTopLeft => true;

        /// <summary>
        /// Gets a value indicating whether this device's depth values range from 0 to 1.
        /// If false, depth values instead range from -1 to 1.
        /// </summary>
        public bool IsDepthRangeZeroToOne => true;

        /// <summary>
        /// Gets a value indicating whether this device's clip space Y values increase from top (-1) to bottom (1).
        /// If false, clip space Y values instead increase from bottom (-1) to top (1).
        /// </summary>
        public bool IsClipSpaceYInverted => !_standardClipYDirection;

        /// <summary>
        /// Gets the <see cref="ResourceFactory"/> controlled by this instance.
        /// </summary>
        public ResourceFactory ResourceFactory { get; }

        /// <summary>
        /// Retrieves the main Swapchain for this device. This property is only valid if the device was created with a main
        /// Swapchain, and will return null otherwise.
        /// </summary>
        public Swapchain MainSwapchain => _mainSwapchain;

        /// <summary>
        /// Gets a <see cref="GraphicsDeviceFeatures"/> which enumerates the optional features supported by this instance.
        /// </summary>
        public GraphicsDeviceFeatures Features { get; }

        /// <summary>
        /// Gets or sets whether the main Swapchain's <see cref="SwapBuffers()"/> should be synchronized to the window system's
        /// vertical refresh rate.
        /// This is equivalent to <see cref="MainSwapchain"/>.<see cref="Swapchain.SyncToVerticalBlank"/>.
        /// This property cannot be set if this GraphicsDevice was created without a main Swapchain.
        /// </summary>
        public virtual bool SyncToVerticalBlank
        {
            get => MainSwapchain?.SyncToVerticalBlank ?? false;
            set
            {
                if (MainSwapchain == null)
                {
                    throw new VeldridException($"This GraphicsDevice was created without a main Swapchain. This property cannot be set.");
                }

                MainSwapchain.SyncToVerticalBlank = value;
            }
        }

        /// <summary>
        /// The required alignment, in bytes, for uniform buffer offsets. <see cref="DeviceBufferRange.Offset"/> must be a
        /// multiple of this value. When binding a <see cref="ResourceSet"/> to a <see cref="CommandList"/> with an overload
        /// accepting dynamic offsets, each offset must be a multiple of this value.
        /// </summary>
        public uint UniformBufferMinOffsetAlignment => GetUniformBufferMinOffsetAlignmentCore();

        /// <summary>
        /// The required alignment, in bytes, for structured buffer offsets. <see cref="DeviceBufferRange.Offset"/> must be a
        /// multiple of this value. When binding a <see cref="ResourceSet"/> to a <see cref="CommandList"/> with an overload
        /// accepting dynamic offsets, each offset must be a multiple of this value.
        /// </summary>
        public uint StructuredBufferMinOffsetAlignment => GetStructuredBufferMinOffsetAlignmentCore();

        internal uint GetUniformBufferMinOffsetAlignmentCore() 
            => (uint)_physicalDeviceProperties.limits.minUniformBufferOffsetAlignment;
        internal uint GetStructuredBufferMinOffsetAlignmentCore()
            => (uint)_physicalDeviceProperties.limits.minStorageBufferOffsetAlignment;

        /// <summary>
        /// Submits the given <see cref="CommandList"/> for execution by this device.
        /// Commands submitted in this way may not be completed when this method returns.
        /// Use <see cref="WaitForIdle"/> to wait for all submitted commands to complete.
        /// <see cref="CommandList.End"/> must have been called on <paramref name="commandList"/> for this method to succeed.
        /// </summary>
        /// <param name="commandList">The completed <see cref="CommandList"/> to execute. <see cref="CommandList.End"/> must have
        /// been previously called on this object.</param>
        public void SubmitCommands(CommandList commandList) => SubmitCommandsCore(commandList, null);

        /// <summary>
        /// Submits the given <see cref="CommandList"/> for execution by this device.
        /// Commands submitted in this way may not be completed when this method returns.
        /// Use <see cref="WaitForIdle"/> to wait for all submitted commands to complete.
        /// <see cref="CommandList.End"/> must have been called on <paramref name="commandList"/> for this method to succeed.
        /// </summary>
        /// <param name="commandList">The completed <see cref="CommandList"/> to execute. <see cref="CommandList.End"/> must have
        /// been previously called on this object.</param>
        /// <param name="fence">A <see cref="Fence"/> which will become signaled after this submission fully completes
        /// execution.</param>
        public void SubmitCommands(CommandList commandList, Fence fence) => SubmitCommandsCore(commandList, fence);

        private protected void SubmitCommandsCore(CommandList cl, Fence fence)
        {
            SubmitCommandList(cl, 0, null, 0, null, fence);
        }

        /// <summary>
        /// Blocks the calling thread until the given <see cref="Fence"/> becomes signaled.
        /// </summary>
        /// <param name="fence">The <see cref="Fence"/> instance to wait on.</param>
        public void WaitForFence(Fence fence)
        {
            if (!WaitForFence(fence, ulong.MaxValue))
            {
                throw new VeldridException("The operation timed out before the Fence was signaled.");
            }
        }

        /// <summary>
        /// Blocks the calling thread until the given <see cref="Fence"/> becomes signaled, or until a time greater than the
        /// given TimeSpan has elapsed.
        /// </summary>
        /// <param name="fence">The <see cref="Fence"/> instance to wait on.</param>
        /// <param name="timeout">A TimeSpan indicating the maximum time to wait on the Fence.</param>
        /// <returns>True if the Fence was signaled. False if the timeout was reached instead.</returns>
        public bool WaitForFence(Fence fence, TimeSpan timeout)
            => WaitForFence(fence, (ulong)timeout.TotalMilliseconds * 1_000_000);
        /// <summary>
        /// Blocks the calling thread until the given <see cref="Fence"/> becomes signaled, or until a time greater than the
        /// given TimeSpan has elapsed.
        /// </summary>
        /// <param name="fence">The <see cref="Fence"/> instance to wait on.</param>
        /// <param name="nanosecondTimeout">A value in nanoseconds, indicating the maximum time to wait on the Fence.</param>
        /// <returns>True if the Fence was signaled. False if the timeout was reached instead.</returns>
        public bool WaitForFence(Fence fence, ulong nanosecondTimeout)
        {
            Vortice.Vulkan.VkFence vkFence = fence.DeviceFence;
            VkResult result = vkWaitForFences(_device, 1, &vkFence, true, nanosecondTimeout);
            return result == VkResult.Success;
        }

        /// <summary>
        /// Blocks the calling thread until one or all of the given <see cref="Fence"/> instances have become signaled.
        /// </summary>
        /// <param name="fences">An array of <see cref="Fence"/> objects to wait on.</param>
        /// <param name="waitAll">If true, then this method blocks until all of the given Fences become signaled.
        /// If false, then this method only waits until one of the Fences become signaled.</param>
        public void WaitForFences(Fence[] fences, bool waitAll)
        {
            if (!WaitForFences(fences, waitAll, ulong.MaxValue))
            {
                throw new VeldridException("The operation timed out before the Fence(s) were signaled.");
            }
        }

        /// <summary>
        /// Blocks the calling thread until one or all of the given <see cref="Fence"/> instances have become signaled,
        /// or until the given timeout has been reached.
        /// </summary>
        /// <param name="fences">An array of <see cref="Fence"/> objects to wait on.</param>
        /// <param name="waitAll">If true, then this method blocks until all of the given Fences become signaled.
        /// If false, then this method only waits until one of the Fences become signaled.</param>
        /// <param name="timeout">A TimeSpan indicating the maximum time to wait on the Fences.</param>
        /// <returns>True if the Fence was signaled. False if the timeout was reached instead.</returns>
        public bool WaitForFences(Fence[] fences, bool waitAll, TimeSpan timeout)
            => WaitForFences(fences, waitAll, (ulong)timeout.TotalMilliseconds * 1_000_000);

        /// <summary>
        /// Blocks the calling thread until one or all of the given <see cref="Fence"/> instances have become signaled,
        /// or until the given timeout has been reached.
        /// </summary>
        /// <param name="fences">An array of <see cref="Fence"/> objects to wait on.</param>
        /// <param name="waitAll">If true, then this method blocks until all of the given Fences become signaled.
        /// If false, then this method only waits until one of the Fences become signaled.</param>
        /// <param name="nanosecondTimeout">A value in nanoseconds, indicating the maximum time to wait on the Fence.</param>
        /// <returns>True if the Fence was signaled. False if the timeout was reached instead.</returns>
        public bool WaitForFences(Fence[] fences, bool waitAll, ulong nanosecondTimeout)
        {
            int fenceCount = fences.Length;
            VkFence* fencesPtr = stackalloc VkFence[fenceCount];
            for (int i = 0; i < fenceCount; i++)
            {
                fencesPtr[i] = fences[i].DeviceFence;
            }

            VkResult result = vkWaitForFences(_device, fenceCount, fencesPtr, waitAll, nanosecondTimeout);
            return result == VkResult.Success;
        }

        /// <summary>
        /// Resets the given <see cref="Fence"/> to the unsignaled state.
        /// </summary>
        /// <param name="fence">The <see cref="Fence"/> instance to reset.</param>
        public void ResetFence(Fence fence)
        {
            Vortice.Vulkan.VkFence vkFence = fence.DeviceFence;
            vkResetFences(_device, 1, &vkFence);
        }

        /// <summary>
        /// Swaps the buffers of the main swapchain and presents the rendered image to the screen.
        /// This is equivalent to passing <see cref="MainSwapchain"/> to <see cref="SwapBuffers(Swapchain)"/>.
        /// This method can only be called if this GraphicsDevice was created with a main Swapchain.
        /// </summary>
        public void SwapBuffers()
        {
            if (MainSwapchain == null)
            {
                throw new VeldridException("This GraphicsDevice was created without a main Swapchain, so the requested operation cannot be performed.");
            }

            SwapBuffers(MainSwapchain);
        }

        /// <summary>
        /// Swaps the buffers of the given swapchain.
        /// </summary>
        /// <param name="swapchain">The <see cref="Swapchain"/> to swap and present.</param>
        public void SwapBuffers(Swapchain swapchain) => SwapBuffersCore(swapchain);

        private void SwapBuffersCore(Swapchain swapchain)
        {
            var vkSC = swapchain;
            VkSwapchainKHR deviceSwapchain = vkSC.DeviceSwapchain;
            uint imageIndex = vkSC.ImageIndex;
            var presentInfo = new VkPresentInfoKHR
            {
                sType = VkStructureType.PresentInfoKHR,
                swapchainCount = 1,
                pSwapchains = &deviceSwapchain,
                pImageIndices = &imageIndex
            };

            object presentLock = vkSC.PresentQueueIndex == _graphicsQueueIndex ? _graphicsQueueLock : vkSC;
            lock (presentLock)
            {
                vkQueuePresentKHR(vkSC.PresentQueue, &presentInfo);
                if (vkSC.AcquireNextImage(_device, VkSemaphore.Null, vkSC.ImageAvailableFence))
                {
                    Vortice.Vulkan.VkFence fence = vkSC.ImageAvailableFence;
                    vkWaitForFences(_device, 1, &fence, true, ulong.MaxValue);
                    vkResetFences(_device, 1, &fence);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Framebuffer"/> object representing the render targets of the main swapchain.
        /// This is equivalent to <see cref="MainSwapchain"/>.<see cref="Swapchain.Framebuffer"/>.
        /// If this GraphicsDevice was created without a main Swapchain, then this returns null.
        /// </summary>
        public Framebuffer SwapchainFramebuffer => MainSwapchain?.Framebuffer;

        /// <summary>
        /// Notifies this instance that the main window has been resized. This causes the <see cref="SwapchainFramebuffer"/> to
        /// be appropriately resized and recreated.
        /// This is equivalent to calling <see cref="MainSwapchain"/>.<see cref="Swapchain.Resize(uint, uint)"/>.
        /// This method can only be called if this GraphicsDevice was created with a main Swapchain.
        /// </summary>
        /// <param name="width">The new width of the main window.</param>
        /// <param name="height">The new height of the main window.</param>
        public void ResizeMainWindow(uint width, uint height)
        {
            if (MainSwapchain == null)
            {
                throw new VeldridException("This GraphicsDevice was created without a main Swapchain, so the requested operation cannot be performed.");
            }

            MainSwapchain.Resize(width, height);
        }

        /// <summary>
        /// A blocking method that returns when all submitted <see cref="CommandList"/> objects have fully completed.
        /// </summary>
        public void WaitForIdle()
        {
            WaitForIdleCore();
            FlushDeferredDisposals();
        }

        private void WaitForIdleCore()
        {
            lock (_graphicsQueueLock)
            {
                vkQueueWaitIdle(_graphicsQueue);
            }

            CheckSubmittedFences();
        }

        /// <summary>
        /// Gets the maximum sample count supported by the given <see cref="PixelFormat"/>.
        /// </summary>
        /// <param name="format">The format to query.</param>
        /// <param name="depthFormat">Whether the format will be used in a depth texture.</param>
        /// <returns>A <see cref="TextureSampleCount"/> value representing the maximum count that a <see cref="Texture"/> of that
        /// format can be created with.</returns>
        public VkSampleCountFlags GetSampleCountLimit(VkFormat format, bool depthFormat)
        {
            VkImageUsageFlags usageFlags = VkImageUsageFlags.Sampled;
            usageFlags |= depthFormat ? VkImageUsageFlags.DepthStencilAttachment : VkImageUsageFlags.ColorAttachment;

            vkGetPhysicalDeviceImageFormatProperties(
                _physicalDevice,
                format,
                VkImageType.Image2D,
                VkImageTiling.Optimal,
                usageFlags,
                VkImageCreateFlags.None,
                out VkImageFormatProperties formatProperties);

            VkSampleCountFlags vkSampleCounts = formatProperties.sampleCounts;
            if ((vkSampleCounts & VkSampleCountFlags.Count32) == VkSampleCountFlags.Count32)
            {
                return VkSampleCountFlags.Count32;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count16) == VkSampleCountFlags.Count16)
            {
                return VkSampleCountFlags.Count16;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count8) == VkSampleCountFlags.Count8)
            {
                return VkSampleCountFlags.Count8;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count4) == VkSampleCountFlags.Count4)
            {
                return VkSampleCountFlags.Count4;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count2) == VkSampleCountFlags.Count2)
            {
                return VkSampleCountFlags.Count2;
            }

            return VkSampleCountFlags.Count1;
        }

        /// <summary>
        /// Maps a <see cref="DeviceBuffer"/> or <see cref="Texture"/> into a CPU-accessible data region. For Texture resources, this
        /// overload maps the first subresource.
        /// </summary>
        /// <param name="resource">The <see cref="DeviceBuffer"/> or <see cref="Texture"/> resource to map.</param>
        /// <param name="mode">The <see cref="MapMode"/> to use.</param>
        /// <returns>A <see cref="MappedResource"/> structure describing the mapped data region.</returns>
        public MappedResource Map(MappableResource resource, MapMode mode) => Map(resource, mode, 0);
        /// <summary>
        /// Maps a <see cref="DeviceBuffer"/> or <see cref="Texture"/> into a CPU-accessible data region.
        /// </summary>
        /// <param name="resource">The <see cref="DeviceBuffer"/> or <see cref="Texture"/> resource to map.</param>
        /// <param name="mode">The <see cref="MapMode"/> to use.</param>
        /// <param name="subresource">The subresource to map. Subresources are indexed first by mip slice, then by array layer.
        /// For <see cref="DeviceBuffer"/> resources, this parameter must be 0.</param>
        /// <returns>A <see cref="MappedResource"/> structure describing the mapped data region.</returns>
        public MappedResource Map(MappableResource resource, MapMode mode, uint subresource)
        {
#if VALIDATE_USAGE
            if (resource is DeviceBuffer buffer)
            {
                if (buffer.MemoryUsage != VmaMemoryUsage.AutoPreferHost)
                {
                    throw new VeldridException("Buffers must have the Staging or Dynamic usage flag to be mapped.");
                }
                if (subresource != 0)
                {
                    throw new VeldridException("Subresource must be 0 for Buffer resources.");
                }
                if ((mode == MapMode.Read || mode == MapMode.ReadWrite) && (buffer.MemoryUsage != VmaMemoryUsage.AutoPreferHost))
                {
                    throw new VeldridException(
                        $"{nameof(MapMode)}.{nameof(MapMode.Read)} and {nameof(MapMode)}.{nameof(MapMode.ReadWrite)} can only be used on buffers created with host visible mapping");
                }
            }
            else if (resource is Texture tex)
            {
                if ((tex.Tiling & VkImageTiling.Linear) == 0)
                {
                    throw new VeldridException("Texture must have the Staging usage flag to be mapped.");
                }
                if (subresource >= tex.ArrayLayers * tex.MipLevels)
                {
                    throw new VeldridException(
                        "Subresource must be less than the number of subresources in the Texture being mapped.");
                }
            }
#endif

            return MapCore(resource, mode, subresource);
        }

        /// <summary>
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="mode"></param>
        /// <param name="subresource"></param>
        /// <returns></returns>
        protected MappedResource MapCore(MappableResource resource, MapMode mode, uint subresource)
        {
            VmaAllocation allocation = default(VmaAllocation);
            VmaAllocationInfo info = default(VmaAllocationInfo);
            IntPtr mappedPtr = IntPtr.Zero;
            uint sizeInBytes;
            uint offset = 0;
            uint rowPitch = 0;
            uint depthPitch = 0;
            if (resource is DeviceBuffer buffer)
            {
                allocation = buffer.Allocation;
                info = buffer.AllocationInfo;
                sizeInBytes = buffer.SizeInBytes;
            }
            else
            {
                Texture texture = Util.AssertSubtype<MappableResource, Texture>(resource);
                VkSubresourceLayout layout = texture.GetSubresourceLayout(subresource);
                allocation = texture.Allocation;
                info = texture.AllocationInfo;
                sizeInBytes = (uint)layout.size;
                offset = (uint)layout.offset;
                rowPitch = (uint)layout.rowPitch;
                depthPitch = (uint)layout.depthPitch;
            }
            
            if (info.pMappedData != null)
            {
                mappedPtr = (IntPtr)info.pMappedData;
            }
            else
            {
                void* ptr;
                VkResult result = Vma.vmaMapMemory(Allocator, allocation, &ptr);
                CheckResult(result);
                mappedPtr = (IntPtr)ptr;
            }

            byte* dataPtr = (byte*)mappedPtr.ToPointer() + offset;
            return new MappedResource(
                resource,
                mode,
                (IntPtr)dataPtr,
                sizeInBytes,
                subresource,
                rowPitch,
                depthPitch);
        }

        /// <summary>
        /// Maps a <see cref="DeviceBuffer"/> or <see cref="Texture"/> into a CPU-accessible data region, and returns a structured
        /// view over that region. For Texture resources, this overload maps the first subresource.
        /// </summary>
        /// <param name="resource">The <see cref="DeviceBuffer"/> or <see cref="Texture"/> resource to map.</param>
        /// <param name="mode">The <see cref="MapMode"/> to use.</param>
        /// <typeparam name="T">The blittable value type which mapped data is viewed as.</typeparam>
        /// <returns>A <see cref="MappedResource"/> structure describing the mapped data region.</returns>
        public MappedResourceView<T> Map<T>(MappableResource resource, MapMode mode) where T : struct
            => Map<T>(resource, mode, 0);
        /// <summary>
        /// Maps a <see cref="DeviceBuffer"/> or <see cref="Texture"/> into a CPU-accessible data region, and returns a structured
        /// view over that region.
        /// </summary>
        /// <param name="resource">The <see cref="DeviceBuffer"/> or <see cref="Texture"/> resource to map.</param>
        /// <param name="mode">The <see cref="MapMode"/> to use.</param>
        /// <param name="subresource">The subresource to map. Subresources are indexed first by mip slice, then by array layer.</param>
        /// <typeparam name="T">The blittable value type which mapped data is viewed as.</typeparam>
        /// <returns>A <see cref="MappedResource"/> structure describing the mapped data region.</returns>
        public MappedResourceView<T> Map<T>(MappableResource resource, MapMode mode, uint subresource) where T : struct
        {
            MappedResource mappedResource = Map(resource, mode, subresource);
            return new MappedResourceView<T>(mappedResource);
        }

        /// <summary>
        /// Invalidates a previously-mapped data region for the given <see cref="DeviceBuffer"/> or <see cref="Texture"/>.
        /// For <see cref="Texture"/> resources, this unmaps the first subresource.
        /// </summary>
        /// <param name="resource">The resource to unmap.</param>
        public void Unmap(MappableResource resource) => Unmap(resource, 0);
        /// <summary>
        /// Invalidates a previously-mapped data region for the given <see cref="DeviceBuffer"/> or <see cref="Texture"/>.
        /// </summary>
        /// <param name="resource">The resource to unmap.</param>
        /// <param name="subresource">The subresource to unmap. Subresources are indexed first by mip slice, then by array layer.
        /// For <see cref="DeviceBuffer"/> resources, this parameter must be 0.</param>
        public void Unmap(MappableResource resource, uint subresource)
        {
            UnmapCore(resource, subresource);
        }

        /// <summary>
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="subresource"></param>
        protected void UnmapCore(MappableResource resource, uint subresource)
        {
            VmaAllocation allocation = default(VmaAllocation);
            VmaAllocationInfo info = default(VmaAllocationInfo);
            if (resource is DeviceBuffer buffer)
            {
                allocation = buffer.Allocation;
                info = buffer.AllocationInfo;
            }
            else
            {
                Texture tex = Util.AssertSubtype<MappableResource, Texture>(resource);
                allocation = tex.Allocation;
                info = tex.AllocationInfo;
            }

            if (info.pMappedData == null)
            {
                Vma.vmaUnmapMemory(Allocator, allocation);
            }
        }

        /// <summary>
        /// Updates a portion of a <see cref="Texture"/> resource with new data.
        /// </summary>
        /// <param name="texture">The resource to update.</param>
        /// <param name="source">A pointer to the start of the data to upload. This must point to tightly-packed pixel data for
        /// the region specified.</param>
        /// <param name="sizeInBytes">The number of bytes to upload. This value must match the total size of the texture region
        /// specified.</param>
        /// <param name="x">The minimum X value of the updated region.</param>
        /// <param name="y">The minimum Y value of the updated region.</param>
        /// <param name="z">The minimum Z value of the updated region.</param>
        /// <param name="width">The width of the updated region, in texels.</param>
        /// <param name="height">The height of the updated region, in texels.</param>
        /// <param name="depth">The depth of the updated region, in texels.</param>
        /// <param name="mipLevel">The mipmap level to update. Must be less than the total number of mipmaps contained in the
        /// <see cref="Texture"/>.</param>
        /// <param name="arrayLayer">The array layer to update. Must be less than the total array layer count contained in the
        /// <see cref="Texture"/>.</param>
        public void UpdateTexture(
            Texture texture,
            IntPtr source,
            uint sizeInBytes,
            uint x, uint y, uint z,
            uint width, uint height, uint depth,
            uint mipLevel, uint arrayLayer)
        {
#if VALIDATE_USAGE
            ValidateUpdateTextureParameters(texture, sizeInBytes, x, y, z, width, height, depth, mipLevel, arrayLayer);
#endif
            UpdateTextureCore(texture, source, sizeInBytes, x, y, z, width, height, depth, mipLevel, arrayLayer);
        }

        /// <summary>
        /// Updates a portion of a <see cref="Texture"/> resource with new data contained in an array
        /// </summary>
        /// <param name="texture">The resource to update.</param>
        /// <param name="source">An array containing the data to upload. This must contain tightly-packed pixel data for the
        /// region specified.</param>
        /// <param name="x">The minimum X value of the updated region.</param>
        /// <param name="y">The minimum Y value of the updated region.</param>
        /// <param name="z">The minimum Z value of the updated region.</param>
        /// <param name="width">The width of the updated region, in texels.</param>
        /// <param name="height">The height of the updated region, in texels.</param>
        /// <param name="depth">The depth of the updated region, in texels.</param>
        /// <param name="mipLevel">The mipmap level to update. Must be less than the total number of mipmaps contained in the
        /// <see cref="Texture"/>.</param>
        /// <param name="arrayLayer">The array layer to update. Must be less than the total array layer count contained in the
        /// <see cref="Texture"/>.</param>
        public void UpdateTexture<T>(
            Texture texture,
            T[] source,
            uint x, uint y, uint z,
            uint width, uint height, uint depth,
            uint mipLevel, uint arrayLayer) where T : struct
        {
            uint sizeInBytes = (uint)(Unsafe.SizeOf<T>() * source.Length);
#if VALIDATE_USAGE
            ValidateUpdateTextureParameters(texture, sizeInBytes, x, y, z, width, height, depth, mipLevel, arrayLayer);
#endif
            GCHandle gch = GCHandle.Alloc(source, GCHandleType.Pinned);
            UpdateTextureCore(
                texture,
                gch.AddrOfPinnedObject(),
                sizeInBytes,
                x, y, z,
                width, height, depth,
                mipLevel, arrayLayer);
            gch.Free();
        }

        private protected void UpdateTextureCore(
            Texture texture,
            IntPtr source,
            uint sizeInBytes,
            uint x,
            uint y,
            uint z,
            uint width,
            uint height,
            uint depth,
            uint mipLevel,
            uint arrayLayer)
        {
            var vkTex = texture;
            bool isStaging = vkTex.Tiling == VkImageTiling.Linear;
            if (isStaging)
            {
                uint subresource = texture.CalculateSubresource(mipLevel, arrayLayer);
                VkSubresourceLayout layout = vkTex.GetSubresourceLayout(subresource);
                byte* imageBasePtr = (byte*)vkTex.AllocationInfo.pMappedData + layout.offset;

                uint srcRowPitch = FormatHelpers.GetRowPitch(width, texture.Format);
                uint srcDepthPitch = FormatHelpers.GetDepthPitch(srcRowPitch, height, texture.Format);
                Util.CopyTextureRegion(
                    source.ToPointer(),
                    0, 0, 0,
                    srcRowPitch, srcDepthPitch,
                    imageBasePtr,
                    x, y, z,
                    (uint)layout.rowPitch, (uint)layout.depthPitch,
                    width, height, depth,
                    texture.Format);
            }
            else
            {
                Texture stagingTex = GetFreeStagingTexture(width, height, depth, texture.Format);
                UpdateTexture(stagingTex, source, sizeInBytes, 0, 0, 0, width, height, depth, 0, 0);
                SharedCommandPool pool = GetFreeCommandPool();
                VkCommandBuffer cb = pool.BeginNewCommandBuffer();
                CommandList.CopyTextureCore_VkCommandBuffer(
                    cb,
                    stagingTex, 0, 0, 0, 0, 0,
                    texture, x, y, z, mipLevel, arrayLayer,
                    width, height, depth, 1);
                lock (_stagingResourcesLock)
                {
                    _submittedStagingTextures.Add(cb, stagingTex);
                }
                pool.EndAndSubmit(cb);
            }
        }

        [Conditional("VALIDATE_USAGE")]
        private static void ValidateUpdateTextureParameters(
            Texture texture,
            uint sizeInBytes,
            uint x, uint y, uint z,
            uint width, uint height, uint depth,
            uint mipLevel, uint arrayLayer)
        {
            if (FormatHelpers.IsCompressedFormat(texture.Format))
            {
                if (x % 4 != 0 || y % 4 != 0 || height % 4 != 0 || width % 4 != 0)
                {
                    Util.GetMipDimensions(texture, mipLevel, out uint mipWidth, out uint mipHeight, out _);
                    if (width != mipWidth && height != mipHeight)
                    {
                        throw new VeldridException($"Updates to block-compressed textures must use a region that is block-size aligned and sized.");
                    }
                }
            }
            uint expectedSize = FormatHelpers.GetRegionSize(width, height, depth, texture.Format);
            if (sizeInBytes < expectedSize)
            {
                throw new VeldridException(
                    $"The data size is less than expected for the given update region. At least {expectedSize} bytes must be provided, but only {sizeInBytes} were.");
            }

            // Compressed textures don't necessarily need to have a Texture.Width and Texture.Height that are a multiple of 4.
            // But the mipdata width and height *does* need to be a multiple of 4.
            uint roundedTextureWidth, roundedTextureHeight;
            if (FormatHelpers.IsCompressedFormat(texture.Format))
            {
                roundedTextureWidth = (texture.Width + 3) / 4 * 4;
                roundedTextureHeight = (texture.Height + 3) / 4 * 4;
            }
            else
            {
                roundedTextureWidth = texture.Width;
                roundedTextureHeight = texture.Height;
            }

            if (x + width > roundedTextureWidth || y + height > roundedTextureHeight || z + depth > texture.Depth)
            {
                throw new VeldridException($"The given region does not fit into the Texture.");
            }

            if (mipLevel >= texture.MipLevels)
            {
                throw new VeldridException(
                    $"{nameof(mipLevel)} ({mipLevel}) must be less than the Texture's mip level count ({texture.MipLevels}).");
            }

            uint effectiveArrayLayers = texture.ArrayLayers;
            if ((texture.CreateFlags & VkImageCreateFlags.CubeCompatible) != 0)
            {
                effectiveArrayLayers *= 6;
            }
            if (arrayLayer >= effectiveArrayLayers)
            {
                throw new VeldridException(
                    $"{nameof(arrayLayer)} ({arrayLayer}) must be less than the Texture's effective array layer count ({effectiveArrayLayers}).");
            }
        }

        /// <summary>
        /// Updates a <see cref="DeviceBuffer"/> region with new data.
        /// This function must be used with a blittable value type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to upload.</typeparam>
        /// <param name="buffer">The resource to update.</param>
        /// <param name="bufferOffsetInBytes">An offset, in bytes, from the beginning of the <see cref="DeviceBuffer"/> storage, at
        /// which new data will be uploaded.</param>
        /// <param name="source">The value to upload.</param>
        public unsafe void UpdateBuffer<T>(
            DeviceBuffer buffer,
            uint bufferOffsetInBytes,
            T source) where T : struct
        {
            ref byte sourceByteRef = ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref source));
            fixed (byte* ptr = &sourceByteRef)
            {
                UpdateBuffer(buffer, bufferOffsetInBytes, (IntPtr)ptr, (uint)Unsafe.SizeOf<T>());
            }
        }

        /// <summary>
        /// Updates a <see cref="DeviceBuffer"/> region with new data.
        /// This function must be used with a blittable value type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to upload.</typeparam>
        /// <param name="buffer">The resource to update.</param>
        /// <param name="bufferOffsetInBytes">An offset, in bytes, from the beginning of the <see cref="DeviceBuffer"/>'s storage, at
        /// which new data will be uploaded.</param>
        /// <param name="source">A reference to the single value to upload.</param>
        public unsafe void UpdateBuffer<T>(
            DeviceBuffer buffer,
            uint bufferOffsetInBytes,
            ref T source) where T : struct
        {
            ref byte sourceByteRef = ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref source));
            fixed (byte* ptr = &sourceByteRef)
            {
                UpdateBuffer(buffer, bufferOffsetInBytes, (IntPtr)ptr, Util.USizeOf<T>());
            }
        }

        /// <summary>
        /// Updates a <see cref="DeviceBuffer"/> region with new data.
        /// This function must be used with a blittable value type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to upload.</typeparam>
        /// <param name="buffer">The resource to update.</param>
        /// <param name="bufferOffsetInBytes">An offset, in bytes, from the beginning of the <see cref="DeviceBuffer"/>'s storage, at
        /// which new data will be uploaded.</param>
        /// <param name="source">A reference to the first of a series of values to upload.</param>
        /// <param name="sizeInBytes">The total size of the uploaded data, in bytes.</param>
        public unsafe void UpdateBuffer<T>(
            DeviceBuffer buffer,
            uint bufferOffsetInBytes,
            ref T source,
            uint sizeInBytes) where T : struct
        {
            ref byte sourceByteRef = ref Unsafe.AsRef<byte>(Unsafe.AsPointer(ref source));
            fixed (byte* ptr = &sourceByteRef)
            {
                UpdateBuffer(buffer, bufferOffsetInBytes, (IntPtr)ptr, sizeInBytes);
            }
        }

        /// <summary>
        /// Updates a <see cref="DeviceBuffer"/> region with new data.
        /// This function must be used with a blittable value type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of data to upload.</typeparam>
        /// <param name="buffer">The resource to update.</param>
        /// <param name="bufferOffsetInBytes">An offset, in bytes, from the beginning of the <see cref="DeviceBuffer"/>'s storage, at
        /// which new data will be uploaded.</param>
        /// <param name="source">An array containing the data to upload.</param>
        public unsafe void UpdateBuffer<T>(
            DeviceBuffer buffer,
            uint bufferOffsetInBytes,
            T[] source) where T : struct
        {
            GCHandle gch = GCHandle.Alloc(source, GCHandleType.Pinned);
            UpdateBuffer(buffer, bufferOffsetInBytes, gch.AddrOfPinnedObject(), (uint)(Unsafe.SizeOf<T>() * source.Length));
            gch.Free();
        }

        /// <summary>
        /// Updates a <see cref="DeviceBuffer"/> region with new data.
        /// </summary>
        /// <param name="buffer">The resource to update.</param>
        /// <param name="bufferOffsetInBytes">An offset, in bytes, from the beginning of the <see cref="DeviceBuffer"/>'s storage, at
        /// which new data will be uploaded.</param>
        /// <param name="source">A pointer to the start of the data to upload.</param>
        /// <param name="sizeInBytes">The total size of the uploaded data, in bytes.</param>
        public void UpdateBuffer(
            DeviceBuffer buffer,
            uint bufferOffsetInBytes,
            IntPtr source,
            uint sizeInBytes)
        {
            if (bufferOffsetInBytes + sizeInBytes > buffer.SizeInBytes)
            {
                throw new VeldridException(
                    $"The data size given to UpdateBuffer is too large. The given buffer can only hold {buffer.SizeInBytes} total bytes. The requested update would require {bufferOffsetInBytes + sizeInBytes} bytes.");
            }
            UpdateBufferCore(buffer, bufferOffsetInBytes, source, sizeInBytes);
        }

        private protected void UpdateBufferCore(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes)
        {
            var vkBuffer = buffer;
            DeviceBuffer copySrcVkBuffer = null;
            IntPtr mappedPtr;
            byte* destPtr;
            bool isPersistentMapped = vkBuffer.AllocationInfo.pMappedData != null;
            if (isPersistentMapped)
            {
                mappedPtr = (IntPtr)vkBuffer.AllocationInfo.pMappedData;
                destPtr = (byte*)mappedPtr + bufferOffsetInBytes;
            }
            else
            {
                copySrcVkBuffer = GetFreeStagingBuffer(sizeInBytes);
                mappedPtr = (IntPtr)copySrcVkBuffer.AllocationInfo.pMappedData;
                destPtr = (byte*)mappedPtr;
            }

            Unsafe.CopyBlock(destPtr, source.ToPointer(), sizeInBytes);

            if (!isPersistentMapped)
            {
                SharedCommandPool pool = GetFreeCommandPool();
                VkCommandBuffer cb = pool.BeginNewCommandBuffer();

                VkBufferCopy copyRegion = new VkBufferCopy
                {
                    dstOffset = bufferOffsetInBytes,
                    size = sizeInBytes
                };
                vkCmdCopyBuffer(cb, copySrcVkBuffer.Buffer, vkBuffer.Buffer, 1, &copyRegion);

                pool.EndAndSubmit(cb);
                lock (_stagingResourcesLock)
                {
                    _submittedStagingBuffers.Add(cb, copySrcVkBuffer);
                }
            }
        }

        /// <summary>
        /// Gets whether or not the given <see cref="PixelFormat"/>, <see cref="TextureType"/>, and <see cref="TextureUsage"/>
        /// combination is supported by this instance.
        /// </summary>
        /// <param name="format">The PixelFormat to query.</param>
        /// <param name="type">The TextureType to query.</param>
        /// <param name="usage">The TextureUsage to query.</param>
        /// <returns>True if the given combination is supported; false otherwise.</returns>
        public bool GetPixelFormatSupport(
            VkFormat format,
            VkImageType type,
            VkImageUsageFlags usage,
            VkImageTiling tiling)
        {
            return GetPixelFormatSupportCore(format, type, usage, tiling, out _);
        }

        /// <summary>
        /// Gets whether or not the given <see cref="PixelFormat"/>, <see cref="TextureType"/>, and <see cref="TextureUsage"/>
        /// combination is supported by this instance, and also gets the device-specific properties supported by this instance.
        /// </summary>
        /// <param name="format">The PixelFormat to query.</param>
        /// <param name="type">The TextureType to query.</param>
        /// <param name="usage">The TextureUsage to query.</param>
        /// <param name="properties">If the combination is supported, then this parameter describes the limits of a Texture
        /// created using the given combination of attributes.</param>
        /// <returns>True if the given combination is supported; false otherwise. If the combination is supported,
        /// then <paramref name="properties"/> contains the limits supported by this instance.</returns>
        public bool GetPixelFormatSupport(
            VkFormat format,
            VkImageType type,
            VkImageUsageFlags usage,
            VkImageTiling tiling,
            out PixelFormatProperties properties)
        {
            return GetPixelFormatSupportCore(format, type, usage, tiling, out properties);
        }

        private protected bool GetPixelFormatSupportCore(
            VkFormat format,
            VkImageType type,
            VkImageUsageFlags usage,
            VkImageTiling tiling,
            out PixelFormatProperties properties)
        {
            VkFormat vkFormat = format;
            VkImageType vkType = type;
            VkImageUsageFlags vkUsage = usage;

            VkResult result = vkGetPhysicalDeviceImageFormatProperties(
                _physicalDevice,
                vkFormat,
                vkType,
                tiling,
                vkUsage,
                VkImageCreateFlags.None,
                out VkImageFormatProperties vkProps);

            if (result == VkResult.ErrorFormatNotSupported)
            {
                properties = default(PixelFormatProperties);
                return false;
            }
            CheckResult(result);

            properties = new PixelFormatProperties(
                vkProps.maxExtent.width,
                vkProps.maxExtent.height,
                vkProps.maxExtent.depth,
                vkProps.maxMipLevels,
                vkProps.maxArrayLayers,
                (uint)vkProps.sampleCounts);
            return true;
        }

        /// <summary>
        /// Adds the given object to a deferred disposal list, which will be processed when this GraphicsDevice becomes idle.
        /// This method can be used to safely dispose a device resource which may be in use at the time this method is called,
        /// but which will no longer be in use when the device is idle.
        /// </summary>
        /// <param name="disposable">An object to dispose when this instance becomes idle.</param>
        public void DisposeWhenIdle(IDisposable disposable)
        {
            lock (_deferredDisposalLock)
            {
                _disposables.Add(disposable);
            }
        }

        private void FlushDeferredDisposals()
        {
            lock (_deferredDisposalLock)
            {
                foreach (IDisposable disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
            }
        }

        /// <summary>
        /// Performs API-specific disposal of resources controlled by this instance.
        /// </summary>
        protected void PlatformDispose()
        {
            Debug.Assert(_submittedFences.Count == 0);
            foreach (VkFence fence in _availableSubmissionFences)
            {
                vkDestroyFence(_device, fence, null);
            }

            _mainSwapchain?.Dispose();
            if (_debugCallbackFunc != null)
            {
                _debugCallbackFunc = null;
                FixedUtf8String debugExtFnName = "vkDestroyDebugReportCallbackEXT";
                IntPtr destroyFuncPtr = (IntPtr)vkGetInstanceProcAddr(_instance, debugExtFnName.Span);
                vkDestroyDebugReportCallbackEXT_d destroyDel
                    = Marshal.GetDelegateForFunctionPointer<vkDestroyDebugReportCallbackEXT_d>(destroyFuncPtr);
                destroyDel(_instance, _debugCallbackHandle, null);
            }

            _descriptorPoolManager.DestroyAll();
            vkDestroyCommandPool(_device, _graphicsCommandPool, null);
            vkDestroyCommandPool(_device, _transferCommandPool, null);

            Debug.Assert(_submittedStagingTextures.Count == 0);
            foreach (var tex in _availableStagingTextures)
            {
                tex.Dispose();
            }

            Debug.Assert(_submittedStagingBuffers.Count == 0);
            foreach (var buffer in _availableStagingBuffers)
            {
                buffer.Dispose();
            }

            while (_sharedGraphicsCommandPools.TryPop(out SharedCommandPool sharedPool))
            {
                sharedPool.Destroy();
            }

            Vma.vmaDestroyAllocator(_vmaAllocator);

            VkResult result = vkDeviceWaitIdle(_device);
            CheckResult(result);
            vkDestroyDevice(_device, null);
            vkDestroyInstance(_instance, null);
        }

        /// <summary>
        /// Creates and caches common device resources after device creation completes.
        /// </summary>
        protected void PostDeviceCreated()
        {
            PointSampler = ResourceFactory.CreateSampler(SamplerDescription.Point);
            LinearSampler = ResourceFactory.CreateSampler(SamplerDescription.Linear);
            if (Features.SamplerAnisotropy)
            {
                _aniso4xSampler = ResourceFactory.CreateSampler(SamplerDescription.Aniso4x);
            }
        }

        /// <summary>
        /// Gets a simple point-filtered <see cref="Sampler"/> object owned by this instance.
        /// This object is created with <see cref="SamplerDescription.Point"/>.
        /// </summary>
        public Sampler PointSampler { get; private set; }

        /// <summary>
        /// Gets a simple linear-filtered <see cref="Sampler"/> object owned by this instance.
        /// This object is created with <see cref="SamplerDescription.Linear"/>.
        /// </summary>
        public Sampler LinearSampler { get; private set; }

        /// <summary>
        /// Gets a simple 4x anisotropic-filtered <see cref="Sampler"/> object owned by this instance.
        /// This object is created with <see cref="SamplerDescription.Aniso4x"/>.
        /// This property can only be used when <see cref="GraphicsDeviceFeatures.SamplerAnisotropy"/> is supported.
        /// </summary>
        public Sampler Aniso4xSampler
        {
            get
            {
                if (!Features.SamplerAnisotropy)
                {
                    throw new VeldridException(
                        "GraphicsDevice.Aniso4xSampler cannot be used unless GraphicsDeviceFeatures.SamplerAnisotropy is supported.");
                }

                Debug.Assert(_aniso4xSampler != null);
                return _aniso4xSampler;
            }
        }
        
        private void SubmitCommandList(
            CommandList cl,
            uint waitSemaphoreCount,
            VkSemaphore* waitSemaphoresPtr,
            uint signalSemaphoreCount,
            VkSemaphore* signalSemaphoresPtr,
            Fence fence)
        {
            VkCommandBuffer vkCB = cl.CommandBuffer;

            cl.CommandBufferSubmitted(vkCB);
            SubmitCommandBuffer(cl, vkCB, waitSemaphoreCount, waitSemaphoresPtr, signalSemaphoreCount, signalSemaphoresPtr, fence, cl.IsTransfer);
        }

        private void SubmitCommandBuffer(
            CommandList vkCL,
            VkCommandBuffer vkCB,
            uint waitSemaphoreCount,
            VkSemaphore* waitSemaphoresPtr,
            uint signalSemaphoreCount,
            VkSemaphore* signalSemaphoresPtr,
            Fence fence,
            bool isTransfer)
        {
            CheckSubmittedFences();

            bool useExtraFence = fence != null;
            VkPipelineStageFlags waitDstStageMask = VkPipelineStageFlags.ColorAttachmentOutput;
            var cbSubmitInfo = new VkCommandBufferSubmitInfo
            {
                sType = VkStructureType.CommandBufferSubmitInfo,
                commandBuffer = vkCB,
                deviceMask = 0,
            };
            var si = new VkSubmitInfo2
            {
                sType = VkStructureType.SubmitInfo2,
                flags = VkSubmitFlags.None,
                commandBufferInfoCount = 1,
                pCommandBufferInfos = &cbSubmitInfo,
            };

            var vkFence = VkFence.Null;
            var submissionFence = VkFence.Null;
            if (useExtraFence)
            {
                vkFence = fence.DeviceFence;
                submissionFence = GetFreeSubmissionFence();
            }
            else
            {
                vkFence = GetFreeSubmissionFence();
                submissionFence = vkFence;
            }

            if (isTransfer)
            {
                lock (_transferQueueLock)
                {
                    VkResult result = vkQueueSubmit2(_transferQueue, 1, &si, vkFence);
                    CheckResult(result);
                    if (useExtraFence)
                    {
                        result = vkQueueSubmit2(_transferQueue, 0, null, submissionFence);
                        CheckResult(result);
                    }
                }
            }
            else
            {
                lock (_graphicsQueueLock)
                {
                    VkResult result = vkQueueSubmit2(_graphicsQueue, 1, &si, vkFence);
                    CheckResult(result);
                    if (useExtraFence)
                    {
                        result = vkQueueSubmit2(_graphicsQueue, 0, null, submissionFence);
                        CheckResult(result);
                    }
                }
            }

            lock (_submittedFencesLock)
            {
                _submittedFences.Add(new FenceSubmissionInfo(submissionFence, vkCL, vkCB));
            }
        }

        private void CheckSubmittedFences()
        {
            lock (_submittedFencesLock)
            {
                for (int i = 0; i < _submittedFences.Count; i++)
                {
                    FenceSubmissionInfo fsi = _submittedFences[i];
                    if (vkGetFenceStatus(_device, fsi.Fence) == VkResult.Success)
                    {
                        CompleteFenceSubmission(fsi);
                        _submittedFences.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        break; // Submissions are in order; later submissions cannot complete if this one hasn't.
                    }
                }
            }
        }

        private void CompleteFenceSubmission(FenceSubmissionInfo fsi)
        {
            VkFence fence = fsi.Fence;
            VkCommandBuffer completedCB = fsi.CommandBuffer;
            fsi.CommandList?.CommandBufferCompleted(completedCB);
            VkResult resetResult = vkResetFences(_device, 1, &fence);
            CheckResult(resetResult);
            ReturnSubmissionFence(fence);
            lock (_stagingResourcesLock)
            {
                if (_submittedStagingTextures.TryGetValue(completedCB, out var stagingTex))
                {
                    _submittedStagingTextures.Remove(completedCB);
                    _availableStagingTextures.Add(stagingTex);
                }
                if (_submittedStagingBuffers.TryGetValue(completedCB, out var stagingBuffer))
                {
                    _submittedStagingBuffers.Remove(completedCB);
                    if (stagingBuffer.SizeInBytes <= MaxStagingBufferSize)
                    {
                        _availableStagingBuffers.Add(stagingBuffer);
                    }
                    else
                    {
                        stagingBuffer.Dispose();
                    }
                }
                if (_submittedSharedCommandPools.TryGetValue(completedCB, out var sharedPool))
                {
                    _submittedSharedCommandPools.Remove(completedCB);
                    lock (_graphicsCommandPoolLock)
                    {
                        if (sharedPool.IsCached)
                        {
                            _sharedGraphicsCommandPools.Push(sharedPool);
                        }
                        else
                        {
                            sharedPool.Destroy();
                        }
                    }
                }
            }
        }

        private void ReturnSubmissionFence(Vortice.Vulkan.VkFence fence)
        {
            _availableSubmissionFences.Enqueue(fence);
        }

        private VkFence GetFreeSubmissionFence()
        {
            if (_availableSubmissionFences.TryDequeue(out Vortice.Vulkan.VkFence availableFence))
            {
                return availableFence;
            }
            else
            {
                VkFenceCreateInfo fenceCI = new VkFenceCreateInfo
                {
                    sType = VkStructureType.FenceCreateInfo
                };
                Vortice.Vulkan.VkFence newFence;
                VkResult result = vkCreateFence(_device, &fenceCI, null, out newFence);
                CheckResult(result);
                return newFence;
            }
        }

        internal void SetResourceName(DeviceResource resource, string name)
        {
            if (_debugMarkerEnabled)
            {
                switch (resource)
                {
                    case DeviceBuffer buffer:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Buffer, buffer.Buffer.Handle, name);
                        break;
                    case CommandList commandList:
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.CommandBuffer,
                            (ulong)commandList.CommandBuffer.Handle,
                            string.Format("{0}_CommandBuffer", name));
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.CommandPool,
                            commandList.CommandPool.Handle,
                            string.Format("{0}_CommandPool", name));
                        break;
                    case Framebuffer framebuffer:
                        var vkFramebuffer = Util.AssertSubtype<Framebuffer, VkFramebuffer>(framebuffer);
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.Framebuffer,
                            vkFramebuffer.CurrentFramebuffer.Handle,
                            name);
                        break;
                    case Pipeline pipeline:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Pipeline, pipeline.DevicePipeline.Handle, name);
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.PipelineLayout, pipeline.PipelineLayout.Handle, name);
                        break;
                    case ResourceLayout resourceLayout:
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.DescriptorSetLayout,
                            resourceLayout.DescriptorSetLayout.Handle,
                            name);
                        break;
                    case ResourceSet resourceSet:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.DescriptorSet, resourceSet.DescriptorSet.Handle, name);
                        break;
                    case Sampler sampler:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Sampler, sampler.DeviceSampler.Handle, name);
                        break;
                    case Shader shader:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.ShaderModule, shader.ShaderModule.Handle, name);
                        break;
                    case Texture tex:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Image, tex.OptimalDeviceImage.Handle, name);
                        break;
                    case TextureView texView:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.ImageView, texView.ImageView.Handle, name);
                        break;
                    case Fence fence:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Fence, fence.DeviceFence.Handle, name);
                        break;
                    case Swapchain sc:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.SwapchainKHR, sc.DeviceSwapchain.Handle, name);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetDebugMarkerName(VkDebugReportObjectTypeEXT type, ulong target, string name)
        {
            Debug.Assert(_setObjectNameDelegate != null);

            int byteCount = Encoding.UTF8.GetByteCount(name);
            sbyte* utf8Ptr = stackalloc sbyte[byteCount + 1];
            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, (byte*)utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;
            
            var nameInfo = new VkDebugMarkerObjectNameInfoEXT
            {
                sType = VkStructureType.DebugMarkerObjectNameInfoEXT,
                objectType = type,
                @object = target,
                pObjectName = utf8Ptr
            };
            VkResult result = _setObjectNameDelegate(_device, &nameInfo);
            CheckResult(result);
        }

        private void CreateInstance(bool debug, VulkanDeviceOptions options)
        {
            VkResult result = vkInitialize();
            CheckResult(result);
            if (result != VkResult.Success)
            {
                throw new VeldridException(
                    "Vulkan initialization failed. Your GPU may not support Vulkan or you may not have a recent driver.");
            }

            HashSet<string> availableInstanceLayers = new HashSet<string>(EnumerateInstanceLayers());
            HashSet<string> availableInstanceExtensions = new HashSet<string>(GetInstanceExtensions());
            
            var applicationInfo = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                apiVersion = new Vortice.Vulkan.VkVersion(1, 3, 0),
                applicationVersion = new Vortice.Vulkan.VkVersion(1, 0, 0),
                engineVersion = new Vortice.Vulkan.VkVersion(1, 0, 0),
                pApplicationName = s_name.StringPtr,
                pEngineName = s_name.StringPtr
            };

            StackList<IntPtr, Size64Bytes> instanceExtensions = new StackList<IntPtr, Size64Bytes>();
            StackList<IntPtr, Size64Bytes> instanceLayers = new StackList<IntPtr, Size64Bytes>();

            if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME))
            {
                _surfaceExtensions.Add(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(CommonStrings.VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME))
                {
                    _surfaceExtensions.Add(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME);
                }
            }

            foreach (var ext in _surfaceExtensions)
            {
                instanceExtensions.Add(ext);
            }

            string[] requestedInstanceExtensions = options.InstanceExtensions ?? Array.Empty<string>();
            List<FixedUtf8String> tempStrings = new List<FixedUtf8String>();
            foreach (string requiredExt in requestedInstanceExtensions)
            {
                if (!availableInstanceExtensions.Contains(requiredExt))
                {
                    throw new VeldridException($"The required instance extension was not available: {requiredExt}");
                }

                FixedUtf8String utf8Str = new FixedUtf8String(requiredExt);
                instanceExtensions.Add(utf8Str);
                tempStrings.Add(utf8Str);
            }

            bool debugReportExtensionAvailable = false;
            if (debug)
            {
                if (availableInstanceExtensions.Contains(CommonStrings.VK_EXT_DEBUG_REPORT_EXTENSION_NAME))
                {
                    debugReportExtensionAvailable = true;
                    instanceExtensions.Add(CommonStrings.VK_EXT_DEBUG_REPORT_EXTENSION_NAME);
                }
                if (availableInstanceLayers.Contains(CommonStrings.StandardValidationLayerName))
                {
                    _standardValidationSupported = true;
                    instanceLayers.Add(CommonStrings.StandardValidationLayerName);
                }
            }

            var instanceCI = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &applicationInfo,
                enabledExtensionCount = instanceExtensions.Count,
                ppEnabledExtensionNames = (sbyte**)instanceExtensions.Data,
                enabledLayerCount = instanceLayers.Count,
                ppEnabledLayerNames = (instanceLayers.Count > 0) ? (sbyte**)instanceLayers.Data : null
            };

            result = vkCreateInstance(&instanceCI, null, out _instance);
            CheckResult(result);
            if (result != VkResult.Success)
            {
                throw new VeldridException("Failed to create Vulkan instance.");
            }
            vkLoadInstanceOnly(_instance);

            if (debug && debugReportExtensionAvailable)
            {
                EnableDebugCallback();
            }

            foreach (FixedUtf8String tempStr in tempStrings)
            {
                tempStr.Dispose();
            }
        }

        internal bool HasSurfaceExtension(FixedUtf8String extension)
        {
            return _surfaceExtensions.Contains(extension);
        }

        internal void EnableDebugCallback(VkDebugReportFlagsEXT flags = VkDebugReportFlagsEXT.Warning | VkDebugReportFlagsEXT.Error)
        {
            Debug.WriteLine("Enabling Vulkan Debug callbacks.");
            _debugCallbackFunc = DebugCallback;
            IntPtr debugFunctionPtr = Marshal.GetFunctionPointerForDelegate(_debugCallbackFunc);
            var debugCallbackCI = new VkDebugReportCallbackCreateInfoEXT
            {
                sType = VkStructureType.DebugReportCallbackCreateInfoEXT,
                flags = flags,
                pfnCallback = (delegate* unmanaged<VkDebugReportFlagsEXT, VkDebugReportObjectTypeEXT, ulong, nuint, int, sbyte*, sbyte*, void*, uint>)debugFunctionPtr
            };
            IntPtr createFnPtr;
            using (FixedUtf8String debugExtFnName = "vkCreateDebugReportCallbackEXT")
            {
                createFnPtr = (IntPtr)vkGetInstanceProcAddr(_instance, debugExtFnName.Span);
            }
            if (createFnPtr == IntPtr.Zero)
            {
                return;
            }

            vkCreateDebugReportCallbackEXT_d createDelegate = Marshal.GetDelegateForFunctionPointer<vkCreateDebugReportCallbackEXT_d>(createFnPtr);
            VkResult result = createDelegate(_instance, &debugCallbackCI, IntPtr.Zero, out _debugCallbackHandle);
            CheckResult(result);
        }

        private uint DebugCallback(
            VkDebugReportFlagsEXT flags,
            VkDebugReportObjectTypeEXT objectType,
            ulong @object,
            nuint location,
            int messageCode,
            sbyte* pLayerPrefix,
            sbyte* pMessage,
            void* pUserData)
        {
            string message = Util.GetString((byte*)pMessage);
            VkDebugReportFlagsEXT debugReportFlags = (VkDebugReportFlagsEXT)flags;

#if DEBUG
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
#endif

            string fullMessage = $"[{debugReportFlags}] ({objectType}) {message}";

            if (debugReportFlags == VkDebugReportFlagsEXT.Error)
            {
                throw new VeldridException("A Vulkan validation error was encountered: " + fullMessage);
            }

            Console.WriteLine(fullMessage);
            return 0;
        }

        private void CreatePhysicalDevice()
        {
            int deviceCount = 0;
            vkEnumeratePhysicalDevices(_instance, &deviceCount, null);
            if (deviceCount == 0)
            {
                throw new InvalidOperationException("No physical devices exist.");
            }

            VkPhysicalDevice[] physicalDevices = new VkPhysicalDevice[deviceCount];
            fixed (VkPhysicalDevice* pPhysicalDevice = &physicalDevices[0])
                vkEnumeratePhysicalDevices(_instance, &deviceCount, pPhysicalDevice);
            
            // Sort the list of devices such that discrete devices have priority over integrated ones
            int discreteCount = 0;
            for (int i = 0; i < deviceCount; i++)
            {
                vkGetPhysicalDeviceProperties(physicalDevices[i], out var props);
                if (props.deviceType == VkPhysicalDeviceType.DiscreteGpu)
                {
                    // Push discrete GPUs to the top
                    (physicalDevices[discreteCount], physicalDevices[i]) = (physicalDevices[i], physicalDevices[discreteCount]);
                    discreteCount++;
                }
            }

            // Search for a GPU that supports required features
            foreach (var device in physicalDevices)
            {
                vkGetPhysicalDeviceProperties(device, out var physicalDeviceProperties);
                string deviceName;
                sbyte* utf8NamePtr = physicalDeviceProperties.deviceName;
                deviceName = Encoding.UTF8.GetString((byte*)utf8NamePtr, (int)VK_MAX_PHYSICAL_DEVICE_NAME_SIZE);

                var deviceVulkan13Features = new VkPhysicalDeviceVulkan13Features
                {
                    sType = VkStructureType.PhysicalDeviceVulkan13Features,
                };
                var deviceVulkan12Features = new VkPhysicalDeviceVulkan12Features
                {
                    sType = VkStructureType.PhysicalDeviceVulkan12Features,
                    pNext = &deviceVulkan13Features,
                };
                var deviceVulkan11Features = new VkPhysicalDeviceVulkan11Features
                {
                    sType = VkStructureType.PhysicalDeviceVulkan11Features,
                    pNext = &deviceVulkan12Features,
                };
                var deviceFeatures = new VkPhysicalDeviceFeatures2
                {
                    sType = VkStructureType.PhysicalDeviceFeatures2,
                    pNext = &deviceVulkan11Features,
                };

                vkGetPhysicalDeviceFeatures2(device, &deviceFeatures);
                vkGetPhysicalDeviceMemoryProperties(device, out var physicalDeviceMemProperties);
                
                // Check for required features
                if (deviceFeatures.features.multiDrawIndirect != VkBool32.True ||
                    deviceFeatures.features.drawIndirectFirstInstance != VkBool32.True ||
                    deviceFeatures.features.shaderInt64 != VkBool32.True ||
                    deviceFeatures.features.fragmentStoresAndAtomics != VkBool32.True)
                    continue;
                if (deviceVulkan11Features.storageBuffer16BitAccess != VkBool32.True)
                    continue;
                if (deviceVulkan12Features.drawIndirectCount != VkBool32.True ||
                    deviceVulkan12Features.descriptorIndexing != VkBool32.True ||
                    deviceVulkan12Features.descriptorBindingVariableDescriptorCount != VkBool32.True ||
                    deviceVulkan12Features.runtimeDescriptorArray != VkBool32.True ||
                    deviceVulkan12Features.descriptorBindingSampledImageUpdateAfterBind != VkBool32.True ||
                    deviceVulkan12Features.shaderSampledImageArrayNonUniformIndexing != VkBool32.True)
                    continue;
                if (deviceVulkan13Features.synchronization2 != VkBool32.True ||
                    deviceVulkan13Features.dynamicRendering != VkBool32.True ||
                    deviceVulkan13Features.maintenance4 != VkBool32.True)
                    continue;
                
                // We found a physical device with the required features
                _physicalDevice = device;
                _physicalDeviceMemProperties = physicalDeviceMemProperties;
                _physicalDeviceFeatures = deviceFeatures.features;
                _physicalDeviceFeatures11 = deviceVulkan11Features;
                _physicalDeviceFeatures12 = deviceVulkan12Features;
                _physicalDeviceFeatures13 = deviceVulkan13Features;
                return;
            }

            throw new VeldridException(
                "Could not find a supported GPU. Your GPU may be too old or your drivers may be out of date.");
        }

        private void CreateLogicalDevice(VkSurfaceKHR surface, bool preferStandardClipY, VulkanDeviceOptions options)
        {
            GetQueueFamilyIndices(surface);

            HashSet<uint> familyIndices = new HashSet<uint> { _graphicsQueueIndex, _presentQueueIndex, _transferQueueIndex };
            VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[familyIndices.Count];
            uint queueCreateInfosCount = (uint)familyIndices.Count;

            int i = 0;
            foreach (uint index in familyIndices)
            {
                float priority = 1f;
                var queueCreateInfo = new VkDeviceQueueCreateInfo
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = (index == _transferQueueIndex) ? _transferQueueIndex : _graphicsQueueIndex,
                    queueCount = 1,
                    pQueuePriorities = &priority
                };
                queueCreateInfos[i] = queueCreateInfo;
                i += 1;
            }

            var deviceFeatures = new VkPhysicalDeviceFeatures
            {
                samplerAnisotropy = _physicalDeviceFeatures.samplerAnisotropy,
                fillModeNonSolid = _physicalDeviceFeatures.fillModeNonSolid,
                geometryShader = _physicalDeviceFeatures.geometryShader,
                depthClamp = _physicalDeviceFeatures.depthClamp,
                multiViewport = _physicalDeviceFeatures.multiViewport,
                textureCompressionBC = _physicalDeviceFeatures.textureCompressionBC,
                textureCompressionETC2 = _physicalDeviceFeatures.textureCompressionETC2,
                multiDrawIndirect = _physicalDeviceFeatures.multiDrawIndirect,
                drawIndirectFirstInstance = _physicalDeviceFeatures.drawIndirectFirstInstance,
                shaderStorageImageMultisample = _physicalDeviceFeatures.shaderStorageImageMultisample,
                shaderInt64 = VkBool32.True,
                fragmentStoresAndAtomics = VkBool32.True
            };

            var deviceFeatures11 = new VkPhysicalDeviceVulkan11Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan11Features,
                storageBuffer16BitAccess = VkBool32.True
            };

            var deviceFeatures12 = new VkPhysicalDeviceVulkan12Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan12Features,
                drawIndirectCount = VkBool32.True,
                descriptorIndexing = VkBool32.True,
                descriptorBindingVariableDescriptorCount = VkBool32.True,
                descriptorBindingSampledImageUpdateAfterBind = VkBool32.True,
                runtimeDescriptorArray = VkBool32.True,
                shaderSampledImageArrayNonUniformIndexing = VkBool32.True
            };
            
            var deviceFeatures13 = new VkPhysicalDeviceVulkan13Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan13Features,
                synchronization2 = VkBool32.True,
                dynamicRendering = VkBool32.True,
                maintenance4 = VkBool32.True
            };
            deviceFeatures11.pNext = &deviceFeatures12;
            deviceFeatures12.pNext = &deviceFeatures13;

            int propertyCount = 0;
            VkResult result = vkEnumerateDeviceExtensionProperties(_physicalDevice, (sbyte*)null, &propertyCount, null);
            CheckResult(result);
            VkExtensionProperties* properties = stackalloc VkExtensionProperties[(int)propertyCount];
            result = vkEnumerateDeviceExtensionProperties(_physicalDevice, (sbyte*)null, &propertyCount, properties);
            CheckResult(result);

            HashSet<string> requiredInstanceExtensions = new HashSet<string>(options.DeviceExtensions ?? Array.Empty<string>());

            bool hasMemReqs2 = false;
            bool hasDedicatedAllocation = false;
            StackList<IntPtr> extensionNames = new StackList<IntPtr>();
            for (int property = 0; property < propertyCount; property++)
            {
                string extensionName = Util.GetString((byte*)properties[property].extensionName);
                if (extensionName == "VK_EXT_debug_marker")
                {
                    extensionNames.Add(CommonStrings.VK_EXT_DEBUG_MARKER_EXTENSION_NAME);
                    requiredInstanceExtensions.Remove(extensionName);
                    _debugMarkerEnabled = true;
                }
                else if (extensionName == "VK_KHR_swapchain")
                {
                    extensionNames.Add((IntPtr)properties[property].extensionName);
                    requiredInstanceExtensions.Remove(extensionName);
                }
                else if (preferStandardClipY && extensionName == "VK_KHR_maintenance1")
                {
                    extensionNames.Add((IntPtr)properties[property].extensionName);
                    requiredInstanceExtensions.Remove(extensionName);
                    _standardClipYDirection = true;
                }
                else if (extensionName == "VK_KHR_get_memory_requirements2")
                {
                    extensionNames.Add((IntPtr)properties[property].extensionName);
                    requiredInstanceExtensions.Remove(extensionName);
                    hasMemReqs2 = true;
                }
                else if (extensionName == "VK_KHR_dedicated_allocation")
                {
                    extensionNames.Add((IntPtr)properties[property].extensionName);
                    requiredInstanceExtensions.Remove(extensionName);
                    hasDedicatedAllocation = true;
                }
                else if (requiredInstanceExtensions.Remove(extensionName))
                {
                    extensionNames.Add((IntPtr)properties[property].extensionName);
                }
            }

            if (requiredInstanceExtensions.Count != 0)
            {
                string missingList = string.Join(", ", requiredInstanceExtensions);
                throw new VeldridException(
                    $"The following Vulkan device extensions were not available: {missingList}");
            }

            StackList<IntPtr> layerNames = new StackList<IntPtr>();
            if (_standardValidationSupported)
            {
                layerNames.Add(CommonStrings.StandardValidationLayerName);
            }
            
            var physicalDeviceFeatures2 = new VkPhysicalDeviceFeatures2
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
                features = deviceFeatures,
                pNext = &deviceFeatures11
            };
            
            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                sType = VkStructureType.DeviceCreateInfo,
                queueCreateInfoCount = queueCreateInfosCount,
                pQueueCreateInfos = queueCreateInfos,
                pEnabledFeatures = null,
                enabledLayerCount = layerNames.Count,
                ppEnabledLayerNames = (sbyte**)layerNames.Data,
                enabledExtensionCount = extensionNames.Count,
                ppEnabledExtensionNames = (sbyte**)extensionNames.Data,
                pNext = &physicalDeviceFeatures2
            };

            result = vkCreateDevice(_physicalDevice, &deviceCreateInfo, null, out _device);
            CheckResult(result);
            vkLoadDevice(_device);

            vkGetDeviceQueue(_device, _graphicsQueueIndex, 0, out _graphicsQueue);
            vkGetDeviceQueue(_device, _transferQueueIndex, 0, out _transferQueue);

            if (_debugMarkerEnabled)
            {
                _setObjectNameDelegate = Marshal.GetDelegateForFunctionPointer<vkDebugMarkerSetObjectNameEXT_t>(
                    GetInstanceProcAddr("vkDebugMarkerSetObjectNameEXT"));
                _markerBegin = Marshal.GetDelegateForFunctionPointer<vkCmdDebugMarkerBeginEXT_t>(
                    GetInstanceProcAddr("vkCmdDebugMarkerBeginEXT"));
                _markerEnd = Marshal.GetDelegateForFunctionPointer<vkCmdDebugMarkerEndEXT_t>(
                    GetInstanceProcAddr("vkCmdDebugMarkerEndEXT"));
                _markerInsert = Marshal.GetDelegateForFunctionPointer<vkCmdDebugMarkerInsertEXT_t>(
                    GetInstanceProcAddr("vkCmdDebugMarkerInsertEXT"));
            }
            if (hasDedicatedAllocation && hasMemReqs2)
            {
                _getBufferMemoryRequirements2 = GetDeviceProcAddr<vkGetBufferMemoryRequirements2_t>("vkGetBufferMemoryRequirements2")
                    ?? GetDeviceProcAddr<vkGetBufferMemoryRequirements2_t>("vkGetBufferMemoryRequirements2KHR");
                _getImageMemoryRequirements2 = GetDeviceProcAddr<vkGetImageMemoryRequirements2_t>("vkGetImageMemoryRequirements2")
                    ?? GetDeviceProcAddr<vkGetImageMemoryRequirements2_t>("vkGetImageMemoryRequirements2KHR");
            }
        }

        private IntPtr GetInstanceProcAddr(string name)
        {
            int byteCount = Encoding.UTF8.GetByteCount(name);
            sbyte* utf8Ptr = stackalloc sbyte[byteCount + 1];

            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, (byte*)utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            return (IntPtr)vkGetInstanceProcAddr(_instance, new ReadOnlySpan<sbyte>(utf8Ptr, byteCount + 1));
        }

        private IntPtr GetDeviceProcAddr(string name)
        {
            int byteCount = Encoding.UTF8.GetByteCount(name);
            sbyte* utf8Ptr = stackalloc sbyte[byteCount + 1];

            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, (byte*)utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            return (IntPtr)vkGetDeviceProcAddr(_device, utf8Ptr);
        }

        private T GetDeviceProcAddr<T>(string name)
        {
            IntPtr funcPtr = GetDeviceProcAddr(name);
            if (funcPtr != IntPtr.Zero) { return Marshal.GetDelegateForFunctionPointer<T>(funcPtr); }
            else { return default; }
        }

        private void GetQueueFamilyIndices(VkSurfaceKHR surface)
        {
            int queueFamilyCount = 0;
            vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &queueFamilyCount, null);
            VkQueueFamilyProperties[] qfp = new VkQueueFamilyProperties[queueFamilyCount];
            fixed (VkQueueFamilyProperties *p = &qfp[0])
                vkGetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &queueFamilyCount, p);

            bool foundGraphics = false;
            bool foundTransfer = false;
            bool foundPresent = surface == VkSurfaceKHR.Null;

            for (uint i = 0; i < qfp.Length; i++)
            {
                if ((qfp[i].queueFlags & VkQueueFlags.Graphics) != 0)
                {
                    _graphicsQueueIndex = i;
                }

                if ((qfp[i].queueFlags & VkQueueFlags.Transfer) != 0)
                {
                    _transferQueueIndex = i;
                    foundTransfer = true;
                }

                if (!foundPresent)
                {
                    vkGetPhysicalDeviceSurfaceSupportKHR(_physicalDevice, i, surface, out VkBool32 presentSupported);
                    if (presentSupported)
                    {
                        _presentQueueIndex = i;
                        foundPresent = true;
                    }
                }

                if (foundGraphics && foundPresent && foundTransfer)
                {
                    return;
                }
            }

            if (!foundTransfer)
            {
                _transferQueueIndex = _graphicsQueueIndex;
            }
        }

        private void CreateDescriptorPool()
        {
            _descriptorPoolManager = new VkDescriptorPoolManager(this);
        }

        private void CreateGraphicsCommandPool()
        {
            var commandPoolCI = new VkCommandPoolCreateInfo
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
                queueFamilyIndex = _graphicsQueueIndex
            };
            var result = vkCreateCommandPool(_device, &commandPoolCI, null, out _graphicsCommandPool);
            CheckResult(result);
        }

        private void CreateTransferCommandPool()
        {
            var commandPoolCI = new VkCommandPoolCreateInfo
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
                queueFamilyIndex = _transferQueueIndex
            };
            var result = vkCreateCommandPool(_device, &commandPoolCI, null, out _transferCommandPool);
            CheckResult(result);
        }

        internal VkFilter GetFormatFilter(VkFormat format)
        {
            if (!_filters.TryGetValue(format, out VkFilter filter))
            {
                vkGetPhysicalDeviceFormatProperties(_physicalDevice, format, out VkFormatProperties vkFormatProps);
                filter = (vkFormatProps.optimalTilingFeatures & VkFormatFeatureFlags.SampledImageFilterLinear) != 0
                    ? VkFilter.Linear
                    : VkFilter.Nearest;
                _filters.TryAdd(format, filter);
            }

            return filter;
        }
        
        private SharedCommandPool GetFreeCommandPool()
        {
            if (!_sharedGraphicsCommandPools.TryPop(out SharedCommandPool sharedPool))
            {
                sharedPool = new SharedCommandPool(this, false);
            }

            return sharedPool;
        }

        private IntPtr MapBuffer(DeviceBuffer buffer, uint numBytes)
        {
            if (buffer.AllocationInfo.pMappedData != null)
            {
                return (IntPtr)buffer.AllocationInfo.pMappedData;
            }
            else
            {
                void* mappedPtr;
                VkResult result = Vma.vmaMapMemory(Allocator, buffer.Allocation, &mappedPtr);
                CheckResult(result);
                return (IntPtr)mappedPtr;
            }
        }

        private void UnmapBuffer(DeviceBuffer buffer)
        {
            if (buffer.AllocationInfo.pMappedData == null)
            {
                Vma.vmaUnmapMemory(Allocator, buffer.Allocation);
            }
        }
        
        private Texture GetFreeStagingTexture(uint width, uint height, uint depth, VkFormat format)
        {
            uint totalSize = FormatHelpers.GetRegionSize(width, height, depth, format);
            lock (_stagingResourcesLock)
            {
                for (int i = 0; i < _availableStagingTextures.Count; i++)
                {
                    var tex = _availableStagingTextures[i];
                    if (tex.AllocationInfo.size >= totalSize)
                    {
                        _availableStagingTextures.RemoveAt(i);
                        tex.SetStagingDimensions(width, height, depth, format);
                        return tex;
                    }
                }
            }

            uint texWidth = Math.Max(256, width);
            uint texHeight = Math.Max(256, height);
            var newTex = ResourceFactory.CreateTexture(TextureDescription.Texture3D(
                texWidth, texHeight, depth, 1, format, 
                VkImageUsageFlags.None, 
                VkImageCreateFlags.None, 
                VkImageTiling.Linear));
            newTex.SetStagingDimensions(width, height, depth, format);

            return newTex;
        }

        private DeviceBuffer GetFreeStagingBuffer(uint size)
        {
            lock (_stagingResourcesLock)
            {
                for (int i = 0; i < _availableStagingBuffers.Count; i++)
                {
                    var buffer = _availableStagingBuffers[i];
                    if (buffer.SizeInBytes >= size)
                    {
                        _availableStagingBuffers.RemoveAt(i);
                        return buffer;
                    }
                }
            }

            uint newBufferSize = Math.Max(MinStagingBufferSize, size);
            var newBuffer = ResourceFactory.CreateBuffer(
                new BufferDescription(
                    newBufferSize,
                    VkBufferUsageFlags.None,
                    VmaMemoryUsage.AutoPreferHost,
                    VmaAllocationCreateFlags.Mapped));
            return newBuffer;
        }

        internal static bool IsSupported()
        {
            return s_isSupported.Value;
        }

        private static bool CheckIsSupported()
        {
            if (!IsVulkanLoaded())
            {
                return false;
            }
            
            var applicationInfo = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                apiVersion = new VkVersion(1, 2, 0),
                applicationVersion = new VkVersion(1, 0, 0),
                engineVersion = new VkVersion(1, 0, 0),
                pApplicationName = (sbyte*)s_name.StringPtr,
                pEngineName = (sbyte*)s_name.StringPtr
            };

            var instanceCI = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &applicationInfo
            };
            VkResult result = vkCreateInstance(&instanceCI, null, out VkInstance testInstance);
            if (result != VkResult.Success)
            {
                return false;
            }
            vkLoadInstanceOnly(testInstance);

            int physicalDeviceCount = 0;
            result = vkEnumeratePhysicalDevices(testInstance, &physicalDeviceCount, null);
            if (result != VkResult.Success || physicalDeviceCount == 0)
            {
                vkDestroyInstance(testInstance, null);
                return false;
            }

            vkDestroyInstance(testInstance, null);

            HashSet<string> instanceExtensions = new HashSet<string>(GetInstanceExtensions());
            if (!instanceExtensions.Contains(CommonStrings.VK_KHR_SURFACE_EXTENSION_NAME))
            {
                return false;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return instanceExtensions.Contains(CommonStrings.VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (RuntimeInformation.OSDescription.Contains("Unix")) // Android
                {
                    return instanceExtensions.Contains(CommonStrings.VK_KHR_ANDROID_SURFACE_EXTENSION_NAME);
                }
                else
                {
                    return instanceExtensions.Contains(CommonStrings.VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (RuntimeInformation.OSDescription.Contains("Darwin")) // macOS
                {
                    return instanceExtensions.Contains(CommonStrings.VK_MVK_MACOS_SURFACE_EXTENSION_NAME);
                }
                else // iOS
                {
                    return instanceExtensions.Contains(CommonStrings.VK_MVK_IOS_SURFACE_EXTENSION_NAME);
                }
            }

            return false;
        }

        internal void ClearColorTexture(Texture texture, VkClearColorValue color)
        {
            VkImageSubresourceRange range = new VkImageSubresourceRange(
                 VkImageAspectFlags.Color,
                 0,
                 texture.MipLevels,
                 0,
                 texture.ArrayLayers);
            SharedCommandPool pool = GetFreeCommandPool();
            VkCommandBuffer cb = pool.BeginNewCommandBuffer();
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, VkImageLayout.TransferDstOptimal);
            vkCmdClearColorImage(cb, texture.OptimalDeviceImage, VkImageLayout.TransferDstOptimal, &color, 1, &range);
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, VkImageLayout.ColorAttachmentOptimal);
            pool.EndAndSubmit(cb);
        }

        internal void ClearDepthTexture(Texture texture, VkClearDepthStencilValue clearValue)
        {
            VkImageAspectFlags aspect = FormatHelpers.IsStencilFormat(texture.Format)
                ? VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil
                : VkImageAspectFlags.Depth;
            VkImageSubresourceRange range = new VkImageSubresourceRange(
                aspect,
                0,
                texture.MipLevels,
                0,
                texture.ArrayLayers);
            SharedCommandPool pool = GetFreeCommandPool();
            VkCommandBuffer cb = pool.BeginNewCommandBuffer();
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, VkImageLayout.TransferDstOptimal);
            vkCmdClearDepthStencilImage(
                cb,
                texture.OptimalDeviceImage,
                VkImageLayout.TransferDstOptimal,
                &clearValue,
                1,
                &range);
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, VkImageLayout.DepthStencilAttachmentOptimal);
            pool.EndAndSubmit(cb);
        }

        internal void TransitionImageLayout(Texture texture, VkImageLayout layout)
        {
            SharedCommandPool pool = GetFreeCommandPool();
            VkCommandBuffer cb = pool.BeginNewCommandBuffer();
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, layout);
            pool.EndAndSubmit(cb);
        }

        private class SharedCommandPool
        {
            private readonly GraphicsDevice _gd;
            private readonly VkCommandPool _pool;
            private readonly VkCommandBuffer _cb;

            public bool IsCached { get; }

            public SharedCommandPool(GraphicsDevice gd, bool isCached)
            {
                _gd = gd;
                IsCached = isCached;

                var commandPoolCI = new VkCommandPoolCreateInfo
                {
                    sType = VkStructureType.CommandPoolCreateInfo,
                    flags = VkCommandPoolCreateFlags.Transient | VkCommandPoolCreateFlags.ResetCommandBuffer,
                    queueFamilyIndex = _gd.GraphicsQueueIndex
                };
                VkResult result = vkCreateCommandPool(_gd.Device, &commandPoolCI, null, out _pool);
                CheckResult(result);

                var allocateInfo = new VkCommandBufferAllocateInfo
                {
                    sType = VkStructureType.CommandBufferAllocateInfo,
                    commandBufferCount = 1,
                    level = VkCommandBufferLevel.Primary,
                    commandPool = _pool
                };
                fixed (VkCommandBuffer *cbp = &_cb)
                    result = vkAllocateCommandBuffers(_gd.Device, &allocateInfo, cbp);
                CheckResult(result);
            }

            public VkCommandBuffer BeginNewCommandBuffer()
            {
                var beginInfo = new VkCommandBufferBeginInfo
                {
                    sType = VkStructureType.CommandBufferBeginInfo,
                    flags = VkCommandBufferUsageFlags.OneTimeSubmit
                };
                VkResult result = vkBeginCommandBuffer(_cb, &beginInfo);
                CheckResult(result);

                return _cb;
            }

            public void EndAndSubmit(VkCommandBuffer cb)
            {
                VkResult result = vkEndCommandBuffer(cb);
                CheckResult(result);
                _gd.SubmitCommandBuffer(null, cb, 0, null, 0, null, null, false);
                lock (_gd._stagingResourcesLock)
                {
                    _gd._submittedSharedCommandPools.Add(cb, this);
                }
            }

            internal void Destroy()
            {
                vkDestroyCommandPool(_gd.Device, _pool, null);
            }
        }

        private struct FenceSubmissionInfo
        {
            public VkFence Fence;
            public CommandList CommandList;
            public VkCommandBuffer CommandBuffer;
            public FenceSubmissionInfo(VkFence fence, CommandList commandList, VkCommandBuffer commandBuffer)
            {
                Fence = fence;
                CommandList = commandList;
                CommandBuffer = commandBuffer;
            }
        }

        /// <summary>
        /// Frees unmanaged resources controlled by this device.
        /// All created child resources must be Disposed prior to calling this method.
        /// </summary>
        public void Dispose()
        {
            WaitForIdle();
            PointSampler.Dispose();
            LinearSampler.Dispose();
            Aniso4xSampler.Dispose();
            PlatformDispose();
        }

        /// <summary>
        /// Tries to get a <see cref="BackendInfoVulkan"/> for this instance. This method will only succeed if this is a Vulkan
        /// GraphicsDevice.
        /// </summary>
        /// <param name="info">If successful, this will contain the <see cref="BackendInfoVulkan"/> for this instance.</param>
        /// <returns>True if this is a Vulkan GraphicsDevice and the operation was successful. False otherwise.</returns>
        public bool GetVulkanInfo(out BackendInfoVulkan info)
        {
            info = _vulkanInfo;
            return true;
        }

        /// <summary>
        /// Gets a <see cref="BackendInfoVulkan"/> for this instance. This method will only succeed if this is a Vulkan
        /// GraphicsDevice. Otherwise, this method will throw an exception.
        /// </summary>
        /// <returns>The <see cref="BackendInfoVulkan"/> for this instance.</returns>
        public BackendInfoVulkan GetVulkanInfo()
        {
            if (!GetVulkanInfo(out BackendInfoVulkan info))
            {
                throw new VeldridException($"{nameof(GetVulkanInfo)} can only be used on a Vulkan GraphicsDevice.");
            }

            return info;
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Vulkan.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Vulkan API.</returns>
        public static GraphicsDevice CreateVulkan(GraphicsDeviceOptions options)
        {
            return new GraphicsDevice(options, null);
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Vulkan.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="vkOptions">The Vulkan-specific options used to create the device.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Vulkan API.</returns>
        public static GraphicsDevice CreateVulkan(GraphicsDeviceOptions options, VulkanDeviceOptions vkOptions)
        {
            return new GraphicsDevice(options, null, vkOptions);
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Vulkan, with a main Swapchain.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="swapchainDescription">A description of the main Swapchain to create.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Vulkan API.</returns>
        public static GraphicsDevice CreateVulkan(GraphicsDeviceOptions options, SwapchainDescription swapchainDescription)
        {
            return new GraphicsDevice(options, swapchainDescription);
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Vulkan, with a main Swapchain.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="vkOptions">The Vulkan-specific options used to create the device.</param>
        /// <param name="swapchainDescription">A description of the main Swapchain to create.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Vulkan API.</returns>
        public static GraphicsDevice CreateVulkan(
            GraphicsDeviceOptions options,
            SwapchainDescription swapchainDescription,
            VulkanDeviceOptions vkOptions)
        {
            return new GraphicsDevice(options, swapchainDescription, vkOptions);
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Vulkan, with a main Swapchain.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="surfaceSource">The source from which a Vulkan surface can be created.</param>
        /// <param name="width">The initial width of the window.</param>
        /// <param name="height">The initial height of the window.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Vulkan API.</returns>
        public static GraphicsDevice CreateVulkan(GraphicsDeviceOptions options, VkSurfaceSource surfaceSource, uint width, uint height)
        {
            SwapchainDescription scDesc = new SwapchainDescription(
                surfaceSource.GetSurfaceSource(),
                width, height,
                options.SwapchainDepthFormat,
                options.SyncToVerticalBlank,
                options.SwapchainSrgbFormat);

            return new GraphicsDevice(options, scDesc);
        }
    }
    
    internal unsafe delegate VkResult vkCreateDebugReportCallbackEXT_d(
        VkInstance instance,
        VkDebugReportCallbackCreateInfoEXT* createInfo,
        IntPtr allocatorPtr,
        out VkDebugReportCallbackEXT ret);

    internal unsafe delegate void vkDestroyDebugReportCallbackEXT_d(
        VkInstance instance,
        VkDebugReportCallbackEXT callback,
        VkAllocationCallbacks* pAllocator);

    internal unsafe delegate VkResult vkDebugMarkerSetObjectNameEXT_t(VkDevice device, VkDebugMarkerObjectNameInfoEXT* pNameInfo);
    internal unsafe delegate void vkCmdDebugMarkerBeginEXT_t(VkCommandBuffer commandBuffer, VkDebugMarkerMarkerInfoEXT* pMarkerInfo);
    internal unsafe delegate void vkCmdDebugMarkerEndEXT_t(VkCommandBuffer commandBuffer);
    internal unsafe delegate void vkCmdDebugMarkerInsertEXT_t(VkCommandBuffer commandBuffer, VkDebugMarkerMarkerInfoEXT* pMarkerInfo);

    internal unsafe delegate void vkGetBufferMemoryRequirements2_t(VkDevice device, VkBufferMemoryRequirementsInfo2* pInfo, VkMemoryRequirements2* pMemoryRequirements);
    internal unsafe delegate void vkGetImageMemoryRequirements2_t(VkDevice device, VkImageMemoryRequirementsInfo2* pInfo, VkMemoryRequirements2* pMemoryRequirements);
}
