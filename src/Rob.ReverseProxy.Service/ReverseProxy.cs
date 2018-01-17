using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var forwardingRequest = new HttpRequestMessage();
                forwardingRequest.Method = new HttpMethod(outerOwinContext.Request.Method);

                foreach (var header in outerOwinContext.Request.Headers)
                {
                    if (!forwardingRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    {
                        forwardingRequest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }

                if (outerOwinContext.Request.Body != null
                    && (!outerOwinContext.Request.Body.CanSeek
                        || outerOwinContext.Request.Body.Length > 0))
                {
                    var requestStream = new MemoryStream();
                    outerOwinContext.Request.Body.CopyTo(requestStream);
                    forwardingRequest.Content = new StreamContent(requestStream);
                }

                var host = "localhost:33333";
                forwardingRequest.Headers.Host = host;
                forwardingRequest.RequestUri = new Uri($"http://{host}{outerOwinContext.Request.Path}{outerOwinContext.Request.QueryString}");

                var cancellationTokenSource = new CancellationTokenSource(100000);
                var cancellationToken = cancellationTokenSource.Token;
                var forwardingResponse = await forwardingClient.SendAsync(forwardingRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                outerOwinContext.Response.StatusCode = (int)forwardingResponse.StatusCode;
                outerOwinContext.Response.ContentType = forwardingResponse.Content?.Headers.ContentType?.MediaType;
                outerOwinContext.Response.ReasonPhrase = forwardingResponse.ReasonPhrase;

                foreach (var header in forwardingResponse.Headers)
                {
                    outerOwinContext.Response.Headers[header.Key] = header.Value.FirstOrDefault();
                }
            
                if (forwardingResponse.Content != null)
                {
                    foreach (var contentHeader in forwardingResponse.Content.Headers)
                    {
                        outerOwinContext.Response.Headers[contentHeader.Key] = contentHeader.Value.FirstOrDefault();
                    }

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