using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Membership;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class BuildPoolViewModel
    {
        [Required]
        [MinLength(1, ErrorMessage="Title cannot be empty.")]
        [MaxLength(64, ErrorMessage="Title too long.")]
        [LocalizedDisplayName("Pool.Title")]
        public string Title { get; set; }
        [LocalizedDisplayName("Pool.Description")]
        public string Description { get; set; }
        [LocalizedDisplayName("Pool.IsStorageEnabled")]
        public bool IsFileStorageEnabled { get; set; }
        [LocalizedDisplayName("Pool.IsChatEnabled")]
        public bool IsChatEnabled { get; set; }
        [LocalizedDisplayName("Pool.IsLinkRepositoryEnabled")]
        public bool IsLinkRepositoryEnabled { get; set; }
        [LocalizedDisplayName("Pool.Rights")]
        public ICollection<PoolRightViewModel> Rights { get; set; }

        public Pool Generate()
        {
            Pool p = new Pool();
            p.Title = Title;
            p.ChatEnabled = IsChatEnabled;
            p.FileStorageEnabled = IsFileStorageEnabled;
            p.LinkRepositoryEnabled = IsLinkRepositoryEnabled;
            return p;
        }
    }
}