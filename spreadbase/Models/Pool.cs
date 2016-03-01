using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class Pool
    {
        public Pool()
        {
            Created = DateTime.Now;
            IsActive = true;
        }
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> Created { get; set; }
        public string PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] Salt { get; set; }
        public virtual Account Owner { get; set; }
        public virtual ICollection<Account> Participants { get; set; }
        public bool IsActive { get; set; }
    }
}