using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace spreadbase.Controllers
{
    [RequireHttps]
    [Authorize]
    public class PanelController : Controller
    {
        //
        // GET: /Panel/

        public ActionResult Index()
        {
            return View();
        }

    }
}
