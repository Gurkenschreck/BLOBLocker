using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class Account
    {
        public Account()
        {
        }

        [Key]
        public int ID { get; set; }
        public string Alias { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public byte[] Salt { get; set; }

        public virtual CryptoConfig Config { get; set; }
        public virtual AccountAddition Addition { get; set; }
        public virtual ICollection<AccountRoleLink> Roles { get; set; }
    }
}