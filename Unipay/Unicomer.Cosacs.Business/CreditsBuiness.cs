using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;


namespace Unicomer.Cosacs.Business
{
    public class CreditsBuiness : ICredits
    {
        public JResponse GetMaxWithdrawalAmount(string custId)
        {
            JResponse objJResponse = new JResponse();
            CreditsRepository objCredits = new CreditsRepository();
            List<string> maxAmount = objCredits.GetMaxWithdrawalAmount(custId); 

            if (maxAmount != null && maxAmount.Count > 1 && !string.IsNullOrWhiteSpace(maxAmount[2]))
            {
                objJResponse.Result = JsonConvert.SerializeObject(new { maxAmount = maxAmount[2] });
                objJResponse.Status = true;
                objJResponse.StatusCode = Convert.ToInt16(maxAmount[1]);
                objJResponse.Message = maxAmount[0];
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = Convert.ToInt16(maxAmount[1]);
                objJResponse.Message = maxAmount[0];
            }
            return objJResponse;
        }
        public JResponse GetPaymentOptionsByAmount(string custId, string loanAmount)
        {
            JResponse objJResponse = new JResponse();
            CreditsRepository objCredits = new CreditsRepository();
            PaymentOptionList response = objCredits.GetPaymentOptionsByAmount(custId, loanAmount); 

            if (response != null && response.content != null && response.content.Count > 0 && response.status.Equals("200"))
            {
                objJResponse.Result = JsonConvert.SerializeObject(new { content = response.content });
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = response.message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = Convert.ToInt16(response.status);
                objJResponse.Message = response.message;
            }

            return objJResponse;
        }

        public JResponse UpdateCreditInformation(CreditInformation CrINformation)
        {
            JResponse objJResponse = new JResponse();
            CreditsRepository objCredits = new CreditsRepository();

            List<string> Result = objCredits.UpdateCreditInformation(CrINformation);

            if (Convert.ToInt32(Result[1])==(int)HttpStatusCode.OK)
            {
                objJResponse.Result = "";
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = Result[0];
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                objJResponse.Message = Result[0];
            }
            return objJResponse;
        }
    }
}
