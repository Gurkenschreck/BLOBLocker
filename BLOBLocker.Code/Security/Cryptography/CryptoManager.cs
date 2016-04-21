using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Security.Cryptography
{
    public class CryptoManager : IDisposable
    {
        public CryptoManager()
        {

        }

       /* public SymmetricCipher<AesManaged> CreatePoolCipher()
        {

        }*/

        ~CryptoManager()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {

        }
    }
}
