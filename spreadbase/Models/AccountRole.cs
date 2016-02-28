using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class AccountRole
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
    }

    public class AccountRoleLink
    {
        public int ID { get; set; }
        public virtual Account Account { get; set; }
        public virtual AccountRole Role { get; set; }
    }
}