using spreadbase.Models;
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
        SpreadBaseContext context = new SpreadBaseContext();
        //
        // GET: /Panel/
        /*private void FillSession()
        {
            if(Session["acc"] == null)
            {
                string name = HttpContext.User.Identity.Name;
                var curAcc = (from acc in context.Accounts
                             where acc.Alias == name
                             select acc).FirstOrDefault();

                if(curAcc != null)
                {
                    Session["acc"] = curAcc;
                }
            }
        }*/
        public ActionResult Index()
        {
            string name = HttpContext.User.Identity.Name;
            var curAcc = (from acc in context.Accounts
                          where acc.Alias == name
                          select acc).FirstOrDefault();

            Session["contacts"] = curAcc.Addition.Contacts.ToList<Account>();
            return View();
        }

        public ActionResult AddContact()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddContact(string alias)
        {
            if(string.IsNullOrWhiteSpace(alias))
            {
                ModelState.AddModelError("alias", "Invalid alias");
            }
            if(ModelState.IsValid)
            {
                var correspondingAccount = (from acc in context.Accounts
                                           where acc.Alias == alias
                                           select acc).FirstOrDefault();

                

                if(correspondingAccount != null)
                {
                    
                        string name = HttpContext.User.Identity.Name;
                        var curAcc = (from acc in context.Accounts
                                      where acc.Alias == name
                                      select acc).FirstOrDefault();

                        curAcc.Addition.Contacts.Add(correspondingAccount);
                        int j = context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("alias", "No such user found");
                }
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ShareContacts(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                ModelState.AddModelError("alias", "Invalid alias");
            }
            if (ModelState.IsValid)
            {
                var correspondingAccount = (from acc in context.Accounts
                                            where acc.Alias == alias
                                            select acc).FirstOrDefault();

                if (correspondingAccount != null)
                {
                    Account curAcc = Session["user"] as Account;
                    /*List<int> correspondingContactIds = correspondingAccount.Addition.ContactIDs.ToList();
                    foreach(int contactId in curAcc.Addition.ContactIDs)
                    {
                        if(!correspondingContactIds.Contains(contactId))
                            correspondingContactIds.Add(contactId);
                    }*/



                    context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("alias", "No such user found");
                }
            }
            return View("~/Views/Panel/AddContact.cshtml");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
