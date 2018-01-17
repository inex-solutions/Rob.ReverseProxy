using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Service.ContentCopying
{
    public class NonChunkedCopyStrategy : ICopyStrategy
    {
        public async void Copy(HttpResponseMessage source, IOwinResponse target, CancellationTokenSource cancellationTokenSource)
        {
            Console.Write("-");
            await source.Content.CopyToAsync(target.Body);
        }
    }
}