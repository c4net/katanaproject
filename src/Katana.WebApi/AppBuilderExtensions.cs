using System;
using System.Linq;
using System.Net.Http;
using Owin;

namespace Katana.WebApi
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseMessageHandler(this IAppBuilder builder, DelegatingHandler handler)
        {
            return builder.UseMessageHandler(inner =>
            {
                handler.InnerHandler = inner;
                return handler;
            });
        }

        public static IAppBuilder UseMessageHandler(this IAppBuilder builder, Func<HttpMessageHandler, HttpMessageHandler> middleware)
        {
            builder.AddAdapters(Adapters.Adapter1, Adapters.Adapter2);
            return builder.Use(middleware);
        }

        public static IAppBuilder UseMessageHandler<T>(this IAppBuilder builder) where T : HttpMessageHandler
        {
            builder.AddAdapters(Adapters.Adapter1, Adapters.Adapter2);
            return builder.Use<HttpMessageHandler>(app => CreateMessageHandler(typeof(T), new[] { typeof(HttpMessageHandler) }, new object[] { app }));
        }

        public static IAppBuilder UseMessageHandler<T, T1>(this IAppBuilder builder, T1 t1) where T : HttpMessageHandler
        {
            builder.AddAdapters(Adapters.Adapter1, Adapters.Adapter2);
            return builder.Use<HttpMessageHandler>(app => CreateMessageHandler(typeof(T), new[] { typeof(HttpMessageHandler), typeof(T1) }, new object[] { app, t1 }));
        }

        static HttpMessageHandler CreateMessageHandler(Type handlerType, Type[] parameterTypes, object[] parameters)
        {
            // TODO: enumerate constructors, use most appropriate match
            var constructorInfo = handlerType.GetConstructor(parameterTypes);
            if (constructorInfo == null)
            {
                throw new InvalidOperationException(string.Format("{0} does not have matching constructor", handlerType));
            }
            var handler = constructorInfo.Invoke(parameters);
            return (HttpMessageHandler)handler;
        }
    }
}
