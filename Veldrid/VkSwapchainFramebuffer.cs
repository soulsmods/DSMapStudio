﻿using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.VulkanUtil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Veldrid
{
    internal unsafe class VkSwapchainFramebuffer : VkFramebufferBase
    {
        private readonly GraphicsDevice _gd;
        private readonly Swapchain _swapchain;
        private readonly VkSurfaceKHR _surface;
        private readonly VkFormat? _depthFormat;
        private uint _currentImageIndex;

        private VkFramebuffer[] _scFramebuffers;
        private VkImage[] _scImages;
        private VkFormat _scImageFormat;
        private VkExtent2D _scExtent;
        private FramebufferAttachment[][] _scColorTextures;

        private FramebufferAttachment? _depthAttachment;
        private uint _desiredWidth;
        private uint _desiredHeight;
        private bool _destroyed;
        private string _name;
        private OutputDescription _outputDescription;

        public override Vortice.Vulkan.VkFramebuffer CurrentFramebuffer => _scFramebuffers[(int)_currentImageIndex].CurrentFramebuffer;

        public override VkRenderPass RenderPassNoClear_Init => _scFramebuffers[0].RenderPassNoClear_Init;
        public override VkRenderPass RenderPassNoClear_Load => _scFramebuffers[0].RenderPassNoClear_Load;
        public override VkRenderPass RenderPassClear => _scFramebuffers[0].RenderPassClear;

        public override IReadOnlyList<FramebufferAttachment> ColorTargets => _scColorTextures[(int)_currentImageIndex];

        public override FramebufferAttachment? DepthTarget => _depthAttachment;

        public override uint RenderableWidth => _scExtent.width;
        public override uint RenderableHeight => _scExtent.height;

        public override uint Width => _desiredWidth;
        public override uint Height => _desiredHeight;

        public uint ImageIndex => _currentImageIndex;

        public override OutputDescription OutputDescription => _outputDescription;

        public override uint AttachmentCount { get; }

        public Swapchain Swapchain => _swapchain;

        public VkSwapchainFramebuffer(
            GraphicsDevice gd,
            Swapchain swapchain,
            VkSurfaceKHR surface,
            uint width,
            uint height,
            VkFormat? depthFormat)
            : base()
        {
            _gd = gd;
            _swapchain = swapchain;
            _surface = surface;
            _depthFormat = depthFormat;

            AttachmentCount = depthFormat.HasValue ? 2u : 1u; // 1 Color + 1 Depth
        }

        internal void SetImageIndex(uint index)
        {
            _currentImageIndex = index;
        }

        internal void SetNewSwapchain(
            VkSwapchainKHR deviceSwapchain,
            uint width,
            uint height,
            VkSurfaceFormatKHR surfaceFormat,
            VkExtent2D swapchainExtent)
        {
            _desiredWidth = width;
            _desiredHeight = height;

            // Get the images
            int scImageCount = 0;
            VkResult result = vkGetSwapchainImagesKHR(_gd.Device, deviceSwapchain, &scImageCount, null);
            CheckResult(result);
            if (_scImages == null)
            {
                _scImages = new VkImage[(int)scImageCount];
            }
            fixed (VkImage* pscImages = _scImages)
            result = vkGetSwapchainImagesKHR(_gd.Device, deviceSwapchain, &scImageCount, pscImages);
            CheckResult(result);

            _scImageFormat = surfaceFormat.format;
            _scExtent = swapchainExtent;

            CreateDepthTexture();
            CreateFramebuffers();

            _outputDescription = OutputDescription.CreateFromFramebuffer(this);
        }

        private void DestroySwapchainFramebuffers()
        {
            if (_scFramebuffers != null)
            {
                for (int i = 0; i < _scFramebuffers.Length; i++)
                {
                    _scFramebuffers[i]?.Dispose();
                    _scFramebuffers[i] = null;
                }
                Array.Clear(_scFramebuffers, 0, _scFramebuffers.Length);
            }
        }

        private void CreateDepthTexture()
        {
            if (_depthFormat.HasValue)
            {
                _depthAttachment?.Target.Dispose();
                var depthTexture = _gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                    Math.Max(1, _scExtent.width),
                    Math.Max(1, _scExtent.height),
                    1,
                    1,
                    _depthFormat.Value,
                    VkImageUsageFlags.DepthStencilAttachment,
                    VkImageCreateFlags.None,
                    VkImageTiling.Optimal));
                _depthAttachment = new FramebufferAttachment(depthTexture, 0);
            }
        }

        private void CreateFramebuffers()
        {
            if (_scFramebuffers != null)
            {
                for (int i = 0; i < _scFramebuffers.Length; i++)
                {
                    _scFramebuffers[i]?.Dispose();
                    _scFramebuffers[i] = null;
                }
                Array.Clear(_scFramebuffers, 0, _scFramebuffers.Length);
            }

            Util.EnsureArrayMinimumSize(ref _scFramebuffers, (uint)_scImages.Length);
            Util.EnsureArrayMinimumSize(ref _scColorTextures, (uint)_scImages.Length);
            for (uint i = 0; i < _scImages.Length; i++)
            {
                var colorTex = new Texture(
                    _gd,
                    Math.Max(1, _scExtent.width),
                    Math.Max(1, _scExtent.height),
                    1,
                    1,
                    _scImageFormat,
                    VkImageUsageFlags.ColorAttachment,
                    VkSampleCountFlags.Count1,
                    _scImages[i]);
                FramebufferDescription desc = new FramebufferDescription(_depthAttachment?.Target, colorTex);
                VkFramebuffer fb = new VkFramebuffer(_gd, ref desc, true);
                _scFramebuffers[i] = fb;
                _scColorTextures[i] = new FramebufferAttachment[] { new FramebufferAttachment(colorTex, 0) };
            }
        }

        public override void TransitionToIntermediateLayout(VkCommandBuffer cb)
        {
            for (int i = 0; i < ColorTargets.Count; i++)
            {
                FramebufferAttachment ca = ColorTargets[i];
                var vkTex = ca.Target;
                vkTex.SetImageLayout(0, ca.ArrayLayer, VkImageLayout.ColorAttachmentOptimal);
            }
        }

        public override void TransitionToFinalLayout(VkCommandBuffer cb)
        {
            for (int i = 0; i < ColorTargets.Count; i++)
            {
                FramebufferAttachment ca = ColorTargets[i];
                var vkTex = ca.Target;
                vkTex.TransitionImageLayout(cb, 0, 1, ca.ArrayLayer, 1, VkImageLayout.PresentSrcKHR);
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

        protected override void DisposeCore()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                _depthAttachment?.Target.Dispose();
                DestroySwapchainFramebuffers();
            }
        }
    }
}
