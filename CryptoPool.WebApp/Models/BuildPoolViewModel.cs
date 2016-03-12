using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoPool.WebApp.Models
{
    public class BuildPoolViewModel
    {
        [Required]
        [MaxLength(64, ErrorMessage="Description too long.")]
        public string Description { get; set; }
        public bool IsFileStorageEnabled { get; set; }
        public bool IsChatEnabled { get; set; }
        public bool IsLinkRepositoryEnabled { get; set; }

        public Pool Generate()
        {
            Pool p = new Pool();
            p.Description = Description;
            p.ChatEnabled = IsChatEnabled;
            p.FileStorageEnabled = IsFileStorageEnabled;
            p.LinkRepositoryEnabled = IsLinkRepositoryEnabled;
            return p;
        }
    }
}