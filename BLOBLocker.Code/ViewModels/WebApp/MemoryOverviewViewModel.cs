using BLOBLocker.Entities.Models.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class MemoryOverviewViewModel
    {
        public string PUID { get; set; }
        public ICollection<AssignedMemory> Memory { get; set; }
        public int Rights { get; set; }
    }
}
