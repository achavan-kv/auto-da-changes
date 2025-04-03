using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.StandingOrder;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BStorderProcess.
    /// </summary>
    public class BStorderProcess : CommonObject
    {
        private short _origBr = 0;
        public short OrigBr
        {
            get { return _origBr; }
            set { _origBr = value; }
        }
        private string _acctno = "";
        public string AccountNumber
        {
            get { return _acctno; }
            set { _acctno = value; }
        }
        private DateTime _datetrans = DateTime.Today;
        public DateTime DateTrans
        {
            get { return _datetrans; }
            set { _datetrans = value; }
        }
        private string _transcode = "";
        public string TransTypeCode
        {
            get { return _transcode; }
            set { _transcode = value; }
        }
        private short _branchno = 0;
        public short BranchNumber
        {
            get { return _branchno; }
            set { _branchno = value; }
        }
        private int _transno = 0;
        public int TransRefNo
        {
            get { return _transno; }
            set { _transno = value; }
        }
        private decimal _transvalue = 0;
        public decimal TransValue
        {
            get { return _transvalue; }
            set { _transvalue = value; }
        }
        private int _runno = 0;
        public int RunNumber
        {
            get { return _runno; }
            set { _runno = value; }
        }
        private int _empeeno = 0;
        public int EmployeeNumber
        {
            get { return _empeeno; }
            set { _empeeno = value; }
        }
        private string _transupdated = "";
        public string TransUpdated
        {
            get { return _transupdated; }
            set { _transupdated = value; }
        }
        private string _transprinted = "N";
        public string TransPrinted
        {
            get { return _transprinted; }
            set { _transprinted = value; }
        }
        private string _bankcode = "";
        public string BankCode
        {
            get { return _bankcode; }
            set { _bankcode = value; }
        }
        private string _bankacctno = "";
        public string BankAccountNumber
        {
            get { return _bankacctno; }
            set { _bankacctno = value; }
        }
        private string _chequeno = "";
        public string ChequeNumber
        {
            get { return _chequeno; }
            set { _chequeno = value; }
        }
        private string _ftnotes = "";
        public string FTNotes
        {
            get { return _ftnotes; }
            set { _ftnotes = value; }
        }
        private short _paymethod = 0;
        public short PayMethod
        {
            get { return _paymethod; }
            set { _paymethod = value; }
        }
        private string _source = "";
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        private string _error = "";
        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }
        private string _bankName = "";
        public string BankName
        {
            get { return _bankName; }
            set { _bankName = value; }
        }
        public BStorderProcess()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Returns the Standing Order transactions for a RunNo
        /// 
        /// </summary>
        /// <param name="runNo"></param>
        /// <returns></returns>
        /// 
        public DataSet GetByRunNo(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            DataSet ds = new DataSet();
            DStorderProcess SOTrans = new DStorderProcess();

            SOTrans.GetByRunNo(conn, trans, runNo);
            ds.Tables.Add(SOTrans.StorderProcess);

            DAccount acct = new DAccount();
            BPayment bp = new BPayment();
            BBranch br = new BBranch();
            int empeeno = 0;
            decimal collectionFee = 0;
            decimal bailiffFee = 0;
            decimal transValue;
            int debitAccount = 0;
            int segmentId = 0;

            short branchNo = Convert.ToInt16(Country[CountryParameterNames.HOBranchNo]);
            bool generateFee = Convert.ToBoolean(Country[CountryParameterNames.genFeeStandingOrder]);       //IP - 16/09/10 - CR1092 - COASTER to COSACS Enhancements

            string sundryAcct = acct.GetSundryCreditAccount(conn, trans, branchNo);
            string bdwRecoverAcct = string.Empty;   //IP - 16/08/10 - CR1092 

            // Process table and add transactions to FinTrans if 
            //	standing order transaction not flagged as error

            foreach (DataRow row in SOTrans.StorderProcess.Rows)
            {
                if ((string)row[CN.Error] != SOstatus.AccountNotExists)         //IP - 24/01/13 - #11729 - LW75565
                {
                    //IP/JC - 03/04/12 - #9665
                    row[CN.Rebate] = acct.CalculateRebate(conn, trans, (string)row[CN.AcctNo]);
                    row[CN.SettlementFigure] = Convert.ToDecimal(row[CN.SettlementFigure]) - Convert.ToDecimal(row[CN.Rebate]);
                }

                if (Convert.ToBoolean(row[CN.IsInterest]) == false)
                {

                    if (generateFee)    //IP - 16/09/10 - CR1092 - Generate FEEs for payments if Country Parameter is TRUE.
                    {
                        //IP - 23/08/10 - CR1092 - COASTER to CoSACS Enhancements - Need to calculate the Credit Fee before processing payment.
                        if ((string)row[CN.Error] != SOstatus.Error && (string)row[CN.Error] != SOstatus.WrittenOff && (string)row[CN.Error] != SOstatus.AccountNotExists
                            && (string)row[CN.Error] != SOstatus.Settled)
                        {

                            acct.Populate(conn, trans, (string)row[CN.AcctNo]);

                            transValue = Math.Abs(Convert.ToDecimal(row[CN.TransValue]));

                            bp.CalculateCreditFee(conn, trans, Convert.ToString(Country[CountryParameterNames.CountryCode]), Convert.ToString(row[CN.AcctNo]),
                                   ref transValue, TransType.Payment, ref empeeno, acct.Arrears, out collectionFee, out bailiffFee, out debitAccount, out segmentId);

                            //If a collection fee has been calculated for the account
                            if (collectionFee > 0)
                            {
                                BranchNumber = branchNo;
                                AccountNumber = (string)row[CN.AcctNo];
                                TransRefNo = br.GetTransRefNo(conn, trans, branchNo);
                                DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                                TransTypeCode = TransType.CreditFee;
                                EmployeeNumber = 99999;
                                TransValue = collectionFee;
                                BankCode = "";
                                BankAccountNumber = "";
                                ChequeNumber = "";
                                FTNotes = " ";
                                PayMethod = 0;
                                RunNumber = runNo;
                                Source = "Storder";
                                BTransaction AddTrans = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                                TransRefNo, TransValue, EmployeeNumber, //IP - 16/08/10 - CR1092 - Chnaged to use EmployeeNumber from this.User
                                TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                                DateTrans, FTNotes, 0);

                            }

                        }
                    }

                    if ((string)row[CN.Error] != SOstatus.Error && (string)row[CN.Error] != SOstatus.WrittenOff && (string)row[CN.Error] != SOstatus.AccountNotExists)
                    {
                        //OrigBr=999;
                        BranchNumber = branchNo; //IP - 16/08/10 - CR1092 - Changed to use branchNo
                        AccountNumber = (string)row[CN.AcctNo];
                        TransRefNo = (int)row[CN.TransRefNo];
                        DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                        TransTypeCode = TransType.Payment;
                        EmployeeNumber = 99999;
                        TransValue = (decimal)row[CN.TransValue];

                        BankCode = "";
                        BankAccountNumber = "";
                        ChequeNumber = "";
                        FTNotes = " ";
                        PayMethod = (short)row[CN.Paymentmethod];
                        RunNumber = runNo;
                        Source = "Storder";
                        BTransaction AddTrans = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                            TransRefNo, TransValue, EmployeeNumber, //IP - 16/08/10 - CR1092 - Chnaged to use EmployeeNumber from this.User
                            TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTrans, FTNotes, 0);

                        //RD - 05/07/2019 - #CLA Outstanding - deduct amount from payment history to calculate Outstanding balance as per new Rule

                        bp.DeductAmountFromPaymentHistory(conn, trans, AccountNumber, TransValue);

                        acct.GetAccount(AccountNumber);
                        //IP - 06/03/12 - #9665 - LW74273 - Post any rebate that may be due for paying >= the Settlement figure.
                        if (Math.Abs((decimal)row[CN.TransValue]) >= (decimal)row[CN.SettlementFigure] && (decimal)row[CN.SettlementFigure] > 0 && (decimal)row[CN.Rebate] > 0 && acct.IsAmortized == false && acct.IsAmortizedOutStandingBal == false)   //RD - 05/07/2019 - #CLA Outstanding - Rebate is Not applicable for CLA outstanding accounts.
                        {
                            BranchNumber = branchNo;
                            AccountNumber = (string)row[CN.AcctNo];
                            TransRefNo = br.GetTransRefNo(conn, trans, branchNo);
                            DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                            TransTypeCode = TransType.Rebate;
                            EmployeeNumber = 99999;
                            TransValue = (decimal)row[CN.Rebate] * -1;
                            BankCode = "";
                            BankAccountNumber = "";
                            ChequeNumber = "";
                            FTNotes = " ";
                            PayMethod = 0;
                            RunNumber = runNo;
                            Source = "Storder";

                            BTransaction AddReb = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                            TransRefNo, TransValue, EmployeeNumber, //IP - 16/08/10 - CR1092 - Chnaged to use EmployeeNumber from this.User
                            TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTrans, FTNotes, 0);
                        }
                    }
                    else if (((string)row[CN.Error] == SOstatus.WrittenOff))  //IP - 16/08/10 - CR1092 - Post transactions to BDW Recover account for accounts settled with BDW Balance
                    {
                        acct.Populate(conn, trans, (string)row[CN.AcctNo]);

                        //Retrieve the BDW Recover account to post the transaction to for this account
                        bdwRecoverAcct = acct.GetBadDebtWriteOffAccount(conn, trans, acct.Securitised, Convert.ToInt16(((string)row[CN.AcctNo]).Substring(0, 3)));

                        BranchNumber = branchNo;
                        AccountNumber = bdwRecoverAcct;
                        TransRefNo = (int)row[CN.TransRefNo];
                        DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                        TransTypeCode = TransType.DebtPayment;
                        EmployeeNumber = 99999;
                        TransValue = (decimal)row[CN.TransValue];
                        BankCode = "";
                        BankAccountNumber = "";
                        ChequeNumber = "";
                        FTNotes = " ";
                        PayMethod = (short)row[CN.Paymentmethod];
                        RunNumber = runNo;
                        Source = "Storder";
                        BTransaction AddTrans = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                            TransRefNo, TransValue, EmployeeNumber, //IP - 16/08/10 - CR1092 - Changed to use EmployeeNumber from this.User
                            TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTrans, FTNotes, 0);

                        //IP - 17/08/10 - CR1092 - Update the BDW Balance and BDW Charges
                        acct.BDWBalance += TransValue;
                        if (acct.BDWBalance < 0)
                        {
                            acct.BDWCharges += acct.BDWBalance;
                            acct.BDWBalance = 0;
                            if (acct.BDWCharges < 0)
                                acct.BDWCharges = 0;
                        }
                        acct.Save(conn, trans);

                        //Save a link to the FintransAccount table
                        DFintransAccount FALink = new DFintransAccount(bdwRecoverAcct,
                                    (string)row[CN.AcctNo], DateTrans, BranchNumber, TransRefNo);
                        FALink.SaveAccountLink(conn, trans);
                    }
                    else if (((string)row[CN.Error] == SOstatus.AccountNotExists))  //IP - 16/08/10 - CR1092 - Post transactions to sundry for accounts that do not exist
                    {
                        //IP - 16/08/10 - CR1092 -  If the account does not exist then we need to transfer the amount to the Sundry Credit account.   
                        BranchNumber = branchNo;
                        AccountNumber = sundryAcct;
                        TransRefNo = (int)row[CN.TransRefNo];
                        DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                        TransTypeCode = TransType.SundryCreditTransfer;
                        EmployeeNumber = 99999;
                        TransValue = (decimal)row[CN.TransValue];
                        BankCode = "";
                        BankAccountNumber = "";
                        ChequeNumber = (string)row[CN.AcctNo];
                        FTNotes = " ";
                        PayMethod = (short)row[CN.Paymentmethod];
                        RunNumber = runNo;
                        Source = "Storder";
                        BTransaction AddTrans = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                            TransRefNo, TransValue, EmployeeNumber,
                            TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTrans, FTNotes, 0);
                    }

                }
                else //IP - 06/09/10 - CR1112 - Tallyman Interest Charges - Post interest transactions on each account that has not been marked with an error.
                {
                    if ((string)row[CN.Error] != SOstatus.Error)
                    {

                        BranchNumber = branchNo;
                        AccountNumber = (string)row[CN.AcctNo];
                        TransRefNo = (int)row[CN.TransRefNo];
                        DateTrans = Convert.ToDateTime(row[CN.DateTrans]);
                        TransTypeCode = TransType.Interest;
                        EmployeeNumber = 99999;
                        TransValue = (decimal)row[CN.TransValue];
                        BankCode = "";
                        BankAccountNumber = "";
                        ChequeNumber = "";
                        FTNotes = " ";
                        PayMethod = (short)row[CN.Paymentmethod];
                        RunNumber = runNo;
                        Source = "Storder";
                        BTransaction AddTrans = new BTransaction(conn, trans, AccountNumber, BranchNumber,
                            TransRefNo, TransValue, EmployeeNumber, //IP - 16/08/10 - CR1092 - Chnaged to use EmployeeNumber from this.User
                            TransTypeCode, BankCode, BankAccountNumber, ChequeNumber, PayMethod, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTrans, FTNotes, 0);
                    }
                }

            }  // End of foreach

            return ds;
        }
        /// <summary>
        /// Renames the raw Standing Order data files
        /// </summary>
        /// <param name=></param>
        /// <returns></returns>
        /// 
        public void RenameFiles(int runNo) //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements - Added runNo
        {
            DStorderProcess so = new DStorderProcess();
            so.RenameFiles(runNo);
        }

    }
}
