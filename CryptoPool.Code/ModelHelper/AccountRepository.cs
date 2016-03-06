using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using CryptoPool.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoPool.Entities.Models.WebApp;

namespace CryptoPool.Code.ModelHelper
{
    public class AccountRepository : Repository
    {
        public AccountRepository(CryptoPoolContext context)
            : base(context)
        {
        }

        public void AddRole(Account acc, params string[] roleName)
        {
            foreach (string role in roleName)
            {
                acc.Roles.Add(new AccountRoleLink
                {
                    Account = acc,
                    Role = context.AccountRoles.FirstOrDefault(p => p.Definition == role)
                });
            }
        }
        public Account GetAccount(string name)
        {
            return context.Accounts.FirstOrDefault(p => p.Alias == name);
        }
        public Account CreateNew(string alias, string password, string contactEmail, CryptoPool.Code.ModelHelper.CryptoConfigRepository.Config cryptoConf)
        {
            Account acc = new Account();
            acc.Alias = alias;
            acc.Addition = new AccountAddition();
            acc.Roles = new List<AccountRoleLink>();
            acc.Addition.Contacts = new List<Contact>();
            acc.Addition.Notifications = new List<Notification>();
            acc.Addition.ContactEmail = contactEmail;
            acc.Addition.LastLogin = DateTime.Now;
            
            CryptoConfigRepository configRepo = new CryptoConfigRepository(context);
            byte[] salt;
            CryptoConfiguration config = configRepo.CreateNew(cryptoConf, out salt);
            acc.Salt = salt;
            acc.Config = config;

            using (var symCipher = new SymmetricCipher<AesManaged>(password, salt, config.IV))
            {
                using (var hasher = new Hasher<SHA384Cng>())
                {
                    acc.Password = symCipher.EncryptToString(hasher.Hash(password));
                }
            }

            return acc;
        }
    }
}
