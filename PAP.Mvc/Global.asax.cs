using System;
using System.Collections.Generic;
using System.Linq;
using PAP.Mvc.ModelBinders;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PAP.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Acepta "." o "," como separador decimal en los formularios CRUD.
            ModelBinders.Binders.Add(typeof(decimal), new InvariantDecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new InvariantDecimalModelBinder());
        }
    }
}
