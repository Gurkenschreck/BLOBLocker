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
    /// RequiredParameters performs null checking for the
    /// passed ActionMethod parameters.
    /// Returns Code 400 if the parameter name is not found or
    /// the parameter is null.
    /// </summary>
    public class RequiredParametersAttribute : ActionFilterAttribute
    {
        string[] attributes;

        public RequiredParametersAttribute(params string[] attributeNames)
        {
            attributes = attributeNames;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            foreach(var param in attributes)
                if(filterContext.ActionParameters[param] == null)
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        }
    }
}
