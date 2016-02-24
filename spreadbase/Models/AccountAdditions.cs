using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace spreadbase.Models
{
    public class AccountAddition
    {
        public AccountAddition()
        {
            CreatedOn = DateTime.Now;
            Contacts = new List<Account>();
        }

        public int ID { get; set; }
        public virtual ICollection<Account> Contacts { get; set; }

        public string ContactEmail { get; set; }


        public Nullable<DateTime> LastLogin { get; set; }
        public Nullable<DateTime> LastFailedLogin { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
    }
}