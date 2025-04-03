using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

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
            string token = string.Empty;
            if (isTokenRequred)
                token = GetToken();

            if (!isLocalUrl)
            {
                if (ConfigurationManager.AppSettings["MVEBaseURL"] != null)
                    baseURL = string.Format("{0}{1}", ConfigurationManager.AppSettings["MVEBaseURL"], postUrl);
            }
            else if (ConfigurationManager.AppSettings["MVELocalBaseURL"] != null)
                baseURL = string.Format("{0}{1}", ConfigurationManager.AppSettings["MVELocalBaseURL"], postUrl);

            //if (ConfigurationManager.AppSettings["MVEAuthToken"] != null)
            //    token = ConfigurationManager.AppSettings["MVEAuthToken"];

            if (!string.IsNullOrWhiteSpace(baseURL))
                _request = (HttpWebRequest)WebRequest.Create(baseURL);

            if (isTokenRequred)
            {
                if (!string.IsNullOrWhiteSpace(token))
                    _request.Headers.Add("Authorization", token);
                else
                    throw new WebException(Convert.ToString(HttpStatusCode.Unauthorized), WebExceptionStatus.ProtocolError);
            }
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


        public string GetToken()
        {
            string baseApiURL = string.Empty;

            string token = ReadWriteToken(DateTime.MinValue, null);

            if (string.IsNullOrWhiteSpace(token))
            {
                if (ConfigurationManager.AppSettings["MVEBaseURL"] != null)
                {
                    string key = "#3MbR&XC3xPP";
                    string user = string.Empty;

                    baseApiURL = string.Format("{0}v1/Tokens", ConfigurationManager.AppSettings["MVEBaseURL"]);

                    if (!string.IsNullOrWhiteSpace(baseApiURL))
                        _request = (HttpWebRequest)WebRequest.Create(baseApiURL);

                    //if (ConfigurationManager.AppSettings["RequestKey"] != null)
                    //key = "#3MbR&XC3xPP"; // ConfigurationManager.AppSettings["RequestKey"];

                    if (ConfigurationManager.AppSettings["RequestUser"] != null)
                        user = ConfigurationManager.AppSettings["RequestUser"];

                    string requestBody = JsonConvert.SerializeObject(new { key = key, user = user });
                    string result = ExecuteWebClient("POST", "application/json; charset=utf-8", requestBody, true);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        TokenResponse tokenModel = JsonConvert.DeserializeObject<TokenResponse>(result);
                        if (tokenModel != null && !string.IsNullOrWhiteSpace(tokenModel.token))
                        {
                            token = tokenModel.token;
                            ReadWriteToken(tokenModel.expiration, tokenModel.token);
                        }
                    }
                    _request = null;
                }
            }
            return string.Format("bearer {0}", token);
        }

        public string ReadWriteToken(DateTime expiration, string token = "")
        {
            ApiTokenKeyRepository TR = new ApiTokenKeyRepository();
            return TR.ReadWriteToken(expiration, token);
        }
        #endregion "Methods"
    }
}
