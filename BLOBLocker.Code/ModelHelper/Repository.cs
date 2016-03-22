using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ModelHelper
{
    public abstract class Repository
    {
        protected BLWAContext context;
        public Repository(BLWAContext context)
        {
            this.context = context;
        }
    }
}
