using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.BLL.WebApi;
using STL.Common;
using STL.Common.Constants;
using STL.Common.Constants.AccountCodes;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.Categories;
using STL.Common.Constants.CodeReference;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ExchangeRate;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.DAL;


namespace STL.BLL
{
    /// <summary>
    /// Summary description for BPayment.
    /// </summary>
    public class BPayment : CommonObject
    {
        //
        // Temporary Country stuff for rounding to country decimal places.
        // TO DO: replace with a common function server side.
        //
        private DCountry _country = null;
        private string _countryDecimalPlaces = "";
        private int _countryPrecision = 2;
        private int count = 0;
        private bool IsNumeric(string text)
        {
            Regex reg = new Regex("((^[+-][0-9]+$)|(^[0-9]*$))");
            return reg.IsMatch(text);
        }

        private void GetCountry(SqlConnection conn, SqlTransaction trans, string countryCode)
        {
            if (this._country == null)
            {
                this._country = new DCountry();
                this._country.GetDefaults(conn, trans, countryCode);
                DataRow row = this._country.Table.Rows[0];
                this._countryDecimalPlaces = (string)row[CN.DecimalPlaces];

                // Try to extract country precision from Decimal Places format (or leave as 2)
                if (this.IsNumeric(this._countryDecimalPlaces.Substring(1, 1)))
                {
                    this._countryPrecision = System.Convert.ToInt32(this._countryDecimalPlaces.Substring(1, 1));
                }
            }
        }

        private new void CountryRound(ref decimal moneyValue)
        {
            // Round money to Country Decimal Places
            moneyValue = Math.Round(moneyValue, this._countryPrecision);
        }

        /// <summary>
        /// CalculateCreditFee overloaded to calculate a list of accounts
        /// </summary>
        public decimal CalculateCreditFee(
            SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            ref DataSet accountSet,		//mandatory
            ref decimal paymentAmount,	//mandatory but if zero calcs WHOLE payment
            string paymentType)			//mandatory
        {
            decimal totFee = 0;
            decimal totPayment = 0;
            int debitAccount;
            decimal curPayment;
            decimal curCollectionFee;
            decimal curBailiffFee;
            int empeeNo;
            int segmentId;

            foreach (DataTable accountList in accountSet.Tables)
            {
                if (accountList.TableName == TN.Payments || accountList.TableName == TN.Accounts)
                {
                    foreach (DataRow accountRow in accountList.Rows)
                    {
                        // The row is ignored if there is no payment on this row,
                        // unless the overall payment has been entered as zero,
                        // which assumes the user wants the whole payment returned.
                        curCollectionFee = 0;
                        curBailiffFee = 0;
                        curPayment = (decimal)accountRow[CN.Payment];
                        empeeNo = (int)accountRow[CN.EmployeeNo];
                        segmentId = 0;
                        debitAccount = 0;

                        if ((decimal)accountRow[CN.Arrears] >= 0.01M
                            && (curPayment >= 0.01M || paymentAmount == 0))
                        {
                            this.CalculateCreditFee(
                                conn,
                                trans,
                                countryCode,
                                (string)accountRow[CN.acctno],
                                ref curPayment,
                                paymentType,
                                ref empeeNo,
                                (decimal)accountRow[CN.Arrears],
                                out curCollectionFee,
                                out curBailiffFee,
                                out debitAccount,
                                out segmentId);
                        }
                        totFee += curCollectionFee;
                        totPayment += curPayment;
                        accountRow[CN.EmployeeNo] = empeeNo;
                        accountRow[CN.SegmentID] = segmentId;
                        accountRow[CN.DebitAccount] = debitAccount;
                        accountRow[CN.CalculatedFee] = curCollectionFee;
                        accountRow[CN.CollectionFee] = curCollectionFee;
                        accountRow[CN.BailiffFee] = curBailiffFee;
                        accountRow[CN.NetPayment] = curPayment - curCollectionFee;
                        accountRow[CN.Payment] = curPayment;
                    }
                }
            }
            paymentAmount = totPayment;
            return totFee;
        }

        /// <summary>
        /// CalculateCreditFee overloaded to calculate a single account
        /// </summary>
        public void CalculateCreditFee(
            SqlConnection conn,
            SqlTransaction trans,
            string countryCode,
            string accountNo,				//mandatory
            ref decimal paymentAmount,		//mandatory but if zero calcs WHOLE payment
            string paymentType,				//mandatory
            ref int allocatedCourtsPerson,	//optional
            decimal arrears,				//optional
            out decimal collectionFee,		//output only CR680, CR377, CR158
            out decimal bailiffFee,			//output only CR680, CR377, CR158
            out int debitAccount,			//output only
            out int segmentId,              //Tallyman segmentid returned
            bool reverseFeeCalc = false)		// #13746		
        {
            //bool status = true;
            decimal collectionPercent = 0;	// To be collected from the customer
            decimal commissionPercent = 0;	// To be paid to the Bailiff

            DataRow collectionRates = null;
            DAccount acct = null;
            DFollUpAlloc fua = null;
            collectionFee = 0;
            bailiffFee = 0;
            debitAccount = 0;
            segmentId = 0;

            // Need country decimal places rounding
            this.GetCountry(conn, trans, countryCode);

            // Load arrears if not supplied
            if (arrears == 0)
            {
                acct = new DAccount(conn, trans, accountNo);
                acct.CalcArrears(conn, trans, 0, 0);
                arrears = acct.Arrears;
            }

            //IP - 03/06/10 - UAT(248) UAT5.2.1.0 Log - Below section commented out to match version 4.3
            //AA 248 calculate credit fee based on strategy account in 
            bool doFeeCalculation = true;
            if ((bool)Country[CountryParameterNames.LinkToTallyman])
            {
                doFeeCalculation = false;
                collectionFee = TMIsinFeeStrategy(accountNo);

                if (collectionFee > 0)
                {
                    // Will debit account for Tallyman if in arrears
                    doFeeCalculation = true;
                }

            }

            if (doFeeCalculation == true)
            {
                // Need to keep this arrears value before it is recalculated because
                // old OpenROAD code used this to determine Whole/Part payment
                decimal hackArrears = arrears;

                // Load allocated courts person if not supplied
                if (allocatedCourtsPerson == 0)
                {
                    fua = new DFollUpAlloc();
                    fua.Populate(conn, trans, accountNo);
                    allocatedCourtsPerson = fua.EmployeeNo;
                }

                if (arrears > 0 && allocatedCourtsPerson != 0)
                {
                    //
                    // Get the collection percentage
                    //
                    DBailCommnBas comm = new DBailCommnBas();

                    if (paymentType == TransType.Repossession)
                    {
                        // For Repossesion there is no collection fee, only the commission
                        collectionRates = comm.GetCollectionRates(conn, trans, allocatedCourtsPerson, accountNo, CollectionType.Repossession);

                        collectionPercent = 0;
                        commissionPercent = Convert.ToDecimal(collectionRates[CN.CommissionPercent]);
                        decimal repossPercent = Convert.ToDecimal(collectionRates[CN.RepossPercent]);
                        decimal allocPercent = Convert.ToDecimal(collectionRates[CN.AllocPercent]);
                        decimal repPercent = Convert.ToDecimal(collectionRates[CN.RepPercent]);

                        if (repossPercent > 0)
                        {
                            commissionPercent = repossPercent;
                            if (acct == null) acct = new DAccount(conn, trans, accountNo);
                            arrears = acct.RepossArrears;
                        }
                        else
                        {
                            if (fua == null)
                            {
                                fua = new DFollUpAlloc();
                                fua.Populate(conn, trans, accountNo);
                            }
                            if (allocPercent > 0 && fua.AllocatedArrears > 0)
                            {
                                commissionPercent = allocPercent;
                                arrears = fua.AllocatedArrears;
                            }
                            else if (repPercent > 0)
                            {
                                commissionPercent = repPercent;
                            }
                        }
                    }
                    else
                    {
                        // For Whole or Part Payment
                        // Assume Whole to find out whether the payment will cover the full fee
                        collectionRates = comm.GetCollectionRates(conn, trans, allocatedCourtsPerson, accountNo, CollectionType.Whole);

                        collectionPercent = Convert.ToDecimal(collectionRates[CN.CollectionPercent]);
                        commissionPercent = Convert.ToDecimal(collectionRates[CN.CommissionPercent]);
                        debitAccount = Convert.ToInt16(collectionRates[CN.DebitAccount]);

                        // #13746 - reverse calculate payent & fee from TotalAmount
                        if (reverseFeeCalc)
                        {
                            arrears = arrears / (1 + collectionPercent / 100);
                        }

                        this.CountryRound(ref paymentAmount);
                        this.CountryRound(ref arrears);
                        decimal wholeFeePart = 0;

                        // The fee part only needs to be included if it is going to debit the account
                        if (debitAccount == 1)
                        {
                            // Need to use a different fee equation to agree with old OpenROAD code which
                            // used a different equation when determining Whole/Part payment.
                            // decimal wholeFeePart = this.FeeEquation(arrears, 0, collectionPercent, bailiffCommission);
                            wholeFeePart = this.HackFeeEquation(hackArrears, 0, collectionPercent, collectionRates);
                            this.CountryRound(ref wholeFeePart);
                        }

                        if (paymentAmount > 0 && paymentAmount < (arrears + wholeFeePart))
                        {
                            // It is only a Part Payment
                            collectionRates = comm.GetCollectionRates(conn, trans, allocatedCourtsPerson, accountNo, CollectionType.Part);
                            collectionPercent = Convert.ToDecimal(collectionRates[CN.CollectionPercent]);
                            commissionPercent = Convert.ToDecimal(collectionRates[CN.CommissionPercent]);
                        }

                        // If no collectionPercent or debitAccount is false then collection fee is zero
                        if (collectionPercent > 0 && debitAccount == 1)
                        {
                            // Calculate based on the retrieved arrears and collection rate
                            collectionFee = this.FeeEquation(arrears, paymentAmount, collectionPercent, collectionRates);
                            this.CountryRound(ref collectionFee);
                        }

                        // When zero payment supplied return full payment (but note fee assumes Part payment as in OpenROAD)
                        if (paymentAmount == 0)
                            paymentAmount = (debitAccount == 1) ? arrears + collectionFee : arrears;

                        this.CountryRound(ref paymentAmount);
                    }
                }
            }

            //
            // Commission paid to bailiff CR680, CR377, CR158
            //
            if (this.CanPayCommission(conn, trans, allocatedCourtsPerson, accountNo))
            {
                decimal totalForCommission = ((arrears + collectionFee) < paymentAmount ? arrears + collectionFee : paymentAmount);
                if (paymentType != TransType.Repossession
                    && (bool)Country[CountryParameterNames.BailiffCommissionEqualsFee])
                {
                    // The Commission is the same as the fee being collected
                    bailiffFee = collectionFee;
                }
                else if ((bool)Country[CountryParameterNames.TotalPaymentCommission])
                {
                    // Commission based on the total amount paid
                    // Calculation from CR158
                    bailiffFee = totalForCommission / 100 * commissionPercent;
                }
                else
                {
                    // Commission based on the total amount paid less the fee
                    // Calculation from CR158
                    bailiffFee = totalForCommission - ((totalForCommission * 100) / (100 + commissionPercent));
                }

                if (collectionRates != null)
                {
                    // Ensure that the bailiff commission is within the min and max values
                    decimal minValue = Convert.ToDecimal(collectionRates[CN.MinValue]);
                    decimal maxValue = Convert.ToDecimal(collectionRates[CN.MaxValue]);
                    bailiffFee = bailiffFee < minValue ? minValue : bailiffFee;
                    bailiffFee = bailiffFee > maxValue ? maxValue : bailiffFee;
                }
                this.CountryRound(ref bailiffFee);
            }
        }


        private bool CanPayCommission(SqlConnection conn, SqlTransaction trans,
            int allocatedCourtsPerson, string acctNo)
        {
            bool canPayCommission = false;
            if (allocatedCourtsPerson != 0)
            {
                // Only certain employee types have commission paid CR680, CR377, CR158
                DEmployee emp = new DEmployee();
                emp.GetEmployeeDetails(conn, trans, allocatedCourtsPerson);

                if (emp.commissionType == ECT.CommissionAlways)
                    canPayCommission = true;
                else if (emp.commissionType == ECT.CommissionOnAction)
                {
                    // The follow up action codes can specify a number of days
                    // since the action during which commission will be paid.
                    // The number of days is held in the reference column of the
                    // action type.
                    DBailAction ba = new DBailAction();
                    ba.GetBailActions(acctNo);
                    foreach (DataRow row in ba.BailActions.Rows)
                    {
                        short commissionDays = Convert.ToInt16(row[CN.CommissionDays]);
                        DateTime dateAdded = (DateTime)row[CN.DateAdded];
                        if (commissionDays > 0 && dateAdded.AddDays(commissionDays) >= DateTime.Today)
                            canPayCommission = true;
                    }
                }
            }
            return canPayCommission;
        }


        /// <summary>
        /// RD & DR 25/11/04 
        /// TMCalculateCreditFee to calculate a Tallyman Bailiff Fee
        /// </summary>
        public int TMIsinFeeStrategy(string accountNo)
        {
            DAccount acct = new DAccount();
            acct.GetSegments(accountNo, (string)Country[CountryParameterNames.TallymanServerDB]);
            int chargefee = 0;
            foreach (DataRow segmentRow in acct.Segments.Rows)
            {
                // Found a row on Tallyman -- if reference >0 then charge fee only numeric references brought back
                chargefee = System.Convert.ToInt32(segmentRow[CN.TMChargeFee]);

            }

            return chargefee;
        }

        /// <summary>
        /// RD & DR 25/11/04 
        /// TMCalculateCreditFee to calculate a Tallyman Bailiff Fee
        /// </summary>
        public decimal TMCalculateCreditFee(
            string accountNo,			//mandatory
            ref decimal paymentAmount,	//mandatory but if zero calcs WHOLE payment
            decimal arrears,			//mandatory
            out int segmentId)			//Tallyman segmentid returned
        {
            decimal creditFee = 0;
            segmentId = 0;
            DAccount acct = new DAccount();
            acct.GetSegments(accountNo, (string)Country[CountryParameterNames.TallymanServerDB]);
            foreach (DataRow segmentRow in acct.Segments.Rows)
            {
                // Found a row on Tallyman so must calculate the fee
                creditFee = this.FeeEquation(arrears, paymentAmount, System.Convert.ToDecimal(segmentRow[CN.TMPercentage]), null);
                segmentId = (int)segmentRow[CN.SegmentID];
            }
            if (paymentAmount == 0) paymentAmount = arrears + creditFee;
            this.CountryRound(ref paymentAmount);
            return creditFee;
        }

        // FeeEquation to calculate inclusive or exclusive fee
        private decimal FeeEquation(decimal arrears, decimal paymentAmount, decimal collectionPercent, DataRow bailiffCommission)
        {
            decimal totalForFee = 0.0M;
            decimal creditFee = 0.0M;

            if ((bool)Country[CountryParameterNames.ArrearsOnPay])		//exclusive
            {
                arrears = arrears / 100 * (100 + collectionPercent);
                this.CountryRound(ref arrears);
                totalForFee = (paymentAmount > 0 && paymentAmount < arrears) ? paymentAmount : arrears;
                creditFee = (totalForFee / (100 + collectionPercent)) * collectionPercent;
            }
            else
            {
                totalForFee = (paymentAmount > 0 && paymentAmount < arrears) ? paymentAmount : arrears;
                creditFee = (totalForFee / 100) * collectionPercent;
            }

            if (bailiffCommission != null)
            {
                // Ensure that the credit fee is within the min and max values
                decimal minValue = Convert.ToDecimal(bailiffCommission[CN.MinValue]);
                decimal maxValue = Convert.ToDecimal(bailiffCommission[CN.MaxValue]);
                creditFee = creditFee < minValue ? minValue : creditFee;
                creditFee = creditFee > maxValue ? maxValue : creditFee;
            }

            this.CountryRound(ref creditFee);
            return creditFee;
        }

        // Need a different fee equation to agree with old OpenROAD code which used
        // a different equation when determining Whole/Part payment.
        private decimal HackFeeEquation(decimal arrears, decimal paymentAmount, decimal collectionPercent, DataRow bailiffCommission)
        {
            decimal creditFee = (arrears / 100) * collectionPercent;
            this.CountryRound(ref creditFee);
            return creditFee;
        }

        public DataTable GetMambuCLPaymentMethods(string acctNo)
        {
            DAccount da = new DAccount();
            DataTable dt = da.GetMambuCLPaymentMethods(acctNo);
            return dt;
        
        }

        public DataTable GetMambuHPPaymentMethods()
        {
            DAccount da = new DAccount();
            DataTable dt = da.GetMambuHPPaymentMethods();
            return dt;

        }

        public DataSet GetPaymentAccounts(SqlConnection conn, SqlTransaction trans,
            string customerID, string countryCode,
            bool lockAccounts, out decimal addToValue)
        {
            addToValue = 0;
            decimal delTot = 0;
            decimal collectionFee = 0;
            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();
            DAccount acct = new DAccount();
            DAgreement dagr = new DAgreement();             //#15117

            DataTable custDetails = cust.GetDetailsForPayment(conn, trans, customerID);
            if (custDetails.Rows.Count > 0)
            {
                DataRow row = custDetails.Rows[0];
                if ((string)row[CN.FirstName] == "SUNDRY CREDIT")
                    row[CN.SundryCredit] = true;
                else
                    row[CN.SundryCredit] = false;
            }

            DataTable accounts = cust.GetAccountsForPayment(conn, trans, customerID);
            PaymentWebApi api = new PaymentWebApi();
            foreach (DataRow row in accounts.Rows)
            {
                if (!DBNull.Value.Equals(row[CN.IsMambuAccount]) && (bool)row[CN.IsMambuAccount] == true)
                {
                    DataTable dt = api.GetMambuAccountsForPayment(null, row[CN.AccountNumber].ToString());
                    if (dt.Rows.Count > 0)
                    { 
                        string json = api.GetLoanSchedule(row[CN.AccountNumber].ToString());//(loanAccountId);
                        if (json != "Success")
                        {
                            throw new STLException(json);
                        }
                    }
                }
            }

            foreach (DataRow row in accounts.Rows)
            {
                try
                {
                    decimal rebate = 0;    // DSR Init here for each iteration
                    bool locked = false;
                    acct.AccountNumber = (string)row[CN.AccountNumber];
                    collectionFee = 0;
                    dagr.Populate(conn, trans, Convert.ToString(row[CN.AccountNumber]), Convert.ToInt32(row["agrmtno"]));         //#15117

                    /* Attempt to lock each account */
                    /* JJ Emergency fix - don't lock the accounts if lockAccounts is
                     * false - also don't do arrears calc or fee calculation. This is 
                     * because of deadlocking in Jamaica */
                    if (lockAccounts)
                    {
                        try
                        {
                            acct.Lock(conn, trans, acct.AccountNumber, this.User);
                            locked = true;
                        }
                        catch (Exception ex)	/* if unable to lock record the reason */
                        {
                            row[CN.LockedBy] = ex.Message;
                        }

                        if (locked)
                        {
                            dagr.EmployeeNumChange = this.User;
                            dagr.DateChange = DateTime.Now;                 //#15117
                            dagr.Save(conn, trans);                         //#15117

                            acct.CalcArrears(conn, trans, 0, 0);

                        }

                        row[CN.Arrears] = acct.Arrears;

                        // Calculate fee
                        if ((decimal)row[CN.Arrears] > 0)
                        {
                            decimal paymentAmount = 0.0M;
                            int debitAccount = 0;
                            int empeeNo = Convert.ToInt32(row[CN.EmployeeNo]);
                            int segmentId = 0;
                            decimal bailiffFee = 0;

                            this.CalculateCreditFee(
                                conn,
                                trans,
                                countryCode,
                                acct.AccountNumber,
                                ref paymentAmount, // Zero so will calc for Whole payment
                                TransType.Payment,
                                ref empeeNo,
                                acct.Arrears,
                                out collectionFee, // Charge to the account (on Payment screen)
                                out bailiffFee,    // Commission to the Bailiff
                                out debitAccount,
                                out segmentId);

                            row[CN.DebitAccount] = debitAccount;
                            row[CN.SegmentID] = segmentId;
                            row[CN.EmployeeNo] = empeeNo;
                            row[CN.CollectionFee] = collectionFee;
                            row[CN.CalculatedFee] = collectionFee;
                            row[CN.BailiffFee] = bailiffFee;
                        }
                    }

                    // Is account delivered ?
                    BDelivery del = new BDelivery();
                    // Instance of object DAccount to be created here so that only one object gets created in this routine
                    DAccount account = new DAccount(conn, trans, (string)row[CN.AccountNumber]);
                    if (del.AccountFullyDelivered(conn, trans, (string)row[CN.AccountNumber], account.AgreementTotal, out delTot) == 1 ||
                        del.PartialDeliveryCheck(conn, trans, (string)row[CN.AccountNumber], account.AgreementTotal, Convert.ToDouble(Country[CountryParameterNames.GlobalDeliveryPcent])) == 1)
                    {
                        row[CN.DeliveredIndicator] = 1;
                        if (AT.IsCreditType((string)row[CN.AccountType]) &&
                            (decimal)row[CN.OutstandingBalance] >= 0.01M)
                        {

                            if (Convert.ToBoolean(row[CN.IsMambuAccount]) == true)
                                acct.CalculateRebateForMambu(conn, trans, row[CN.acctno].ToString(), out rebate);
                            else
                                rebate = acct.CalculateRebate(conn, trans, (string)row[CN.AccountNumber]);

                            //To set rebate 0 for amortized cash loan accounts
                            //2019Feb23 - Fix issue for appling to rabate 0 to all account - It should only for amortized 
                            acct.GetAccount((string)row[CN.AccountNumber]);
                            if (acct.IsAmortized == true)
                            {
                                rebate = 0;
                            }
                            else
                            {
                                rebate = rebate < 0 ? 0 : rebate;
                            }
                            //decimal settlement = (decimal)row[CN.SettlementFigure]; // TODO - rebate + collectionFee;
                            decimal settlement = 0;
                            if (acct.IsAmortized == true)
                            {
                                DCustomer dCustomer = new DCustomer();
                                dCustomer.GetEarlySettlementFig(row[CN.acctno].ToString(), out settlement);
                            }
                            else
                            {
                                settlement = (decimal)row[CN.OutstandingBalance] - rebate + collectionFee;
                            }
                            settlement = settlement < 0 ? 0 : settlement;
                            row[CN.SettlementFigure] = settlement;
                            row[CN.Rebate] = rebate;
                        }
                    }
                    else
                        row[CN.DeliveredIndicator] = 0;
                  
                    if (Convert.ToBoolean(row[CN.IsMambuAccount]) == true)
                    {
                        decimal settlementfig = 0;
                        DCustomer dCustomer = new DCustomer();
                        dCustomer.GetEarlySettlementFigForMambu(row[CN.acctno].ToString(), out settlementfig);
                        row[CN.SettlementFigure] = settlementfig;
                    }



                    // Set to follow amount
                    if (!DBNull.Value.Equals(row[CN.AgreementTotal]))
                        row[CN.ToFollowAmount] = (decimal)row[CN.AgreementTotal] - delTot;
                    else
                        row[CN.ToFollowAmount] = 0;

                    if ((string)row[CN.AccountType] != AT.ReadyFinance)
                    {
                        //TO DO call the DBAddToCalc stuff
                        addToValue += acct.AddToCalculation(conn, trans, (string)row[CN.AccountNumber], rebate);
                    }

                    // Set the Free Instalment flag
                    row[CN.FreeInstalment] = this.FreeInstalmentAvailable(conn, trans, (string)row[CN.AccountNumber]);

                    // Update account status
                    if (((string)row[CN.Status] == "O" ||
                        (string)row[CN.Status] == "0" ||
                        (string)row[CN.Status] == "U") &&
                        locked)
                        acct.UpdateStatus(conn, trans, (string)row[CN.AccountNumber]);
                }
                finally
                {
                    /* if called from AccountDetails we don't want to leave the accounts
                     * locked */
                    /* this is no longer necessary because our emergency fix means 
                     * the accounts won't be locked
                    if(!lockAccounts)
                        acct.Unlock(conn, trans, acct.AccountNumber, this.User);
                    */
                }
            }

            ds.Tables.AddRange(new DataTable[] { custDetails, accounts });
            return ds;
        }

        public void GetAccountSettlement(SqlConnection conn, SqlTransaction trans,
            string countryCode, string accountNo,
            out decimal settlement, out decimal rebate, out decimal collectionFee)
        {
            DAccount acct = new DAccount(conn, trans, accountNo);
            settlement = 0;
            rebate = 0;
            collectionFee = 0;
            decimal delTot = 0;

            // Check whether a fee is due
            if (AT.IsCreditType(acct.AccountType) && acct.OutstandingBalance >= 0.01M)
            {
                decimal paymentAmount = 0;
                int empeeNo = 0;
                decimal bailiffFee = 0;
                int debitAccount = 0;
                int segmentId = 0;

                this.CalculateCreditFee(
                    conn,
                    trans,
                    countryCode,
                    accountNo,
                    ref paymentAmount, // Zero so will calc for Whole payment
                    TransType.Payment,
                    ref empeeNo,
                    acct.Arrears,
                    out collectionFee, // Charge to the account (on Payment screen)
                    out bailiffFee,    // Commission to the Bailiff
                    out debitAccount,
                    out segmentId);
            }

            // Rebate is only paid on delivered accounts
            BDelivery del = new BDelivery();
            // Instance of object DAccount to be created here so that only one object gets created in this routine
            DAccount account = new DAccount(conn, trans, accountNo);
            if (del.AccountFullyDelivered(conn, trans, accountNo, account.AgreementTotal, out delTot) == 1 ||
                del.PartialDeliveryCheck(conn, trans, accountNo, account.AgreementTotal, Convert.ToDouble(Country[CountryParameterNames.GlobalDeliveryPcent])) == 1)
            {
                rebate = acct.CalculateRebate(conn, trans, accountNo);
                rebate = rebate < 0 ? 0 : rebate;
            }

            // Return the settlement amount
            // This may just be the outstanding balance if there is no fee or rebate due
            settlement = acct.OutstandingBalance - rebate + collectionFee;
            settlement = settlement < 0 ? 0 : settlement;
        }

        // Returns a DataSet with a DataView which filters out lineitems which are not required
        public DataSet CheckDeliveryDate(string accountNo, string countryCode, out DateTime chequeClearance)
        {
            chequeClearance = DateTime.Today.AddDays(Convert.ToInt32(Country[CountryParameterNames.ChequeDays]));

            DLineItem item = new DLineItem();
            item.AccountNumber = accountNo;
            item.GetUndeliveredItemsForAccount();
            DataSet deliverySet = new DataSet();
            deliverySet.Tables.Add(item.ItemDetails);

            //Setting row filter in PL layer as setting it here made no difference.
            /*string rowFilter =CN.Price + " > 0 and " + 
                                CN.DateReqDel + " <= '" + chequeClearance.ToShortDateString() + "' and " +
                                //CN.DateReqDel + " <> '1/1/1900' and " +
                                CN.DateReqDel + " not = '" + DateTime.MinValue.AddYears(1899).ToShortDateString() + "' and " +
                                CN.Qtydiff + " = 'N'";
            deliverySet.Tables[0].DefaultView.RowFilter = rowFilter;*/

            return deliverySet;
        }

        public DataSet SavePayment(SqlConnection conn, SqlTransaction trans,
            string accountNo, bool sundryCredit,
            short paymentMethod, string chequeNo,
            string bankCode, string bankAcctNo,
            short branchNo, DataSet payments,
            decimal localTender,
            decimal localChange,
            out int commissionRef, out int paymentRef,
            out int rebateRef, out decimal rebateSum,
            int user, int authorisedBy,
            DateTime chequeClearance,
            int receiptNo, string countryCode,
            string voucherReference,
            bool courtsVoucher,
            int voucherAuthorisedBy,
            string acctNoCompany,
            int returnedChequeAuthorisedBy, int agrmtno,        // CR 543 Added Peter Chong [26-Sep-2006]
            string storeCardAcctno,                             //IP - 10/01/11 - Store Card
            long? storeCardNo,                                //IP - 10/01/11 - Store Card
            out int id
            )
        {
            int fintransId = 0;
            decimal collectionFee = 0;
            decimal bailiffFee = 0;
            decimal payment = 0;
            decimal rebate = 0;
            decimal settlement = 0;
            decimal paymentTotal = 0;
            decimal calculatedFee = 0;
            decimal arrears = 0;
            decimal badDebtBalance = 0;
            decimal badDebtCharges = 0;
            int transRefNo = 0;
            int required = 0;
            int[] transRef;
            commissionRef = 0;
            paymentRef = 0;
            rebateRef = 0;
            rebateSum = 0;
            int refIndex = 0;
            DateTime dateTrans = DateTime.Now;
            DBranch branch = new DBranch();
            string transAcctNo = "";
            int allocatedCourtsPerson;
            bool debitAccount = false;
            string exchangeAccountNo = "";
            int exchangeTransRefNo = 0;
            id = 0;
            DateTime dateAcctOpen = DateTime.Now;
            // List of transactions for receipt slip printing
            //66669 Add a column to indicatet whether or not the data has been committed to the database
            DataSet transactionSet = new DataSet();
            transactionSet.Tables.Add(TN.Transactions);
            transactionSet.Tables[TN.Transactions].Columns.AddRange(
                new DataColumn[] {  new DataColumn(CN.AcctNo),
                                     new DataColumn(CN.DateTrans,           Type.GetType("System.DateTime")),
                                     new DataColumn(CN.TransTypeCode),
                                     new DataColumn(CN.TransRefNo,          Type.GetType("System.Int32")),
                                     new DataColumn(CN.TransValue,          Type.GetType("System.Decimal")),
                                     new DataColumn(CN.TransPrinted),
                                     new DataColumn(CN.Committed),
                                     new DataColumn(CN.AgreementTotal,      typeof(decimal)),
                                     new DataColumn(CN.DateAcctOpen,    Type.GetType("System.DateTime"))
                                 });
            
            // Loop through the payments
            foreach (DataTable dt in payments.Tables)
            {
                if (dt.TableName == TN.Payments)
                {
                    // Reserve the required number of ref nos
                    required = dt.Rows.Count * 3;
                    transRefNo = branch.GetTransRefNos(conn, trans, branchNo, required);
                    transRef = new int[required];

                    for (int i = 0; i < required; i++)
                        transRef[i] = transRefNo--;

                    foreach (DataRow row in dt.Rows)
                    {
                        payment = (decimal)row[CN.Payment];
                        rebate = (decimal)row[CN.Rebate];
                        settlement = (decimal)row[CN.SettlementFigure];
                        transAcctNo = (string)row[CN.AccountNo];
                        arrears = (decimal)row[CN.Arrears];
                        allocatedCourtsPerson = (int)row[CN.EmployeeNo];
                        collectionFee = (decimal)row[CN.CollectionFee];
                        bailiffFee = (decimal)row[CN.BailiffFee];
                        calculatedFee = (decimal)row[CN.CalculatedFee];
                        debitAccount = Convert.ToBoolean(row[CN.DebitAccount]);
                        if (!(row[CN.BDWBalance] == DBNull.Value))
                            badDebtBalance = (decimal)row[CN.BDWBalance];
                        else
                            badDebtBalance = 0;
                        if (!(row[CN.BDWCharges] == DBNull.Value))
                            badDebtCharges = (decimal)row[CN.BDWCharges];
                        else
                            badDebtCharges = 0;
                        dateAcctOpen = (DateTime)row[CN.DateAcctOpen];
                        // Add a PAYMENT transaction
                        // Payment transaction must be processed first because the
                        // foreign exchange record must use the same transrefno
                        if (payment >= 0)
                        {
                            paymentTotal += payment;
                            paymentRef = transRef[refIndex++];
                            exchangeAccountNo = transAcctNo;
                            exchangeTransRefNo = paymentRef;
                            fintransId = 0;
                            SavePaymentTransaction(conn, trans, paymentRef, dateTrans,
                                transAcctNo, payment, paymentMethod,
                                chequeNo, bankCode, bankAcctNo, user,
                                branchNo, sundryCredit, countryCode,
                                voucherReference, courtsVoucher,
                                voucherAuthorisedBy, acctNoCompany,
                                badDebtBalance, badDebtCharges, agrmtno, ref transactionSet,
                                storeCardAcctno, storeCardNo, localChange, localTender, ref fintransId,dateAcctOpen);

                            id = fintransId;

                            //IP - 10/01/11 - Store Card - added store card parameters
                            if (payment > 0)
                                DeductAmountFromPaymentHistory(conn, trans, transAcctNo, (payment - collectionFee)); // Added by RD, for CLA Outstanding Balance Calculation
                        }
                        else
                            if (payment < 0)
                            throw new STLException(GetResource("M_PAYMENTNEGATIVE"));

                        // Add a FEE transaction
                        commissionRef = 0;
                        if (collectionFee > 0 && debitAccount)
                        {
                            commissionRef = transRef[refIndex++];
                            SaveFeeTransaction(conn, trans, commissionRef, dateTrans,
                                transAcctNo, collectionFee, paymentMethod,
                                chequeNo, bankCode, bankAcctNo, user,
                                branchNo, countryCode, ref transactionSet, dateAcctOpen);
                        }
                        else
                            if (collectionFee < 0)
                            throw new STLException(GetResource("M_COMMISSIONNEGATIVE"));

                        // The user may have authorised a different (or zero) fee
                        if (collectionFee != calculatedFee && debitAccount)
                        {
                            // Add a Transaction Audit record
                            // 'commissionRef' intentionally zero if no FEE transaction saved above
                            DTransactionAudit ta = new DTransactionAudit();
                            ta.Write(conn, trans, transAcctNo, user, authorisedBy, collectionFee, calculatedFee, dateTrans, commissionRef, TransType.CreditFee);
                        }

                        // Add a REBATE transaction
                        if (rebate > 0 &&
                            payment >= settlement &&
                            settlement > 0)
                        {
                            rebateRef = transRef[refIndex++];
                            SaveRebateTransaction(conn, trans, rebateRef, dateTrans,
                                transAcctNo, rebate, paymentMethod,
                                chequeNo, bankCode, bankAcctNo, user,
                                branchNo, countryCode, ref transactionSet, dateAcctOpen);
                            rebateSum += rebate;
                        }
                        else
                        {
                            if (Day90Check(conn, trans, accountNo, out rebate))
                            {
                                rebateRef = transRef[refIndex++];
                                SaveRebateTransaction(conn, trans, rebateRef, dateTrans,
                                    transAcctNo, rebate, paymentMethod,
                                    chequeNo, bankCode, bankAcctNo, user,
                                    branchNo, countryCode, ref transactionSet, dateAcctOpen);
                                rebateSum += rebate;
                            }
                        }

                        // Store the COMMISSION
                        if (bailiffFee > 0)
                        {
                            DBailiffCommn bcm = new DBailiffCommn();
                            bcm.AllocatedCourtsPerson = allocatedCourtsPerson;
                            bcm.AccountNo = transAcctNo;
                            bcm.Status = "H";	/*Hold commission payment*/
                            bcm.DateTrans = dateTrans;	// Must match BailAction datetime
                            bcm.TransRefNo = paymentRef;
                            bcm.ChequeColln = bankAcctNo.Length == 0 ? "N" : "Y";
                            bcm.TransValue = bailiffFee;
                            bcm.Save(conn, trans);
                        }

                        if (allocatedCourtsPerson > 0)
                        {
                            DBailAction ba = new DBailAction();
                            ba.EmployeeNo = allocatedCourtsPerson;
                            ba.AccountNo = transAcctNo;
                            ba.Code = TransType.Payment;
                            ba.AmtCommPaidOn = Convert.ToDouble(payment);
                            if (arrears > 0 &&
                                Math.Abs(payment) > Math.Abs(arrears))
                                ba.ActionValue = Convert.ToDouble(Math.Abs(arrears));
                            else
                                ba.ActionValue = Convert.ToDouble(payment);
                            ba.DateAdded = dateTrans;    // Must match BailiffCommn datetime
                            ba.AddedBy = this.User;
                            ba.DateDue = dateTrans; //CR 684 - set Due Date to date of payment
                            ba.Save(conn, trans);
                        }
                    }
                    
                    if (PayMethod.IsPayMethod(paymentMethod, PayMethod.Cheque))	//cheque payment
                    {
                        DAgreement agree = new DAgreement(conn, trans, accountNo, 1);
                        agree.DepositChequeClears = chequeClearance;
                        agree.DateChange = DateTime.Now;                //#14392
                        agree.Save(conn, trans);

                        //CR 543 Add a record to the transaction audit table [Peter Chong] [26-Sep-2006]
                        if (returnedChequeAuthorisedBy != 0)
                        {
                            DTransactionAudit ta = new DTransactionAudit();
                            ta.Write(conn, trans, transAcctNo, user, returnedChequeAuthorisedBy, payment, 0, dateTrans, paymentRef, TransType.ReturnedChequeAuthorisation);
                        }
                    }

                   
                }
            }

            DFollUpAlloc foa = new DFollUpAlloc();
            foa.AutoDeallocate(conn, trans, accountNo);
            AccountRepository AccountR = new AccountRepository();
            if (AccountR.InstantCreditDACheck(accountNo, user, conn, trans)) //IP - 03/03/11 - #3255 - Added user
                new DAgreement(conn, trans, accountNo, 1) { User = Users.ICAutoDA }.ClearProposal(conn, trans, accountNo, "AUTO");
            PayTempReceipt(conn, trans, receiptNo, accountNo, paymentTotal);

            return transactionSet;
        }

        private void SaveFeeTransaction(SqlConnection conn, SqlTransaction trans,
            int transRefNo, DateTime transDate,
            string accountNo, decimal transValue,
            short paymentMethod, string chequeNo,
            string bankCode, string bankAcctNo, int user,
            short branchNo, string countryCode, ref DataSet transactionSet, DateTime dateAcctOpen)
        {
            if (transValue != 0)
            {
                if (transValue > 0)
                {
                    DAccount acct = new DAccount(conn, trans, accountNo);               //IP - 29/06/11 - 5.13 - LW73743 - #4134

                    BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
                        transRefNo, transValue, user,
                        TransType.CreditFee, bankCode,
                        bankAcctNo, chequeNo, paymentMethod, countryCode, transDate, "", 0);

                    transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { accountNo, transDate, TransType.CreditFee, transRefNo, transValue, 'N', "", acct.AgreementTotal, dateAcctOpen }); //IP - 29/06/11 - 5.13 - LW73743 - #4134
                }
                else
                    throw new STLException(GetResource("M_COMMISSIONNEGATIVE"));
            }
        }

        private void SavePaymentTransaction(SqlConnection conn, SqlTransaction trans,
            int transRefNo, DateTime transDate,
            string accountNo, decimal transValue,
            short paymentMethod, string chequeNo,
            string bankCode, string bankAcctNo, int user,
            short branchNo, bool sundryCredit,
            string countryCode, string voucherReference,
            bool courtsVoucher, int voucherAuthorisedBy,
            string accountNoCompany, decimal badDebtBalance,
            decimal badDebtCharges, int agrmtno, ref DataSet transactionSet,
            string storeCardAcctno, long? storeCardNo, decimal localChange, decimal localTender, ref int fintransId,DateTime dateAcctOpen
           )
        {
            string transType = TransType.Payment;		/* default */


            if (transValue != 0 || PayMethod.IsForeignCurrencyPayMethod(paymentMethod))
            {
                if (transValue > 0)
                {
                    transValue *= -1;

                    string toAccountNo = accountNo;

                    ///if this is a bad debt writeoff payment 
                    DAccount acct = new DAccount(conn, trans, accountNo);
                    if (acct.CurrentStatus == "S" &&
                        (Math.Abs(badDebtBalance) >= 0.01M ||
                        Math.Abs(badDebtCharges) >= 0.01M))
                    {
                        ///retrieve the bdw account for this branch
                        string bdwAccountNo = acct.GetBadDebtWriteOffAccount(conn, trans, acct.Securitised, Convert.ToInt16(accountNo.Substring(0, 3)));
                        if (bdwAccountNo.Length == 0)
                        {
                            throw new STLException(GetResource("M_NOBDWACCOUNT", new object[] { accountNo.Substring(0, 3) }));
                        }

                        ///update the bdw balance and charges
                        acct.BDWBalance += transValue;
                        if (acct.BDWBalance < 0)
                        {
                            acct.BDWCharges += acct.BDWBalance;
                            acct.BDWBalance = 0;
                            if (acct.BDWCharges < 0)
                                acct.BDWCharges = 0;
                        }
                        acct.Save(conn, trans);

                        transType = TransType.DebtPayment;
                        toAccountNo = bdwAccountNo;
                    }


                    /* if payment is by gift voucher do a transfer not a payment transaction */
                    if (paymentMethod == PayMethod.GiftVoucher || paymentMethod == PayMethod.BSGiftVoucher || paymentMethod == PayMethod.BSGiftVoucherBNS
                        || paymentMethod == PayMethod.UNGiftVoucherNCB)
                    {
                        int refNoRedeemed = 0;
                        DGiftVoucher gv = new DGiftVoucher();
                        gv.Populate(conn, trans, voucherReference, courtsVoucher, accountNoCompany);
                        string voucherAcct = courtsVoucher ? gv.AccountNoSold : accountNoCompany;

                        DAccount vAcct = new DAccount();
                        vAcct.Validate(conn, trans, voucherAcct);
                        if (Convert.ToBoolean(vAcct.AccountExists))
                        {
                            decimal giftValue = 0;
                            if (acct.AccountType == AT.ReadyFinance)
                                giftValue = Math.Abs(transValue);
                            else
                                giftValue = gv.Value;

                            /* write a transfer record */
                            BTransaction t = new BTransaction();
                            t.User = user;
                            refNoRedeemed = t.TransferTransaction(conn, trans, voucherAcct, toAccountNo, TransType.Transfer, giftValue, branchNo, countryCode, transDate, "", 0, paymentMethod, agrmtno, null); //IP - 29/11/10 - Store Card - added agrmtno //IP - 30/11/10 - Store Card - Added StoreCardNumber
                            fintransId = t.FintransId;

                            DAccount tmpAcct = new DAccount(conn, trans, toAccountNo);
                            transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { toAccountNo, transDate, TransType.Transfer, refNoRedeemed, -giftValue, 'N', "", tmpAcct.AgreementTotal,dateAcctOpen });

                            if (gv.Free)		/* if it was a free gift voucher journal it off */
                            {
                                DBranch b = new DBranch();
                                int refNo = b.GetTransRefNo(conn, trans, branchNo);
                                t = new BTransaction(conn, trans, gv.AccountNoSold, branchNo, refNo, -giftValue, User, TransType.GiftVoucherJournal, "", "", "", 0, countryCode, transDate, "", 0);

                                tmpAcct = new DAccount(conn, trans, gv.AccountNoSold);
                                transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { gv.AccountNoSold, transDate, TransType.GiftVoucherJournal, refNo, -giftValue, 'N', tmpAcct.AgreementTotal,dateAcctOpen });
                            }
                        }
                        else
                        {
                            throw new STLException(GetResource("M_NOGIFTVOUCHERACCOUNT", new object[] { voucherAcct }));
                        }

                        /* update the gift voucher record to show it has been redeemed */
                        gv.Redeem(conn, trans, voucherReference, courtsVoucher, user, DateTime.Now, voucherAuthorisedBy, toAccountNo, accountNoCompany, refNoRedeemed);
                    }
                    else if (PayMethod.IsPayMethod(paymentMethod, PayMethod.StoreCard))      //IP - 10/01/11 - Store Card
                    {
                        /* write a transfer record */
                        int refNo = 0;
                        BTransaction t = new BTransaction();
                        t.User = user;
                        refNo = t.TransferTransaction(conn, trans, storeCardAcctno, toAccountNo, TransType.StoreCardPayment, Math.Abs(transValue), branchNo, countryCode, transDate, "", 0, PayMethod.StoreCard, agrmtno, storeCardNo); //IP - 29/11/10 - Store Card - added agrmtno //IP - 30/11/10 - Store Card - Added StoreCardNumber

                        DAccount tmpAcct = new DAccount(conn, trans, toAccountNo);

                        transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { toAccountNo, transDate, TransType.StoreCardPayment, refNo, -transValue, 'N', "", tmpAcct.AgreementTotal,dateAcctOpen });

                        if (acct.AccountType == AT.Cash)
                        {
                            var valueOfWOC = new DLineItem().GetWarrantiesOnCreditValue(conn, trans, accountNo);
                            var agree = new DAgreement(conn, trans, accountNo, 1);

                            bool autoda = false;

                            if ((acct.AgreementTotal - valueOfWOC) != 0)
                            {
                                if (((Math.Abs(transValue) / (acct.AgreementTotal - valueOfWOC)) * 100 >
                                    (decimal)Country[CountryParameterNames.CODPercentage]) &&
                                    agree.CODFlag == "Y")
                                    autoda = true;
                            }

                            if (Math.Abs(transValue) >= (acct.AgreementTotal - valueOfWOC) || autoda)
                            {
                                agree.User = User;

                                // Collect hold property value before clear proposal action.
                                var AcctR = new AccountRepository();
                                bool holdProp = AcctR.GetAccountData(accountNo, conn, trans);

                                agree.ClearProposal(conn, trans, accountNo, DASource.Auto);

                                // CR - Jyoti - Create Service Request for Installation items only after Payment is done
                                // instead of creating it on line item sale on windows PoS.
                                if (holdProp)
                                {
                                    BTransaction tran = new BTransaction();
                                    tran.ServiceRequestCreate(conn, trans, accountNo);
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
                                        new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["BookingID"]));        //#13829 - Update before GetBookingData 
                                    }

                                    var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);

                                    new Chub().SubmitMany(bookings, conn, trans);
                                }

                                BPayment affinityPayment = new BPayment();
                                affinityPayment.DeliverAffinity(conn, trans, accountNo);
                            }
                        }

                    }
                    else
                    {
                        ///write the transaction to the account (could be the bdw account)
                        BTransaction t = new BTransaction(conn, trans, toAccountNo, branchNo,
                            transRefNo, transValue, user,
                            transType, bankCode,
                            bankAcctNo, chequeNo, paymentMethod, countryCode, transDate, "", agrmtno);
                        fintransId = t.FintransId;

                        if (count == 0)
                        {
                            // Save an Exchange transaction if a foreign currency was taken.
                            // There must only be one exchange record even though there could be
                            // multiple accounts with payments in the loop above.
                            
                            if (PayMethod.IsForeignCurrencyPayMethod(paymentMethod))
                            {
                                this.SaveExchangeTransaction(conn, trans, toAccountNo, transRefNo, transDate, paymentMethod, localTender, localChange, branchNo);
                            }
                            count++;
                        }


                        if (transType == TransType.DebtPayment)
                        {
                            // Save a link on Fintrans_Account table
                            string fromAccountNo = accountNo;
                            DFintransAccount FALink = new DFintransAccount(toAccountNo,
                                fromAccountNo, transDate, branchNo, transRefNo);
                            FALink.SaveAccountLink(conn, trans);
                        }

                        DAccount tmpAcct = new DAccount(conn, trans, toAccountNo);
                        transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { toAccountNo, transDate, TransType.Payment, transRefNo, transValue, 'N', "", tmpAcct.AgreementTotal,dateAcctOpen });
                    }

                    if (sundryCredit)
                    {
                        //not implementing this functionality at this time
                    }
                }
                else
                    if (transValue < 0)
                    throw new STLException(GetResource("M_PAYMENTNEGATIVE"));
            }
        }

        private void SaveRebateTransaction(SqlConnection conn, SqlTransaction trans,
            int transRefNo, DateTime transDate,
            string accountNo, decimal transValue,
            short paymentMethod, string chequeNo,
            string bankCode, string bankAcctNo, int user,
            short branchNo, string countryCode, ref DataSet transactionSet,DateTime dateAcctOpen)
        {
            if (transValue != 0)
            {
                if (transValue > 0)
                {
                    transValue *= -1;
                    BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
                        transRefNo, transValue, user,
                        TransType.Rebate, bankCode,
                        bankAcctNo, chequeNo, paymentMethod, countryCode, transDate, "", 0);

                    transactionSet.Tables[TN.Transactions].Rows.Add(new Object[] { accountNo, transDate, TransType.Rebate, transRefNo, transValue, 'N',"", null , dateAcctOpen });
                }
                else
                    throw new STLException(GetResource("M_REBATENEGATIVE"));
            }
        }
        public bool SavePaymentDetails(SqlConnection conn, 
                                        SqlTransaction trans,
                                        decimal totalAmountReceived,
                                        decimal totalCash,
                                        decimal totalCheque,
                                        decimal cashChange,
                                        decimal chequeChange,
                                        decimal totalPaymentPaid,
                                        List<PaymentDetail> paymentDetails,
                                        string transactionType)
        {

            try
            {
                DBSPaymentDetails paymentDetail = new DBSPaymentDetails();
                paymentDetail.TotalAmountReceived = totalAmountReceived;
                paymentDetail.TotalCash = totalCash;
                paymentDetail.TotalCheque = totalCheque;
                paymentDetail.CashChange = cashChange;
                paymentDetail.ChequeChange = chequeChange;
                paymentDetail.TotalPaymentPaid = totalPaymentPaid;
                paymentDetail.PaymentDetails = paymentDetails;
                paymentDetail.Save(conn, trans, transactionType);
                return true;
            }
            catch
            {
                return false;
            }
        }        

        private bool Day90Check(SqlConnection conn, SqlTransaction trans,
            string accountNo, out decimal rebate)
        {
            bool status = false;
            rebate = 0;

            DAccount acct = new DAccount();
            status = acct.Day90Check(conn, trans, accountNo, out rebate);

            return status;
        }

        private void PayTempReceipt(SqlConnection conn, SqlTransaction trans,
            int receiptNo, string accountNo, decimal paymentAmount)
        {
            if (receiptNo != 0)
            {
                DTempReceipt receipt = new DTempReceipt();
                receipt.PayTempReceipt(conn, trans, accountNo, paymentAmount, receiptNo, DateTime.Today);
            }
        }

        public DataSet GetTempReceipt(int receiptNo)
        {
            DataSet ds = new DataSet();
            DTempReceipt tr = new DTempReceipt();
            ds.Tables.Add(tr.GetTempReceipt(receiptNo));
            return ds;
        }

        public void DeliverAffinity(SqlConnection conn, SqlTransaction trans, string accountNo, bool readyAssist = false)  //#18603
        {
            short branchNo = System.Convert.ToInt16(accountNo.Substring(0, 3));
            string affinity = "N";
            bool status = true;
            DAgreement agree = null;
            DAccount acct = null;
            DTermsType terms = null;
            //DLineItem line = null;
            BDelivery nonStockDelivery = new BDelivery();
            decimal transTotal = 0;
            //bool updateFlag = false;
            int stockCount = 0;
            int affinityCount = 0;
            DLineItem lineItem = null;
            DBranch branch = new DBranch();
            //int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

            //make sure the terms type for this account is Affinity
            acct = new DAccount(conn, trans, accountNo);
            acct.User = User;
            if (!(bool)Country[CountryParameterNames.AffinityStockSales] && !readyAssist)  //#18603 - bypass if Ready Assist
            {
                terms = new DTermsType();
                terms.GetTermsTypeDetail(conn, trans, Country[CountryParameterNames.CountryCode].ToString(), acct.TermsType, acct.AccountNumber, "", acct.DateAccountOpen);
                foreach (DataRow r in terms.TermsTypeDetails.Rows)
                    affinity = (string)r[CN.Affinity];

                if (affinity != "Y")
                    status = false;
            }
            else
            {
                lineItem = new DLineItem();
                lineItem.AccountNumber = accountNo;
                lineItem.CheckForStockItems(conn, trans, out stockCount, out affinityCount);

                if ((affinityCount == 0 || stockCount > 0) && !readyAssist) //#18603 - bypass if Ready Assist
                    status = false;
            }
            if (status)		//do not deliver if not authorised
            {
                agree = new DAgreement(conn, trans, accountNo, 1);
                if (agree.HoldProp == "Y")
                    status = false;
            }

            if (status)
            {
                int transRefNo = branch.GetTransRefNo(conn, trans, branchNo); //72891 Free gifts on cash loans causing process to lock itself out. Include branch update within same transaction.

                //Deliver the line items
                nonStockDelivery.User = User; //IP - 30/11/09 - UAT5.2 (511)
                nonStockDelivery.UseExistingTransaction = true;
                nonStockDelivery.DeliverNonStocks(conn, trans, accountNo, acct.AccountType,
                   Country[CountryParameterNames.CountryCode].ToString(), branchNo, transRefNo, ref transTotal, agree.AgreementNumber);

                if (transTotal != 0)
                {
                    //write the fintrans record
                    BTransaction t =
                        new BTransaction(conn, trans, accountNo, branchNo, transRefNo,
                        transTotal, User, TransType.Delivery, "", "", "", 0,
                        Country[CountryParameterNames.CountryCode].ToString(), DateTime.Now, FootNote.ImmediateDelivery, agree.AgreementNumber);
                }

                PaymentWebApi apis = new PaymentWebApi();
                DataTable data = apis.GetMambuAccountsForPayment(null, accountNo);

                if (data == null || data.Rows == null || data.Rows.Count == 0)
                {
                    //check the order total does not exceed the agreement total
                    if (transTotal > acct.AgreementTotal)
                        throw new STLException(GetResource("M_DELEXCEEDSAGREEMENT", new object[] { accountNo }));
                }

                agree.DeliveryFlag = "Y";
                agree.DateChange = DateTime.Now;                //#14392
                agree.Save(conn, trans);

                //#19148 - CR15594
                if (stockCount == 0 && affinityCount >= 1)
                {
                    acct.AccountNumber = accountNo;
                    acct.CalcArrears(conn, trans, 0, 0);
                }

            }
        }


        // cr906 rdb 19/09/07 Adapted from DeliverAffinity
        public void DeliverCashLoan(SqlConnection conn, SqlTransaction trans, ref Blue.Cosacs.Shared.CashLoanDetails det, Blue.Cosacs.Shared.CashLoanDisbursementDetails cashLoanDisbursementDet)
        {

            short branchNo = System.Convert.ToInt16(det.accountNo.Substring(0, 3));
            bool status = false;
            DAgreement agree = null;
            DAccount acct = null;
            DTermsType terms = null;
            BDelivery nonStockDelivery = new BDelivery();
            decimal transTotal = 0;
            decimal transTotalCashLoan = 0;
            DBranch branch = new DBranch();
            DInstalPlan inst = new DInstalPlan();


            //make sure the terms type for this account is Affinity
            acct = new DAccount(conn, trans, det.accountNo);
            acct.User = this.User;
            acct.AccountNumber = det.accountNo;

            terms = new DTermsType();
            terms.GetTermsTypeDetail(conn, trans, Country[CountryParameterNames.CountryCode].ToString(), acct.TermsType, acct.AccountNumber, "", acct.DateAccountOpen);
            foreach (DataRow r in terms.TermsTypeDetails.Rows)
                status = (bool)r[CN.IsLoan];

            if (status)		//do not deliver if not authorised
            {
                agree = new DAgreement(conn, trans, det.accountNo, 1);
                if (agree.HoldProp == "Y")
                    status = false;
            }

            if (status)
            {
                int transRefNo = branch.GetTransRefNo(conn, trans, branchNo); //72891 Free gifts on cash loans causing process to lock itself out. Include branch update within same transaction.

                //Deliver the line items
                nonStockDelivery.User = User; //IP - 30/11/09 - UAT5.2 (511)
                nonStockDelivery.UseExistingTransaction = true;
                //Firstly deliver all non stocks on the account apart from the Loan Item

                //Do not deliver Admin, DT, and etc for cash loan amoritized account
                if (!IsCashLoanAmortizedAccount(conn, trans, det.accountNo))
                {
                    nonStockDelivery.DeliverNonStocks(conn, trans, det.accountNo, acct.AccountType,
                    Country[CountryParameterNames.CountryCode].ToString(), branchNo, transRefNo, ref transTotal, agree.AgreementNumber);
                }

                transRefNo = branch.GetTransRefNo(conn, trans, branchNo);         //#19561
                det.transrefno = transRefNo;                        //Set the Transaction no for the CLD as this will be required for receipt print

                //Now we can process the delivery for the Cash LOAN item.
                nonStockDelivery.DeliverCashLoan(conn, trans, det.accountNo, acct.AccountType,
                    Country[CountryParameterNames.CountryCode].ToString(), branchNo, transRefNo, ref transTotalCashLoan, agree.AgreementNumber, cashLoanDisbursementDet);

                //For amozrtized cash loan disbursal
                if (!(bool)Country[CountryParameterNames.CL_Amortized])
                {
                    transTotal += transTotalCashLoan;
                }
                else
                {
                    goto X;
                }
                
                PaymentWebApi apis = new PaymentWebApi();
                DataTable data = apis.GetMambuAccountsForPayment(null, det.accountNo);

                if (data == null || data.Rows == null || data.Rows.Count == 0)
                {
                    //check the order total does not exceed the agreement total
                    if (transTotal > acct.AgreementTotal)
                            throw new STLException(GetResource("M_DELEXCEEDSAGREEMENT", new object[] { det.accountNo }));
                }

                X:
                acct.OutstandingBalance += transTotal;
                acct.Save(conn, trans);
                PaymentWebApi api = new PaymentWebApi();

                DataTable dt = api.GetMambuAccountsForPayment(null, det.accountNo);
                if (dt.Rows.Count > 0)
                {
                    agree.DateDel = DateTime.Now;
                }
                agree.DeliveryFlag = "Y";
                agree.DateChange = DateTime.Now;            //#14392
                agree.Save(conn, trans);

                acct.CalcArrears(conn, trans, 0, 0); //Update Instalplan.Datefirst

                acct.Populate(conn, trans, det.accountNo);
                det.outstBal = acct.OutstandingBalance;

                inst.Populate(conn, trans, det.accountNo, 1);
                det.firstInstalDate = inst.DateFirst;
            }
        }

        public BPayment()
        {

        }

        /// <summary>
        /// GetCashierTotals
        /// </summary>
        /// <param name="byBranch">int</param>
        /// <param name="branchno">int</param>
        /// <param name="employeeno">int</param>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetCashierTotals(short branchno, int employeeno, DateTime datefrom, DateTime dateto, bool listCheques, out decimal total)
        {
            total = 0;
            DataSet ds = new DataSet();
            DFinTrans da = new DFinTrans();

            /* if we are listing cheques - then datefrom needs to be set to the value of
             * dateto and dateto needs to be set to the end of that day. */
            if (listCheques)
            {
                datefrom = new DateTime(dateto.Year, dateto.Month, dateto.Day, 0, 0, 0);
                dateto = new DateTime(datefrom.Year, datefrom.Month, datefrom.Day, 23, 59, 59);
            }

            ds.Tables.Add(GetCashierDeposits(employeeno, 2, datefrom, dateto, branchno, "-1", false, -1));  //IP - 15/12/11 - #8810
            ds.Tables.Add(da.GetCashierTotals(employeeno == -1, branchno, employeeno, datefrom, dateto, listCheques, out total));

            return ds;
        }

        private decimal GetCashierDepositTotal(int employeeno, DateTime datefrom, DateTime dateto)
        {
            decimal deposits = 0;
            DCashierDeposit cd = new DCashierDeposit();
            DataTable dt = cd.Get(employeeno, 0, datefrom, dateto, 0, "-1", false, -1); //IP - 15/12/11 - #8810 - CR1234

            foreach (DataRow dr in dt.Rows)
                deposits += (decimal)dr[CN.DepositValue];

            return deposits;
        }

        /// <summary>
        /// GetDateLastAudit
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <param name="dateLast">DateTime</param>
        /// <returns>DateTime</returns>
        /// 
        public DateTime GetDateLastAudit(int empeeno)
        {
            DEmployee da = new DEmployee();
            return da.GetDateLastAudit(empeeno);
        }


        /// <summary>
        /// GetChequeDetails
        /// </summary>
        /// <param name="chequeno">string</param>
        /// <param name="bankcode">string</param>
        /// <param name="bankacctno">string</param>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetChequeDetails(string chequeno, string bankcode, string bankacctno, DateTime datefrom, DateTime dateto)
        {
            DataSet ds = new DataSet();
            DFinTrans da = new DFinTrans();
            ds = da.GetChequeDetails(chequeno, bankcode, bankacctno, datefrom, dateto);
            return ds;
        }

        public void ReverseCheque(SqlConnection conn, SqlTransaction trans,
            string acctno, string chequeno, string bankcode,
            string bankacctno, decimal transvalue, short payMethod,
            short branchno, string lTransType, DateTime lDateTrans,
            int lTransRefNo, string countryCode)
        {
            int refNo = 0;
            DBranch b = new DBranch();
            refNo = b.GetTransRefNo(conn, trans, branchno);
            DateTime dateTrans = DateTime.Now;

            BTransaction t = new BTransaction(conn, trans, acctno, branchno,
                refNo, transvalue, User, TransType.Return,
                bankcode, bankacctno, chequeno, payMethod,
                countryCode, dateTrans, "", 0);

            // Save an Exchange transaction if a foreign currency was taken
            if (PayMethod.IsForeignCurrencyPayMethod(payMethod))
            {
                this.SaveExchangeTransaction(conn, trans, acctno, refNo, dateTrans, payMethod, -transvalue, 0, branchno);
            }

            if (lTransType == TransType.DebtPayment && transvalue != 0) //To do check whether should do this for corrections/refunds on special account.
            {
                // Find the originating account for this DPY transaction
                DFintransAccount DFLink = new DFintransAccount();
                string toAccountNo = DFLink.GetLink(acctno, lDateTrans, branchno, lTransRefNo);
                DAccount acct = new DAccount();
                acct.Populate(conn, trans, toAccountNo);

                // Increase the BDW balance
                acct.BDWBalance += transvalue;
                acct.Save(conn, trans);

                // Save a link on Fintrans_Account table for the return
                string fromAccountNo = acctno;
                DFintransAccount FALink = new DFintransAccount(fromAccountNo,
                    toAccountNo, dateTrans, branchno, refNo);
                FALink.SaveAccountLink(conn, trans);
            }
        }

        /// <summary>
        /// SaveCashierTotal
        /// </summary>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <param name="empeeno">int</param>
        /// <param name="runno">int</param>
        /// <param name="empeenoauth">int</param>
        /// <param name="usertotal">double</param>
        /// <param name="systemtotal">double</param>
        /// <param name="difference">double</param>
        /// <param name="branchno">int</param>
        /// <param name="reason">string</param>
        /// <returns>int</returns>
        /// 
        public int SaveCashierTotal(SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto, int empeeno, int runno, int empeenoauth, bool canReverse, decimal usertotal, decimal systemtotal, decimal difftotal, decimal deposits, short branchno, string countryCode, DataSet breakdown)
        {
            int status = 0;
            DateTime dateTrans = DateTime.Now;
            DCashierTotal da = new DCashierTotal();
            DCashierTotalsBreakdown bd = new DCashierTotalsBreakdown();
            BAccount acct = new BAccount();
            BTransaction t = null;
            int identity = 0;

            status = da.Save(conn, trans, datefrom, dateto, empeeno, runno, empeenoauth, usertotal, systemtotal, difftotal, deposits, branchno, out identity);

            string overAcctNo = acct.GetOveragesAccount(conn, trans, branchno);
            if (overAcctNo.Length == 0)
            {
                // acctno not returned. If the customer didn't exist they
                // will have been created by the stored proc, therefore 
                // we just need to create an account and tie it to the cust
                overAcctNo = acct.NewCashierTotalAcct(conn, trans, countryCode, branchno, branchno, "OVERAGE" + branchno.ToString());
            }

            // CR695 awaiting sign off
            //if (canReverse)
            //{
            //	// CR695 Can reverse overage when equal to shortage
            //	string shortAcctNo = acct.GetReceivableAccount(conn, trans, empeeno);
            //	if(shortAcctNo.Length == 0)
            //	{
            //		/* acctno not returned. If the customer didn't exist they
            //		 * will have been created by the stored proc, therefore 
            //		 * we just need to create an account and tie it to the cust */
            //		shortAcctNo = acct.NewCashierTotalAcct(conn, trans, countryCode, branchno, empeeno, "SHORTAGE"+empeeno.ToString());
            //	}
            //
            //	t = new BTransaction(conn, trans, overAcctNo, branchno, 0, localDiff, User, TransType.Transfer, "", "", shortAcctNo, 0, countryCode, dateTrans, FootNote.Automatic);
            //	t = new BTransaction(conn, trans, shortAcctNo, branchno, 0, -localDiff, User, TransType.Transfer, "", "", overAcctNo, 0, countryCode, dateTrans, FootNote.Automatic);
            //}

            string shortAcctNo = acct.GetReceivableAccount(conn, trans, empeeno);
            if (shortAcctNo.Length == 0)
            {
                // acctno not returned. If the customer didn't exist they
                // will have been created by the stored proc, therefore 
                // we just need to create an account and tie it to the cust
                shortAcctNo = acct.NewCashierTotalAcct(conn, trans, countryCode, branchno, empeeno, "SHORTAGE" + empeeno.ToString());
            }

            /* save the breakdown of the cashier totals by pay method */
            foreach (DataTable dt in breakdown.Tables)
            {
                foreach (DataRow r in dt.Rows)
                {
                    /* don't bother saving the breakdown if all the values are zero */
                    if (Convert.ToDecimal(StripCurrency((string)r[CN.SystemValue])) != 0 ||
                        Convert.ToDecimal(StripCurrency((string)r[CN.UserValue])) != 0 ||
                        Convert.ToDecimal(StripCurrency((string)r[CN.Deposit])) != 0 ||
                        Convert.ToDecimal(StripCurrency((string)r[CN.Difference])) != 0)
                    {
                        decimal systemValue = Convert.ToDecimal(StripCurrency((string)r[CN.SystemValue]));
                        decimal userValue = Convert.ToDecimal(StripCurrency((string)r[CN.UserValue]));
                        decimal deposit = Convert.ToDecimal(StripCurrency((string)r[CN.Deposit]));
                        decimal difference = userValue - (systemValue - deposit);

                        bd.Save(conn, trans, identity, (string)r[CN.Code],
                            systemValue,
                            userValue,
                            deposit,
                            difference,
                            (string)r[CN.Reason],
                            Convert.ToDecimal(StripCurrency((string)r[CN.SecuritisedValue])));

                        if (difference != 0)
                        {
                            short payMethod = Convert.ToInt16(r[CN.Code]);
                            if (PayMethod.IsForeignCurrencyPayMethod(payMethod))
                            {
                                // Convert the difference to local currency
                                difference = CalcExchangeAmount(conn, trans, payMethod, 0, difference);
                            }

                            /* if there is an Overage (usertotal > systemtotal) */
                            if (difference > 0)
                            {
                                t = new BTransaction(conn, trans, overAcctNo, branchno, 0, -difference, User, TransType.Overage, "", "", "", payMethod, countryCode, dateTrans, FootNote.Automatic, 0);
                            }

                            /* if there is a shortage (usertotal < systemtotal) */
                            if (difference < 0)
                            {
                                t = new BTransaction(conn, trans, shortAcctNo, branchno, 0, -difference, User, TransType.Shortage, "", "", "", payMethod, countryCode, dateTrans, FootNote.Automatic, 0);
                            }
                        }
                    }
                }
            }
            return status;
        }

        ////Replaces partial SaveCashierTotal for creating shortage/overage accounts and posting shortage/overage
        public int SaveCashierTotalNew(SqlConnection conn, SqlTransaction trans, DateTime datefrom, DateTime dateto, int empeeno, int runno, int empeenoauth, bool canReverse, decimal usertotal, decimal systemtotal, decimal difftotal, decimal deposits, short branchno, string countryCode, List<CashierTotalsBreakdown> breakdown)
        {
            int status = 0;
            DateTime dateTrans = DateTime.Now;
            BAccount acct = new BAccount();
            BTransaction t = null;


            string overAcctNo = acct.GetOveragesAccount(conn, trans, branchno);
            if (overAcctNo.Length == 0)
            {
                // acctno not returned. If the customer didn't exist they
                // will have been created by the stored proc, therefore 
                // we just need to create an account and tie it to the cust
                overAcctNo = acct.NewCashierTotalAcct(conn, trans, countryCode, branchno, branchno, "OVERAGE" + branchno.ToString());
            }

            string shortAcctNo = acct.GetReceivableAccount(conn, trans, empeeno);
            if (shortAcctNo.Length == 0)
            {
                // acctno not returned. If the customer didn't exist they
                // will have been created by the stored proc, therefore 
                // we just need to create an account and tie it to the cust
                shortAcctNo = acct.NewCashierTotalAcct(conn, trans, countryCode, branchno, empeeno, "SHORTAGE" + empeeno.ToString());
            }


            foreach (var total in breakdown)
            {
                if (Convert.ToDecimal(total.systemtotal) != 0 ||
                    Convert.ToDecimal(total.usertotal) != 0 ||
                    Convert.ToDecimal(total.deposit) != 0 ||
                    Convert.ToDecimal(total.difference) != 0)
                {
                    decimal systemValue = Convert.ToDecimal(total.systemtotal);
                    decimal userValue = Convert.ToDecimal(total.usertotal);
                    decimal deposit = Convert.ToDecimal(total.deposit);
                    decimal difference = Convert.ToDecimal(total.difference);

                    if (difference != 0)
                    {
                        short payMethod = Convert.ToInt16(total.paymethod);
                        if (PayMethod.IsForeignCurrencyPayMethod(payMethod))
                        {
                            // Convert the difference to local currency
                            difference = CalcExchangeAmount(conn, trans, payMethod, 0, difference);
                        }

                        /* if there is an Overage (usertotal > systemtotal) */
                        if (difference > 0)
                        {
                            t = new BTransaction(conn, trans, overAcctNo, branchno, 0, -difference, User, TransType.Overage, "", "", "", payMethod, countryCode, dateTrans, FootNote.Automatic, 0, total.cashiertotalid); //IP/JC - 06/01/12 - #8821
                        }

                        /* if there is a shortage (usertotal < systemtotal) */
                        if (difference < 0)
                        {
                            t = new BTransaction(conn, trans, shortAcctNo, branchno, 0, -difference, User, TransType.Shortage, "", "", "", payMethod, countryCode, dateTrans, FootNote.Automatic, 0, total.cashiertotalid); //IP/JC - 06/01/12 - #8821
                        }
                    }
                }

            }


            return status;
        }

        /// <summary>
        /// GetCashierTotalsHistory
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetCashierTotalsHistory(int empeeno)
        {
            DataSet ds = new DataSet();
            DCashierTotal da = new DCashierTotal();
            ds.Tables.Add(da.GetHistory(empeeno));
            return ds;
        }

        public DataSet GetCashierTotalsReversal(int empeeno, out bool canReverse,
            out DateTime dateFrom, out DateTime dateTo,
            out decimal diffTotal, out decimal systemTotal,
            out decimal depositTotal, out decimal userTotal)
        {
            DataSet ds = new DataSet();
            DCashierTotal da = new DCashierTotal();
            DataTable reversal = da.GetReversal(empeeno);

            if (reversal.Rows.Count > 0)
            {
                // Set the date range
                dateFrom = (DateTime)(reversal.Rows[0][CN.DateFrom]);
                dateTo = (DateTime)(reversal.Rows[0][CN.DateTo]);

                // If the first row has a blank paymethod and zero values then
                // there is no breakdown, so don't include this row.
                if (reversal.Rows[0][CN.PayMethod].ToString() == "" &&
                    (decimal)(reversal.Rows[0][CN.Difference]) == 0 &&
                    (decimal)(reversal.Rows[0][CN.SystemTotal]) == 0 &&
                    (decimal)(reversal.Rows[0][CN.Deposit]) == 0 &&
                    (decimal)(reversal.Rows[0][CN.UserTotal]) == 0)
                {
                    reversal.Rows[0].Delete();
                    reversal.AcceptChanges();
                }

                // Can reverse only within last ten days (even if no breakdown was saved)
                canReverse = (dateTo > DateTime.Now.AddDays(-10));
            }
            else
            {
                // Nothing to reverse
                dateFrom = DateTime.Today;
                dateTo = DateTime.Today;
                canReverse = false;
            }

            // Sum the totals
            diffTotal = 0;
            systemTotal = 0;
            depositTotal = 0;
            userTotal = 0;
            foreach (DataRow row in reversal.Rows)
            {
                diffTotal += (decimal)row[CN.Difference];
                systemTotal += (decimal)row[CN.SystemTotal];
                depositTotal += (decimal)row[CN.Deposit];
                userTotal += (decimal)row[CN.UserTotal];
            }

            ds.Tables.Add(reversal);
            return ds;
        }

        public void SaveCashierTotalsReversal(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNo, int empeeNo)
        {
            decimal difference;
            string transType;
            string acctNo;
            BAccount acct = new BAccount();
            BTransaction t = null;
            DCashierTotal da = new DCashierTotal();
            da.SaveReversal(conn, trans, empeeNo, out difference);

            if (difference != 0)
            {
                if (difference > 0)
                {
                    // Post a reversal to the Branch Overage account
                    transType = TransType.Overage;
                    acctNo = acct.GetOveragesAccount(conn, trans, branchNo);
                    if (acctNo.Length == 0)
                        throw new STLException(GetResource("M_NOOVERAGESACCOUNT"));
                }
                else
                {
                    // Post a reversal to the employee Shortage account
                    transType = TransType.Shortage;
                    acctNo = acct.GetReceivableAccount(conn, trans, empeeNo);
                    if (acctNo.Length == 0)
                        throw new STLException(GetResource("M_NORECEIVABLEACCOUNT"));
                }
                t = new BTransaction(conn, trans, acctNo, branchNo, 0, -difference, User, transType, "", "", "", 0, countryCode, DateTime.Now, FootNote.Automatic, 0);
            }
        }


        public DataSet GetCashierTotalsBreakdown(int cashiertotalsid, bool print)
        {
            DataSet ds = new DataSet();
            DCashierTotalsBreakdown da = new DCashierTotalsBreakdown();
            ds.Tables.Add(da.Get(cashiertotalsid, print));
            return ds;
        }

        /// <summary>
        /// GetCashierDeposits
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <param name="postedtofact">int</param>
        /// <param name="cashiertotals">int</param>
        /// <param name="datefrom">DateTime</param>
        /// <param name="dateto">DateTime</param>
        /// <param name="branchNo">short</param>
        /// <returns>DataSet</returns>
        /// 
        public DataTable GetCashierDeposits(int empeeno, short postedtofact, DateTime datefrom, DateTime dateto, short branchNo, string depositType, bool branchFloats, int paymentMethod) //IP - 15/12/11 - #8810 - CR1234
        {
            DCashierDeposit da = new DCashierDeposit();
            return da.Get(empeeno, postedtofact, datefrom, dateto, branchNo, depositType, branchFloats, paymentMethod); //IP - 15/12/11 - #8810 - CR1234
        }

        /// <summary>
        /// SaveCashierDeposits
        /// There was an existing server side function to save cashier deposits which
        /// is called from the cashier disbursements screen. To use this function we
        /// must provide the deposits in the same format. This is why this method exists
        /// and why it seems to be doing some slightly strange re-working of the data.
        /// First we retrieve the current figures for this cashier again just to double
        /// check that they haven't changed since we loaded the screen. We then create a 
        /// new datatable of the format expected by the private save method.
        /// </summary>
        /// 
        public int SaveCashierDeposits(SqlConnection conn, SqlTransaction trans, short branchNo, int cashier, DataSet deposits)
        {
            DataView depositsView = deposits.Tables[TN.CashierOutstandingIncome].DefaultView;
            DataView refreshView = null;

            /* double check that we have the right figures */
            DataSet refresh = GetCashierOutstandingIncomeByPayMethod(cashier, branchNo);
            refreshView = refresh.Tables[TN.CashierOutstandingIncome].DefaultView;


            DataTable newTable = new DataTable(TN.CashierOutstandingIncome);
            newTable.Columns.AddRange(new DataColumn[] {    new DataColumn(CN.Value, Type.GetType("System.Decimal")),
                                                           new DataColumn(CN.Reference),
                                                           new DataColumn(CN.PayMethod),
                                                           new DataColumn(CN.DateDeposit, Type.GetType("System.DateTime")),
                                                           new DataColumn(CN.TransTypeCode),
                                                           new DataColumn(CN.Runno, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.EmployeeNo, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.EmployeeNoEntered, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.BranchNo, Type.GetType("System.Int16")),
                                                           new DataColumn(CN.IsCashierFloat, Type.GetType("System.Int16")),
                                                           new DataColumn(CN.IncludeInCashierTotals, Type.GetType("System.Int16"))});

            foreach (DataRow depositRow in depositsView.Table.Rows)
            {
                decimal totalDeposit = (decimal)depositRow[CN.BankedValue] + (decimal)depositRow[CN.SafeValue];
                decimal forDeposit = 0;

                refreshView.RowFilter = CN.Code + " = '" + ((string)depositRow[CN.PayMethod]).Trim() + "'";
                foreach (DataRowView refreshRow in refreshView)
                    forDeposit += (decimal)refreshRow[CN.ForDeposit];

                /* if the two values exceed the amount available for deposit
                 * we must reduce them both by the proportion by which forDeposit
                 * exceeds totalDeposit */
                if (forDeposit >= 0 && totalDeposit > forDeposit)
                {
                    decimal ratio = forDeposit / totalDeposit;
                    depositRow[CN.BankedValue] = (decimal)depositRow[CN.BankedValue] * ratio;
                    depositRow[CN.SafeValue] = (decimal)depositRow[CN.SafeValue] * ratio;
                }

                decimal bankDeposit = (decimal)depositRow[CN.BankedValue];
                decimal safeDeposit = (decimal)depositRow[CN.SafeValue];

                // BANK Deposits
                // The transaction type code is specific to branch and pay method 
                // and is set in the private version of SaveCashierDeposits.
                if (bankDeposit != 0)
                {
                    if (forDeposit >= 0 && bankDeposit > 0)
                    {
                        // A positive deposit
                        AddDepositRow(newTable, depositRow, bankDeposit, "", false);
                    }
                    else if (forDeposit >= 0 && bankDeposit < 0)
                    {
                        // A float deposit
                        AddDepositRow(newTable, depositRow, bankDeposit, "", true);
                    }
                    else if (forDeposit < 0 && bankDeposit >= forDeposit)
                    {
                        // A negative deposit
                        AddDepositRow(newTable, depositRow, bankDeposit, "", false);
                    }
                    else if (forDeposit < 0 && bankDeposit < forDeposit)
                    {
                        // A negative deposit and a float
                        AddDepositRow(newTable, depositRow, forDeposit, "", false);
                        AddDepositRow(newTable, depositRow, bankDeposit - forDeposit, "", true);
                    }
                }

                // SAFE Deposits
                // The transction type code is always TransType.Safe ('SAF')
                if (safeDeposit != 0)
                {
                    if (forDeposit >= 0 && safeDeposit > 0)
                    {
                        // A positive deposit
                        AddDepositRow(newTable, depositRow, safeDeposit, TransType.Safe, false);
                    }
                    else if (forDeposit >= 0 && safeDeposit < 0)
                    {
                        // A float deposit
                        AddDepositRow(newTable, depositRow, safeDeposit, TransType.Safe, true);
                    }
                    else if (forDeposit < 0 && bankDeposit + safeDeposit >= forDeposit)
                    {
                        // A negative deposit
                        AddDepositRow(newTable, depositRow, safeDeposit, TransType.Safe, false);
                    }
                    else if (forDeposit < 0 && bankDeposit + safeDeposit < forDeposit)
                    {
                        // A negative deposit and a float
                        decimal negSafeDeposit = 0;
                        if (bankDeposit > forDeposit)
                        {
                            // A negative safe deposit
                            negSafeDeposit = forDeposit - bankDeposit;
                            AddDepositRow(newTable, depositRow, negSafeDeposit, TransType.Safe, false);
                        }
                        // A float
                        AddDepositRow(newTable, depositRow, safeDeposit - negSafeDeposit, TransType.Safe, true);
                    }
                }
            }

            DataSet newDeposits = new DataSet();
            newDeposits.Tables.Add(newTable);
            return SaveCashierDeposits(conn, trans, newDeposits);
        }

        private void AddDepositRow(DataTable depositTable, DataRow depositRow, decimal amount, string transTypeCode, bool isFloat)
        {
            DataRow newRow = depositTable.NewRow();
            newRow[CN.Value] = amount;
            newRow[CN.TransTypeCode] = transTypeCode;
            newRow[CN.Reference] = depositRow[CN.Reference];
            newRow[CN.PayMethod] = depositRow[CN.PayMethod];
            newRow[CN.DateDeposit] = depositRow[CN.DateDeposit];
            newRow[CN.Runno] = depositRow[CN.Runno];
            newRow[CN.EmployeeNo] = depositRow[CN.EmployeeNo];
            newRow[CN.EmployeeNoEntered] = depositRow[CN.EmployeeNoEntered];
            newRow[CN.BranchNo] = depositRow[CN.BranchNo];
            // this deposit may cause the start of session if it is a float
            // in that case the cashier would not be totalled anymore and 
            // the float should be included in cashier totals
            if (isFloat)
            {
                newRow[CN.IncludeInCashierTotals] = 1;
                newRow[CN.IsCashierFloat] = 1;
            }
            else
            {
                newRow[CN.IncludeInCashierTotals] = depositRow[CN.IncludeInCashierTotals];
                newRow[CN.IsCashierFloat] = 0;
            }
            depositTable.Rows.Add(newRow);
        }

        public int SaveCashierDisbursements(SqlConnection conn, SqlTransaction trans, DataSet disbursements)
        {
            return SaveCashierDeposits(conn, trans, disbursements);
        }

        public bool IsDepositForRefund(SqlConnection conn, SqlTransaction trans, string accountNo,
            decimal amount, short payMethod, int fintransId)
        {
            DAccount d = new DAccount();
            return d.IsDepositForRefund(conn, trans, accountNo,amount, payMethod, fintransId);
        }

        public int  Paymentref(SqlConnection conn, SqlTransaction trans, string accountNo, decimal amount)
        {
           DAccount d = new DAccount();
           return  d.Paymentref(conn, trans, accountNo, amount);
        } 

        private int SaveCashierDeposits(SqlConnection conn, SqlTransaction trans, DataSet deposits)
        {
            int status = 0;
            DCashierDeposit da = new DCashierDeposit();
            DBranch branch = new DBranch();
            string depositType = "";

            foreach (DataTable dt in deposits.Tables)
            {
                /* there will only be one table */
                foreach (DataRow r in dt.Rows)
                {
                    if ((decimal)r[CN.Value] != 0)
                    {
                        if ((string)r[CN.TransTypeCode] == "")
                            // CR696 Set the deposit type for this branch and paymethod
                            depositType = branch.GetDepositType((short)r[CN.BranchNo], (string)r[CN.PayMethod]);
                        else
                            depositType = (string)r[CN.TransTypeCode];

                        da.Save(conn, trans,
                            (DateTime)r[CN.DateDeposit],
                            depositType,
                            (int)r[CN.Runno],
                            (int)r[CN.EmployeeNo],
                            (int)r[CN.EmployeeNoEntered],
                            0,
                            0,
                            "",
                            DateTime.MinValue.AddYears(1899),
                            (decimal)r[CN.Value],
                            (short)r[CN.BranchNo],
                            (string)r[CN.Reference],
                            (string)r[CN.PayMethod],
                            (short)r[CN.IncludeInCashierTotals],
                            (short)r[CN.IsCashierFloat]);

                        /* if this is a deposit from safe to bank it will be identified 
                         * by employeeno = -1. In this case we must do a float from 
                         * the safe to empeeno -1, then a deposit from empeeno -1 to the 
                         * bank */
                        if ((int)r[CN.EmployeeNo] == -1)
                        {
                            da.Save(conn, trans,
                                (DateTime)r[CN.DateDeposit],
                                TransType.Safe,
                                (int)r[CN.Runno],
                                (int)r[CN.EmployeeNo],
                                (int)r[CN.EmployeeNoEntered],
                                0,
                                0,
                                "",
                                DateTime.MinValue.AddYears(1899),
                                -((decimal)r[CN.Value]),
                                (short)r[CN.BranchNo],
                                "",
                                (string)r[CN.PayMethod],
                                (short)r[CN.IncludeInCashierTotals],
                                (short)r[CN.IsCashierFloat]);
                        }
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// VoidCashierDeposit
        /// </summary>
        /// <param name="depositid">int</param>
        /// <param name="datevoided">DateTime</param>
        /// <param name="voidedby">int</param>
        /// <returns>int</returns>
        /// 
        public int VoidCashierDeposit(int depositid, DateTime datevoided, int voidedby, bool reverse)
        {
            int status = 0;
            DCashierDeposit da = new DCashierDeposit();
            status = da.Void(depositid, datevoided, voidedby, reverse);

            return status;
        }

        /// <summary>
        /// GetAccountPayments
        /// </summary>
        /// <param name="acctno">string</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetAccountPayments(string acctno)
        {
            DataSet ds = new DataSet();
            DFinTrans da = new DFinTrans();
            ds = da.GetAccountPayments(acctno);
            return ds;
        }

        /// <summary>
        /// GetAmountPaid
        /// </summary>
        /// <param name="acctno">string</param>
        /// <param name="amount">double</param>
        /// <returns>decimal</returns>
        /// 
        public decimal GetAmountPaid(string acctno)
        {
            DFinTrans da = new DFinTrans();
            return da.GetAmountPaid(acctno);
        }

        public void SaveCorrection(SqlConnection conn, SqlTransaction trans,
            string accountNo, short branchNo,
            decimal amount, string transType, string bankCode,
            string bankAcctNo, string chequeNo, short payMethod,
            string countryCode, DateTime dateTrans,
            string footNote, int paymentRef,
            int authorisedBy, string CodeSource, out int refNo, out int fintransId)
        {
            fintransId = 0;
            CLAmortizationReversePayment(conn, trans, accountNo, amount); // Added by KM, for CLA Outstanding Balance Calculation Correction
            DCorrectedPayments cp = new DCorrectedPayments();
            DTransactionAudit ta = new DTransactionAudit();

            BBranch branch = new BBranch();
             refNo = branch.GetTransRefNo(conn, trans, branchNo);

            /* write the Correction transaction */
            BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
                refNo, amount, User, transType,
                bankCode, bankAcctNo, chequeNo,
                payMethod, countryCode, dateTrans,
                footNote, 0);
            fintransId = t.FintransId;
            t.FintransId = 0;
            // Save an Exchange transaction if a foreign currency was taken
            if (PayMethod.IsForeignCurrencyPayMethod(payMethod))
            {
                this.SaveExchangeTransaction(conn, trans, accountNo, refNo, dateTrans, payMethod, -amount, 0, branchNo);
            }

            /* write a record to the CorrectedPayments table so we know that 
             * this payment has been corrected */
            cp.Write(conn, trans, accountNo, paymentRef, refNo);

            /* if authorisation was required write a record to the transaction audit table */
            if (authorisedBy != 0)
                ta.Write(conn, trans, accountNo, User, authorisedBy, amount, amount, dateTrans, refNo, transType);
        }

        /// <summary>
        /// SaveBDWCorrection
        /// To reverse a BDW payment on the wrong account
        /// </summary>
        /// <param name="conn">SqlConnection</param>
        /// <param name="trans">SqlTransaction</param>
        /// <param name="bdwAccountNo">string</param>
        /// <param name="accountNo">string</param>
        /// <param name="branchNo">short</param>
        /// <param name="amount">decimal</param>
        /// <param name="countryCode">string</param>
        /// <returns>void</returns>
        /// 
        public void SaveBDWCorrection(
            SqlConnection conn,
            SqlTransaction trans,
            string bdwAccountNo,
            short branchNo,
            decimal amount,
            string countryCode,
            DateTime lDateTrans,
            short lBranchNo,
            int paymentRef,
            int authorisedBy)
        {
            DCorrectedPayments cp = new DCorrectedPayments();
            DTransactionAudit ta = new DTransactionAudit();
            BAccount acct = new BAccount();
            DBranch branch = new DBranch();
            int refNo = branch.GetTransRefNo(conn, trans, branchNo);
            DateTime dateTrans = DateTime.Now;

            // Find the originating account for this DPY transaction
            DFintransAccount DFLink = new DFintransAccount();
            string accountNo = DFLink.GetLink(bdwAccountNo, lDateTrans, lBranchNo, paymentRef);
            acct.Populate(conn, trans, accountNo);

            if (amount != 0)
            {
                /* Reverse the DPY record */
                new BTransaction(conn, trans, bdwAccountNo,
                    branchNo, refNo,
                    amount, User, TransType.Correction,
                    "", "", "", 1, countryCode, dateTrans,
                    "", 0);

                // Increase the BDW balance
                acct.BDWBalance += amount;

                // Save a link on Fintrans_Account table for the correction
                DFintransAccount FALink = new DFintransAccount(bdwAccountNo,
                    accountNo, dateTrans, branchNo, refNo);
                FALink.SaveAccountLink(conn, trans);
            }
            acct.Save(conn, trans);

            /* write a record to the CorrectedPayments table so we know that 
             * this payment has been corrected */
            cp.Write(conn, trans, bdwAccountNo, paymentRef, refNo);

            /* if authorisation was required write a record to the transaction audit table */
            if (authorisedBy != 0)
                ta.Write(conn, trans, bdwAccountNo, User, authorisedBy, amount, amount, dateTrans, refNo, TransType.DebtPayment);

        }

        public DataTable ValidateMambuSettled(string customerID, string accountID)
        {

            DCustomer cust = new DCustomer();
            DataTable loanAccountIds = cust.GetMambuAccountsSettled(customerID, accountID);
            return loanAccountIds;
        }
        public void SaveRefund(SqlConnection conn, SqlTransaction trans,
            string accountNo, short branchNo,
            decimal amount, string transType, string bankCode,
            string bankAcctNo, string chequeNo, short payMethod,
            string countryCode, DateTime dateTrans,
            DateTime linkedDateTrans, short linkedBranchNo, int linkedRefNo,
            string footNote, int authorisedBy, string CodeSource,
             out int refNo, out int fintransId)
        {
            fintransId = 0;
            var bt = new BTransaction();
            refNo = 0;
            if (transType == TransType.StoreCardRefund)
            {
                bt.User = authorisedBy;
                bt.TransferTransaction(conn, trans, accountNo, chequeNo, transType, amount, branchNo, countryCode, dateTrans, footNote, linkedRefNo, payMethod, 1, Convert.ToInt64(bankAcctNo));
                return;
            }
            CLAmortizationReversePayment(conn, trans, accountNo, amount); // Added by KM, for CLA Outstanding Balance Calculation
            DTransactionAudit ta = new DTransactionAudit();
            BBranch branch = new BBranch();
             refNo = branch.GetTransRefNo(conn, trans, branchNo);

            BTransaction t = new BTransaction();
            /* write the Refund transaction */
            //BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
            //    refNo, amount, User, transType,
            //    bankCode, bankAcctNo, chequeNo,
            //    payMethod, countryCode, dateTrans,
            //    footNote);

            //68738 call this function now as there is code for autosettling of account [PC]
            t.User = User;
            t.WriteGeneralTransaction(conn, trans, accountNo, branchNo,
                amount, transType,
                bankCode, bankAcctNo, chequeNo,
                payMethod, countryCode,
                footNote, 0);
            fintransId = t.FintransId;
            t.FintransId = 0;
            // Save an Exchange transaction if a foreign currency was taken
            if (PayMethod.IsForeignCurrencyPayMethod(payMethod))
            {
                this.SaveExchangeTransaction(conn, trans, accountNo, t.TransactionRefNo, t.TransactionDate, payMethod, -amount, 0, branchNo);
            }

            /* write a record to the transaction audit table */
            ta.Write(conn, trans, accountNo, User, authorisedBy, amount, amount, dateTrans, refNo, transType);


            // Check whether the refund is linked to another transaction and account
            // (for DPY refunds on a BDW account)
            if (linkedRefNo != 0)
            {
                // Find the originating account for this transaction
                DFintransAccount DFLink = new DFintransAccount();
                string linkedAcctNo = DFLink.GetLink(accountNo, linkedDateTrans, linkedBranchNo, linkedRefNo);

                // Save a link on Fintrans_Account table for the refund
                DFintransAccount FALink = new DFintransAccount(accountNo,
                    linkedAcctNo, dateTrans, branchNo, refNo);
                FALink.SaveAccountLink(conn, trans);
            }
                        
        }

        /// <summary>
        /// GetUnexportedCashierTotals
        /// </summary>
        /// <param name="branchno">int</param>
        /// <param name="totals">double</param>
        /// <param name="deposits">double</param>
        /// <returns>int</returns>
        /// 
        public DataSet GetUnexportedCashierTotals(int branchno)
        {
            //int status = 0;
            DCashierTotal da = new DCashierTotal();
            return da.GetUnexportedTotals(branchno); ;
        }

        /// <summary>
        /// HasCashierTotalled
        /// </summary>
        /// <param name="empeeno">int</param>
        /// <param name="branchno">int</param>
        /// <param name="nottotalled">int</param>
        /// <returns>bool</returns>
        /// 
        public bool HasCashierTotalled(int empeeno)
        {

            DFinTrans da = new DFinTrans();
            return da.HasCashierTotalled(empeeno);

        }

        /// <summary>
        /// GetUnexportedCashierTotals
        /// </summary>
        /// <param name="branchno">int</param>
        /// <param name="total">double</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetUnexportedCashierTotals(short branchno, out decimal total)
        {
            DataSet ds = new DataSet();
            DCashierTotal da = new DCashierTotal();
            ds = da.GetUnexported(branchno, out total);
            return ds;
        }


        /// <summary>
        /// GetExchangeRates
        /// </summary>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetExchangeRates(SqlConnection conn, SqlTransaction trans)
        {
            DataSet ds = new DataSet();
            DExchangeRate de = new DExchangeRate();
            ds.Tables.Add(de.GetCurrent(conn, trans));
            return ds;
        }

        /// <summary>
        /// GetExchangeRateHistory
        /// </summary>
        /// <param name="currency">int</param>
        /// <param name="dateFrom">DateTime</param>
        /// <param name="dateTo">DateTime</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetExchangeRateHistory(string currency, DateTime dateFrom, DateTime dateTo)
        {
            DataSet ds = new DataSet();
            DExchangeRate de = new DExchangeRate();
            ds.Tables.Add(de.GetHistory(currency, dateFrom, dateTo));
            return ds;
        }

        /// <summary>
        /// SaveExchangeRates
        /// </summary>
        /// <param name="exchangeRateSet">DataSet</param>
        /// <returns>void</returns>
        /// 
        public void SaveExchangeRates(SqlConnection conn, SqlTransaction trans, DataSet exchangeRateSet)
        {
            DExchangeRate exchangeRate = new DExchangeRate();

            foreach (DataTable table in exchangeRateSet.Tables)
                if (table.TableName == TN.ExchangeRates)
                    foreach (DataRow row in table.Rows)
                        if ((string)row[CN.Status] == RateStatus.Edit)
                        {
                            exchangeRate.currency = (string)row[CN.Code];
                            exchangeRate.rate = Convert.ToDecimal(row[CN.ExchangeRate]);
                            exchangeRate.empeeNo = (int)row[CN.EmployeeNo];
                            exchangeRate.SaveNewRate(conn, trans);
                        }
        }

        /// <summary>
        /// CalcForeignTender
        /// </summary>
        /// <param name="currency">int</param>
        /// <param name="localTendered">decimal</param>
        /// <returns>decimal</returns>
        /// 
        public decimal CalcForeignTender(SqlConnection conn, SqlTransaction trans, int currency, decimal localTender)
        {
            decimal foreignTender = 0.0M;
            DataSet ExchangeRateSet = this.GetExchangeRates(conn, trans);
            DataTable ExchangeRateTable = ExchangeRateSet.Tables[TN.ExchangeRates];

            int i = 0;
            decimal curExchangeRate = 0.0M;
            while (i < ExchangeRateTable.Rows.Count && curExchangeRate == 0)
            {
                if (System.Convert.ToInt16(ExchangeRateTable.Rows[i][CN.Code]) == currency)
                    curExchangeRate = System.Convert.ToDecimal(ExchangeRateTable.Rows[i][CN.ExchangeRate]);
                i++;
            }

            if (curExchangeRate != 0)
                foreignTender = Math.Round(localTender / curExchangeRate, 2);
            else
                foreignTender = 0.0M;

            return foreignTender;
        }

        public decimal CalcExchangeAmount(SqlConnection conn, SqlTransaction trans, int fromCurrency, int toCurrency, decimal amount)
        {
            // Convert from any currency to another currency
            // If there is no exchange rate set for a currency then it will be
            // assumed to be the local currency or the same value as the local currency
            DataSet ExchangeRateSet = this.GetExchangeRates(conn, trans);
            DataTable ExchangeRateTable = ExchangeRateSet.Tables[TN.ExchangeRates];

            int i = 0;
            decimal fromExchangeRate = 0.0M;
            decimal toExchangeRate = 0.0M;
            while (i < ExchangeRateTable.Rows.Count && fromExchangeRate == 0 && toExchangeRate == 0)
            {
                if (System.Convert.ToInt16(ExchangeRateTable.Rows[i][CN.Code]) == fromCurrency)
                    fromExchangeRate = System.Convert.ToDecimal(ExchangeRateTable.Rows[i][CN.ExchangeRate]);

                if (System.Convert.ToInt16(ExchangeRateTable.Rows[i][CN.Code]) == toCurrency)
                    toExchangeRate = System.Convert.ToDecimal(ExchangeRateTable.Rows[i][CN.ExchangeRate]);

                i++;
            }

            if (fromExchangeRate != 0)
                amount = Math.Round(amount * fromExchangeRate, 2);

            if (toExchangeRate != 0)
                amount = Math.Round(amount / toExchangeRate, 2);

            return amount;
        }

        /// <summary>
        /// SaveExchangeTransaction
        /// </summary>
        /// <param name="accountNo">string</param>
        /// <param name="currency">int</param>
        /// <param name="localTender">decimal</param>
        /// <param name="localChange">decimal</param>
        /// <returns>void</returns>
        /// 
        public void SaveExchangeTransaction(SqlConnection conn, SqlTransaction trans, string accountNo, int transRefNo, DateTime dateTrans, int currency, decimal localTender, decimal localChange, short branchNumber)
        {
            // Calculate Foreign Currency amount from Tendered Amount
            // The Tendered Amount can be negative for a Refund or Correction
            decimal foreignTender = this.CalcForeignTender(conn, trans, currency, localTender);

            if (Math.Abs(foreignTender) >= 0.01M || localChange >= 0.01M)
            {
                DExchangeTrans ExchangeTrans = new DExchangeTrans();
                ExchangeTrans.acctNo = accountNo;
                ExchangeTrans.transRefNo = transRefNo;
                ExchangeTrans.dateTrans = dateTrans;
                ExchangeTrans.payMethod = currency;
                ExchangeTrans.foreignTender = foreignTender;
                ExchangeTrans.localChange = localChange;
                ExchangeTrans.branchNo = branchNumber;
                ExchangeTrans.SaveExchangeTrans(conn, trans);
            }
        }

        public DataSet GetBreakDownForEmployee(int employeeno, DateTime datefrom, DateTime dateto)
        {
            DataSet ds = new DataSet();
            DFinTrans da = new DFinTrans();

            ds.Tables.Add(da.GetBreakDownForEmployee(employeeno, datefrom, dateto));

            return ds;
        }


        public int FreeInstalmentAvailable(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
            int voucherAmount = 0;

            if ((bool)Country[CountryParameterNames.TierPCEnabled])
            {
                // Tier1/2 Privilege Club
                DAccount da = new DAccount();
                voucherAmount = da.PrivilegeClubVoucher(conn, trans, acctNo);
            }
            else
            {
                // Classic Privilege Club
                //
                // An account with a good history will be given a free instalment.
                // The busines logic for this is all in a stored procedure.
                //
                // The rules for an account to qualify are:
                //
                //  - The account's termstype is one that qualifies for one-month free instalment
                //    . TermTypeTable.HasFreeInstalment = 1
                //
                //  - The account has never been in arrears
                //    . Acct.DateIntoArrears = blank
                //
                //  - At least 11 instalments have been paid after delivery
                //    . InstalPlan.DateFirst != blank AND InstalPlan.DateFirst <= 11 months old
                //
                //  - The account is in advance by at least one instalment
                //    OR the account is 12 months old and zero arrears or in advance
                //    . (Acct.Arrears <= -(InstalPlan.InstalAmount)) OR (InstalPlan.DateFirst <= 12 months old AND Acct.Arrears <= 0)
                //
                //  - The free instalment (Marketing Promotion) has not already been applied
                //    . Record(s) do not exist where FinTrans.TransTypeCode = 'MKT' with SUM(FinTrans.TransValue) >= InstalPlan.InstalAmount

                DAccount da = new DAccount();
                voucherAmount = da.FreeInstalmentAvailable(conn, trans, acctNo);
            }

            return voucherAmount;
        }

        public bool IsDepositReferenceUnique(string reference)
        {
            DCashierDeposit cd = new DCashierDeposit();
            if ((bool)Country[CountryParameterNames.DepositUniqueReference])
            {
                return cd.IsDepositReferenceUnique(reference);
            }
            else
            {
                return true;
            }
        }

        public DataSet GetCashierOutstandingIncome(short branchNo)
        {
            DataSet ds = new DataSet();
            DEmployee e = new DEmployee();
            ds.Tables.Add(e.GetCashierOutstandingIncome(branchNo));
            return ds;
        }

        public DataSet GetCashierOutstandingIncomeByPayMethod(int empeeno, short branchno)
        {
            DataSet ds = new DataSet();
            DEmployee e = new DEmployee();
            ds.Tables.Add(e.GetCashierOutstandingIncomeByPayMethod(empeeno, branchno));
            return ds;
        }

        public DataSet GetCashierTotalsForPrint(short branchno, int employeeno, DateTime datefrom, DateTime dateto, bool listCheques, out decimal total)
        {
            total = 0;
            DataSet ds = new DataSet();
            DFinTrans da = new DFinTrans();

            /* if we are listing cheques - then datefrom needs to be set to the value of
             * dateto and dateto needs to be set to the end of that day. */
            if (listCheques)
            {
                datefrom = new DateTime(dateto.Year, dateto.Month, dateto.Day, 0, 0, 0);
                dateto = new DateTime(datefrom.Year, datefrom.Month, datefrom.Day, 23, 59, 59);
            }

            ds.Tables.Add(GetCashierDeposits(employeeno, 0, datefrom, dateto, branchno, "-1", false, -1));   //IP - 15/12/11 - #8810 - CR1234    
            ds.Tables.Add(da.GetCashierTotalsForPrint(employeeno == -1, branchno, employeeno, datefrom, dateto, listCheques, out total));

            return ds;
        }

        public DataSet GetCashierTotalsSummary(short branchno, DateTime datefrom, DateTime dateto)
        {
            DCashierTotal ct = new DCashierTotal();
            DataSet ds = new DataSet();
            ds.Tables.Add(ct.GetSummary(branchno, datefrom, dateto));
            return ds;
        }

        public void LockDepositScreen(short branchno)
        {
            DBranch b = new DBranch();
            b.LockDepositScreen(branchno);
        }

        public void UnLockDepositScreen(short branchno)
        {
            DBranch b = new DBranch();
            b.UnLockDepositScreen(branchno);
        }

        public bool CashierMustDeposit(int empeeno)
        {
            DCashierDeposit cd = new DCashierDeposit();
            return cd.CashierMustDeposit(empeeno);
        }

        public decimal GetOutstandingSafeDeposits(int empeeno, short branchno)
        {
            DCashierDeposit cd = new DCashierDeposit();
            return cd.GetOutstandingSafeDeposits(empeeno, branchno);
        }

        public void ReverseSafeDeposits(int empeeno)
        {
            DCashierDeposit cd = new DCashierDeposit();
            cd.ReverseSafeDeposits(empeeno, User);
        }

        /// <summary>
        /// GetPaymentHolidays
        /// </summary>
        /// <param name="acctno">string</param>
        /// <param name="agrmtno">int</param>
        /// <returns>DataSet</returns>
        /// 
        public DataSet GetPaymentHolidays(SqlConnection conn, SqlTransaction trans,
            string customerID)
        {
            DataSet ds = new DataSet();
            DPaymentHolidays da = new DPaymentHolidays();
            ds.Tables.Add(da.Get(conn, trans, customerID));
            return ds;
        }

        /// <summary>
        /// WritePaymentHoliday
        /// </summary>
        /// <param name="acctno">string</param>
        /// <param name="agrmtno">int</param>
        /// <param name="datetaken">DateTime</param>
        /// <param name="empeeno">int</param>
        /// <param name="newdatefirst">DateTime</param>
        /// <returns>void</returns>
        /// 
        public void WritePaymentHoliday(SqlConnection conn, SqlTransaction trans,
            string acctno, int agrmtno,
            DateTime datetaken, int empeeno,
            DateTime newdatefirst)
        {
            DPaymentHolidays da = new DPaymentHolidays();
            da.Write(conn, trans, acctno, agrmtno, datetaken, empeeno, newdatefirst);
        }

        public void IncludeDeposits(SqlConnection conn, SqlTransaction trans,
            int empNo, short includeDeposits)
        {
            DCashierDeposit cd = new DCashierDeposit();
            cd.IncludeDeposits(conn, trans, empNo, includeDeposits);
        }

        public DataSet GetBranchCashierList(short branchNo, DateTime dateFrom, DateTime dateTo)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            DCashierTotal dct = new DCashierTotal();
            DCashierTotalsBreakdown dctb = new DCashierTotalsBreakdown();

            // The date range must be from the start of the start day to the end of the end day
            DateTime dateStart = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
            DateTime dateEnd = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);

            // Load the Cashiers and their totals for this Branch
            dt = dct.GetBranchCashierList(branchNo, dateStart, dateEnd);
            dt.TableName = TN.CashierByBranch;
            ds.Tables.Add(dt);

            // Load the breakdown for each Cashier at this Branch
            dt = dctb.GetBranchCashierBreakdown(branchNo, dateStart, dateEnd);
            dt.TableName = TN.CashierBreakdown;
            ds.Tables.Add(dt);

            return ds;
        }

        public decimal GetWarrantyAdjustment(string acctno)
        {
            DFinTrans da = new DFinTrans();
            return da.GetWarrantyAdjustment(null, null, acctno);
        }


        public void WriteFreeInstalment(SqlConnection conn, SqlTransaction trans,
            string accountNo, short branchNo, decimal amount, string countryCode)
        {
            DateTime timeToday = DateTime.Now;

            // Write the transaction for a Free Instalment
            BTransaction txn = new BTransaction();
            txn.User = User;
            txn.WriteGeneralTransaction(conn, trans, accountNo, branchNo,
                -amount, TransType.FreeInstalment, "", "", "", 0,
                countryCode, FootNote.FreeInstalment, 0);

            // Add the Account Code (or update the date) for a Free Instalment
            BAccount acct = new BAccount();
            acct.AddCodeToAccount(conn, trans, accountNo, AC.FreeInstalment, User, timeToday);

            // Save the free instalment
            DFinTrans freeInstalment = new DFinTrans();
            freeInstalment.SaveFreeInstalment(conn, trans, accountNo, branchNo, timeToday, -amount, false);
        }

        // CR 543 Returned cheques - Peter Chong [26-Sep-2006]
        /// <summary>
        /// Cheques the number of returned cheques within a given period for a customer. 
        /// </summary>
        /// <returns>A dataset containing a list of returned cheques if authorisation required</returns>
        public DataSet GetReturnedCheques(SqlConnection conn, SqlTransaction trans, string customerId, out bool authorisationRequired)
        {
            authorisationRequired = false;

            //Get country parameters 
            int returnedChequePeriod;
            int returnedChequeAllowedCount;
            try
            {
                returnedChequePeriod = Convert.ToInt32(Country[CountryParameterNames.ReturnedChequePeriod]);
                returnedChequeAllowedCount = Convert.ToInt32(Country[CountryParameterNames.ReturnedChequeNumberAllowed]);

                // this functionality is disabled if returned cheque period is 0
                if (returnedChequePeriod <= 0)
                    return null;
            }
            catch { return null; }

            DPaymentReturnedCheques da = new DPaymentReturnedCheques();
            DataSet ds = new DataSet();
            DataTable dt = da.Get(conn, trans, customerId, returnedChequePeriod, returnedChequeAllowedCount, out authorisationRequired);

            ds.Tables.Add(dt);

            return ds;
        }

        //66669 
        /// <summary>
        /// Calls the Data Access Layer method CheckForCommittedData to determine whether or not transaction has been committed
        /// </summary>
        /// <param name="transactionSet"></param>
        /// <returns>Ammended dataset transactionSet with information as to whether or not data has been committed</returns>
        public DataSet CheckForCommittedData(DataSet transactionSet)
        {
            string acctno;
            int transrefNo;
            bool dataCommitted = true;
            DFinTrans finTrans = new DFinTrans();

            foreach (DataRow row in transactionSet.Tables[TN.Transactions].Rows)
            {
                acctno = row[CN.AcctNo].ToString();
                transrefNo = Convert.ToInt32(row[CN.TransRefNo]);      //jec 10/04/08 ToInt32 (was ToInt16)
                dataCommitted = finTrans.CheckForCommittedData(acctno, transrefNo);
                row[CN.Committed] = (dataCommitted == true) ? "Y" : "N";
            }
            return transactionSet;
        }

        public bool GetAgreementData(DataTable tblTrans)
        {
            string AccountNo = string.Empty;

            foreach (DataRow row in tblTrans.Rows)
            { 
                AccountNo = row[CN.AcctNo].ToString();
                DAccount dAccount = new DAccount(AccountNo);
                row[CN.AgreementTotal] = dAccount.AgreementTotal;
                row.AcceptChanges();
            }
            return true;
        }

        // uat376 rdb BDW Reversal
        public bool ReverseBDW(SqlConnection conn, SqlTransaction trans, string acctno, string countryCode, int user)
        {
            DateTime dateTrans = DateTime.Now;
            bool success = false;
            BAccount acct = new BAccount();

            acct.Populate(acctno);

            //do not proceed if  acct is not settled
            if (acct.CurrentStatus == "S")
            {
                // revert status back to last in Status table
                DataSet ds = acct.GetStatusForAccount(acctno);

                // get last status of account before BDW
                foreach (DataRow sdr in ds.Tables[0].Rows)
                {
                    if (acct.CurrentStatus != sdr["Statuscode"].ToString())
                    {
                        acct.CurrentStatus = sdr["Statuscode"].ToString();
                        //IP - 28/08/09 - UAT(737) - BDWBalance and BDWCharges should be set to 0 after a reversal.
                        acct.BDWBalance = 0;
                        acct.BDWCharges = 0;
                        acct.Save(conn, trans);
                        break;
                    }
                }


                // get all transactions added during BDW
                DFinTrans finTrans = new DFinTrans();
                DataTable dt = finTrans.GetBDWTransactions(conn, trans, acctno);
                // reverse these all;
                foreach (DataRow dr in dt.Rows)
                {
                    short branchNo = Convert.ToInt16(dr["branchNo"]);
                    int refNo = Convert.ToInt32(dr["transrefNo"]);
                    decimal amount = -Convert.ToDecimal(dr["transvalue"]);
                    string transType = TransType.BadDebtWriteOffReversal;
                    string bankCode = dr["bankcode"] != DBNull.Value ? Convert.ToString(dr["bankcode"]) : string.Empty;
                    string bankAcctNo = dr["bankacctno"] != DBNull.Value ? Convert.ToString(dr["bankacctno"]) : string.Empty;
                    string chequeNo = dr["chequeNo"] != DBNull.Value ? Convert.ToString(dr["chequeNo"]) : string.Empty;
                    short payMethod = Convert.ToInt16(dr["payMethod"] != DBNull.Value ? dr["payMethod"] : 0);
                    string footNote = "BDR";

                    /* write the Correction transaction */
                    BTransaction t = new BTransaction(conn, trans, acctno, branchNo,
                     refNo, amount, user, transType,
                     bankCode, bankAcctNo, chequeNo,
                     payMethod, countryCode, dateTrans,
                     footNote, 0);
                }

                success = true;

            }
            return success;
        }

        //IP - 21/02/12 - #9626 - UAT90 - Save Cash Loan Disbursements (CLD) to CashierDeposits
        public void CashLoanSaveToCashierDeposits(SqlConnection conn, SqlTransaction trans, ref Blue.Cosacs.Shared.CashLoanDetails det, short branchNo, short disbursementType)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[] {         new DataColumn(CN.TransTypeCode),
                                                           new DataColumn(CN.Value, Type.GetType("System.Decimal")),
                                                           new DataColumn(CN.Reference),
                                                           new DataColumn(CN.PayMethod),
                                                           new DataColumn(CN.DateDeposit, Type.GetType("System.DateTime")),
                                                           new DataColumn(CN.Runno, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.EmployeeNo, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.EmployeeNoEntered, Type.GetType("System.Int32")),
                                                           new DataColumn(CN.BranchNo, Type.GetType("System.Int16")),
                                                           new DataColumn(CN.IsCashierFloat, Type.GetType("System.Int16")),
                                                           new DataColumn(CN.IncludeInCashierTotals, Type.GetType("System.Int16"))});

            dt.Rows.Add("CLD", det.loanAmount, det.accountNo, disbursementType, DateTime.Now, 0, det.empeenoDisburse, det.empeenoDisburse, branchNo, 0, 1);

            ds.Tables.Add(dt);

            this.SaveCashierDeposits(conn, trans, ds);

        }

        //Get early settlement figure for amortized cash loan account
        public void GetEarlySettlementFig(string accountNumber, out decimal settlementFig)
        {
            settlementFig = 0;
            DCustomer cust = new DCustomer();
            cust.GetEarlySettlementFig(accountNumber, out settlementFig);
        }

        public void ValidatePayment(short payMethod, out bool result)
        {
            result = false;
            DAccount payment = null;
            payment = new DAccount();
            payment.ValidatePayment(payMethod,out result);
            
            
        }

        public void CheckIfDeposit(string acctno, decimal paymentAmt, out bool result)
        {
            result = false;
            DAccount payment = null;
            payment = new DAccount();
            payment.CheckIfDeposit(acctno, paymentAmt, out result);


        }

        public void GetDepositAmount(string acctno, out decimal depositAmt)
        {
            depositAmt = 0;
            DAccount payment = null;
            payment = new DAccount();
            payment.GetDepositAmount(acctno, out depositAmt);


        }

        public void CheckIfDepositPayment(string acctno, decimal paymentAmt, out bool result)
        {
            result = false;
            DAccount payment = null;
            payment = new DAccount();
            payment.CheckIfDepositPayment(acctno, paymentAmt, out result);


        }
        #region CLA OutStanding

        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Rahul D
        // Details  :  Call function for amount decuction if the account is created on AmortizedOutStandingBal


        /// <summary>
        /// Function will deduct amount from account in table CLAPaymentHistory
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tran"></param>
        /// <param name="acctno"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool DeductAmountFromPaymentHistory(SqlConnection con, SqlTransaction tran, string acctno, decimal amount)
        {
            DAccount account = new DAccount(con, tran, acctno);
            account.GetAccount(acctno);
            if (account.IsAmortizedOutStandingBal && account.IsAmortized)
            {
                DFinTrans dfin = new DFinTrans();
                if (dfin.DeductAmountFromPaymentHistory(con, tran, acctno, amount))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsCashLoanAmortizedAccount(SqlConnection conn, SqlTransaction tran, string acctno)
        {
            DAccount account = new DAccount(conn, tran, acctno);
            account.GetAccount(acctno);
            if (account.IsAmortizedOutStandingBal && account.IsAmortized)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="acctno"></param>
        /// <param name="amount"></param>
        /// <param name="creditDebit"></param>
        /// <param name="transtype"></param>
        public void CLGeneralFinanceTransaction(SqlConnection conn, SqlTransaction trans, string acctno, decimal amount, int creditDebit, string transtype)
        {
            if (IsCashLoanAmortizedAccount(conn, trans, acctno))
            {
                DFinTrans dfin = new DFinTrans();
                dfin.CLGeneralFinanceTransaction(conn, trans, acctno, amount, creditDebit, transtype);
            }
        }
        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Kedar M
        // Details  :  Call for function  amount Reversal if the account is created on AmortizedOutStandingBal

        /// <summary>
        /// Function will Reverse amount from account in table CLAPaymentHistory
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tran"></param>
        /// <param name="acctno"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CLAmortizationReversePayment(SqlConnection con, SqlTransaction tran, string acctno, decimal amount)
        {
            DAccount account = new DAccount(con, tran, acctno);
            account.GetAccount(acctno);
            if (account.IsAmortizedOutStandingBal && account.IsAmortized)
            {
                DFinTrans dfin = new DFinTrans();
                if (dfin.CLAmortizationReversePayment(con, tran, acctno, amount))
                {
                    return true;
                }
            }
            return false;
        }

        // CR       :  CLA outstanding Balance Calculation
        // Author   :  Rahul D, Zensar
        // Details  :  Function to get max amount to be credited or debited for validation
        /// <summary>
        /// Function to get max amount to be credited or debited for validation
        /// </summary>
        /// <param name="conn">SQL Connection</param>
        /// <param name="trans">SQL Transation</param>
        /// <param name="acctno">Account number : String</param>
        /// <param name="amount">Amount : Decimal</param>
        /// <param name="creditDebit">For Debit 1, for credit -1</param>
        /// <param name="transtype">Trans type code</param>
        /// <returns></returns>
        public int CLGeneralFinanceTransactionValidation(SqlConnection conn, SqlTransaction trans, string acctno, out decimal amount, int creditDebit, string transtype)
        {
            amount = 0;
            DFinTrans dfin = new DFinTrans();
            return dfin.CLGeneralFinanceTransactionValidation(conn, trans, acctno, out amount, creditDebit, transtype);
        }


        public void GetOutstandingAndCharges(string acctno, out decimal outstanding, out decimal bdCharges)
        {
            outstanding = 0;
            bdCharges = 0;
            DFinTrans dfin = new DFinTrans();
            if (IsCashLoanAmortizedAccount(null, null, acctno))
            {
                dfin.GetOutstandingAndCharges(acctno, out outstanding, out bdCharges);
            }

        }
        #endregion
    }
}