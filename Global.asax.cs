using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ecommerce_Website_MVC;

namespace Ecommerce_Website_MVC
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}

