using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoPool.WebApp.Models
{
    public class AccountViewModel
    {
        [Required]
        [Display(Name="Account_Alias",
            ResourceType=typeof(CryptoPool.Entities.Models.Resources.Models))]
        public string Alias { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password",
            ResourceType = typeof(CryptoPool.Entities.Models.Resources.Models))]
        public string Password { get; set; }
        [Display(Name = "AccountAddition_ContactEmail",
            ResourceType = typeof(CryptoPool.Entities.Models.Resources.Models))]
        public string ContactEmail { get; set; }

        public string RegistrationCode { get; set; }
    }
}