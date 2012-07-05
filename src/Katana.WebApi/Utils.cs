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


        public static HttpResponseMessage GetResponseMessage(ResultParameters result)
        {
            HttpResponseMessage responseMessage;

            var responseHeadersWrapper = result.Headers as ResponseHeadersWrapper;
            if (responseHeadersWrapper != null)
            {
                responseMessage = responseHeadersWrapper.ResponseMessage;
            }
            else
            {
                responseMessage = new HttpResponseMessage((HttpStatusCode)result.Status);

            }
            //responseMessage.Content = new StreamContent(result.Body);
            return responseMessage;
            //if (result.Headers is (ResponseHeadersWrapper))
            //var responseMessage = Get<HttpResponseMessage>(env, "System.Net.Http.HttpResponseMessage");
            //if (responseMessage != null)
            //{
            //    return responseMessage;
            //}

            //var request = Adapters.GetRequestMessage(call);

            //int statusCode;
            //if (status == null || !int.TryParse(status.Substring(0, 3), out statusCode))
            //{
            //    statusCode = 500;
            //}

            //var message = new HttpResponseMessage((HttpStatusCode)statusCode)
            //                  {
            //                      RequestMessage = request,
            //                      Content = new BodyStreamContent(body, GetCancellationToken(env))
            //                  };

            //if (status != null && status.Length > 4)
            //{
            //    message.ReasonPhrase = status.Substring(4);
            //}

            //foreach (var kv in headers)
            //{
            //    if (!message.Headers.TryAddWithoutValidation(kv.Key, kv.Value))
            //    {
            //        message.Content.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            //    }
            //}

            //return message;
        }

        public static ResultParameters GetResultParameters(HttpResponseMessage responseMessage)
        {
            return new ResultParameters
                       {
                           Properties = new Dictionary<string, object>(),
                           Status = (int)responseMessage.StatusCode,
                           Headers = new ResponseHeadersWrapper(responseMessage),
                           Body = (stream, cancel) => responseMessage.Content.CopyToAsync(stream)
                       };
        }
    }
}
