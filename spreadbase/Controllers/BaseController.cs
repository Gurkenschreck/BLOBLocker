﻿using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpreadBase.Controllers
{
    [RequireHttps]
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected SpreadBaseContext context = new SpreadBaseContext();

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
