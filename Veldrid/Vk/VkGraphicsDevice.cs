﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;

namespace Veldrid.Vk
{
    internal unsafe class VkGraphicsDevice : GraphicsDevice
    {
        private static readonly FixedUtf8String s_name = "Veldrid-VkGraphicsDevice";
        private static readonly Lazy<bool> s_isSupported = new Lazy<bool>(CheckIsSupported, isThreadSafe: true);

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
        private readonly List<VkTexture> _availableStagingTextures = new List<VkTexture>();
        private readonly List<VkBuffer> _availableStagingBuffers = new List<VkBuffer>();

        private readonly Dictionary<VkCommandBuffer, VkTexture> _submittedStagingTextures
            = new Dictionary<VkCommandBuffer, VkTexture>();
        private readonly Dictionary<VkCommandBuffer, VkBuffer> _submittedStagingBuffers
            = new Dictionary<VkCommandBuffer, VkBuffer>();
        private readonly Dictionary<VkCommandBuffer, SharedCommandPool> _submittedSharedCommandPools
            = new Dictionary<VkCommandBuffer, SharedCommandPool>();

        public override GraphicsBackend BackendType => GraphicsBackend.Vulkan;

        public override bool IsUvOriginTopLeft => true;

        public override bool IsDepthRangeZeroToOne => true;

        public override bool IsClipSpaceYInverted => !_standardClipYDirection;

        public override Swapchain MainSwapchain => _mainSwapchain;

        public override GraphicsDeviceFeatures Features { get; }

        public override bool GetVulkanInfo(out BackendInfoVulkan info)
        {
            info = _vulkanInfo;
            return true;
        }

        public VkInstance Instance => _instance;
        public VkDevice Device => _device;
        public VkPhysicalDevice PhysicalDevice => _physicalDevice;
        public VkPhysicalDeviceMemoryProperties PhysicalDeviceMemProperties => _physicalDeviceMemProperties;
        public VkQueue GraphicsQueue => _graphicsQueue;
        public uint GraphicsQueueIndex => _graphicsQueueIndex;
        public uint TransferQueueIndex => _transferQueueIndex;
        public uint PresentQueueIndex => _presentQueueIndex;
        public VmaAllocator Allocator => _vmaAllocator;
        public VkDescriptorPoolManager DescriptorPoolManager => _descriptorPoolManager;
        public vkCmdDebugMarkerBeginEXT_t MarkerBegin => _markerBegin;
        public vkCmdDebugMarkerEndEXT_t MarkerEnd => _markerEnd;
        public vkCmdDebugMarkerInsertEXT_t MarkerInsert => _markerInsert;
        public vkGetBufferMemoryRequirements2_t GetBufferMemoryRequirements2 => _getBufferMemoryRequirements2;
        public vkGetImageMemoryRequirements2_t GetImageMemoryRequirements2 => _getImageMemoryRequirements2;

        private readonly object _submittedFencesLock = new object();
        private readonly ConcurrentQueue<Vortice.Vulkan.VkFence> _availableSubmissionFences = new ConcurrentQueue<Vortice.Vulkan.VkFence>();
        private readonly List<FenceSubmissionInfo> _submittedFences = new List<FenceSubmissionInfo>();
        private readonly VkSwapchain _mainSwapchain;

        private readonly List<FixedUtf8String> _surfaceExtensions = new List<FixedUtf8String>();

        public VkGraphicsDevice(GraphicsDeviceOptions options, SwapchainDescription? scDesc)
            : this(options, scDesc, new VulkanDeviceOptions()) { }

        public VkGraphicsDevice(GraphicsDeviceOptions options, SwapchainDescription? scDesc, VulkanDeviceOptions vkOptions)
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

            ResourceFactory = new VkResourceFactory(this);

            if (scDesc != null)
            {
                SwapchainDescription desc = scDesc.Value;
                _mainSwapchain = new VkSwapchain(this, ref desc, surface);
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

        public override ResourceFactory ResourceFactory { get; }

        private protected override void SubmitCommandsCore(CommandList cl, Fence fence)
        {
            SubmitCommandList(cl, 0, null, 0, null, fence);
        }

        private void SubmitCommandList(
            CommandList cl,
            uint waitSemaphoreCount,
            VkSemaphore* waitSemaphoresPtr,
            uint signalSemaphoreCount,
            VkSemaphore* signalSemaphoresPtr,
            Fence fence)
        {
            VkCommandList vkCL = Util.AssertSubtype<CommandList, VkCommandList>(cl);
            VkCommandBuffer vkCB = vkCL.CommandBuffer;

            vkCL.CommandBufferSubmitted(vkCB);
            SubmitCommandBuffer(vkCL, vkCB, waitSemaphoreCount, waitSemaphoresPtr, signalSemaphoreCount, signalSemaphoresPtr, fence, vkCL.IsTransfer);
        }

        private void SubmitCommandBuffer(
            VkCommandList vkCL,
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

            Vortice.Vulkan.VkFence vkFence = Vortice.Vulkan.VkFence.Null;
            Vortice.Vulkan.VkFence submissionFence = Vortice.Vulkan.VkFence.Null;
            if (useExtraFence)
            {
                vkFence = Util.AssertSubtype<Fence, VkFence>(fence).DeviceFence;
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
            Vortice.Vulkan.VkFence fence = fsi.Fence;
            VkCommandBuffer completedCB = fsi.CommandBuffer;
            fsi.CommandList?.CommandBufferCompleted(completedCB);
            VkResult resetResult = vkResetFences(_device, 1, &fence);
            CheckResult(resetResult);
            ReturnSubmissionFence(fence);
            lock (_stagingResourcesLock)
            {
                if (_submittedStagingTextures.TryGetValue(completedCB, out VkTexture stagingTex))
                {
                    _submittedStagingTextures.Remove(completedCB);
                    _availableStagingTextures.Add(stagingTex);
                }
                if (_submittedStagingBuffers.TryGetValue(completedCB, out VkBuffer stagingBuffer))
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
                if (_submittedSharedCommandPools.TryGetValue(completedCB, out SharedCommandPool sharedPool))
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

        private Vortice.Vulkan.VkFence GetFreeSubmissionFence()
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

        private protected override void SwapBuffersCore(Swapchain swapchain)
        {
            VkSwapchain vkSC = Util.AssertSubtype<Swapchain, VkSwapchain>(swapchain);
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

        internal void SetResourceName(DeviceResource resource, string name)
        {
            if (_debugMarkerEnabled)
            {
                switch (resource)
                {
                    case VkBuffer buffer:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Buffer, buffer.DeviceBuffer.Handle, name);
                        break;
                    case VkCommandList commandList:
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.CommandBuffer,
                            (ulong)commandList.CommandBuffer.Handle,
                            string.Format("{0}_CommandBuffer", name));
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.CommandPool,
                            commandList.CommandPool.Handle,
                            string.Format("{0}_CommandPool", name));
                        break;
                    case VkFramebuffer framebuffer:
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.Framebuffer,
                            framebuffer.CurrentFramebuffer.Handle,
                            name);
                        break;
                    case VkPipeline pipeline:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Pipeline, pipeline.DevicePipeline.Handle, name);
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.PipelineLayout, pipeline.PipelineLayout.Handle, name);
                        break;
                    case VkResourceLayout resourceLayout:
                        SetDebugMarkerName(
                            VkDebugReportObjectTypeEXT.DescriptorSetLayout,
                            resourceLayout.DescriptorSetLayout.Handle,
                            name);
                        break;
                    case VkResourceSet resourceSet:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.DescriptorSet, resourceSet.DescriptorSet.Handle, name);
                        break;
                    case VkSampler sampler:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Sampler, sampler.DeviceSampler.Handle, name);
                        break;
                    case VkShader shader:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.ShaderModule, shader.ShaderModule.Handle, name);
                        break;
                    case VkTexture tex:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Image, tex.OptimalDeviceImage.Handle, name);
                        break;
                    case VkTextureView texView:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.ImageView, texView.ImageView.Handle, name);
                        break;
                    case VkFence fence:
                        SetDebugMarkerName(VkDebugReportObjectTypeEXT.Fence, fence.DeviceFence.Handle, name);
                        break;
                    case VkSwapchain sc:
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

        public bool HasSurfaceExtension(FixedUtf8String extension)
        {
            return _surfaceExtensions.Contains(extension);
        }

        public void EnableDebugCallback(VkDebugReportFlagsEXT flags = VkDebugReportFlagsEXT.Warning | VkDebugReportFlagsEXT.Error)
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

        protected override MappedResource MapCore(MappableResource resource, MapMode mode, uint subresource)
        {
            VmaAllocation allocation = default(VmaAllocation);
            VmaAllocationInfo info = default(VmaAllocationInfo);
            IntPtr mappedPtr = IntPtr.Zero;
            uint sizeInBytes;
            uint offset = 0;
            uint rowPitch = 0;
            uint depthPitch = 0;
            if (resource is VkBuffer buffer)
            {
                allocation = buffer.Allocation;
                info = buffer.AllocationInfo;
                sizeInBytes = buffer.SizeInBytes;
            }
            else
            {
                VkTexture texture = Util.AssertSubtype<MappableResource, VkTexture>(resource);
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

        protected override void UnmapCore(MappableResource resource, uint subresource)
        {
            VmaAllocation allocation = default(VmaAllocation);
            VmaAllocationInfo info = default(VmaAllocationInfo);
            if (resource is VkBuffer buffer)
            {
                allocation = buffer.Allocation;
                info = buffer.AllocationInfo;
            }
            else
            {
                VkTexture tex = Util.AssertSubtype<MappableResource, VkTexture>(resource);
                allocation = tex.Allocation;
                info = tex.AllocationInfo;
            }

            if (info.pMappedData == null)
            {
                Vma.vmaUnmapMemory(Allocator, allocation);
            }
        }

        protected override void PlatformDispose()
        {
            Debug.Assert(_submittedFences.Count == 0);
            foreach (Vortice.Vulkan.VkFence fence in _availableSubmissionFences)
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
            foreach (VkTexture tex in _availableStagingTextures)
            {
                tex.Dispose();
            }

            Debug.Assert(_submittedStagingBuffers.Count == 0);
            foreach (VkBuffer buffer in _availableStagingBuffers)
            {
                buffer.Dispose();
            }

            while (_sharedGraphicsCommandPools.TryPop(out SharedCommandPool sharedPool))
            {
                sharedPool.Destroy();
            }

            Vma.vmaDestroyAllocator(_vmaAllocator);
            //_memoryManager.Dispose();

            VkResult result = vkDeviceWaitIdle(_device);
            CheckResult(result);
            vkDestroyDevice(_device, null);
            vkDestroyInstance(_instance, null);
        }

        private protected override void WaitForIdleCore()
        {
            lock (_graphicsQueueLock)
            {
                vkQueueWaitIdle(_graphicsQueue);
            }

            CheckSubmittedFences();
        }

        public override TextureSampleCount GetSampleCountLimit(PixelFormat format, bool depthFormat)
        {
            VkImageUsageFlags usageFlags = VkImageUsageFlags.Sampled;
            usageFlags |= depthFormat ? VkImageUsageFlags.DepthStencilAttachment : VkImageUsageFlags.ColorAttachment;

            vkGetPhysicalDeviceImageFormatProperties(
                _physicalDevice,
                VkFormats.VdToVkPixelFormat(format),
                VkImageType.Image2D,
                VkImageTiling.Optimal,
                usageFlags,
                VkImageCreateFlags.None,
                out VkImageFormatProperties formatProperties);

            VkSampleCountFlags vkSampleCounts = formatProperties.sampleCounts;
            if ((vkSampleCounts & VkSampleCountFlags.Count32) == VkSampleCountFlags.Count32)
            {
                return TextureSampleCount.Count32;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count16) == VkSampleCountFlags.Count16)
            {
                return TextureSampleCount.Count16;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count8) == VkSampleCountFlags.Count8)
            {
                return TextureSampleCount.Count8;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count4) == VkSampleCountFlags.Count4)
            {
                return TextureSampleCount.Count4;
            }
            else if ((vkSampleCounts & VkSampleCountFlags.Count2) == VkSampleCountFlags.Count2)
            {
                return TextureSampleCount.Count2;
            }

            return TextureSampleCount.Count1;
        }

        private protected override bool GetPixelFormatSupportCore(
            PixelFormat format,
            TextureType type,
            TextureUsage usage,
            out PixelFormatProperties properties)
        {
            VkFormat vkFormat = VkFormats.VdToVkPixelFormat(format, (usage & TextureUsage.DepthStencil) != 0);
            VkImageType vkType = VkFormats.VdToVkTextureType(type);
            VkImageTiling tiling = usage == TextureUsage.Staging ? VkImageTiling.Linear : VkImageTiling.Optimal;
            VkImageUsageFlags vkUsage = VkFormats.VdToVkTextureUsage(usage);

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

        private protected override void UpdateBufferCore(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes)
        {
            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(buffer);
            VkBuffer copySrcVkBuffer = null;
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
                vkCmdCopyBuffer(cb, copySrcVkBuffer.DeviceBuffer, vkBuffer.DeviceBuffer, 1, &copyRegion);

                pool.EndAndSubmit(cb);
                lock (_stagingResourcesLock)
                {
                    _submittedStagingBuffers.Add(cb, copySrcVkBuffer);
                }
            }
        }

        private SharedCommandPool GetFreeCommandPool()
        {
            if (!_sharedGraphicsCommandPools.TryPop(out SharedCommandPool sharedPool))
            {
                sharedPool = new SharedCommandPool(this, false);
            }

            return sharedPool;
        }

        private IntPtr MapBuffer(VkBuffer buffer, uint numBytes)
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

        private void UnmapBuffer(VkBuffer buffer)
        {
            if (buffer.AllocationInfo.pMappedData == null)
            {
                Vma.vmaUnmapMemory(Allocator, buffer.Allocation);
            }
        }

        private protected override void UpdateTextureCore(
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
            VkTexture vkTex = Util.AssertSubtype<Texture, VkTexture>(texture);
            bool isStaging = (vkTex.Usage & TextureUsage.Staging) != 0;
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
                VkTexture stagingTex = GetFreeStagingTexture(width, height, depth, texture.Format);
                UpdateTexture(stagingTex, source, sizeInBytes, 0, 0, 0, width, height, depth, 0, 0);
                SharedCommandPool pool = GetFreeCommandPool();
                VkCommandBuffer cb = pool.BeginNewCommandBuffer();
                VkCommandList.CopyTextureCore_VkCommandBuffer(
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

        private VkTexture GetFreeStagingTexture(uint width, uint height, uint depth, PixelFormat format)
        {
            uint totalSize = FormatHelpers.GetRegionSize(width, height, depth, format);
            lock (_stagingResourcesLock)
            {
                for (int i = 0; i < _availableStagingTextures.Count; i++)
                {
                    VkTexture tex = _availableStagingTextures[i];
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
            VkTexture newTex = (VkTexture)ResourceFactory.CreateTexture(TextureDescription.Texture3D(
                texWidth, texHeight, depth, 1, format, TextureUsage.Staging));
            newTex.SetStagingDimensions(width, height, depth, format);

            return newTex;
        }

        private VkBuffer GetFreeStagingBuffer(uint size)
        {
            lock (_stagingResourcesLock)
            {
                for (int i = 0; i < _availableStagingBuffers.Count; i++)
                {
                    VkBuffer buffer = _availableStagingBuffers[i];
                    if (buffer.SizeInBytes >= size)
                    {
                        _availableStagingBuffers.RemoveAt(i);
                        return buffer;
                    }
                }
            }

            uint newBufferSize = Math.Max(MinStagingBufferSize, size);
            VkBuffer newBuffer = (VkBuffer)ResourceFactory.CreateBuffer(
                new BufferDescription(newBufferSize, BufferUsage.Staging));
            return newBuffer;
        }

        public override void ResetFence(Fence fence)
        {
            Vortice.Vulkan.VkFence vkFence = Util.AssertSubtype<Fence, VkFence>(fence).DeviceFence;
            vkResetFences(_device, 1, &vkFence);
        }

        public override bool WaitForFence(Fence fence, ulong nanosecondTimeout)
        {
            Vortice.Vulkan.VkFence vkFence = Util.AssertSubtype<Fence, VkFence>(fence).DeviceFence;
            VkResult result = vkWaitForFences(_device, 1, &vkFence, true, nanosecondTimeout);
            return result == VkResult.Success;
        }

        public override bool WaitForFences(Fence[] fences, bool waitAll, ulong nanosecondTimeout)
        {
            int fenceCount = fences.Length;
            Vortice.Vulkan.VkFence* fencesPtr = stackalloc Vortice.Vulkan.VkFence[fenceCount];
            for (int i = 0; i < fenceCount; i++)
            {
                fencesPtr[i] = Util.AssertSubtype<Fence, VkFence>(fences[i]).DeviceFence;
            }

            VkResult result = vkWaitForFences(_device, fenceCount, fencesPtr, waitAll, nanosecondTimeout);
            return result == VkResult.Success;
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
                apiVersion = new Vortice.Vulkan.VkVersion(1, 2, 0),
                applicationVersion = new Vortice.Vulkan.VkVersion(1, 0, 0),
                engineVersion = new Vortice.Vulkan.VkVersion(1, 0, 0),
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

        internal void ClearColorTexture(VkTexture texture, VkClearColorValue color)
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

        internal void ClearDepthTexture(VkTexture texture, VkClearDepthStencilValue clearValue)
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

        internal override uint GetUniformBufferMinOffsetAlignmentCore()
            => (uint)_physicalDeviceProperties.limits.minUniformBufferOffsetAlignment;

        internal override uint GetStructuredBufferMinOffsetAlignmentCore()
            => (uint)_physicalDeviceProperties.limits.minStorageBufferOffsetAlignment;

        internal void TransitionImageLayout(VkTexture texture, VkImageLayout layout)
        {
            SharedCommandPool pool = GetFreeCommandPool();
            VkCommandBuffer cb = pool.BeginNewCommandBuffer();
            texture.TransitionImageLayout(cb, 0, texture.MipLevels, 0, texture.ArrayLayers, layout);
            pool.EndAndSubmit(cb);
        }

        private class SharedCommandPool
        {
            private readonly VkGraphicsDevice _gd;
            private readonly VkCommandPool _pool;
            private readonly VkCommandBuffer _cb;

            public bool IsCached { get; }

            public SharedCommandPool(VkGraphicsDevice gd, bool isCached)
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
            public Vortice.Vulkan.VkFence Fence;
            public VkCommandList CommandList;
            public VkCommandBuffer CommandBuffer;
            public FenceSubmissionInfo(Vortice.Vulkan.VkFence fence, VkCommandList commandList, VkCommandBuffer commandBuffer)
            {
                Fence = fence;
                CommandList = commandList;
                CommandBuffer = commandBuffer;
            }
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
