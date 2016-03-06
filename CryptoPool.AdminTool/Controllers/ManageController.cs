using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CryptoPool.Code;

namespace CryptoPool.AdminTool.Controllers
{
    [Authorize(Roles="Administrator,Moderator")]
    [RequireHttps]
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }
    }
}