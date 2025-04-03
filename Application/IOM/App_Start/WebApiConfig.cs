using IOM.Attributes;
using System.Web.Http;

namespace IOM
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.Filters.Add(new TwoFactorAuthAttribute());
            config.Filters.Add(new ElmahErrorAttribute());
            config.Filters.Add(new LoggerAttribute());
            //config.Filters.Add(new TimeUpdateAttribute());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );
        }
    }
}