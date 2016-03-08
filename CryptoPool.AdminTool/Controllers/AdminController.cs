using CryptoPool.Code.Attributes;
using CryptoPool.Code.Controllers;
using CryptoPool.Entities.Models.AdminTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoPool.AdminTool.Controllers
{
    [Authorize(Roles="Administrator")]
    [LocalOnly]
    public class AdminController : BaseController
    {
        AdminToolContext atContext = new AdminToolContext();
        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            var configs = context.SystemConfigurations
                .ToDictionary(k => k.Key,
                              v => v.Value,
                              StringComparer.OrdinalIgnoreCase);
            return View(configs);
        }
    }
}