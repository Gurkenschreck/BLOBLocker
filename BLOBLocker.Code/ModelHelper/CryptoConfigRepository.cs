using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ModelHelper
{
    public class CryptoConfigRepository : Repository
    {
        public class Config
        {
            public string Password { get; set; }
            public int SaltByteLength { get; set; }
            public int SymKeySize { get; set; }
            public int RSAKeySize { get; set; }
            public int HashIterations { get; set; }
        }
        public CryptoConfigRepository(BLWAContext context)
            :base(context)
        {  }

        public CryptoConfiguration CreateNew(Config con, out byte[] salt)
        {
            /*int saltLenght = Convert.ToInt32(con.Application["security.SaltByteLength"]);
            int symKeySize = Convert.ToInt32(con.Application["security.AccountKeySize"]);
            int rsaKeySize = Convert.ToInt32(con.Application["security.AccountRSAKeySize"]);
            int hashIterations = Convert.ToInt32(con.Application["security.HashIterationCount"]);*/
            int saltLength = con.SaltByteLength;
            int symKeySize = con.SymKeySize;
            int rsaKeySize = con.RSAKeySize;
            int hashIterations = con.HashIterations;
            string password = con.Password;

            var cryptoConfig = new CryptoConfiguration();
            cryptoConfig.IterationCount = hashIterations;
            cryptoConfig.KeySize = symKeySize;
            cryptoConfig.RSAKeySize = rsaKeySize;

            byte[] iv;

            using (var symC = new SymmetricCipher<AesManaged>(password, out salt, out iv, keySize: symKeySize, iterations: hashIterations))
            {
                cryptoConfig.IV = iv;

                using (var rsaC = new RSACipher<RSACryptoServiceProvider>(rsaKeySize))
                {
                    cryptoConfig.PublicKey = rsaC.ToXmlString(false);
                    cryptoConfig.PrivateKey = Convert.FromBase64String(rsaC.ToEncryptedXmlString<AesManaged>(true, symC.Key, symC.IV));
                    cryptoConfig.PublicKeySignature = rsaC.SignStringToString<SHA256Cng>(cryptoConfig.PublicKey);
                }
            }
            return cryptoConfig;
        }
    }
}
