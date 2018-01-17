using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Service.ContentCopying
{
    public class BitwiseCopyStrategy : ICopyStrategy
    {
        public async void Copy(HttpResponseMessage source, IOwinResponse target, CancellationTokenSource cancellationTokenSource)
        {
            int read;
            Stream forwardingResponseStream = await source.Content.ReadAsStreamAsync();

            while ((read = forwardingResponseStream.ReadByte()) != -1)
            {
                Console.Write(".");
                cancellationTokenSource.CancelAfter(100000);
                target.Body.WriteByte((byte)read);
            }
        }
    }
}