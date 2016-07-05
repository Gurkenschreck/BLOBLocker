using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// Deriving from BaseProcessor, all processors using
    /// cryptography derive from this.
    /// </summary>
    public abstract class CryptoProcessor : BaseProcessor    
    {
        protected SymmetricCipher<AesManaged> aesCipher = null;

        protected override void disposeImplementation(bool disposing)
        {
            if (disposing)
            {
                if (aesCipher == null)
                    aesCipher.Dispose();
            }
        }
    }
}
