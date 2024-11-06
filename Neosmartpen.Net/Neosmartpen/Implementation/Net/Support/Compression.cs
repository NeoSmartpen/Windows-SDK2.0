using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.IO;


namespace Neosmartpen.Net.Support
{
    internal class Compression
    {
        public static byte[] Compress(byte[] inbuf)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var outputStream = new DeflaterOutputStream(ms, new Deflater(9, true)))
                {
                    outputStream.Write(inbuf, 0, inbuf.Length);
                    outputStream.Flush();
                    outputStream.Finish();
                    return ms.ToArray();
                }
            }
        }

        public static byte[] Decompress(byte[] inbuf)
        {
            using (var compressed = new MemoryStream(inbuf))
            {
                using (var decompressed = new MemoryStream())
                {
                    using (var inputStream = new InflaterInputStream(compressed))
                    {
                        inputStream.CopyTo(decompressed);
                    }
                    return decompressed.ToArray();
                }
            }
        }
    }
}
