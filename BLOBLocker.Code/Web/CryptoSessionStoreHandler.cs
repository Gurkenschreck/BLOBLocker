using BLOBLocker.Code.Security.Cryptography;
using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLOBLocker.Code.Web
{
    public sealed class CryptoSessionStoreHandler : IDisposable
    {
        HttpSessionStateBase session;
        HttpRequestBase request;
        HttpResponseBase response;

        bool cookieHttpOnly = true;
        public bool CookieHttpOnly { get { return cookieHttpOnly; } set { cookieHttpOnly = value; } }

        bool cookieSecure = true;
        public bool CookieSecure { get { return cookieSecure; } set { cookieSecure = value; } }
        SymmetricCipher<AesManaged> cipher { get; set; }
        int keySize = 256;
        public int KeySize { get { return keySize; } set { keySize = value; } }

        public CryptoSessionStoreHandler(HttpSessionStateBase session,
            HttpRequestBase request, HttpResponseBase response)
            : this(session, request, response, 256)
        {        }

        public CryptoSessionStoreHandler(HttpSessionStateBase session,
            HttpRequestBase request, HttpResponseBase response, int keySize)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (request == null)
                throw new ArgumentNullException("request");
            this.session = session;
            this.request = request;
            this.response = response;

            KeySize = keySize;
            cipher = new SymmetricCipher<AesManaged>(KeySize);
        }

        public CryptoSessionStoreHandler(HttpSessionStateBase session,
            HttpRequestBase request, HttpResponseBase response, byte[] key, byte[] iv)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (request == null)
                throw new ArgumentNullException("request");
            this.session = session;
            this.request = request;

            cipher = new SymmetricCipher<AesManaged>(key, iv);
            KeySize = cipher.KeySize;
        }

        ~CryptoSessionStoreHandler()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public CryptoKeyInformation GetCryptoSessionStoreInformation(string storeName, bool cloneValues = false)
        {
            byte[] storeSessionKey = session[storeName + "__SessionKey"] as byte[];
            byte[] storeSessionIV = session[storeName + "__SessionIV"] as byte[];
            byte[] storeSessionStoredKeyPart = session[storeName + "__SessionStoredKeyPart"] as byte[];
            HttpCookie cryptoCookie = request.Cookies[storeName + "__Crypto"];

            return new CryptoKeyInformation(Convert.FromBase64String(cryptoCookie.Value),
                storeSessionKey, storeSessionIV, storeSessionStoredKeyPart, cloneValues);
        }

        public void InjectCryptoSessionStore(string storeName, CryptoKeyInformation cssi)
        {
            session[storeName + "__SessionKey"] = cssi.SessionKey.Clone() as byte[];
            session[storeName + "__SessionIV"] = cssi.SessionIV.Clone() as byte[];
            session[storeName + "__SessionStoredKeyPart"] = cssi.SessionStoredKeyPart.Clone() as byte[];

            HttpCookie cryptoCookie = new HttpCookie(storeName + "__Crypto",
                Convert.ToBase64String(cssi.CryptoCookiePart));
            cryptoCookie.Secure = CookieSecure;
            cryptoCookie.HttpOnly = CookieHttpOnly;
            response.SetCookie(cryptoCookie);
        }

        public void StoreData(byte[] plain, out CryptoKeyInformation cssi)
        {
            //1. Encrypt plain priv key
            byte[] encPriKey = cipher.Encrypt(plain);
            //2. Extract key and iv
            byte[] key = cipher.Key;
            byte[] iv = cipher.IV;
            //3. Split encrypted privKey
            byte[] Pa;
            byte[] Pb;
            Pa = encPriKey.Take(cipher.KeySize / 8).ToArray();
            Pb = encPriKey.Skip(cipher.KeySize / 8).ToArray();
            //4. Take n bytes of Pa as new key
            // In this case you could just use Pa as the key because it is already keySize / 8 bits longs
            byte[] a = Pa.Take(cipher.KeySize / 8).ToArray();
            //5. Save Pa in cookie of user
            //6. Initialize 2nd cipher to encrypt Pb for session storage
            using (var cookieCipher = new SymmetricCipher<AesManaged>(a, iv))
            {
                cssi = new CryptoKeyInformation(Pa,
                    key, iv, cookieCipher.Encrypt(Pb), false);
            }
        }

        public byte[] ExtractData(string storeName)
        {
            return ExtractData(GetCryptoSessionStoreInformation(storeName));
        }

        public byte[] ExtractData(CryptoKeyInformation cssi)
        {
            // 1. Get session key and iv
            byte[] rKey = cssi.SessionKey;
            byte[] rIV = cssi.SessionIV;

            // 2. Get user part of key
            byte[] rPa = cssi.CryptoCookiePart;
            // 3. Get key extracted from user cookie key
            byte[] ra = rPa.Take(cipher.KeySize / 8).ToArray();
            // 4. Initialize cipher with extracted keypart and cookieIV
            using (var cookieCipher = new SymmetricCipher<AesManaged>(ra, rIV))
            {
                // 5. Get server side second key part and decrypt it
                byte[] encAccKeyPart = cssi.SessionStoredKeyPart;
                byte[] rPb = cookieCipher.Decrypt(encAccKeyPart);
                // 6. Combine both pairs
                byte[] rP = new byte[rPa.Length + rPb.Length];
                Array.Copy(rPa, rP, rPa.Length);
                Array.Copy(rPb, 0, rP, rPa.Length, rPb.Length);

                // 7. Decrypt encrypted private key of account
                using (var symCi = new SymmetricCipher<AesManaged>(rKey, rIV))
                {
                    return symCi.Decrypt(rP);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                cipher.Dispose();
                KeySize = 0;
            }
        }
    }
}
