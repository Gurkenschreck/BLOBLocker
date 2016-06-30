using BLOBLocker.Entities.Models.WebApp;
using BLOBLocker.WebApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BLOBLocker.WebApp.API.Controllers
{
    public class SecurityController : ApiController
    {
        public SecurityInformation GetInformations()
        {
            return new SecurityInformation
            {
                IPV4Address = Request.Properties["MS_HttpContext"] != null ? ((HttpContextBase)Request.Properties["MS_HttpContext"]).Request.UserHostAddress : "" 
            };
        }
    }
}
