using Blue.Cosacs.Shared;
using STL.BLL;
using STL.Common.Constants.AuditSource;
using STL.Common.Static;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Repository
{
    public class TransactionRepository
    {
        public ValidateUserAccountsModel GetUserAccounts(string CustId)
        {
            var ds = new DataSet();
            var CT = new CreditTransaction();
            var objUserAccounts = new ValidateUserAccountsModel();
            string message = CT.GetUserAccounts(ds, CustId);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<DueDates> dueDates = new List<DueDates>();
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[1] != null && ds.Tables[1].Rows != null && ds.Tables[1].Rows.Count > 0)
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    dueDates = ds.Tables[1].Rows.OfType<DataRow>()
                   .Select(p => new DueDates()
                   {
                       amount = p["amount"] != null ? Convert.ToDecimal(p["amount"]) : Convert.ToDecimal(0),
                       date = p["date"] != null ? Convert.ToInt64(Convert.ToDateTime(p["date"]).Subtract(epoch).TotalMilliseconds) : 0,
                       days = p["days"] != null ? Convert.ToInt32(p["days"]) : 0,
                       invoiceNumber = p["invoiceNumber"] != null ? Convert.ToString(p["invoiceNumber"]) : string.Empty,
                       status = p["status"] != null ? Convert.ToString(p["status"]).Trim() : string.Empty,
                       totalRemainingAmount = p["totalRemainingAmount"] != null ? Convert.ToDecimal(p["totalRemainingAmount"]) : Convert.ToDecimal(0)
                   }).ToList();
                }

                objUserAccounts = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ValidateUserAccountsModel()
                    {
                        _UserAccountsModel = new UserAccountsModel
                        {
                            dueDates = dueDates.ToArray(),
                            extUId = ds.Tables[0].Rows[0]["CustId"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["CustId"]) : string.Empty,
                            totalCreditDue = ds.Tables[0].Rows[0]["TotalCreditDue"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["TotalCreditDue"]) : string.Empty,
                            totalCreditLine = ds.Tables[0].Rows[0]["CreditLimit"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["CreditLimit"]) : string.Empty,
                            monthlyDue = ds.Tables[0].Rows[0]["MonthlyDue"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["MonthlyDue"]) : string.Empty,
                            amountDueNow = ds.Tables[0].Rows[0]["AmountDueNow"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["AmountDueNow"]) : string.Empty,
                            monthlyPaymentCapacity = ds.Tables[0].Rows[0]["monthlyPaymentCapacity"] != null ? Convert.ToString(ds.Tables[0].Rows[0]["monthlyPaymentCapacity"]) : string.Empty
                        },

                        Message = message
                    })
                    .FirstOrDefault();
            }
            else
            {
                objUserAccounts = new ValidateUserAccountsModel
                {
                    Message = message,
                    _UserAccountsModel = null
                };
            }
            return objUserAccounts;
        }


        public CreditApp CreateRFAccount(string countryCode, short branchNo, string customerID, int user, bool isLoan,
                                      ref CashLoanDetails det, out bool rescore,
                                      out bool reOpenS1, out bool reOpenS2, out string err)
        {
            CreditApp objCrApp = new CreditApp();
            SqlConnection conn = null;
            rescore = false;
            reOpenS1 = false;
            reOpenS2 = false;
            err = "";



            BAccount acct = null;
            string accountNo = "";
            string auditSource = string.Empty;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    acct = new BAccount();
                    acct.CustomerID = customerID;
                    acct.User = Credential.UserId;

                    if (det == null || (det != null && det.accountNo == "000000000000"))
                    {
                        accountNo = acct.CreateRFAccount(conn, trans, countryCode, branchNo, customerID, user, isLoan, out rescore);


                        reOpenS1 = acct.ReOpenS1;
                        reOpenS2 = acct.ReOpenS2;

                        if (accountNo.Length == 0)
                        {
                            err = "Unable to create RF account.";
                        }
                    }

                    if (det != null)
                    {
                        var dateprop = new DateTime();
                        var unclearStage = "";
                        var propResult = "";
                        var points = 0;
                        var newaccountNo = "";

                        if (det.accountNo == "000000000000")
                        {
                            auditSource = AS.NewAccount;
                            det.accountNo = accountNo.Replace("-", "");
                        }
                        else
                        {
                            auditSource = AS.Revise;
                            det.accountNo = det.accountNo.Replace("-", "");
                            accountNo = det.accountNo;
                        }

                        DProposal pr = new DProposal();
                        pr.GetUnclearedStage(conn, trans, det.accountNo, ref newaccountNo, ref unclearStage, ref dateprop, ref propResult, ref points);
                        det.unclearStage = unclearStage;
                        det.dateprop = dateprop;



                        BAccount ba = new BAccount();
                        ba.User = Credential.UserId;

                        ba.InsertCashLoanItem(conn, trans, det, auditSource, countryCode, branchNo);

                        if (det.loanStatus == "R")
                        {
                            DProposalFlag pf = new DProposalFlag();
                            pf.EmployeeNoFlag = Credential.UserId;
                            pf.CustomerID = det.custId;
                            pf.DateProp = dateprop;
                            pf.DateCleared = DateTime.MinValue.AddYears(1899);
                            pf.CheckType = "R";
                            pf.Save(conn, trans, det.accountNo);
                        }
                    }

                    trans.Commit();
                }

            }

            objCrApp.accountNumber = accountNo;

            return objCrApp;
        }

        public CreditAppQuestion GetCreditAppQuestions(string CustId)
        {

            CreditAppQuestion objCreditAppQuestion = new CreditAppQuestion();
            CreditAppQuestions objCreditAppQuestions = new CreditAppQuestions();
            List<CrAppQuestions> crAppQuestionsList = new List<CrAppQuestions>();

            var ds = new DataSet();
            var CAQT = new CreditAppQuestionsTransaction();
            string message = CAQT.GetCreditAppQuestions(ds, CustId);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<CodeList> codeList = null;
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    codeList = ds.Tables[1].Rows.OfType<DataRow>()
                     .Select(p => new CodeList()
                     {
                         Code = Convert.ToString(p["code"]),
                         Script = Convert.ToString(p["codedescript"]),
                         Category = Convert.ToString(p["category"])
                     }).ToList();
                }

                List<ConstraintObj> constraintList = null;
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    constraintList = (from p in ds.Tables[2].AsEnumerable()
                                      select new ConstraintObj()
                                      {
                                          qId = p["QId"] != null && p["QId"] != DBNull.Value ? Convert.ToInt32(p["QId"]) : 0,
                                          constraintObj = new Constraints
                                          {
                                              max = p["Max"] != null && p["Max"] != DBNull.Value ? Convert.ToInt64(p["Max"]) : -1,
                                              min = p["Min"] != null && p["Min"] != DBNull.Value ? Convert.ToInt64(p["Min"]) : -1,
                                              regex = p["Regex"] != null && p["Regex"] != DBNull.Value ? Convert.ToString(p["Regex"]) : string.Empty,
                                              customErrorMessages = new CustomErrorMessage
                                              {
                                                  max = Convert.ToString(p["MaxErrorMessage"]),
                                                  min = Convert.ToString(p["MinErrorMessage"]),
                                                  regex = Convert.ToString(p["RegexErrorMessage"])
                                              }
                                          }
                                      }).ToList();
                }
                List<QuestionObj> questionList = null;
                if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                {
                    questionList =
                        (from p in ds.Tables[4].AsEnumerable()
                         select new QuestionObj()
                         {
                             dId = p["DId"] != null && p["DId"] != DBNull.Value ? Convert.ToInt32(p["DId"]) : 0,
                             question = new Question
                             {
                                 qId = p["Id"] != null && p["Id"] != DBNull.Value ? Convert.ToString(p["Id"]) : string.Empty,
                                 filterName = p["filterName"] != null && p["filterName"] != DBNull.Value ? Convert.ToString(p["filterName"]) : string.Empty
                             }
                         }).ToList();
                }

                List<DependsOnObj> dependsOnList = null;
                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    dependsOnList = ds.Tables[3].Rows.OfType<DataRow>()
                     .Select(p => new DependsOnObj()
                     {
                         qId = p["QId"] != null && p["QId"] != DBNull.Value ? Convert.ToInt32(p["QId"]) : 0,
                         dId = p["Id"] != null && p["Id"] != DBNull.Value ? Convert.ToInt32(p["Id"]) : 0,
                         dependsOn = new DependsOn
                         {
                             catalog = p["catalog"] != null && p["catalog"] != DBNull.Value ? Convert.ToString(p["catalog"]) : string.Empty,
                             questions = questionList != null && questionList.Count > 0 ?
                                        questionList
                                        .Where(x => Convert.ToString(x.dId).Trim() == Convert.ToString(p["Id"]))
                                        .Select(s => new Question()
                                        {
                                            qId = s.question.qId,
                                            filterName = s.question.filterName
                                        }
                                        ).ToList()
                                        : null
                         }
                     }).ToList();
                }

                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime currentUtcDate = DateTime.UtcNow.Date;

                crAppQuestionsList = ds.Tables[0].Rows.OfType<DataRow>()
                     .Select(p => new CrAppQuestions()
                     {
                         qId = Convert.ToInt32(p["QuestionId"]),
                         question = Convert.ToString(p["Question"]).Trim(),
                         inputType = Convert.ToString(p["inputType"]),
                         inputCategory = Convert.ToString(p["inputCategory"]),
                         questionTitle = Convert.ToString(p["CategorySection"]),
                         answer = new string[0],
                         mandatory = Convert.ToBoolean(p["IsMandatory"]),
                         options = !string.IsNullOrWhiteSpace(Convert.ToString(p["Category"]))
                                                        ? new OptionsList
                                                        {
                                                            option = codeList
                                                            .Where(x => x.Category.Trim() == Convert.ToString(p["Category"]))
                                                            .Select(s => new OptionModel()
                                                            {
                                                                id = s.Code,
                                                                text = s.Script
                                                            }
                                                                    ).ToList()
                                                        }
                                                        : new OptionsList
                                                        {
                                                            option = new List<OptionModel>()
                                                        },
                         constraints = constraintList != null && constraintList.Count > 0 ?
                                                    constraintList
                                                    .Where(x => x.qId.Equals(Convert.ToInt32(p["QuestionId"])))
                                                    .Select(y => new Constraints()
                                                    {
                                                        max = Convert.ToInt32(p["QuestionId"]).Equals(1001) && y.constraintObj.max > 0 ? (Int64)(currentUtcDate.AddYears(-Convert.ToInt32(y.constraintObj.max)).Subtract(epoch)).TotalMilliseconds : y.constraintObj.max,
                                                        min = Convert.ToInt32(p["QuestionId"]).Equals(1001) && y.constraintObj.min > 0 ? (Int64)(currentUtcDate.AddYears(-Convert.ToInt32(y.constraintObj.min)).Subtract(epoch)).TotalMilliseconds : y.constraintObj.min,
                                                        regex = y.constraintObj.regex,
                                                        customErrorMessages = y.constraintObj.customErrorMessages
                                                    }
                                                    ).FirstOrDefault()
                                                : null,
                         dependsOn = dependsOnList != null && dependsOnList.Count > 0
                                        ? dependsOnList
                                        .Where(x => x.qId.Equals(Convert.ToInt32(p["QuestionId"])))
                                        .Select(y => new DependsOn
                                        {
                                            catalog = y.dependsOn.catalog,
                                            questions = y.dependsOn.questions
                                        }).FirstOrDefault()
                                        : null
                     }).ToList();
                objCreditAppQuestions.questionsAndAnswers = crAppQuestionsList;
            }
            objCreditAppQuestion.CreditAppQuestions = objCreditAppQuestions;
            objCreditAppQuestion.Message = message;
            return objCreditAppQuestion;
        }

        public ContractDetails GetContractPDF(GetContract objGetContract)
        {
            var ds = new DataSet();
            var GCR = new GetContractRepository();
            ContractDetails contractDetails = new ContractDetails();
            string message = GCR.GetCustomerContractDetails(ds, objGetContract);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                contractDetails.CustomerContractDetail = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new CustomerContractDetails()
                    {
                        CustID = objGetContract.CustId,
                        Name = Convert.ToString(p["Name"]).Trim(),
                        AccountNumber = Convert.ToString(p["AccountNumber"]).Trim(),
                        Address = Convert.ToString(p["Address"]).Trim(),
                        PropDate = Convert.ToString(p["propdate"]).Trim(),
                        PropMonth = Convert.ToString(p["propmonth"]).Trim(),
                        PropYear = Convert.ToString(p["propyear"]).Trim(),
                        PurchaseAmount = Convert.ToString(p["PurchaseAmount"]).Trim(),
                        InstallmentAmount = Convert.ToString(p["installment"]).Trim(),
                        DueDay = Convert.ToString(p["DueDay"]).Trim()
                    })
                    .FirstOrDefault();
            }
            contractDetails.Message = message;
            return contractDetails;
        }

        public int ScoreProposal(string country, string accountNo, string accountType, string customerID, DateTime dateProp, short branchNo,
            ref bool referDeclined, out string newBand, out string refCode, out decimal points, out decimal RFLimit,
            out string result, out string bureauFailure, int user, out string referralReasons, out string err)
        {

            err = "";
            refCode = "";
            points = 0;
            result = "";
            RFLimit = 0;
            bureauFailure = "";
            newBand = "";
            referralReasons = string.Empty;

            SqlConnection conn = null;

            BProposal prop = null;
            using (conn = new SqlConnection(Connections.Default))
            {


                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    prop = new BProposal();
                    prop.User = Credential.UserId;
                    prop.Score(conn, trans, country, accountNo, accountType, customerID, dateProp, branchNo, out newBand, out refCode, out points, out RFLimit, user, out result, out bureauFailure, ref referDeclined, out referralReasons);
                    trans.Commit();
                }
            }
            return 0;
        }

        public int CheckAccountToCancel(string accountNo, string countryCode, ref decimal outstBalance,
        ref bool outstPayments, out string err)
        {
            err = "";

            SqlConnection conn = null;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    BAccount acct = new BAccount();
                    acct.User = Credential.UserId;
                    acct.CheckAccountToCancel(conn, trans, accountNo, countryCode, Credential.UserId, ref outstBalance, ref outstPayments);
                    trans.Commit();
                }

            }
            return 0;
        }

        public void SetCreditLimit(string custID, decimal creditLimit, out string err)
        {
            err = "";

            SqlConnection conn = null;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    BCustomer cust = new BCustomer();
                    cust.SetCreditLimit(conn, trans, custID, creditLimit);

                    trans.Commit();
                }
            }

        }

        public CountryMaintenanceModel GetCountryMaintenceDetails(string custId)
        {
            CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
            DataSet ds = new DataSet();
            var CM = new CountryMaintenance();
            List<string> result = CM.CountryMaintenanceDetails(custId, ds);
            dtCountryMaintenanceModel.DtCountryMaintenance = ds;
            if (result != null && result.Count > 1)
            {
                dtCountryMaintenanceModel.Message = result[0];
                dtCountryMaintenanceModel.StatusCode = result[1];
            }
            return dtCountryMaintenanceModel;
        }

        public DataSet GetProposalStage1(string customerID, string accountNo)
        {
            BProposal prop = new BProposal();
            DataSet ds = prop.GetProposalStage1(customerID, accountNo, "H");
            return ds;
        }

        public int SaveProposal(string customerID, string accountNo, DataSet app1, DataSet app2, bool sanction, out string err)
        {
            err = "";

            SqlConnection conn = null;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    BProposal prop = new BProposal();
                    prop.User = Credential.UserId;
                    prop.Save(conn, trans, customerID, accountNo, app1, app2, sanction);

                    trans.Commit();
                }

            }
            return 0;
        }

        public DataSet GetProposalStage2(string customerID, DateTime dtDateProp, string accountNo, string relation, out string err)
        {
            err = "";

            DataSet ds = null;

            BProposal p = new BProposal();
            ds = p.GetProposalStage2(customerID, dtDateProp, accountNo, relation);

            return ds;
        }

        public int SaveProposalStage2(string customerID, string accountNo, DataSet app1, DataSet app2, bool complete, out string err)
        {
            err = "";

            SqlConnection conn = null;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    BProposal prop = new BProposal();
                    prop.User = Credential.UserId;
                    prop.SaveStage2(conn, trans, customerID, accountNo, app1, app2, complete);

                    trans.Commit();
                }

            }
            return 0;
        }

        public int SaveDocConfiramtion(string CustId, string acctNo, DateTime dateprop)
        {
            Int16 origbr = 0;
            string custid = CustId;
            string checktype = "DC";
            DateTime datecleared = DateTime.Now;
            Int32 empeenopflg = 1000001;
            string acctno = acctNo;

            var CUR = new SaveDocConfirmation();
            Blue.Cosacs.Repositories.AccountRepository AccountR = new Blue.Cosacs.Repositories.AccountRepository();

            SqlConnection conn = null;
            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    CUR.updateDocFlag(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno);

                    AccountR.InstantCreditDACheck(acctno, 1000001, conn, trans);

                    trans.Commit();
                }

            }
            return 1;
        }

        public int UpdateAddressTypeandWorkPhone(string CustId, string AddressType, string Address, string ResidenatialStatus, string WorkAddress, string WorkPhone)
        {
            var uca = new UpdateCustAddress();
            int recordCount = uca.SaveCustAddress(CustId, AddressType, Address, ResidenatialStatus, WorkAddress, WorkPhone);
            return recordCount;
        }

        public CustomerCreditSummary GetCustomerCreditSummary(string CustId)
        {
            var ds = new DataSet();
            var CT = new CustomerCreditSummaryTransaction();
            var objCustomerCreditSummary = new CustomerCreditSummary();
            string message = CT.GetCustomerCreditSummaryTransaction(ds, CustId);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                string dueDateString = Convert.ToString(ds.Tables[0].Rows[0]["DueDate"]);
                List<string> stringArray = !string.IsNullOrWhiteSpace(dueDateString) ? (dueDateString.Contains(',') ? dueDateString.Split(',').ToList() : new List<string> { dueDateString }) : new List<string>();
                List<Int64> resultList = new List<Int64>();
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                foreach (string curDateTimeStr in stringArray)
                {
                    if (!string.IsNullOrWhiteSpace(curDateTimeStr))
                    {
                        DateTime currentDateTime = Convert.ToDateTime(curDateTimeStr);
                        Int64 unixMilliSecondTimeSpan = Convert.ToInt64(currentDateTime.Subtract(epoch).TotalMilliseconds);
                        resultList.Add(unixMilliSecondTimeSpan);
                    }
                }


                objCustomerCreditSummary = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new CustomerCreditSummary()
                    {
                        _CustomerCreditSummary = new CustomerCreditSummaryDetails
                        {
                            extUId = Convert.ToString(ds.Tables[0].Rows[0]["CustId"]),
                            customerName = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]),
                            dueDate = resultList,
                            totalOutstandingAmount = Convert.ToString(ds.Tables[0].Rows[0]["TotalCreditDue"]),
                            monthlyOutstandingAmount = Convert.ToString(ds.Tables[0].Rows[0]["MonthlyDue"]),
                            creditAvailable = Convert.ToString(ds.Tables[0].Rows[0]["CreditAvailable"]),
                        },
                        Message = message
                    })
                    .FirstOrDefault();
            }
            else
            {
                objCustomerCreditSummary = new CustomerCreditSummary
                {
                    Message = message,
                    _CustomerCreditSummary = null
                };
            }


            return objCustomerCreditSummary;
        }

        public string GetUserTransactions(UserTransactionInputModel objUserTransactionInputModel, out UserTransactions objUserTransactions)
        {
            var ds = new DataSet();
            var CT = new UserTransaction();
            objUserTransactions = new UserTransactions();
            string message = CT.GetUserTransaction(ds, objUserTransactionInputModel);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    objUserTransactions.transactions = ds.Tables[0].Rows.OfType<DataRow>()
                         .Select(p => new TransactionDetails()
                         {
                             id = Convert.ToString(p["AccountNumber"]).Trim(),
                             date = p["DateOfTransaction"] != null ? Convert.ToInt64(Convert.ToDateTime(p["DateOfTransaction"]).Subtract(epoch).TotalMilliseconds) : 0,
                             amount = Convert.ToDecimal(p["TotalAmount"]),
                             concept = Convert.ToString(p["StoreName"]),
                             type = Convert.ToString(p["type"]),

                         }).ToList();
                }
                if (ds.Tables[1] != null && ds.Tables[1].Rows != null && ds.Tables[1].Rows.Count > 0)
                {
                    objUserTransactions.lastPage = ((int)ds.Tables[1].Rows[0]["lastPage"]).Equals(1);
                    objUserTransactions.totalElements = (int)ds.Tables[1].Rows[0]["totalElements"];
                    objUserTransactions.totalPages = (int)ds.Tables[1].Rows[0]["totalPages"];
                    objUserTransactions.pageSize = (int)ds.Tables[1].Rows[0]["pageSize"];
                    objUserTransactions.pageNumber = (int)ds.Tables[1].Rows[0]["pageNumber"];
                    objUserTransactions.sort = (string)ds.Tables[1].Rows[0]["sort"];
                    objUserTransactions.firstPage = ((int)ds.Tables[1].Rows[0]["firstPage"]).Equals(1);
                    objUserTransactions.numberOfElements = (int)ds.Tables[1].Rows[0]["numberOfElements"];
                }
            }
            return message;
        }

        public DateTime GetProposalDate(string accountNumber, string custId)
        {
            var ds = new DataSet();
            var CT = new GetProposalDate();

            CT.GetProposalDateDetails(ds, custId, accountNumber);
            return Convert.ToDateTime(ds.Tables[0].Rows[0]["dateprop"]);

        }

        public List<CreditDcoumentStatus> CreditDocStatus()
        {
            DataSet ds = new DataSet();
            var docStatus = new DcoumentStatus();
            CreditDcoumentStatus CDS = new CreditDcoumentStatus();
            List<CreditDcoumentStatus> objCDS = new List<CreditDcoumentStatus>();
            docStatus.getDcoumentStatus(ds);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                objCDS = ds.Tables[0].Rows.OfType<DataRow>()
                      .Select(p => new CreditDcoumentStatus()
                      {
                          custId = Convert.ToString(p["custId"]),
                          accountNumber = Convert.ToString(p["acctNo"]).Trim(),
                          status = Convert.ToString(p["DocumentStatus"]).Trim(),
                          email = Convert.ToString(p["email"]).Trim(),

                      }).ToList();
            }
            return objCDS;
        }

        public DataSet GetRFAccountInformation(string custId)
        {
            var ds = new DataSet();
            var CT = new GetRFAccountInformation();

            CT.GetRFAccountInformationDetails(ds, custId);
            return ds;

        }

        public DataSet CheckAccount(string custId, string storeId)
        {
            var ds = new DataSet();
            var CT = new CheckAccount();

            CT.CheckAccountDetails(ds, custId, storeId);
            return ds;

        }

        public int TPInsertLineItem(string CustId, decimal loanAmount, int numberOfInstallments, string storeId, string accountNumber, Int16 Branch)
        {
            var uca = new UpdateLineItem();
            int recordCount = uca.SaveLineItem(CustId, loanAmount, numberOfInstallments, storeId, accountNumber, Branch);
            return recordCount;
        }

        public int CompleteReferralStage(string customerID, string accountNo, DateTime dateProp, string newNotes, string notes, bool approved,
            bool rejected, bool reOpen, int branch, decimal creditLimit, string CountryCode, out string err)
        {

            err = "";
            SqlConnection conn = null;

            BProposal prop = null;
            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    prop = new BProposal();
                    prop.User = Credential.UserId;
                    prop.CompleteReferralStage(conn, trans, customerID, accountNo, dateProp, newNotes, notes, approved, rejected, reOpen, branch, creditLimit, CountryCode);
                    trans.Commit();
                }
            }
            return 0;
        }

        public int SaveScoreHist(string CustomerID, DateTime dateprop, char? scorecard, short? points, float creditlimit,
           string scoringband, int user, string reasonchanged, string AccountNo,
           out string err)
        {

            err = "";
            BProposal prop = new BProposal();
            SqlConnection conn = null;


            using (conn = new SqlConnection(Connections.Default))
            {
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        prop.User = Credential.UserId;
                        prop.SaveScoreHist(conn, trans, CustomerID, dateprop, scorecard, points, creditlimit, scoringband,
                            user, reasonchanged, AccountNo);
                        trans.Commit();
                    }
                }

                return 0;
            }
        }

        public int ClearProposal(string accountNumber, out string err)
        {
            err = "";
            BAgreement agreement;
            SqlConnection conn = null;
            string source = DASource.Manual;

            using (conn = new SqlConnection(Connections.Default))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    agreement = new BAgreement();
                    agreement.User = Credential.UserId;
                    agreement.ClearProposal(conn, trans, accountNumber, source);
                    trans.Commit();
                }
            }

            return 0;
        }

        public bool UpIsTPContractUpload(string CustId, string AcctNo)
        {
            DataSet ds = new DataSet();
            var objStatus = new ContractStatusSaveOnServer();
            objStatus.ContractSaveOnServerStatus(CustId, AcctNo);
            return true;
        }

        public List<CutomerContract> EmailContract(string path, string imageLink)
        {
            DataSet ds = new DataSet();
            var docStatus = new EmailContracts();
            List<CutomerContract> objEmlcntr = new List<CutomerContract>();
            docStatus.getEmailContracts(ds);

            string abc = string.Empty;

            string htmlBody = string.Empty;
            htmlBody = File.ReadAllText(path);

            string ContractSavePath = System.Configuration.ConfigurationManager.AppSettings["ContractSavePath"];
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                objEmlcntr = ds.Tables[0].Rows.OfType<DataRow>()
                 .Select(p => new CutomerContract()
                 {
                     Custid = Convert.ToString(p["custId"]),
                     AccountNumber = Convert.ToString(p["accountNumber"]).Trim(),
                     EmailId = Convert.ToString(p["emailId"]).Trim(),
                     FilePath = (ContractSavePath + Convert.ToString(p["filePath"]).Trim()),
                     MailBody = (htmlBody.Replace("@FirstName", Convert.ToString(p["FirstName"]).Trim())).Replace("@LogoImage", imageLink),
                     MailSubject = Convert.ToString(p["mailSubject"]).Trim(),

                 }).ToList();
            }
            return objEmlcntr;
        }

        public List<string> UpdateContractNotificationStatus(string objJSON)
        {
            var ICD = new UpdateContractNotificationStatusXmlUpdateRepository();
            return ICD.UpdateContractNotificationStatusXmlUpdate(objJSON);
        }
    }
}

