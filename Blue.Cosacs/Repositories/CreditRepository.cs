using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.Credit;
using STL.Common;
using STL.Common.Constants.CashLoans;
using STL.Common.Constants.ColumnNames;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Blue.Cosacs.Repositories
{
    public class CreditRepository
    {
        private DataTable dtItems = new DataTable();
        private CommonObject commonObject = new CommonObject();

        public CountryParameterCollection CountryParms
        {
            get { return commonObject.Country; }
        }

        public void CompleteReferralStage(SqlConnection conn, SqlTransaction trans, CompleteReferralStageRequest r)
        {
            //CompleteReferralStageRequest request
            DProposalFlag propFlag = new DProposalFlag();
            propFlag.CheckType = "R";
            propFlag.CustomerID = r.CustomerId;
            propFlag.DateProp = r.DateProp;
            propFlag.DateCleared = DateTime.Now;
            propFlag.EmployeeNoFlag = r.User;
            propFlag.Save(conn, trans, r.AccountNo);

            string propResult = r.Approved == true ? "A" : "D";

            if (r.NewNotes.Trim().Length > 0)
            {
                // Get Employee name to audit comments
                DEmployee employee = new DEmployee();
                employee.GetEmployeeDetails(conn, trans, r.User);
                r.RefNotes = employee.EmployeeName + " (" + r.User + ") - " + DateTime.Now + " :\n\n" + r.NewNotes + "\n\n" + r.RefNotes;
            }

            DProposal prop = new DProposal();
            // Save the result
            prop.SetPropResult(conn, trans, r.CustomerId, r.AccountNo, r.DateProp, propResult, r.RefNotes);
            // Save the audit record
            prop.AuditPropResult(conn, trans, r.CustomerId, r.AccountNo, r.DateProp, r.User);
            DAccount acct = new DAccount();
            DCustomer cust = new DCustomer();

            cust.GetBasicCustomerDetails(conn, trans, r.CustomerId, r.AccountNo, "H");

            if (r.rejected)
                cust.SetCreditLimit(conn, trans, r.CustomerId, 0, "A");
            else
            {
                if (cust.RFLimit != r.CreditLimit)		/* limit has been overridden */
                    cust.SetOverrideLimit(conn, trans, r.CustomerId, r.CreditLimit);
                else
                    cust.SetCreditLimit(conn, trans, r.CustomerId, r.CreditLimit, "A");
            }

            if (r.reOpen && !r.rejected)
            {
                // If previously rejected account, write record to FACT
                FactTransCancel(conn, trans, r.AccountNo, r.branchno, true, DateTime.Now);
                acct.ResetAgrmnTotal(conn, trans, r.AccountNo);
            }
            else
                // DSR 24/02/03 Update the account status
                acct.UpdateStatus(conn, trans, r.AccountNo);

            if (CountryParms.GetCountryParameterValue<bool>(CountryParameterNames.PrizeVouchersActive) && r.Approved)
            {
                DAgreement agreement = new DAgreement(conn, trans, r.AccountNo, 1);
                DCustomer customer = new DCustomer();

                customer.User = r.User;

                IssuePrizeVouchers(conn, trans, r.AccountNo, agreement.CashPrice, 0, r.User);
            }
        }

        public void IssuePrizeVouchers(SqlConnection conn, SqlTransaction trans,
                                      string acctNo, decimal cashPrice, int buffNo
                                     , int User)
        {
            DateTime dateIssued = DateTime.Now;
            var numVouchers = cashPrice / CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.ValuePerVoucher);

            numVouchers = Convert.ToDecimal(Math.Floor(Convert.ToDouble(numVouchers)));

            if (numVouchers > 0)
            {
                int voucherID = 0;
                DCustomer cust = new DCustomer();
                cust.User = User;
                cust.IssuePrizeVouchers(conn, trans, acctNo, cashPrice, dateIssued,
                                        buffNo, out voucherID);

                for (int i = 1; i <= numVouchers; i++)
                {
                    cust.SavePrizeVouchers(conn, trans, voucherID);
                }
            }
        }

        public void FactTransCancel(SqlConnection conn, SqlTransaction trans, string acctNo,
            int branch, bool reOpen, DateTime dateCancelled)
        {
            DLineItem item = new DLineItem();
            DFACTTrans fact = new DFACTTrans();
            DBranch bc = new DBranch();
            DDelivery del = new DDelivery();

            double quantity = 0;
            double price = 0;
            double taxamt = 0;
            double value = 0;
            string tranType = "";
            string tcCode = "";
            bool status = true;
            int buffNo = 0;

            buffNo = bc.GetBuffNo(conn, trans, (short)branch);

            del.AccountNumber = acctNo;
            del.GetDeliveries(conn, trans);
            fact.TranDate = dateCancelled;
            fact.BuffNo = buffNo;

            foreach (DataRow row in del.Deliveries.Rows)
            {
                if (Convert.ToDecimal(row[CN.Value]) == 0)
                {
                    fact.ItemNumber = (string)row[CN.ItemNo];
                    fact.StockLocation = (short)row[CN.StockLocn];
                    //if ((decimal)row[CN.OrdVal] > 0) // IP - 22/02/10 - CR1072 - LW 70059 - Sanction Fixes from 4.3 - Merge
                    if (Convert.ToDecimal(row[CN.Value]) > 0)
                    {
                        fact.Quantity = -1;
                        fact.TranType = "13";
                        fact.TCCode = "11";
                    }
                    else
                    {
                        fact.Quantity = 1;
                        fact.TranType = "03";
                        fact.TCCode = "32";
                    }

                    //69474 no account number is being passed to the facttrans table
                    fact.AccountNumber = acctNo;
                    fact.Price = 0;
                    //decimal total = 0 - (decimal)row[CN.OrdVal]; // IP - 22/02/10 - CR1072 - LW 70059 - Sanction Fixes from 4.3 - Merge
                    decimal total = 0 - Convert.ToDecimal(row[CN.Value]);
                    fact.Value = (double)total;
                    fact.Save(conn, trans);
                }
            }

            tranType = "04";
            tcCode = "58";

            buffNo = bc.GetBuffNo(conn, trans, (short)branch);

            item.AccountNumber = acctNo;

            dtItems = item.GetItemsForCanxAccount(conn, trans);    // 68181 RD 22/02/06 Modified so that canx records are posted to fact

            foreach (DataRow row in dtItems.Rows)
            {
                if ((string)row["ItemType"] == "S")
                {
                    fact.AccountNumber = acctNo;
                    fact.ItemNumber = (string)row[CN.ItemNo];
                    fact.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                    fact.TranType = tranType;
                    fact.TCCode = tcCode;

                    if (reOpen)
                    {
                        int rowCount = fact.DeleteCancellation(conn, trans);

                        if (rowCount == 0 && (Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty])) > 0)
                        {
                            quantity = Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty]);
                            price = Convert.ToDouble(row[CN.Price]);
                            taxamt = Convert.ToDouble(row[CN.TaxAmt]);
                            value = quantity * price;
                            tranType = "01";
                            tcCode = "61";
                            status = true;
                        }
                        else
                            status = false;
                    }
                    else
                    {
                        quantity = -1 * (Convert.ToDouble(row[CN.Quantity]) - Convert.ToDouble(row[CN.DelQty]));
                        price = Convert.ToDouble(row[CN.Price]);
                        taxamt = Convert.ToDouble(row[CN.TaxAmt]);
                        value = quantity * price;
                    }

                    if (status)
                    {
                        fact.AgreementNumber = 1;
                        fact.BuffNo = buffNo;
                        fact.TranType = tranType;
                        fact.TCCode = tcCode;
                        fact.Quantity = quantity;
                        fact.Price = price;
                        fact.TaxAmt = taxamt;
                        fact.Value = value;
                        fact.Save(conn, trans);
                    }
                }
            }
        }

        public CashLoanQualificationResponse CashLoanQualification(string custId, int branch)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var instCredQual = "N";
                var loanQual = "N";
                int? err = 0;
                decimal existCashLoan = 0;
                bool isMambuAccount = false;
                var acctno = "";

                DataSet referralDS = new DataSet();

                SqlParameter[] xparams = {
                                        new SqlParameter("@custId", custId),
                                        new SqlParameter("@branch", branch.ToString()),
                                        new SqlParameter("@AcctNo", SqlDbType.VarChar,12)
                                        {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@IsMambuAccount", SqlDbType.Bit)
                                        {
                                            Direction = ParameterDirection.Output
                                        }
                };

                SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "dbo.SP_BS_CashLoanQualification";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(xparams);
                command.ExecuteNonQuery();

                if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                {
                    acctno = xparams[2].Value.ToString();
                }

                if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                {
                    isMambuAccount = (bool)xparams[3].Value;
                }

                CashLoan Cashloan = null;

                if (!string.IsNullOrWhiteSpace(acctno))
                {
                    Cashloan = (from cl in ctx.CashLoan where cl.AcctNo == acctno select cl).AnsiFirstOrDefault(ctx);
                }

                if (Cashloan == null || Cashloan.LoanStatus == CashLoanStatus.Referred)       // #8761  or pending Loan referred
                {
                    referralDS = new InstantCreditApprovalsCheckGen(connection, transaction).ExecuteDataSet(custId, "", "L", out instCredQual, out loanQual, out err);
                }

                if (loanQual == "Y" || Cashloan != null)
                {
                    var cashLoanQualificationResponse = GetCashLoanQualification(ctx, custId, branch, Cashloan, loanQual, referralDS, existCashLoan, true);   //Added parameter by Rahul Sonawane 10.7CR CashLoan
                    cashLoanQualificationResponse.IsMambuAccount = isMambuAccount;
                    return cashLoanQualificationResponse;
                }
                else
                {
                    //#CR-7- Extend Referrals to Cash Loans
                    if (loanQual == "N" || Cashloan != null)
                    {
                        var cashLoanQualificationResponse = GetCashLoanQualification(ctx, custId, branch, Cashloan, loanQual, referralDS, existCashLoan, false);  //Added parameter by Rahul Sonawane 10.7CR CashLoan
                        cashLoanQualificationResponse.IsMambuAccount = isMambuAccount;
                        return cashLoanQualificationResponse;
                    }
                    else
                    {
                        return new CashLoanQualificationResponse
                        {
                            Qualified = false,
                            LoanQual = loanQual,             // 'X' = Custid not found
                            Customer = new Customer(),
                            CustAddress = new CustAddress(),
                            CustTel = new List<CustTel>(),
                            TermsType = new List<TermsTypeAllBands>(),
                            StampDuty = 0,                   // #10013
                            Cashloan = new CashLoan(),
                            DAed = false,
                            Referral = new DataSet(),
                            Dateprop = new DateTime(),         // #8487
                            TotalCreditUsed = existCashLoan,   //Added for Exist Cash Loan
                            IsMambuAccount = isMambuAccount  // Sudhir code change 0n 4 march
                        };
                    }
                }
            });
        }

        private CashLoanQualificationResponse GetCashLoanQualification(Context ctx, string custId, int branch, CashLoan cashloan, string loanQual, DataSet referralDS, decimal existCashLoan, bool isQualified)
        {
            bool loanAllowed = false;
            var delAuth = false;
            var dateprop = new DateTime();              // #8487
            var customer = ctx.Customer
                                          .Where(c => c.custid == custId)
                                          .AnsiFirstOrDefault(ctx);

            var found = (from ac in ctx.acctnoctrl
                         where ac.branchno == branch && ac.acctcat == "R"
                         select ac).AnsiFirstOrDefault(ctx);
            if (found != null)
                loanAllowed = true;

            //Check if Cash Loan account has been Delivery Authorised.
            if (cashloan != null)
            {
                var da = (from ag in ctx.Agreement
                          where ag.acctno == cashloan.AcctNo
                          select ag.holdprop).AnsiFirstOrDefault(ctx);

                dateprop = (from p in ctx.Proposal                  // #8487
                            where p.acctno == cashloan.AcctNo

                            select p.dateprop).AnsiFirstOrDefault(ctx);

                if (da == "N")
                    delAuth = true;
            }

            var termsType = (from tt in ctx.TermsTypeAllBands
                             where tt.isloan == true && tt.Band == customer.ScoringBand
                             && tt.IsActive == 1                        //#17776  //#17676
                             && (customer.CashLoanNew == true && tt.LoanNewCustomer == true
                                    || (customer.CashLoanRecent == true && tt.LoanRecentCustomer == true)
                                    || (customer.CashLoanExisting == true && tt.LoanExistingCustomer == true)
                                    || (customer.CashLoanStaff == true && tt.LoanStaff == true))
                             select tt).AnsiToList(ctx);

            //#17840
            if (cashloan != null)
            {
                //If the Terms Type on the Cash Loan table is not an active Terms Type, continue to add to the 
                //list of Terms Type as this Cash Loan was initially accepted with this Terms Type.
                var cashLoanTT = (from tt in ctx.TermsTypeAllBands
                                  where tt.TermsType == cashloan.TermsType
                                  && (customer.CashLoanNew == true && tt.LoanNewCustomer == true
                                    || (customer.CashLoanRecent == true && tt.LoanRecentCustomer == true)
                                    || (customer.CashLoanExisting == true && tt.LoanExistingCustomer == true)
                                    || (customer.CashLoanStaff == true && tt.LoanStaff == true))
                                  select tt).AnsiFirstOrDefault(ctx);

                if (cashLoanTT != null && !termsType.Any(t => t.TermsType == cashLoanTT.TermsType))

                {
                    termsType.Add(cashLoanTT);
                }
            }

            // #8549 if no bands found - customer does not have a scoring band - pick the band with lowest score
            if (termsType.Count == 0)
            {
                termsType = (from tt in ctx.TermsTypeAllBands
                             where tt.isloan == true && tt.PointsFrom == 0
                              && (customer.CashLoanNew == true && tt.LoanNewCustomer == true
                                    || (customer.CashLoanRecent == true && tt.LoanRecentCustomer == true)
                                    || (customer.CashLoanExisting == true && tt.LoanExistingCustomer == true)
                                    || (customer.CashLoanStaff == true && tt.LoanStaff == true))
                             select tt).AnsiToList(ctx);
            }

            return new CashLoanQualificationResponse
            {
                Qualified = isQualified,
                LoanQual = loanQual,
                Customer = customer,
                CustAddress = (from ca in ctx.CustAddress
                               where ca.custid == custId && ca.addtype == "H" &&
                               ca.datemoved == null
                               select ca).AnsiFirstOrDefault(ctx),

                CustTel = (from ct in ctx.CustTel
                           where ct.custid == custId && ct.datediscon == null
                           select ct).AnsiToList(ctx),

                TermsType = termsType,          // #8549

                StampDuty = (from sc in ctx.SundChgTyp                  // #10013
                             where sc.accttype == "R" && sc.ItemID == StockItemCache.Get(StockItemKeys.SD)
                             select sc.amount).AnsiFirstOrDefault(ctx),

                Cashloan = cashloan,
                DAed = delAuth,
                Referral = referralDS,
                Dateprop = dateprop,         // #8487
                LoanAllowed = Convert.ToBoolean(loanAllowed),
                TotalCreditUsed = existCashLoan,   //Added By RahuSonawane
            };
        }

        //#14065 - Check Cash Loan Qualification
        public CheckLoanQualificationResponse CheckLoanQualification(string custId, int branch)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var ds = new DataSet();
                var instCredQual = "N";
                var loanQual = "N";
                int? err = 0;

                ds = new InstantCreditApprovalsCheckGen(connection, transaction).ExecuteDataSet(custId, "", "L", out instCredQual, out loanQual, out err);

                return new CheckLoanQualificationResponse
                {
                    LoanQualified = loanQual

                };

            });
        }

        public void CashLoanStausInsert(Blue.Cosacs.Shared.CashLoanDetails det, SqlConnection conn, SqlTransaction trans)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                // IP - 27/09/12 - #10480 - LW75156
                if (det.loanStatus == "D" || det.loanStatus == "P")
                {
                    var cashLoan = (from cl in ctx.CashLoan
                                    where cl.AcctNo == det.accountNo
                                    && cl.Custid == det.custId
                                    && cl.LoanStatus == "D"
                                    select cl).AnsiToList(ctx);

                    if (cashLoan.Count > 0)
                    {
                        throw new STLException("Cash Loan has already been disbursed for this account");
                    }
                }

                CashLoanSaveSP c = new CashLoanSaveSP(conn, trans);
                c.Custid = det.custId;
                c.AcctNo = det.accountNo;
                c.LoanAmount = det.loanAmount;
                c.LoanStatus = det.loanStatus;
                c.Term = det.term;
                c.TermsType = det.termsType;
                c.EmpeenoAccept = det.empeenoAccept;
                c.EmpeenoDisburse = det.empeenoDisburse;
                c.DatePrinted = det.datePrinted.HasValue ? (det.datePrinted.Value == DateTime.MinValue ? DateTime.Today : det.datePrinted.Value) : det.datePrinted;                //#8491
                c.ReferralReasons = det.referralReasons;        //IP - 24/02/12 - #9598 - UAT 87
                c.CashLoanPurpose = det.cashLoanPurpose;        //#19337 - CR18568
                c.AdminChargeWaived = det.waiveAdminCharge;
                c.AdminCharge = det.adminChg;
                c.EmpeenoAdminChargeWaived = det.empeenoAdminChargeWaived;
                c.EmpeenoLoanAmountChanged = det.empeenoLoanAmountChanged;
                c.Bank = det.Bank;
                c.BankAccountType = det.BankAccountType;
                c.BankBranch = det.BankBranch;
                c.BankAcctNo = det.BankAccountNo;
                c.Notes = det.Notes;
                c.BankReferenceNo = det.BankReferenceNumber;
                c.BankAccountName = det.BankAccountName;
                c.ExecuteNonQuery();

                var type = "CashLoanAccept";

                if (det.loanStatus == "P")
                {
                    type = "CashLoanPrint";
                }
                else if (det.loanStatus == "D")
                {
                    type = "CashLoanDisburse";
                }

                //Audit
                EventStore.Instance.Log(det, type, EventCategory.CashLoan, new { Acctno = det.accountNo, Custid = det.custId });

            }, conn: conn, trans: trans);
        }




        //Method to return a CashLoan record
        public CashLoan GetCashLoan(string acctNo)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                {
                    return (from cl in ctx.CashLoan
                            where cl.AcctNo == acctNo
                            select cl).AnsiFirstOrDefault(ctx);
                });
        }

        //Method to update Customer.CashLoanBlocked to either
        //1. Blocked - "B"
        //2. Unblocked - "U"
        //3. NotBlocked - ""
        public UpdateCashLoanBlockedResponse UpdateCashLoanBlocked(string custID, string BlockedStatus)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                {

                    var customer = (from c in ctx.Customer
                                    where c.custid == custID
                                    select c).AnsiFirstOrDefault(ctx);          //#8557 - Changed from SingleOrDefault

                    if (customer != null && customer.CashLoanBlocked != BlockedStatus)
                    {
                        customer.CashLoanBlocked = BlockedStatus;
                        ctx.SubmitChanges();
                    }

                    return new UpdateCashLoanBlockedResponse
                    {
                    };

                });



        }

        public void UpdateCashLoanBankDetails(SqlConnection conn, SqlTransaction tran, string custID, string acctNo, string Bank, string BankAccountType, string BankBranch, string BankAcctNo,
                                                            string Notes, string BankReferenceNo, string BankAccountName)
        {
            Context.ExecuteTx((ctx, connection, transaction) =>
            {
                var cashLoan = (from c in ctx.CashLoan
                                where c.Custid == custID
                                && c.AcctNo == acctNo
                                select c).AnsiFirstOrDefault(ctx);

                if (cashLoan != null)
                {
                    cashLoan.Bank = Bank;
                    cashLoan.BankAccountType = BankAccountType;
                    cashLoan.BankBranch = BankBranch;
                    cashLoan.BankAcctNo = BankAcctNo;
                    cashLoan.Notes = Notes;
                    cashLoan.BankReferenceNo = BankReferenceNo;
                    cashLoan.BankAccountName = BankAccountName;

                    ctx.SubmitChanges();
                }

            }, conn: conn, trans: tran);

        }

    }
}
