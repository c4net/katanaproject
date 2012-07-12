using System;
using System.Configuration;
using System.Web;
using Gate.Builder;
using Owin;

namespace Katana.Server.AspNet
{
    public class OwinHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            if (OwinApplication.Instance == null)
            {
                return;
            }


            var handleAllRequests = ConfigurationManager.AppSettings["owin:HandleAllRequests"];

            if (string.Equals("True", handleAllRequests, StringComparison.InvariantCultureIgnoreCase))
            {
                var handler = new OwinHttpHandler(
                    pathBase: Utils.NormalizePath(HttpRuntime.AppDomainAppVirtualPath),
                    appAccessor: OwinApplication.Accessor);

                context.PostResolveRequestCache += (sender, e) => context.Context.RemapHandler(handler);
            }
        }

        public void Dispose()
        {
        }
    }
}
