using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Owin;

namespace Katana.WebApi.CallContent
{
    public class BodyDelegateContent : HttpContent
    {
        private readonly BodyDelegate _body;
        private readonly CancellationToken _cancel;

        public BodyDelegateContent(BodyDelegate body, CancellationToken cancel)
        {
            _body = body;
            _cancel = cancel;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _body.Invoke(stream, _cancel);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}