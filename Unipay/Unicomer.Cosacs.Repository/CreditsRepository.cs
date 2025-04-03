using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public partial class CreditsRepository
    {
        #region GetMaxWithdrawalAmountRepository 
        public List<string> GetMaxWithdrawalAmount(string custId)
        {
            List<string> resultList = new List<string>();
            DataSet ds = new DataSet();
            GetMaxWithdrawalAmountRepository GMWAR = new GetMaxWithdrawalAmountRepository();
            resultList = GMWAR.GetMaxWithdrawalAmount(ds, custId); // 0- Message, 1- Status, 2- maxAmount
            string maxAmount = "0.00";
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string creditAvailable = Convert.ToString(ds.Tables[0].Rows[0]["CreditAvailable"]);
                if (!string.IsNullOrWhiteSpace(creditAvailable))
                    maxAmount = Convert.ToString(Math.Round(Convert.ToDecimal(creditAvailable), 2));
            }
            resultList.Add(maxAmount);
            return resultList;
        }
        #endregion

        #region GetPaymentOptionsByAmount 
        public PaymentOptionList GetPaymentOptionsByAmount(string custId, string loanAmount)
        {
            List<string> resultList = new List<string>();
            PaymentOptionList result = new PaymentOptionList();
            DataSet ds = new DataSet();
            GetPaymentOptionsByAmountRepository GPOBA = new GetPaymentOptionsByAmountRepository();
            resultList = GPOBA.GetPaymentOptionsByAmount(ds, custId, loanAmount); // 0- Message, 1- Status, 2- maxAmount
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.content = ds.Tables[0].Rows.OfType<DataRow>()
                     .Select(p => new PaymentOption()
                     {
                         numberOfInstallments = Convert.ToString(p["numberOfInstallments"]),
                         amount = loanAmount,
                         interest = Convert.ToString(p["interest"]).Trim(),
                         interestRateAnnual = Convert.ToString(p["interestRateAnnual"]),
                         effectiveTaxes = Convert.ToString(p["effectiveTaxes"]).Trim()
                     }).ToList();
            }
            result.message = "User not found";
            result.status = "404";

            if (resultList != null && resultList.Count > 0)
            {
                result.message = resultList[0];
                result.status = resultList[1];
            }
            return result;
        }
        #endregion

        #region UpdateCreditInformation 
        public List<string> UpdateCreditInformation(CreditInformation CrINformation)
        {
            string Message = string.Empty;
            int Status=0;
            List<string> _UpdateStatus = new List<string>();
            UpdateCreditInformation uca = new UpdateCreditInformation();
            _UpdateStatus = uca.SaveCreditInformation(CrINformation.CustId, CrINformation.accountNumber, CrINformation.dayOfMonth, Message, Status);
            return _UpdateStatus;

        }
        #endregion
    }
}
