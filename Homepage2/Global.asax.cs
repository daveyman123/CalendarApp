using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;

namespace Homepage2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles); GlobalConfiguration.Configuration
             .UseSqlServerStorage("Data Source=tcp:bobdylan.database.windows.net,1433;Initial Catalog=CalendarDB;Persist Security Info=False;User ID=daveyman123;Password=dJGTT901;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False");
        }
    }
}
