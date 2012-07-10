using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Katana.WebApi.CallContent;
using Katana.WebApi.CallHeaders;
using Owin;

namespace Katana.WebApi
{
    public static class Utils
    {
        internal static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }


        public static HttpResponseMessage GetResponseMessage(HttpRequestMessage request, ResultParameters result, CancellationToken cancellationToken)
        {
            var responseMessage = new HttpResponseMessage((HttpStatusCode)result.Status)
            {
                RequestMessage = request,
                Content = new BodyDelegateContent(result.Body, cancellationToken)
            };
            foreach (var header in result.Headers)
            {
                if (!responseMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    if (responseMessage.Content != null)
                    {
                        responseMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
            object value;
            if (result.Properties != null && result.Properties.TryGetValue("owin.ReasonPhrase", out value))
            {
                responseMessage.ReasonPhrase = Convert.ToString(value);
            }

            return responseMessage;
        }

        public static ResultParameters GetResultParameters(HttpResponseMessage responseMessage)
        {
            return new ResultParameters
                       {
                           Status = (int)responseMessage.StatusCode,
                           Headers = new ResponseHeadersWrapper(responseMessage),
                           Body = (stream, cancel) => responseMessage.Content.CopyToAsync(stream),
                           Properties = new Dictionary<string, object> { { "owin.ReasonPhrase", responseMessage.ReasonPhrase } },
                       };
        }
    }
}
