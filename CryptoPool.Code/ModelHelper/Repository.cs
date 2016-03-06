using CryptoPool.Entities.Models;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPool.Code.ModelHelper
{
    public abstract class Repository
    {
        protected CryptoPoolContext context;
        public Repository(CryptoPoolContext context)
        {
            this.context = context;
        }
    }
}
