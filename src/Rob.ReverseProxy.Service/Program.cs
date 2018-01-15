using System;
using System.ServiceProcess;

namespace Rob.ReverseProxy.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var service = new ReverseProxyService();

            if (Environment.UserInteractive)
            {
                service.StartService(args);
                Console.WriteLine("Service started");
                Console.WriteLine("Press enter to exit");
                service.StopService();
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] {service});
            }
        }
    }
}
