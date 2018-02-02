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

using System.Net;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Rob.ReverseProxy.Middleware;
using Rob.ReverseProxy.Middleware.Configuration;

namespace Rob.ReverseProxy.Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseReverseProxy(new ReverseProxyConfiguration
            {
                ForwardingEntries = new[]
                {
                    new ForwardingEntry{SourceUrlMatch = @"^https?\://.*:9800/api.*$", TargetHost = "localhost:33333", AllowOnlyRoles = new [] {"Reverse Proxy Users"}},
                    new ForwardingEntry{SourceUrlMatch = @"^https?\://.*:9800", TargetHost = "localhost:9090", AllowOnlyRoles = new [] {"Reverse Proxy Users"}},
                    new ForwardingEntry{SourceUrlMatch = @"^https?\://.*:9901", TargetHost = "localhost:33633", AllowOnlyRoles = new [] {"Reverse Proxy Users"}},
                }
            });

            HttpListener listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
                FileSystem = new PhysicalFileSystem("static-files")
            };
            app.UseFileServer(options);

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }
}