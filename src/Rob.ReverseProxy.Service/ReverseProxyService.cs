using System.ServiceProcess;
using Microsoft.Owin.Hosting;

namespace Rob.ReverseProxy.Service
{
    public partial class ReverseProxyService : ServiceBase
    {
        public ReverseProxyService()
        {
            InitializeComponent();
        }

        public void StartService(string[] args)
        {
            var startOptions = new StartOptions();
            startOptions.Urls.Add("http://*:9900");
            WebApp.Start<Startup>(startOptions);
        }

        public void StopService()
        {

        }

        protected override void OnStart(string[] args)
        {
            StartService(args);
        }

        protected override void OnStop()
        {
            StopService();
        }
    }
}
