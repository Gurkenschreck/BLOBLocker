using BLOBLocker.Entities.Models.Models.WebApp;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class PoolConfigModel
    {
        public bool IsOwner { get; set; }
        public Pool Pool { get; set; }
        public Account Account { get; set; }
        public PoolShare PoolShare { get; set; }
        public AssignedMemory BasicMemory { get; set; }
        public AssignedMemory AdditionalMemory { get; set; }
        public RightsEditViewModel RightsEditViewModel { get; set; }
        public TitleDescriptionViewModel TitleDescriptionViewModel { get; set; }
    }
}