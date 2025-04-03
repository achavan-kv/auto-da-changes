using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using STL.Common.Constants.Giro;
using STL.Common.Constants.FTransaction;
using STL.BLL;
using STL.DAL;



namespace STL.Batch
{
    /// <summary>
    /// Summary description for DDGiro.
    /// </summary>
    public class DDGiro : CommonObject
    {
        //private Logging log;
        private new int _user = 0;
        public int user
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _countrycode = "";
        public string countryCode
        {
            get { return _countrycode; }
            set { _countrycode = value; }
        }

        private int _runNo = 0;
        public int runNo
        {
            get { return _runNo; }
            set { _runNo = value; }
        }

        private string _interfaceName = "";
        public string interfaceName
        {
            get { return _interfaceName; }
            set { _interfaceName = value; }
        }


        public DDGiro(int curUser, string curCountryCode, string curInterfaceName, int curRunNo)
        {
            this.User = curUser;
            this.countryCode = curCountryCode;
            this.interfaceName = curInterfaceName;
            this.runNo = curRunNo;
        }


        public string DDEOD_Daily()
        {
            // Start each mandate with an Approval Date and a Date Delivered
            string confirmResult = this.ConfirmMandates();
            // Cancel mandates not approved or with a cancelled rejection
            string cancelResult = this.CancelMandates();

            BAccount bar = new BAccount();
            bar.LettersGenerateCSVfiles(runNo, "Giro");

            if (confirmResult == EODResult.Fail || cancelResult == EODResult.Fail)
                return EODResult.Fail;
            else
                return EODResult.Pass;
        }


        public string DDEOD_Payment()
        {
            // Instantiate the BDDMandate class to work out the next effective date
            // as today + lead time, the next due date and retrieve the minimum period of grace.
            //
            BDDMandate giroDates = new BDDMandate(this.countryCode, System.DateTime.Today);
            BDDPayment ddPayment = new BDDPayment();

            // Check Giro has not disabled Payment processing
            string enableMsg = "";
            if (ddPayment.PaymentsEnabled(giroDates, out enableMsg))
            {
                // Create the Represent bank file
                string representResult = this.CreateRepresentFile(giroDates);

                // Create the Fee bank file
                string feeResult = this.CreateFeeFile(giroDates);

                // Create the Extra Payment bank file
                string extraResult = this.CreateExtraFile(giroDates);

                // Add new due payments to the DDPAYMENT table 
                string normalResult = this.SubmitPayments(giroDates);

                if (normalResult == EODResult.Pass)
                {
                    // Copy the new due payments to the Normal Payment bank file
                    normalResult = this.CreateNormalFile(giroDates);
                }

                if (representResult == EODResult.Fail || feeResult == EODResult.Fail ||
                    extraResult == EODResult.Fail || normalResult == EODResult.Fail)
                    return EODResult.Fail;
                else
                    return EODResult.Pass;
            }
            else
            {
                // Pass the enable msg on for logging in EODConfiguration
                throw new STLException(enableMsg);
            }
        }


        public string DDEOD_Rejection()
        {
            // Instantiate the BDDMandate class to work out the next effective date
            // as today + lead time, the next due date and retrieve the minimum period of grace.
            //
            BDDMandate giroDates = new BDDMandate(this.countryCode, System.DateTime.Today);
            BDDRejection ddRejection = new BDDRejection();

            // Check Giro has not disabled Rejection processing
            string enableMsg = "";
            if (ddRejection.RejectionsEnabled(giroDates, out enableMsg))
            {
                // Complete transactions that have not been rejected 
                string applyResult = this.ApplyTransactions(giroDates);

                // Cancel mandates that have now exceeded max rejections
                string cancelResult = this.CancelMandates();

                if (applyResult == EODResult.Fail || cancelResult == EODResult.Fail)
                    return EODResult.Fail;
                else
                    return EODResult.Pass;
            }
            else
            {
                // Pass the enable msg on for logging in EODConfiguration
                throw new STLException(enableMsg);
            }
        }


        public string CancelMandates()
        {
            // Cancel mandates not approved or with a cancelled rejection
            string progress = "Cancelling DD Giro Mandates ...";
            Console.WriteLine(progress);

            string eodResult = EODResult.Pass;
            //SqlConnection conn = null;
            //SqlTransaction trans = null;

            try
            {
                using (var conn = new SqlConnection(Connections.Default))
                {
                    retries = 0;
                    do
                    {
                        try
                        {
                            conn.Open();
                            var trans = conn.BeginTransaction();
                            var ddMandate = new BDDMandate(this.countryCode, System.DateTime.Today);
                            ddMandate.CancelMandates(conn, trans);
                            trans.Commit();
                            break;
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == Deadlock && retries < maxRetries)
                            {
                                retries++;
                                if (conn.State != ConnectionState.Closed)
                                    conn.Close();
                            }
                            else
                                throw ex;
                        }
                    } while (retries <= maxRetries);
                }
            }
            catch (Exception)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw;
            }
            //finally
            //{
            //    if (conn.State != ConnectionState.Closed)
            //    {
            //        conn.Close();
            //    }
            //}

            progress = "Finished cancelling DD Giro Mandates.";
            Console.WriteLine(progress);
            return eodResult;
        }


        public string ConfirmMandates()
        {
            // Start each mandate with an Approval Date and a Date Delivered
            string progress = "Confirming DD Giro Mandates that are ready to start ...";
            Console.WriteLine(progress);

            string eodResult = EODResult.Pass;
            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                DataSet confirmMandateSet = null;
                BDDMandate ddMandate = new BDDMandate(this.countryCode, System.DateTime.Today);
                conn = new SqlConnection(Connections.Default);

                // Retrieve a list of approved mandates with a delivery date that are ready to start
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        confirmMandateSet = ddMandate.ConfirmMandateList(conn, trans);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed) conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
                conn.Close();


                // Set the start date for each account and adjust date first where necessary
                foreach (DataTable confirmMandateList in confirmMandateSet.Tables)
                {
                    foreach (DataRow mandateRow in confirmMandateList.Rows)
                    {
                        // The retry loop is per account so that if one account retries
                        // we don't have to start all over again
                        retries = 0;
                        do
                        {
                            try
                            {
                                conn.Open();
                                trans = conn.BeginTransaction();
                                ddMandate.ConfirmMandate(conn, trans,
                                    (string)mandateRow[CN.AccountNumber], (int)mandateRow[CN.MandateId],
                                    (DateTime)mandateRow[CN.ApprovalDate], (short)mandateRow[CN.DueDay]);
                                trans.Commit();
                                break;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == Deadlock && retries < maxRetries)
                                {
                                    retries++;
                                    if (conn.State != ConnectionState.Closed) conn.Close();
                                }
                                else
                                    throw ex;
                            }
                        } while (retries <= maxRetries);
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Finished confirming DD Giro Mandates that are ready to start.";
            Console.WriteLine(progress);
            return eodResult;
        }


        public string CreateExtraFile(BDDMandate giroDates)
        {
            // Copy due Extra payments from the DDPAYMENT table to file
            string eodResult = EODResult.Pass;
            string progress = "Creating Extra Payments file ...";
            Console.WriteLine(progress);

            eodResult = this.CreateFile(
                giroDates,
                DDPaymentType.Extra,
                DDFileName.Extra,
                DDFileSuffix.Extra,
                DDErrorFileName.Extra,
                DDRecordLength.LineLengthPay);

            progress = "Finished creating Extra Payments file.";
            Console.WriteLine(progress);

            return eodResult;
        }


        public string CreateNormalFile(BDDMandate giroDates)
        {
            // Copy due Normal payments from the DDPAYMENT table to file
            string eodResult = EODResult.Pass;
            string progress = "Creating Normal Payments file ...";
            Console.WriteLine(progress);

            eodResult = this.CreateFile(
                giroDates,
                DDPaymentType.Normal,
                DDFileName.Normal,
                DDFileSuffix.Normal,
                DDErrorFileName.Normal,
                DDRecordLength.LineLengthPay);

            progress = "Finished creating Normal Payments file.";
            Console.WriteLine(progress);

            return eodResult;
        }


        public string CreateFeeFile(BDDMandate giroDates)
        {
            // Copy due fees from the DDPAYMENT table to file
            string eodResult = EODResult.Pass;
            string progress = "Creating Fee file ...";
            Console.WriteLine(progress);

            eodResult = this.CreateFile(
                giroDates,
                DDPaymentType.Fee,
                DDFileName.Fee,
                DDFileSuffix.Fee,
                DDErrorFileName.Fee,
                DDRecordLength.LineLengthPay);

            progress = "Finished creating Fee file.";
            Console.WriteLine(progress);

            return eodResult;
        }


        public string CreateRepresentFile(BDDMandate giroDates)
        {
            // Copy due representations from the DDPAYMENT table to file
            string eodResult = EODResult.Pass;
            string progress = "Creating Represenation file ...";
            Console.WriteLine(progress);

            eodResult = this.CreateFile(
                giroDates,
                DDPaymentType.Represent,
                DDFileName.Represent,
                DDFileSuffix.Represent,
                DDErrorFileName.Represent,
                DDRecordLength.LineLengthPay);

            progress = "Finished creating Represenation file.";
            Console.WriteLine(progress);

            return eodResult;
        }


        public string CreateFile(
            BDDMandate giroDates,
            string paymentType,
            string fileName,
            string fileSuffix,
            string msgName,
            int lineLength)
        {
            // Copy payments due to a bank file
            string eodResult = EODResult.Pass;
            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();

                        BDDPayment ddPayment = new BDDPayment();
                        ddPayment.CreateFile(conn, trans,
                            interfaceName,
                            runNo,
                            giroDates,
                            paymentType,
                            fileName,
                            fileSuffix,
                            msgName,
                            lineLength);

                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return eodResult;
        }


        public string SubmitPayments(BDDMandate giroDates)
        {
            // Add new payments due to the DDPAYMENT table
            string progress = "Adding new payments ready to submit to the bank ...";
            Console.WriteLine(progress);

            string eodResult = EODResult.Pass;
            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                /* Delete old payment entries */
                this.TidyPayments();

                conn = new SqlConnection(Connections.Default);
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        BDDPayment ddPayment = new BDDPayment();
                        // Create new payments now due
                        ddPayment.SubmitPayments(conn, trans, giroDates);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Finished adding new payments ready to submit to the bank.";
            Console.WriteLine(progress);
            return eodResult;
        }


        public string TidyPayments()
        {
            /* Delete data over two months old from the DDPayment table */
            string progress = "Deleting giro data over two months old ...";
            Console.WriteLine(progress);

            string eodResult = EODResult.Pass;
            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        BDDPayment ddPayment = new BDDPayment();
                        ddPayment.TidyPayments(conn, trans);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Finished deleting giro data over two months old.";
            Console.WriteLine(progress);
            return eodResult;
        }


        public string ApplyTransactions(BDDMandate giroDates)
        {
            /* Create payment transactions for payments not rejected */
            // This is split into separate proceeses with their own retry loops
            // because each process can be committed as its own transaction and
            // will not have to be repeated if a subsequent process retries.
            string progress = "Processing rejections file ...";
            Console.WriteLine(progress);

            string eodResult = EODResult.Pass;
            SqlConnection conn = null;
            SqlTransaction trans = null;

            try
            {
                BDDRejection ddRejection = new BDDRejection();
                conn = new SqlConnection(Connections.Default);

                retries = 0;
                do
                {
                    progress = "Loading rejections file ...";
                    Console.WriteLine(progress);

                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        ddRejection.LoadRejections(conn, trans, giroDates);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed) conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
                conn.Close();

                retries = 0;
                do
                {
                    progress = "Logging rejection records that do not match any payments ...";
                    Console.WriteLine(progress);

                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        ddRejection.LogUnmatchedRejections(conn, trans, this.interfaceName, this.runNo);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
                conn.Close();

                retries = 0;
                do
                {
                    progress = "Updating payments that have been rejected ...";
                    Console.WriteLine(progress);

                    try
                    {
                        conn.Open();
                        trans = conn.BeginTransaction();
                        ddRejection.RejectPayments(conn, trans, giroDates);
                        trans.Commit();
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
                conn.Close();


                progress = "Adding new rejection fees to accounts ...";
                Console.WriteLine(progress);

                // Retrieve the fees on hold waiting to create their financial transaction
                conn.Open();
                DataTable feeOnHoldList = ddRejection.FeeOnHoldList(conn, trans);
                conn.Close();

                foreach (DataRow feeRow in feeOnHoldList.Rows)
                {
                    try
                    {
                        retries = 0;
                        do
                        {
                            try
                            {
                                conn.Open();
                                trans = conn.BeginTransaction();

                                BTransaction feeTrans = new BTransaction(conn, trans,
                                    (string)feeRow[CN.AccountNumber],
                                    Convert.ToInt16(((string)feeRow[CN.AccountNumber]).Substring(0, 3)),
                                    0,
                                    (decimal)feeRow[CN.Amount],
                                    this.User,
                                    TransType.GiroFeeRaised,
                                    (string)feeRow[CN.BankCode],
                                    (string)feeRow[CN.BankAccountNo2],
                                    "",
                                    PayMethod.CreditCard,
                                    this.countryCode,
                                    giroDates.today,
                                    "GIRO", 0);

                                // Take the fee payment off hold
                                ddRejection.FeeOffHold(conn, trans, (int)feeRow[CN.PaymentId]);

                                trans.Commit();
                                break;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == Deadlock && retries < maxRetries)
                                {
                                    retries++;
                                    if (conn.State != ConnectionState.Closed)
                                        conn.Close();
                                }
                                else
                                    throw ex;
                            }
                        } while (retries <= maxRetries);
                    }
                    catch (Exception)
                    {
                        // Continue to process the other accounts
                        // GIRO TODO : but log a warning for this account
                    }
                    conn.Close();
                }


                progress = "Adding credit transactions to accounts ...";
                Console.WriteLine(progress);

                // Retrieve credit transactions for all payments that were submitted before 
                // (today - lead time) and have not been rejected.
                conn.Open();
                DataTable creditList = ddRejection.CreditList(conn, trans, giroDates);
                conn.Close();

                foreach (DataRow creditRow in creditList.Rows)
                {
                    try
                    {
                        retries = 0;
                        do
                        {
                            try
                            {
                                conn.Open();
                                trans = conn.BeginTransaction();

                                string transactionType = "";
                                switch ((string)creditRow[CN.PaymentType])
                                {
                                    case DDPaymentType.Extra:
                                        transactionType = TransType.GiroExtra;
                                        break;
                                    case DDPaymentType.Normal:
                                        transactionType = TransType.GiroNormal;
                                        break;
                                    case DDPaymentType.Represent:
                                        transactionType = TransType.GiroRepresent;
                                        break;
                                    case DDPaymentType.Fee:
                                        transactionType = TransType.GiroFeePaid;
                                        break;
                                    default :
                                        transactionType = TransType.GiroNormal;
                                        break;
                                }

                                BTransaction creditTrans = new BTransaction(conn, trans,
                                    (string)creditRow[CN.AccountNumber],
                                    Convert.ToInt16(((string)creditRow[CN.AccountNumber]).Substring(0,3)),
                                    0,
                                    -(decimal)creditRow[CN.Amount],
                                    this.User,
                                    transactionType,
                                    (string)creditRow[CN.BankCode],
                                    (string)creditRow[CN.BankAccountNo2],
                                    "",
                                    PayMethod.CreditCard,
                                    this.countryCode,
                                    giroDates.today,
                                    "GIRO", 0);

                                // Mark the DD payment as complete
                                ddRejection.CompletePayment(conn, trans,
                                    (int)creditRow[CN.PaymentId],
                                    (string)creditRow[CN.PaymentType],
                                    (string)creditRow[CN.AccountNumber]);

                                trans.Commit();
                                break;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == Deadlock && retries < maxRetries)
                                {
                                    retries++;
                                    if (conn.State != ConnectionState.Closed)
                                        conn.Close();
                                }
                                else
                                    throw ex;
                            }
                        } while (retries <= maxRetries);
                    }
                    catch (Exception)
                    {
                        // Continue to process the other accounts
                        // GIRO TODO : but log a warning for this account
                    }
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                eodResult = EODResult.Fail;
                // Pass this on for logging in EODConfiguration
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            progress = "Finished processing rejections file.";
            Console.WriteLine(progress);
            return eodResult;
        }

    }
}
