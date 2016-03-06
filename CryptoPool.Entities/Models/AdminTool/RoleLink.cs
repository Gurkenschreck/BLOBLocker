using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.AdminTool
{
    public class RoleLink
    {
        [Key]
        public int ID { get; set; }
        public virtual Account Account { get; set; }
        public virtual Role Role { get; set; }
    }
}