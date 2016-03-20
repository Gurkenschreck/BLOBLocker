using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code.Attributes
{
    public class RestoreModelStateAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext.Controller.TempData.ContainsKey("ModelState"))
            {
                filterContext.Controller.ViewData.ModelState.Merge((ModelStateDictionary)filterContext.Controller.TempData["ModelState"]);
            }
        }
    }
}
