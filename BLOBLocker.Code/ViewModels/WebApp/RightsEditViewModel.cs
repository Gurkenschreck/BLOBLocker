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
        public string PoolUID { get; set; }
        public ICollection<PoolRightViewModel> Rights { get; set; }
    }
}
