using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BLOBLocker.Code.Extention;

namespace BLOBLocker.Code.ViewModels.WebApp
{
    public class ChangePasswordViewModel : IValidatableObject
    {
        [Required]
        [LocalizedDisplayName("Account.CurrentPassword")]
        public string CurrentPassword { get; set; }
        [Required]
        [LocalizedDisplayName("Account.NewPassword")]
        public string NewPassword { get; set; }
        [Required]
        [LocalizedDisplayName("Account.ConfirmNewPassword")]
        public string NewPasswordConfirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NewPassword.Equals(NewPasswordConfirm))
                yield return new ValidationResult(HttpContext.GetGlobalResourceObject(null, "Account.PasswordsDoNotMatch").As<string>(), new [] { "NewPassword" });
        }
    }
}
