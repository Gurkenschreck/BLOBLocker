using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.Code.Controllers
{
    [RequireHttps]
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected BLWAContext context = new BLWAContext();

        protected override void Dispose(bool disposing)
        {
            if(context != null)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
