using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class InvitationViewModel 
    {
        public InvitationViewModel()
        {
            ShowAll = true;
        }

        public string PoolUID { get; set; }
        [Required]

        public string InviteAlias { get; set; }

        public Nullable<DateTime> ShowSince { get; set; }
        public bool ShowAll { get; set; } 
    }
}