using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared;

namespace STL.PL.StoreCard.Common
{
    public static class StoreCardCommon
    {
        public static List<StoreCardAccountStatus_Lookup> Status;
        public enum StoreCardAccountTabs { Activation, Cancellation, PaymentDetails };
        //public static View_StoreCardAll ConvertStoreCard(STL.PL.WSStoreCard.View_StoreCardAll c)
        //{
        //    return new View_StoreCardAll()
        //    {
        //        CardNumber = c.CardNumber,
        //        CardName = c.CardName,
        //        IssueYear = c.IssueYear,
        //        IssueMonth = c.IssueMonth,
        //        ExpirationYear = c.ExpirationYear,
        //        ExpirationMonth = c.ExpirationMonth,
        //        AcctNo = c.AcctNo,
        //        ExportRunNo = c.ExportRunNo,
        //        MonthlyAmount = c.MonthlyAmount,
        //        PaymentMethod = c.PaymentMethod,
        //        PaymentOption = c.PaymentOption,
        //        RateId = c.RateId,
        //        InterestRate = c.InterestRate,
        //        RateFixed = c.RateFixed,
        //        StatementFrequency = c.StatementFrequency,
        //        DateLastStatementPrinted = c.DateLastStatementPrinted,
        //        DatePaymentDue = c.DatePaymentDue,
        //       // St = c.Status,
        //        StatusCode = c.StatusCode,
        //        NoStatements = c.NoStatements,
        //        ContactMethod = c.ContactMethod,
        //        DateNotePrinted = c.DateNotePrinted,
        //        CustID = c.CustID,
        //        otherid = c.otherid,
        //        branchnohdle = c.branchnohdle,
        //        name = c.name,
        //        firstname = c.firstname,
        //        title = c.title,
        //        alias = c.alias,
        //        addrsort = c.addrsort,
        //        namesort = c.namesort,
        //        dateborn = c.dateborn,
        //        sex = c.sex,
        //        ethnicity = c.ethnicity,
        //        morerewardsno = c.morerewardsno,
        //        effectivedate = c.effectivedate,
        //        IdNumber = c.IdNumber,
        //        IdType = c.IdType,
        //        creditblocked = c.creditblocked,
        //        RFCreditLimit = c.RFCreditLimit,
        //        RFCardSeqNo = c.RFCardSeqNo,
        //        RFCardPrinted = c.RFCardPrinted,
        //        datelastscored = c.datelastscored,
        //        RFDateReminded = c.RFDateReminded,
        //        empeenochange = c.empeenochange,
        //        datechange = c.datechange,
        //        maidenname = c.maidenname,
        //        OldRFCreditLimit = c.OldRFCreditLimit,
        //        LimitType = c.LimitType,
        //        AvailableSpend = c.AvailableSpend,
        //        ScoringBand = c.ScoringBand,
        //        InstantCredit = c.InstantCredit,
        //        StoreType = c.StoreType,
        //        LoanQualified = c.LoanQualified,
        //        dependants = c.dependants,
        //        maritalstat = c.maritalstat,
        //        Nationality = c.Nationality,
        //        recurringarrears = c.recurringarrears,
        //        age = c.age,
        //        ScoreCardType = c.ScoreCardType,
        //        StoreCardApproved = c.StoreCardApproved,
        //        StoreCardLimit = c.StoreCardLimit,
        //        StoreCardAvailable = c.StoreCardAvailable,
        //        SCardApprovedDate = c.SCardApprovedDate
                
        //    };
        //}
    }

    public class StoreCardCustDetails
    {
        public string Custid { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string NameConCat()
        {
            const int max = 26;
            var total = Title.Length + FirstName.Length + LastName.Length + 2;

            if (total <= max)
                return string.Format("{0} {1} {2}", Title, FirstName, LastName);
            else if (total - FirstName.Length + 1 <= max)
                return string.Format("{0} {1} {2}", Title, FirstName.Substring(0, 1), LastName);
            else if (LastName.Length + 2 <= max)
                return string.Format("{0} {1}", FirstName.Substring(0, 1), LastName);
            else if (LastName.Length <= max)
                return string.Format("{0}", LastName);
            else
                return string.Format("{0}", LastName.Substring(0, 26));
        }
    }
}
