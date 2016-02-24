using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public enum AccountType : byte
    {
        Standard = 0,
        Upgraded,
        Administrator
    }
}