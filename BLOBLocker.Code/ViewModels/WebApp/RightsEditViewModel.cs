using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class RightsEditViewModel
    {
        [LocalizedDisplayName("Pool.PUID")]
        public string PoolUID { get; set; }
        [LocalizedDisplayName("Pool.Rights")]
        public ICollection<PoolRightViewModel> Rights { get; set; }
    }
}
