using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Symmetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Data
{
    public class CryptoConfigHandler
    {
        public class CryptoConfigProperties
        {
            public int SaltByteLength { get; set; }
            public int SymmetricKeySize { get; set; }
            public int RSAKeySize { get; set; }
            public int HashIterations { get; set; }
        }

        public CryptoConfiguration SetupCryptoConfig(string password, CryptoConfigProperties properties, out byte[] salt)
        {
            CryptoConfiguration newConfig = new CryptoConfiguration();

            newConfig.IterationCount = properties.HashIterations;
            newConfig.KeySize = properties.SymmetricKeySize;
            newConfig.RSAKeySize = properties.RSAKeySize;

            byte[] iv;
            using (var symC = new SymmetricCipher<AesManaged>(password, out salt, out iv, keySize: newConfig.KeySize, iterations: newConfig.IterationCount))
            {
                newConfig.IV = iv;

                using (var rsaC = new RSACipher<RSACryptoServiceProvider>(newConfig.RSAKeySize))
                {
                    newConfig.PublicKey = rsaC.ToXmlString(false);
                    newConfig.PrivateKey = Convert.FromBase64String(rsaC.ToEncryptedXmlString<AesManaged>(true, symC.Key, symC.IV));
                    newConfig.PublicKeySignature = rsaC.SignStringToString<SHA256Cng>(newConfig.PublicKey);
                }
            }
            return newConfig;
        }
    }
}
