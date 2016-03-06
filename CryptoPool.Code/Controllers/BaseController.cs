using CryptoPool.Entities.Models;
using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoPool.Code.Controllers
{
    [RequireHttps]
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected CryptoPoolContext context = new CryptoPoolContext();

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
