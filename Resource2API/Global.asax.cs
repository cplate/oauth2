using System.Web.Http;

namespace Resource2API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            GlobalConfiguration.Configuration.EnableSystemDiagnosticsTracing();
        }
    }
}