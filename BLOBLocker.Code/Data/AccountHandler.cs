using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLOBLocker.Code.Data;
using BLOBLocker.Entities.Models.Models.WebApp;

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

            MemoryPoolHandler memPoolHandler = new MemoryPoolHandler();
            newAcc.MemoryPool = memPoolHandler.SetupNew(newAcc, memPoolProps);

            byte[] salt;
            CryptoConfigHandler cryptoConfigHandler = new CryptoConfigHandler();
            newAcc.Config = cryptoConfigHandler.SetupCryptoConfig(accProps.Password, cryptoProps, out salt);
            newAcc.Salt = salt;

            return newAcc;
        }
    }
}
