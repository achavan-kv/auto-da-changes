using Blue.Cosacs.BLL.WebApi;
using Newtonsoft.Json;
using STL.DAL;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace STL.BLL.WebApi
{
    public class PaymentWebApi
    {
        public string GetLoanSchedule(string cosacsAcctId)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["UnipayUrl"] + "loan-schedule?cosacsAccountId=" + cosacsAcctId.Trim();

                using (HttpClient httpClient = new HttpClient())
                {
                    var responsemsg = httpClient.GetAsync(url).Result;

                    if (responsemsg != null && responsemsg.IsSuccessStatusCode)
                    {
                        return "Success";
                    }
                    else
                    {
                        try
                        {
                            string responseBodyAsText = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                            if (!string.IsNullOrEmpty(responseBodyAsText))
                            {
                                responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);
                                var errormodel = JsonConvert.DeserializeObject<PaymentErrorModel>(responseBodyAsText);
                                if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.FirstOrDefault() != null && errormodel.GeneralResponse.FirstOrDefault().Errors != null && errormodel.GeneralResponse.FirstOrDefault().Errors.FirstOrDefault() != null)
                                {
                                    if (errormodel.GeneralResponse.First().Errors.First().ErrorReason != null && errormodel.GeneralResponse.First().Errors.First().ErrorCode > 0)
                                    {
                                        return cosacsAcctId + ":" + "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().ErrorReason;
                                    }
                                    else if (errormodel.GeneralResponse.First().Errors.First().Detail != null && errormodel.GeneralResponse.First().Errors.First().Code != null)
                                    {
                                        return cosacsAcctId + ":" + "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().Detail;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        return cosacsAcctId + ":" + "An error has occurred while adjusting the payment in MAMBU. Please contact IT Support or MAMBU Team.";
                    }
                }
            }

            catch (WebException ex)
            {
                using (StreamReader r = new StreamReader(ex.Response.GetResponseStream()))
                {
                    string response = r.ReadToEnd(); // access the reponse message
                }
                throw new Exception(ex.Message);
            }

        }

        public bool SavePayment(PaymentCommand paymentCommand, decimal settlement, out string errorMessage, out string errorInfo)
        {
            errorMessage = "";
            errorInfo = "";
            bool resultVal = false;

            string url = ConfigurationManager.AppSettings["UnipayUrl"] + "repayment";
            string json;
            string result = "";

            if (settlement == paymentCommand.Amount)
            {
                url = ConfigurationManager.AppSettings["UnipayUrl"] + "loan-payoff";
                HttpResponseMessage responsemsg = null;
                using (HttpClient httpClient = new HttpClient())
                {
                    LoanPayOffCommand loanPayOffCommand = new LoanPayOffCommand();
                    loanPayOffCommand.CosacsAccountId = paymentCommand.CosacsAccountId;

                    json = JsonConvert.SerializeObject(loanPayOffCommand);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    responsemsg = httpClient.PostAsync(url, data).Result;

                    result = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                }

                if (!responsemsg.IsSuccessStatusCode)
                {
                    resultVal = false;
                    try
                    {
                        string responseBodyAsText = errorMessage = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);
                        if (!string.IsNullOrEmpty(responseBodyAsText))
                        {
                            var errormodel = JsonConvert.DeserializeObject<PaymentErrorModel>(responseBodyAsText);
                            if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.FirstOrDefault() != null && errormodel.GeneralResponse.FirstOrDefault().Errors != null && errormodel.GeneralResponse.FirstOrDefault().Errors.FirstOrDefault() != null)
                            {
                                if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().ErrorReason != null && errormodel.GeneralResponse.First().Errors.First().ErrorCode > 0)
                                {
                                    errorInfo = "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().ErrorReason;
                                    return resultVal;
                                }
                                else if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().Detail != null && errormodel.GeneralResponse.First().Errors.First().Code != null)
                                {
                                    errorInfo = "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().Detail;
                                    return resultVal;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    errorInfo = "An error has occurred while adjusting the payment in MAMBU. Please contact IT Support or MAMBU Team.";
                    return resultVal;
                }
                else
                {
                    resultVal = true;
                }
            }
            else
            {
                HttpResponseMessage responsemsg = null;
                using (HttpClient httpClient = new HttpClient())
                {
                    json = JsonConvert.SerializeObject(paymentCommand);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    responsemsg = httpClient.PostAsync(url, data).Result;

                    result = responsemsg.Content.ReadAsStringAsync().Result;
                    if (!responsemsg.IsSuccessStatusCode)
                    {
                        resultVal = false;
                        try
                        {
                            string responseBodyAsText = errorMessage = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);
                            if (!string.IsNullOrEmpty(responseBodyAsText))
                            {
                                var errormodel = JsonConvert.DeserializeObject<PaymentErrorModel>(responseBodyAsText);
                                if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.FirstOrDefault() != null && errormodel.GeneralResponse.FirstOrDefault().Errors != null && errormodel.GeneralResponse.FirstOrDefault().Errors.FirstOrDefault() != null)
                                {
                                    if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().ErrorReason != null && errormodel.GeneralResponse.First().Errors.First().ErrorCode > 0)
                                    {
                                        errorInfo = "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().ErrorReason;
                                        return resultVal;
                                    }
                                    else if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().Detail != null && errormodel.GeneralResponse.First().Errors.First().Code != null)
                                    {
                                        errorInfo = "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().Detail;
                                        return resultVal;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                        errorInfo = "An error has occurred while adjusting the payment in MAMBU. Please contact IT Support or MAMBU Team.";
                        return resultVal;
                    }
                    else
                    {
                        resultVal = true;
                    }
                }
            }

            return resultVal;
        }

        public DataTable GetMambuAccountsForPayment(string customerID, string accountID)
        {
            DCustomer cust = new DCustomer();
            DataTable loanAccountIds = cust.GetMambuAccountsForPayment(customerID, accountID);
            return loanAccountIds;
        }

        //adjustpayment 
        public bool PostLoanScheduleAdjustPayment(string cosacsAcctId, int transRefNo, out string error)
        {
            string url = ConfigurationManager.AppSettings["UnipayUrl"] + "adjust-payment";
            error = "";
            bool result = false;

            DCustomer d = new DCustomer();
            DataTable dt = d.GetTransactionId(cosacsAcctId, transRefNo);//get trans id
            string returnresult = "";

            if (dt.Rows.Count > 0)
            {
                AdjustPaymentModel adjustPayment = new AdjustPaymentModel();
                adjustPayment.CosacsAccountId = cosacsAcctId;
                adjustPayment.LoanTransactionId = dt.Rows[0][0].ToString();//transRefNo.ToString();//get the transactionid
                adjustPayment.Notes = "Adjust Payment";

                using (HttpClient httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(adjustPayment);
                    var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                    var responsemsg = httpClient.PostAsync(url, data).Result;

                    returnresult = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                    if (responsemsg != null && responsemsg.IsSuccessStatusCode)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        try
                        {
                            string responseBodyAsText = responsemsg != null ? responsemsg.Content.ReadAsStringAsync().Result : string.Empty;
                            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);
                            if (!string.IsNullOrEmpty(responseBodyAsText))
                            {
                                var errormodel = JsonConvert.DeserializeObject<PaymentErrorModel>(responseBodyAsText);
                                if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.FirstOrDefault() != null && errormodel.GeneralResponse.FirstOrDefault().Errors != null && errormodel.GeneralResponse.FirstOrDefault().Errors.FirstOrDefault() != null)
                                {
                                    if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().ErrorReason != null && errormodel.GeneralResponse.First().Errors.First().ErrorCode > 0)
                                    {
                                        error = cosacsAcctId + ":" + "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().ErrorReason;
                                        return result;
                                    }
                                    else if (errormodel != null && errormodel.GeneralResponse != null && errormodel.GeneralResponse.First().Errors.First().Detail != null && errormodel.GeneralResponse.First().Errors.First().Code != null)
                                    {
                                        error = cosacsAcctId + ":" + "MAMBU Error Response : " + errormodel.GeneralResponse.First().Errors.First().Detail;
                                        return result;
                                    }
                                }
                            }

                        }
                        catch (Exception)
                        {

                        }
                        error = "An error has occurred while adjusting the payment in MAMBU. Please contact IT Support or MAMBU Team.";
                        return result;
                    }

                }

            }
            return result;
        }
    }

    public class PaymentCommand
    {
        public string CosacsAccountId { get; set; }
        public decimal Amount { get; set; }
    }

    public class LoanPayOffCommand
    {
        public string CosacsAccountId { get; set; }
    }
}
