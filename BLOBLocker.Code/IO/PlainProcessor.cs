using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// Leaves the data as is. No data modification takes place.
    /// </summary>
    public sealed class PlainProcessor : BaseProcessor
    {
        protected override void ProcessStream(Stream originalStream, Stream processedStream)
        {
            originalStream.CopyTo(processedStream);
        }

        protected override void GetOriginalStream(Stream processedStream, Stream originalStream)
        {
            processedStream.CopyTo(originalStream);
        }
    }
}
