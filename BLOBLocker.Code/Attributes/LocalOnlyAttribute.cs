using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code.Attributes
{
    /// <summary>
    /// A controller or action can only be called if the
    /// request comes from the local computer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LocalOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsLocal)
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
