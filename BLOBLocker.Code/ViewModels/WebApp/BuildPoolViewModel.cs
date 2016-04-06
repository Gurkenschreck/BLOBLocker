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
        [MaxLength(64, ErrorMessage="Title too long.")]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsFileStorageEnabled { get; set; }
        public bool IsChatEnabled { get; set; }
        public bool IsLinkRepositoryEnabled { get; set; }

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