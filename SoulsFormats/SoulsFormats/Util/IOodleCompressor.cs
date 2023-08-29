namespace SoulsFormats;

public interface IOodleCompressor
{
        public byte[] Compress(byte[] source, Oodle.OodleLZ_Compressor compressor, Oodle.OodleLZ_CompressionLevel level);
        public byte[] Decompress(byte[] source, long uncompressedSize);
}
