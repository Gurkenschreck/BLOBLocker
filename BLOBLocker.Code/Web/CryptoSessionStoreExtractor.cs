using BLOBLocker.Code.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLOBLocker.Code.Web
{
    public sealed class CryptoSessionStoreExtractor
    {
        HttpSessionStateBase session;
        HttpRequestBase request;

        public CryptoSessionStoreExtractor(HttpSessionStateBase session,
            HttpRequestBase request)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            if (request == null)
                throw new ArgumentNullException("request");
            this.session = session;
            this.request = request;
        }

        public CryptoKeyInformation GetCryptoKeyInformation(string storeName)
        {
            byte[] storeSessionKey = session[storeName + "_SessionKey"] as byte[];
            byte[] storeSessionIV = session[storeName + "_SessionIV"] as byte[];
            byte[] storeSessionStoredKeyPart = session[storeName + "_SessionStoredKeyPart"] as byte[];
            HttpCookie cryptoCookie = request.Cookies[storeName + "_Crypto"];

            return new CryptoKeyInformation(Convert.FromBase64String(cryptoCookie.Value),
                storeSessionKey, storeSessionIV, storeSessionStoredKeyPart);
        }
    }
}
