﻿using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Veldrid.Vk.VulkanUtil;

namespace Veldrid.Vk
{
    internal unsafe class VkTextureView : TextureView
    {
        private readonly VkGraphicsDevice _gd;
        private readonly VkImageView _imageView;
        private bool _destroyed;
        private string _name;

        public VkImageView ImageView => _imageView;

        public new VkTexture Target => (VkTexture)base.Target;

        public ResourceRefCount RefCount { get; }

        public VkTextureView(VkGraphicsDevice gd, ref TextureViewDescription description)
            : base(ref description)
        {
            _gd = gd;
            VkTexture tex = Util.AssertSubtype<Texture, VkTexture>(description.Target);

            VkImageAspectFlags aspectFlags;
            if ((description.Target.Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil)
            {
                aspectFlags = VkImageAspectFlags.Depth;
            }
            else
            {
                aspectFlags = VkImageAspectFlags.Color;
            }

            var imageViewCI = new VkImageViewCreateInfo
            {
                sType = VkStructureType.ImageViewCreateInfo,
                image = tex.OptimalDeviceImage,
                format = VkFormats.VdToVkPixelFormat(Format, (Target.Usage & TextureUsage.DepthStencil) != 0),
                subresourceRange = new VkImageSubresourceRange(
                    aspectFlags,
                    description.BaseMipLevel,
                    description.MipLevels,
                    description.BaseArrayLayer,
                    description.ArrayLayers)
            };

            if ((tex.Usage & TextureUsage.Cubemap) == TextureUsage.Cubemap)
            {
                imageViewCI.viewType = description.ArrayLayers == 1 ? VkImageViewType.ImageCube : VkImageViewType.ImageCubeArray;
                imageViewCI.subresourceRange.layerCount *= 6;
            }
            else
            {
                switch (tex.Type)
                {
                    case TextureType.Texture1D:
                        imageViewCI.viewType = description.ArrayLayers == 1
                            ? VkImageViewType.Image1D
                            : VkImageViewType.Image1DArray;
                        break;
                    case TextureType.Texture2D:
                        imageViewCI.viewType = description.ArrayLayers == 1
                            ? VkImageViewType.Image2D
                            : VkImageViewType.Image2DArray;
                        break;
                    case TextureType.Texture3D:
                        imageViewCI.viewType = VkImageViewType.Image3D;
                        break;
                }
            }

            vkCreateImageView(_gd.Device, &imageViewCI, null, out _imageView);
            RefCount = new ResourceRefCount(DisposeCore);
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

        public override void Dispose()
        {
            RefCount.Decrement();
        }

        private void DisposeCore()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                vkDestroyImageView(_gd.Device, ImageView, null);
            }
        }
    }
}
