using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Rob.ReverseProxy.Service.ContentCopying;

namespace Rob.ReverseProxy.Service
{ 
    public class ReverseProxy
    {
        public ReverseProxy(Func<IDictionary<string, object>, Task> next)
        {
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            string sourceRequestInfo = "";
            try
            {
                var outerOwinContext = new OwinContext(env);
                sourceRequestInfo = $"{outerOwinContext.Request.Method} {outerOwinContext.Request.Uri} ({outerOwinContext.Request.Context})";

                var forwardingClient = new HttpClient();

                var host = "localhost:33333";
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