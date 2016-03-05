using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SpreadBase.App_Code.ModelFactory
{
    public class AccountFactory
    {
        public Account CreateNewAccount(HttpContextBase con, Account provAcc)
        {
            int saltLenght = Convert.ToInt32(con.Application["security.SaltByteLength"]);
            int symKeySize = Convert.ToInt32(con.Application["security.AccountKeySize"]);
            int rsaKeySize = Convert.ToInt32(con.Application["security.AccountRSAKeySize"]);
            int hashIterations = Convert.ToInt32(con.Application["security.HashIterationCount"]);

            Account newAcc = new Account();
            newAcc.Alias = provAcc.Alias;
            newAcc.Roles = new List<AccountRoleLink>();
            newAcc.Addition = new AccountAddition();
            newAcc.Addition.ContactEmail = provAcc.Addition.ContactEmail;
            newAcc.Addition.LastLogin = DateTime.Now;
            newAcc.Addition.Notifications = new List<Notification>();

            string pwHash;
            using (var hasher = new Hasher<SHA384Cng>())
            {
                pwHash = hasher.HashToString(provAcc.Password);
            }

            var cryptoConfig = new SpreadBase.Models.CryptoConfig();
            newAcc.Config = cryptoConfig;
            cryptoConfig.IterationCount = hashIterations;
            cryptoConfig.KeySize = symKeySize;
            cryptoConfig.RSAKeySize = rsaKeySize;

            byte[] salt;
            byte[] iv;

            using (var symC = new SymmetricCipher<AesManaged>(provAcc.Password, out salt, out iv, keySize: symKeySize, iterations: hashIterations))
            {
                newAcc.Salt = salt;
                cryptoConfig.IV = iv;

                using (var rsaC = new RSACipher<RSACryptoServiceProvider>(rsaKeySize))
                {
                    cryptoConfig.PublicKey = rsaC.ToXmlString(false);
                    cryptoConfig.PrivateKey = Convert.FromBase64String(rsaC.ToEncryptedXmlString<AesManaged>(true, provAcc.Password, salt, iv));
                    cryptoConfig.PublicKeySignature = rsaC.SignStringToString<SHA256Cng>(cryptoConfig.PublicKey);
                }
                newAcc.Password = symC.EncryptToString(pwHash);
            }
            return newAcc;
        }
    }
}