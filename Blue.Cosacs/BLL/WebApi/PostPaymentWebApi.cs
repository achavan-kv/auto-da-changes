using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data;
using STL.DAL;
using System.Data.SqlClient;
using Blue.Cosacs.BLL.WebApi;
using System.Net.Http;

namespace Blue.Cosacs.BLL.WebApi
{
    public class PostPaymentWebApi
    {
        public string PostPayment(string cosacsAcctId)
        {
            string url = ConfigurationManager.AppSettings["UnipayUrl"] + "invoice-request";

            DCustomer d = new DCustomer();
            DataTable dt = d.GetInvoiceNo(cosacsAcctId);
            string returnresult = "";

            if (dt.Rows.Count > 0)
            {

                ProcessDeliveryModel processDelivery = new ProcessDeliveryModel();

                processDelivery.CosacsAccountId = cosacsAcctId;
                processDelivery.InvoiceNumber = dt.Rows[0][0].ToString();



                using (HttpClient httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(processDelivery);
                    var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                    var responsemsg = httpClient.PostAsync(url, data).Result;

                    returnresult = responsemsg.Content.ReadAsStringAsync().Result;
                    if (responsemsg.IsSuccessStatusCode)
                    {
                        return returnresult;
                    }
                    else
                    {
                        string responseBodyAsText = responsemsg.Content.ReadAsStringAsync().Result;
                        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine); // Insert new lines
                        var errormodel = JsonConvert.DeserializeObject<PostPaymentErrorModel>(responseBodyAsText);
                        //var errormodel2 = JsonConvert.DeserializeObject<PostPaymentErrorModel>(errormodel.generalResponse.First().errors.First().detail);

                        if (errormodel == null)
                        {
                            throw (new STL.Common.STLException("An error has occured while sending the sales order to EMMA for approval. Please contact IT Support or EMMA Team."));

                        }
                       
                        //if (responseBodyAsText.Contains("detail"))
                           
                        //{
                        //    throw (new STL.Common.STLException("EMMA Error Response : " + errormodel.generalResponse.First().errors.First().detail));

                        //}
                        if (errormodel.generalResponse.First().errors.First().detail.Contains("generalResponse"))
                        {
                            var errorModel2 = JsonConvert.DeserializeObject<PostPaymentErrorModel>(errormodel.generalResponse.First().errors.First().detail);
                            if (errorModel2.generalResponse.First().errors != null)
                            {
                                throw (new STL.Common.STLException("EMMA Error Response : " + errorModel2.generalResponse.First().errors.First().detail));
                            }
                            else
                            {
                                throw (new STL.Common.STLException("EMMA Error Response : " + errorModel2.generalResponse.First().responseDescription)); ;
                            }
                        }
                        else
                        {
                            throw (new STL.Common.STLException("An error has occured while sending the sales order to EMMA for approval. Please contact IT Support or EMMA Team."));
                        }
                    }   


                }
            }

            return returnresult;

        }

    }
}

