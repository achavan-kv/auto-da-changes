using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Blue.Cosacs.Shared.Services
{
    partial class Client
    {
        private static readonly Net.DefaultServiceClientAsync client = new Net.DefaultServiceClientAsync();

        protected static void Execute<TRequest, TResponse>(TRequest request, 
            Action<TResponse> onSuccess, Action<TResponse, Exception> onException, Control control = null)
        {
            Client.OnBegin(request);
            client.Call<TRequest, TResponse>(request, 
                                response =>
                                {
                                    if (control != null)
                                        control.BeginInvoke(new InvokeDelegate(delegate
                                            {
                                                Client.OnSuccess(response, onSuccess);
                                            }));
                                    else Client.OnSuccess(response, onSuccess);
                                },
                                (response, exception) => Client.OnException(response, exception, onException));
        }

        private delegate void InvokeDelegate();

        private static void OnBegin<TRequest>(TRequest request)
        {
            if (Begin != null)
                Begin(request);
        }

        private static void OnSuccess<TResponse>(TResponse response, Action<TResponse> action)
        {
            if (Success != null)
                Success(response);

            if (action != null)
                action(response);
        }

        private static void OnException<TResponse>(TResponse response, Exception ex, Action<TResponse, Exception> action)
        {
            if (action != null)
                action(response, ex);
            
            if (Exception != null)
                Exception(response, ex, action != null);
            else 
                throw ex; // general client exception handling stuff
        }

        public static event EventHandlerStart Begin;
        public static event EventHandlerSuccess Success;
        public static event EventHandlerException Exception;

        public delegate void EventHandlerStart(object request);
        public delegate void EventHandlerSuccess(object response);
        public delegate void EventHandlerException(object response, System.Exception ex, bool handled);
    }
}
