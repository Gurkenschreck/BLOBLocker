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

        public Account GetAccount(string name)
        {
            return context.Accounts.FirstOrDefault(p => p.Alias == name);
        }
        public bool HasPoolRights(Account acc, Pool pool)
        {
            return (acc.PoolShares.Any(p => p.PoolID == pool.ID && p.IsActive));
        }
    }
}
