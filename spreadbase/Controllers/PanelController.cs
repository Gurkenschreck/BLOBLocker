using SpreadBase.Controllers;
using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var acc = GetCurrentAccount();

            return View(acc);
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
            if (addAlias == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(addAlias))
            {
                ModelState.AddModelError("addAlias", "Invalid addAlias");
            }
            Account curAcc = GetCurrentAccount();
            if(ModelState.IsValid)
            {
                var correspondingAccount = context.Accounts.FirstOrDefault(p => p.Alias == addAlias);
                if(correspondingAccount != null)
                {
                    curAcc.Addition.Contacts.Add(correspondingAccount);

                    Notification not1 = new Notification();
                    not1.Description = string.Format("You added {0} to your contact list", correspondingAccount.Alias);

                    Notification not2 = new Notification();
                    not2.Description = string.Format("{0} added you to his list", curAcc.Alias);
                    
                    curAcc.Addition.Notifications.Add(not1);
                    correspondingAccount.Addition.Notifications.Add(not2);
                    context.SaveChanges();

                }
                else
                {
                    ModelState.AddModelError("addAlias", "No such user found");
                }
            }
            return View(curAcc);
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
                var curAcc = GetCurrentAccount();
                var correspondingAccount = context.Accounts.FirstOrDefault(acc => acc.Alias == shareWithAlias);

                if (correspondingAccount != null)
                {
                    if (curAcc.ID != correspondingAccount.ID)
                    {
                        ICollection<Account> corAccContacts = correspondingAccount.Addition.Contacts;
                        foreach (Account c in curAcc.Addition.Contacts)
                        {
                            if (!corAccContacts.Contains(c))
                                corAccContacts.Add(c);
                        }
                        correspondingAccount.Addition.Notifications.Add(new Notification
                        {
                            Description = string.Format("{0} has shared all his contacts with you.", curAcc.Alias)
                        });
                        curAcc.Addition.Notifications.Add(new Notification
                        {
                            Description = string.Format("You shared all your contacts with {0}.", correspondingAccount.Alias)
                        });

                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("addAlias", "You cannot share with yourself");
                    }
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
