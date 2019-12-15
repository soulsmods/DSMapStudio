using System;
using System.Runtime.InteropServices;

namespace SoulsFormats
{
    internal static class Oodle26
    {
        [DllImport("oo2core_6_win64.dll")]
        private static extern uint OodleLZ_Compress(Compressor compressor, byte[] src_buf, ulong src_len, byte[] dst_buf, CompressionLevel level,
            CompressOptions options, ulong offs, ulong unused, IntPtr scratch, ulong scratch_size);

        [DllImport("oo2core_6_win64.dll")]
        private static extern IntPtr OodleLZ_CompressOptions_GetDefault(Compressor compressor, CompressionLevel compressionLevel);

        [DllImport("oo2core_6_win64.dll")]
        private static extern uint OodleLZ_Decompress(byte[] compBuf, ulong src_len, byte[] decodeTo, ulong dst_size,
            FuzzSafe fuzzSafe, int crc, int verbose, IntPtr dst_base, ulong e, IntPtr cb, IntPtr cb_ctx, IntPtr scratch, ulong scratch_size, int threadPhase);

        [DllImport("oo2core_6_win64.dll")]
        private static extern uint OodleLZ_GetCompressedBufferSizeNeeded(ulong src_len);

        public static byte[] Compress(byte[] source, Compressor compressor, CompressionLevel level)
        {
            uint compressionBound = OodleLZ_GetCompressedBufferSizeNeeded((ulong)source.LongLength);
            CompressOptions options = CompressOptions.GetDefault(compressor, level);

            byte[] dest = new byte[compressionBound];
            uint destLength = OodleLZ_Compress(compressor, source, (ulong)source.LongLength, dest, level, options, 0, 0, IntPtr.Zero, 0);
            Array.Resize(ref dest, (int)destLength);
            return dest;
        }

        public static byte[] Decompress(byte[] source, ulong uncompressedSize)
        {
            byte[] dest = new byte[uncompressedSize];
            OodleLZ_Decompress(source, (ulong)source.LongLength, dest, uncompressedSize, FuzzSafe.Yes, 0, 0, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, 0);
            return dest;
        }

        public const int OODLELZ_FAILED = 0;

        public enum Compressor
        {
            LZH,
            LZHLW,
            LZNIB,
            None,
            LZB16,
            LZBLW,
            LZA,
            LZNA,
            Kraken,
            Mermaid,
            BitKnit,
            Selkie,
            Hydra,
            Leviathan,
        }

        public enum CompressionLevel
        {
            None,
            SuperFast,
            VeryFast,
            Fast,
            Normal,
            Optimal1,
            Optimal2,
            Optimal3,
            Optimal4,
            TooHigh,
        }

        public enum FuzzSafe
        {
            No,
            Yes,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CompressOptions
        {
            int Unk00;
            int Unk04;
            int Unk08;
            int Unk0C;
            int Unk10;
            int Unk14;
            int SpaceSpeedTradeoffBytes;
            int Unk1C;
            int Unk20;
            int DictionarySize;
            int Unk28;
            int Unk2C;

            public static CompressOptions GetDefault(Compressor compressor, CompressionLevel compressionLevel)
            {
                IntPtr ptr = OodleLZ_CompressOptions_GetDefault(compressor, compressionLevel);
                return Marshal.PtrToStructure<CompressOptions>(ptr);
            }
        }
    }
}
