using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CryptoPool.Code.Membership
{
    public sealed class CryptoCookieBakery : IDisposable
    {
        byte[] cookieKey;
        public byte[] CookieKey
        {
            get
            {
                return cookieKey.Clone() as byte[];
            } 
            private set 
            {
                cookieKey = value.Clone() as byte[];
            }
        }
        byte[] cookieIV;
        public byte[] CookieIV
        {
            get
            {
                return cookieIV.Clone() as byte[];
            }
            private set
            {
                cookieIV = value.Clone() as byte[];
            }
        }
        public CryptoCookieBakery()
        {

        }
        public CryptoCookieBakery(byte[] cookieKey, byte[] cookieIV)
        {
            CookieKey = cookieKey;
            CookieIV = cookieIV;
        }

        public string GetValue(HttpCookie cookie)
        {
            if (cookieKey == null)
                throw new ArgumentNullException("set cookieKey first");
            if (cookieIV == null)
                throw new ArgumentNullException("set cookieIV first");
            return GetValue(cookie, cookieKey, cookieIV);
        }
        public string GetValue(HttpCookie cookie, byte[] cookieKey, byte[] cookieIV)
        {
            using(var cookieCipher = new SymmetricCipher<AesManaged>(cookieKey, cookieIV))
            {
                return cookieCipher.DecryptToString(cookie.Value);
            }
        }

        public HttpCookie CreateCookie(string name, byte[] privateKey, int cookieKeySize)
        {
            return CreateCookie(name, Convert.ToBase64String(privateKey), cookieKeySize);
        }
        public HttpCookie CreateCookie(string name, string privateKeyXmlString, int cookieKeySize)
        {
            HttpCookie cookie = new HttpCookie(name);
            using (var cookieCipher = new SymmetricCipher<AesManaged>(cookieKeySize))
            {
                cookie.Value = cookieCipher.EncryptToString(privateKeyXmlString);
                CookieKey = cookieCipher.Key;
                CookieIV = cookieCipher.IV;
            }
            cookie.Secure = true;
            return cookie;
        }

        // IDisposable
        ~CryptoCookieBakery()
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
            if(disposing)
            {
                if(cookieKey != null)
                    Cipha.Security.Cryptography.Utilities.SetArrayValuesZero(cookieIV);
                if(cookieIV != null)
                    Cipha.Security.Cryptography.Utilities.SetArrayValuesZero(cookieKey);
            }
        }
    }
}
