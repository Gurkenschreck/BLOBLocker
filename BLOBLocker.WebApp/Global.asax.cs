using BLOBLocker.Code;
using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using BLOBLocker.WebApp.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace BLOBLocker.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;

        protected void Application_Start()
        {
            ViewEngines.Engines.Add(new BlobLockerViewEngine());
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            TaskConfig.StartBackgroundService(Application, cancellationToken);
        }
        protected void Session_Start(object sender, EventArgs args)
        {
            if(User.Identity.IsAuthenticated)
            {
                Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Secret"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("/Account/Login");
            }
        }

        protected void Application_End(object sender, EventArgs args)
        {
            cancellationTokenSource.Cancel();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs args)
        {
            var custumLanguage = Session["customCulture"] as CultureInfo;
            if (custumLanguage != null)
            {
                var culture = custumLanguage;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
                Session["customCulture"] = Thread.CurrentThread.CurrentCulture;
            }
        }
    }
}