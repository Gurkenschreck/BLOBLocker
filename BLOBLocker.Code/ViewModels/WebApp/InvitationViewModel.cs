using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [LocalizedDisplayName("Pool.PUID")]
        [Required]
        public string PoolUID { get; set; }

        [LocalizedDisplayName("Invite.InvitationAlias")]
        [Required]
        public string InviteAlias { get; set; }

        [LocalizedDisplayName("Invite.ShowMessagesSince")]
        public Nullable<DateTime> ShowSince { get; set; }
        [LocalizedDisplayName("Invite.ShowAllMessages")]
        public bool ShowAll { get; set; } 
    }
}