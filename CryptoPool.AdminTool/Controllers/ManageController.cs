using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CryptoPool.Code;
using CryptoPool.Code.Controllers;
using CryptoPool.Entities.Models.AdminTool;

namespace CryptoPool.AdminTool.Controllers
{
    [Authorize(Roles="Administrator,Moderator")]
    [RequireHttps]
    public class ManageController : BaseController
    {
        AdminToolContext atContext = new AdminToolContext();
        // GET: Manage
        public ActionResult Index()
        {
            var roles = atContext.Accounts
                .FirstOrDefault(p => p.Alias == User.Identity.Name)
                .Roles.Select(p => p.Role);
            return View(roles);
        }


        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                atContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}