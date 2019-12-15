using System;
using System.Collections.Generic;
using System.Linq;
using static SoulsFormats.DDS;

namespace SoulsFormats
{
    /* Known TPF texture formats
      0 - DXT1
      1 - DXT1
      3 - DXT3
      5 - DXT5
      6 - B5G5R5A1_UNORM
      9 - B8G8R8A8
     10 - R8G8B8 on PC, A8G8B8R8 on PS3
     16 - A8
     22 - A16B16G16R16f
     23 - DXT5
     24 - DXT1
     25 - DXT1
     33 - DXT5
    100 - BC6H_UF16
    102 - BC7_UNORM
    103 - ATI1
    104 - ATI2
    105 - A8B8G8R8
    106 - BC7_UNORM
    107 - BC7_UNORM
    108 - DXT1
    109 - DXT1
    110 - DXT5
    112 - BC7_UNORM_SRGB
    113 - BC6H_UF16
    */

    /* BCn block sizes
    BC1 (DXT1) - 8
    BC2 (DXT3) - 16
    BC3 (DXT5) - 16
    BC4 (ATI1) - 8
    BC5 (ATI2) - 16
    BC6 - 16
    BC7 - 16
    */
    internal static class Headerizer
    {
        private static Dictionary<byte, int> CompressedBPB = new Dictionary<byte, int>
        {
            [0] = 8,
            [1] = 8,
            [3] = 16,
            [5] = 16,
            [23] = 16,
            [24] = 8,
            [25] = 8,
            [33] = 16,
            [100] = 16,
            [102] = 16,
            [103] = 8,
            [104] = 16,
            [106] = 16,
            [107] = 16,
            [108] = 8,
            [109] = 8,
            [110] = 16,
            [112] = 16,
            [113] = 16,
        };

        private static Dictionary<byte, int> UncompressedBPP = new Dictionary<byte, int>
        {
            [6] = 2,
            [9] = 4,
            [10] = 4,
            [16] = 1,
            [22] = 8,
            [105] = 4,
        };

        private static Dictionary<byte, string> FourCC = new Dictionary<byte, string>
        {
            [0] = "DXT1",
            [1] = "DXT1",
            [3] = "DXT3",
            [5] = "DXT5",
            [22] = "q\0\0\0", // 0x71
            [23] = "DXT5",
            [24] = "DXT1",
            [25] = "DXT1",
            [33] = "DXT5",
            [103] = "ATI1",
            [104] = "ATI2",
            [108] = "DXT1",
            [109] = "DXT1",
            [110] = "DXT5",
        };

        private static byte[] DX10Formats = { 6, 100, 102, 106, 107, 112, 113 };

        public static byte[] Headerize(TPF.Texture texture)
        {
            if (SFEncoding.ASCII.GetString(texture.Bytes, 0, 4) == "DDS ")
                return texture.Bytes;

            var dds = new DDS();
            byte format = texture.Format;
            short width = texture.Header.Width;
            short height = texture.Header.Height;
            int mipCount = texture.Mipmaps;
            TPF.TexType type = texture.Type;

            dds.dwFlags = DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT;
            if (CompressedBPB.ContainsKey(format))
                dds.dwFlags |= DDSD.PITCH;
            else if (UncompressedBPP.ContainsKey(format))
                dds.dwFlags |= DDSD.LINEARSIZE;

            dds.dwHeight = height;
            dds.dwWidth = width;

            if (CompressedBPB.ContainsKey(format))
                dds.dwPitchOrLinearSize = Math.Max(1, (width + 3) / 4) * CompressedBPB[format];
            else if (UncompressedBPP.ContainsKey(format))
                dds.dwPitchOrLinearSize = (width * UncompressedBPP[format] + 7) / 8;

            if (mipCount == 0)
                mipCount = DetermineMipCount(width, height);
            dds.dwMipMapCount = mipCount;

            dds.dwCaps = DDSCAPS.TEXTURE;
            if (type == TPF.TexType.Cubemap)
                dds.dwCaps |= DDSCAPS.COMPLEX;
            if (mipCount > 1)
                dds.dwCaps |= DDSCAPS.COMPLEX | DDSCAPS.MIPMAP;

            if (type == TPF.TexType.Cubemap)
                dds.dwCaps2 = CUBEMAP_ALLFACES;
            else if (type == TPF.TexType.Volume)
                dds.dwCaps2 = DDSCAPS2.VOLUME;

            PIXELFORMAT ddspf = dds.ddspf;

            if (FourCC.ContainsKey(format) || DX10Formats.Contains(format))
                ddspf.dwFlags = DDPF.FOURCC;
            if (format == 6)
                ddspf.dwFlags |= DDPF.ALPHAPIXELS | DDPF.RGB;
            else if (format == 9)
                ddspf.dwFlags |= DDPF.ALPHAPIXELS | DDPF.RGB;
            else if (format == 10)
                ddspf.dwFlags |= DDPF.RGB;
            else if (format == 16)
                ddspf.dwFlags |= DDPF.ALPHA;
            else if (format == 105)
                ddspf.dwFlags |= DDPF.ALPHAPIXELS | DDPF.RGB;

            if (FourCC.ContainsKey(format))
                ddspf.dwFourCC = FourCC[format];
            else if (DX10Formats.Contains(format))
                ddspf.dwFourCC = "DX10";

            if (format == 6)
            {
                ddspf.dwRGBBitCount = 16;
                ddspf.dwRBitMask = 0b01111100_00000000;
                ddspf.dwGBitMask = 0b00000011_11100000;
                ddspf.dwBBitMask = 0b00000000_00011111;
                ddspf.dwABitMask = 0b10000000_00000000;
            }
            else if (format == 9)
            {
                ddspf.dwRGBBitCount = 32;
                ddspf.dwRBitMask = 0x00FF0000;
                ddspf.dwGBitMask = 0x0000FF00;
                ddspf.dwBBitMask = 0x000000FF;
                ddspf.dwABitMask = 0xFF000000;
            }
            else if (format == 10)
            {
                ddspf.dwRGBBitCount = 24;
                ddspf.dwRBitMask = 0x00FF0000;
                ddspf.dwGBitMask = 0x0000FF00;
                ddspf.dwBBitMask = 0x000000FF;
            }
            else if (format == 16)
            {
                ddspf.dwRGBBitCount = 8;
                ddspf.dwABitMask = 0x000000FF;
            }
            else if (format == 105)
            {
                ddspf.dwRGBBitCount = 32;
                ddspf.dwRBitMask = 0x000000FF;
                ddspf.dwGBitMask = 0x0000FF00;
                ddspf.dwBBitMask = 0x00FF0000;
                ddspf.dwABitMask = 0xFF000000;
            }

            if (DX10Formats.Contains(format))
            {
                dds.header10 = new HEADER_DXT10();
                dds.header10.dxgiFormat = (DXGI_FORMAT)texture.Header.DXGIFormat;
                if (type == TPF.TexType.Cubemap)
                    dds.header10.miscFlag = RESOURCE_MISC.TEXTURECUBE;
            }

            byte[] bytes = texture.Bytes;
            int imageCount = type == TPF.TexType.Cubemap ? 6 : 1;

            List<Image> images = null;
            if (CompressedBPB.ContainsKey(format))
                images = Image.ReadCompressed(bytes, width, height, imageCount, mipCount, 0x80, CompressedBPB[format]);
            else if (UncompressedBPP.ContainsKey(format))
                images = Image.ReadUncompressed(bytes, width, height, imageCount, mipCount, 0x80, UncompressedBPP[format]);

            if (format == 10)
            {
                int texelSize = -1;
                int texelWidth = -1;
                if (format == 10)
                {
                    texelSize = 4;
                    texelWidth = width;
                }

                foreach (Image image in images)
                {
                    for (int i = 0; i < image.MipLevels.Count; i++)
                    {
                        byte[] swizzled = image.MipLevels[i];
                        byte[] unswizzled = DeswizzlePS3(swizzled, texelSize, texelWidth / (int)Math.Pow(2, i));
                        if (format == 10)
                        {
                            byte[] trimmed = new byte[unswizzled.Length / 4 * 3];
                            for (int j = 0; j < unswizzled.Length / 4; j++)
                            {
                                Array.Reverse(unswizzled, j * 4, 4);
                                Array.Copy(unswizzled, j * 4, trimmed, j * 3, 3);
                            }
                            unswizzled = trimmed;
                        }
                        image.MipLevels[i] = unswizzled;
                    }
                }
            }

            if (images != null)
                bytes = Image.Write(images);

            return dds.Write(bytes);
        }

        private static int DetermineMipCount(int width, int height)
        {
            return (int)Math.Ceiling(Math.Log(Math.Max(width, height), 2)) + 1;
        }

        // Black magic stolen from Insomniac Games
        // https://web.archive.org/web/20080704105751/http://www.insomniacgames.com/tech/articles/0108/curiouslysmallcode.php
        private static byte[] DeswizzlePS3(byte[] swizzled, int texelSize, int texelWidth)
        {
            byte[] unswizzled = new byte[swizzled.Length];

            int x = 0;
            int y = 0;
            for (int i = 0; i < swizzled.Length / texelSize; i++)
            {
                Array.Copy(swizzled, i * texelSize, unswizzled, y * texelWidth * texelSize + x * texelSize, texelSize);

                int and0 = x & y;
                int and1 = and0 + 1;
                int xinc = and0 ^ and1;
                int yinc = x & xinc;
                x ^= xinc;
                y ^= yinc;
            }

            return unswizzled;
        }

        private class Image
        {
            public List<byte[]> MipLevels;

            public Image()
            {
                MipLevels = new List<byte[]>();
            }

            public static byte[] Write(List<Image> images)
            {
                var bw = new BinaryWriterEx(false);
                foreach (Image image in images)
                    foreach (byte[] mip in image.MipLevels)
                        bw.WriteBytes(mip);
                return bw.FinishBytes();
            }

            public static List<Image> ReadUncompressed(byte[] bytes, int width, int height, int imageCount, int mipCount, int padBetween, int bytesPerPixel)
            {
                var images = new List<Image>(imageCount);
                var br = new BinaryReaderEx(false, bytes);
                for (int i = 0; i < imageCount; i++)
                {
                    var image = new Image();
                    br.Pad(padBetween);
                    for (int j = 0; j < mipCount; j++)
                    {
                        int scale = (int)Math.Pow(2, j);
                        int w = width / scale;
                        int h = height / scale;
                        image.MipLevels.Add(br.ReadBytes(w * h * bytesPerPixel));
                    }
                    images.Add(image);
                }
                return images;
            }

            public static List<Image> ReadCompressed(byte[] bytes, int width, int height, int imageCount, int mipCount, int padBetween, int bytesPerBlock)
            {
                var images = new List<Image>(imageCount);
                var br = new BinaryReaderEx(false, bytes);
                for (int i = 0; i < imageCount; i++)
                {
                    var image = new Image();
                    br.Pad(padBetween);
                    for (int j = 0; j < mipCount; j++)
                    {
                        int scale = (int)Math.Pow(2, j);
                        int w = width / scale;
                        int h = height / scale;
                        int blocks = (int)Math.Max(1, w / 4f) * (int)Math.Max(1, h / 4f);
                        image.MipLevels.Add(br.ReadBytes(blocks * bytesPerBlock));
                    }
                    images.Add(image);
                }
                return images;
            }
        }
    }
}
