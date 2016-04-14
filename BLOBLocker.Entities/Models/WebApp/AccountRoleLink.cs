using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.Models.WebApp
{
    public class AccountRoleLink
    {
        public int ID { get; set; }
        public virtual Account Account { get; set; }
        public virtual AccountRole Role { get; set; }
    }
}
