using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    /// <summary>
    /// Represents how the data is stored on
    /// the disk.
    /// </summary>
    public enum StoringMode
    {
        Unprocessed = 0,
        Encrypted,
        Compressed,
        CompressedAndEncrypted
    }
}
