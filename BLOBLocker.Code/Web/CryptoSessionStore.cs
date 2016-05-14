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
    
    public sealed class CryptoSessionStore : IDisposable
    {
        public const string _divider = "__";
        public const string _sessionKeyLabel = "SessionKey";
        public const string _sessionIVLabel = "SessionIV";
        public const string _cookieLabel = "Crypto";
        public const string _encryptedStoreKeyPart = "EncStoreKeyPart";
        public const string _encryptedStorageIV = "EncStoreIV";

        string storeName;
        bool cookieHttpOnly = true;
        bool cookieSecure = true;
        HttpSessionStateBase session;
        HttpRequestBase request;
        HttpResponseBase response;
        SymmetricCipher<AesManaged> storeCipher;
        SessionStoreCredentials sessionStoreCredentials;
        int initializeKeySize;

        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }

        public bool CookieHttpOnly
        {
            get { return cookieHttpOnly; }
            set { cookieHttpOnly = value; }
        }

        public bool CookieSecure
        {
            get { return cookieSecure; }
            set { cookieSecure = value; }
        }

        public string DataStorageLabelPart
        {
            get
            {
                return storeName + _divider + "data" + _divider;
            }
        }
        public string SessionKeyLabel
        {
            get
            {
                return storeName + _divider + _sessionKeyLabel;
            }
        }
        public string SessionIVLabel
        {
            get
            {
                return storeName + _divider + _sessionIVLabel;
            }
        }

        public string CookieKeyPartLabel
        {
            get
            {
                return storeName + _divider + _cookieLabel;
            }
        }
        public string EncryptedStoreKeyPartLabel
        {
            get
            {
                return storeName + _divider + _encryptedStoreKeyPart;
            }
        }

        public string EncryptedStoreIVLabel
        {
            get
            {
                return storeName + _divider + _encryptedStorageIV;
            }
        }

        bool StoreExists
        {
            get
            {
                bool storeExists = false;

                storeExists = (request.Cookies[CookieKeyPartLabel] != null)
                    && session[EncryptedStoreKeyPartLabel] != null
                    && session[EncryptedStoreIVLabel] != null
                    && session[SessionKeyLabel] != null
                    && session[SessionIVLabel] != null;

                return storeExists;
            }
        }

        public byte[] this[string name]
        {
            get 
            {
                if (session[DataStorageLabelPart + name] != null)
                {
                    return storeCipher.Decrypt((byte[])session[DataStorageLabelPart + name]);
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                if (value != null)
                    session[DataStorageLabelPart + name] = storeCipher.Encrypt(value);
                else
                    session[DataStorageLabelPart + name] = null;
            }
        }

        public CryptoSessionStore(string storeName,
            HttpSessionStateBase session, HttpRequestBase request, HttpResponseBase response)
            : this(storeName, 256, session, request, response) { }

        public CryptoSessionStore(string storeName, int keySize,
            HttpSessionStateBase session, HttpRequestBase request, HttpResponseBase response)
        {
            if (string.IsNullOrWhiteSpace(storeName))
                throw new ArgumentNullException("storeName");
            if (session == null)
                throw new ArgumentNullException("session");
            if (request == null)
                throw new ArgumentNullException("request");
            if (response == null)
                throw new ArgumentNullException("response");

            this.storeName = storeName;
            initializeKeySize = keySize;
            this.session = session;
            this.request = request;
            this.response = response;

            initialize();
        }
        ~CryptoSessionStore()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        void initialize()
        {
            if (StoreExists)
            {
                extractCryptoSessionStore();
                initializeExistingCipher();
            }
            else
            {
                initializeNewCipher(initializeKeySize);
                injectCryptoSessionStore();
            }
        }

        void extractCryptoSessionStore()
        {
            byte[] encryptedStoreKeyPart = session[EncryptedStoreKeyPartLabel] as byte[];
            byte[] encryptedStoreIV = session[EncryptedStoreKeyPartLabel] as byte[];
            byte[] storeSessionKey = session[SessionKeyLabel] as byte[];
            byte[] storeSessionIV = session[SessionIVLabel] as byte[];
            HttpCookie cryptoCookie = request.Cookies[CookieKeyPartLabel];

            sessionStoreCredentials = new SessionStoreCredentials(Convert.FromBase64String(cryptoCookie.Value),
                encryptedStoreKeyPart, encryptedStoreIV,
                storeSessionKey, storeSessionIV);
        }

        void injectCryptoSessionStore()
        {
            session[EncryptedStoreKeyPartLabel] = sessionStoreCredentials.EncryptedStoreKeyPart.Clone() as byte[];
            session[EncryptedStoreIVLabel] = sessionStoreCredentials.EncryptedStoreIV.Clone() as byte[];
            session[SessionKeyLabel] = sessionStoreCredentials.Key.Clone() as byte[];
            session[SessionIVLabel] = sessionStoreCredentials.IV.Clone() as byte[];
            HttpCookie cryptoCookie = new HttpCookie(CookieKeyPartLabel,
                Convert.ToBase64String(sessionStoreCredentials.CookieKeyPart));

            cryptoCookie.HttpOnly = cookieHttpOnly;
            cryptoCookie.Secure = cookieSecure;
            response.SetCookie(cryptoCookie);
        }

        void initializeExistingCipher()
        {
            if (sessionStoreCredentials == null)
                throw new InvalidOperationException("sessionStoreCredentials needs to be initialized first");

            using(var cookieKeyPartCipher = new SymmetricCipher<AesManaged>(sessionStoreCredentials.CookieKeyPart,
                sessionStoreCredentials.IV))
            {
                byte[] plainSessionKeyPart = cookieKeyPartCipher.Decrypt(sessionStoreCredentials.EncryptedStoreKeyPart);

                byte[] cookieKeyPart = sessionStoreCredentials.CookieKeyPart;

                byte[] encryptedSessionStoreKey = new byte[cookieKeyPart.Length 
                    + plainSessionKeyPart.Length];
                Array.Copy(cookieKeyPart, encryptedSessionStoreKey, cookieKeyPart.Length);
                Array.Copy(plainSessionKeyPart, 0, encryptedSessionStoreKey, cookieKeyPart.Length, plainSessionKeyPart.Length);

                using (var sessionKeyCipher = new SymmetricCipher<AesManaged>(sessionStoreCredentials.Key,
                    sessionStoreCredentials.IV))
                {
                    byte[] a = sessionKeyCipher.Decrypt(encryptedSessionStoreKey);
                    byte[] b = sessionKeyCipher.Decrypt(sessionStoreCredentials.EncryptedStoreIV); // makes probs
                    storeCipher = new SymmetricCipher<AesManaged>(a, b);                    
                }
                Utilities.SetArrayValuesZero(encryptedSessionStoreKey);
            }
        }

        void initializeNewCipher(int keySize)
        {
            storeCipher = new SymmetricCipher<AesManaged>(keySize);

            // 1. Initialize SessionKeyCipher
            using (var sessionKeyCipher = new SymmetricCipher<AesManaged>(keySize))
            {
                // 2. 
                byte[] plainStoreCipherKey = storeCipher.Key;
                byte[] plainStoreCipherIV = storeCipher.IV;

                // 3. Encrypt 
                byte[] encryptedStoreCipherKey = sessionKeyCipher.Encrypt(plainStoreCipherKey);
                byte[] encryptedStoreCipherIV = sessionKeyCipher.Encrypt(plainStoreCipherIV);

                byte[] cookieKeyPart = encryptedStoreCipherKey.Take(sessionKeyCipher.KeySize / 8).ToArray();
                byte[] plainSessionKeyPart = encryptedStoreCipherKey.Skip(sessionKeyCipher.KeySize / 8).ToArray();
                Utilities.SetArrayValuesZero(encryptedStoreCipherKey);
                byte[] encryptedSessionKeyPart;
                using (var cookieKeyPartCipher = new SymmetricCipher<AesManaged>(cookieKeyPart, sessionKeyCipher.IV))
                {
                    encryptedSessionKeyPart = cookieKeyPartCipher.Encrypt(plainSessionKeyPart);
                }

                sessionStoreCredentials = new SessionStoreCredentials(cookieKeyPart,
                    encryptedSessionKeyPart, encryptedStoreCipherIV,
                    sessionKeyCipher.Key, sessionKeyCipher.IV);
                Utilities.SetArrayValuesZero(cookieKeyPart);
                Utilities.SetArrayValuesZero(encryptedStoreCipherIV);
                Utilities.SetArrayValuesZero(plainSessionKeyPart);
                Utilities.SetArrayValuesZero(encryptedSessionKeyPart);
            }
        }

        public void WipeSessionStore()
        {
            foreach (string key in session.Keys)
            {
                if (key.Contains(storeName + _divider))
                {
                    if (session[key] is byte[])
                    {
                        Utilities.SetArrayValuesZero((byte[])session[key]);
                    }
                }
            }
        }
        public static void WipeAllStores(HttpSessionStateBase session)
        {
            foreach (string key in session.Keys)
            {
                if (key.Contains(_divider))
                {
                    if (session[key] is byte[])
                    {
                        Utilities.SetArrayValuesZero((byte[])session[key]);
                    }
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
                storeCipher.Dispose();
                sessionStoreCredentials.Dispose();
            }
        }
    }
}
