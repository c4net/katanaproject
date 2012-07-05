using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Owin;

namespace Katana.WebApi
{
    public class CallAppDelegate : HttpMessageHandler
    {
        private readonly AppDelegate _app;

        public CallAppDelegate(AppDelegate app)
        {
            _app = app;
        }

        public static HttpMessageHandler Create(AppDelegate app)
        {
            return new CallAppDelegate(app);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Adapters.GetCallParameters(request)
                .Then(call => _app.Invoke(call)
                    .Then(result => Utils.GetResponseMessage(result)));
        }
    }
}

