using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class MessageViewModel
    {
        [Required]
        public string PUID { get; set; }
        [Required]
        public string MessageText { get; set; }
    }
}