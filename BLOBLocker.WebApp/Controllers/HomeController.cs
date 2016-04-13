using BLOBLocker.Code.Controllers;
using BLOBLocker.WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            //var x = HttpContext.GetGlobalResourceObject("a", "General.Hello");
            return View();
        }

    }
}
