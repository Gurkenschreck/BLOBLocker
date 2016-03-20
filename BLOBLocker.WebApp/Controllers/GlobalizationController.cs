using BLOBLocker.Code.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
{
    [AllowAnonymous]
    public class GlobalizationController : BaseController
    {
        //
        // GET: /Globalization/
        [HttpGet]
        public ActionResult SetSessionCulture(string culture)
        {
            var culinfo = new CultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = culinfo;
            Thread.CurrentThread.CurrentUICulture = culinfo;

            Session["customCulture"] = culinfo;
            var caller = Request.UrlReferrer.OriginalString;
            return Redirect(caller);
        }
    }
}
