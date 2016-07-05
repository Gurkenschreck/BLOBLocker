using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// Encrypts and decrypts data.
    /// </summary>
    public sealed class EncryptionProcessor : CryptoProcessor
    {
        public EncryptionProcessor(SymmetricCipher<AesManaged> aesCipher)
        {
            if (aesCipher == null)
                throw new ArgumentNullException("aesCipher");
            this.aesCipher = aesCipher;
        }

        protected override void ProcessStream(Stream originalStream, Stream processedStream)
        {
            using (var cipherStream = aesCipher.CreateStream())
            {
                cipherStream.EncryptStream(originalStream, processedStream);
            }
        }

        protected override void GetOriginalStream(Stream processedStream, Stream originalStream)
        {
            using (var cipherStream = aesCipher.CreateStream())
            {
                cipherStream.DecryptStream(processedStream, originalStream);
            }
        }
    }
}
