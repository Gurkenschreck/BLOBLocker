using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.Code.Attributes
{
    /// <summary>
    /// The controller or actions can only be accessed
    /// via ajax requests.
    /// </summary>
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}