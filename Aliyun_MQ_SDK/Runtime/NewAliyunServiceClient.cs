using System;
using Aliyun.MQ.Runtime.Internal;
using Aliyun.MQ.Runtime.Internal.Auth;
using Aliyun.MQ.Runtime.Internal.Transform;

namespace Aliyun.MQ.Runtime
{
    public abstract class NewAliyunServiceClient : IDisposable
    {
        internal NewAliyunServiceClient(ServiceCredentials credentials, ClientConfig config)
        {
        }

        internal NewAliyunServiceClient(string accessKeyId, string secretAccessKey, ClientConfig config,
            string stsToken)
            : this(new BasicServiceCredentials(accessKeyId, secretAccessKey, stsToken), config)
        {
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected abstract IServiceSigner CreateSigner();

        protected virtual void Dispose(bool disposing)
        {
            return;
        }

        protected TResponse Invoke<TRequest, TResponse>(TRequest request,
            IMarshaller<IRequest, WebServiceRequest> marshaller, ResponseUnmarshaller unmarshaller)
            where TRequest : WebServiceRequest
            where TResponse : WebServiceResponse
        {
            return null;
        }

        protected void Invoke<TRequest, TResponse>(TRequest request,
            IMarshaller<IRequest, WebServiceRequest> marshaller,
            ResponseUnmarshaller unmarshaller,
            System.Threading.CancellationToken cancellationToken)
            where TRequest : WebServiceRequest
            where TResponse : WebServiceResponse
        {
            return null;
        }
    }
}