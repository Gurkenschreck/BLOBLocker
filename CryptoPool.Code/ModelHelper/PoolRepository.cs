using CryptoPool.Entities.Models;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPool.Code.ModelHelper
{
    public class PoolRepository : Repository
    {
        public PoolRepository(CryptoPoolContext context)
            : base(context)
        { }

        public Pool CreateNew()
        {
            Pool pool = new Pool();

            context.Pools.Add(pool);
            return pool;
        }
    }
}
