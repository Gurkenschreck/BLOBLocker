using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.Data;
using BLOBLocker.Entities.Models.WebApp;
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
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Transparency()
        {
            return View();
        }

        public ActionResult VerticalMenu()
        {
            ICollection<Pool> pools = null;

            if (User.Identity.IsAuthenticated)
            {
                pools = new AccountRepository().GetByKey(User.Identity.Name).Pools.Where(p => p.IsActive).ToArray();
            }
            return PartialView("_VerticalMenu", pools);
        }
    }
}
