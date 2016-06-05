using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BLOBLocker.Code.Extention;
using BLOBLocker.Code.Resources;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class AccountViewModel : IValidatableObject
    {
        [Required]
        [LocalizedDisplayName("Account.Alias")]
        public string Alias { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Account.Password")]
        public string Password { get; set; }
        [Required]
        [LocalizedDisplayName("Account.ConfirmNewPassword")]
        public string ConfirmPassword { get; set; }
        [EmailAddress]
        [LocalizedDisplayName("Account.Email")]
        public string ContactEmail { get; set; }
        [LocalizedDisplayName("Account.RegistrationCode")]
        public string RegistrationCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Alias))
                yield return new ValidationResult(HttpContext.GetGlobalResourceObject(null, "Account.EmptyAliasError").As<string>(), new[] { "Alias" });
            
            if(string.IsNullOrEmpty(Password))
                yield return new ValidationResult(HttpContext.GetGlobalResourceObject(null, "Account.EmptyPasswordError").As<string>(), new[] { "Password" });

            if (!Password.Equals(ConfirmPassword))
                yield return new ValidationResult(HttpContext.GetGlobalResourceObject(null, "Account.PasswordsDoNotMatch").As<string>(), new[] { "ConfirmPassword" });
        }
    }
}