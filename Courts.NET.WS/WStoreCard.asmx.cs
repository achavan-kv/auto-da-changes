using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using Blue.Cosacs.StoreCardUtil;
using STL.Common;

[WebService(Namespace = "http://schemas.bluebridgeltd.com/cosacs/storecard/2010/11/")]
public class WStoreCard : CommonService
{
    public class StoreCardParameters : StoreCard
    {

    }

    public class StoreCardPayParameters : StorecardPaymentDetails
    {

    }

    public WStoreCard()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public void AccountUpdateOutstandingBalance(string acctno)
    {
        new AccountRepository().UpdateOutstandingBalance(acctno);
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public void SaveStoreCardRates(List<StoreCardRate> Rates)
    {
        var SCRates = new StoreCardRatesRepository();
        SCRates.Save(Rates, STL.Common.Static.Credential.UserId.ToString());
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<StoreCardRate> GetStoreCardRates()
    { 
        var SCRates = new StoreCardRatesRepository();
        return new List<StoreCardRate>(SCRates.LoadAll());
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public bool RateInUse(int id)
    {
        return new StoreCardRatesRepository().RateInUse(id);
    }

    

    [WebMethod]
    [SoapHeader("authentication")]
    public bool? SetAwaitingActivation(string acctno)
    {
        return new StoreCardRepository().SetAwaitingActivation(Convert.ToBoolean(Country["StoreCardCheckQual"]),acctno);
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public long GenerateStoreCard()
    {
        return StoreCardGen.GenerateAndSaveCountryStoreCardNumber();
    }
}
