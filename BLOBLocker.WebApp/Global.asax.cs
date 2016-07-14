using BLOBLocker.Code;
using BLOBLocker.Code.Web;
using BLOBLocker.Entities.Models;
using BLOBLocker.Entities.Models.WebApp;
using BLOBLocker.WebApp.App_Start;
using Cipha.Security.Cryptography;
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

        protected void Application_End(object sender, EventArgs args)
        {
            cancellationTokenSource.Cancel();
        }

        protected void Session_Start(object sender, EventArgs args)
        {
            Session["customCulture"] = Thread.CurrentThread.CurrentUICulture;

            if(User.Identity.IsAuthenticated)
            {
                foreach (var cookieName in Request.Cookies.AllKeys)
                {
                    Response.Cookies[cookieName].Value = "";
                    Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
                }
                
                Response.Redirect(FormsAuthentication.LoginUrl);
            }
        }

        protected void Session_End(object sender, EventArgs args)
        {
            CryptoSessionStore.WipeAllStores(Session);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs args)
        {
            CultureInfo customLanguage = null;
            if (HttpContext.Current.Session != null)
            {
                if (Session != null)
                {
                    customLanguage = Session["customCulture"] as CultureInfo;
                }
            }
            if (customLanguage != null)
            {
                Thread.CurrentThread.CurrentCulture = customLanguage;
                Thread.CurrentThread.CurrentUICulture = customLanguage;
            }
            else
            {
                if (HttpContext.Current.Session != null)
                {
                    Session["customCulture"] = Thread.CurrentThread.CurrentCulture;
                }
            }
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs args)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.AddHeader("REFRESH", string.Format("{0};URL={1}?returnUrl={2}",
                    FormsAuthentication.Timeout.TotalSeconds,
                    "/Account/SignOut",
                    Request.Url.PathAndQuery));
            }
        }
    }
}