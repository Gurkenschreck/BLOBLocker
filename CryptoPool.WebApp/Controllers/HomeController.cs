using CryptoPool.Code.Controllers;
using CryptoPool.WebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoPool.WebApp.Controllers
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
