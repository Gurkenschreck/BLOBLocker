using BLOBLocker.Code.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.Controllers
{
    [Authorize]
    [AjaxOnly]
    public class AjaxController : Controller
    {
        
    }
}
