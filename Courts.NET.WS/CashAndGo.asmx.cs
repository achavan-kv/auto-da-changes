using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using STL.WS;
using STL.Common.Constants.AccountTypes;
using Blue.Cosacs.Shared;
using System.Xml;
using STL.Common.Constants.Tags;
using System.Web.Caching;
using STL.Common;
using STL.DAL;

namespace Blue.Cosacs.Web
{
    /// <summary>
    /// Summary description for Customers
    /// </summary>
    [WebService(Namespace = "http://www.bluebridgeltd.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CashAndGo : System.Web.Services.WebService
    {
        WAccountManager accountManager;
        Customers customers;

        public CashAndGo()
        {
            accountManager = new WAccountManager();
            customers = new Customers();

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public SaveNewAccountWithWarrantyResult SaveNewAccountWithWarranty(
            CustomerResult.Parameters.AccountAssotiation accAssParams, 
            WAccountManager.SaveNewAccountParameters savNAccParams)
        {
            var error = "";
            
            var propResult = savNAccParams.PropResult;
            var dateProp = savNAccParams.DateProp;
            var bureauFailure = "";
            var storeCardTransRefNo = 0;
            var referralReasons = string.Empty; //IP - 15/03/11 - #3314 - CR1245

            var _PTWarrantyAccountNo = "";

            accountManager.GenerateAccountNumber(accAssParams.CountryCode, accAssParams.BranchNo, AT.Special, false, out _PTWarrantyAccountNo, out error, userid: savNAccParams.SalesPerson);

            var agreementNo = accountManager.GetBuffNo(accAssParams.BranchNo, out error);

            var contractNo = accountManager.AutoWarranty(accAssParams.BranchNo.ToString(), out error);

            savNAccParams.LineItems.ChildNodes.Cast<XmlNode>()
                .First(n => n.Attributes[Tags.Type].Value == "Warranty")
                    .Attributes[Tags.ContractNumber].Value = contractNo;

            savNAccParams.AccountNumber = _PTWarrantyAccountNo;
            savNAccParams.AccountType = AT.Special;

            if (CachedItems.GetCountryParamters(accAssParams.CountryCode).GetCountryParameterValue<bool>(CountryParameterNames.WarrantyCustomerDetails))
            {
                accountManager.SaveNewAccount(
                    savNAccParams,
                    "",
                    0,
                    0,
                    ref agreementNo,
                    ref propResult,
                    ref dateProp,
                    out bureauFailure,
                    out storeCardTransRefNo,
                    out referralReasons,    //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
                    out error);

                accountManager.LockAccount(_PTWarrantyAccountNo, savNAccParams.User.ToString(), out error);
                var rescore = false;
                accountManager.AddCustomerToAccount(
                    _PTWarrantyAccountNo,
                    accAssParams.CustomerId,
                    "H",
                    AT.Special,
                    out rescore,
                    out error,
                    userid: savNAccParams.SalesPerson);
            }

            agreementNo = 1;
            accountManager.SaveNewAccount(
                savNAccParams,
                "",
                0,
                0,
                ref agreementNo,
                ref propResult,
                ref dateProp,
                out bureauFailure,
                out storeCardTransRefNo,
                out referralReasons,     //IP - 15/03/11 - #3314 - CR1245 - Return referral reasons
                out error);

            return new SaveNewAccountWithWarrantyResult
            {
                SaveNewAccountResult = new WAccountManager.SaveNewAccountResult
                {
                    AgreementNo = agreementNo,
                    PropResult = propResult,
                    DateProp = dateProp,
                    BureauFailure = bureauFailure,
                },
                AccountNo = _PTWarrantyAccountNo,
                CustId = accAssParams.CustomerId,
                ContractNo = contractNo
            };
        }
    
        [WebMethod]
        public SaveNewAccountWithWarrantyResult CreateCustomerAndSaveNewAccountWithWarranty(
            CustomerResult customer, 
            CustomerResult.Parameters.AccountAssotiation accAssParams, 
            WAccountManager.SaveNewAccountParameters savNAccParams)
        {
            accAssParams.CustomerId = customers.Create(customer).custid;

            return SaveNewAccountWithWarranty(accAssParams, savNAccParams);
        }
    }

    public class SaveNewAccountWithWarrantyResult
    {
        public WAccountManager.SaveNewAccountResult SaveNewAccountResult { get; set; }
        public string CustId { get; set; }
        public string AccountNo { get; set; }
        public string ContractNo { get; set; }
    }
}