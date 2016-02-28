﻿using SpreadBase.Controllers;
using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;

namespace SpreadBase.Controllers
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

            foreach(Configuration conf in dbValues)
            {
                settings[conf.Key] = conf.Value;
            }
            
            return View(settings);
        }

        [HttpGet]
        public ActionResult Overview()
        {
            var settings = new Dictionary<string, string>();

            var dbValues = from conf in context.SystemConfigurations
                           select conf;

            foreach (Configuration conf in dbValues)
            {
                settings[conf.Key] = conf.Value;
            }

            return View(settings);
        }

        [HttpPost]
        public ActionResult Edit(Configuration config)
        {
            if(string.IsNullOrWhiteSpace(config.Key))
            {
                ModelState.AddModelError("key", "Key is invalid");
            }
            if (string.IsNullOrWhiteSpace(config.Value))
            {
                ModelState.AddModelError("value", "Value is invalid");
            }
            if (ModelState.IsValid)
            {
                var dbValues = (from conf in context.SystemConfigurations
                                where conf.Key == config.Key
                                select conf).FirstOrDefault();
                
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
        public ActionResult Create(Configuration newConfig)
        {
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
                foreach (Configuration pair in context.SystemConfigurations)
                {
                    HttpContext.ApplicationInstance.Application[pair.Key] = pair.Value;
                }
            }
            return "finished";
        }
    }
}
