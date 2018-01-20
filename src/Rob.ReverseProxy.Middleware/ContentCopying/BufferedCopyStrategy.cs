using System.IO;
using System.Net.Http;
using System.Threading;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Middleware.ContentCopying
{
    public class BufferedCopyStrategy : ICopyStrategy
    {
        public async void Copy(HttpResponseMessage source, IOwinResponse target, CancellationTokenSource cancellationTokenSource)
        {
            int read;
            byte[] buffer = new byte[1024*1024];
            Stream forwardingResponseStream = await source.Content.ReadAsStreamAsync();

            while ((read = forwardingResponseStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                cancellationTokenSource.CancelAfter(100000);
                target.Body.Write(buffer, 0, read);
            }
        }
    }
}