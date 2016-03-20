using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code.Attributes
{
    public class PreserveModelStateAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.Controller.TempData["ModelState"] =
                filterContext.Controller.ViewData.ModelState;
        }
    }
}
