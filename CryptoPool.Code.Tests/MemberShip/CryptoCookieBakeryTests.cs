using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CryptoPool.Code.Membership;
using System.Web;
using System.Security.Cryptography;
using Cipha.Security.Cryptography.Asymmetric;
using System.Collections;
using System.Collections.Generic;

namespace CryptoPool.Code.Tests.MemberShip
{
    [TestClass]
    public class CryptoCookieBakeryTests
    {
        [TestMethod]
        public void CreateCookie_CreateNewCookieWithKey_Pass()
        {
            HttpCookie cryptoCookie;
            string expected;
            string got;
            int cookieKeySize = 256;
            using(var asymCipher = new RSACipher<RSACryptoServiceProvider>(2048))
            {
                expected = asymCipher.ToXmlString(true);
                using(CryptoCookieBakery bakery = new CryptoCookieBakery())
                {
                    cryptoCookie = bakery.CreateCookie("PWCookie",
                        asymCipher.ToXmlString(true),
                        cookieKeySize);
                    got = bakery.GetValue(cryptoCookie, bakery.CookieKey, bakery.CookieIV);
                }
            }
            Assert.AreEqual(expected, got);
        }
        [TestMethod]
        public void GetValue_CreateNewCookieWithKey_Pass()
        {
            HttpCookie cryptoCookie;
            string expected;
            string got;
            int cookieKeySize = 256;
            byte[] cookieKey;
            byte[] cookieIV;
            using (var asymCipher = new RSACipher<RSACryptoServiceProvider>(2048))
            {
                expected = asymCipher.ToXmlString(true);
                using (CryptoCookieBakery bakery = new CryptoCookieBakery())
                {
                    cryptoCookie = bakery.CreateCookie("PWCookie",
                        asymCipher.ToXmlString(true),
                        cookieKeySize);
                    cookieKey = bakery.CookieKey;
                    cookieIV = bakery.CookieIV;
                }
            }
            // Later
            using (var bakery = new CryptoCookieBakery())
            {
                got = bakery.GetValue(cryptoCookie, cookieKey, cookieIV);
            }

            Assert.AreEqual(expected, got);
        }
    }
}
