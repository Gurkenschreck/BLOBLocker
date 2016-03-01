using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpreadBase.Models
{
    public class AccountAddition
    {
        public AccountAddition()
        {
            CreatedOn = DateTime.Now;
            Contacts = new List<Account>();
            Notifications = new List<Notification>();
        }
        [Key]
        public int ID { get; set; }
        [Display(Name = "AccountAddition_Contacts",
            ResourceType = typeof(Resources.Models.Models))]
        public virtual ICollection<Account> Contacts { get; set; }
        [Display(Name = "AccountAddition_Notifications",
            ResourceType = typeof(Resources.Models.Models))]
        public virtual ICollection<Notification> Notifications { get; set; }
        [Display(Name = "AccountAddition_ContactEmail",
            ResourceType = typeof(Resources.Models.Models))]
        public string ContactEmail { get; set; }

        [Display(Name = "AccountAddition_LastLogin",
            ResourceType = typeof(Resources.Models.Models))]
        public Nullable<DateTime> LastLogin { get; set; }
        [Display(Name = "AccountAddition_LastFailedLogin",
            ResourceType = typeof(Resources.Models.Models))]
        public Nullable<DateTime> LastFailedLogin { get; set; }
        [Display(Name = "AccountAddition_CreatedOn",
            ResourceType = typeof(Resources.Models.Models))]
        public Nullable<DateTime> CreatedOn { get; set; }
    }
}