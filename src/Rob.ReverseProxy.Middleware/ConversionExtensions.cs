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

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin;

namespace Rob.ReverseProxy.Middleware
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