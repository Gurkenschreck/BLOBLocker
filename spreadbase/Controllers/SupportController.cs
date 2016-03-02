using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpreadBase.Controllers
{
    [Authorize(Roles="Moderator, Administrator")]
    public class SupportController : BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Faq()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ShowUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShowUser(string name)
        {
            Account found = GetAccount(name);
            return View(found);
        }
    }
}
