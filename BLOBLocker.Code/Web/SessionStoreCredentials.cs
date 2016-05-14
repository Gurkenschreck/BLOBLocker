using Cipha.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Web
{
    public sealed class SessionStoreCredentials : IDisposable
    {
        public byte[] CookieKeyPart { get; private set; }
        public byte[] EncryptedStoreKeyPart { get; private set; }
        public byte[] EncryptedStoreIV { get; private set; }
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public SessionStoreCredentials(byte[] cookieKeyPart,
            byte[] encryptedStoreKeyPart,
            byte[] encryptedStoreIV,
            byte[] key,
            byte[] iv)
        {
            CookieKeyPart = cookieKeyPart.Clone() as byte[];
            EncryptedStoreKeyPart = encryptedStoreKeyPart.Clone() as byte[];
            EncryptedStoreIV = encryptedStoreIV.Clone() as byte[];
            Key = key.Clone() as byte[];
            IV = iv.Clone() as byte[];
        }

        ~SessionStoreCredentials()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (CookieKeyPart != null)
                    Utilities.SetArrayValuesZero(CookieKeyPart);
                if (EncryptedStoreKeyPart != null)
                    Utilities.SetArrayValuesZero(EncryptedStoreKeyPart);
                if (EncryptedStoreIV != null)
                    Utilities.SetArrayValuesZero(EncryptedStoreIV);
                if (Key != null)
                    Utilities.SetArrayValuesZero(Key);
                if (IV != null)
                    Utilities.SetArrayValuesZero(IV);
            }
        }
    }
}
