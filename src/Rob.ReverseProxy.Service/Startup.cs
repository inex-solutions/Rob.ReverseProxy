using Owin;

namespace Rob.ReverseProxy.Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseReverseProxy();
        }
    }
}