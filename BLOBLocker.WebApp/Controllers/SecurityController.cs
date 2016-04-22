using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public ActionResult Peek(string url, 
            bool? activateJS,
            bool? activateForms,
            bool? activateSameOrigin,
            bool? activateTopNavigation,
            bool? activateProxy,
            bool? activatePointerLock,
            bool? activatePopups)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "/Security/Info?noLayout=true";
            }

            if (activateProxy != null && activateProxy == true)
            {
                url = "/Security/Proxy?url=" + url;
            }


            ViewBag.URL = url;
            ViewBag.ActivateJS = activateJS;
            ViewBag.ActivateForms = activateForms;
            ViewBag.ActivateSameOrigin = activateSameOrigin;
            ViewBag.ActivateTopNavigation = activateTopNavigation;
            ViewBag.ActivateProxy = activateProxy;
            ViewBag.ActivatePopups = activatePopups;
            ViewBag.ActivatePointerLock = activatePointerLock;

            return View();
        }

        
        public void Proxy(string url)
        {
            try
            {
                ReverseProxy rproxy = new ReverseProxy();
                rproxy.ProcessRequest(HttpContext.ApplicationInstance.Context, url);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            }
        }
    }
}
