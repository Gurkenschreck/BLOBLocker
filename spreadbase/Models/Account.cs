using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public class Account
    {
        [Key]
        public int ID { get; set; }
        private bool isActivated = true;
        public bool IsActivated { get { return isActivated; } set { isActivated = value; } }
        [MaxLength(32)]
        public string Alias { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
        public virtual CryptoConfig Config { get; set; }
        public virtual AccountAdditions Additions { get; set; }

        private AccountType type = AccountType.Standard;
        public AccountType Type 
        {
            get { return type; }
            set { type = value; }
        }
    }
}