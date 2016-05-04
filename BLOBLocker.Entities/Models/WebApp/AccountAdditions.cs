using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class AccountAddition
    {
        public AccountAddition()
        {
            CreatedOn = DateTime.Now;
            Contacts = new List<Contact>();
            Notifications = new List<Notification>();
        }
        [Key]
        public int ID { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public string ContactEmail { get; set; }

        public Nullable<DateTime> LastLogin { get; set; }
        public Nullable<DateTime> LastFailedLogin { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
    }
}