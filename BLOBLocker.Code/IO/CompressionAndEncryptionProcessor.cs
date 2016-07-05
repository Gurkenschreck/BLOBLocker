using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.IO
{
    /// <summary>
    /// Compresses and encrypts the compressed data,
    /// eventually saving space when saving data 
    /// securely to a hard drive.
    /// </summary>
    public sealed class CompressionAndEncryptionProcessor : CryptoProcessor
    {
        public CompressionAndEncryptionProcessor(SymmetricCipher<AesManaged> aesCipher)
        {
            if (aesCipher == null)
                throw new ArgumentNullException("aesCipher");
            this.aesCipher = aesCipher;
        }

        protected override void ProcessStream(Stream originalStream, Stream processedStream)
        {
            using (var cry = new CryptoStream(processedStream, aesCipher.Algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (var compressedFileStream = new GZipStream(cry, CompressionMode.Compress))
                {
                    originalStream.CopyTo(compressedFileStream);
                }
            }
        }

        protected override void GetOriginalStream(Stream processedStream, Stream originalStream)
        {
            using (var cry = new CryptoStream(processedStream, aesCipher.Algorithm.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (var decompressionStream = new GZipStream(cry, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(originalStream);
                }
            }
        }
    }
}
