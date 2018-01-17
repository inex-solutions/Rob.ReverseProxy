using System.Net.Http;
using System.Threading;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Service.ContentCopying
{
    public interface ICopyStrategy
    {
        void Copy(HttpResponseMessage source, IOwinResponse target, CancellationTokenSource cancellationTokenSource);
    }
}