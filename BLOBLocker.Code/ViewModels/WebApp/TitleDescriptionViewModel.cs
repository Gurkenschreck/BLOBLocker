using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class TitleDescriptionViewModel
    {
        [Required]
        [LocalizedDisplayName("Pool.PUID")]
        public string PUID { get; set; }
        [Required]
        [LocalizedDisplayName("Pool.Title")]
        public string Title { get; set; }
        [LocalizedDisplayName("Pool.Description")]
        public string Description { get; set; }
        [Required]
        public int Rights { get; set; }
    }
}
