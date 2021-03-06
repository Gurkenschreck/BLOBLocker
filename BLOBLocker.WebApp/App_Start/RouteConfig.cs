﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BLOBLocker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Robots.txt",
                "robots.txt",
                new { controller = "Home", action = "Robots" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{puid}",
                defaults: new { controller = "Home", action = "Index", puid = UrlParameter.Optional }
            );
        }
    }
}