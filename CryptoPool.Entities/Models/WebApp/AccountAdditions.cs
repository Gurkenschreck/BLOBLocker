using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CryptoPool.Entities.Models.WebApp
{
    public class AccountAddition
    {
        public AccountAddition()
        {
            CreatedOn = DateTime.Now;
        }
        [Key]
        public int ID { get; set; }
        [Display(Name = "AccountAddition_Contacts",
            ResourceType = typeof(Resources.Models))]
        public virtual ICollection<Contact> Contacts { get; set; }
        [Display(Name = "AccountAddition_Notifications",
            ResourceType = typeof(Resources.Models))]
        public virtual ICollection<Notification> Notifications { get; set; }
        [Display(Name = "AccountAddition_ContactEmail",
            ResourceType = typeof(Resources.Models))]
        public string ContactEmail { get; set; }

        [Display(Name = "AccountAddition_LastLogin",
            ResourceType = typeof(Resources.Models))]
        public Nullable<DateTime> LastLogin { get; set; }
        [Display(Name = "AccountAddition_LastFailedLogin",
            ResourceType = typeof(Resources.Models))]
        public Nullable<DateTime> LastFailedLogin { get; set; }
        [Display(Name = "AccountAddition_CreatedOn",
            ResourceType = typeof(Resources.Models))]
        public Nullable<DateTime> CreatedOn { get; set; }
    }
}