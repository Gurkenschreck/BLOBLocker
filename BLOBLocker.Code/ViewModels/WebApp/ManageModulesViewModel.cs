using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class ManageModulesViewModel
    {
        [Required]
        public string PUID { get; set; }
        [Required]
        [LocalizedDisplayName("Pool.IsChatEnabled")]
        public bool EnableChat { get; set; }
        [Required]
        [LocalizedDisplayName("Pool.IsStorageEnabled")]
        public bool EnableFileStorage { get; set; }
        [Required]
        [LocalizedDisplayName("Pool.IsLinkRepositoryEnabled")]
        public bool EnableLinkRepository { get; set; }
    }
}
