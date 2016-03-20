using BLOBLocker.WebApp.Controllers;
using BLOBLocker.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using BLOBLocker.Code.Controllers;
using BLOBLocker.Entities.Models.WebApp;

namespace BLOBLocker.WebApp.Controllers
{
    [Authorize(Roles="Administrator")]
    public class ManageController : BaseController
    {
        //
        // GET: /Manage/
        [HttpGet]
        public ActionResult Index()
        {
            var settings = new Dictionary<string, string>();
            var dbValues = context.SystemConfigurations.ToList();
            foreach (SystemConfiguration conf in dbValues)
            {
                settings[conf.Key] = conf.Value;
            }
            return View(settings);
        }

        [HttpGet]
        public ActionResult Overview()
        {
            var dbValues = context.SystemConfigurations.ToDictionary(p => p.Key, q => q.Value);
            
            return View(dbValues);
        }

        [HttpPost]
        public ActionResult Edit(SystemConfiguration config)
        {
            if (config == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(string.IsNullOrWhiteSpace(config.Key))
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
                if(dbValues == null)
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
            lock (HttpContext.ApplicationInstance.Application)
            {
                foreach (SystemConfiguration pair in context.SystemConfigurations)
                {
                    HttpContext.ApplicationInstance.Application[pair.Key] = pair.Value;
                }
            }
            return "ok";
        }
    }
}
