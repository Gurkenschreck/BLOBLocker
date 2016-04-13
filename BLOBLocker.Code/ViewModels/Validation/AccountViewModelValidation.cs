using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLOBLocker.Code.Extention;
using BLOBLocker.Entities.Models.WebApp;
using BLOBLocker.Code.ViewModels.WebApp;

namespace BLOBLocker.Code.ViewModels.Validation
{
    public class AccountViewModelValidation : IActionModelValidation
    {
        public IEnumerable<ValidationResult> Validate(IDictionary<string, object> settings, IDictionary<string, object> actionParams)
        {
            var acc = actionParams["acc"].As<AccountViewModel>();
            bool enableRegistration = settings["system.EnableRegistration"].As<bool>();
            if (!enableRegistration)
            {
                //yield return new ValidationResult(Resoucres.Account.Strings.RegistrationDisabled);
                yield return new ValidationResult("Registration is currently disabled.");
            }
            bool isRegistrationRestricted = settings["system.RestrictRegistration"].As<bool>();
            if (isRegistrationRestricted)
            {
                string expectedRegistrationCode = settings["system.RestrictedRegistrationCode"].As<string>();
                if (string.IsNullOrEmpty(acc.RegistrationCode) || acc.RegistrationCode != expectedRegistrationCode)
                {
                    //yield return new ValidationResult(Resources.Account.Strings.RestrictedRegistrationMessage);
                    yield return new ValidationResult("Invalid registration code.", new string[] { "RegistrationCode" });
                }
            }
            int accLimit = settings["system.AccountLimit"].As<int>();
            using (var context = new BLWAContext())
            {
                int actual = context.Accounts.Count();
                if (actual >= accLimit)
                {
                    //yield return new ValidationResult(Resources.Account.Strings.AccountLimitReached);
                    yield return new ValidationResult("Account limit has been reached.");
                }
            }
        }
    }
}
