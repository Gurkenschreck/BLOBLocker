using BLOBLocker.Entities.Models.WebApp;
using Cipha.Security.Cryptography;
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
            using (var symC = new SymmetricCipher<AesManaged>(password, out salt, out iv, properties.SaltByteLength, newConfig.KeySize, 0, newConfig.IterationCount))
            {
                newConfig.IV = iv;

                using (var rsaC = new RSACipher<RSACryptoServiceProvider>(newConfig.RSAKeySize))
                {
                    newConfig.PublicKey = rsaC.ToXmlString(false);
                    newConfig.PrivateKey = Convert.FromBase64String(rsaC.ToEncryptedXmlString<AesManaged>(true, symC.Key, symC.IV));
                    newConfig.PublicKeySignature = rsaC.SignStringToString<SHA256Cng>(newConfig.PublicKey);
                }

                newConfig.Key = symC.Encrypt(Utilities.GenerateBytes(32));
            }
            return newConfig;
        }

        public bool ExchangeSymmetricKey(string currentPassword, string newPassword, byte[] currentSalt, CryptoConfiguration configToChange, CryptoConfigProperties properties, out byte[] salt)
        {
            using (var oldCipher = new SymmetricCipher<AesManaged>(currentPassword,
                currentSalt, configToChange.IV, iterations: configToChange.IterationCount))
            {
                byte[] plainCurrentPriRSAKey = null;
                try
                {
                    plainCurrentPriRSAKey = oldCipher.Decrypt(configToChange.PrivateKey);
                }
                catch (CryptographicException)
                {
                    salt = null;
                    return false;
                }

                byte[] newSalt;
                byte[] newIV;
                using (var newCipher = new SymmetricCipher<AesManaged>(newPassword,
                    out newSalt, out newIV, properties.SaltByteLength, properties.SymmetricKeySize, iterations: properties.HashIterations))
                {
                    salt = newSalt;
                    configToChange.IterationCount = properties.HashIterations;
                    configToChange.IV = newIV;
                    configToChange.Key = newCipher.Encrypt(Utilities.GenerateBytes(32));

                    configToChange.PrivateKey = newCipher.Encrypt(plainCurrentPriRSAKey);
                }
            }
            return true;
        }
    }
}
