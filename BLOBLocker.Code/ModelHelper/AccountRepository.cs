using Cipha.Security.Cryptography.Asymmetric;
using Cipha.Security.Cryptography.Hash;
using Cipha.Security.Cryptography.Symmetric;
using BLOBLocker.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BLOBLocker.Entities.Models.WebApp;
using BLOBLocker.Entities.Models.Models.WebApp;

namespace BLOBLocker.Code.ModelHelper
{
    public class AccountRepository : Repository
    {
        public AccountRepository(BLWAContext context)
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

        public bool IsInRole(Account acc, string roleName)
        {
            return acc.Roles.Any(p => p.Role.Definition == roleName);
        }

        public Account GetAccount(string name)
        {
            return context.Accounts.FirstOrDefault(p => p.Alias == name);
        }
        public Account CreateNew(string alias, string password, string contactEmail, int basicSpace, BLOBLocker.Code.ModelHelper.CryptoConfigRepository.Config cryptoConf)
        {
            Account acc = new Account();
            acc.Alias = alias;
            acc.Addition = new AccountAddition();
            acc.Roles = new List<AccountRoleLink>();
            acc.MemoryPool = new MemoryPool();
            acc.MemoryPool.BasicSpace = basicSpace;
            acc.MemoryPool.AdditionalSpace = 0;
            acc.MemoryPool.Owner = acc;
            acc.Addition.Contacts = new List<Contact>();
            acc.Addition.Notifications = new List<Notification>();
            acc.Addition.ContactEmail = contactEmail;
            acc.Addition.LastLogin = DateTime.Now;
            
            CryptoConfigRepository configRepo = new CryptoConfigRepository(context);
            byte[] salt;
            CryptoConfiguration config = configRepo.CreateNew(cryptoConf, out salt);
            acc.Salt = salt;
            acc.Config = config;

            /*using (var symCipher = new SymmetricCipher<AesManaged>(password, salt, config.IV))
            {
                using (var hasher = new Hasher<SHA384Cng>())
                {
                    acc.Password = symCipher.EncryptToString(hasher.Hash(password));
                }
            }*/

            return acc;
        }
        public bool HasPoolRights(Account acc, Pool pool)
        {
            return (acc.PoolShares.Any(p => p.PoolID == pool.ID && p.IsActive));
        }
        public void MemoryLeft(Account acc, out int basic, out int additional)
        {
            basic =  acc.MemoryPool.BasicSpace - acc.MemoryPool.AssignedMemory
                                                           .Where(p => p.IsBasic && p.IsEnabled )
                                                           .Select(p => p.Space)
                                                           .Sum();

            additional = acc.MemoryPool.AdditionalSpace - acc.MemoryPool.AssignedMemory
                                                               .Where(p => !p.IsBasic && p.IsEnabled)
                                                               .Select(p => p.Space)
                                                               .Sum();
        }
    }
}
