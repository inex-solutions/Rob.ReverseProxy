using System.ServiceProcess;

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
