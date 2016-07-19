using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class ClosePoolViewModel
    {
        [LocalizedDisplayName("Pool.PUID")]
        [Required]
        public string PUID { get; set; }
        [LocalizedDisplayName("Pool.Title")]
        [Required]
        public string TitleConfirmation { get; set; }    
    }
}
