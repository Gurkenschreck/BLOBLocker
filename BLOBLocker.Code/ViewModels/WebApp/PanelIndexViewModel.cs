using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class PanelIndexViewModel
    {
        public AccountAddition Addition { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
        public IEnumerable<Pool> Pools { get; set; }
        public IEnumerable<PoolShare> PoolShares { get; set; }
    }
}
