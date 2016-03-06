using CryptoPool.Code.Controllers;
using CryptoPool.Code.ModelHelper;
using CryptoPool.Entities.Models;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoPool.WebApp.Controllers
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
            Account found = accRepo.GetAccount(name);
            return View(found);
        }
    }
}
