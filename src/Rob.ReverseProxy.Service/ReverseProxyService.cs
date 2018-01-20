#region Copyright & License
// The MIT License (MIT)
// 
// Copyright 2018 INEX Solutions Ltd
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

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
            startOptions.Urls.Add("https://*:9800");  //netsh http add sslcert ipport=0.0.0.0:9800 appid={11C7C3C9-2D89-4E34-98DA-24175BFEFD0E} certhash=5a8580b09ec04120ed41392a75207dfc8ec6493f
            startOptions.Urls.Add("https://*:9900");  //netsh http add sslcert ipport=0.0.0.0:9900 appid={11C7C3C9-2D89-4E34-98DA-24175BFEFD0E} certhash=5a8580b09ec04120ed41392a75207dfc8ec6493f
            startOptions.Urls.Add("https://*:9901");  //netsh http add sslcert ipport=0.0.0.0:9901 appid={11C7C3C9-2D89-4E34-98DA-24175BFEFD0E} certhash=5a8580b09ec04120ed41392a75207dfc8ec6493f
            startOptions.Urls.Add("http://*:9999");
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
