using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(FT_stepoverAPI.Startup))]
namespace FT_stepoverAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {


           
            //configura a web api para auto-hospedagem
            var config = new HttpConfiguration();
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
           );

            app.UseWebApi(config);
        }
    }
}