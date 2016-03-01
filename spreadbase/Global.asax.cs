using SpreadBase.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SpreadBase
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            using(SpreadBaseContext context = new SpreadBaseContext())
            {
                foreach(Configuration conf in context.SystemConfigurations)
                {
                    Application[conf.Key] = conf.Value;
                }
            }
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