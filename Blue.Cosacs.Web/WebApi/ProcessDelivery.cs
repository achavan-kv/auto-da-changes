using Blue.Cosacs.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Blue.Cosacs.Web.WebApi
{
    public class ProcessDelivery
    {
        static public string PostProcessDelivery(ProcessDeliveryCommand processDeliveryCommand)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["UnipayUrl"] + "delivery");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var myContent = JsonConvert.SerializeObject(processDeliveryCommand);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response =  client.PostAsync(client.BaseAddress,byteContent).Result;
                string result = "Success";
                if (!response.IsSuccessStatusCode)
                {
                    string responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                    result = "delivery: " + response.StatusCode + " : " + response.ReasonPhrase + " : " + responseBodyAsText;

                }
                return result;
                
            }
            //string HtmlResult = "";


            //string URI = ConfigurationManager.AppSettings["UnipayUrl"] + "delivery";
           
            //using (WebClient wc = new WebClient())
            //{
            //    try
            //    {
            //        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            //        string data = JsonConvert.SerializeObject(processDeliveryCommand);
            //        HtmlResult = wc.UploadString(URI, data);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}


            //return HtmlResult;


        }
    }
}