using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Membership
{
    public class PoolRightHelper
    {
        public static bool AccountHasRight(PoolShare share, PoolRight right)
        {
            return ((share.Rights & (int)right) == (int)right);
        }
    }
}
