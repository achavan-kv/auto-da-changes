using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Blue.Cosacs.Shared;
using STL.Common.Constants.FTransaction;

namespace Blue.Cosacs
{
    public class SCStatement
    {
        public StorecardPaymentDetails PaymentDetails { get; set; }
        public Customer Customer { get; set; }
        public CustAddress CustAddress {get; set;}
        public Blue.Cosacs.Shared.StoreCard StoreCard { get; set; }
        public List<view_FintransStoreCardStatements> AllFin { get; set; }
        public double InterestDue { get; set; }

        public decimal OpenBalance 
        { 
            get 
            {
                return AllFin.Where(d => d.DateTrans < StoreCardStatement.DateFrom).Sum(v => v.Value);
            } 
        }
        public decimal Purchases
        {
            get
            {
                return AllFin.Where(d => d.DateTrans >= StoreCardStatement.DateFrom &&
                                         d.DateTrans < StoreCardStatement.DateTo &&
                                         (d.Code == TransType.StoreCardPayment || 
                                         d.Code == TransType.StoreCardRefund)).Sum(v => v.Value);       //#13699
            }
        }
        public decimal Fees
        {
            get
            {
                return AllFin.Where(d => d.DateTrans >= StoreCardStatement.DateFrom &&
                                         d.DateTrans < StoreCardStatement.DateTo &&
                                         (d.Code == TransType.AdminCharge ||
                                         d.Code == TransType.StoreCardAnnualFee ||
                                         d.Code == TransType.StoreCardCardReplaceFee ||
                                         d.Code == TransType.StoreCardCardStatementFee)).Sum(v => v.Value);
            }
        }

        public decimal Interest
        {
            get
            {
                return AllFin.Where(d => d.DateTrans >= StoreCardStatement.DateFrom &&
                                         d.DateTrans < StoreCardStatement.DateTo &&
                                         d.Code == TransType.StoreCardInterest).Sum(v => v.Value);
            }
        }

        public decimal Payments
        {
            get
            {
                return AllFin.Where(d => d.DateTrans >= StoreCardStatement.DateFrom &&
                                         d.DateTrans < StoreCardStatement.DateTo &&
                                         d.Code == TransType.Payment).Sum(v => v.Value);
            }
        }
        public decimal EndBalance
        {
            get
            {
                return AllFin.Where(f => f.DateTrans < StoreCardStatement.DateTo).Sum(v => v.Value);
            } 
        }
        public IEnumerable<view_FintransStoreCardStatements> StatementFin
        {
            get
            {
                var total = OpenBalance;
                return AllFin.Where(d => d.DateTrans >= StoreCardStatement.DateFrom &&
                                         d.DateTrans < StoreCardStatement.DateTo).OrderBy(d => d.DateTrans).Select(f => new view_FintransStoreCardStatements 
                                                                                                                            {
                                                                                                                                DateTrans = f.DateTrans,
                                                                                                                                Description = f.Description,
                                                                                                                                Value = f.Value,
                                                                                                                                Code = f.Code,
                                                                                                                                CardNumber = f.CardNumber,
                                                                                                                                agrmtno = f.agrmtno,
                                                                                                                                AcctNo = f.AcctNo,
                                                                                                                                branchname = f.branchname,
                                                                                                                                BranchNo= f.BranchNo,
                                                                                                                                TransferAccount = f.TransferAccount,
                                                                                                                                RunningTotal = total += f.Value
                                                                                                                            }).OrderBy(d => d.DateTrans);       // #8994 jec 25/01/12 order by ascending
            }
        }
        public StoreCardStatement StoreCardStatement { get; set; }
        public List<CountryMaintenance> CountryMaintenance { get; set; }
        public string DecimalPlace
        {
            get { return CountryMaintenance.Find(c => c.CodeName == "decimalplaces").Value; }
        }

        public DateTime DueDate
        {
            get { return StoreCardStatement.DateTo.AddDays(Convert.ToInt32(CountryMaintenance.Find(c => c.CodeName == "SCardInterestFreeDays").Value));}
        }

        public Decimal MinPayment
        {
            get { return Convert.ToDecimal(CountryMaintenance.Find(c => c.CodeName == "StoreCardMinPayment").Value); }
        }

        public Decimal MinPaymentPercent
        {
            get { return Convert.ToDecimal(CountryMaintenance.Find(c => c.CodeName == "StoreCardPaymentPercent").Value) / 100; }
        }



        public Decimal MinAmount                // #9543 jec 27/01/12
        {
            get {
                if (EndBalance <= 0)
                    return (0);            // #9543 jec 27/01/12 removed .ToString(DecimalPlace)

                 var MiniPayment = (EndBalance * MinPaymentPercent > MinPayment ? EndBalance * MinPaymentPercent : MinPayment);
                 //if (MiniPayment < PaymentDetails.MinimumPayment && PaymentDetails.MinimumPayment > EndBalance * MinPaymentPercent)
                 //    MiniPayment = PaymentDetails.MinimumPayment;
                 if (MiniPayment < StoreCardStatement.StmtMinPayment && StoreCardStatement.StmtMinPayment > EndBalance * MinPaymentPercent)
                     MiniPayment = StoreCardStatement.StmtMinPayment;       // #10125 jec 21/05/12
                 if (MiniPayment > EndBalance)
                     MiniPayment = EndBalance;

                 //#10081 jec - Monthly payment set and < Balance and > Balance * percent 
                 if (PaymentDetails.MonthlyAmount > 0
                        && PaymentDetails.MonthlyAmount <= EndBalance
                        && PaymentDetails.MonthlyAmount > EndBalance * MinPaymentPercent)
                     MiniPayment = Convert.ToDecimal(PaymentDetails.MonthlyAmount);

                 var UnderPayment = OverDue;     // #8445 jec 23/01/12
                 if (UnderPayment < 0
                     || (UnderPayment > 0 && MiniPayment + UnderPayment>EndBalance))    // #10120 MinPay+Overdue > Account Balance
                     UnderPayment = 0;

                 //return (MiniPayment + UnderPayment);    // #9543 jec 27/01/12 removed .ToString(DecimalPlace)
                 return (MiniPayment);      // #10125 jec 21/05/12
            }
        }

        public decimal OverDue                      // #8445 jec 25/01/12
        {
            get
            {
                if (PrevMinPayment + Payments < 0)      // #8445 26/01/12
                    return 0;
                else
                    return PrevMinPayment + Payments;
            }
        }

        public decimal PrevMinPayment
        {
            get
            {
                return StoreCardStatement.PrevStmtMinPayment;
            }
        }

        public long CardNumber                  // #8445 jec 25/01/12
        {
            get
            {
                return StoreCard.CardNumber;
            }
        }

        public string ConvertCode(string code)
        {
            switch (code)
            {
                case "SCT":
                    return "Purchase"; //+ Environment.NewLine;                        //IP - 27/03/12 - #9834 - UAT108
                    //return "Sale" ; //+ Environment.NewLine;
                case "PAY":
                    return "Payment - Thank You.";
                case TransType.StoreCardInterest:
                    return "Interest Fee";
                case TransType.AdminCharge:
                    return "Admin Fee";
                case TransType.StoreCardCardReplaceFee:
                    return "Card Replacement Fee";
                case TransType.StoreCardCardStatementFee:
                    return "Statement Fee";
                case TransType.StoreCardRefund:
                    return "Refund";
                default:
                    return code;
            }
        }
    }
}
