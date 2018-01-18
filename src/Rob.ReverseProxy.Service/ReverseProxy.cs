﻿#region Copyright & License
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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Rob.ReverseProxy.Service.Configuration;
using Rob.ReverseProxy.Service.ContentCopying;

namespace Rob.ReverseProxy.Service
{ 
    public class ReverseProxy
    {
        private readonly ForwardingEntryMap _forwardingEntryMap;

        public ReverseProxy(Func<IDictionary<string, object>, Task> next, ReverseProxyConfiguration configuration)
        {
            _forwardingEntryMap = new ForwardingEntryMap(configuration.ForwardingEntries);
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            string sourceRequestInfo = "";
            try
            {
                var outerOwinContext = new OwinContext(env);
                sourceRequestInfo = $"{outerOwinContext.Request.Method} {outerOwinContext.Request.Uri} ({outerOwinContext.Request.Context})";

                string host;
                if (!_forwardingEntryMap.TryGetForwardingEntry(outerOwinContext.Request.Host.Value, out host)
                    && !_forwardingEntryMap.TryGetForwardingEntry("*", out host))
                {
                    outerOwinContext.Response.StatusCode = 503;
                    outerOwinContext.Response.ReasonPhrase = "Service Unavailable";
                    return;
                }

                var forwardingClient = new HttpClient();

                var forwardingRequest = outerOwinContext.Request.CreateHttpRequestMessageFromRequest(host);
                
                var cancellationTokenSource = new CancellationTokenSource(100000);
                var cancellationToken = cancellationTokenSource.Token;
                var forwardingResponse = await forwardingClient.SendAsync(forwardingRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                outerOwinContext.Response.UpdateOwinFromHttpResponseHeaders(forwardingResponse);
            
                if (forwardingResponse.Content != null)
                {
                    var copyStrategy = CopyStrategyFactory.GetCopyStrategy(forwardingResponse);
                    copyStrategy.Copy(forwardingResponse, outerOwinContext.Response, cancellationTokenSource);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UNHANDLED EXCEPTION:");
                Console.WriteLine(sourceRequestInfo);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}