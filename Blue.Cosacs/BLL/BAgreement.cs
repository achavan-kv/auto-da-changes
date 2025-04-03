using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.DAL;
using Blue.Cosacs.Messages.Service;
using Blue.Cosacs;
using System.Xml;
using STL.Common.Constants.AccountTypes;
using STL.BLL.WebApi;
//IP - 03/02/10 - CR1072 - 3.1.9


namespace STL.BLL
{
    /// <summary>
    /// Summary description for BAgreement.
    /// </summary>
    public class BAgreement : CommonObject
    {
        private int _createdBy = 0;
        public int CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        private int _agreementNo = 1;
        public int AgreementNumber
        {
            get
            {
                return _agreementNo;
            }
            set
            {
                _agreementNo = value;
            }
        }
        private string _codFlag = "N";
        public string CODFlag
        {
            get
            {
                return _codFlag;
            }
            set
            {
                _codFlag = value;
            }
        }
        private int _salesPerson = 0;
        public int SalesPerson
        {
            get
            {
                return _salesPerson;
            }
            set
            {
                _salesPerson = value;
            }
        }
        private string _soa = "";
        public string SOA
        {
            get
            {
                return _soa;
            }
            set
            {
                _soa = value;
            }
        }
        private string _holdProp = "Y";
        public string HoldProp
        {
            get
            {
                return _holdProp;
            }
            set
            {
                _holdProp = value;
            }
        }
        private decimal _cashPrice = 0;
        public decimal CashPrice
        {
            get
            {
                return _cashPrice;
            }
            set
            {
                _cashPrice = value;
            }
        }
        private decimal _agreementTotal = 0;
        public decimal AgreementTotal
        {
            get
            {
                return _agreementTotal;
            }
            set
            {
                _agreementTotal = value;
            }
        }
        private decimal _deposit = 0;
        public decimal Deposit
        {
            get { return _deposit; }
            set { _deposit = value; }
        }

        private decimal _serviceCharge = 0;
        public decimal ServiceCharge
        {
            get { return _serviceCharge; }
            set { _serviceCharge = value; }
        }
        private decimal _discount = 0;
        public decimal Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }
        private string _accountNo = "";
        public string AccountNumber
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }
        private DateTime _agreementDate = DateTime.Today;
        public DateTime AgreementDate
        {
            get { return _agreementDate; }
            set { _agreementDate = value; }
        }
        private short _origBr = 0;
        public short OrigBr
        {
            get { return _origBr; }
            set { _origBr = value; }
        }
        private DateTime _depChequeClear = DateTime.Today;
        public DateTime DepositChequeClears
        {
            get { return _depChequeClear; }
            set { _depChequeClear = value; }
        }
        private string _holdMerch = "";
        public string HoldMerch
        {
            get { return _holdMerch; }
            set { _holdMerch = value; }
        }
        private DateTime _dateDel = DateTime.MinValue.AddYears(1899);
        public DateTime DateDel
        {
            get { return _dateDel; }
            set { _dateDel = value; }
        }
        private DateTime _dateNextDue = DateTime.Today;
        public DateTime DateNextDue
        {
            get { return _dateNextDue; }
            set { _dateNextDue = value; }
        }
        private decimal _oldAgreementBal = 0;
        public decimal OldAgreementBalance
        {
            get { return _oldAgreementBal; }
            set { _oldAgreementBal = value; }
        }
        private decimal _sundryChargeTotal = 0;
        public decimal SundryChargeTotal
        {
            get { return _sundryChargeTotal; }
            set { _sundryChargeTotal = value; }
        }
        private string _paymethod = "";
        public string PayMethod
        {
            get { return _paymethod; }
            set { _paymethod = value; }
        }
        private string _unpaidFlag = "";
        public string UnpaidFlag
        {
            get { return _unpaidFlag; }
            set { _unpaidFlag = value; }
        }
        private string _deliveryFlag = "";
        public string DeliveryFlag
        {
            get { return _deliveryFlag; }
            set { _deliveryFlag = value; }
        }
        private string _fullDelFlag = "";
        public string FullDelFlag
        {
            get { return _fullDelFlag; }
            set { _fullDelFlag = value; }
        }
        private string _paymentMethod = "";
        public string PaymentMethod
        {
            get { return _paymentMethod; }
            set { _paymentMethod = value; }
        }
        private int? _employeeNumAuth = null;
        public int? EmployeeNumAuth
        {
            get { return _employeeNumAuth; }
            set { _employeeNumAuth = value; }
        }
        private DateTime? _dateAuth = null;
        public DateTime? DateAuth
        {
            get { return _dateAuth; }
            set { _dateAuth = value; }
        }
        private int _employeeNumChange = 0;
        public int EmployeeNumChange
        {
            get { return _employeeNumChange; }
            set { _employeeNumChange = value; }
        }
        private DateTime _dateChange = DateTime.Now;
        public DateTime DateChange
        {
            get { return _dateChange; }
            set { _dateChange = value; }
        }

        private decimal _pxalloxed = 0;
        public decimal PxAllowed
        {
            get { return _pxalloxed; }
            set { _pxalloxed = value; }
        }
        private short _paymentholidays = 0;
        public short PaymentHolidays
        {
            get { return _paymentholidays; }
            set { _paymentholidays = value; }
        }
        private string _auditsource = "";
        public string AuditSource
        {
            get { return _auditsource; }
            set { _auditsource = value; }
        }

        private bool _taxFree = false;
        public bool TaxFree
        {
            get { return _taxFree; }
            set { _taxFree = value; }
        }

        private DAgreement data;
        public void ClearProposal(SqlConnection conn, SqlTransaction trans, string accountNumber, string source, XmlNode replacement = null) //#18409//IP - 04/02/10 - CR1072 - 3.1.9 - Added Source of Delivery Authorisation.
        {
            data = new DAgreement();
            data.User = User;

            // Collect hold property value before clear proposal action.
            var AcctR = new AccountRepository();
            bool holdProp = AcctR.GetAccountData(accountNumber, conn, trans);

            data.ClearProposal(conn, trans, accountNumber, source);

            // CR - Jyoti - Create Service Request for Installation items only after "Clear Proposal"
            // instead of creating it on line item sale on windows PoS.
            if (holdProp)
            {
                BTransaction tran = new BTransaction();
                tran.ServiceRequestCreate(conn, trans, accountNumber);
            }
           
            AcctR.bookingType = "D";

            DataTable lineItemBooking = data.LineItemBooking;

            // #10178 - Send bookings to warehouse for scheduled orders
            if (lineItemBooking != null && lineItemBooking.Rows.Count > 0)          //IP/JC - 30/05/12 - #10178
            {
                lineItemBooking.Columns.Add("BookingID");

                //#14644 - No longer sending Cancellation from CoSACS
                //foreach (DataRow row in lineItemBooking.Rows)                        //IP - 08/06/12 - #10319
                //{
                //    if ((Convert.ToSingle(row[CN.QtyBooked]) != Convert.ToSingle(row["OrigBookQty"]) && Convert.ToSingle(row[CN.DelQty]) > 0 && (Convert.ToInt32(row["FailureId"]) == 0))      // #10605
                //            || (Convert.ToSingle(row["OrigBookQty"]) != (Convert.ToSingle(row["FailedQty"])) + Convert.ToSingle(row["DeliveredQty"]) + Convert.ToSingle(row["ScheduleQty"])))
                //    //#10488     //If there has been a previous booking and no booking failure then need to cancel prior to re-sending new booking
                //    {
                //        var cancelations = AcctR.GetCancelData(conn, trans, (int)row[CN.ID], this.User, "Cancellation - details changed");

                //        new Chub().CancelMany(cancelations);

                //    }
                //}


                AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);


                //Finally if actioned from Failed Delivery / Colelction screen then update as actioned.
                foreach (DataRow row in lineItemBooking.Rows)
                {
                    if (Convert.ToInt32(row["PreviousBookingId"]) != 0)
                    {
                        new WarehouseRepository().UpdateBookingFailureActioned(conn, trans, Convert.ToInt32(row["PreviousBookingId"]), Convert.ToInt32(row["BookingID"]));
                    }

                    new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["BookingID"]));        //IP - 15/06/12 - #10387 - Update LineItemBookingSchedule with the BookingId
                }

                var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);

                new Chub().SubmitMany(bookings, conn, trans);

            }

            // check for Installations and create Service Request
            //commented for ZEN/UNC/CRF/CR2018-009 because SR is created while saving the product.
            //var Inst = new InstallationRepository();

            //var installations = (IEnumerable<RequestSubmit>)Inst.GetInstallationData(conn, trans, lineItemBooking);
            
            //new Chub().SubmitMany(installations, conn, trans);

            //  AcctR.ICClearOutstandingFlags(conn, trans, accountNumber, User);
            // Deliver Affinity, but only if an Affinity Terms Type

            BPayment affinityPayment = new BPayment();
            affinityPayment.User = User;

            var repository = new AccountRepository();

            //#18603 - CR15594 - Check if its Ready Assist and update the Ready Assist Contract Date to DA date.
            var readyAssist = repository.IsReadyAssist(conn, trans, accountNumber, this.AgreementNumber); //#18603

            affinityPayment.DeliverAffinity(conn, trans, accountNumber, readyAssist);  //#18603

            //Deliver warranties where parent already delivered

            repository.DeliverWarranties(conn, trans, accountNumber, this.AgreementNumber, replacement);

            if (readyAssist == true)
            {
                var status = ReadyAssistStatus.Active;
                repository.UpdateReadyAssistContractDate(conn, trans, accountNumber, this.AgreementNumber);
                repository.UpdateReadyAssistStatus(conn, trans, accountNumber, this.AgreementNumber, status);

                //Deliver Ready Assist on the account
                //repository.DeliverReadyAssist(conn, trans, accountNumber, this.AgreementNumber, this.User, da.AccountType);
            }



            //cr906 rdb 19/09/07
            //affinityPayment.DeliverCashLoan(conn, trans, accountNumber);      //IP - 10/10/11 - #3918 - CR1232 - Commented out as we do not want to deliver Cash Loan at this point (now processed through Cash Loan Disbursement screen).
        }

        /// <summary>
        /// This method returns a dataset containing the details relating to a specific 
        /// terms type code
        /// </summary>
        /// <param name="countryCode">country code</param>
        /// <param name="termsType">terms type code</param>
        /// <returns></returns>
        public DataSet GetTermsTypeDetails(string countryCode, string termsType, string acctNo, DateTime dateAcctOpen)
        {
            DataSet ds = new DataSet();
            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(null, null, countryCode, termsType, acctNo, "", dateAcctOpen);
            ds.Tables.Add(terms.TermsTypeDetails);
            return ds;
        }

        public DataSet TermsTypeGetDetails(string acctNo)
        {
            DataSet ds = new DataSet();
            DTermsType terms = new DTermsType();
            ds.Tables.Add(terms.TermsTypeGetDetails(acctNo));
            return ds;
        }

        public double GetInterestRate(string countryCode, string termsType, string acctNo,
                                           DateTime acctOpen)
        {
            DataSet ds = new DataSet();
            double intRate = 0;

            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(null, null, countryCode, termsType, acctNo, "", acctOpen);
            ds.Tables.Add(terms.TermsTypeDetails);

            if (ds.Tables[0].Rows.Count > 0)
            {
                intRate = (double)ds.Tables[0].Rows[0]["servpcent"];
            }

            return intRate;
        }

        public void CalculateInstalPlan(decimal subTotal,
                                        decimal deposit,
                                        decimal deferredTerms,
                                        decimal months,
                                        out decimal monthly,
                                        out decimal final)
        {
            monthly = 0;
            final = 0;
            //calculate the monthly and final installments
            if (months > 0 && deposit < subTotal)
            {
                if (months == 1)
                {
                    monthly = subTotal + deferredTerms - deposit;
                    final = monthly;
                }
                else
                {
                    monthly = (subTotal + deferredTerms - deposit) / months;

                    //if(Math.Round(monthly, 0) < monthly)
                    //    monthly = 1 + Math.Round(monthly,0);
                    //else
                    //    monthly = Math.Round(monthly,0);

                    // 11/12/07 rdb Malaysia require rounding to 0.1 (normally 1) use country param
                    int decimalPlaces = Convert.ToInt32(Country[CountryParameterNames.IntalmentRounding]);
                    decimal roundTo = Convert.ToDecimal(Math.Pow(0.1, Convert.ToDouble(decimalPlaces)));

                    if (Math.Round(monthly, decimalPlaces) < monthly)
                        monthly = roundTo + Math.Round(monthly, decimalPlaces);
                    else
                        monthly = Math.Round(monthly, decimalPlaces);

                    //possible that the final instalment will come out negative due
                    //to the rounding. If so the monthly payment should be reduced
                    while ((subTotal + deferredTerms - deposit) - (monthly * (months - 1)) < 0)
                        monthly--;

                    final = (subTotal + deferredTerms - deposit) - (monthly * (months - 1));
                    CountryRound(ref final);		//Round to 2 decimal places
                }
            }
        }

        public decimal CalculateMonthToExtend(SqlConnection conn, SqlTransaction trans,
            string countryCode,
            string termsType,
            string acctNo,
            string overrideBand,
            DateTime dateOpened,
            decimal deposit,
            decimal months,
            decimal subTotal,   
            string accountType,
            decimal adminSubTotal,		/* may not include warranties */
            decimal monthlyInstalment,
            //ref decimal insuranceCharge,
            //ref decimal adminCharge,
            decimal cmInsuranceTax,
            decimal cmAdminTax,
            //decimal cmDtTax,
            decimal mmiThresholdLimit,
            //ref DataSet variableRatesSet,
            decimal saleOrderVoucherValue,
            XmlNode saleOrderDT,
            bool taxExempt,
            bool waiveAdminCharge = false
            ,bool cashLoan = false
            )
        {

            Function = "BAgreement::CalculateMonthToExtend()";
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            insuranceCharge = 0;

            if (!waiveAdminCharge)
            {
                adminCharge = 0;
            }

            decimal insPcent = 0;
            decimal adminPcent = 0;
            decimal servicePcent = 0;
            decimal annual = 0;
            decimal actual = 0;
            decimal adminValue = 0;
            int monthsIntFree = 0;
            int monthsDeferred = 0;
            int oldMonthsIntFree = 0;
            int oldMonthsDeferred = 0;

            decimal newMonthlyInstalment = monthlyInstalment;
            decimal newFinalInstalment = 0;
            decimal existingTerms = months;

            // First get the termsType details
            // CR440 moved months int free and deferred from Acct Type to Terms Type
            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(conn, trans, countryCode, termsType, acctNo, overrideBand, dateOpened);

            // Old accounts will have old account types with their own months int free and deferred
            DAccount dAcct = new DAccount();
            dAcct.SelectType(conn, trans, accountType, countryCode, out oldMonthsIntFree, out oldMonthsDeferred);
            if (oldMonthsIntFree != 0 || oldMonthsDeferred != 0)
            {
                // Use the values from the old account type
                monthsIntFree = oldMonthsIntFree;
                monthsDeferred = oldMonthsDeferred;
            }
            else //GJ 10/8/05 - Only bother getting values from the DataTable if we aren't using the old values,
                 //even then, we need to allow for DBNull values.
            {
                if (terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree] != DBNull.Value)
                {
                    monthsIntFree = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree]);
                }
                if (terms.TermsTypeDetails.Rows[0][CN.DeferredMonths] != DBNull.Value)
                {
                    monthsDeferred = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.DeferredMonths]);
                }
            }

            if (((string)Country[CountryParameterNames.AdminChargeItem]).Length > 0)
            {
                adminPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.AdminPcent]) / 100;
                adminValue = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["AdminValue"]);
            }

            if (((string)Country[CountryParameterNames.InsuranceChargeItem]).Length > 0)
                insPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.InsPcent]) / 100;

            terms.GetVariableRates(conn, trans, termsType, dateOpened);
            int monthDiffDeferredIntFree = monthsDeferred - monthsIntFree;
            months += monthDiffDeferredIntFree;
            while (newMonthlyInstalment > mmiThresholdLimit)
            {
                months = months + 1;

                /* calculate the admin charge */
                if (!waiveAdminCharge) //If Cash Loan Admin Charge waived from Cash Loan Application screen do not calculate...value used is passed in and already set in ref variable adminCharge
                {
                    adminCharge = ((adminSubTotal - deposit) * adminPcent * (months / 12)) + adminValue;   //admin value only for Cash Loan, will be 0 for regular Terms Types
                }

                /* calculate the insurance charge unless flag is set - ALLOW FOR NULL !*/
                if (terms.TermsTypeDetails.Rows[0][CN.InsIncluded] == DBNull.Value
                    || !Convert.ToBoolean(terms.TermsTypeDetails.Rows[0][CN.InsIncluded]))
                {
                    insuranceCharge = (subTotal - deposit) * insPcent * (months / 12);
                }

                /* admin and insurance charges must be added onto the cash price
                 * of the account before calculating DT 
	            * Not including if Jamaica and having insurance included in Service Charge
	            */
                if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint])
                {
                    subTotal += adminCharge; // only include admin charge
                }
                else
                {
                    subTotal += adminCharge + insuranceCharge;
                }

                if (terms.VariableRates.Rows.Count == 0)
                {
                    // there are no variable interest rates so we will use the
                    // default service charge
                    if (months > 0 && deposit < subTotal)
                    {
                        servicePcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["servpcent"]) / 100;

                        //If Cash Loan exclude Admin Charge from Service Charge calculation
                        if (cashLoan)
                        {
                            annual = ((subTotal - adminCharge) - deposit) * servicePcent;
                        }
                        else
                        {
                            //Calculate the annual service charge
                            annual = (subTotal - deposit) * servicePcent;
                        }
                        //adjust for the duration in months
                        actual = annual * (months / 12);
                    }
                }
                else
                {
                    // variable interest rates exist for this terms type so we must
                    // calculate the service charge for each period

                    decimal interestCharge = 0;
                    decimal periodValue = 0;
                    decimal monthlyPayment = 0;
                    decimal previousPeriod = 0;
                    int order = 1;
                    DateTime dateFrom = DateTime.Now;

                    if (months > 0 && deposit < subTotal)
                    {
                        foreach (DataRow r in terms.VariableRates.Rows)
                        {
                            decimal variableMonths = 1 + (Convert.ToInt32(r[CN.ToMonth]) - Convert.ToInt32(r[CN.FromMonth]));

                            if (order == 1)
                                dateFrom = dateOpened.AddMonths(1);
                            else
                                dateFrom = dateFrom.AddMonths(Convert.ToInt32(previousPeriod + 1));

                            decimal rate = Convert.ToDecimal(r[CN.Rate]) / 100;

                            // calculate the annual service charge and then adjust for
                            // the duration in months
                            annual = (subTotal - deposit) * rate;
                            interestCharge = (annual * (months / 12)) / months;

                            // once the monthly interest charge has been calculated we
                            // need to calculate the charge for a particular period
                            // e.g. calculate interest charge for months 1-12 @ 13.2%
                            periodValue = interestCharge * variableMonths;

                            // calculate the monthly instalment the customer
                            // will have to pay for a particular period
                            monthlyPayment = ((subTotal - deposit) / months) + interestCharge;
                            monthlyPayment = Math.Round(monthlyPayment, 0);

                            while ((subTotal + interestCharge - deposit) - (monthlyPayment * (variableMonths - 1)) < 0)
                                monthlyPayment--;

                            if (order == terms.VariableRates.Rows.Count)
                                variableMonths--;

                            // total up interest charges for each period to give us
                            // out total service charge figure
                            actual += periodValue;

                            order++;
                            previousPeriod = variableMonths;
                        }
                        //variableRatesSet.Tables.Add(variableRates);
                    }
                }

                //we need to round down the deferred terms at this point if
                //we're going to otherwise the instalments will be slightly too high
                //check to see if we need to round down terms
                if ((bool)Country[CountryParameterNames.NoCents])
                {
                    if (countryCode != "I" &&       /* round down to 0 decimal places */
                      countryCode != "C")
                    {
                        actual = Convert.ToDecimal(Math.Floor(Convert.ToDouble(actual)));
                    }
                    else        /* round up to nearest 100 */
                    {
                        actual /= 100;
                        actual = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(actual)));
                        actual *= 100;
                    }
                }

                // malaysia require rounding to 5 cents
                // we need to make this a country paramter
                if (Convert.ToBoolean(Country[CountryParameterNames.ServiceChargeRounding]))
                    actual = Math.Round(Math.Ceiling(actual / 0.05m)) * 0.05m;

                if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint])
                {
                    actual = actual - insuranceCharge;
                }


                actual += saleOrderVoucherValue;
                if(actual < 0)
                {
                    actual = 0;
                }
                saleOrderDT.Attributes[Tags.UnitPrice].Value = StripCurrency(actual.ToString(DecimalPlaces));
                saleOrderDT.Attributes[Tags.Value].Value = StripCurrency(actual.ToString(DecimalPlaces));
                BItem i = new BItem();
                decimal cmDtTax = i.CalculateTaxAmount(saleOrderDT, taxExempt);

                // All result values must be rounded
                CountryRound(ref insuranceCharge);
                CountryRound(ref adminCharge);
                CountryRound(ref actual);

                if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                    cmDtTax = cmAdminTax = cmInsuranceTax = 0;

                decimal cmDeferredTerms = (actual + cmDtTax + insuranceCharge + adminCharge + cmInsuranceTax + cmAdminTax);

                CalculateInstalPlan(subTotal, deposit, cmDeferredTerms, (months - monthDiffDeferredIntFree) , out newMonthlyInstalment, out newFinalInstalment);

            }

            months = (months - monthDiffDeferredIntFree) - existingTerms;
            return months;

        }

        public decimal CalculateAmountToDeposite(SqlConnection conn, SqlTransaction trans,
                                                string countryCode, string termsType, string acctNo, string overrideBand, DateTime dateOpened,
                                                decimal installementAmount, decimal months, decimal subTotal, string accountType,
                                                decimal adminSubTotal,      /* may not include warranties */
                                                decimal insuranceCharge, decimal adminCharge, decimal depositeDefferedTermDiff,
                                                bool waiveAdminCharge = false, bool cashLoan = false)
        {

            Function = "BAgreement::CalculateAmountToDeposite()";
            //insuranceCharge = 0;
            decimal deposit = 0;

            if (!waiveAdminCharge)
            {
                adminCharge = 0;
            }

            decimal insPcent = 0;
            decimal adminPcent = 0;
            decimal servicePcent = 0;
            decimal adminValue = 0;
            int monthsIntFree = 0;
            int monthsDeferred = 0;
            int oldMonthsIntFree = 0;
            int oldMonthsDeferred = 0;

            // First get the termsType details
            // CR440 moved months int free and deferred from Acct Type to Terms Type
            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(conn, trans, countryCode, termsType, acctNo, overrideBand, dateOpened);

            // Old accounts will have old account types with their own months int free and deferred
            DAccount dAcct = new DAccount();
            dAcct.SelectType(conn, trans, accountType, countryCode, out oldMonthsIntFree, out oldMonthsDeferred);
            if (oldMonthsIntFree != 0 || oldMonthsDeferred != 0)
            {
                // Use the values from the old account type
                monthsIntFree = oldMonthsIntFree;
                monthsDeferred = oldMonthsDeferred;
            }
            else //GJ 10/8/05 - Only bother getting values from the DataTable if we aren't using the old values,
            //even then, we need to allow for DBNull values.
            {
                if (terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree] != DBNull.Value)
                {
                    monthsIntFree = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree]);
                }
                if (terms.TermsTypeDetails.Rows[0][CN.DeferredMonths] != DBNull.Value)
                {
                    monthsDeferred = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.DeferredMonths]);
                }
            }
            months += monthsDeferred - monthsIntFree;

            if (((string)Country[CountryParameterNames.AdminChargeItem]).Length > 0)
            {
                adminPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.AdminPcent]) / 100;
                adminValue = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["AdminValue"]);
            }

            if (((string)Country[CountryParameterNames.InsuranceChargeItem]).Length > 0)
                insPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.InsPcent]) / 100;

            if (Convert.ToBoolean(Country[CountryParameterNames.ServiceChargeRounding]))
                installementAmount = Math.Round(Math.Ceiling(installementAmount / 0.05m)) * 0.05m;


            if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint])
            {
                installementAmount = installementAmount + insuranceCharge;
            }

            terms.GetVariableRates(conn, trans, termsType, dateOpened);
            if (terms.VariableRates.Rows.Count == 0)
            {
                // there are no variable interest rates so we will use the
                // default service charge
                if (months > 0 && installementAmount < subTotal)
                {
                    servicePcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["servpcent"]) / 100;
                    //annual = (installementAmount * 12) / months;
                    decimal servicePcentForMonths = servicePcent * (months / 12);

                    //If Cash Loan exclude Admin Charge from Service Charge calculation
                    if (cashLoan)
                    {
                        deposit = (((subTotal - adminCharge) * servicePcentForMonths) + depositeDefferedTermDiff) / (1 + servicePcentForMonths);
                    }
                    else
                    {
                        //Calculate the annual service charge
                        //deposit = subTotal - (annual / servicePcent);
                        deposit = ((subTotal * servicePcentForMonths) + depositeDefferedTermDiff) / (1 + servicePcentForMonths);
                    }
                }
            }
            else
            {
                decimal servicePcentForMonths = 0;
                decimal variableMonths = 0;
                if (months > 0 && installementAmount < subTotal)
                {
                    foreach (DataRow r in terms.VariableRates.Rows)
                    {
                        if(Convert.ToInt32(r[CN.ToMonth]) <= months)
                        {
                            variableMonths = 1 + (Convert.ToInt32(r[CN.ToMonth]) - Convert.ToInt32(r[CN.FromMonth]));
                        }
                        else if(Convert.ToInt32(r[CN.FromMonth]) <= months)
                        {
                            variableMonths = 1 + (months - Convert.ToInt32(r[CN.FromMonth]));
                        }
                        
                        decimal rate = Convert.ToDecimal(r[CN.Rate]) / 100;
                        servicePcentForMonths += rate * (variableMonths / 12);
                    }
                    deposit = ((subTotal * servicePcentForMonths) + depositeDefferedTermDiff) / (1 + servicePcentForMonths);
                }
            }
            return deposit;
        }

            public decimal CalculateServiceCharge(SqlConnection conn, SqlTransaction trans,
            string countryCode,
            string termsType,
            string acctNo,
            string overrideBand,
            decimal deposit,
            decimal months,
            decimal subTotal,
            DateTime dateOpened,
            string accountType,
            decimal adminSubTotal,		/* may not include warranties */
            ref decimal insuranceCharge,
            ref decimal adminCharge,
            ref DataSet variableRatesSet,
            bool waiveAdminCharge = false,
            bool cashLoan = false)
        {
            Function = "BAgreement::CalculateServiceCharge()";
            insuranceCharge = 0;

            if (!waiveAdminCharge)
            {
                adminCharge = 0;
            }

            decimal insPcent = 0;
            decimal adminPcent = 0;
            decimal servicePcent = 0;
            decimal annual = 0;
            decimal actual = 0;
            decimal adminValue = 0;
            int monthsIntFree = 0;
            int monthsDeferred = 0;
            int oldMonthsIntFree = 0;
            int oldMonthsDeferred = 0;

            // First get the termsType details
            // CR440 moved months int free and deferred from Acct Type to Terms Type
            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(conn, trans, countryCode, termsType, acctNo, overrideBand, dateOpened);

            // Old accounts will have old account types with their own months int free and deferred
            DAccount dAcct = new DAccount();
            dAcct.SelectType(conn, trans, accountType, countryCode, out oldMonthsIntFree, out oldMonthsDeferred);
            if (oldMonthsIntFree != 0 || oldMonthsDeferred != 0)
            {
                // Use the values from the old account type
                monthsIntFree = oldMonthsIntFree;
                monthsDeferred = oldMonthsDeferred;
            }
            else //GJ 10/8/05 - Only bother getting values from the DataTable if we aren't using the old values,
            //even then, we need to allow for DBNull values.
            {
                if (terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree] != DBNull.Value)
                {
                    monthsIntFree = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree]);
                }
                if (terms.TermsTypeDetails.Rows[0][CN.DeferredMonths] != DBNull.Value)
                {
                    monthsDeferred = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.DeferredMonths]);
                }
            }
            months += monthsDeferred - monthsIntFree;

            if (((string)Country[CountryParameterNames.AdminChargeItem]).Length > 0)
            {
                adminPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.AdminPcent]) / 100;
                adminValue = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["AdminValue"]);
            }

            if (((string)Country[CountryParameterNames.InsuranceChargeItem]).Length > 0)
                insPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.InsPcent]) / 100;

            /* calculate the admin charge */
            if (!waiveAdminCharge ) //If Cash Loan Admin Charge waived from Cash Loan Application screen do not calculate...value used is passed in and already set in ref variable adminCharge
            {
                adminCharge = ((adminSubTotal - deposit) * adminPcent * (months / 12)) + adminValue ;   //admin value only for Cash Loan, will be 0 for regular Terms Types
            }

            /* calculate the insurance charge unless flag is set - ALLOW FOR NULL !*/
            if (terms.TermsTypeDetails.Rows[0][CN.InsIncluded] == DBNull.Value
                || !Convert.ToBoolean(terms.TermsTypeDetails.Rows[0][CN.InsIncluded]))
            {
                insuranceCharge = (subTotal - deposit) * insPcent * (months / 12);
            }

            //Service charge calculation for legacy cash loan accounts
            //if (!(bool)Country[CountryParameterNames.CL_Amortized])
            {
                /* admin and insurance charges must be added onto the cash price
                     * of the account before calculating DT 
                * Not including if Jamaica and having insurance included in Service Charge
                */
                if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint])
                {
                    subTotal += adminCharge; // only include admin charge
                }
                else
                {
                    subTotal += adminCharge + insuranceCharge;
                }


                terms.GetVariableRates(conn, trans, termsType, dateOpened);
                if (terms.VariableRates.Rows.Count == 0)
                {
                    // there are no variable interest rates so we will use the
                    // default service charge
                    if (months > 0 && deposit < subTotal)
                    {
                        servicePcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["servpcent"]) / 100;

                        //If Cash Loan exclude Admin Charge from Service Charge calculation
                        if (cashLoan)
                        {
                            annual = ((subTotal - adminCharge) - deposit) * servicePcent;
                        }
                        else
                        {
                            //Calculate the annual service charge
                            annual = (subTotal - deposit) * servicePcent;
                        }
                        //adjust for the duration in months
                        actual = annual * (months / 12);
                    }
                }
                else
                {
                    // variable interest rates exist for this terms type so we must
                    // calculate the service charge for each period

                    decimal interestCharge = 0;
                    decimal periodValue = 0;
                    decimal monthlyPayment = 0;
                    decimal previousPeriod = 0;
                    int order = 1;
                    DateTime dateFrom = DateTime.Now;

                    if (months > 0 && deposit < subTotal)
                    {
                        DataTable variableRates = new DataTable(TN.Rates);

                        variableRates.Columns.Add(new DataColumn(CN.AcctNo, System.Type.GetType("System.String")));
                        variableRates.Columns.Add(new DataColumn(CN.InstalOrder, System.Type.GetType("System.Int32")));
                        variableRates.Columns.Add(new DataColumn(CN.Instalment2, System.Type.GetType("System.Decimal")));
                        variableRates.Columns.Add(new DataColumn(CN.InstalmentNumber, System.Type.GetType("System.String")));
                        variableRates.Columns.Add(new DataColumn(CN.DateFrom, System.Type.GetType("System.DateTime")));
                        variableRates.Columns.Add(new DataColumn(CN.ServiceCharge, System.Type.GetType("System.Decimal")));

                        foreach (DataRow r in terms.VariableRates.Rows)
                        {
                            decimal variableMonths = 1 + (Convert.ToInt32(r[CN.ToMonth]) - Convert.ToInt32(r[CN.FromMonth]));

                            if (order == 1)
                                dateFrom = dateOpened.AddMonths(1);
                            else
                                dateFrom = dateFrom.AddMonths(Convert.ToInt32(previousPeriod + 1));

                            decimal rate = Convert.ToDecimal(r[CN.Rate]) / 100;

                            // calculate the annual service charge and then adjust for
                            // the duration in months
                            annual = (subTotal - deposit) * rate;
                            interestCharge = (annual * (months / 12)) / months;

                            // once the monthly interest charge has been calculated we
                            // need to calculate the charge for a particular period
                            // e.g. calculate interest charge for months 1-12 @ 13.2%
                            periodValue = interestCharge * variableMonths;

                            // calculate the monthly instalment the customer
                            // will have to pay for a particular period
                            monthlyPayment = ((subTotal - deposit) / months) + interestCharge;
                            monthlyPayment = Math.Round(monthlyPayment, 0);

                            while ((subTotal + interestCharge - deposit) - (monthlyPayment * (variableMonths - 1)) < 0)
                                monthlyPayment--;

                            if (order == terms.VariableRates.Rows.Count)
                                variableMonths--;

                            // save variable instalment details to a datatable so 
                            // that the user can view these details from the NewAccount
                            // screen
                            DataRow row = variableRates.NewRow();
                            row[CN.AcctNo] = "";
                            row[CN.InstalOrder] = order;
                            row[CN.Instalment2] = monthlyPayment;
                            row[CN.InstalmentNumber] = Convert.ToInt32(variableMonths);
                            row[CN.DateFrom] = dateFrom;
                            row[CN.ServiceCharge] = periodValue;
                            variableRates.Rows.Add(row);

                            // total up interest charges for each period to give us
                            // out total service charge figure
                            actual += periodValue;

                            order++;
                            previousPeriod = variableMonths;
                        }
                        variableRatesSet.Tables.Add(variableRates);
                    }
                }
            }

            //we need to round down the deferred terms at this point if
            //we're going to otherwise the instalments will be slightly too high
            //check to see if we need to round down terms
            if ((bool)Country[CountryParameterNames.NoCents])
            {
                  if (countryCode != "I" &&		/* round down to 0 decimal places */
                    countryCode != "C")
                {
                    actual = Convert.ToDecimal(Math.Floor(Convert.ToDouble(actual)));
                }
                else		/* round up to nearest 100 */
                {
                    actual /= 100;
                    actual = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(actual)));
                    actual *= 100;
                }
            }

            // malaysia require rounding to 5 cents
            // we need to make this a country paramter
            if (Convert.ToBoolean(Country[CountryParameterNames.ServiceChargeRounding]))
                actual = Math.Round(Math.Ceiling(actual / 0.05m)) * 0.05m;

            if ((bool)Country[CountryParameterNames.IncInsinServAgrPrint])
            {
                actual = actual - insuranceCharge;
            }
            // All result values must be rounded
            CountryRound(ref insuranceCharge);
            CountryRound(ref adminCharge);
            CountryRound(ref actual);
            return actual;
        }

        //Service charge calculation with instalment schedule for amortized cash loan accounts
        public decimal CalculateAmortizedInstalPlan(SqlConnection conn, SqlTransaction trans,
            string countryCode,
            string termsType,
            string acctNo,
            string overrideBand,
            decimal months,
            decimal subTotal,
            DateTime dateOpened,
            string accountType,
            ref decimal insuranceCharge,
            ref decimal adminCharge,
            out decimal monthly,
            out decimal final,
            bool waiveAdminCharge = false)
        {
            Function = "BAgreement::CalculateAmortizedInstalPlan()";

            insuranceCharge = 0;

            if (!waiveAdminCharge)
            {
                adminCharge = 0;
            }

            monthly = 0;
            final = 0;
            decimal actual = 0;
            decimal servicePcent = 0;
            decimal insPcent = 0;
            decimal adminPcent = 0;
            decimal adminValue = 0;
            int monthsIntFree = 0;
            int monthsDeferred = 0;
            int oldMonthsIntFree = 0;
            int oldMonthsDeferred = 0;

            DTermsType terms = new DTermsType();
            terms.GetTermsTypeDetail(conn, trans, countryCode, termsType, acctNo, overrideBand, dateOpened);

            DAccount dAcct = new DAccount();
            dAcct.SelectType(conn, trans, accountType, countryCode, out oldMonthsIntFree, out oldMonthsDeferred);
            if (oldMonthsIntFree != 0 || oldMonthsDeferred != 0)
            {
                monthsIntFree = oldMonthsIntFree;
                monthsDeferred = oldMonthsDeferred;
            }
            else 
            {
                if (terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree] != DBNull.Value)
                {
                    monthsIntFree = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree]);
                }
                if (terms.TermsTypeDetails.Rows[0][CN.DeferredMonths] != DBNull.Value)
                {
                    monthsDeferred = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.DeferredMonths]);
                }
            }
            months += monthsDeferred - monthsIntFree;

            if (((string)Country[CountryParameterNames.AdminChargeItem]).Length > 0)
            {
                adminPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.AdminPcent]) / 100;
                adminValue = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["AdminValue"]);
            }

            if (((string)Country[CountryParameterNames.InsuranceChargeItem]).Length > 0)
                insPcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0][CN.InsPcent]) / 100;

            /* calculate the admin charge */
            if (!waiveAdminCharge)    //kedar
            {
                if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                {
                    //adminCharge = (subTotal * adminPcent * (months / 12)) + adminValue * Convert.ToDecimal(Country[CountryParameterNames.TaxRate]) / 100;
                    adminCharge = (subTotal * adminPcent * (months / 12)) + adminValue; //* Convert.ToDecimal(Country[CountryParameterNames.TaxRate]) / 100;
                    adminCharge = adminCharge + (adminCharge * (Convert.ToDecimal(Country[CountryParameterNames.TaxRate]) / 100));

                }
                else
                {
                    adminCharge = (subTotal * adminPcent * (months / 12))+ adminValue;
                }
            }


            /* calculate the insurance charge unless flag is set - ALLOW FOR NULL !*/
            if (terms.TermsTypeDetails.Rows[0][CN.InsIncluded] == DBNull.Value
                || !Convert.ToBoolean(terms.TermsTypeDetails.Rows[0][CN.InsIncluded]))
            {
                insuranceCharge = subTotal * insPcent * (months / 12);
            }

            terms.GetVariableRates(conn, trans, termsType, dateOpened);
            if (terms.VariableRates.Rows.Count == 0)
            {
                if (months > 0)
                {
                    servicePcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["servpcent"]) / 100;
                    if (servicePcent > 0)
                    {
                        monthly = Math.Round((subTotal * (servicePcent / 12) * (decimal)Math.Pow(Convert.ToDouble(1 + (servicePcent / 12)), Convert.ToDouble(months))) /
                                    ((decimal)Math.Pow(Convert.ToDouble(1 + (servicePcent / 12)), Convert.ToDouble(months)) - 1), 2);
                        DAgreement agree = new DAgreement();
                        agree.CalculateAmortizedCashLoan(conn, trans, subTotal, servicePcent, months, monthly, out actual, out final);
                    }
                }
            }

            if ((bool)Country[CountryParameterNames.NoCents])
            {
                if (countryCode != "I" &&		/* round down to 0 decimal places */
                    countryCode != "C")
                {
                    actual = Convert.ToDecimal(Math.Floor(Convert.ToDouble(actual)));
                }
                else		/* round up to nearest 100 */
                {
                    actual /= 100;
                    actual = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(actual)));
                    actual *= 100;
                }
            }

            CountryRound(ref insuranceCharge);
            CountryRound(ref adminCharge);
            CountryRound(ref actual);
            return actual;
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            DAgreement agree = new DAgreement();
            agree.OrigBr = this.OrigBr;
            agree.AccountNumber = this.AccountNumber;
            agree.AgreementNumber = this.AgreementNumber;
            agree.AgreementDate = this.AgreementDate;
            agree.SalesPerson = this.SalesPerson;
            agree.DepositChequeClears = this.DepositChequeClears;
            agree.HoldMerch = this.HoldMerch;
            agree.HoldProp = this.HoldProp;
            agree.DateDel = this.DateDel;
            agree.DateNextDue = this.DateNextDue;
            agree.OldAgreementBalance = this.OldAgreementBalance;
            agree.CashPrice = this.CashPrice;
            agree.Discount = this.Discount;
            agree.PxAllowed = this.PxAllowed;
            agree.ServiceCharge = this.ServiceCharge;
            agree.SundryChargeTotal = this.SundryChargeTotal;
            agree.AgreementTotal = this.AgreementTotal;
            agree.Deposit = this.Deposit;
            agree.CODFlag = this.CODFlag;
            agree.SOA = this.SOA;
            agree.PayMethod = this.PayMethod;
            agree.UnpaidFlag = this.UnpaidFlag;
            agree.DeliveryFlag = this.DeliveryFlag;
            agree.FullDelFlag = this.FullDelFlag;
            agree.PaymentMethod = this.PaymentMethod;
            agree.EmployeeNumAuth = this.EmployeeNumAuth;
            agree.DateAuth = this.DateAuth;
            agree.EmployeeNumChange = this.EmployeeNumChange;
            agree.DateChange = this.DateChange;
            agree.CreatedBy = this.CreatedBy;
            agree.PaymentHolidays = this.PaymentHolidays;
            agree.AuditSource = this.AuditSource;
            agree.TaxFree = this.TaxFree;

            //BTransaction Trans = new BTransaction(); -- Causing locking issues so not doing.
            //Trans.GetByAcctNo(conn, trans, this.AccountNumber); //get payment total
            //Trans.User = User;
            //Trans.DACheck(conn, trans, string.Empty, Trans.PayTot, ref agree, 0); //

            agree.Save(conn, trans);
        }

        public void Populate(SqlConnection conn, SqlTransaction trans,
                                string accountNo, int agreementNo)
        {
            DAgreement agree = new DAgreement();

            this.AccountNumber = accountNo;
            this.AgreementNumber = agreementNo;

            if (agree.Populate(conn, trans, accountNo, agreementNo))
            {
                this.OrigBr = agree.OrigBr;
                this.AgreementDate = agree.AgreementDate;
                this.SalesPerson = agree.SalesPerson;
                this.DepositChequeClears = agree.DepositChequeClears;
                this.HoldMerch = agree.HoldMerch;
                this.HoldProp = agree.HoldProp;
                this.DateDel = agree.DateDel;
                this.DateNextDue = agree.DateNextDue;
                this.OldAgreementBalance = agree.OldAgreementBalance;
                this.CashPrice = agree.CashPrice;
                this.Discount = agree.Discount;
                this.PxAllowed = agree.PxAllowed;
                this.ServiceCharge = agree.ServiceCharge;
                this.SundryChargeTotal = agree.SundryChargeTotal;
                this.AgreementTotal = agree.AgreementTotal;
                this.Deposit = agree.Deposit;
                this.CODFlag = agree.CODFlag;
                this.SOA = agree.SOA;
                this.PayMethod = agree.PayMethod;
                this.UnpaidFlag = agree.UnpaidFlag;
                this.DeliveryFlag = agree.DeliveryFlag;
                this.FullDelFlag = agree.FullDelFlag;
                this.PaymentMethod = agree.PaymentMethod;
                this.EmployeeNumAuth = agree.EmployeeNumAuth;
                this.DateAuth = agree.DateAuth;
                this.EmployeeNumChange = agree.EmployeeNumChange;
                this.DateChange = agree.DateChange;
                this.CreatedBy = agree.CreatedBy;
                this.PaymentHolidays = agree.PaymentHolidays;
                this.TaxFree = agree.TaxFree;
            }
        }

        /// <summary>
        /// This method returns a dataset containing the agreement details 
        /// relating to a specific account
        /// </summary>
        /// <param name="countryCode">account number</param>
        /// <returns></returns>
        public DataSet GetAgreement(SqlConnection conn, SqlTransaction trans, string accountNumber, int ageementNumber, bool GR) //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
        {
            DataSet ds = new DataSet();
            DAccount acct = new DAccount();
            DAgreement agrmt = new DAgreement();
            DFinTrans finTrans = new DFinTrans();

            //69650 If this is being fired from the Goods Return screen then first check for whether the account is locked
            int locked = 0;
            if (GR)
            {
                locked = acct.CheckAccountLocked(accountNumber, this.User);
            }

            if (locked == 0)
            {
                acct.GetAccountDetails(conn, trans, accountNumber, ageementNumber); //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
                agrmt.GetAgreement(conn, trans, accountNumber, ageementNumber);

                
                //ds.Tables.Add(acct.AccountDetails);
                //if (accountNumber.Substring(3, 1) != "5" || Convert.ToString(ds.Tables[TN.AccountDetails].Rows[0][CN.TermsType])=="WC")         // #15186 get fin details if Warr on Cred
                if (accountNumber.Substring(3, 1) != "5" || Convert.ToString(acct.AccountDetails.Rows[0][CN.TermsType]) == "WC")
                {
                    finTrans.GetFinTransDetails(conn, trans, accountNumber); //IP - 11/02/11 - Sprint 5.10 - #2978 - Added conn, trans
                    foreach (DataRow row in finTrans.FinTrans.Rows)
                    {
                        decimal rebate = 0;
                        PaymentWebApi mambuAccount = new PaymentWebApi();
                        DataTable loanAccountIds = mambuAccount.GetMambuAccountsForPayment(null, accountNumber);
                        if (loanAccountIds.Rows.Count > 0)
                            acct.CalculateRebateForMambu(conn, trans, accountNumber, out rebate);
                        row[CN.Rebate] = rebate;
                    }
                    ds.Tables.Add(finTrans.FinTrans);
                }

                ds.Tables.Add(acct.AccountDetails);
                ds.Tables.Add(agrmt.AgreementList);

                if ((bool)Country[CountryParameterNames.CL_Amortized])
                {
                    acct.GetAmortizedScheduleDetails(conn, trans, accountNumber);
                    ds.Tables.Add(acct.AmortizedSchedule);
                }
            }
            else
            {
                ds.Tables.Add("Locked");
            }

            return ds;
        }

        //CR-2018-13 – Raj – 07 Dec 18 –  Pass  the parameter Invoice Number to Get Account Number 
        public string GetInvoiceAccountDetails(SqlConnection conn, SqlTransaction trans, string InvoiceNo ) //
        {
            DAgreement agrmt = new DAgreement();
            return agrmt.GetInvoiceAcctDetails(InvoiceNo);          
            
        }

        public bool isReprint(string acctno)
        {
            DAgreement d = new DAgreement();
            return d.isReprint(acctno);
        }

        public BAgreement()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
