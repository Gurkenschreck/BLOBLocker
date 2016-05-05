using Cipha.Security.Cryptography;
using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLOBLocker.Code.Membership
{
    public sealed class CredentialHandler : IDisposable
    {
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        SymmetricCipher<AesManaged> cipher;

        public CredentialHandler(int keySize)
        {
            cipher = new SymmetricCipher<AesManaged>(keySize);
            HttpOnly = true;
            Secure = true;
        }
        public CredentialHandler(byte[] key, byte[] iv)
        {
            cipher = new SymmetricCipher<AesManaged>(key, iv);
            HttpOnly = true;
            Secure = true;
        }

        ~CredentialHandler()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Inject(byte[] plain, out HttpCookie keypartCookie,
            out byte[] sessionCookieKey,
            out byte[] sessionCookieIV,
            out byte[] sessionStoredKeyPart) 
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
            keypartCookie = new HttpCookie("Secret");
            keypartCookie.Value = Convert.ToBase64String(Pa);
            keypartCookie.HttpOnly = true;
            keypartCookie.Secure = true;
            //6. Initialize 2nd cipher to encrypt Pb for session storage
            using (var cookieCipher = new SymmetricCipher<AesManaged>(a, iv))
            {
                sessionStoredKeyPart = cookieCipher.Encrypt(Pb);
                sessionCookieKey = key;
                sessionCookieIV = iv;
            }
        }

        public byte[] Extract(byte[] storedKey, byte[] sessionCookieKey, byte[] sessionCookieIV, byte[] sessionStoredKeyPart)
        {
            // 1. Get session key and iv
            byte[] rKey = sessionCookieKey;
            byte[] rIV = sessionCookieIV;

            // 2. Get user part of key
            byte[] rPa = storedKey;
            // 3. Get key extracted from user cookie key
            byte[] ra = rPa.Take(cipher.KeySize / 8).ToArray();
            // 4. Initialize cipher with extracted keypart and cookieIV
            using (var cookieCipher = new SymmetricCipher<AesManaged>(ra, rIV))
            {
                // 5. Get server side second key part and decrypt it
                byte[] encAccKeyPart = sessionStoredKeyPart;
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
            if(disposing)
            {
                cipher.Dispose();
            }
        }
    }
}
