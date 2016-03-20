using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ModelHelper
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
