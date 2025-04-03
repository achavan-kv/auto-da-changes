using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public static class CustomerExtensions
    {
        public static Customer ToCustomer(this CustomerResult c)
        {
            return  new Customer
            {
                addrsort = c.addrsort,
                age = c.age,
                alias = c.alias,
                AvailableSpend = c.AvailableSpend,
                branchnohdle = c.branchnohdle,
                creditblocked = c.creditblocked,
                custid = c.custid,
                dateborn = c.dateborn,
                datechange = c.datechange,
                datelastscored = c.datelastscored,
                dependants = c.dependants,
                effectivedate = c.effectivedate,
                empeenochange = c.empeenochange,
                ethnicity = c.ethnicity,
                firstname = c.firstname,
                IdNumber = c.IdNumber,
                IdType = c.IdType,
                InstantCredit = c.InstantCredit,
                LimitType = c.LimitType,
                LoanQualified = c.LoanQualified,
                maidenname = c.maidenname,
                maritalstat = c.maritalstat,
                morerewardsno = c.morerewardsno,
                name = c.name,
                namesort = c.namesort,
                Nationality = c.Nationality,
                OldRFCreditLimit = c.OldRFCreditLimit,
                origbr = c.origbr,
                otherid = c.otherid,
                recurringarrears = c.recurringarrears,
                RFCardPrinted = c.RFCardPrinted,
                RFCardSeqNo = c.RFCardSeqNo,
                RFCreditLimit = c.RFCreditLimit,
                RFDateReminded = c.RFDateReminded,
                SCardApprovedDate = c.SCardApprovedDate,
                ScoreCardType = c.ScoreCardType,
                ScoringBand = c.ScoringBand,
                sex = c.sex,
                StoreCardApproved = c.StoreCardApproved,
                StoreCardAvailable = c.StoreCardAvailable,
                StoreCardLimit = c.StoreCardLimit,
                StoreType = c.StoreType,
                title = c.title
            };
        }
        public static CustomerResult ToCustomerResult(this Customer c)
        {
            return new CustomerResult
            {
                addrsort = c.addrsort,
                age = c.age,
                alias = c.alias,
                AvailableSpend = c.AvailableSpend,
                branchnohdle = c.branchnohdle,
                creditblocked = c.creditblocked,
                custid = c.custid,
                dateborn = c.dateborn,
                datechange = c.datechange,
                datelastscored = c.datelastscored,
                dependants = c.dependants,
                effectivedate = c.effectivedate,
                empeenochange = c.empeenochange,
                ethnicity = c.ethnicity,
                firstname = c.firstname,
                IdNumber = c.IdNumber,
                IdType = c.IdType,
                InstantCredit = c.InstantCredit,
                LimitType = c.LimitType,
                LoanQualified = c.LoanQualified,
                maidenname = c.maidenname,
                maritalstat = c.maritalstat,
                morerewardsno = c.morerewardsno,
                name = c.name,
                namesort = c.namesort,
                Nationality = c.Nationality,
                OldRFCreditLimit = c.OldRFCreditLimit,
                origbr = c.origbr,
                otherid = c.otherid,
                recurringarrears = c.recurringarrears,
                RFCardPrinted = c.RFCardPrinted,
                RFCardSeqNo = c.RFCardSeqNo,
                RFCreditLimit = c.RFCreditLimit,
                RFDateReminded = c.RFDateReminded,
                SCardApprovedDate = c.SCardApprovedDate,
                ScoreCardType = c.ScoreCardType,
                ScoringBand = c.ScoringBand,
                sex = c.sex,
                StoreCardApproved = c.StoreCardApproved,
                StoreCardAvailable = c.StoreCardAvailable,
                StoreCardLimit = c.StoreCardLimit,
                StoreType = c.StoreType,
                title = c.title
            };
        }
    }
}
