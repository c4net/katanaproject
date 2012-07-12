using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Gate;
using Katana.WebApi;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Gate.Adapters.Nancy;
using Nancy;
using System.Web.Http;
using System.Net.Http;


namespace Katana.Server.AspNet.WebApplication
{
    public class Startup
    {
        public static void Configuration(IAppBuilder builder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}");

            builder.UseShowExceptions();

            builder.UseMessageHandler(new TraceRequestFilter());

            builder.Map("/api", map => map.UseMessageHandler(new HttpServer(config)));

            builder.RunNancy();
        }
    }

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = _ => "Hello, Nancy!";

            Get["/blah"] = _ => Response.AsRedirect("/redir?x=5");

            Get["/redir"] = _ => "Welcome back. x = " + Request.Query.x;
        }
    }

    public class HelloController : ApiController
    {
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent("Hello, WebAPI!", Encoding.UTF8, "text/plain")
            };
        }
    }
}
