using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Code.ViewModels.AdminTool;
using BLOBLocker.Entities.Models.AdminTool;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.AdminTool.Controllers
{
    [Authorize(Roles="Administrator")]
    [LocalOnly]
    public class AdminController : BaseController
    {
        BLATContext atContext = new BLATContext();
        // GET: Admin
        [HttpGet]
        public ActionResult Overview()
        {
            AdminOverviewViewModel aovm = new AdminOverviewViewModel();
            var configs = context.SystemConfigurations.Where(p => p.Key != "ConfigChanged")
                                .OrderBy(p => p.Key)
                                .ToDictionary(k => k.Key,
                                            v => v.Value,
                                            StringComparer.OrdinalIgnoreCase);
                                            
            aovm.ConfigValues = configs;
            return View(aovm);
        }

        [RequiredParameters("config")]
        [HttpPost]
        public ActionResult Edit(SystemConfiguration config)
        {
            if (ModelState.IsValid)
            {
                var dbValues = context.SystemConfigurations.FirstOrDefault(p => p.Key == config.Key);
                if (dbValues == null)
                {
                    ModelState.AddModelError("unknownKey", "Cannot change not present key");
                }
                else
                {
                    dbValues.Value = config.Value;
                    context.SaveChanges();
                }
            }
            return RedirectToAction("Overview");
        }

        [RequiredParameters("newConfig")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(SystemConfiguration newConfig)
        {
            if (ModelState.IsValid)
            {
                context.SystemConfigurations.Add(newConfig);
                context.SaveChanges();
            }

            return RedirectToAction("Overview");
        }

        [HttpPost]
        public string ApplyDBConfiguration()
        {
            var conf = context.SystemConfigurations.FirstOrDefault(p => p.Key == "ConfigChanged");
            if(conf != null)
            {
                conf.Value = "true";
                context.SaveChangesAsync();
            }
            return "ok";
        }

        [HttpGet]
        public ActionResult Accounts()
        {
            var accs = atContext.Accounts.ToList();
            return View(accs);
        }

        [RequiredParameters("id")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditAccount(int id)
        {
            BLOBLocker.Entities.Models.AdminTool.Account acc = atContext.Accounts.FirstOrDefault(p => p.ID == id);
            AdminEditAccountModel aeam = new AdminEditAccountModel(acc);
            return View(aeam);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateAccount()
        {
            BLOBLocker.Entities.Models.AdminTool.Account acc = new Entities.Models.AdminTool.Account();
            var aeam = new AdminEditAccountModel(acc);
            return View(aeam);
        }

        [RequiredParameters("aeam")]
        [HttpPost]
        public ActionResult AddOrChangeAccount(AdminEditAccountModel aeam, bool add)
        {

            if (ModelState.IsValid)
            {
                if (add)
                {
                    BLOBLocker.Entities.Models.AdminTool.Account newAcc = aeam.Parse();
                    if (aeam.IsAdmin)
                    {
                        var adminRole = atContext.Roles.First(p => p.Definition == "Administrator");
                        newAcc.Roles.Add(new RoleLink()
                        {
                            Role = adminRole,
                            Account = newAcc
                        });
                    }
                    if (aeam.IsModerator)
                    {
                        var modRole = atContext.Roles.First(p => p.Definition == "Moderator");
                        newAcc.Roles.Add(new RoleLink()
                        {
                            Role = modRole,
                            Account = newAcc
                        });
                    }
                    if (aeam.IsTranslator)
                    {
                        var transRole = atContext.Roles.First(p => p.Definition == "Translator");
                        newAcc.Roles.Add(new RoleLink()
                        {
                            Role = transRole,
                            Account = newAcc
                        });
                    }
                    atContext.Accounts.Add(newAcc);
                    atContext.SaveChanges();
                }
                else
                {
                    BLOBLocker.Entities.Models.AdminTool.Account acc = atContext.Accounts.FirstOrDefault(p => p.ID == aeam.ID);
                    var roles = atContext.Roles.ToList();
                    aeam.ApplyChanges(acc, roles, atContext);
                    atContext.Entry<BLOBLocker.Entities.Models.AdminTool.Account>(acc).State = System.Data.Entity.EntityState.Modified;
                    atContext.SaveChanges();
                }
                return RedirectToAction("Accounts");
            }
            return View(aeam);
        }
    }
}