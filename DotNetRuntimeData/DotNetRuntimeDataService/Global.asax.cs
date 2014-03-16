using System.Web.Http;
using System.Web.Routing;

namespace DotNetRuntimeDataService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}