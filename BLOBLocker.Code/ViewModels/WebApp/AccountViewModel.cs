using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class AccountViewModel : IValidatableObject
    {
        [Required]
        [Display(Name="Account_Alias",
            ResourceType=typeof(BLOBLocker.Entities.Models.Resources.Models))]
        public string Alias { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Account_Password",
            ResourceType = typeof(BLOBLocker.Entities.Models.Resources.Models))]
        public string Password { get; set; }
        [Display(Name = "AccountAddition_ContactEmail",
            ResourceType = typeof(BLOBLocker.Entities.Models.Resources.Models))]
        public string ContactEmail { get; set; }

        public string RegistrationCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Alias))
                yield return new ValidationResult("Alias cannot be empty or white string", new[] { "Alias" });

            if(string.IsNullOrEmpty(Password))
                yield return new ValidationResult("Password cannot be empty", new[] { "Password" });
        }
    }
}