using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Service
{
    public static class ConversionExtensions
    {
        public static HttpRequestMessage CreateHttpRequestMessageFromRequest(this IOwinRequest srcRequest, string host)
        {
            var forwardingRequest = new HttpRequestMessage();
            forwardingRequest.Method = new HttpMethod(srcRequest.Method);

            foreach (var header in srcRequest.Headers)
            {
                if (!forwardingRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    forwardingRequest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            if (srcRequest.Body != null
                && (!srcRequest.Body.CanSeek
                    || srcRequest.Body.Length > 0))
            {
                var requestStream = new MemoryStream();
                srcRequest.Body.CopyTo(requestStream);
                forwardingRequest.Content = new StreamContent(requestStream);
            }

            forwardingRequest.Headers.Host = host;
            forwardingRequest.RequestUri = new Uri($"http://{host}{srcRequest.Path}{srcRequest.QueryString}");

            return forwardingRequest;
        }

        public static void UpdateOwinFromHttpResponseHeaders(this IOwinResponse owinResponse, HttpResponseMessage srcResponse)
        {
            owinResponse.StatusCode = (int)srcResponse.StatusCode;
            owinResponse.ContentType = srcResponse.Content?.Headers.ContentType?.MediaType;
            owinResponse.ReasonPhrase = srcResponse.ReasonPhrase;

            foreach (var header in srcResponse.Headers)
            {
                owinResponse.Headers[header.Key] = header.Value.FirstOrDefault();
            }

            if (srcResponse.Content != null)
            {
                foreach (var contentHeader in srcResponse.Content.Headers)
                {
                    owinResponse.Headers[contentHeader.Key] = contentHeader.Value.FirstOrDefault();
                }
            }
        }
    }
}