using Owin;

namespace Rob.ReverseProxy.Service
{
    public static class ReverseProxyExtensions
    {
        public static void UseReverseProxy(this IAppBuilder app)
        {
            app.Use<ReverseProxy>();
        }

    }
}