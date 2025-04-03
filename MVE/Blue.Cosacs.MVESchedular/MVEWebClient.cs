using System;
using System.Configuration;
using System.IO;
using System.Net;



namespace Unicomer.Cosacs.Business
{
    public class MVEWebClient
    {
        #region "Private Properties"
        //private string _token { get; set; }
        //private string _fullUrl { get; set; }
        private HttpWebRequest _request { get; set; }
        #endregion "Private Properties"
        #region "Constructor"
        public MVEWebClient()
        {
        }
        public MVEWebClient(string postUrl, bool isTokenRequred = true, bool isLocalUrl = false)
        {
            string baseURL = string.Empty;


            if (!isLocalUrl)
            {
                if (ConfigurationSettings.AppSettings["MVEBaseURL"] != null)
                    baseURL = string.Format("{0}{1}", ConfigurationSettings.AppSettings["MVEBaseURL"], postUrl);
            }
            else if (ConfigurationSettings.AppSettings["MVELocalBaseURL"] != null)
                baseURL = string.Format("{0}{1}", ConfigurationSettings.AppSettings["MVELocalBaseURL"], postUrl);

            //if (ConfigurationManager.AppSettings["MVEAuthToken"] != null)
            //    token = ConfigurationManager.AppSettings["MVEAuthToken"];

            if (!string.IsNullOrWhiteSpace(baseURL))
                _request = (HttpWebRequest)WebRequest.Create(baseURL);


        }
        #endregion "Constructor"
        #region "Methods"
        public dynamic ExecuteWebClient(string methodType, string contentType, dynamic data, bool isIncludeSecurityProtocol = true)
        {
            if (!string.IsNullOrWhiteSpace(methodType))
                _request.Method = methodType;
            if (isIncludeSecurityProtocol)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (!string.IsNullOrWhiteSpace(contentType))
                _request.ContentType = contentType;
            //_request.KeepAlive = false;
            //_request.ProtocolVersion = HttpVersion.Version10;
            using (var streamWriter = new StreamWriter(_request.GetRequestStream()))
            {
                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();
            }
            WebResponse response = _request.GetResponse();
            if (response != null)
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    if (streamReader != null)
                        return streamReader.ReadToEnd();
                }
            }
            return null;
        }

        public dynamic ExecuteGetWebClient(string contentType, bool isIncludeSecurityProtocol = true)
        {
            _request.Method = "GET";
            if (isIncludeSecurityProtocol)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            if (!string.IsNullOrWhiteSpace(contentType))
                _request.ContentType = contentType;
            //_request.KeepAlive = false;
            //_request.ProtocolVersion = HttpVersion.Version10;

            //using (var streamWriter = new StreamWriter(_request.GetRequestStream()))
            //{
            //    streamWriter.Write(data);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}
            WebResponse response = null;
            try
            {
                response = _request.GetResponse();
            }
            catch (Exception ex)
            {
            }
            if (response != null)
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    if (streamReader != null)
                        return streamReader.ReadToEnd();
                }
            }
            return null;
        }


        #endregion
    }
}
