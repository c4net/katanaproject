using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gate.Builder;
using Owin;
using Shouldly;
using Xunit;

namespace Katana.WebApi.Tests
{
    public class UseMessageHandlerTests
    {
        [Fact]
        public void MessageHandlerWillBeCreatedByAppBuilder()
        {
            var builder = new AppBuilder();

            builder.UseMessageHandler<HelloWorldHandler>();

            var app = builder.Materialize<HttpMessageHandler>();


            app.ShouldBeTypeOf<HelloWorldHandler>();
            var handler = (HelloWorldHandler)app;
            handler.CtorTwoCalled.ShouldBe(true);
        }

        [Fact]
        public Task CallingAppDelegateShouldInvokeMessageHandler()
        {
            var builder = new AppBuilder();

            builder.UseMessageHandler<HelloWorldHandler>();

            var app = builder.Materialize<AppDelegate>();

            var env = new Dictionary<string, object>
            {
                {"owin.Version", "1.0"},
                {"owin.RequestMethod", "GET"},
                {"owin.RequestScheme", "http"},
                {"owin.RequestPathBase", ""},
                {"owin.RequestPath", "/"},
                {"owin.RequestQueryString", ""},
            };

            return app.Invoke(
                new CallParameters
                {
                    Environment = env,
                    Headers = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase),
                    Body = new MemoryStream()
                }).Then(result =>
                {
                    result.Status.ShouldBe(200);
                    result.Headers["Content-Type"].ShouldBe(new[]
                    {
                        "text/plain; charset=utf-8"
                    });
                });
        }
    }
}
