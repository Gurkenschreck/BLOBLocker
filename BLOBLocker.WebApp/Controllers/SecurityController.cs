using BLOBLocker.Code.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
{
    [AllowAnonymous]
    public class SecurityController : BaseController
    {
        //
        // GET: /Security/Info
        [HttpGet]
        public ActionResult Info(bool? noLayout)
        {
            if (noLayout == null)
                noLayout = false;

            ViewBag.NoLayout = noLayout;

            return View();
        }


        [HttpGet]
        public ActionResult Peek(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "/Security/Info?noLayout=true";
            }
            else
            {
                if (!url.StartsWith("https"))
                    url = url.Replace("http", "https");
            }
            ViewBag.URL = url;
            return View();
        }
    }
}
