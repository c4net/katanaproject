using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Katana.WebApi.CallContent;
using Katana.WebApi.CallHeaders;
using Owin;

namespace Katana.WebApi
{
    public class CallMessageHandler
    {
        private readonly HttpMessageInvoker _invoker;

        public CallMessageHandler(HttpMessageHandler handler)
        {
            _invoker = new HttpMessageInvoker(handler, disposeHandler: true);
        }

        public static AppDelegate Create(HttpMessageHandler handler)
        {
            return new CallMessageHandler(handler).Send;
        }

        public Task<ResultParameters> Send(CallParameters call)
        {
            return _invoker.SendAsync(Adapters.GetRequestMessage(call), call.Completed)
                .Then(responseMessage => Utils.GetResultParameters(responseMessage), call.Completed);
        }
    }
}

