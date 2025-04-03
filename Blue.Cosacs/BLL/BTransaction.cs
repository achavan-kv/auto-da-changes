using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using STL.Common;
using STL.Common.Constants;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource; //IP - 03/02/10 - CR1072 - 3.1.9
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.InstantCredit;       //CR907         jec 02/08/07
using STL.Common.Constants.TableNames;
using STL.DAL;
using Blue.Cosacs;
using Blue.Cosacs.Messages.Service;

namespace STL.BLL
{


    /// <summary>
    /// Summary description for BTransaction.
    /// </summary>
    public class BTransaction : CommonObject
    {
        private decimal _AddTot = 0;
        private decimal _AdmTot = 0;
        private decimal _DelTot = 0;
        private decimal _FeeTot = 0;
        private decimal _GrtTot = 0;
        private decimal _IntTot = 0;
        private decimal _PayTot = 0;
        private decimal _RebTot = 0;
        private decimal _RepTot = 0;
        private decimal _RpoTot = 0;
        private decimal _UnclearChq = 0;
        private bool _IsCancellation = false;
        //private bool _IsCashZeroAgrmt; // = false;  // 67825 RD 14/03/06

        private int _fintransId = 0;
        public int FintransId
        {
            get { return _fintransId; }
            set { _fintransId = value; }
        }
        public decimal AddTot
        {
            get { return _AddTot; }
        }
        public decimal AdmTot
        {
            get { return _AdmTot; }
        }
        public decimal DelTot
        {
            get { return _DelTot; }
            set { _DelTot = value; }
        }
        public decimal FeeTot
        {
            get { return _FeeTot; }
        }
        public decimal GrtTot
        {
            get { return _GrtTot; }
        }

        public decimal IntTot
        {
            get { return _IntTot; }
        }
        public decimal PayTot
        {
            get { return _PayTot; }
            set { _PayTot = value; }
        }
        public decimal RebTot
        {
            get { return _RebTot; }
        }
        public decimal RepTot
        {
            get { return _RepTot; }
        }
        public decimal RpoTot
        {
            get { return _RpoTot; }
        }
        public decimal UnclearChq
        {
            get { return _UnclearChq; }
        }

        public bool IsCancellation
        {
            get { return _IsCancellation; }
            set { _IsCancellation = value; }
        }

        //69300 Create a new boolean to identify whether or not ClearProposal has been called
        private bool m_clearproposal;
        public bool clearProposal
        {
            get
            {
                return m_clearproposal;
            }
            set
            {
                m_clearproposal = value;
            }
        }

        public DateTime TransactionDate { get; set; }
        public int TransactionRefNo { get; set; }
        public BTransaction()
        {
        }

        /// <summary>
        /// this is a rough translation of the open road AddTrans method
        /// and should be called every time we add a record to the fintrans
        /// table.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="accountNo"></param>
        private void AddTransaction(SqlConnection conn, SqlTransaction trans,
            string accountNo, string transType,
            decimal transValue, string countryCode,
            short branchNo, int transRefNo,
            string chequeNo, string bankAcctNo,
            string bankCode, DateTime dateTrans,
            string footNote, short payMethod, int agrmtno, int? cashierTotID = null) //IP/JC - 06/01/12 - #8821
        {
            /*
             * 1.check the account status
             * 2.update the outstanding balance
             * 3.make sure balance < 1000000
             * 4.make a note of the arrears before the transaction (Why?)
             * 5.Settle the account if necessary
             * 6.Un-settle the account if necessary
             * 7.Get the next transrefno (or use the one that's passed in?)
             * 8.Update datelastpaid if it's a payment.
             * 9.Reset the status if reposession (to 6).
             * 10.Write the fintrans record.
             * 11.Run the arears calc for good measure.
             */

            //round the transaction value for Mauritius.... 
            CountryRound(ref transValue);

            decimal initArrears = 0;
            string initStatus = "";
            string paidAndTakenAcct = "";
            decimal valueOfWOC = 0;			/* to store warranties on credit value */
            DBranch branch = new DBranch();
            int buffNo = 0;
            DDelivery delivery = null;
            DFinTrans fin = null;
            DLineItem lineItem = new DLineItem();
            bool warrantyAdjustment = false;

            //round the transaction value for Mauritius.... 69449
            CountryRound(ref transValue);

            valueOfWOC = lineItem.GetWarrantiesOnCreditValue(conn, trans, accountNo);

            DAgreement agree = new DAgreement(conn, trans, accountNo, 1);

            DAccount acct = new DAccount(conn, trans, accountNo);
            acct.User = this.User;
            acct.UpdateStatus(conn, trans, accountNo);

            paidAndTakenAcct = acct.GetPaidAndTakenAccount(conn, trans, branchNo.ToString());

            //CLA Amortization -- GFT -- To refund the excess amount from Refund Screen
            //RD    -   05/11/2019
            // If customer paid the excess of amount to settle the account then do not change outstanding balance
            // for cash loan acct in case of refund
            if ((new BPayment().IsCashLoanAmortizedAccount(null, null, accountNo)) &&
                       transType == TransType.Refund &&
                       acct.OutstandingBalance == 0 &&
                       acct.Arrears < 0 &&
                       acct.CurrentStatus == "S")
            {
                acct.OutstandingBalance = 0;
            }
            else
            {
                acct.OutstandingBalance += transValue;
            }

            // 68629 increment the as400bal by the trans val [PC]
            // // uat376 rdb BDW Reversal - excluding new transaction BDR
            if (transType != TransType.AdminCharge && transType != TransType.Interest && transType != TransType.BadDebtWriteOffReversal)
                acct.AS400Bal += transValue;
            /*
            if((acct.OutstandingBalance >= 1000000 ||
                acct.OutstandingBalance <= -1000000 ||
                Math.Abs(transValue) > 1000000) &&
                countryCode != "I" &&
                countryCode != "C" &&
                countryCode != "A")
                throw new Exception(GetResource("M_BALANCEGRTMILLION"));
                //throw new Exception("Balance/value > 1 million cr/dr");
            */

            if ((transType == TransType.CreditFee &&
                transValue > 0) ||
                transType == TransType.Repossession ||
                transType == TransType.Payment)
            {
                initArrears = acct.Arrears;
                initStatus = acct.CurrentStatus;
            }

            /* JJ can't get all the fintrans for P&T it will cripple performance */
            if (accountNo != paidAndTakenAcct)
                GetByAcctNo(conn, trans, accountNo);


            /* perform auto DA for cash accounts */
            if (transType == TransType.Correction
                || transType == TransType.GiroExtra
                || transType == TransType.GiroNormal
                || transType == TransType.GiroRepresent
                || transType == TransType.TakeonTransfer
                || transType == TransType.Payment
                || transType == TransType.Refund
                || transType == TransType.Return
                || transType == TransType.SundryCreditTransfer
                || transType == TransType.Transfer)
            {
                this.PayTot += transValue;
                //66764 RD/DR 10/05/05 Modified to check for uncleared cheque transactions
                if (PayMethod.IsPayMethod(payMethod, PayMethod.Cheque)
                    && dateTrans.AddDays(Convert.ToDouble(Country[CountryParameterNames.ChequeDays])) > DateTime.Today)
                    this._UnclearChq += transValue;

                decimal clearedPayTot = PayTot - this._UnclearChq;

                // CR907                jec 02/08/07
                // If account qualifies for Instant Credit and deposit/first instal has been paid AutoDA
                if (AT.IsCreditType(acct.AccountType))
                {
                    bool autoDA = false;
                    string paid = "";
                    //DAgreement Agree = new DAgreement();
                    //agree.GetAgreement(accountNo,1);
                    //agree.AgreementList.Columns.
                    DAccount dfi = new DAccount();
                    paid = dfi.DepFirstInstal(conn, trans, accountNo, clearedPayTot, this.User);
                    autoDA = (paid == IC.Approved);
                    // if deposit/first instal paid  - AutoDA
                    if (autoDA)
                    {
                        string source = DASource.Auto; //IP - 04/02/10 - CR1072 - 3.1.9 - Source of Delivery Authorisation.
                        // Clear All Flags      jec 14/11/07
                        DProposalFlag pflag = new DProposalFlag();
                        //pflag.User = this.User;
                        pflag.User = Users.ICAutoDA; //IP - 18/03/11 - #3333
                        pflag.ClearAll(conn, trans, accountNo);

                        //agree.User = User;
                        agree.User = Users.ICAutoDA; //IP - 18/03/11 - #3333
                        agree.ClearProposal(conn, trans, accountNo, source);
                        clearProposal = true;

                        var AcctR = new AccountRepository();                            //#13790

                        DataTable lineItemBooking = agree.LineItemBooking;              //#13790

                        if (lineItemBooking != null && lineItemBooking.Rows.Count > 0)  //#13790         
                        {
                            lineItemBooking.Columns.Add("BookingID");

                            AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);
                            AcctR.bookingType = "D";

                            foreach (DataRow row in lineItemBooking.Rows)
                            {
                                new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["BookingID"]));    //#13829 - Update before GetBookingData     //#13790 - Update LineItemBookingSchedule with the BookingId
                            }

                            var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);

                            new Chub().SubmitMany(bookings, conn, trans);

                           
                        }

                        // CR - Deliver only non stock items.
                        agree.DeliverNonStocks(conn, trans, accountNo);

                        //#19193 - CR15594 - Check if its Ready Assist
                        var repository = new AccountRepository();
                        var readyAssist = repository.IsReadyAssist(conn, trans, accountNo, 1); //#18603

                        BPayment affinityPayment = new BPayment();
                        affinityPayment.DeliverAffinity(conn, trans, accountNo, readyAssist);

                        if (readyAssist == true)
                        {
                            var status = ReadyAssistStatus.Active;
                            repository.UpdateReadyAssistContractDate(conn, trans, accountNo, 1);
                            repository.UpdateReadyAssistStatus(conn, trans, accountNo, 1, status);
                        }

                    }
                }

                if (acct.AccountType == AT.Cash)
                {
                    bool autoDA = false;
                    if ((Math.Abs(clearedPayTot) >= (acct.AgreementTotal - valueOfWOC)) &&
                        !PayMethod.IsPayMethod(payMethod, PayMethod.Cheque))	/* is it fully paid */
                        autoDA = true;
                    else								/* or sufficiently paid */
                    {
                        //TO DO this needs to take into account warranties on credit
                        // i.e. should compare paytot with (agrmttot - woc)
                        // 67783 RD/DR Added to avoid getting devide by zero error
                        if ((acct.AgreementTotal - valueOfWOC) != 0)
                        {
                            if (((Math.Abs(clearedPayTot) / (acct.AgreementTotal - valueOfWOC)) * 100 >
                                (decimal)Country[CountryParameterNames.CODPercentage]) &&
                                !PayMethod.IsPayMethod(payMethod, PayMethod.Cheque) &&
                                agree.CODFlag == "Y")
                                autoDA = true;
                        }
                    }
                    if (autoDA)
                    {
                        string source = DASource.Auto; //IP - 04/02/10 - CR1072 - 3.1.9 - Source of Delivery Authorisation.
                        agree.User = User;

                        // Collect hold property value before clear proposal action.
                        var AcctR = new AccountRepository();
                        bool holdProp = AcctR.GetAccountData(accountNo, conn, trans);

                        agree.ClearProposal(conn, trans, accountNo, source);
                        clearProposal = true;

                        // CR - Jyoti - Create Service Request for Installation items only after Payment is done
                        // instead of creating it on line item sale on windows PoS.
                        if (holdProp)
                        {
                            ServiceRequestCreate(conn, trans, accountNo);
                        }

                        DataTable lineItemBooking = agree.LineItemBooking;

                        // #10178 - Send bookings to warehouse for scheduled orders
                        if (lineItemBooking != null && lineItemBooking.Rows.Count > 0)          //IP/JC - 30/05/12 - #10178
                        {
                            lineItemBooking.Columns.Add("BookingID");

                            AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);
                            AcctR.bookingType = "D";

                            foreach (DataRow row in lineItemBooking.Rows)
                            {
                                new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["BookingID"]));  //#13829 - Update before GetBookingData       //#13790 - Update LineItemBookingSchedule with the BookingId
                            }

                            var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);

                            new Chub().SubmitMany(bookings, conn, trans);
                            
                            
                        }
                        // CR - Deliver only non stock items.
                        agree.DeliverNonStocks(conn, trans, accountNo);
                        //#19216 - CR15594
                        var readyAssist = AcctR.IsReadyAssist(conn, trans, accountNo, 1);

                        BPayment affinityPayment = new BPayment();
                        affinityPayment.DeliverAffinity(conn, trans, accountNo, readyAssist);

                        if (readyAssist == true)
                        {
                            var status = ReadyAssistStatus.Active;
                            AcctR.UpdateReadyAssistContractDate(conn, trans, accountNo, 1);
                            AcctR.UpdateReadyAssistStatus(conn, trans, accountNo, 1, status);
                        }

                    }
                }
            }

            //IP - 15/04/09 - CR971 - commented out TransType.Repossession as when this is a repossession
            //the delivery record will have been written in SaveRepoDetails method which then would fire
            //a trigger to write the fintrans record. The DelTotal therefore already includes
            //the repossession value when calculated in GetByAcctNo.
            if (transType == TransType.AddTo
                || transType == TransType.Delivery
                || transType == TransType.GoodsReturn
                || transType == TransType.Redelivery
                //|| transType == TransType.Repossession 
                || transType == TransType.Refinance        //CR976 jec
                || transType == TransType.RefinanceDep         //CR976 jec
                || transType == TransType.Repossession2)
            {
                this.DelTot += transValue;		/* need to include the current transaction 
												 * which hasn't been written to the database
												 * yet */
            }

            // Settle the account if necessary. This requires reptot, rpotot and deltot.
            if (accountNo != paidAndTakenAcct)
            {
                // Various conditions will settle an account
                // BUT in all conditions the account must balance to ZERO (NOT IN CREDIT).
                // Note that small balances less than a cent above or below zero are considered to be zero.
                bool creditBalance = acct.OutstandingBalance <= -0.01M;
                bool zeroBalance = Math.Abs(acct.OutstandingBalance) < 0.01M;
                bool fullyDelivered = (this.DelTot != 0 && (acct.AgreementTotal <= this.DelTot || acct.AgreementTotal <= (this.DelTot - this.RepTot - this.RpoTot)));
                bool badDebtWriteOff = (transType == TransType.BadDebtWriteOff);
                bool isCancelled = acct.IsCancelled(conn, trans, accountNo);
                fin = new DFinTrans();
                warrantyAdjustment = (fin.GetWarrantyAdjustment(conn, trans, accountNo) > 0);

                // 67825 RD 14/03/2006 Added to check if cash account with zero agreement total is fully delivered, if so then settle
                DFinTrans da = new DFinTrans();
                bool isCashZeroAgrmt = da.GetCashAcctWith0Agrmttotal(conn, trans, accountNo);
                // will also settle if credit add-to transaction whether or not fully delivered.
                // uat376 rdb BDW Reversal - on BDW reversal DO NOT settle account (will enter here if we reverse an admin trans of 0)
                //if ((zeroBalance && (fullyDelivered || (transType == TransType.AddTo && transValue < 0)
                #region settle if 0 balance and fully delivered or writeoff

                if ((zeroBalance && (fullyDelivered || (transType == TransType.AddTo && transValue < 0 && acct.AgreementTotal == 0) //IP - 17/05/10 - UAT(165)UAT5.2.1.0 - only settle when reversing Add-To when agreement total is also 0.
                    || (transType == TransType.Refinance && transValue < 0)         //CR976 jec
                    || badDebtWriteOff || isCancelled || warrantyAdjustment || isCashZeroAgrmt))
                    && transType != TransType.BadDebtWriteOffReversal)
                {
                    // TO DO: make sure that we don't settle if there are
                    // any items in the lineitem table with no
                    // corresponding record in the delivery/schedule table
                    // May be free gifts so can't rely on DelTot alone
                    // Difficult because we don't know the line item or if its
                    // delivery record has been written yet??
                    acct.Arrears = 0;
                    acct.CurrentStatus = "S";
                    // check whether any items still outstanding - if restate order quantity of 0
                    DLineItem item = new DLineItem();
                    DFACTTrans fact = new DFACTTrans();
                    DBranch bc = new DBranch();
                    DDelivery del = new DDelivery();

                    double quantity = 0;
                    double price = 0;
                    double taxamt = 0;
                    double value = 0;
                    bool status = true;

                    string tranType = "01";
                    string tcCode = "58";

                    item.AccountNumber = accountNo;
                    DataTable dtItems = new DataTable();
                    dtItems = item.GetItemsForCanxAccount(conn, trans);    // 68181 RD 22/02/06 Modified so that canx records are posted to fact
                    if (dtItems.Rows.Count > 0) //get the buffno only if there are some rows to cancel.
                    {
                        if (clearProposal)
                        {
                            buffNo = bc.GetBuffNo(conn, trans, (short)branchNo); //#15993 // 69300 JH 29/10/2007 Passing the SqlConnection and SqlTransaction so as to avoid locking issues
                        }
                        else
                        {
                            buffNo = bc.GetBuffNo(conn, trans, (short)branchNo); //#15993
                        }
                    }

                    foreach (DataRow row in dtItems.Rows)
                    {
                        if ((string)row["ItemType"] == "S")
                        {
                            fact.AccountNumber = accountNo;
                            fact.ItemNumber = (string)row[CN.ItemNo];
                            fact.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                            fact.TranType = tranType;
                            fact.TCCode = tcCode;

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

                    //end paste
                }
                else if (creditBalance &&
                        transType != TransType.Repossession &&  // 68459 RD 23/08/06 
                        transType != TransType.Repossession2)
                {
                    acct.Arrears = 0;
                    acct.CurrentStatus = "1";
                }
                //CLA Amortization -- GFT -- To refund the excess amount from Refund Screen
                //CLA Amortization -- GFT -- To refund the excess amount from Refund Screen
                //RD 4/11/2019 --
                // If customer paid the excess of amount to settle the account then do not change arrears and acct status
                // for cash loan acct in case of refund
                else if ((new BPayment().IsCashLoanAmortizedAccount(null,null,accountNo)) && 
                        transType == TransType.Refund && 
                        acct.OutstandingBalance==0 && 
                        acct.Arrears<0 && 
                        acct.CurrentStatus == "S")
                {
                    acct.Arrears += transValue;
                    acct.CurrentStatus = "S";
                }

                else if (acct.CurrentStatus == "S")
                {
                    DStatus stat = new DStatus();
                    acct.CurrentStatus = stat.Unsettle(conn, trans, accountNo, DateTime.Now, User);
                }
            }
            #endregion
            // uat376 rdb BDW Reversal - dont add value to arrears forBDW reversal added to if
            if (acct.Arrears > 0 &&
                acct.AccountType != AT.Special &&
                transType != TransType.Delivery &&
                transType != TransType.GoodsReturn &&
                transType != TransType.Repossession &&
                transType != TransType.Repossession2 &&
                transType != TransType.AddTo &&
                transType != TransType.Refinance &&         //CR976 jec
                transType != TransType.RepoTransferredToRecovery && //IP - 15/04/09 - CR971
                transType != TransType.RepoAfterWriteoff && //IP - 15/04/09 - CR971
                transType != TransType.InsClaimTransferredToRecovery && //IP - 15/04/09 - CR971
                transType != TransType.InsClaimAfterWriteoff && //IP - 15/04/09 - CR971
                transType != TransType.BadDebtWriteOffReversal &&
                transType != TransType.WriteOffService)     //IP - 08/02/11 - Sprint 5.9 - #2977 //IP - 15/02/11 - Sprint 5.10 - #2977 - Changed from SDW to WOS
            {
                acct.Arrears += transValue;
            }

            if (transValue < 0 &&
                (transType == TransType.Payment ||
                transType == TransType.Transfer ||
                transType == TransType.GiroExtra ||
                transType == TransType.GiroNormal ||
                transType == TransType.GiroRepresent))
            {
                // acct.DateLastPaid = DateTime.Today;
                //IP - 22/10/2007 Livewire (68801)
                acct.DateLastPaid = dateTrans;
            }

            //IP - 18/02/10 - CR1072 - LW 69897 - Payment Fixes from 4.3 - Merge
            // 69897 if correction then get datelastpaid for this account
            //as reverting to previous datelastpaid more important for tallyman
            if (transValue > 0 &&
                (transType == TransType.Refund ||
                 transType == TransType.Correction ||
                 transType == TransType.Transfer))
            {
                acct.DateLastPaid = acct.GetDateLastpaid(conn, trans, accountNo, transValue);
            }

            // 68459 RD 14/09/06 Removed check for acct.OutstandingBalance >= 0.01M
            //  to ensure that Repo will always update status to 6
            if (transType == TransType.Repossession ||
                transType == TransType.Repossession2)
            {
                if (acct.CurrentStatus == "1" ||
                    acct.CurrentStatus == "2" ||
                    acct.CurrentStatus == "3" ||
                    acct.CurrentStatus == "4" ||
                    acct.CurrentStatus == "5" ||
                    acct.CurrentStatus == "0")
                {
                    acct.HighestStatus = "6";
                }
                acct.CurrentStatus = "6";
            }

            acct.Save(conn, trans);


            if (PayMethod.IsPayMethod(payMethod, PayMethod.Cheque) &&
                (transType == TransType.Payment ||
                transType == TransType.Return ||
                transType == TransType.Correction ||
                transType == TransType.Refund ||
                transType == TransType.DebtPayment))
            {
                DChequeDetail cheq = new DChequeDetail();
                cheq.Write(conn, trans, accountNo, bankCode, bankAcctNo, chequeNo, Convert.ToDouble(transValue), transRefNo);
            }

            /* This check has been removed because it is not always an error
             * for the deltot < 0. The only other delivered item on an account may be
             * a discount and therefore have a negative delivered value which 
             * is a perfectly legitimate state of affairs. 
             * 
            if( (transType == TransType.GoodsReturn ||
                transType == TransType.Repossession ||
                transType == TransType.Repossession2 ||
                transType == TransType.Delivery) &&
                transValue < 0 )
            {
                if(acct.AccountType != AT.Special)
                    if(this.DelTot < 0)
                        throw new Exception(GetResource("M_RETURNEXCEEDSDELIVERY", new object [] {transType}));
                        //throw new Exception(transType + " CR > DEL DR");
            }
            */

            if (transRefNo == 0)
                transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

            // Deliver a rebate
            if (transType == TransType.Rebate)
            {
                if (clearProposal)
                {
                    buffNo = branch.GetBuffNo(conn, trans, branchNo);   //#15993 // 69300 JH 29/10/2007 Passing the SqlConnection and SqlTransaction so as to avoid locking issues
                }
                else
                {
                    buffNo = branch.GetBuffNo(conn, trans, branchNo);   //#15993
                }

                delivery = new DDelivery();
                delivery.AccountNumber = accountNo;
                delivery.AgreementNumber = 1;
                delivery.BranchNumber = branchNo;
                delivery.BuffBranchNumber = branchNo;
                delivery.BuffNo = buffNo;
                delivery.DateDelivered = DateTime.Today;
                delivery.DateTrans = dateTrans;
                delivery.DeliveredQuantity = 1;
                delivery.DeliveryOrCollection = DelType.Normal;
                delivery.ItemNumber = "RB";
                delivery.ItemId = StockItemCache.Get(StockItemKeys.RB);
                delivery.Quantity = 1;
                delivery.StockLocation = branchNo;
                delivery.TransRefNo = transRefNo;
                delivery.TransValue = transValue;
                delivery.User = User;
                delivery.NotifiedBy = User;
                delivery.ParentItemNo = ""; //IP & JC -26/01/09 - Need to set this
                delivery.Write(conn, trans);
            }

            // If not Refinance transaction (RFN)   - CR976 jec 08/04/09 AA or rebate as that posted as part of trigger for delivery
            if (transType != TransType.Refinance && transType != TransType.RefinanceDep && transType != TransType.Rebate)
            {
                fin = new DFinTrans();
                fin.OrigBr = 0;
                fin.BranchNumber = branchNo;
                fin.AccountNumber = accountNo;
                fin.TransRefNo = transRefNo;
                fin.DateTrans = dateTrans;
                fin.TransTypeCode = transType;
                fin.EmployeeNumber = User;
                fin.TransUpdated = "";
                fin.TransPrinted = "N";
                fin.TransValue = transValue;
                fin.Source = "COSACS";
                fin.ChequeNumber = chequeNo;
                fin.BankAccountNumber = bankAcctNo;
                fin.BankCode = bankCode;
                fin.FTNotes = footNote;
                fin.PayMethod = payMethod;
                fin.Agrmtno = agrmtno;
                fin.Agrmtno = agrmtno;
                fin.CashierTotID = cashierTotID;            //IP/JC - 06/01/12 - #8821
                //DBranch dbranch = new DBranch();          //jec #3943 LW73615 this code causes a block 2nd time through
                //fin.TransRefNo = dbranch.BranchTransrefnoCheckUpdate(fin.AccountNumber, fin.BranchNumber, fin.TransRefNo);
                fin.Write(conn, trans);
                this.FintransId = fin.FintransId;
            }

            if (AT.IsCreditType(acct.AccountType))
            {
                BCustomer cust = new BCustomer();
                cust.SetAvailableSpend(conn, trans, acct.CustomerID);
            }

            //IP - 22/12/10 - Store Card - If a payment is being processed towards a Store Card account, we need to update the Store Card Available amount.
            //In the instance where Store Card is used as a payment method for purchases on cash /paid & taken accounts, the Store Card Available will be updated in the TransferTransaction method.
            if (acct.AccountType == AT.StoreCard && transType != TransType.StoreCardPayment)
            {
                CustomerUpdateStoreCardAvailable(conn, trans, acct.CustomerID);
            }

            //IP - 26/11/2007 - added '&& !this.IsCancellation' into condition as accounts with a warranty
            //adjustment appeared with two cancellation records, as there was no check to ensure that 
            //this.IsCancellation = false before a Cancellation record is written using the Country Parameter.
            // Only cancel account if delivery total is zero   jec UAT475 14/07/08
            if (this.DelTot == 0 &&            // delivery total zero
                this.GrtTot == 0 &&                  // don't cancel if there is a goods collection - 69449 
                                                     // and this 
                ((Math.Abs(acct.OutstandingBalance) < 0.01M && warrantyAdjustment && !this.IsCancellation) ||
                 // or this
                 (transType == TransType.Correction
                        || transType == TransType.Refund
                        || transType == TransType.SundryCreditTransfer
                        || transType == TransType.Transfer)
                    && (acct.IsRejected(conn, trans, accountNo)
                    && Math.Abs(acct.OutstandingBalance) < 0.01M
                    && !this.IsCancellation)))
            {
                BAccount ba = new BAccount();
                ba.CancelAccount(conn, trans, accountNo, acct.CustomerID, branchNo,
                    (string)Country[CountryParameterNames.CancellationRejectionCode],
                    0, countryCode, "", 0);
            }

            //Finally call the arrears calc routine
            acct.CalcArrears(conn, trans, 0, 0);

            if (!acct.CurrentStatus.Equals("6"))
            {
                /* re-run update status if this is not a
                 * repossession to make sure that the 
                 * account status reflects the state of affairs after the 
                 * transaction has been added */
                acct.UpdateStatus(conn, trans, accountNo);
            }
        }


        /*
        public BTransaction(SqlConnection conn, SqlTransaction trans, string accountNo, short branchNo, int refNo, decimal amount, int user, string type, string countryCode)
        {
            User = user;
            AddTransaction(conn, trans, accountNo, type, amount, countryCode, branchNo, refNo, "", "", "");
        }
        */

        public BTransaction(SqlConnection conn, SqlTransaction trans, string accountNo, short branchNo,
            int refNo, decimal amount, int user, string type,
            string bankCode, string bankAcctNo, string chequeNo,
            short payMethod, string countryCode, DateTime dateTrans,
            string footNote, int agrmtno, int? cashierTotID = null) //IP/JC - 06/01/12 - #8821
        {
            User = user;
            AddTransaction(conn, trans, accountNo, type, amount,
                countryCode, branchNo, refNo, chequeNo,
                bankAcctNo, bankCode, dateTrans, footNote, payMethod, agrmtno, cashierTotID);   //IP/JC - 06/01/12 - #8821
        }

        public DataSet GetReposessionAndRedelivery(string accountNumber)
        {
            DataSet ds = new DataSet();
            DFinTrans finTrans = new DFinTrans();
            finTrans.GetReposessionAndRedelivery(accountNumber);
            ds.Tables.Add(finTrans.FinTrans);

            return ds;
        }

        public DataSet GetFinTransQueryResults(DateTime datestart,
            DateTime datefinish,
            string transtypeoperand,
            string transtypecode,
            string valueoperand,
            int startrunno,
            int endrunno,
            string runnooperand,
            int startrefno,
            int endrefno,
            string refnooperand,
            int empeeno,
            string empeenooperand,
            int branchno,
            string branchnooperand,
            int accountinbranch,
            string dateoperand,
            string branchsetname,
            string transtypesetname,
            string employeesetname,
            int valueonly,
            int includeothercharges)
        {
            if (transtypeoperand.ToLower() == "from")
            {
                transtypeoperand = "between";
            }
            valueoperand.Replace("0", "");
            if (runnooperand.ToLower() == "from")
            {
                runnooperand = "between";
            }
            if (refnooperand.ToLower() == "from")
            {
                refnooperand = "between";
            }
            if (dateoperand.ToLower() == "from")
            {
                dateoperand = "between";
            }
            if (branchnooperand.ToLower().IndexOf("set") > 0)
            {
                branchnooperand = "set";
            }
            if (empeenooperand.ToLower().IndexOf("set") > 0)
            {
                empeenooperand = "set";
            }
            if (transtypeoperand.ToLower().IndexOf("set") > 0)
            {
                transtypeoperand = "set";
            }

            DataSet ds = new DataSet();
            DFinTrans finTrans = new DFinTrans();
            finTrans.GetFinTransQueryResults(datestart,
                datefinish,
                transtypeoperand,
                transtypecode,
                valueoperand,
                startrunno,
                endrunno,
                runnooperand,
                startrefno,
                endrefno,
                refnooperand,
                empeeno,
                empeenooperand,
                branchno,
                branchnooperand,
                accountinbranch,
                dateoperand,
                branchsetname,
                transtypesetname,
                employeesetname,
                valueonly,
                includeothercharges);
            ds.Tables.Add(finTrans.FinTrans);

            return ds;
        }

        public DataSet GetUnprintedTransactions(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DataSet ds = new DataSet();
            DFinTrans finTrans = new DFinTrans();
            finTrans.GetByAcctNo(conn, trans, accountNo);
            DataTable dt = finTrans.FinTrans.Clone();
            foreach (DataRow r in finTrans.FinTrans.Rows)
                if ((string)r[CN.TransPrinted] == "N")
                    dt.Rows.Add(r);
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Returns the financial transactions for an account and sets
        /// properties for the following totals:
        ///   AddTot ~ Add To;
        ///   AdmTot ~ Admin Charge;
        ///   DelTot ~ Delivery;
        ///   FeeTot ~ Giro Fee;
        ///   IntTot ~ Interest;
        ///   PayTot ~ Normal Payment;
        ///   RebTot ~ Rebate;
        ///   RepTot ~ Repossession;
        ///   RpoTot ~ Repossession (old);
        ///   UnclearChq ~ Uncleared Cheque;
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        /// 
        public DataSet GetByAcctNo(SqlConnection conn, SqlTransaction trans, string accountNumber)
        {
            DataSet ds = new DataSet();
            DFinTrans finTrans = new DFinTrans();
            finTrans.GetByAcctNo(conn, trans, accountNumber);
            ds.Tables.Add(finTrans.FinTrans);

            //
            // Calculate totals
            //
            this._AddTot = 0;
            this._AdmTot = 0;
            this._GrtTot = 0;
            this._DelTot = 0;
            this._FeeTot = 0;
            this._IntTot = 0;
            this._PayTot = 0;
            this._RebTot = 0;
            this._RepTot = 0;
            this._RpoTot = 0;
            this._UnclearChq = 0;

            foreach (DataRow row in finTrans.FinTrans.Rows)
            {
                if ((string)row[CN.TransTypeCode] == TransType.Payment
                    && PayMethod.IsPayMethod(Convert.ToInt16(row[CN.PayMethod].ToString()), PayMethod.Cheque)
                    && ((DateTime)row[CN.DateTrans]).AddDays(Convert.ToDouble(Country[CountryParameterNames.ChequeDays])) > DateTime.Today)
                {
                    this._UnclearChq += (decimal)row[CN.TransValue];
                }

                if ((string)row[CN.TransTypeCode] == TransType.Rebate)
                {
                    this._RebTot += (decimal)row[CN.TransValue];
                    if ((bool)Country[CountryParameterNames.NoCents]) this._RebTot = Decimal.Floor(this._RebTot);
                }
                else if ((string)row[CN.TransTypeCode] == TransType.Correction
                    || (string)row[CN.TransTypeCode] == TransType.GiroExtra
                    || (string)row[CN.TransTypeCode] == TransType.GiroNormal
                    || (string)row[CN.TransTypeCode] == TransType.GiroRepresent
                    || (string)row[CN.TransTypeCode] == TransType.TakeonTransfer
                    || (string)row[CN.TransTypeCode] == TransType.Payment
                    || (string)row[CN.TransTypeCode] == TransType.Refund
                    || (string)row[CN.TransTypeCode] == TransType.Return
                    || (string)row[CN.TransTypeCode] == TransType.SundryCreditTransfer
                    || (string)row[CN.TransTypeCode] == TransType.Transfer)
                {
                    this._PayTot += (decimal)row[CN.TransValue];
                }
                else if ((string)row[CN.TransTypeCode] == TransType.AddTo
                    || (string)row[CN.TransTypeCode] == TransType.Delivery
                    || (string)row[CN.TransTypeCode] == TransType.GoodsReturn
                    || (string)row[CN.TransTypeCode] == TransType.CashLoan          // jec 12/10/11
                    || (string)row[CN.TransTypeCode] == TransType.Redelivery
                    || (string)row[CN.TransTypeCode] == TransType.Repossession
                    || (string)row[CN.TransTypeCode] == TransType.Repossession2)
                {
                    this._DelTot += (decimal)row[CN.TransValue];

                    if ((string)row[CN.TransTypeCode] == TransType.AddTo)
                        this._AddTot += (decimal)row[CN.TransValue];

                    if ((string)row[CN.TransTypeCode] == TransType.GoodsReturn)
                        this._GrtTot += (decimal)row[CN.TransValue];

                    if ((string)row[CN.TransTypeCode] == TransType.Repossession)
                        this._RepTot += (decimal)row[CN.TransValue];

                    if ((string)row[CN.TransTypeCode] == TransType.Repossession2)
                        this._RpoTot += (decimal)row[CN.TransValue];
                }
                else if ((string)row[CN.TransTypeCode] == TransType.CreditFee)
                    this._FeeTot += (decimal)row[CN.TransValue];
                else if ((string)row[CN.TransTypeCode] == TransType.Interest)
                    this._IntTot += (decimal)row[CN.TransValue];
                else if ((string)row[CN.TransTypeCode] == TransType.AdminCharge)
                    this._AdmTot += (decimal)row[CN.TransValue];

            }  // End of foreach

            return ds;
        }


        public void SaveRepoDetails(SqlConnection conn, SqlTransaction trans, string type, decimal amount,
            string acctNo, decimal oustBalance, int branchNo, int user,
            string countryCode, DataSet ds, string accountType)
        {
            decimal collectionFee = 0;
            decimal bailiffFee = 0;
            decimal total = 0;
            int transRefNo = 0;
            int empNo = 0;
            int segmentId = 0;
            int debitAccount = 0;
            string empType = "";
            string empName = "";
            DateTime dateNow = DateTime.Now;

            DBranch branch = new DBranch();
            DDelivery del = new DDelivery();
            DAccount acct = new DAccount(conn, trans, acctNo);

            this.User = user;

            foreach (DataRow row in ds.Tables["Deliveries"].Rows)
            {
                short locn = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                if (locn == 0) locn = (short)Convert.ToInt32(row[CN.StockLocn]);
                transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);

                del.OrigBr = 0;
                del.AccountNumber = (string)row[CN.AcctNo];
                del.AgreementNumber = Convert.ToInt32(row[CN.AgrmtNo]);
                del.DateDelivered = Convert.ToDateTime(row[CN.DateDel]);
                del.DeliveryOrCollection = (string)row[CN.DelOrColl];
                del.ItemNumber = (string)row[CN.ItemNo];
                del.ItemId = Convert.ToInt32(row[CN.ItemId]);                   //IP - 25/05/11 - CR1212 - RI
                del.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                del.Quantity = Convert.ToDouble(row[CN.Quantity]);
                del.BuffNo = branch.GetBuffNo(conn, trans, locn);
                del.BuffBranchNumber = (short)Convert.ToInt32(row[CN.BuffBranchNo]);
                del.DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                del.BranchNumber = (short)Convert.ToInt32(row[CN.BuffBranchNo]);
                del.TransRefNo = transRefNo;
                del.TransValue = Convert.ToDecimal(row[CN.TransValue]);
                del.ReturnValue = Convert.ToDouble(row[CN.RetVal]);
                del.ReturnItemId = Convert.ToInt32(row[CN.RetItemId]);
                del.ReturnItemNumber = (string)row[CN.RetItemNo];
                del.ReturnStockLocation = Convert.ToInt16(row[CN.RetStockLocn]);
                del.RunNumber = 0;
                del.ContractNo = (string)row[CN.ContractNo];
                del.NotifiedBy = user;
                del.ftNotes = "DNRE";
                // uat363 rdb add parentItemNo
                del.ParentItemNo = row[CN.ParentItemNo].ToString();
                del.ParentItemId = Convert.ToInt32(row[CN.ParentItemId]);       //IP - 25/05/11 - CR1212 - RI
                del.Write(conn, trans);

                //RI cr1212
                if (Convert.ToString(row[CN.ItemType]) == "S")
                {
                    if (Convert.ToInt32(row[CN.RetItemId]) == 0)
                        throw new STLException("Invalid Repossessed Item : Item Id 0 is not allowed");

                    var qty = Math.Abs(Convert.ToDecimal(row[CN.Quantity]));
                    //var costPrice = Math.Abs(Convert.ToDecimal(row[CN.RetVal])) / qty;
                    var salePrice = Math.Abs(Convert.ToDecimal(row[CN.RetVal])) / qty;      // #4167 jec 06/07/11
                    salePrice = Math.Round(salePrice, 2);
                    del.UpdateRepossessedStock(conn, trans,
                                            Convert.ToInt32(row[CN.RetItemId]), Convert.ToInt16(row[CN.RetStockLocn]),
                                            salePrice, qty);
                }

                // write collection reason CR36
                del.CollectReason = (string)row[CN.Reason];
                del.CollectType = (string)row[CN.CollectionType];
                del.WriteCollectReason(conn, trans);

                if (del.TransValue != 0)
                    AddTransaction(conn, trans, acctNo, type, del.TransValue, countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", DateTime.Now, "", 0, del.AgreementNumber);
                //IP - 09/04/09 - CR971 - If the account has a BDWBalance or BDWCharges and is being repossessed
                //add 'RPR' (Repo Transferred to Recovery) transtype to fintrans as a debit for the same amount as the repossession 

                if (type == TransType.Repossession && (acct.BDWBalance > 0 || acct.BDWCharges > 0))
                {
                    DateTime transdate = DateTime.Now;

                    transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);
                    //Post an 'RPR' to the customers account which should reduce the balance to 0
                    AddTransaction(conn, trans, acctNo, TransType.RepoTransferredToRecovery, Math.Abs(del.TransValue), countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", transdate, "", 0, del.AgreementNumber);

                    //Retrieve the BDW account for this branch
                    string bdwAccountNo = acct.GetBadDebtWriteOffAccount(conn, trans, acct.Securitised, Convert.ToInt16(acctNo.Substring(0, 3)));
                    //Write the  'RPY' transaction to the BDW account and a link to the fintransaccount.
                    AddTransaction(conn, trans, bdwAccountNo, TransType.RepoAfterWriteoff, del.TransValue, countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", transdate, "", 0, del.AgreementNumber);

                    // Save a link on FintransAccount table
                    string fromAccountNo = acctNo;
                    DFintransAccount FALink = new DFintransAccount(bdwAccountNo,
                        fromAccountNo, transdate, (short)branchNo, transRefNo);
                    FALink.SaveAccountLink(conn, trans);

                }


                if (Convert.ToDecimal(row[CN.Refund]) != 0)
                {
                    // CR784 - the unexpired portion of the warranty will be credited
                    // onto the customer's account
                    string rebateType = (string)row[CN.Type] == "E" ? TransType.ElecWarrantyRecovery : TransType.FurnWarrantyRecovery;
                    transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);
                    AddTransaction(conn, trans, acctNo, rebateType, Convert.ToDecimal(row[CN.Refund]), countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", DateTime.Now, "", 0, del.AgreementNumber);
                }

                //#18610 - CR15594 - Post debit to account for used portion of Ready Assist
                if (Convert.ToDecimal(row["ReadyAssistUsed"]) > 0)
                {
                    string transType = AT.IsCreditType(acct.AccountType) ? TransType.ReadyAssistRecoveryCredit : TransType.ReadyAssistRecoveryCash;       //#19267

                    transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);
                    AddTransaction(conn, trans, acctNo, transType, Convert.ToDecimal(row["ReadyAssistUsed"]), countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", DateTime.Now, "", 0, del.AgreementNumber);
                }

                if (Convert.ToDecimal(row["AnnualServiceContractUsed"]) > 0)
                {
                    string transType = AT.IsCreditType(acct.AccountType) ? TransType.AnnualServiceRecoveryCredit : TransType.AnnualServiceRecoveryCash;

                    transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);
                    AddTransaction(conn, trans, acctNo, transType, Convert.ToDecimal(row["AnnualServiceContractUsed"]), countryCode, Convert.ToInt16(branchNo), transRefNo, "", "", "", DateTime.Now, "", 0, del.AgreementNumber);
                }


                //#18611 - CR15594
                if (Convert.ToBoolean(row["ReadyAssist"]) == true)
                {
                    var repository = new AccountRepository();

                    decimal unusedPortion = 0;

                    if (Convert.ToDecimal(row["ReadyAssistUsed"]) > 0)
                    {
                        unusedPortion = Convert.ToDecimal(Convert.ToDecimal(row["Price"]) - Convert.ToDecimal(row["ReadyAssistUsed"]));
                    }
                    else
                    {
                        unusedPortion = Convert.ToDecimal(row["Price"]);
                    }


                    var status = ReadyAssistStatus.Cancelled;
                    repository.UpdateReadyAssistUnusedPortion(conn, trans, acctNo, 1, unusedPortion);
                    repository.UpdateReadyAssistStatus(conn, trans, acctNo, 1, status);
                }


                total += del.TransValue;
            }

            decimal payAmount = total;

            DFollUpAlloc follup = new DFollUpAlloc();
            follup.GetAllocatedCourtsPerson(acctNo, ref empNo, ref empType, ref empName);

            payAmount = -payAmount;

            BPayment pay = new BPayment();
            pay.CalculateCreditFee(conn, trans, countryCode, acctNo,
                ref payAmount, TransType.Repossession,
                ref empNo, acct.Arrears, out collectionFee, out bailiffFee, out debitAccount, out segmentId);


            if (bailiffFee > 0 && (empNo != 0 || segmentId != 0))
            {
                transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);

                DBailiffCommn bcm = new DBailiffCommn();
                bcm.AllocatedCourtsPerson = empNo;
                bcm.AccountNo = acctNo;
                bcm.Status = "H";	/*Hold commission payment*/
                bcm.DateTrans = dateNow;    // Must match BailAction datetime
                bcm.TransRefNo = transRefNo;
                bcm.ChequeColln = "N";
                bcm.TransValue = bailiffFee;
                bcm.Save(conn, trans);
            }

            if (empNo != 0)
            {
                DBailAction ba = new DBailAction();
                ba.EmployeeNo = empNo;
                ba.AccountNo = acctNo;
                ba.Code = TransType.Repossession;
                ba.AmtCommPaidOn = Convert.ToDouble(total);
                if (acct.Arrears > 0 && Math.Abs(total) > Math.Abs(acct.Arrears))
                    ba.ActionValue = Convert.ToDouble(Math.Abs(acct.Arrears));
                else
                    ba.ActionValue = Convert.ToDouble(total);
                ba.DateAdded = dateNow;    // Must match BailiffCommn datetime
                ba.AddedBy = user;
                ba.Save(conn, trans);
            }

            /* this is no longer necessary since the lineitemwarranty table is not used 
            EraseWarranty(conn, trans, accountType, countryCode, acctNo, Convert.ToInt16(branchNo), ds);
            */

            User = user;

            //68459 Since this is a repossession the account status must be set to 6 - JH 23/07/2007
            //IP - 15/04/09 - CR971 - Archiving - If the account being repossessed has a BDWbalance > 0
            //or BDWCharges > 0 then we want the account to remain settled once the repossession has completed.
            if (acct.BDWBalance == 0 && acct.BDWCharges == 0)
                acct.UpdateRepossessionStatus(conn, trans, acctNo);
        }

        public void SetPaymentCardPrinted(SqlConnection conn, SqlTransaction trans, string accountNo,
            int transRefNo, DateTime transactionDate,
            string printed, int startLine)
        {
            DFinTrans ft = new DFinTrans();
            ft.SetPaymentCardPrinted(conn, trans, accountNo, transRefNo, transactionDate, printed, startLine);
        }

        public decimal GetRebate(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DFinTrans ft = new DFinTrans();
            return ft.GetRebate(conn, trans, accountNo);
        }

        public string GetAddedToAccount(SqlConnection conn, SqlTransaction trans, string accountNo, decimal addToValue)
        {
            DFinTrans ft = new DFinTrans();
            return ft.GetAddedToAccount(conn, trans, accountNo, addToValue);
        }

        public void WriteGeneralTransaction(SqlConnection conn, SqlTransaction trans,
            string accountNo, short branchNo,
            decimal amount, string transType, string bankCode,
            string bankAcctNo, string chequeNo, short payMethod,
            string countryCode, string footNote,
            int creditDebit)
        {
            /* First of all, what type of transaction are we writing?
             * For the majority we will just write the fintrans record 
             * and that's all, but for BDW and FEE transactions we have 
             * do a bit more work. */
            DataSet fintrans = GetByAcctNo(conn, trans, accountNo);
            BAccount acct = new BAccount();
            acct.Populate(conn, trans, accountNo);
            DBranch branch = new DBranch();
            DateTime dateTrans = this.TransactionDate = DateTime.Now;
            int refNo = this.TransactionRefNo= branch.GetTransRefNo(conn, trans, branchNo);
            BAgreement agreement = new BAgreement();
            agreement.Populate(conn, trans, accountNo, 1);
            //decimal rebate = 0;
            DAccount account = new DAccount(conn, trans, accountNo);
            BPayment payment = new BPayment();
            decimal outstandingBal = 0, bdcharges = 0;
            switch (transType)
            {
                case TransType.BadDebtWriteOff:
                    #region BadDebtWriteOff
                    /* add the "W" code to the account */
                    acct.AddCodeToAccount(conn, trans, accountNo, "W", User, dateTrans);

                    //Calculate the Unearned Finance Income BDU - split this out from BDW
                    var bduAmount = account.CalculateBduRebate(conn, trans, accountNo) * -1;
                    var bdwAmount = amount - bduAmount;

                    /* write the BDW record */
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        bdwAmount, User, TransType.BadDebtWriteOff,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);

                    if (Convert.ToString(acct.AccountType) != AT.StoreCard)
                    {
                        /* write the BDU record */
                        new BTransaction(conn, trans, accountNo,
                           branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                           bduAmount, User, TransType.BadDebtUnearnedFinanceIncome,
                           "", "", "", 0, countryCode, dateTrans,
                           footNote, agreement.AgreementNumber);
                    }


                    acct.OutstandingBalance += amount;
                    // The BDWBalance should be +ve or zero LW69125 JH 19/07/2007
                    acct.BDWBalance = -amount;

                    /* reverse the interest payments */
                    amount = IntTot;

                    if (amount < acct.OutstandingBalance)
                        amount = -amount;
                    else
                        amount = -acct.OutstandingBalance;

                    if (amount != 0)
                    {
                        new BTransaction(conn, trans, accountNo,
                            branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                            amount, User, TransType.Interest,
                            "", "", "", 0, countryCode, dateTrans,
                            footNote, agreement.AgreementNumber);
                        acct.OutstandingBalance += amount;
                    }

                    /* reverse the admin fee */
                    amount = AdmTot;

                    if (amount < acct.OutstandingBalance)
                        amount = -amount;
                    else
                        amount = -acct.OutstandingBalance;

                    if (amount != 0)
                    {
                        new BTransaction(conn, trans, accountNo,
                            branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                            amount, User, TransType.AdminCharge,
                            "", "", "", 0, countryCode, dateTrans,
                            footNote, agreement.AgreementNumber);
                        acct.OutstandingBalance += amount;
                    }
                    // 68473 RD Modified in order to re-open account on processing reversal of BDW
                    if (creditDebit == -1)
                    {
                        /* status will have been saved to S in the BTransaction constructor 
					    * must make sure we don't overwrite to what it was before here */
                        acct.Arrears = 0;
                        acct.CurrentStatus = "S";

                        acct.BDWCharges = IntTot + AdmTot;
                        acct.User = this.User;
                        acct.Save(conn, trans);
                    }
                    else
                    {
                        // Will have to change the back to 6 as this would be status after repo
                        acct.Arrears = acct.OutstandingBalance;
                        acct.CurrentStatus = "6";

                        acct.BDWCharges = IntTot + AdmTot;
                        acct.User = this.User;
                        acct.Save(conn, trans);
                    }
                    break;
                #endregion
                case TransType.CreditFee:
                    #region CreditFee
                    /* write the fee transaction */
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, transType,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);

                    if (creditDebit == -1)
                    {
                        DBailiffCommn bail = new DBailiffCommn();
                        bool found = false;
                        fintrans.Tables[TN.FinTrans].DefaultView.RowFilter = "transtypecode = 'FEE' and transvalue = " + (-amount).ToString();

                        foreach (DataRowView r in fintrans.Tables[TN.FinTrans].DefaultView)
                        {
                            if (bail.Erase(conn, trans, (int)r[CN.TransRefNo]))
                                found = true;
                        }

                        /* if we didn't find an exact match remove enough bailiffcomm
                         * records to cover the FEE - logic copied from OpenRoad */
                        if (!found)
                        {
                            decimal toErase = 0;
                            fintrans.Tables[TN.FinTrans].DefaultView.RowFilter = "transtypecode = 'FEE' and transvalue > 0";
                            foreach (DataRowView r in fintrans.Tables[TN.FinTrans].DefaultView)
                            {
                                toErase += (decimal)r[CN.TransValue];
                                if (toErase < (-amount))
                                {
                                    if (!bail.Erase(conn, trans, (int)r[CN.TransRefNo]))
                                        toErase -= (decimal)r[CN.TransValue];
                                }
                                else
                                    toErase -= (decimal)r[CN.TransValue];
                            }
                        }
                    }
                    break;
                #endregion
                case TransType.InsuranceClaim: //IP - 16/04/09 - CR971 - Archiving
                    #region InsuranceClaim
                    //First need to process the 'INS' transaction on the customers account.
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, transType,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);

                    //If the account has a BDWBalance or BDWCharges
                    if (account.BDWBalance > 0 || account.BDWCharges > 0)
                    {
                        decimal transvalue = 0;
                        //We need to post the opposite amount
                        //as an 'IPR' transaction on the customers account which is determined by the 'INS'
                        //transaction being a credit or debit transaction.
                        if (creditDebit == -1)
                        {
                            transvalue = Math.Abs(amount);
                        }
                        else
                        {
                            transvalue = -1 * amount;
                        }
                        //Firstly need to post an 'IPR' transaction (Insurance Claim Transferred to recovery)
                        //on the customers account to reduce the balance to 0.
                        refNo = branch.GetTransRefNo(conn, trans, (short)branchNo);
                        AddTransaction(conn, trans, accountNo, TransType.InsClaimTransferredToRecovery, transvalue, countryCode, Convert.ToInt16(branchNo), refNo, "", "", "", dateTrans, "", 0, agreement.AgreementNumber);

                        //Retrieve the BDW account for this branch
                        string bdwAccountNo = account.GetBadDebtWriteOffAccount(conn, trans, account.Securitised, Convert.ToInt16(accountNo.Substring(0, 3)));
                        //Write the  'IPY' transaction to the BDW account and a link to the fintransaccount.
                        AddTransaction(conn, trans, bdwAccountNo, TransType.InsClaimAfterWriteoff, amount, countryCode, Convert.ToInt16(branchNo), refNo, "", "", "", dateTrans, "", 0, agreement.AgreementNumber);

                        // Save a link on FintransAccount table
                        string fromAccountNo = accountNo;
                        DFintransAccount FALink = new DFintransAccount(bdwAccountNo,
                            fromAccountNo, dateTrans, (short)branchNo, refNo);
                        FALink.SaveAccountLink(conn, trans);

                    }
                    break;
                #endregion
                case TransType.Rebate:
                    #region Rebate
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, transType,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);


                    bool isCancelled = account.IsCancelled(conn, trans, accountNo);
                    /*sl: amend outstanding balance to account for rebate */
                    //account.OutstandingBalance = account.OutstandingBalance + amount; RM 23/9/09 this issue failed was adding outstanding balance twice

                    if ((((Math.Abs(account.OutstandingBalance) < 0.01M) &&
                        (account.AgreementTotal == this.DelTot ||
                        account.AgreementTotal == this.DelTot - this.RepTot - this.RpoTot) &&
                        this.DelTot != 0) ||
                        (Math.Abs(account.OutstandingBalance) < 0.01M && isCancelled)))
                    {
                        /* TO DO: make sure that we don't settle if there are
                         * any items in the lineitem table with no
                         * corresponding record in the delivery/schedule table
                         * May be free gifts so can't rely on DelTot alone
                         * Difficult because we don't know the line item or if its
                         * delivery record has been written yet?? */
                        account.Arrears = 0;
                        account.CurrentStatus = "S";
                    }
                    else
                    {
                        if (Math.Abs(account.OutstandingBalance) >= 0.01M &&
                            account.CurrentStatus == "S")
                        {
                            DStatus stat = new DStatus();
                            account.CurrentStatus = stat.Unsettle(conn, trans, accountNo, DateTime.Now, User);
                        }
                    }

                    // add amount to balance calculated before current transaction
                    account.OutstandingBalance += amount;

                    account.Save(conn, trans);
                    break;
                #endregion
                case TransType.Refund:   //68738 Added code to settle an account after a refund [PC] 10/Jan/2007
                    #region Refund
                    this.TransactionRefNo = branch.GetTransRefNo(conn, trans, branchNo);
                    BTransaction btran=new BTransaction(conn, trans, accountNo,
                       branchNo, this.TransactionRefNo,
                       amount, User, transType, bankCode,
                       bankAcctNo, chequeNo, payMethod, countryCode, dateTrans,
                       footNote, agreement.AgreementNumber);
                    this.FintransId = btran.FintransId;
                    btran.FintransId = 0;

                    //73190 Prevent Cash and Go acccount settling after refund.
                    string paidAndTakenAcct = acct.GetPaidAndTakenAccount(conn, trans, branchNo.ToString());

                    if (Math.Abs(account.OutstandingBalance) < 0.01M && Math.Abs(account.AgreementTotal) < 0.01M
                        && account.AccountNumber != paidAndTakenAcct)
                    {
                        account.CurrentStatus = "S";
                        account.Save(conn, trans);
                    }

                    #endregion
                    break; //End 68738


                case TransType.CLAdminFee:                                  //#CL Amortization Outstanding Bal Calculation, Rahul D, Zensar, 25/JUL/2019
                //case TransType.CLBDRecovey:
                case TransType.CLCreditBalance:
                case TransType.CLInsuranceClaim:
                case TransType.CLPenaltyInterest:
                case TransType.CLServiceChargeCorrection:
                case TransType.CLLateFeeReversal:
                    /* write the transaction record */
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, transType,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);

                    // To perform GFT transaction on CLAmortisationPayment History Table
                    payment.CLGeneralFinanceTransaction(conn, trans, accountNo, amount, creditDebit, transType);
                    payment.GetOutstandingAndCharges(accountNo, out outstandingBal, out bdcharges);
                    acct.OutstandingBalance = outstandingBal;
                    acct.Arrears += amount;// < 0 ? -1*amount : amount;
                    acct.Save(conn, trans);
                    break;
                case TransType.CLWriteOff:
                    #region Cash loan General Finance transaction 
                    /* add the "W" code to the account */
                    acct.AddCodeToAccount(conn, trans, accountNo, "W", User, dateTrans);

                    /* write the CLW record */
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, TransType.CLWriteOff,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);
                    // To perform GFT transaction on CLAmortisationPayment History Table
                    //payment.GetOutstandingAndCharges(accountNo, out outstandingBal, out bdcharges);
                    payment.CLGeneralFinanceTransaction(conn, trans, accountNo, amount, creditDebit, transType);

                    acct.Arrears = 0;
                    acct.CurrentStatus = "S";
                    acct.BDWBalance = acct.OutstandingBalance;
                    acct.OutstandingBalance = 0;
                    acct.BDWCharges = Math.Abs(amount)-acct.BDWBalance;
                    acct.User = this.User;
                    acct.Save(conn, trans);
                    #endregion
                    break;

                default:
                    new BTransaction(conn, trans, accountNo,
                        branchNo, branch.GetTransRefNo(conn, trans, branchNo),
                        amount, User, transType,
                        "", "", "", 0, countryCode, dateTrans,
                        footNote, agreement.AgreementNumber);
                    break;
            }
        }


        /// <summary>
        /// SaveLoyaltyCardRecord
        /// </summary>
        /// <param name="transrefno">int</param>
        /// <param name="datetrans">DateTime</param>
        /// <param name="acctno">string</param>
        /// <param name="morerewardsno">string</param>
        /// <param name="agreementno">int</param>
        /// <returns>void</returns>
        /// 
        public void SaveLoyaltyCardRecord(SqlConnection conn, SqlTransaction trans,
                    int transrefno, DateTime datetrans,
                    string acctno, string morerewardsno,
                    int agreementno)
        {

            DEposLoyaltyCard da = new DEposLoyaltyCard();
            da.Save(conn, trans, transrefno, datetrans, acctno, morerewardsno, agreementno);

        }

        /// <summary>
        /// GetForTransfer
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetForTransfer(string acctno, DateTime beforeDate, bool limitRows, out decimal availableTransfer)
        {
            DFinTrans da = new DFinTrans();
            return da.GetForTransfer(acctno, beforeDate, limitRows, out availableTransfer);
        }

        public int TransferTransaction(SqlConnection conn, SqlTransaction trans,
            string fromAccountNo,
            string toAccountNo,
            string transType,
            decimal transValue,
            short branchNo,
            string countryCode,
            DateTime dateTrans,
            string reasonCode,
            int oldRefNo,
            short payMethod,
            int agrmtNo,          //IP - 29/11/10 - Store Card - Added agrmtNo
            Int64? storeCardNo,   //IP - 30/11/10 - Added storeCardNo
            string cashierTotID = ""  //IP - 14/02/12 - #8819 - CR1234 - for transfer of overage/shortage need to record this in finxfr table.
           )
        {
            int refNo = 0;

            if (Math.Abs(transValue) > 0)
            {
                DBranch branch = new DBranch();
                DFinXfr xfer = new DFinXfr();

                if (transType == TransType.Transfer || transType == TransType.SundryCreditTransfer) //IP - 07/03/11 - #2636 - Rebate may need to be posted if transferring from sundry to a customers account
                {
                    if (transType == TransType.Transfer) //IP - 07/03/11 - #2636 - previously this was only done for XFR, therefore no need to do this for SCX 
                    {
                        // Attempt to void the gift voucher record if this is a gift voucher reversal
                        DGiftVoucher gv = new DGiftVoucher();
                        gv.VoidOther(conn, trans, fromAccountNo, oldRefNo);
                    }

                    // Check whether the account can be settled with a rebate
                    decimal settlement = 0;
                    decimal rebate = 0;
                    decimal collectionFee = 0;
                    BPayment payment = new BPayment();
                    payment.GetAccountSettlement(conn, trans, countryCode, toAccountNo,
                        out settlement, out rebate, out collectionFee);

                    // Transfer Transaction will ignore any fee due
                    settlement = settlement - collectionFee;

                    if (transValue >= settlement)
                    {
                        // Post the rebate if any due
                        if (rebate >= 0.01M)
                        {
                            refNo = branch.GetTransRefNo(conn, trans, branchNo);
                            AddTransaction(conn, trans, toAccountNo, TransType.Rebate, -rebate, countryCode,
                                branchNo, refNo, toAccountNo, "", "", dateTrans, reasonCode, payMethod, agrmtNo); //IP - 29/11/10 - Store Card - use agrmtNo instead of 0
                        }
                    }
                }
                int transrefnoRequired = 0;
                if (transType == TransType.TakeonTransfer)
                {
                    toAccountNo = "JOURNAL";
                    transrefnoRequired = 1;
                }
                else
                    transrefnoRequired = 2;

                BAgreement bagr = new BAgreement();
                bagr.GetAgreement(conn, trans, fromAccountNo, 0, false);           //IP - 30/11/2010 - Store Card - Get the agreement number for the from account //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans


                // Transfer FROM account
                //refNo = branch.GetTransRefNo(branchNo); 
                refNo = branch.GetTransRefNos(conn, trans, branchNo, transrefnoRequired) - 1;
                AddTransaction(conn, trans, fromAccountNo, transType, transValue, countryCode,
                    branchNo, refNo, toAccountNo, "", "", dateTrans, reasonCode, payMethod, bagr.AgreementNumber); //IP - 30/11/10 - Store Card - use AgreementNumber instead of 0

                //RD - 05/07/2019 - #CLA outstanding -- Calling function to deduct Amount from Payment history table for CLA outstading Acct
                DAccount toAcct = new DAccount();
                toAcct.GetAccount(toAccountNo);
                DAccount fromAcct = new DAccount();
                fromAcct.GetAccount(fromAccountNo);
                BPayment bp = new BPayment();
                if (toAcct.IsAmortized == true && toAcct.IsAmortizedOutStandingBal == true)
                    bp.DeductAmountFromPaymentHistory(conn, trans, toAccountNo, transValue); // Deduct amount from account in table Payment history
                if (fromAcct.IsAmortized == true && fromAcct.IsAmortizedOutStandingBal == true)
                    bp.CLAmortizationReversePayment(conn, trans, fromAccountNo, transValue);  //Reverse amount from account in table Payment History
                                                                                              // RD Code End for #CLA outstanding

                xfer.Write(conn, trans, fromAccountNo, refNo, dateTrans, toAccountNo, agrmtNo, storeCardNo, cashierTotID, oldRefNo); //IP - 20/02/12 - #9633 - CR1234 - save original transRefNo //IP - 14/02/12 - #8819 - CR1234 //IP - 30/11/10 - Store Card - Added storeCardNo

                //Transfer TO account
                if (transType != TransType.TakeonTransfer)
                {
                    refNo++;
                    AddTransaction(conn, trans, toAccountNo, transType, -transValue, countryCode,
                        branchNo, refNo, fromAccountNo, "", "", dateTrans, reasonCode, payMethod, agrmtNo); //IP - 29/11/10 - Store Card - use agrmtNo instead of 0
                    xfer.Write(conn, trans, toAccountNo, refNo, dateTrans, fromAccountNo, bagr.AgreementNumber, storeCardNo, cashierTotID, oldRefNo); //IP - 20/02/12 - #9633 - CR1234 - save original transRefNo //IP - 15/02/12 - #8819 - CR1234 - Added cashierTotID //IP - 30/11/10 - Store Card - Added agrmtNo and storeCardNo

                }

                if (transType == TransType.StoreCardRefund)
                {
                    var cp = new DCorrectedPayments();
                    cp.Write(conn, trans, fromAccountNo, oldRefNo, refNo);
                }
                //IP - 22/12/10 - Store Card - If Store Card is used as a payment method then once transfer processed we need to update the Store Card Available amount.
                if (transType == TransType.StoreCardPayment || transType == TransType.StoreCardRefund)
                {
                    //First retrieve the customer that the Store Card is linked to.
                    //var storeCardRepo = new StoreCardRepository();
                    var custid = new StoreCardRepository().CustomerIdFromStoreCard(storeCardNo);

                    //First make sure that the RF Available is updated before updating the Store Card Available
                    BCustomer cust = new BCustomer();
                    cust.SetAvailableSpend(conn, trans, custid);

                    CustomerUpdateStoreCardAvailable(conn, trans, custid);

                }


            }
            return refNo;
        }

        public DataSet JournalEnquiryGet(DateTime datefirst, DateTime datelast,
            int firstrefno,
            int lastrefno,
            int empeeno,
            int branch,
            int combination)
        {
            DFinTrans fint = new DFinTrans();
            fint.JournalEnquiryGet(datefirst, datelast, firstrefno, lastrefno, empeeno, branch, combination);
            return fint.Transactions;
        }

        /// <summary>
        /// GetSundryAccountTransactionTotal
        /// </summary>
        /// <param name="branchno">int</param>
        /// <param name="before">DateTime</param>
        /// <param name="total">double</param>
        /// <returns>int</returns>
        /// 
        public int GetSundryAccountTransactionTotal(int branchno, DateTime before, out decimal total)
        {
            int status = 0;
            DFinTrans da = new DFinTrans();
            status = da.GetSundryTotal(branchno, before, out total);

            return status;
        }

        /// <summary>
        /// GetCashiersWithOutstandingPayments
        /// </summary>
        /// <param name="branchno">int</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetCashiersWithOutstandingPayments(short branchno)
        {
            DEmployee da = new DEmployee();
            return da.GetCashiersWithOutstandingPayments(branchno);
        }

        public DataSet GetSUCBFinancialDetails(int runno, SqlConnection conn, SqlTransaction trans)
        {
            DataSet ds = new DataSet();
            DFinTrans ft = new DFinTrans();

            ds.Tables.AddRange(new DataTable[] {ft.SUCBGetFinancialTotals(runno),
                                                   ft.SUCBGetFinancialDetails(runno,conn ,  trans)});
            return ds;
        }
        // CR907  - AutoDA if deposit/first instal has been paid              jec 24/08/07
        public void AutoDA(SqlConnection conn, SqlTransaction trans, string accountNo, int user)
        {
            // CR907                jec 02/08/07
            // If account qualifies for Instant Credit and deposit/first instal has been paid AutoDA

            bool autoDA = false;
            string paid = "";
            string source = DASource.Auto; //IP - 04/02/10 - CR1072 - 3.1.9 - Source of Delivery Authorisation

            DAccount dfi = new DAccount();
            paid = dfi.DepFirstInstal(conn, trans, accountNo, 0, user);
            autoDA = (paid == IC.Approved);
            // if deposit/first instal paid  - AutoDA
            if (autoDA)
            {
                DAgreement agree = new DAgreement();
                agree.User = user;

                agree.ClearProposal(conn, trans, accountNo, source);
                clearProposal = true;

                BPayment affinityPayment = new BPayment();
                affinityPayment.DeliverAffinity(conn, trans, accountNo);
            }

        }

        //IP - 22/12/10 - Store Card - Update the Store Card Available for a Customer
        public void CustomerUpdateStoreCardAvailable(SqlConnection conn, SqlTransaction trans, string custID)
        {
            decimal storeCardLimit = 0;
            decimal storeCardAvailable = 0;

            DCustomer cust = new DCustomer();
            cust.CustomerUpdateAndGetStoreCardAvailable(conn, trans, ref storeCardLimit, ref storeCardAvailable, custID);

        }

        // Jyoti - Create Service Request CR for Installation items only after Delivery Authorization is done.
        public void ServiceRequestCreate(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            var Inst = new InstallationRepository();

            var installations = (IEnumerable<RequestSubmit>)Inst.GetInstallationDataPaidAndTaken(conn, trans, accountNo);

            new Chub().SubmitMany(installations, conn, trans);
        }
    }
}
