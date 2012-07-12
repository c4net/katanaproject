using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Katana.WebApi.CallContent;
using Katana.WebApi.CallHeaders;
using Owin;

namespace Katana.WebApi
{
    public static class Adapters
    {
        static Adapters()
        {
            Adapter1 = CallAppDelegate.Create;
            Adapter2 = CallMessageHandler.Create;
        }

        public static Func<AppDelegate, HttpMessageHandler> Adapter1 { get; set; }
        public static Func<HttpMessageHandler, AppDelegate> Adapter2 { get; set; }


        public static HttpRequestMessage GetRequestMessage(CallParameters call)
        {
            var requestHeadersWrapper = call.Headers as RequestHeadersWrapper;
            var requestMessage = requestHeadersWrapper != null ? requestHeadersWrapper.Message : null;

            if (requestMessage == null)
            {
                // initial transition to HRM, or headers dictionary has been substituted
                var requestScheme = Utils.Get<string>(call.Environment, "owin.RequestScheme");
                var requestMethod = Utils.Get<string>(call.Environment, "owin.RequestMethod");
                var requestPathBase = Utils.Get<string>(call.Environment, "owin.RequestPathBase");
                var requestPath = Utils.Get<string>(call.Environment, "owin.RequestPath");
                var requestQueryString = Utils.Get<string>(call.Environment, "owin.RequestQueryString");

                // default values, in absense of a host header
                var host = "127.0.0.1";
                var port = String.Equals(requestScheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80;

                // if a single host header is available
                string[] hostAndPort;
                if (call.Headers.TryGetValue("Host", out hostAndPort) &&
                    hostAndPort != null &&
                    hostAndPort.Length == 1 &&
                    !String.IsNullOrWhiteSpace(hostAndPort[0]))
                {
                    // try to parse as "host:port" format
                    var delimiterIndex = hostAndPort[0].LastIndexOf(':');
                    int portValue;
                    if (delimiterIndex != -1 &&
                        Int32.TryParse(hostAndPort[0].Substring(delimiterIndex + 1), out portValue))
                    {
                        // use those two values
                        host = hostAndPort[0].Substring(0, delimiterIndex);
                        port = portValue;
                    }
                    else
                    {
                        // otherwise treat as host name
                        host = hostAndPort[0];
                    }
                }

                var uriBuilder = new UriBuilder(requestScheme, host, port, requestPathBase + requestPath);
                if (!String.IsNullOrEmpty(requestQueryString))
                {
                    uriBuilder.Query = requestQueryString;
                }

                requestMessage = new HttpRequestMessage(new HttpMethod(requestMethod), uriBuilder.Uri);
                call.Environment["System.Net.Http.HttpRequestMessage"] = requestMessage;

                if (call.Body != null)
                {
                    requestMessage.Content = new BodyStreamContent(call.Body);
                }

                foreach (var kv in call.Headers)
                {
                    if (!requestMessage.Headers.TryAddWithoutValidation(kv.Key, kv.Value))
                    {
                        if (requestMessage.Content != null)
                        {
                            requestMessage.Content.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }
                    }
                }
            }

            var bodyStreamContent = requestMessage.Content as BodyStreamContent;

            var sameBody =
                (bodyStreamContent == null && call.Body == null) ||
                (bodyStreamContent != null && ReferenceEquals(bodyStreamContent.Body, call.Body));

            if (!sameBody)
            {
                if (call.Body == null)
                {
                    requestMessage.Content = null;
                }
                else
                {
                    // body stream has been substituted
                    var callBodyContent = new BodyStreamContent(call.Body);
                    if (requestMessage.Content != null)
                    {
                        foreach (var kv in requestMessage.Content.Headers)
                        {
                            callBodyContent.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                        }
                    }
                    requestMessage.Content = callBodyContent;
                }
            }


            requestMessage.Properties["OwinEnvironment"] = call.Environment;
            requestMessage.Properties["CallCompleted"] = call.Completed;
            return requestMessage;
        }


        public static Task<CallParameters> GetCallParameters(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var owinEnvironment = Utils.Get<IDictionary<string, object>>(request.Properties, "OwinEnvironment");
            var callCompleted = Utils.Get<CancellationToken>(request.Properties, "CallCompleted");

            if (owinEnvironment == null)
            {
                throw new InvalidOperationException("Running OWIN components over a Web API server is not currently supported");
            }

            if (request.Content != null)
            {
                return request.Content.ReadAsStreamAsync()
                    .Then(stream => new CallParameters
                    {
                        Environment = owinEnvironment,
                        Headers = new RequestHeadersWrapper(request),
                        Body = stream,
                        Completed = callCompleted
                    });
            }
            else
            {
                return TaskHelpers.FromResult(new CallParameters
                {
                    Environment = owinEnvironment,
                    Headers = new RequestHeadersWrapper(request),
                    Body = null,
                    Completed = callCompleted
                });
            }
        }
    }
}
