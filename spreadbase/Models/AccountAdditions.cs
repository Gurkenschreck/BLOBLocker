using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public class AccountAdditions
    {
        [Key, ForeignKey("Account")]
        public int ID { get; set; }
        public virtual Account Account { get; set; }
        [MaxLength(32)]
        public string ContactEmail { get; set; }
        public virtual List<Account> Contacts { get; set; }


        public Nullable<DateTime> LastLogin { get; set; }
        public Nullable<DateTime> LastFailedLogin { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
    }
}