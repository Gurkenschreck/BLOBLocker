using Cipha.Security.Cryptography.Hash;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Entities.Models.AdminTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Cipha.Security.Cryptography;
using BLOBLocker.Code.ViewModels.AdminTool;

namespace BLOBLocker.AdminTool.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        BLATContext atContext = new BLATContext();

        // GET: Account
        [HttpGet]
        public ActionResult Index()
        {
            LoginViewModel lvm = new LoginViewModel();
            return View(lvm);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(LoginViewModel lvm)
        {
            var dbAccount = atContext.Accounts.FirstOrDefault(p => p.Alias == lvm.Alias);
            if(dbAccount == null)
            {
                ModelState.AddModelError("AliasOrPasswordWrong", "Alias and/or password wrong.");
            }
            else
            {
                if (!dbAccount.IsActive)
                    ModelState.AddModelError("IsActive", "Account is disabled.");
            }
            
            if (ModelState.IsValid)
            {
                using(var deriver = new Rfc2898DeriveBytes(lvm.Password, dbAccount.Salt, 21423))
                {
                    byte[] derived = deriver.GetBytes(dbAccount.Salt.Length);
                    if (Utilities.SlowEquals(dbAccount.DerivedPassword, derived))
                    {
                        dbAccount.LastLogin = DateTime.Now;
                        atContext.SaveChangesAsync();
                        FormsAuthentication.SetAuthCookie(dbAccount.Alias, false);
                        if (Request.QueryString["ReturnUrl"] == null)
                            return RedirectToAction("Home");
                        else
                            Response.Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else
                    {
                        ModelState.AddModelError("AliasOrPasswordWrong", "Alias and/or password wrong.");
                    }
                }
            }
            return View(lvm);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Home()
        {
            var roles = atContext.Accounts
                .FirstOrDefault(p => p.Alias == User.Identity.Name)
                .Roles.Select(p => p.Role);
            return View(roles);
        }
    }
}