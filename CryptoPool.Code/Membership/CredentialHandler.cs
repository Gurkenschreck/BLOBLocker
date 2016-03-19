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
    public sealed class CredentialHandler : IDisposable
    {
        int keySize;
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        SymmetricCipher<AesManaged> cipher;
        public byte[] Key { get { return cipher.Key; } }
        public byte[] IV { get { return cipher.IV; } }
        public CredentialHandler(int keySize)
        {
            this.keySize = keySize;
            HttpOnly = true;
            Secure = true;
            cipher = new SymmetricCipher<AesManaged>(keySize);
        }
        public CredentialHandler(byte[] key, byte[] iv)
        {
            cipher = new SymmetricCipher<AesManaged>(key, iv);
            this.keySize = cipher.KeySize;
            HttpOnly = true;
            Secure = true;
        }
        public CredentialHandler(HttpSessionStateBase Session)
            : this(Session["CookieKey"] as byte[], Session["CookieIV"] as byte[])
        {   }
        ~CredentialHandler()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Inject(byte[] plain, HttpSessionStateBase Session , out HttpCookie keypartCookie) 
        {
            //1. Encrypt plain priv key
            byte[] encPriKey = cipher.Encrypt(plain);
            //2. Extract key and iv
            byte[] key = cipher.Key;
            byte[] iv = cipher.IV;
            //3. Split encrypted privKey
            byte[] Pa;
            byte[] Pb;
            Pa = encPriKey.Take((encPriKey.Length / 2)).ToArray();
            Pb = encPriKey.Skip((encPriKey.Length / 2)).ToArray();
            //4. Take last n bytes of Pa as new key
            byte[] a = Pa.Skip(Pa.Length - (keySize / 8)).ToArray();
            //5. Save Pa in cookie of user
            keypartCookie = new HttpCookie("Secret");
            keypartCookie.Value = Convert.ToBase64String(Pa);
            keypartCookie.HttpOnly = true;
            keypartCookie.Secure = true;
            //5. Initialize 2nd cipher to encrypt Pb for session storage
            using (var cookieCipher = new SymmetricCipher<AesManaged>(a, iv))
            {
                Session["AccKeyPart"] = cookieCipher.Encrypt(Pb);
                Session["CookieKey"] = key;
                Session["CookieIV"] = iv;
            }
        }

        public byte[] Extract(HttpCookie keypartCookie, HttpSessionStateBase Session)
        {
            // 1. Get session key and iv
            byte[] rKey = Session["CookieKey"] as byte[];
            byte[] rIV = Session["CookieIV"] as byte[];

            // 2. Get user part of key
            HttpCookie receivedCookie = keypartCookie; //Request.Cookies["Secret"];
            byte[] rPa = Convert.FromBase64String(receivedCookie.Value);
            // 3. Get key extracted from user cookie key
            byte[] ra = rPa.Skip(rPa.Length - (keySize / 8)).ToArray();
            // 4. Initialize cipher with extracted keypart and cookieIV
            using (var cookieCipher = new SymmetricCipher<AesManaged>(ra, rIV))
            {
                // 5. Get server side second key part and decrypt it
                byte[] encAccKeyPart = Session["AccKeyPart"] as byte[];
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
                keySize = 0;
                cipher.Dispose();
            }
        }
    }
}
