using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.AdminTool
{
    public class Account
    {
        [Key]
        public int ID { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public byte[] DerivedPassword { get; set; }
        public virtual ICollection<RoleLink> Roles { get; set; }
        public Nullable<DateTime> LastLogin { get; set; }
        public byte[] Salt { get; set; }
        public bool IsActive { get; set; }
    }
}