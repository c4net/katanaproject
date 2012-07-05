using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Katana.WebApi.CallHeaders
{
    public class ResponseHeadersWrapper : MessageHeadersWrapper
    {
        private readonly HttpResponseMessage _message;

        public ResponseHeadersWrapper(HttpResponseMessage message)
        {
            _message = message;
        }

        protected override HttpHeaders MessageHeaders
        {
            get { return ResponseMessage.Headers; }
        }

        protected override HttpHeaders ContentHeaders
        {
            get { return ResponseMessage.Content != null ? ResponseMessage.Content.Headers : null; }
        }

        public HttpResponseMessage ResponseMessage
        {
            get { return _message; }
        }
    }
}
