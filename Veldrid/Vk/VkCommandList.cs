﻿using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Veldrid.Vk
{
    internal unsafe class VkCommandList : CommandList
    {
        private readonly VkGraphicsDevice _gd;
        private VkCommandPool _pool;
        private VkCommandBuffer _cb;
        private bool _destroyed;

        private bool _commandBufferBegun;
        private bool _commandBufferEnded;
        private VkRect2D[] _scissorRects = Array.Empty<VkRect2D>();

        private VkClearValue[] _clearValues = Array.Empty<VkClearValue>();
        private bool[] _validColorClearValues = Array.Empty<bool>();
        private VkClearValue? _depthClearValue;
        private readonly List<VkTexture> _preDrawSampledImages = new List<VkTexture>();

        // Graphics State
        private VkFramebufferBase _currentFramebuffer;
        private bool _currentFramebufferEverActive;
        private VkRenderPass _activeRenderPass;
        private VkPipeline _currentGraphicsPipeline;
        private BoundResourceSetInfo[] _currentGraphicsResourceSets = Array.Empty<BoundResourceSetInfo>();
        private bool[] _graphicsResourceSetsChanged;

        private bool _newFramebuffer; // Render pass cycle state

        // Compute State
        private VkPipeline _currentComputePipeline;
        private BoundResourceSetInfo[] _currentComputeResourceSets = Array.Empty<BoundResourceSetInfo>();
        private bool[] _computeResourceSetsChanged;
        private string _name;

        private readonly object _commandBufferListLock = new object();
        private readonly Queue<VkCommandBuffer> _availableCommandBuffers = new Queue<VkCommandBuffer>();
        private readonly List<VkCommandBuffer> _submittedCommandBuffers = new List<VkCommandBuffer>();

        private StagingResourceInfo _currentStagingInfo;
        private readonly object _stagingLock = new object();
        private readonly Dictionary<VkCommandBuffer, StagingResourceInfo> _submittedStagingInfos = new Dictionary<VkCommandBuffer, StagingResourceInfo>();
        private readonly List<StagingResourceInfo> _availableStagingInfos = new List<StagingResourceInfo>();
        private readonly List<VkBuffer> _availableStagingBuffers = new List<VkBuffer>();

        public VkCommandPool CommandPool => _pool;
        public VkCommandBuffer CommandBuffer => _cb;

        public bool IsTransfer => _isTransfer;

        public ResourceRefCount RefCount { get; }

        public VkCommandList(VkGraphicsDevice gd, ref CommandListDescription description)
            : base(ref description, gd.Features, gd.UniformBufferMinOffsetAlignment, gd.StructuredBufferMinOffsetAlignment)
        {
            _gd = gd;
            var poolCI = new VkCommandPoolCreateInfo
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
                queueFamilyIndex = description.IsTransfer ? gd.TransferQueueIndex : gd.GraphicsQueueIndex
            };
            VkResult result = vkCreateCommandPool(_gd.Device, &poolCI, null, out _pool);
            CheckResult(result);

            _cb = GetNextCommandBuffer();
            RefCount = new ResourceRefCount(DisposeCore);
        }

        private VkCommandBuffer GetNextCommandBuffer()
        {
            lock (_commandBufferListLock)
            {
                if (_availableCommandBuffers.Count > 0)
                {
                    VkCommandBuffer cachedCB = _availableCommandBuffers.Dequeue();
                    VkResult resetResult = vkResetCommandBuffer(cachedCB, VkCommandBufferResetFlags.None);
                    CheckResult(resetResult);
                    return cachedCB;
                }
            }

            var cbAI = new VkCommandBufferAllocateInfo
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = _pool,
                commandBufferCount = 1,
                level = VkCommandBufferLevel.Primary
            };
            VkCommandBuffer cb = new VkCommandBuffer();
            VkResult result = vkAllocateCommandBuffers(_gd.Device, &cbAI, &cb);
            CheckResult(result);
            return cb;
        }

        public void CommandBufferSubmitted(VkCommandBuffer cb)
        {
            RefCount.Increment();
            foreach (ResourceRefCount rrc in _currentStagingInfo.Resources)
            {
                rrc.Increment();
            }

            _submittedStagingInfos.Add(cb, _currentStagingInfo);
            _currentStagingInfo = null;
        }

        public void CommandBufferCompleted(VkCommandBuffer completedCB)
        {

            lock (_commandBufferListLock)
            {
                for (int i = 0; i < _submittedCommandBuffers.Count; i++)
                {
                    VkCommandBuffer submittedCB = _submittedCommandBuffers[i];
                    if (submittedCB == completedCB)
                    {
                        _availableCommandBuffers.Enqueue(completedCB);
                        _submittedCommandBuffers.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            lock (_stagingLock)
            {
                if (_submittedStagingInfos.TryGetValue(completedCB, out StagingResourceInfo info))
                {
                    RecycleStagingInfo(info);
                    _submittedStagingInfos.Remove(completedCB);
                }
            }

            RefCount.Decrement();
        }

        public override void Begin()
        {
            if (_commandBufferBegun)
            {
                throw new VeldridException(
                    "CommandList must be in its initial state, or End() must have been called, for Begin() to be valid to call.");
            }
            if (_commandBufferEnded)
            {
                _commandBufferEnded = false;
                _cb = GetNextCommandBuffer();
                if (_currentStagingInfo != null)
                {
                    RecycleStagingInfo(_currentStagingInfo);
                }
            }

            _currentStagingInfo = GetStagingResourceInfo();

            var beginInfo = new VkCommandBufferBeginInfo
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.OneTimeSubmit
            };
            vkBeginCommandBuffer(_cb, &beginInfo);
            _commandBufferBegun = true;

            ClearCachedState();
            _currentFramebuffer = null;
            _currentGraphicsPipeline = null;
            ClearSets(_currentGraphicsResourceSets);
            Util.ClearArray(_scissorRects);

            _currentComputePipeline = null;
            ClearSets(_currentComputeResourceSets);
        }

        private protected override void ClearColorTargetCore(uint index, RgbaFloat clearColor)
        {
            VkClearValue clearValue = new VkClearValue
            {
                color = new VkClearColorValue(clearColor.R, clearColor.G, clearColor.B, clearColor.A),
                //depthStencil = new VkClearDepthStencilValue(1.0f, 0)
            };

            if (_activeRenderPass != VkRenderPass.Null)
            {
                VkClearAttachment clearAttachment = new VkClearAttachment
                {
                    colorAttachment = index,
                    aspectMask = VkImageAspectFlags.Color,
                    clearValue = clearValue
                };

                Texture colorTex = _currentFramebuffer.ColorTargets[(int)index].Target;
                VkClearRect clearRect = new VkClearRect
                {
                    baseArrayLayer = 0,
                    layerCount = 1,
                    rect = new VkRect2D(0, 0, colorTex.Width, colorTex.Height)
                };

                vkCmdClearAttachments(_cb, 1, &clearAttachment, 1, &clearRect);
            }
            else
            {
                // Queue up the clear value for the next RenderPass.
                _clearValues[index] = clearValue;
                _validColorClearValues[index] = true;
            }
        }

        private protected override void ClearDepthStencilCore(float depth, byte stencil)
        {
            VkClearValue clearValue = new VkClearValue { depthStencil = new VkClearDepthStencilValue(depth, stencil) };

            if (_activeRenderPass != VkRenderPass.Null)
            {
                VkImageAspectFlags aspect = FormatHelpers.IsStencilFormat(_currentFramebuffer.DepthTarget.Value.Target.Format)
                    ? VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil
                    : VkImageAspectFlags.Depth;
                VkClearAttachment clearAttachment = new VkClearAttachment
                {
                    aspectMask = aspect,
                    clearValue = clearValue
                };

                uint renderableWidth = _currentFramebuffer.RenderableWidth;
                uint renderableHeight = _currentFramebuffer.RenderableHeight;
                if (renderableWidth > 0 && renderableHeight > 0)
                {
                    VkClearRect clearRect = new VkClearRect
                    {
                        baseArrayLayer = 0,
                        layerCount = 1,
                        rect = new VkRect2D(0, 0, renderableWidth, renderableHeight)
                    };

                    vkCmdClearAttachments(_cb, 1, &clearAttachment, 1, &clearRect);
                }
            }
            else
            {
                // Queue up the clear value for the next RenderPass.
                _depthClearValue = clearValue;
            }
        }

        private protected override void DrawCore(uint vertexCount, uint instanceCount, uint vertexStart, uint instanceStart)
        {
            PreDrawCommand();
            vkCmdDraw(_cb, (int)vertexCount, (int)instanceCount, vertexStart, instanceStart);
        }

        private protected override void DrawIndexedCore(uint indexCount, uint instanceCount, uint indexStart, int vertexOffset, uint instanceStart)
        {
            PreDrawCommand();
            vkCmdDrawIndexed(_cb, (int)indexCount, (int)instanceCount, indexStart, vertexOffset, instanceStart);
        }

        protected override void DrawIndirectCore(DeviceBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            PreDrawCommand();
            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(indirectBuffer);
            _currentStagingInfo.Resources.Add(vkBuffer.RefCount);
            vkCmdDrawIndirect(_cb, vkBuffer.DeviceBuffer, offset, (int)drawCount, stride);
        }

        protected override void DrawIndexedIndirectCore(DeviceBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            PreDrawCommand();
            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(indirectBuffer);
            _currentStagingInfo.Resources.Add(vkBuffer.RefCount);
            vkCmdDrawIndexedIndirect(_cb, vkBuffer.DeviceBuffer, offset, (int)drawCount, stride);
        }

        private void PreDrawCommand()
        {
            TransitionImages(_preDrawSampledImages, VkImageLayout.ShaderReadOnlyOptimal);
            _preDrawSampledImages.Clear();

            EnsureRenderPassActive();

            FlushNewResourceSets(
                _currentGraphicsResourceSets,
                _graphicsResourceSetsChanged,
                VkPipelineBindPoint.Graphics,
                _currentGraphicsPipeline.PipelineLayout);

            if (!_currentGraphicsPipeline.ScissorTestEnabled)
            {
                SetFullScissorRects();
            }
        }

        private void FlushNewResourceSets(
            BoundResourceSetInfo[] resourceSets,
            bool[] resourceSetsChanged,
            VkPipelineBindPoint bindPoint,
            VkPipelineLayout pipelineLayout)
        {
            VkPipeline pipeline = bindPoint == VkPipelineBindPoint.Graphics ? _currentGraphicsPipeline : _currentComputePipeline;

            int setCount = resourceSets.Length;
            VkDescriptorSet* descriptorSets = stackalloc VkDescriptorSet[setCount];
            uint* dynamicOffsets = stackalloc uint[pipeline.DynamicOffsetsCount];
            int currentBatchCount = 0;
            uint currentBatchFirstSet = 0;
            int currentBatchDynamicOffsetCount = 0;

            for (uint currentSlot = 0; currentSlot < resourceSets.Length; currentSlot++)
            {
                bool batchEnded = !resourceSetsChanged[currentSlot] || currentSlot == resourceSets.Length - 1;

                if (resourceSetsChanged[currentSlot])
                {
                    resourceSetsChanged[currentSlot] = false;
                    VkResourceSet vkSet = Util.AssertSubtype<ResourceSet, VkResourceSet>(resourceSets[currentSlot].Set);
                    descriptorSets[currentBatchCount] = vkSet.DescriptorSet;
                    currentBatchCount += 1;

                    SmallFixedOrDynamicArray curSetOffsets = resourceSets[currentSlot].Offsets;
                    for (uint i = 0; i < curSetOffsets.Count; i++)
                    {
                        dynamicOffsets[currentBatchDynamicOffsetCount] = curSetOffsets.Get(i);
                        currentBatchDynamicOffsetCount += 1;
                    }

                    // Increment ref count on first use of a set.
                    _currentStagingInfo.Resources.Add(vkSet.RefCount);
                    foreach (ResourceRefCount refCount in vkSet.RefCounts)
                    {
                        _currentStagingInfo.Resources.Add(refCount);
                    }
                }

                if (batchEnded)
                {
                    if (currentBatchCount != 0)
                    {
                        // Flush current batch.
                        vkCmdBindDescriptorSets(
                            _cb,
                            bindPoint,
                            pipelineLayout,
                            currentBatchFirstSet,
                            currentBatchCount,
                            descriptorSets,
                            currentBatchDynamicOffsetCount,
                            dynamicOffsets);
                    }

                    currentBatchCount = 0;
                    currentBatchFirstSet = currentSlot + 1;
                }
            }
        }

        private void TransitionImages(IReadOnlyList<VkTexture> sampledTextures, VkImageLayout layout)
        {
            for (int i = 0; i < sampledTextures.Count; i++)
            {
                VkTexture tex = sampledTextures[i];
                tex.TransitionImageLayout(_cb, 0, tex.MipLevels, 0, tex.ArrayLayers, layout);
            }
        }

        public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
        {
            PreDispatchCommand();

            vkCmdDispatch(_cb, groupCountX, groupCountY, groupCountZ);
        }

        private void PreDispatchCommand()
        {
            EnsureNoRenderPass();

            for (uint currentSlot = 0; currentSlot < _currentComputeResourceSets.Length; currentSlot++)
            {
                VkResourceSet vkSet = Util.AssertSubtype<ResourceSet, VkResourceSet>(
                    _currentComputeResourceSets[currentSlot].Set);
                TransitionImages(vkSet.SampledTextures, VkImageLayout.ShaderReadOnlyOptimal);
                TransitionImages(vkSet.StorageTextures, VkImageLayout.General);
                foreach (VkTexture storageTex in vkSet.StorageTextures)
                {
                    if ((storageTex.Usage & TextureUsage.Sampled) != 0)
                    {
                        _preDrawSampledImages.Add(storageTex);
                    }
                }
            }

            FlushNewResourceSets(
                _currentComputeResourceSets,
                _computeResourceSetsChanged,
                VkPipelineBindPoint.Compute,
                _currentComputePipeline.PipelineLayout);
        }

        protected override void DispatchIndirectCore(DeviceBuffer indirectBuffer, uint offset)
        {
            PreDispatchCommand();

            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(indirectBuffer);
            _currentStagingInfo.Resources.Add(vkBuffer.RefCount);
            vkCmdDispatchIndirect(_cb, vkBuffer.DeviceBuffer, offset);
        }

        protected override void ResolveTextureCore(Texture source, Texture destination)
        {
            if (_activeRenderPass != VkRenderPass.Null)
            {
                EndCurrentRenderPass();
            }

            VkTexture vkSource = Util.AssertSubtype<Texture, VkTexture>(source);
            _currentStagingInfo.Resources.Add(vkSource.RefCount);
            VkTexture vkDestination = Util.AssertSubtype<Texture, VkTexture>(destination);
            _currentStagingInfo.Resources.Add(vkDestination.RefCount);
            VkImageAspectFlags aspectFlags = ((source.Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil)
                ? VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil
                : VkImageAspectFlags.Color;
            VkImageResolve region = new VkImageResolve
            {
                extent = new VkExtent3D { width = source.Width, height = source.Height, depth = source.Depth },
                srcSubresource = new VkImageSubresourceLayers { layerCount = 1, aspectMask = aspectFlags },
                dstSubresource = new VkImageSubresourceLayers { layerCount = 1, aspectMask = aspectFlags }
            };

            vkSource.TransitionImageLayout(_cb, 0, 1, 0, 1, VkImageLayout.TransferSrcOptimal);
            vkDestination.TransitionImageLayout(_cb, 0, 1, 0, 1, VkImageLayout.TransferDstOptimal);

            vkCmdResolveImage(
                _cb,
                vkSource.OptimalDeviceImage,
                 VkImageLayout.TransferSrcOptimal,
                vkDestination.OptimalDeviceImage,
                VkImageLayout.TransferDstOptimal,
                1,
                &region);

            if ((vkDestination.Usage & TextureUsage.Sampled) != 0)
            {
                vkDestination.TransitionImageLayout(_cb, 0, 1, 0, 1, VkImageLayout.ShaderReadOnlyOptimal);
            }
        }

        public override void End()
        {
            if (!_commandBufferBegun)
            {
                throw new VeldridException("CommandBuffer must have been started before End() may be called.");
            }

            _commandBufferBegun = false;
            _commandBufferEnded = true;

            if (!_currentFramebufferEverActive && _currentFramebuffer != null)
            {
                BeginCurrentRenderPass();
            }
            if (_activeRenderPass != VkRenderPass.Null)
            {
                EndCurrentRenderPass();
                _currentFramebuffer.TransitionToFinalLayout(_cb);
            }

            vkEndCommandBuffer(_cb);
            _submittedCommandBuffers.Add(_cb);
        }

        protected override void SetFramebufferCore(Framebuffer fb)
        {
            if (_activeRenderPass.Handle != VkRenderPass.Null)
            {
                EndCurrentRenderPass();
            }
            else if (!_currentFramebufferEverActive && _currentFramebuffer != null)
            {
                // This forces any queued up texture clears to be emitted.
                BeginCurrentRenderPass();
                EndCurrentRenderPass();
            }

            if (_currentFramebuffer != null)
            {
                _currentFramebuffer.TransitionToFinalLayout(_cb);
            }

            VkFramebufferBase vkFB = Util.AssertSubtype<Framebuffer, VkFramebufferBase>(fb);
            _currentFramebuffer = vkFB;
            _currentFramebufferEverActive = false;
            _newFramebuffer = true;
            Util.EnsureArrayMinimumSize(ref _scissorRects, Math.Max(1, (uint)vkFB.ColorTargets.Count));
            uint clearValueCount = (uint)vkFB.ColorTargets.Count;
            Util.EnsureArrayMinimumSize(ref _clearValues, clearValueCount + 1); // Leave an extra space for the depth value (tracked separately).
            Util.ClearArray(_validColorClearValues);
            Util.EnsureArrayMinimumSize(ref _validColorClearValues, clearValueCount);
            _currentStagingInfo.Resources.Add(vkFB.RefCount);

            if (fb is VkSwapchainFramebuffer scFB)
            {
                _currentStagingInfo.Resources.Add(scFB.Swapchain.RefCount);
            }
        }

        private void EnsureRenderPassActive()
        {
            if (_activeRenderPass == VkRenderPass.Null)
            {
                BeginCurrentRenderPass();
            }
        }

        private void EnsureNoRenderPass()
        {
            if (_activeRenderPass != VkRenderPass.Null)
            {
                EndCurrentRenderPass();
            }
        }

        private void BeginCurrentRenderPass()
        {
            Debug.Assert(_activeRenderPass == VkRenderPass.Null);
            Debug.Assert(_currentFramebuffer != null);
            _currentFramebufferEverActive = true;

            uint attachmentCount = _currentFramebuffer.AttachmentCount;
            bool haveAnyAttachments = _currentFramebuffer.ColorTargets.Count > 0 || _currentFramebuffer.DepthTarget != null;
            bool haveAllClearValues = _depthClearValue.HasValue || _currentFramebuffer.DepthTarget == null;
            bool haveAnyClearValues = _depthClearValue.HasValue;
            for (int i = 0; i < _currentFramebuffer.ColorTargets.Count; i++)
            {
                if (!_validColorClearValues[i])
                {
                    haveAllClearValues = false;
                    haveAnyClearValues = true;
                }
                else
                {
                    haveAnyClearValues = true;
                }
            }

            var renderPassBI = new VkRenderPassBeginInfo
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderArea = new VkRect2D(_currentFramebuffer.RenderableWidth, _currentFramebuffer.RenderableHeight),
                framebuffer = _currentFramebuffer.CurrentFramebuffer
            };

            if (!haveAnyAttachments || !haveAllClearValues)
            {
                renderPassBI.renderPass = _newFramebuffer
                    ? _currentFramebuffer.RenderPassNoClear_Init
                    : _currentFramebuffer.RenderPassNoClear_Load;
                vkCmdBeginRenderPass(_cb, &renderPassBI, VkSubpassContents.Inline);
                _activeRenderPass = renderPassBI.renderPass;

                if (haveAnyClearValues)
                {
                    if (_depthClearValue.HasValue)
                    {
                        ClearDepthStencilCore(_depthClearValue.Value.depthStencil.depth, (byte)_depthClearValue.Value.depthStencil.stencil);
                        _depthClearValue = null;
                    }

                    for (uint i = 0; i < _currentFramebuffer.ColorTargets.Count; i++)
                    {
                        if (_validColorClearValues[i])
                        {
                            _validColorClearValues[i] = false;
                            VkClearValue vkClearValue = _clearValues[i];
                            RgbaFloat clearColor = new RgbaFloat(
                                vkClearValue.color.float32[0],
                                vkClearValue.color.float32[1],
                                vkClearValue.color.float32[2],
                                vkClearValue.color.float32[3]);
                            ClearColorTarget(i, clearColor);
                        }
                    }
                }
            }
            else
            {
                // We have clear values for every attachment.
                renderPassBI.renderPass = _currentFramebuffer.RenderPassClear;
                fixed (VkClearValue* clearValuesPtr = &_clearValues[0])
                {
                    renderPassBI.clearValueCount = attachmentCount;
                    renderPassBI.pClearValues = clearValuesPtr;
                    if (_depthClearValue.HasValue)
                    {
                        _clearValues[_currentFramebuffer.ColorTargets.Count] = _depthClearValue.Value;
                        _depthClearValue = null;
                    }
                    vkCmdBeginRenderPass(_cb, &renderPassBI, VkSubpassContents.Inline);
                    _activeRenderPass = _currentFramebuffer.RenderPassClear;
                    Util.ClearArray(_validColorClearValues);
                }
            }

            _newFramebuffer = false;
        }

        private void EndCurrentRenderPass()
        {
            Debug.Assert(_activeRenderPass != VkRenderPass.Null);
            vkCmdEndRenderPass(_cb);
            _currentFramebuffer.TransitionToIntermediateLayout(_cb);
            _activeRenderPass = VkRenderPass.Null;

            // Place a barrier between RenderPasses, so that color / depth outputs
            // can be read in subsequent passes.
            var barrier = new VkMemoryBarrier2
            {
                sType = VkStructureType.MemoryBarrier2,
                srcStageMask = VkPipelineStageFlags2.AllCommands,
                srcAccessMask = VkAccessFlags2.None,
                dstStageMask = VkPipelineStageFlags2.AllCommands,
                dstAccessMask = VkAccessFlags2.None
            };
            var dependencyInfo = new VkDependencyInfo
            {
                sType = VkStructureType.DependencyInfo,
                memoryBarrierCount = 1,
                pMemoryBarriers = &barrier
            };
            vkCmdPipelineBarrier2(_cb, &dependencyInfo);
        }

        private protected override void SetVertexBufferCore(uint index, DeviceBuffer buffer, uint offset)
        {
            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(buffer);
            Vortice.Vulkan.VkBuffer deviceBuffer = vkBuffer.DeviceBuffer;
            ulong offset64 = offset;
            vkCmdBindVertexBuffers(_cb, index, 1, &deviceBuffer, &offset64);
            _currentStagingInfo.Resources.Add(vkBuffer.RefCount);
        }

        private protected override void SetIndexBufferCore(DeviceBuffer buffer, IndexFormat format, uint offset)
        {
            VkBuffer vkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(buffer);
            vkCmdBindIndexBuffer(_cb, vkBuffer.DeviceBuffer, offset, VkFormats.VdToVkIndexFormat(format));
            _currentStagingInfo.Resources.Add(vkBuffer.RefCount);
        }

        private protected override void SetPipelineCore(Pipeline pipeline)
        {
            VkPipeline vkPipeline = Util.AssertSubtype<Pipeline, VkPipeline>(pipeline);
            if (!pipeline.IsComputePipeline && _currentGraphicsPipeline != pipeline)
            {
                Util.EnsureArrayMinimumSize(ref _currentGraphicsResourceSets, vkPipeline.ResourceSetCount);
                ClearSets(_currentGraphicsResourceSets);
                Util.EnsureArrayMinimumSize(ref _graphicsResourceSetsChanged, vkPipeline.ResourceSetCount);
                Util.ClearArray(_graphicsResourceSetsChanged);
                vkCmdBindPipeline(_cb, VkPipelineBindPoint.Graphics, vkPipeline.DevicePipeline);
                _currentGraphicsPipeline = vkPipeline;
            }
            else if (pipeline.IsComputePipeline && _currentComputePipeline != pipeline)
            {
                Util.EnsureArrayMinimumSize(ref _currentComputeResourceSets, vkPipeline.ResourceSetCount);
                ClearSets(_currentComputeResourceSets);
                Util.EnsureArrayMinimumSize(ref _computeResourceSetsChanged, vkPipeline.ResourceSetCount);
                vkCmdBindPipeline(_cb, VkPipelineBindPoint.Compute, vkPipeline.DevicePipeline);
                _currentComputePipeline = vkPipeline;
            }

            _currentStagingInfo.Resources.Add(vkPipeline.RefCount);
        }

        private void ClearSets(BoundResourceSetInfo[] boundSets)
        {
            foreach (BoundResourceSetInfo boundSetInfo in boundSets)
            {
                boundSetInfo.Offsets.Dispose();
            }
            Util.ClearArray(boundSets);
        }

        protected override void SetGraphicsResourceSetCore(uint slot, ResourceSet rs, uint dynamicOffsetsCount, ref uint dynamicOffsets)
        {
            if (!_currentGraphicsResourceSets[slot].Equals(rs, dynamicOffsetsCount, ref dynamicOffsets))
            {
                _currentGraphicsResourceSets[slot].Offsets.Dispose();
                _currentGraphicsResourceSets[slot] = new BoundResourceSetInfo(rs, dynamicOffsetsCount, ref dynamicOffsets);
                _graphicsResourceSetsChanged[slot] = true;
                VkResourceSet vkRS = Util.AssertSubtype<ResourceSet, VkResourceSet>(rs);
            }
        }

        protected override void SetComputeResourceSetCore(uint slot, ResourceSet rs, uint dynamicOffsetsCount, ref uint dynamicOffsets)
        {
            if (!_currentComputeResourceSets[slot].Equals(rs, dynamicOffsetsCount, ref dynamicOffsets))
            {
                _currentComputeResourceSets[slot].Offsets.Dispose();
                _currentComputeResourceSets[slot] = new BoundResourceSetInfo(rs, dynamicOffsetsCount, ref dynamicOffsets);
                _computeResourceSetsChanged[slot] = true;
                VkResourceSet vkRS = Util.AssertSubtype<ResourceSet, VkResourceSet>(rs);
            }
        }

        public override void SetScissorRect(uint index, uint x, uint y, uint width, uint height)
        {
            if (index == 0 || _gd.Features.MultipleViewports)
            {
                VkRect2D scissor = new VkRect2D((int)x, (int)y, (int)width, (int)height);
                if (_scissorRects[index] != scissor)
                {
                    _scissorRects[index] = scissor;
                    vkCmdSetScissor(_cb, index, 1, &scissor);
                }
            }
        }

        public override void SetViewport(uint index, ref Viewport viewport)
        {
            if (index == 0 || _gd.Features.MultipleViewports)
            {
                float vpY = _gd.IsClipSpaceYInverted
                    ? viewport.Y
                    : viewport.Height + viewport.Y;
                float vpHeight = _gd.IsClipSpaceYInverted
                    ? viewport.Height
                    : -viewport.Height;

                VkViewport vkViewport = new VkViewport
                {
                    x = viewport.X,
                    y = vpY,
                    width = viewport.Width,
                    height = vpHeight,
                    minDepth = viewport.MinDepth,
                    maxDepth = viewport.MaxDepth
                };

                vkCmdSetViewport(_cb, index, 1, &vkViewport);
            }
        }

        private protected override void UpdateBufferCore(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes)
        {
            VkBuffer stagingBuffer = GetStagingBuffer(sizeInBytes);
            _gd.UpdateBuffer(stagingBuffer, 0, source, sizeInBytes);
            CopyBuffer(stagingBuffer, 0, buffer, bufferOffsetInBytes, sizeInBytes);
        }

        protected override void CopyBufferCore(
            DeviceBuffer source,
            uint sourceOffset,
            DeviceBuffer destination,
            uint destinationOffset,
            uint sizeInBytes)
        {
            EnsureNoRenderPass();

            VkBuffer srcVkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(source);
            _currentStagingInfo.Resources.Add(srcVkBuffer.RefCount);
            VkBuffer dstVkBuffer = Util.AssertSubtype<DeviceBuffer, VkBuffer>(destination);
            _currentStagingInfo.Resources.Add(dstVkBuffer.RefCount);

            VkBufferCopy region = new VkBufferCopy
            {
                srcOffset = sourceOffset,
                dstOffset = destinationOffset,
                size = sizeInBytes
            };

            VkMemoryBarrier2 barrier;
            VkBufferMemoryBarrier2 bbarrier;
            VkDependencyInfo dependencyInfo;

            // If we're doing a readback, make sure memory is host visible
            if (destination.Usage.HasFlag(BufferUsage.Staging))
            {
                bbarrier = new VkBufferMemoryBarrier2
                {
                    sType = VkStructureType.BufferMemoryBarrier2,
                    srcStageMask = VkPipelineStageFlags2.AllGraphics,
                    srcAccessMask = VkAccessFlags2.MemoryWrite | VkAccessFlags2.ShaderWrite,
                    dstStageMask = VkPipelineStageFlags2.Transfer,
                    dstAccessMask = VkAccessFlags2.TransferRead,
                    srcQueueFamilyIndex = 0,
                    dstQueueFamilyIndex = 0,
                    buffer = srcVkBuffer.DeviceBuffer,
                    offset = 0,
                    size = source.SizeInBytes
                };
                dependencyInfo = new VkDependencyInfo
                {
                    sType = VkStructureType.DependencyInfo,
                    dependencyFlags = VkDependencyFlags.None,
                    bufferMemoryBarrierCount = 1,
                    pBufferMemoryBarriers = &bbarrier,
                };
                vkCmdPipelineBarrier2(_cb, &dependencyInfo);
            }

            vkCmdCopyBuffer(_cb, srcVkBuffer.DeviceBuffer, dstVkBuffer.DeviceBuffer, 1, &region);

            if (destination.Usage.HasFlag(BufferUsage.Staging))
            {
                bbarrier = new VkBufferMemoryBarrier2
                {
                    sType = VkStructureType.BufferMemoryBarrier2,
                    srcStageMask = VkPipelineStageFlags2.Transfer,
                    srcAccessMask = VkAccessFlags2.TransferWrite,
                    dstStageMask = VkPipelineStageFlags2.Host,
                    dstAccessMask = VkAccessFlags2.HostRead,
                    srcQueueFamilyIndex = 0,
                    dstQueueFamilyIndex = 0,
                    buffer = dstVkBuffer.DeviceBuffer,
                    offset = 0,
                    size = source.SizeInBytes,
                };
                dependencyInfo = new VkDependencyInfo
                {
                    sType = VkStructureType.DependencyInfo,
                    dependencyFlags = VkDependencyFlags.None,
                    bufferMemoryBarrierCount = 1,
                    pBufferMemoryBarriers = &bbarrier,
                };
                vkCmdPipelineBarrier2(_cb, &dependencyInfo);
            }
            else if (!IsTransfer)
            {
                if (destination.Usage.HasFlag(BufferUsage.VertexBuffer))
                {
                    barrier = new VkMemoryBarrier2
                    {
                        sType = VkStructureType.MemoryBarrier2,
                        srcStageMask = VkPipelineStageFlags2.Transfer,
                        srcAccessMask = VkAccessFlags2.TransferWrite,
                        dstStageMask = VkPipelineStageFlags2.VertexInput,
                        dstAccessMask = VkAccessFlags2.VertexAttributeRead,
                    };
                    dependencyInfo = new VkDependencyInfo
                    {
                        sType = VkStructureType.DependencyInfo,
                        dependencyFlags = VkDependencyFlags.None,
                        memoryBarrierCount = 1,
                        pMemoryBarriers = &barrier,
                    };
                    vkCmdPipelineBarrier2(_cb, &dependencyInfo);
                }
                else
                {
                    barrier = new VkMemoryBarrier2
                    {
                        sType = VkStructureType.MemoryBarrier2,
                        srcStageMask = VkPipelineStageFlags2.Transfer,
                        srcAccessMask = VkAccessFlags2.TransferWrite,
                        dstStageMask = VkPipelineStageFlags2.DrawIndirect,
                        //dstAccessMask = VkAccessFlags2.VertexAttributeRead;
                        dstAccessMask = VkAccessFlags2.IndirectCommandRead,
                    };
                    dependencyInfo = new VkDependencyInfo
                    {
                        sType = VkStructureType.DependencyInfo,
                        dependencyFlags = VkDependencyFlags.None,
                        memoryBarrierCount = 1,
                        pMemoryBarriers = &barrier,
                    };
                    vkCmdPipelineBarrier2(_cb, &dependencyInfo);
                }
            }
        }

        protected override void CopyTextureCore(
            Texture source,
            uint srcX, uint srcY, uint srcZ,
            uint srcMipLevel,
            uint srcBaseArrayLayer,
            Texture destination,
            uint dstX, uint dstY, uint dstZ,
            uint dstMipLevel,
            uint dstBaseArrayLayer,
            uint width, uint height, uint depth,
            uint layerCount)
        {
            EnsureNoRenderPass();
            CopyTextureCore_VkCommandBuffer(
                _cb,
                source, srcX, srcY, srcZ, srcMipLevel, srcBaseArrayLayer,
                destination, dstX, dstY, dstZ, dstMipLevel, dstBaseArrayLayer,
                width, height, depth, layerCount);

            VkTexture srcVkTexture = Util.AssertSubtype<Texture, VkTexture>(source);
            _currentStagingInfo.Resources.Add(srcVkTexture.RefCount);
            VkTexture dstVkTexture = Util.AssertSubtype<Texture, VkTexture>(destination);
            _currentStagingInfo.Resources.Add(dstVkTexture.RefCount);
        }

        internal static void CopyTextureCore_VkCommandBuffer(
            VkCommandBuffer cb,
            Texture source,
            uint srcX, uint srcY, uint srcZ,
            uint srcMipLevel,
            uint srcBaseArrayLayer,
            Texture destination,
            uint dstX, uint dstY, uint dstZ,
            uint dstMipLevel,
            uint dstBaseArrayLayer,
            uint width, uint height, uint depth,
            uint layerCount)
        {
            VkTexture srcVkTexture = Util.AssertSubtype<Texture, VkTexture>(source);
            VkTexture dstVkTexture = Util.AssertSubtype<Texture, VkTexture>(destination);

            bool sourceIsStaging = (source.Usage & TextureUsage.Staging) == TextureUsage.Staging;
            bool destIsStaging = (destination.Usage & TextureUsage.Staging) == TextureUsage.Staging;

            if (!sourceIsStaging && !destIsStaging)
            {
                VkImageSubresourceLayers srcSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    layerCount = layerCount,
                    mipLevel = srcMipLevel,
                    baseArrayLayer = srcBaseArrayLayer
                };

                VkImageSubresourceLayers dstSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    layerCount = layerCount,
                    mipLevel = dstMipLevel,
                    baseArrayLayer = dstBaseArrayLayer
                };

                VkImageCopy region = new VkImageCopy
                {
                    srcOffset = new VkOffset3D { x = (int)srcX, y = (int)srcY, z = (int)srcZ },
                    dstOffset = new VkOffset3D { x = (int)dstX, y = (int)dstY, z = (int)dstZ },
                    srcSubresource = srcSubresource,
                    dstSubresource = dstSubresource,
                    extent = new VkExtent3D { width = width, height = height, depth = depth }
                };

                srcVkTexture.TransitionImageLayout(
                    cb,
                    srcMipLevel,
                    1,
                    srcBaseArrayLayer,
                    layerCount,
                    VkImageLayout.TransferSrcOptimal);

                dstVkTexture.TransitionImageLayout(
                    cb,
                    dstMipLevel,
                    1,
                    dstBaseArrayLayer,
                    layerCount,
                    VkImageLayout.TransferDstOptimal);

                vkCmdCopyImage(
                    cb,
                    srcVkTexture.OptimalDeviceImage,
                    VkImageLayout.TransferSrcOptimal,
                    dstVkTexture.OptimalDeviceImage,
                    VkImageLayout.TransferDstOptimal,
                    1,
                    &region);

                if ((srcVkTexture.Usage & TextureUsage.Sampled) != 0)
                {
                    srcVkTexture.TransitionImageLayout(
                        cb,
                        srcMipLevel,
                        1,
                        srcBaseArrayLayer,
                        layerCount,
                        VkImageLayout.ShaderReadOnlyOptimal);
                }

                if ((dstVkTexture.Usage & TextureUsage.Sampled) != 0)
                {
                    dstVkTexture.TransitionImageLayout(
                        cb,
                        dstMipLevel,
                        1,
                        dstBaseArrayLayer,
                        layerCount,
                        VkImageLayout.ShaderReadOnlyOptimal);
                }
            }
            else if (sourceIsStaging && !destIsStaging)
            {
                Vortice.Vulkan.VkBuffer srcBuffer = srcVkTexture.StagingBuffer;
                VkSubresourceLayout srcLayout = srcVkTexture.GetSubresourceLayout(
                    srcVkTexture.CalculateSubresource(srcMipLevel, srcBaseArrayLayer));
                VkImage dstImage = dstVkTexture.OptimalDeviceImage;
                dstVkTexture.TransitionImageLayout(
                    cb,
                    dstMipLevel,
                    1,
                    dstBaseArrayLayer,
                    layerCount,
                    VkImageLayout.TransferDstOptimal);

                VkImageSubresourceLayers dstSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    layerCount = layerCount,
                    mipLevel = dstMipLevel,
                    baseArrayLayer = dstBaseArrayLayer
                };

                Util.GetMipDimensions(srcVkTexture, srcMipLevel, out uint mipWidth, out uint mipHeight, out uint mipDepth);
                uint blockSize = FormatHelpers.IsCompressedFormat(srcVkTexture.Format) ? 4u : 1u;
                uint bufferRowLength = Math.Max(mipWidth, blockSize);
                uint bufferImageHeight = Math.Max(mipHeight, blockSize);
                uint compressedX = srcX / blockSize;
                uint compressedY = srcY / blockSize;
                uint blockSizeInBytes = blockSize == 1
                    ? FormatHelpers.GetSizeInBytes(srcVkTexture.Format)
                    : FormatHelpers.GetBlockSizeInBytes(srcVkTexture.Format);
                uint rowPitch = FormatHelpers.GetRowPitch(bufferRowLength, srcVkTexture.Format);
                uint depthPitch = FormatHelpers.GetDepthPitch(rowPitch, bufferImageHeight, srcVkTexture.Format);

                VkBufferImageCopy regions = new VkBufferImageCopy
                {
                    bufferOffset = srcLayout.offset
                        + (srcZ * depthPitch)
                        + (compressedY * rowPitch)
                        + (compressedX * blockSizeInBytes),
                    bufferRowLength = bufferRowLength,
                    bufferImageHeight = bufferImageHeight,
                    imageExtent = new VkExtent3D { width = width, height = height, depth = depth },
                    imageOffset = new VkOffset3D { x = (int)dstX, y = (int)dstY, z = (int)dstZ },
                    imageSubresource = dstSubresource
                };

                vkCmdCopyBufferToImage(cb, srcBuffer, dstImage, VkImageLayout.TransferDstOptimal, 1, &regions);

                if ((dstVkTexture.Usage & TextureUsage.Sampled) != 0)
                {
                    dstVkTexture.TransitionImageLayout(
                        cb,
                        dstMipLevel,
                        1,
                        dstBaseArrayLayer,
                        layerCount,
                        VkImageLayout.ShaderReadOnlyOptimal);
                }
            }
            else if (!sourceIsStaging && destIsStaging)
            {
                VkImage srcImage = srcVkTexture.OptimalDeviceImage;
                srcVkTexture.TransitionImageLayout(
                    cb,
                    srcMipLevel,
                    1,
                    srcBaseArrayLayer,
                    layerCount,
                    VkImageLayout.TransferSrcOptimal);

                Vortice.Vulkan.VkBuffer dstBuffer = dstVkTexture.StagingBuffer;
                VkSubresourceLayout dstLayout = dstVkTexture.GetSubresourceLayout(
                    dstVkTexture.CalculateSubresource(dstMipLevel, dstBaseArrayLayer));
                VkImageSubresourceLayers srcSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    layerCount = layerCount,
                    mipLevel = srcMipLevel,
                    baseArrayLayer = srcBaseArrayLayer
                };

                Util.GetMipDimensions(dstVkTexture, dstMipLevel, out uint mipWidth, out uint mipHeight, out uint mipDepth);
                uint blockSize = FormatHelpers.IsCompressedFormat(srcVkTexture.Format) ? 4u : 1u;
                uint bufferRowLength = Math.Max(mipWidth, blockSize);
                uint bufferImageHeight = Math.Max(mipHeight, blockSize);
                uint compressedDstX = dstX / blockSize;
                uint compressedDstY = dstY / blockSize;
                uint blockSizeInBytes = blockSize == 1
                    ? FormatHelpers.GetSizeInBytes(dstVkTexture.Format)
                    : FormatHelpers.GetBlockSizeInBytes(dstVkTexture.Format);
                uint rowPitch = FormatHelpers.GetRowPitch(bufferRowLength, dstVkTexture.Format);
                uint depthPitch = FormatHelpers.GetDepthPitch(rowPitch, bufferImageHeight, dstVkTexture.Format);

                VkBufferImageCopy region = new VkBufferImageCopy
                {
                    bufferRowLength = mipWidth,
                    bufferImageHeight = mipHeight,
                    bufferOffset = dstLayout.offset
                        + (dstZ * depthPitch)
                        + (compressedDstY * rowPitch)
                        + (compressedDstX * blockSizeInBytes),
                    imageExtent = new VkExtent3D { width = width, height = height, depth = depth },
                    imageOffset = new VkOffset3D { x = (int)srcX, y = (int)srcY, z = (int)srcZ },
                    imageSubresource = srcSubresource
                };

                vkCmdCopyImageToBuffer(cb, srcImage, VkImageLayout.TransferSrcOptimal, dstBuffer, 1, &region);

                if ((srcVkTexture.Usage & TextureUsage.Sampled) != 0)
                {
                    srcVkTexture.TransitionImageLayout(
                        cb,
                        srcMipLevel,
                        1,
                        srcBaseArrayLayer,
                        layerCount,
                        VkImageLayout.ShaderReadOnlyOptimal);
                }
            }
            else
            {
                Debug.Assert(sourceIsStaging && destIsStaging);
                Vortice.Vulkan.VkBuffer srcBuffer = srcVkTexture.StagingBuffer;
                VkSubresourceLayout srcLayout = srcVkTexture.GetSubresourceLayout(
                    srcVkTexture.CalculateSubresource(srcMipLevel, srcBaseArrayLayer));
                Vortice.Vulkan.VkBuffer dstBuffer = dstVkTexture.StagingBuffer;
                VkSubresourceLayout dstLayout = dstVkTexture.GetSubresourceLayout(
                    dstVkTexture.CalculateSubresource(dstMipLevel, dstBaseArrayLayer));

                uint zLimit = Math.Max(depth, layerCount);
                if (!FormatHelpers.IsCompressedFormat(source.Format))
                {
                    uint pixelSize = FormatHelpers.GetSizeInBytes(srcVkTexture.Format);
                    for (uint zz = 0; zz < zLimit; zz++)
                    {
                        for (uint yy = 0; yy < height; yy++)
                        {
                            VkBufferCopy region = new VkBufferCopy
                            {
                                srcOffset = srcLayout.offset
                                    + srcLayout.depthPitch * (zz + srcZ)
                                    + srcLayout.rowPitch * (yy + srcY)
                                    + pixelSize * srcX,
                                dstOffset = dstLayout.offset
                                    + dstLayout.depthPitch * (zz + dstZ)
                                    + dstLayout.rowPitch * (yy + dstY)
                                    + pixelSize * dstX,
                                size = width * pixelSize,
                            };

                            vkCmdCopyBuffer(cb, srcBuffer, dstBuffer, 1, &region);
                        }
                    }
                }
                else // IsCompressedFormat
                {
                    uint denseRowSize = FormatHelpers.GetRowPitch(width, source.Format);
                    uint numRows = FormatHelpers.GetNumRows(height, source.Format);
                    uint compressedSrcX = srcX / 4;
                    uint compressedSrcY = srcY / 4;
                    uint compressedDstX = dstX / 4;
                    uint compressedDstY = dstY / 4;
                    uint blockSizeInBytes = FormatHelpers.GetBlockSizeInBytes(source.Format);

                    for (uint zz = 0; zz < zLimit; zz++)
                    {
                        for (uint row = 0; row < numRows; row++)
                        {
                            VkBufferCopy region = new VkBufferCopy
                            {
                                srcOffset = srcLayout.offset
                                    + srcLayout.depthPitch * (zz + srcZ)
                                    + srcLayout.rowPitch * (row + compressedSrcY)
                                    + blockSizeInBytes * compressedSrcX,
                                dstOffset = dstLayout.offset
                                    + dstLayout.depthPitch * (zz + dstZ)
                                    + dstLayout.rowPitch * (row + compressedDstY)
                                    + blockSizeInBytes * compressedDstX,
                                size = denseRowSize,
                            };

                            vkCmdCopyBuffer(cb, srcBuffer, dstBuffer, 1, &region);
                        }
                    }

                }
            }
        }

        private protected override void GenerateMipmapsCore(Texture texture)
        {
            EnsureNoRenderPass();
            VkTexture vkTex = Util.AssertSubtype<Texture, VkTexture>(texture);
            _currentStagingInfo.Resources.Add(vkTex.RefCount);
            vkTex.TransitionImageLayout(_cb, 0, 1, 0, vkTex.ArrayLayers, VkImageLayout.TransferSrcOptimal);
            vkTex.TransitionImageLayout(_cb, 1, vkTex.MipLevels - 1, 0, vkTex.ArrayLayers, VkImageLayout.TransferDstOptimal);

            VkImage deviceImage = vkTex.OptimalDeviceImage;

            int blitCount = (int)vkTex.MipLevels - 1;
            VkImageBlit* regions = stackalloc VkImageBlit[blitCount];

            for (uint level = 1; level < vkTex.MipLevels; level++)
            {
                uint blitIndex = level - 1;

                regions[blitIndex].srcSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    baseArrayLayer = 0,
                    layerCount = vkTex.ArrayLayers,
                    mipLevel = 0
                };
                regions[blitIndex].srcOffsets[0] = new VkOffset3D();
                regions[blitIndex].srcOffsets[1] = new VkOffset3D { x = (int)vkTex.Width, y = (int)vkTex.Height, z = (int)vkTex.Depth };
                regions[blitIndex].dstOffsets[0] = new VkOffset3D();

                regions[blitIndex].dstSubresource = new VkImageSubresourceLayers
                {
                    aspectMask = VkImageAspectFlags.Color,
                    baseArrayLayer = 0,
                    layerCount = vkTex.ArrayLayers,
                    mipLevel = level
                };

                Util.GetMipDimensions(vkTex, level, out uint mipWidth, out uint mipHeight, out uint mipDepth);
                regions[blitIndex].dstOffsets[1] = new VkOffset3D { x = (int)mipWidth, y = (int)mipHeight, z = (int)mipDepth };
            }

            vkCmdBlitImage(
                _cb,
                deviceImage, VkImageLayout.TransferSrcOptimal,
                deviceImage, VkImageLayout.TransferDstOptimal,
                blitCount, regions,
                _gd.GetFormatFilter(vkTex.VkFormat));

            if ((vkTex.Usage & TextureUsage.Sampled) != 0)
            {
                // This is somewhat ugly -- the transition logic does not handle different source layouts, so we do two batches.
                vkTex.TransitionImageLayout(_cb, 0, 1, 0, vkTex.ArrayLayers, VkImageLayout.ShaderReadOnlyOptimal);
                vkTex.TransitionImageLayout(_cb, 1, vkTex.MipLevels - 1, 0, vkTex.ArrayLayers, VkImageLayout.ShaderReadOnlyOptimal);
            }
        }

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                _gd.SetResourceName(this, value);
            }
        }

        private VkBuffer GetStagingBuffer(uint size)
        {
            lock (_stagingLock)
            {
                VkBuffer ret = null;
                foreach (VkBuffer buffer in _availableStagingBuffers)
                {
                    if (buffer.SizeInBytes >= size)
                    {
                        ret = buffer;
                        _availableStagingBuffers.Remove(buffer);
                        break; ;
                    }
                }
                if (ret == null)
                {
                    ret = (VkBuffer)_gd.ResourceFactory.CreateBuffer(new BufferDescription(size, BufferUsage.Staging));
                    ret.Name = $"Staging Buffer (CommandList {_name})";
                }

                _currentStagingInfo.BuffersUsed.Add(ret);
                return ret;
            }
        }

        private protected override void PushDebugGroupCore(string name)
        {
            vkCmdDebugMarkerBeginEXT_t func = _gd.MarkerBegin;
            if (func == null) { return; }

            int byteCount = Encoding.UTF8.GetByteCount(name);
            sbyte* utf8Ptr = stackalloc sbyte[byteCount + 1];
            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, (byte*)utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            var markerInfo = new VkDebugMarkerMarkerInfoEXT
            {
                sType = VkStructureType.DebugMarkerMarkerInfoEXT,
                pMarkerName = utf8Ptr
            };

            func(_cb, &markerInfo);
        }

        private protected override void PopDebugGroupCore()
        {
            vkCmdDebugMarkerEndEXT_t func = _gd.MarkerEnd;
            if (func == null) { return; }

            func(_cb);
        }

        private protected override void InsertDebugMarkerCore(string name)
        {
            vkCmdDebugMarkerInsertEXT_t func = _gd.MarkerInsert;
            if (func == null) { return; }

            int byteCount = Encoding.UTF8.GetByteCount(name);
            sbyte* utf8Ptr = stackalloc sbyte[byteCount + 1];
            fixed (char* namePtr = name)
            {
                Encoding.UTF8.GetBytes(namePtr, name.Length, (byte*)utf8Ptr, byteCount);
            }
            utf8Ptr[byteCount] = 0;

            VkDebugMarkerMarkerInfoEXT markerInfo = new VkDebugMarkerMarkerInfoEXT
            {
                sType = VkStructureType.DebugMarkerMarkerInfoEXT,
                pMarkerName = utf8Ptr
            };

            func(_cb, &markerInfo);
        }

        public override void Dispose()
        {
            RefCount.Decrement();
        }

        private void DisposeCore()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                vkDestroyCommandPool(_gd.Device, _pool, null);

                Debug.Assert(_submittedStagingInfos.Count == 0);

                foreach (VkBuffer buffer in _availableStagingBuffers)
                {
                    buffer.Dispose();
                }
            }
        }

        private class StagingResourceInfo
        {
            public List<VkBuffer> BuffersUsed { get; } = new List<VkBuffer>();
            public HashSet<ResourceRefCount> Resources { get; } = new HashSet<ResourceRefCount>();
            public void Clear()
            {
                BuffersUsed.Clear();
                Resources.Clear();
            }
        }

        private StagingResourceInfo GetStagingResourceInfo()
        {
            lock (_stagingLock)
            {
                StagingResourceInfo ret;
                int availableCount = _availableStagingInfos.Count;
                if (availableCount > 0)
                {
                    ret = _availableStagingInfos[availableCount - 1];
                    _availableStagingInfos.RemoveAt(availableCount - 1);
                }
                else
                {
                    ret = new StagingResourceInfo();
                }

                return ret;
            }
        }

        private void RecycleStagingInfo(StagingResourceInfo info)
        {
            lock (_stagingLock)
            {
                foreach (VkBuffer buffer in info.BuffersUsed)
                {
                    _availableStagingBuffers.Add(buffer);
                }

                foreach (ResourceRefCount rrc in info.Resources)
                {
                    rrc.Decrement();
                }

                info.Clear();

                _availableStagingInfos.Add(info);
            }
        }
    }
}
