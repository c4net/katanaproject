using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Katana.Server.AspNet.CallEnvironment;
using Katana.Server.AspNet.CallHeaders;
using Owin;

namespace Katana.Server.AspNet
{
    public partial class OwinCallContext
    {
        private HttpContextBase _httpContext;
        private HttpRequestBase _httpRequest;
        private HttpResponseBase _httpResponse;

        public void Execute(RequestContext requestContext, string requestPathBase, string requestPath, AppDelegate app)
        {
            _httpContext = requestContext.HttpContext;
            _httpRequest = _httpContext.Request;
            _httpResponse = _httpContext.Response;

            var requestQueryString = String.Empty;
            if (_httpRequest.Url != null)
            {
                var query = _httpRequest.Url.Query;
                if (query.Length > 1)
                {
                    // pass along the query string without the leading "?" character
                    requestQueryString = query.Substring(1);
                }
            }

            var environment = new AspNetDictionary
            {
                OwinVersion = "1.0",
                HttpVersion = _httpRequest.ServerVariables["SERVER_PROTOCOL"],
                RequestScheme = _httpRequest.IsSecureConnection ? "https" : "http",
                RequestMethod = _httpRequest.HttpMethod,
                RequestPathBase = requestPathBase,
                RequestPath = requestPath,
                RequestQueryString = requestQueryString,

                HostDisableResponseBuffering = DisableResponseBuffering,
                HostUser = _httpContext.User,

                ServerVariableLocalAddr = _httpRequest.ServerVariables["LOCAL_ADDR"],
                ServerVariableRemoteAddr = _httpRequest.ServerVariables["REMOTE_ADDR"],
                ServerVariableRemoteHost = _httpRequest.ServerVariables["REMOTE_HOST"],
                ServerVariableRemotePort = _httpRequest.ServerVariables["REMOTE_PORT"],
                ServerVariableServerPort = _httpRequest.ServerVariables["SERVER_PORT"],

                RequestContext = requestContext,
                HttpContextBase = _httpContext,
            };

            var call = new CallParameters
            {
                Environment = environment,
                Headers = AspNetRequestHeaders.Create(_httpRequest),
                Body = _httpRequest.InputStream,
                Completed = CallDisposed,
            };

            app.Invoke(call)
                .Then(r => OnResult(r))
                .Catch(info =>
                {
                    OnFault(info.Exception);
                    return info.Handled();
                });
        }

        private void OnResult(ResultParameters result)
        {
            _httpResponse.StatusCode = result.Status;

            object reasonPhrase;
            if (result.Properties != null && result.Properties.TryGetValue("owin.ReasonPhrase", out reasonPhrase))
            {
                _httpResponse.StatusDescription = Convert.ToString(reasonPhrase);
            }

            foreach (var header in result.Headers)
            {
                foreach (var value in header.Value)
                {
                    _httpResponse.AddHeader(header.Key, value);
                }
            }

            if (result.Body != null)
            {
                result.Body.Invoke(_httpResponse.OutputStream, CallDisposed)
                    .Then(() => OnEnd(null))
                    .Catch(info =>
                    {
                        OnEnd(info.Exception);
                        return info.Handled();
                    });
            }
            else
            {
                Complete(false, null);
            }
        }

        private void OnFault(Exception ex)
        {
            Complete(false, ex);
        }

        private void OnEnd(Exception ex)
        {
            Complete(false, ex);
        }
    }
}
