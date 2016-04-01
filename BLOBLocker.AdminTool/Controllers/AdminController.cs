using BLOBLocker.AdminTool.Models;
using BLOBLocker.Code.Attributes;
using BLOBLocker.Code.Controllers;
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
            var configs = context.SystemConfigurations
                                .OrderBy(p => p.Key)
                                .ToDictionary(k => k.Key,
                                            v => v.Value,
                                            StringComparer.OrdinalIgnoreCase);
                                            
            aovm.ConfigValues = configs;
            return View(aovm);
        }

        [HttpPost]
        public ActionResult Edit(SystemConfiguration config)
        {
            if (config == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(config.Key))
            {
                ModelState.AddModelError("key", "SharedKey is invalid");
            }
            if (string.IsNullOrWhiteSpace(config.Value))
            {
                ModelState.AddModelError("value", "Value is invalid");
            }
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(SystemConfiguration newConfig)
        {
            if (newConfig == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(newConfig.Key))
            {
                ModelState.AddModelError("key", "invalid key");
            }
            if (string.IsNullOrWhiteSpace(newConfig.Value))
            {
                ModelState.AddModelError("value", "invalid value");
            }
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
        [HttpGet]
        public ActionResult EditAccount(int id)
        {
            if (id < 0 || id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var acc = atContext.Accounts.FirstOrDefault(p => p.ID == id);
            if (acc == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AdminEditAccountModel aeam = new AdminEditAccountModel(acc);
            return View(aeam);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditAccount(AdminEditAccountModel aeam)
        {
            if (aeam == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            BLOBLocker.Entities.Models.AdminTool.Account acc = atContext.Accounts.FirstOrDefault(p => p.ID == aeam.ID);
            if (acc == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var roles = atContext.Roles.ToList();
            aeam.ApplyChanges(acc, roles, atContext);
            atContext.Entry<BLOBLocker.Entities.Models.AdminTool.Account>(acc).State = System.Data.Entity.EntityState.Modified;
            atContext.SaveChanges();

            aeam.Alias = acc.Alias;
            aeam.Email = acc.Email;
            
            return View(aeam);
        }
    }
}