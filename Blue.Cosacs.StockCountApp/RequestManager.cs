
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;

namespace Blue.Cosacs.StockCountApp
{
    public static class RequestManager
    { 
        private static string _cookie;
        
        public static bool Login(string username, string password)
        {
            bool result = false;
            HttpWebResponse response = null;
            try {                
                var payload = Encoding.UTF8.GetBytes(string.Format("username={0}&password={1}&newpassword", username, password));
                var uri = Settings.AuthHost + Settings.AuthPath;
                var request = (HttpWebRequest)HttpWebRequest.Create(uri);                
               
                request.Method = "POST";
                request.UserAgent = Settings.UserAgent;
                request.PreAuthenticate = true;
                request.Credentials = new NetworkCredential(username, password);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = payload.Length;
                request.AllowWriteStreamBuffering = true;

                using (var newStream = request.GetRequestStream())
                {
                    newStream.Write(payload, 0, payload.Length);
                    newStream.Close();
                }
            
                response = (HttpWebResponse)request.GetResponse();
                if (!(response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Unauthorized))
                {
                    if (response.Headers["Set-Cookie"] != null)
                    {
                        _cookie = response.Headers["Set-Cookie"]
                            .Replace("HttpOnly,", "")
                            .Replace("HttpOnly", "");
                    }
                    result = true;
                }                
                
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }

        public static string MakeRequest(string uriString, string verb, string body)
        {
            if (_cookie != null)
            {
                string result = string.Empty;
                HttpWebResponse response = null;
                try
                {

                    var payload = Encoding.UTF8.GetBytes(string.Format("model={0}", EscapeString(body)));
                    var request = (HttpWebRequest)HttpWebRequest.Create(Settings.AuthHost + uriString);
                    request.Headers.Add("Cookie", _cookie);
                    request.UserAgent = Settings.UserAgent;
                    request.Method = verb;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = payload.Length;
                    request.AllowWriteStreamBuffering = true;

                    if (body.Length > 0)
                    {
                        using (var newStream = request.GetRequestStream())
                        {
                            newStream.Write(payload, 0, payload.Length);
                            newStream.Close();
                        }
                    }

                    response = (HttpWebResponse)request.GetResponse();

                    using (Stream stream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(stream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
                return result;
            }
            
            throw new UnauthorizedAccessException("Unauthorized");            
        }

        private static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string EscapeString(string str)
        {
            var limit = 32766;
            var sb = new StringBuilder();
            var loops = str.Length / limit;

            for (int i = 0; i <= loops; i++)
            {
                if (i < loops)
                {
                    sb.Append(Uri.EscapeDataString(str.Substring(limit * i, limit)));
                }
                else
                {
                    sb.Append(Uri.EscapeDataString(str.Substring(limit * i)));
                }
            }
            return sb.ToString();
        }
    }
}
