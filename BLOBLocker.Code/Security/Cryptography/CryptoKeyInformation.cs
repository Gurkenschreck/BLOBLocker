using Cipha.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Security.Cryptography
{
    public sealed class CryptoKeyInformation : IDisposable
    {
        public bool CloneParameters { get; set; }
        public byte[] CryptoCookiePart { get; set; }
        public byte[] SessionKey { get; set; }
        public byte[] SessionIV { get; set; }
        public byte[] SessionStoredKeyPart { get; set; }

        public CryptoKeyInformation(bool cloneParams = true) { this.CloneParameters = cloneParams; }
        public CryptoKeyInformation(byte[] cryptoCookiePart, byte[] sessionKey, byte[] sessionIV, byte[] sessionStoredKeyPart, bool cloneParams = true)
        {
            if (cryptoCookiePart == null)
                throw new ArgumentNullException("cryptoCookie");
            if (sessionKey == null)
                throw new ArgumentNullException("sessionKey");
            if (sessionIV == null)
                throw new ArgumentNullException("sessionIV");
            if (sessionStoredKeyPart == null)
                throw new ArgumentNullException("sessionStoredKeyPart");

            CloneParameters = cloneParams;

            this.CryptoCookiePart = cryptoCookiePart;
            if (CloneParameters)
            {
                this.SessionKey = sessionKey.Clone() as byte[];
                this.SessionIV = sessionIV.Clone() as byte[];
                this.SessionStoredKeyPart = sessionStoredKeyPart.Clone() as byte[];
            }
            else
            {
                this.SessionKey = sessionKey;
                this.SessionIV = sessionIV;
                this.SessionStoredKeyPart = sessionStoredKeyPart;
            }
        }

        ~CryptoKeyInformation()
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
                if (CryptoCookiePart != null)
                    Utilities.SetArrayValuesZero(CryptoCookiePart);
                if (SessionKey != null)
                    Utilities.SetArrayValuesZero(SessionKey);
                if (SessionIV != null)
                    Utilities.SetArrayValuesZero(SessionIV);
                if (SessionStoredKeyPart != null)
                    Utilities.SetArrayValuesZero(SessionStoredKeyPart);
            }
        }
    }
}
