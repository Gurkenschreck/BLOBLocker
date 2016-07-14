using BLOBLocker.Code.ViewModels.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BLOBLocker.Code.Extention;

namespace BLOBLocker.Code.Attributes
{
    /// <summary>
    /// CustomValidate executes custom validation logic before an
    /// action is executed.
    /// 
    /// It accepts a custom ModelValidator which implements the
    /// IActionModelValidation interface. On action method execution,
    /// the attribute will fire the IActionModelValidation's 
    /// Validate method. The resulting ValidationResults will
    /// be added into the current ModelState.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomValidateAttribute : ActionFilterAttribute
    {
        IActionModelValidation valider;

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="valider">An IActionModelValidation implementing class.</param>
        public CustomValidateAttribute(Type valider)
        {
            this.valider = (IActionModelValidation)Activator.CreateInstance(valider);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var settingsDict = new Dictionary<string, object>();
            foreach (var key in filterContext.HttpContext.Application.AllKeys)
            {
                settingsDict[key] = filterContext.HttpContext.Application[key];
            }

            IEnumerable<ValidationResult> result = valider.Validate(settingsDict, filterContext.ActionParameters);
            foreach(var res in result)
            {
                if (res.MemberNames.Any())
                {
                    foreach (var name in res.MemberNames)
                    {
                        filterContext.Controller.ViewData.ModelState.AddModelError(name, res.ErrorMessage);
                    }
                }
                else
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, res.ErrorMessage);

                }
            }
        }
    }
}
