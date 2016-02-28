using SpreadBase.Controllers;
using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpreadBase.Controllers
{
    public class PanelController : BaseController
    {
        private Account GetCurrentAccount()
        {
            string name = HttpContext.User.Identity.Name;
            return context.Accounts.FirstOrDefault(p => p.Alias == name);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(GetCurrentAccount());
        }

        [HttpGet]
        public ActionResult AddContact()
        {
            return View(GetCurrentAccount());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddContact(string addAlias)
        {
            if(string.IsNullOrWhiteSpace(addAlias))
            {
                ModelState.AddModelError("addAlias", "Invalid addAlias");
            }
            if(ModelState.IsValid)
            {
                var correspondingAccount = context.Accounts.FirstOrDefault(p => p.Alias == addAlias);
                if(correspondingAccount != null)
                {
                        string name = HttpContext.User.Identity.Name;
                        var curAcc = context.Accounts.FirstOrDefault(p => p.Alias == name);

                        curAcc.Addition.Contacts.Add(correspondingAccount);
                        context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("addAlias", "No such user found");
                }
            }
            return View(GetCurrentAccount());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShareContacts(string shareWithAlias)
        {
            if (string.IsNullOrWhiteSpace(shareWithAlias))
            {
                ModelState.AddModelError("addAlias", "Invalid addAlias");
            }
            if (ModelState.IsValid)
            {
                var curAcc = context.Accounts.FirstOrDefault(acc => acc.Alias == HttpContext.User.Identity.Name);
                var correspondingAccount = context.Accounts.FirstOrDefault(acc => acc.Alias == shareWithAlias);
                if (correspondingAccount != null)
                {

                    ICollection<Account> corAccContacts = correspondingAccount.Addition.Contacts;
                    foreach(Account c in curAcc.Addition.Contacts)
                    {
                        if(!corAccContacts.Contains(c))
                            corAccContacts.Add(c);
                    }

                    context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("addAlias", "No such user found");
                }
            }
            return RedirectToAction("AddContact");
        }
    }
}
