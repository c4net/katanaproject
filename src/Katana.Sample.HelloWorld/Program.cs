namespace Katana.Sample.HelloWorld
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Gate;
    using Katana.Engine;
    using Katana.Engine.Settings;
    using Owin;
    // using Gate.Middleware;

    class Program
    {
        // Use this project to F5 test different applications and servers together.
        public static void Main(string[] args)
        {
            var settings = new KatanaSettings();

            KatanaEngine engine = new KatanaEngine(settings);

            var info = new StartInfo
            {
                Server = "HttpListener", // Katana.Server.HttpListener
                Startup = "Katana.Sample.HelloWorld.Program.Configuration", // Application
                Url = "http://+:8080/",
                /*
                OutputFile = string.Empty,
                Scheme = arguments.Scheme,
                Host = arguments.Host,
                Port = arguments.Port,
                Path = arguments.Path,
                 */
            };

            IDisposable server = engine.Start(info);
            Console.WriteLine("Running, press any key to exit");
            Console.ReadKey();
        }

        public void Configuration(IAppBuilder builder)
        {
            // TODO: Waiting for OWIN breaking changes to be fixed in Gate.Middleware.
            builder.UseShowExceptions().Use<AppDelegate>(_ => Wilson.App());
        }
    }

    public static class Temp
    {
        public static IAppBuilder UseShowExceptions(this IAppBuilder builder)
        {
            return builder;
        }
    }

    public static class Wilson
    {
        public static AppDelegate App()
        {
            return async call =>
            {
                var req = new Request(call);
                var resp = new Response();
                resp.Headers.SetHeader("Content-Type", "text/plain");
                
                await resp.WriteAsync("Hello world\r\n");
                await resp.WriteAsync("PathBase {0}\r\n", req.PathBase);
                await resp.WriteAsync("Path {0}\r\n", req.Path);

                return await resp.GetResultAsync();
            };
        }
    }
}
