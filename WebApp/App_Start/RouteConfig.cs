using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Default1",
               url: "{controller}/{action}/{id}/{page}.html",
               defaults: new { controller = "Center", action = "Index" }
           );

            routes.MapRoute(
                 name: "Default2",
                 url: "{controller}/{action}.html",
                 defaults: new { controller = "Center", action = "Index" }
             );

            routes.MapRoute(
                name: "Default3",
                url: "{controller}/{action}/{id}.html",
                defaults: new { controller = "Center", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Center", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}