using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using STL.Common.Static;

namespace STL.PL
{
    public class ServerTime
    {
        public static DateTime Request()
        {
            var url = Config.Url + "Time.aspx";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            try
            {
                string serverTimeString;
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    serverTimeString = reader.ReadToEnd().Trim();

                var serverTime =  DateTime.SpecifyKind(DateTime.ParseExact(serverTimeString, "R", null)
                ,
    DateTimeKind.Utc);

                var kind = serverTime.Kind; // will equal DateTimeKind.Utc
                return serverTime.ToLocalTime();
            }
            finally
            {
                response.Close();
            }
        }

        public void RequestAsync()
        {
            var url = Config.Url + "Time.aspx";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.BeginGetResponse(AsyncCallback, request);
        }

        private void AsyncCallback(IAsyncResult asyncResult)
        {
            try
            {
                var response = ((HttpWebRequest)asyncResult.AsyncState).EndGetResponse(asyncResult);

                try
                {
                    string serverTimeString;
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                        serverTimeString = reader.ReadToEnd().Trim();

                    var serverTime = DateTime.ParseExact(serverTimeString, "R", null);
                    //callback(serverTime);
                }
                finally
                {
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine("Status: " + ex.Status);
                Trace.WriteLine(ex.StackTrace);
            }
        }
    }
}
