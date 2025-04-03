using System;

namespace Blue.Cosacs.Shared.Net
{
    //public interface IServiceRequest { }
    //public interface IServiceResponse { }

    public interface IServiceClientAsync
    {
        void Call<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess);
            //where TRequest: IServiceRequest
            //where TResponse: IServiceResponse;

        void Call<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError);
            //where TRequest : IServiceRequest
            //where TResponse : IServiceResponse;
    }
}
