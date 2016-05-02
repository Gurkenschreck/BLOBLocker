using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class TitleDescriptionViewModel
    {
        [LocalizedDisplayName("Pool.PUID")]
        public string PUID { get; set; }
        [LocalizedDisplayName("Pool.Title")]
        public string Title { get; set; }
        [LocalizedDisplayName("Pool.Description")]
        public string Description { get; set; }
    }
}
