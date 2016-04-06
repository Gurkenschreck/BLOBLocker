using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code.Attributes
{
    public class AllParametersRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            foreach (var param in filterContext.ActionParameters)
                if (param.Value == null)
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
