using BLOBLocker.Code.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// Simple data compression using the .NET GZipStream
    /// implementation.
    /// </summary>
    public sealed class CompressionProcessor : BaseProcessor
    {
        protected override void ProcessStream(Stream originalStream, Stream newStream)
        {
            using (var compressedFileStream = new GZipStream(newStream, CompressionMode.Compress))
            {
                originalStream.CopyTo(compressedFileStream);
            }
        }

        protected override void GetOriginalStream(Stream processedStream, Stream originalStream)
        {
            using (var zipStream = new GZipStream(processedStream, CompressionMode.Decompress))
            {
                zipStream.CopyTo(originalStream);
            }
        }
    }
}
