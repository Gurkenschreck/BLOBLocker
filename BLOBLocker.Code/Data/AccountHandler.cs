using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLOBLocker.Code.Data;
using BLOBLocker.Entities.Models.Models.WebApp;
using Cipha.Security.Cryptography.Symmetric;
using System.Security.Cryptography;
using Cipha.Security.Cryptography;
using System.Web;

namespace BLOBLocker.Code.Data
{
    public class AccountHandler
    {
        public class AccountProperties
        {
            public ICollection<AccountRole> Roles { get; set; }
            public string Alias { get; set; }
            public string Password { get; set; }
        }

        public Account SetupAccount(AccountProperties accProps,
            CryptoConfigHandler.CryptoConfigProperties cryptoProps,
            AccountAdditionHandler.AccountAdditionProperties additionProps,
            MemoryPoolHandler.MemoryPoolProperties memPoolProps)
        {
            Account newAcc = new Account();
            newAcc.Alias = accProps.Alias;
            foreach (var role in accProps.Roles)
            {
                newAcc.Roles.Add(new AccountRoleLink
                {
                    Account = newAcc,
                    Role = role
                });
            }

            AccountAdditionHandler accAddHandler = new AccountAdditionHandler();
            newAcc.Addition = accAddHandler.SetupNew(additionProps);

            MemoryPoolHandler memPoolHandler = new MemoryPoolHandler(newAcc);
            newAcc.MemoryPool = memPoolHandler.SetupNew(memPoolProps);

            byte[] salt;
            CryptoConfigHandler cryptoConfigHandler = new CryptoConfigHandler();
            newAcc.Config = cryptoConfigHandler.SetupCryptoConfig(accProps.Password, cryptoProps, out salt);
            newAcc.Salt = salt;

            return newAcc;
        }

        /// <summary>
        /// Logs an account in. Returns a number to identify probable problems.
        /// 
        /// 1: Login success
        /// 2: False password
        /// 3: Account disabled
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Login(Account account, string password, out byte[] priRSAKey)
        {
            priRSAKey = null;
            if (account.IsEnabled)
            {
                using (var symC = new SymmetricCipher<AesManaged>(password, account.Salt, account.Config.IV, iterations: account.Config.IterationCount))
                {
                    try
                    {
                        priRSAKey = symC.Decrypt(account.Config.PrivateKey);
                        account.Addition.LastLogin = DateTime.Now;
                    }
                    catch (CryptographicException)
                    {
                        account.Addition.LastFailedLogin = DateTime.Now;
                        BLOBLocker.Code.ModelHelper.NotificationHelper.SendNotification(account,
                                    HttpContext.GetGlobalResourceObject(null, "b").ToString(),
                                    DateTime.Now);
                        return 2;
                    }
                }
            }
            else
            {
                return 3;
            }

            return 1;
        }
    }
}
