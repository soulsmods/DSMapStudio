using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SoulsFormats;
using Veldrid;

namespace StudioCore.Resource
{
    // Commented out because in the process of porting this code from Monogame
    /*class TextureResource : IResource, IDisposable
    {
        public TPF.Texture Texture { get; private set; } = null;

        public Texture GPUTexture { get; private set; } = null;

        private DDS.DDSCAPS2 FullCubeDDSCaps2 =>
            DDS.DDSCAPS2.CUBEMAP_POSITIVEX |
            DDS.DDSCAPS2.CUBEMAP_NEGATIVEX |
            DDS.DDSCAPS2.CUBEMAP_POSITIVEY |
            DDS.DDSCAPS2.CUBEMAP_NEGATIVEY |
            DDS.DDSCAPS2.CUBEMAP_POSITIVEZ |
            DDS.DDSCAPS2.CUBEMAP_NEGATIVEZ;

        private static PixelFormat GetSurfaceFormatFromString(string str)
        {
            switch (str)
            {
                case "DXT1":
                    return PixelFormat.BC1_Rgba_UNorm;
                case "DXT3":
                    return PixelFormat.BC2_UNorm_SRgb;
                /*case "DXT5":
                    return SurfaceFormat.Dxt5;
                case "ATI1":
                    return SurfaceFormat.ATI1; // Monogame workaround :fatcat:
                case "ATI2":
                    return SurfaceFormat.ATI2;
                default:
                    throw new Exception($"Unknown DDS Type: {str}");
            }
        }

        // From MonoGame.Framework/Graphics/Texture2D.cs and MonoGame.Framework/Graphics/TextureCube.cs
        private (int ByteCount, Rectangle Rect) GetMipInfo(PixelFormat sf, int width, int height, int mip, bool isCubemap)
        {
            width = Math.Max(width >> mip, 1);
            height = Math.Max(height >> mip, 1);

            int formatTexelSize = GetTexelSize(sf);

            if (isCubemap)
            {
                if (IsCompressedFormat(sf))
                {
                    var roundedWidth = (width + 3) & ~0x3;
                    var roundedHeight = (height + 3) & ~0x3;

                    int byteCount = roundedWidth * roundedHeight * formatTexelSize / 16;

                    return (byteCount, new Rectangle(0, 0, roundedWidth, roundedHeight));
                }
                else
                {
                    int byteCount = width * height * formatTexelSize;

                    return (byteCount, new Rectangle(0, 0, width, height));
                }
            }
            else
            {
                if (IsCompressedFormat(sf))
                {
                    int blockWidth, blockHeight;
                    GetBlockSize(sf, out blockWidth, out blockHeight);

                    int blockWidthMinusOne = blockWidth - 1;
                    int blockHeightMinusOne = blockHeight - 1;

                    var roundedWidth = (width + blockWidthMinusOne) & ~blockWidthMinusOne;
                    var roundedHeight = (height + blockHeightMinusOne) & ~blockHeightMinusOne;

                    var rect = new Rectangle(0, 0, roundedWidth, roundedHeight);

                    int byteCount;

                    if (sf == SurfaceFormat.RgbPvrtc2Bpp || sf == SurfaceFormat.RgbaPvrtc2Bpp)
                    {
                        byteCount = (Math.Max(width, 16) * Math.Max(height, 8) * 2 + 7) / 8;
                    }
                    else if (sf == SurfaceFormat.RgbPvrtc4Bpp || sf == SurfaceFormat.RgbaPvrtc4Bpp)
                    {
                        byteCount = (Math.Max(width, 8) * Math.Max(height, 8) * 4 + 7) / 8;
                    }
                    else
                    {
                        byteCount = roundedWidth * roundedHeight * formatTexelSize / (blockWidth * blockHeight);
                    }

                    return (byteCount, rect);
                }
                else
                {
                    int byteCount = width * height * formatTexelSize;

                    return (byteCount, new Rectangle(0, 0, width, height));
                }


            }

        }

        internal static int GetBlockSize(byte tpfTexFormat)
        {
            switch (tpfTexFormat)
            {
                case 105:
                    return 4;
                case 0:
                case 1:
                case 22:
                case 25:
                case 103:
                case 108:
                case 109:
                    return 8;
                case 5:
                case 100:
                case 102:
                case 106:
                case 107:
                case 110:
                    return 16;
                default:
                    throw new NotImplementedException($"TPF Texture format {tpfTexFormat} BlockSize unknown.");
            }
        }

        // Adapted from MonoGame.Framework/Graphics/SurfaceFormat.cs
        public static bool IsCompressedFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1a:
                case SurfaceFormat.Dxt1SRgb:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.Dxt5SRgb:
                case SurfaceFormat.ATI1:
                case SurfaceFormat.ATI2:
                case SurfaceFormat.ATI1SRgb:
                case SurfaceFormat.ATI2SRgb:
                case SurfaceFormat.BC7:
                case SurfaceFormat.BC6HSF16:
                case SurfaceFormat.BC6HTypeless:
                case SurfaceFormat.BC6HUF16:
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                case SurfaceFormat.RgbEtc1:
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbPvrtc4Bpp:
                    return true;
            }
            return false;
        }

        // Adapted from MonoGame.Framework/Graphics/SurfaceFormat.cs
        public static void GetBlockSize(SurfaceFormat surfaceFormat, out int width, out int height)
        {
            switch (surfaceFormat)
            {
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                    width = 8;
                    height = 4;
                    break;
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1SRgb:
                case SurfaceFormat.Dxt1a:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.Dxt5SRgb:
                case SurfaceFormat.ATI1: //Not 100% sure but probably.
                case SurfaceFormat.ATI2:
                case SurfaceFormat.ATI1SRgb:
                case SurfaceFormat.ATI2SRgb:
                case SurfaceFormat.BC7:
                case SurfaceFormat.BC6HSF16:
                case SurfaceFormat.BC6HUF16:
                case SurfaceFormat.BC6HTypeless:
                case SurfaceFormat.RgbPvrtc4Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                case SurfaceFormat.RgbEtc1:
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                    width = 4;
                    height = 4;
                    break;
                default:
                    width = 1;
                    height = 1;
                    break;
            }
        }

        // Adapted from MonoGame.Framework/Graphics/SurfaceFormat.cs
        public static int GetTexelSize(SurfaceFormat surfaceFormat)
        {
            switch (surfaceFormat)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1SRgb:
                case SurfaceFormat.Dxt1a:
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                case SurfaceFormat.RgbPvrtc4Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                case SurfaceFormat.RgbEtc1:
                case SurfaceFormat.ATI1:
                case SurfaceFormat.ATI1SRgb:
                case SurfaceFormat.ATI2:
                case SurfaceFormat.ATI2SRgb:
                    // One texel in DXT1, PVRTC (2bpp and 4bpp) and ETC1 is a minimum 4x4 block (8x4 for PVRTC 2bpp), which is 8 bytes
                    return 8;
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.Dxt5SRgb:
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                case SurfaceFormat.BC7:
                case SurfaceFormat.BC6HSF16:
                case SurfaceFormat.BC6HUF16:
                case SurfaceFormat.BC6HTypeless:
                    // One texel in DXT3 and DXT5 is a minimum 4x4 block, which is 16 bytes
                    return 16;
                case SurfaceFormat.Alpha8:
                    return 1;
                case SurfaceFormat.Bgr565:
                case SurfaceFormat.Bgra4444:
                case SurfaceFormat.Bgra5551:
                case SurfaceFormat.HalfSingle:
                case SurfaceFormat.NormalizedByte2:
                    return 2;
                case SurfaceFormat.Color:
                case SurfaceFormat.ColorSRgb:
                case SurfaceFormat.Single:
                case SurfaceFormat.Rg32:
                case SurfaceFormat.HalfVector2:
                case SurfaceFormat.NormalizedByte4:
                case SurfaceFormat.Rgba1010102:
                case SurfaceFormat.Bgra32:
                case SurfaceFormat.Bgra32SRgb:
                case SurfaceFormat.Bgr32:
                case SurfaceFormat.Bgr32SRgb:
                    return 4;
                case SurfaceFormat.HalfVector4:
                case SurfaceFormat.Rgba64:
                case SurfaceFormat.Vector2:
                    return 8;
                case SurfaceFormat.Vector4:
                    return 16;
                default:
                    throw new ArgumentException();
            }
        }

        public bool _LoadTexture(TPF tpf, int texindex, AccessLevel al)
        {
            Texture = tpf.Textures[texindex];
            int height = Texture?.Header?.Height ?? 0;
            int width = Texture?.Header?.Width ?? 0;
            int dxgiFormat = Texture?.Header?.DXGIFormat ?? 0;
            int mipmapCount = Texture?.Mipmaps ?? 0;
            string fourCC = "DX10";
            int arraySize = Texture?.Header?.TextureCount ?? 1;

            DDS ppDdsHeader_ForDebug = null;

            bool hasFullCubeDDSCaps2 = false;

            int dataStartOffset = 0;

            var br = new BinaryReaderEx(false, Texture.Bytes);

            bool hasHeader = br.ReadASCII(4) == "DDS ";

            int blockSize = !hasHeader ? GetBlockSize(Texture.Format) : -1;

            if (hasHeader)
            {
                DDS header = new DDS(Texture.Bytes);
                height = header.dwHeight;
                width = header.dwWidth;
                mipmapCount = header.dwMipMapCount;
                fourCC = header.ddspf.dwFourCC;

                if ((header.dwCaps2 & FullCubeDDSCaps2) == FullCubeDDSCaps2)
                {
                    hasFullCubeDDSCaps2 = true;
                }

                if (header.header10 != null)
                {
                    arraySize = (int)header.header10.arraySize;
                    dxgiFormat = (int)header.header10.dxgiFormat;
                }

                dataStartOffset = header.DataOffset;

                ppDdsHeader_ForDebug = header;
            }
            else
            {
                if (tpf.Platform == TPF.TPFPlatform.PS4)
                {
                    switch (Texture.Format)
                    {
                        case 0:
                        case 1:
                        case 25:
                        case 103:
                        case 108:
                        case 109:
                            fourCC = "DX10"; //DX10
                            break;
                        case 5:
                        case 100:
                        case 102:
                        case 106:
                        case 107:
                        case 110:
                            fourCC = "DX10"; //DX10
                            break;
                        case 22:
                            //fourCC = 0x71;
                            break;
                        case 105:
                            fourCC = "";
                            break;
                    }
                }
                else if (tpf.Platform == TPF.TPFPlatform.PS3)
                {
                    switch (Texture.Format)
                    {
                        case 0:
                        case 1:
                            //fourCC = 0x31545844;
                            fourCC = "DXT1";
                            break;
                        case 5:
                            //fourCC = 0x35545844;
                            fourCC = "DXT3";
                            break;
                        case 9:
                        case 10:
                            fourCC = "";
                            break;
                    }
                }

                if (mipmapCount == 0)
                {
                    // something Hork came up with :fatcat:
                    mipmapCount = (int)(1 + Math.Floor(Math.Log(Math.Max(width, height), 2)));
                }

                dataStartOffset = 0;
            }

            PixelFormat surfaceFormat;
            if (fourCC == "DX10")
            {
                // See if there are DX9 textures
                int fmt = dxgiFormat;
                if (fmt == 70 || fmt == 71 || fmt == 72)
                    surfaceFormat = SurfaceFormat.Dxt1;
                else if (fmt == 73 || fmt == 74 || fmt == 75)
                    surfaceFormat = SurfaceFormat.Dxt3;
                else if (fmt == 76 || fmt == 77 || fmt == 78)
                    surfaceFormat = SurfaceFormat.Dxt5;
                else if (fmt == 79 || fmt == 80 || fmt == 81)
                    surfaceFormat = SurfaceFormat.ATI1;
                else if (fmt == 82 || fmt == 83 || fmt == 84)
                    surfaceFormat = SurfaceFormat.ATI2;
                else if (fmt == 95)
                    surfaceFormat = SurfaceFormat.BC6HUF16;
                else if (fmt == 96)
                    surfaceFormat = SurfaceFormat.BC6HSF16;
                else if (fmt == 94)
                    surfaceFormat = SurfaceFormat.BC6HTypeless;
                else if (fmt == 97 || fmt == 98 || fmt == 99)
                    surfaceFormat = SurfaceFormat.BC7;
                else
                {
                    // No DX10 texture support in monogame yet
                    //Console.WriteLine($"Unable to load {TexName} because it uses DX10+ exclusive texture type.");
                    return false;
                }
            }
            else
            {
                surfaceFormat = GetSurfaceFormatFromString(fourCC);
            }

            bool mipmaps = mipmapCount > 0;

            // apply memes
            if (tpf.Platform == TPF.TPFPlatform.PC)
            {
                width = IsCompressedFormat(surfaceFormat) ? ((width + 3) & ~0x3) : width;
                height = IsCompressedFormat(surfaceFormat) ? ((height + 3) & ~0x3) : height;
                mipmaps = true;
            }
            else if (tpf.Platform == TPF.TPFPlatform.PS4)
            {
                width = (int)(Math.Ceiling(width / 4f) * 4f);
                height = (int)(Math.Ceiling(height / 4f) * 4f);
            }
            else if (tpf.Platform == TPF.TPFPlatform.PS3)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }

            Texture tex = null;

            int paddedWidth = 0;
            int paddedHeight = 0;
            int paddedSize = 0;
            int copyOffset = dataStartOffset;

            bool isCubeMap = (Texture?.Type == TPF.TexType.Cubemap) || arraySize >= 6 || hasFullCubeDDSCaps2;

            if (isCubeMap)
            {
                tex = new TextureCube(GFX.Device, width, true, surfaceFormat);
            }
            else
            {
                tex = new Texture2D(GFX.Device, width, height,
                    mipmapCount > 0,
                    surfaceFormat,
                    arraySize);
            }

            if (tpf.Platform == TPF.TPFPlatform.PC)
            {
                for (int i = 0; i < arraySize; i++)
                {
                    for (int j = 0; j < mipmapCount; j++)
                    {
                        var mipInfo = GetMipInfo(surfaceFormat, width, height, j, isCubeMap);

                        paddedSize = mipInfo.ByteCount;

                        //if (surfaceFormat == SurfaceFormat.Dxt1 || surfaceFormat == SurfaceFormat.Dxt1SRgb)
                        //    paddedSize /= 2;

                        if (isCubeMap)
                        {
                            ((TextureCube)tex).SetData((CubeMapFace)i, j, null, br.GetBytes(copyOffset, paddedSize), 0, paddedSize);
                        }
                        else
                        {
                            ((Texture2D)tex).SetData(j, i, null, br.GetBytes(copyOffset, paddedSize), 0, paddedSize);
                        }

                        copyOffset += paddedSize;
                    }
                }
            }
            else if (tpf.Platform == TPF.TPFPlatform.PS4)
            {
                if (isCubeMap)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    int currentWidth = width;
                    int currentHeight = height;

                    for (int j = 0; j < mipmapCount; j++)
                    {
                        if (Texture.Format == 105)
                        {
                            paddedWidth = currentWidth;
                            paddedHeight = currentHeight;
                            paddedSize = paddedWidth * paddedHeight * blockSize;
                        }
                        else
                        {
                            paddedWidth = (int)(Math.Ceiling(currentWidth / 32f) * 32f);
                            paddedHeight = (int)(Math.Ceiling(currentHeight / 32f) * 32f);
                            paddedSize = (int)(Math.Ceiling(paddedWidth / 4f) * Math.Ceiling(paddedHeight / 4f) * blockSize);
                        }

                        var deswizzler = new DDSDeswizzler(Texture.Format, br.GetBytes(copyOffset, paddedSize), blockSize);

                        byte[] deswizzledMipMap = null;

                        deswizzler.CreateOutput();
                        deswizzler.DDSWidth = paddedWidth;
                        deswizzler.DeswizzleDDSBytesPS4(currentWidth, currentHeight);
                        deswizzledMipMap = deswizzler.OutputBytes;

                        var finalBytes = (deswizzledMipMap ?? deswizzler.InputBytes);

                        using (var tempMemStream = new System.IO.MemoryStream())
                        {
                            var tempWriter = new BinaryWriter(tempMemStream);


                            if (Texture.Format == 105)
                            {
                                tempWriter.Write(finalBytes);
                            }
                            else
                            {
                                for (int h = 0; h < (int)Math.Ceiling(currentHeight / 4f); h++)
                                {
                                    tempWriter.Write(finalBytes, (int)(h * Math.Ceiling(paddedWidth / 4f) * blockSize), (int)(Math.Ceiling(currentWidth / 4f) * blockSize));
                                }
                            }

                            ((Texture2D)tex).SetData(j, 0, null, tempMemStream.ToArray(), 0, (int)tempMemStream.Length);
                        }

                        copyOffset += paddedSize;

                        if (currentWidth > 1)
                            currentWidth /= 2;

                        if (currentHeight > 1)
                            currentHeight /= 2;
                    }
                }
            }
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TextureResource()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        bool IResource._Load(byte[] bytes, AccessLevel al)
        {
            throw new NotImplementedException();
        }

        bool IResource._Load(string file, AccessLevel al)
        {
            throw new NotImplementedException();
        }
        #endregion
    }*/
}
