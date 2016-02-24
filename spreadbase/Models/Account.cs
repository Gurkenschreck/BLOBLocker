using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public class Account
    {
        [Key]
        public int ID { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }

        public virtual CryptoConfig Config { get; set; }
        public virtual AccountAddition Addition { get; set; }
    }
}