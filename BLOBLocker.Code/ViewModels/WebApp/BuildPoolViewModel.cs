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
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name="Is filestorage enabled")]
        public bool IsFileStorageEnabled { get; set; }
        [Display(Name = "Is chat enabled")]
        public bool IsChatEnabled { get; set; }
        [Display(Name = "Is Linkrepo enabled")]
        public bool IsLinkRepositoryEnabled { get; set; }
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