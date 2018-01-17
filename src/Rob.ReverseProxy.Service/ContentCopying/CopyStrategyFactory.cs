using System.Net.Http;

namespace Rob.ReverseProxy.Service.ContentCopying
{
    public class CopyStrategyFactory
    {
        public static ICopyStrategy GetCopyStrategy(HttpResponseMessage responseMessage)
        {
            return (responseMessage.Headers.TransferEncodingChunked.HasValue && responseMessage.Headers.TransferEncodingChunked.Value)
                ? (ICopyStrategy)new BitwiseCopyStrategy()
                : (ICopyStrategy)new NonChunkedCopyStrategy();
        }
    }
}