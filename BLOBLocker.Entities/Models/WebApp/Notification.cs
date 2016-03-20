using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Entities.Models.WebApp
{
    public class Notification
    {
        public Notification()
        {
            CreatedOn = DateTime.Now;
            IsVisible = true;
        }

        public int ID { get; set; }
        [Display(Name = "Notification_Description",
            ResourceType = typeof(Resources.Models))]
        public string Description { get; set; }
        [Display(Name = "Notification_CreatedOn",
            ResourceType = typeof(Resources.Models))]
        public Nullable<DateTime> CreatedOn { get; set; }
        public bool IsVisible { get; set; }
    }
}