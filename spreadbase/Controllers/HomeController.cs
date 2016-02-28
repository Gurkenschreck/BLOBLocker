using SpreadBase.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpreadBase.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

    }
}
