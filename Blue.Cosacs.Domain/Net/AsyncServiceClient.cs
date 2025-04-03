using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Blue.Cosacs.Shared.Net
{
    public delegate string TextSerializerDelegate(object dto);
    public delegate void StreamSerializerDelegate(object dto, Stream toStream);
    public delegate object TextDeserializerDelegate(Type type, string dto);
    public delegate object StreamDeserializerDelegate(Type type, Stream fromStream);

    /// <summary>
    /// Need to provide async request options
    /// http://msdn.microsoft.com/en-us/library/86wf6409(VS.71).aspx
    /// </summary>
    internal class AsyncServiceClient
    {
        public static Action<HttpWebRequest> HttpWebRequestFilter { get; set; }

        private const int BufferSize = 4096;

        internal class RequestState<TResponse> : IDisposable
        {
            public RequestState()
            {
                BufferRead = new byte[BufferSize];
                TextData = new StringBuilder();
                BytesData = new MemoryStream(BufferSize);
                WebRequest = null;
                ResponseStream = null;
            }

            public string Url;
            public StringBuilder TextData;
            public MemoryStream BytesData;
            public byte[] BufferRead;
            public object Request;
            public HttpWebRequest WebRequest;
            public HttpWebResponse WebResponse;
            public Stream ResponseStream;
            public int Completed;
            public Timer Timer;
            public Action<TResponse> OnSuccess;
            public Action<TResponse, Exception> OnError;

            public void HandleError(TResponse response, Exception ex)
            {
                if (OnError != null)
                    OnError(response, ex);
            }

            public void StartTimer(TimeSpan timeOut)
            {
                this.Timer = new Timer(this.TimedOut, this, (int)timeOut.TotalMilliseconds, System.Threading.Timeout.Infinite);
            }

            public void TimedOut(object state)
            {
                if (Interlocked.Increment(ref Completed) == 1)
                {
                    if (this.WebRequest != null)
                    {
                        this.WebRequest.Abort();
                    }
                }
                this.Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                this.Timer.Dispose();
                this.Dispose();
            }

            public void Dispose()
            {
                if (this.BytesData == null) return;
                this.BytesData.Dispose();
                this.BytesData = null;
            }
        }

        public AsyncServiceClient()
        {
            this.Timeout = TimeSpan.FromSeconds(60);
        }

        public TimeSpan Timeout { get; set; }

        public string ContentType { get; set; }

        public StreamSerializerDelegate StreamSerializer { get; set; }

        public StreamDeserializerDelegate StreamDeserializer { get; set; }

        public void SendAsync<TRequest, TResponse>(string absoluteUrl, TRequest request,
            Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            if (absoluteUrl == null)
                throw new ArgumentNullException("absoluteUrl");
            if (request == null)
                throw new ArgumentNullException("request");
            if (onSuccess == null)
                throw new ArgumentNullException("onSuccess");
            //if (onError == null)
            //    throw new ArgumentNullException("onError");
            const string httpMethod = "POST";
            var requestState = SendWebRequest(httpMethod, absoluteUrl, request, onSuccess, onError);
        }

        private RequestState<TResponse> SendWebRequest<TResponse>(string httpMethod, string absoluteUrl, object request,
            Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            if (httpMethod == null)
                throw new ArgumentNullException("httpMethod");

            var requestUri = absoluteUrl;
            var webRequest = (HttpWebRequest)WebRequest.Create(requestUri);

            var requestState = new RequestState<TResponse>
            {
                Url = requestUri,
                WebRequest = webRequest,
                Request = request,
                OnSuccess = onSuccess,
                OnError = onError,
            };
            requestState.StartTimer(this.Timeout);

            webRequest.Accept = string.Format("{0}, */*", ContentType);
            webRequest.Method = httpMethod;

            if (HttpWebRequestFilter != null)
                HttpWebRequestFilter(webRequest);

            webRequest.ContentType = ContentType;
            Authentication(webRequest);
            webRequest.BeginGetRequestStream(RequestCallback<TResponse>, requestState);

            return requestState;
        }

        private static Uri BaseUri
        {
            get { return new Uri(STL.Common.Static.Config.Url); }
        }

        private void Authentication(HttpWebRequest request)
        {
            var header = Header.Client();
            request.Headers.Add(Header.HttpHeaderUser, header.User);
            request.Headers.Add(Header.HttpHeaderPassword, header.Password);
            request.Headers.Add(Header.HttpHeaderCulture, header.Culture);
            request.Headers.Add(Header.HttpHeaderCountryCode, header.CountryCode);
            request.Headers.Add(Header.HttpHeaderVersion, header.Version);

            if (STL.Common.Static.Credential.Cookie != null)
            {
                if (request.CookieContainer == null)
                    request.CookieContainer = new CookieContainer();

                request.CookieContainer.SetCookies(BaseUri, STL.Common.Static.Credential.Cookie);
            }
        }

        private void SetCookie(HttpWebResponse response)
        {
            var cookieString = response.Headers.Get("Set-Cookie");
            if (cookieString != null)
                Header.SetCookie(cookieString);
        }

        private void RequestCallback<T>(IAsyncResult asyncResult)
        {
            var requestState = (RequestState<T>)asyncResult.AsyncState;
            try
            {
                var req = requestState.WebRequest;
                var postStream = req.EndGetRequestStream(asyncResult);

                StreamSerializer(requestState.Request, postStream);

                postStream.Close();

                var result = requestState.WebRequest.BeginGetResponse(ResponseCallback<T>, requestState);

                PostInvoke(requestState.WebRequest);
            }
            catch (Exception ex)
            {
                HandleResponseError(ex, requestState);
            }
        }

        public static void PostInvoke(HttpWebRequest client)
        {
            if (client.CookieContainer != null)
            {
                SaveNewCookieForLater(client.CookieContainer);
            }
        }

        private static void SaveNewCookieForLater(CookieContainer cookieContainer)
        {
            var cookieCollection = cookieContainer.GetCookies(BaseUri);
            foreach (System.Net.Cookie cookie in cookieCollection)
            {
                if (IsNewCookie(cookie))
                {
                    STL.Common.Static.Credential.Cookie = cookie.ToString();
                }
            }
        }

        private static bool IsNewCookie(System.Net.Cookie cookie)
        {
            var possibleNewCookie = cookie.ToString();
            return !STL.Common.Static.Credential.Cookie.StartsWith(possibleNewCookie);
        }

        private void ResponseCallback<T>(IAsyncResult asyncResult)
        {
            var requestState = (RequestState<T>)asyncResult.AsyncState;
            try
            {
                var webRequest = requestState.WebRequest;
                requestState.WebResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);

                SetCookie(requestState.WebResponse);
                // Read the response into a Stream object.
                var responseStream = requestState.WebResponse.GetResponseStream();
                requestState.ResponseStream = responseStream;

                //var asyncRead = 
                responseStream.BeginRead(requestState.BufferRead, 0, BufferSize, ReadCallBack<T>, requestState);
            }
            catch (Exception e)
            {
                HandleResponseError(e, requestState);
            }
        }

        private void ReadCallBack<T>(IAsyncResult asyncResult)
        {
            var requestState = (RequestState<T>)asyncResult.AsyncState;
            try
            {
                var responseStream = requestState.ResponseStream;
                var read = responseStream.EndRead(asyncResult);

                if (read > 0)
                {

                    requestState.BytesData.Write(requestState.BufferRead, 0, read);
                    var nextAsyncResult = responseStream.BeginRead(
                        requestState.BufferRead, 0, BufferSize, ReadCallBack<T>, requestState);

                    return;
                }

                Interlocked.Increment(ref requestState.Completed);

                var response = default(T);
                try
                {
                    requestState.BytesData.Position = 0;
                    using (var reader = requestState.BytesData)
                    {
                        response = (T)this.StreamDeserializer(typeof(T), reader);
                    }

                    if (requestState.OnSuccess != null)
                        requestState.OnSuccess(response);
                }
                catch (Exception ex)
                {
                    //Log.Debug(string.Format("Error Reading Response Error: {0}", ex.Message), ex);
                    Trace.TraceError("Error Reading Response Error: {0}", ex.Message);
                    requestState.HandleError(default(T), ex);
                }
                finally
                {
                    responseStream.Close();
                }
            }
            catch (Exception ex)
            {
                HandleResponseError(ex, requestState);
            }
        }

        private void HandleResponseError<TResponse>(Exception exception, RequestState<TResponse> requestState)
        {
            var webEx = exception as WebException;
            if (webEx != null && webEx.Status == WebExceptionStatus.ProtocolError)
            {
                var errorResponse = ((HttpWebResponse)webEx.Response);
                Trace.TraceError(webEx.Message);
                Trace.TraceError("Status Code : {0}", errorResponse.StatusCode);
                Trace.TraceError("Status Description : {0}", errorResponse.StatusDescription);
                //Log.Error(webEx);
                //Log.DebugFormat("Status Code : {0}", errorResponse.StatusCode);
                //Log.DebugFormat("Status Description : {0}", errorResponse.StatusDescription);

                try
                {
                    using (var stream = errorResponse.GetResponseStream())
                    {
                        //var response = (TResponse)this.StreamDeserializer(typeof(TResponse), stream);
                        var response = ((ResponseStatusWrapper)this.StreamDeserializer(typeof(ResponseStatusWrapper), stream)).ResponseStatus;
                        requestState.HandleError(default(TResponse), new ServerException(response));
                    }
                }
                catch (JsonReaderException ex)
                {
                    Trace.TraceError(ex.Message);
                    requestState.HandleError(default(TResponse), ex);
                }
                catch (WebException ex)
                {
                    // Oh, well, we tried
                    //Log.Debug(string.Format("WebException Reading Response Error: {0}", ex.Message), ex);
                    Trace.TraceError(ex.Message);
                    requestState.HandleError(default(TResponse), ex);
                }
                return;
            }

            var authEx = exception as AuthenticationException;
            if (authEx != null)
            {
                var customEx = WebRequestUtils.CreateCustomException(requestState.Url, authEx);

                //Log.Debug(string.Format("AuthenticationException: {0}", customEx.Message), customEx);
                Trace.TraceError("AuthenticationException: {0}", customEx.Message);
                requestState.HandleError(default(TResponse), authEx);
            }

            // Log.Debug(string.Format("Exception Reading Response Error: {0}", exception.Message), exception);
            Trace.TraceError("Exception Reading Response Error: {0}", exception.Message);
            requestState.HandleError(default(TResponse), exception);
        }

        public void Dispose() { }
    }
}
