using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.Data;
using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
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
            AccountRepository accRepo = new AccountRepository(context);
            Account found = accRepo.GetByKey(name);
            return View(found);
        }
    }
}
