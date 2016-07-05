using BLOBLocker.Entities.Models.WebApp;
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
    /// Creates new instances of classes deriving
    /// from BaseProcessor.
    /// </summary>
    public sealed class ProcessorFactory
    {
        public BaseProcessor CreateProcessor(StoringMode mode)
        {
            return CreateProcessor(mode, null);
        }

        public BaseProcessor CreateProcessor(StoringMode mode, SymmetricCipher<AesManaged> aesCipher)
        {
            BaseProcessor storer = null;
            switch (mode)
            {
                case StoringMode.Compressed:
                    storer = new CompressionProcessor();
                    break;
                case StoringMode.Encrypted:
                    if (aesCipher == null)
                        throw new InvalidOperationException("aesCipher must be provided");
                    storer = new EncryptionProcessor(aesCipher);
                    break;
                case StoringMode.CompressedAndEncrypted:
                    if (aesCipher == null)
                        throw new InvalidOperationException("aesCipher must be provided");
                    storer = new CompressionAndEncryptionProcessor(aesCipher);
                    break;
                case StoringMode.Unprocessed:
                default:
                    storer = new PlainProcessor();
                    break;
            }
            return storer;
        }
    }
}
