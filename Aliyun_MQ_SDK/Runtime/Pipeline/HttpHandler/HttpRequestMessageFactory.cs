using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace Aliyun.MQ.Runtime.Pipeline.HttpHandler
{
    public class HttpRequestMessageFactory : IHttpRequestFactory<HttpContent>
    {

        public IHttpRequest<HttpContent> CreateHttpRequest(Uri requestUri)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Dispose(true);
            // GC.SuppressFinalize(this);
        }
    }
}