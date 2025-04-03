
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;
using Newtonsoft.Json;
using STL.Common;
using STL.Common.Constants.AccountHolders;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using Blue.Cosacs.Shared;
using System.Xml;
using Unicomer.Cosacs.Business.Helper;

namespace Unicomer.Cosacs.Business
{
    public class TransactionBusiness : Interfaces.ITransaction
    {
        //global variables
        private string referralReasons, _error = String.Empty, decimalPlaces;
        private DataSet prop, prop2, _stage2DataA1, _stage2DataA2 = null;
        DateTime _dateProp = DateTime.Now;
        decimal NetIncomePercentage;

        public JResponse GetUserAccounts(string CustId)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            ValidateUserAccountsModel objUserAccounts = new ValidateUserAccountsModel();
            objUserAccounts = ObjTrans.GetUserAccounts(CustId);
            if (objUserAccounts != null && objUserAccounts._UserAccountsModel != null)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objUserAccounts._UserAccountsModel);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = objUserAccounts.Message;//Need to create resource file.
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = objUserAccounts.Message;//Need to create resource file.
            }
            return objJResponse;
        }

        public JResponse CreateRFAccount(AnswerModel ansModel)
        {

            CreditAnswerModel creditAnsModel = new CreditAnswerModel();
            LoadAnswers(ansModel, creditAnsModel);

            CashLoanDetails det = null;

            bool rescore = false;
            bool reOpenS1 = false;
            bool reOpenS2 = false;
            string Error = string.Empty;
            string Country = string.Empty;
            short Branch;
            DateTime _propDate;

            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            CreditApp objCrApp = new CreditApp();
            UploadDownloadDocument objUdd = new UploadDownloadDocument();

            //Read Country maintenance data from DB.
            CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
            dtCountryMaintenanceModel = ObjTrans.GetCountryMaintenceDetails(ansModel.custId);
            if (dtCountryMaintenanceModel != null && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.Message) && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.StatusCode))
            {
                if (!dtCountryMaintenanceModel.StatusCode.Equals("200"))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = dtCountryMaintenanceModel.Message;//Need to create resource file.
                    return objJResponse;
                }
            }
            DataSet DtCountryMaintenance = dtCountryMaintenanceModel.DtCountryMaintenance;
            DataRow[] foundRows;
            DataRow[] foundDecimalplacesrows;
            DataRow[] foundNetIncomePercentagerows;
            DataRow[] foundTemplateRow;

            //Read Country and branch code from DB.
            Country = (string)DtCountryMaintenance.Tables[0].Rows[0]["countrycode"];
            Branch = (short)DtCountryMaintenance.Tables[0].Rows[0]["origbr"];
            //(short)Convert.ToInt16(ConfigurationManager.AppSettings["BrachCode"]);

            //(short)DtCountryMaintenance.Tables[0].Rows[0]["hobranchno"];
            foundRows = DtCountryMaintenance.Tables[1].Select("codename='" + CountryParameterNames.MinHPage + "'");
            bool ValidCustoermAge = VerifyCustomerAge(Convert.ToDecimal(foundRows[0]["value"]), creditAnsModel.Q1);//New


            foundDecimalplacesrows = DtCountryMaintenance.Tables[1].Select("codename='" + CountryParameterNames.DecimalPlaces + "'");
            decimalPlaces = Convert.ToString(Convert.ToString(foundDecimalplacesrows[0]["value"]));

            foundNetIncomePercentagerows = DtCountryMaintenance.Tables[1].Select("codename='CL_Percentageavailable'");
            NetIncomePercentage = Convert.ToDecimal(Convert.ToDecimal(foundNetIncomePercentagerows[0]["value"]));

            //update Home Address Type, Insert Work Address & Inser Work phone no.
            ObjTrans.UpdateAddressTypeandWorkPhone(creditAnsModel.CustId, "H", creditAnsModel.Q4, creditAnsModel.Q49, creditAnsModel.Q48, creditAnsModel.Q14);//New Q49, Need to save Q48

            try
            {
                //validate customer Age.
                if (ValidCustoermAge == true)
                    objCrApp = ObjTrans.CreateRFAccount(Country, Branch, creditAnsModel.CustId, 11111, false, ref det,
                                                        out rescore, out reOpenS1, out reOpenS2, out Error);
            }
            catch (Exception ex)
            {


                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = ex.Message;
                return objJResponse;

            }

            string acctNo = objCrApp.accountNumber;


            if (!string.IsNullOrEmpty(objCrApp.accountNumber))
            {

                decimal credit = 0;
                string result = String.Empty;
                string refCode = String.Empty;
                decimal points = 0;
                string bureauFailure = "t";
                string newBand = String.Empty;
                bool referDeclined = false;

                string targetFolderPath = string.Empty;
                string idProof_TargetFileName = string.Empty;
                string idProof_UploadedFileName = string.Empty;
                string AddressProof_TargetFileName = string.Empty;
                string AddressProof_UploadedFileName = string.Empty;
                string IncomeProof_TargetFileName = string.Empty;
                string IncomeProof_UploadedFileName = string.Empty;
                string idProofSavedFileName = string.Empty;
                string AddressProofSavedFileName = string.Empty;
                string incomeProofSavedFileName = string.Empty;

                objCrApp.accountNumber = acctNo.Replace("-", "");

                //Read Stage1 proposal data
                prop = ObjTrans.GetProposalStage1(creditAnsModel.CustId, objCrApp.accountNumber);

                //Save Stage1 proposal data
                Save(creditAnsModel, prop, objCrApp.accountNumber);

                _propDate = ObjTrans.GetProposalDate(objCrApp.accountNumber, creditAnsModel.CustId);

                //Set Score result
                SetScoreResult(Country, objCrApp.accountNumber, "R", creditAnsModel.CustId, _propDate, Branch, ref referDeclined,
                               out newBand, out refCode, out points, out credit, out result, out bureauFailure, Credential.UserId, out referralReasons, out Error);

                //Read Stage2 proposal data
                _stage2DataA1 = ObjTrans.GetProposalStage2(creditAnsModel.CustId, _dateProp, objCrApp.accountNumber, Holder.Main, out Error);

                _stage2DataA2 = ObjTrans.GetProposalStage2(creditAnsModel.CustId, _dateProp, objCrApp.accountNumber, Holder.Main, out Error);

                //Save Stage1 proposal data
                SaveProposalStage2(creditAnsModel, _stage2DataA1, objCrApp.accountNumber);

                UploadDocument uploadDocument = new UploadDocument();

                //Folder path for saving the images/pdf.
                uploadDocument.TargetFolderPath = string.Format("{0}\\", Path.Combine(System.Configuration.ConfigurationManager.AppSettings["UploadDocumentTargetFolderPath"], creditAnsModel.CustId, objCrApp.accountNumber));
                uploadDocument.CustId = creditAnsModel.CustId;

                //Save Id proof document.
                uploadDocument.ByteArrayFile = creditAnsModel.Q42;
                uploadDocument.UploadedFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), creditAnsModel.Q53); //MIMEAssistant.GetMIMEType(creditAnsModel.Q53);
                uploadDocument.TargetFileName = string.Format("IdProof1_{0}_{1}", creditAnsModel.CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                UploadContractDocuments(uploadDocument, objCrApp.accountNumber, false);

                //Second Address proof document.
                uploadDocument.ByteArrayFile = creditAnsModel.Q43;
                uploadDocument.UploadedFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), creditAnsModel.Q54); //MIMEAssistant.GetMIMEType(creditAnsModel.Q54);
                uploadDocument.TargetFileName = string.Format("AddressProof_{0}_{1}", creditAnsModel.CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                UploadContractDocuments(uploadDocument, objCrApp.accountNumber, false);

                //Third Income proof document.
                uploadDocument.ByteArrayFile = creditAnsModel.Q44;
                uploadDocument.UploadedFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), creditAnsModel.Q55); //MIMEAssistant.GetMIMEType(creditAnsModel.Q55);
                uploadDocument.TargetFileName = string.Format("IncomeProof_{0}_{1}", creditAnsModel.CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                UploadContractDocuments(uploadDocument, objCrApp.accountNumber, false);

                // Fourth Id proof document back side.
                uploadDocument.ByteArrayFile = creditAnsModel.Q56;
                uploadDocument.UploadedFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), creditAnsModel.Q57); //MIMEAssistant.GetMIMEType(creditAnsModel.Q53);
                uploadDocument.TargetFileName = string.Format("IdProof2_{0}_{1}", creditAnsModel.CustId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                UploadContractDocuments(uploadDocument, objCrApp.accountNumber, false);

                ObjTrans.SaveDocConfiramtion(creditAnsModel.CustId, objCrApp.accountNumber, _propDate);

                //insertSignatureStatus(objCrApp.accountNumber, creditAnsModel.CustId);

            }

            if (!string.IsNullOrEmpty(objCrApp.accountNumber))
            {
                string imageLink;
                imageLink = ConfigurationManager.AppSettings.Get("UploadDocumentTargetFolderPath") + "\\" + creditAnsModel.CustId + "\\" + objCrApp.accountNumber;

                //Mail Body
                string htmlBody = File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["MailContent"]));
                htmlBody = htmlBody.Replace("@CustName", prop.Tables[TN.Customer].Rows[0]["FirstName"].ToString() + ' ' + prop.Tables[TN.Customer].Rows[0]["LastName"].ToString());
                htmlBody = htmlBody.Replace("@AccountNo", objCrApp.accountNumber);
                htmlBody = htmlBody.Replace("@ImageLink", imageLink);


                foundTemplateRow = DtCountryMaintenance.Tables[2].Select("TemplateName='CreditApproval'");

                //Mail Subject
                string subject = Convert.ToString(foundTemplateRow[0]["MailSubject"]);
                subject = subject.Replace("@AccountNo", objCrApp.accountNumber);

                MailTemplate MT = new MailTemplate();
                MT.To = Convert.ToString(foundTemplateRow[0]["MailTo"]);
                MT.Cc = Convert.ToString(foundTemplateRow[0]["MailCC"]);
                MT.body = htmlBody;
                MT.Subject = subject;

                //objCrApp.Message = "Account Created successfully and Record saved successfully.";
                //objCrApp.CredtAppStatus = "Pending for Approval";
                objCrApp.MailContent = MT;

                objJResponse.Result = JsonConvert.SerializeObject(objCrApp);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = "Record saved successfully.";
            }
            else
            {
                if (ValidCustoermAge == false)
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = "Customer is not eligible because age should be greater than 18";//Need to create resource file.

                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = "Account not registered";//Need to create resource file.
                }
            }
            return objJResponse;

        }

        public JResponse GetCreditAppQuestions(string CustId)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository objTransaction = new TransactionRepository();
            CreditAppQuestion objCreditAppQuestion = new CreditAppQuestion();
            CreditAppQuestions objCreditAppQuestions = new CreditAppQuestions();
            objCreditAppQuestion = objTransaction.GetCreditAppQuestions(CustId);
            if (objCreditAppQuestion != null)
            {
                objCreditAppQuestions = objCreditAppQuestion.CreditAppQuestions;
                if (objCreditAppQuestions != null && objCreditAppQuestions.questionsAndAnswers != null && objCreditAppQuestions.questionsAndAnswers.Count > 0)
                {
                    objJResponse.Result = JsonConvert.SerializeObject(objCreditAppQuestions);
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                }

                objJResponse.Message = objCreditAppQuestion.Message;//Need to create resource file.
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No data found for user.";//Need to create resource file.
            }
            return objJResponse;
        }

        public static Byte[] generatePDFArray(CustomerContractDetails objContractDetails)
        {
            StringBuilder htmlFile = new StringBuilder();
            string htmlCode1 = File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Contract1"]));
            htmlCode1 = htmlCode1.Replace("{{Name}}", objContractDetails.Name);
            htmlCode1 = htmlCode1.Replace("{{Address}}", objContractDetails.Address);
            htmlCode1 = htmlCode1.Replace("{{AccountNumber}}", objContractDetails.AccountNumber);
            htmlCode1 = htmlCode1.Replace("{{PropDay}}", objContractDetails.PropDate);
            htmlCode1 = htmlCode1.Replace("{{PropMonth}}", objContractDetails.PropMonth);
            htmlCode1 = htmlCode1.Replace("{{PropYear}}", objContractDetails.PropYear);
            htmlCode1 = htmlCode1.Replace("{{PurchaseAmount}}", objContractDetails.PurchaseAmount);
            htmlFile.Append(htmlCode1);
            string htmlCode2 = File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Contract2"]));
            htmlCode2 = htmlCode2.Replace("{{PurchaseAmount}}", objContractDetails.PurchaseAmount);
            htmlCode2 = htmlCode2.Replace("{{InstalmentAmount}}", objContractDetails.InstallmentAmount);
            htmlCode2 = htmlCode2.Replace("{{DueDay}}", objContractDetails.DueDay);
            htmlFile.Append(htmlCode2);
            // htmlFile.Append(File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Contract2"])));	
            byte[] htmlContract = ASCIIEncoding.ASCII.GetBytes(htmlFile.ToString());
            return htmlContract;
        }

        public JResponse UploadContractDocuments(UploadDocument uploadDocument, string accountNumber, bool isThirdParty = false)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository objTransaction = new TransactionRepository();
            UploadDownloadDocument objUploadDownloadDocument = new UploadDownloadDocument();

            string targetPath = UploadDownloadDocument.GetTargetPath(uploadDocument.TargetFolderPath, uploadDocument.TargetFileName, uploadDocument.UploadedFileName);
            if (uploadDocument.UploadedFiles == null && (uploadDocument.ByteArrayFile != null && uploadDocument.ByteArrayFile.Length > 0))
            {
                string fileName = Path.GetFileName(targetPath);
                string targetFolderPath = uploadDocument.TargetFolderPath.Replace(fileName, string.Empty);
                string message = UploadDownloadDocument.UploadDocumentWithByteArray(targetPath, uploadDocument.ByteArrayFile, uploadDocument.CustId, targetFolderPath, accountNumber, isThirdParty);

                objJResponse.Result = message.Equals("File uploaded successfully.") ? JsonConvert.SerializeObject(new { success = true }) : string.Empty;
                objJResponse.Status = message.Equals("File uploaded successfully.");
                objJResponse.StatusCode = message.Equals("File uploaded successfully.") ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound;
                objJResponse.Message = message;
            }
            else if (uploadDocument.UploadedFiles != null && uploadDocument.UploadedFiles.Count > 0)
            {
                string message = UploadDownloadDocument.UploadDocumentWithFiles(uploadDocument.TargetFolderPath, uploadDocument.CustId, uploadDocument.UploadedFiles, accountNumber, isThirdParty);

                objJResponse.Result = message.Equals("File uploaded successfully.") ? JsonConvert.SerializeObject(new { success = true }) : string.Empty;
                objJResponse.Status = message.Equals("File uploaded successfully.");
                objJResponse.StatusCode = message.Equals("File uploaded successfully.") ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotFound;
                objJResponse.Message = message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No content found to upload.";
            }

            //--- CODE to save contract file on server and update the flag in 'signaturestatus' table -- START
            if (isThirdParty && objJResponse.Status)
            {
                string strContractSavePath = System.Configuration.ConfigurationManager.AppSettings["ContractSavePath"];
                string strContractSaveUserName = System.Configuration.ConfigurationManager.AppSettings["ContractSaveUserName"];
                string strContractSavePassword = System.Configuration.ConfigurationManager.AppSettings["ContractSavePassword"];

                string TargetFileName = uploadDocument.TargetFileName;
                string extFile = !string.IsNullOrWhiteSpace(uploadDocument.UploadedFileName) ? Path.GetExtension(uploadDocument.UploadedFileName) : string.Empty;
                if (string.IsNullOrWhiteSpace(extFile))
                    TargetFileName += ".pdf";
                else
                    TargetFileName += extFile;

                bool uploadstatus = UploadDownloadDocument.UploadBase64DocumentToFTPFolder(TargetFileName, uploadDocument.ByteArrayFile, strContractSavePath, strContractSaveUserName, strContractSavePassword);
                bool updatestatus = UpdateIsTPContractUpload(uploadDocument.CustId, uploadDocument.AccountNumber);
            }
            //--- CODE to save contract file on server and update the flag in 'signaturestatus' table -- END

            return objJResponse;
        }

        public bool UpdateIsTPContractUpload(string CustId, string AcctNo)
        {
            JResponse objJResponse = new JResponse();
            bool objStatus;
            TransactionRepository objTransaction = new TransactionRepository();
            List<CreditDcoumentStatus> objCDS = new List<CreditDcoumentStatus>();

            objStatus = objTransaction.UpIsTPContractUpload(CustId, AcctNo);

            return objStatus;
        }

        public JResponse GetContractPDF(GetContract objgetContract)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository objTransaction = new TransactionRepository();
            ContractDetails objContractDetail = new ContractDetails();
            ContractResult objContractResult = new ContractResult();
            objContractDetail = objTransaction.GetContractPDF(objgetContract);

            if (objContractDetail != null && objContractDetail.CustomerContractDetail != null && !string.IsNullOrEmpty(objContractDetail.CustomerContractDetail.AccountNumber))
            {
                objContractResult.contract = generatePDFArray(objContractDetail.CustomerContractDetail);
                Signatures[] signatureList = new Signatures[3];
                signatureList[0] = new Signatures { id = "1FA", title = "Signature" };
                signatureList[1] = new Signatures { id = "2FA", title = "Signature" };
                signatureList[2] = new Signatures { id = "3FA", title = "Signature" };
                objContractResult.signatures = signatureList;
                objJResponse.Result = JsonConvert.SerializeObject(objContractResult);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = objContractDetail.Message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = objContractDetail.Message;
            }
            return objJResponse;
        }

        private bool Save(CreditAnswerModel creditAnsModel, DataSet prop, string accountNumber)
        {
            bool valid = true;
            try
            {
                bool sanction = false;
                #region applicant 1
                #region information from the customer tables
                DataRow r = prop.Tables[TN.Customer].Rows[0];

                r[CN.DOB] = creditAnsModel.Q1;//New
                r[CN.Title] = creditAnsModel.Q2;//Mandatory
                r[CN.MaritalStatus] = creditAnsModel.Q9;//New
                r[CN.Dependants] = creditAnsModel.Q10;//New
                r[CN.Nationality] = creditAnsModel.Q11;//New
                r[CN.DateIn] = Convert.ToDateTime(DateTime.Now.AddYears(-creditAnsModel.Q50));// _dateProp;//custDetails.Q8;//Mandatory
                r[CN.PrevDateIn] = _dateProp;//custDetails.Q18;//
                r[CN.Address1] = creditAnsModel.Q4;//New
                r[CN.PrevResidentialStatus] = creditAnsModel.Q49;// String.Empty;
                r[CN.PropertyType] = string.Empty;
                r[CN.Sex] = creditAnsModel.Q46;//Sex//new


                #endregion

                #region information from the bank tables
                if (prop.Tables[TN.Bank].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Bank].NewRow();
                    prop.Tables[TN.Bank].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Bank].Rows[0];

                r[CN.CustomerID] = creditAnsModel.CustId;
                r[CN.BankAccountOpened] = _dateProp;// custDetails.Q25;//Mandatory
                r[CN.Code] = string.Empty; //custDetails.Q24;//Mandatory

                #endregion

                #region information from the employment tables

                if (prop.Tables[TN.Employment].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Employment].NewRow();
                    prop.Tables[TN.Employment].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Employment].Rows[0];

                r = prop.Tables[TN.Employment].Rows[0];
                r[CN.DateEmployed] = creditAnsModel.Q15;//Mandatory//Need Default value
                r[CN.PrevDateEmployed] = _dateProp;//custDetails.Q16;
                r[CN.EmploymentStatus] = "E";// custDetails.Q17;// Default Value Employed
                r[CN.PayFrequency] = creditAnsModel.Q13;
                r[CN.WorkType] = creditAnsModel.Q12;//Default value Other.
                if (String.Compare(creditAnsModel.Q19.ToString(), "") == 0)
                    r[CN.AnnualGross] = DBNull.Value;
                else
                    r[CN.AnnualGross] = 12 * Convert.ToDouble((StripCurrency(creditAnsModel.Q19.ToString())));


                #endregion

                #region supplementary information which will go in the proposal table
                //there may or may not be a row in the proposal table
                if (prop.Tables[TN.Proposal].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Proposal].NewRow();
                    prop.Tables[TN.Proposal].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Proposal].Rows[0];

                r[CN.Occupation] = creditAnsModel.Q12;//New
                r[CN.AdditionalIncome] = Convert.ToDecimal(0.0);//custDetails.Q20;//Mandatory
                r[CN.Commitments1] = creditAnsModel.Q21;
                r[CN.Commitments2] = Convert.ToDecimal(0.0);//Default
                r[CN.Commitments3] = Convert.ToDecimal(0.0);//custDetails.Q27;//Mandatory
                r[CN.MonthlyIncome] = creditAnsModel.Q19;//Mandatory
                r[CN.MaritalStatus] = creditAnsModel.Q9;//New
                r[CN.Dependants] = creditAnsModel.Q10;//New
                r[CN.Nationality] = creditAnsModel.Q11;//New
                r[CN.DateProp] = _dateProp;//Default value

                r[CN.PrevEmpMM] = Convert.ToDateTime(creditAnsModel.Q15).Month;
                r[CN.PrevEmpYY] = Convert.ToDateTime(creditAnsModel.Q15).Year;
                r[CN.OtherPayments] = Convert.ToDecimal(0.0);
                //if (!string.IsNullOrWhiteSpace(creditAnsModel.Q58))
                r[CN.CCardNo1] = string.Empty;
                r[CN.CCardNo2] = string.Empty;
                r[CN.CCardNo3] = string.Empty;
                r[CN.CCardNo4] = string.Empty;
                r[CN.AdditionalExpenditure1] = Convert.ToDecimal(0.0);//GetAdditionalExpendicture(creditAnsModel.Q19, NetIncomePercentage);// Convert.ToDecimal(0.0);
                r[CN.AdditionalExpenditure2] = Convert.ToDecimal(0.0);
                r["PurchaseCashLoan"] = false;
                r[CN.RFCategory] = 1;
                r[CN.DistanceFromStore] = 0;
                r[CN.TransportType] = string.Empty;

                if (String.Compare(creditAnsModel.Q52.ToString(), "") == 0)
                    r[CN.Commitments2] = DBNull.Value;
                else
                    r[CN.Commitments2] = MoneyStrToDecimal(creditAnsModel.Q52);

                //if (String.Compare(creditAnsModel.Q51.ToString(), "") == 0)
                //    r[CN.Commitments3] = DBNull.Value;
                //else
                r[CN.Commitments3] = GetAdditionalExpendicture(creditAnsModel.Q19, NetIncomePercentage);
                //r[CN.Commitments3] = GetLivingExpences(creditAnsModel.Q19, MoneyStrToDecimal(creditAnsModel.Q52), NetIncomePercentage); //MoneyStrToDecimal(creditAnsModel.Q51);
                //r[CN.Commitments3] = MoneyStrToDecimal(creditAnsModel.Q51);

                #endregion

                if (prop.Tables[TN.Agreements].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Agreements].NewRow();
                    prop.Tables[TN.Agreements].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Agreements].Rows[0];

                r[CN.PaymentMethod] = "W";

                TransactionRepository ObjTrans = new TransactionRepository();

                ObjTrans.SaveProposal(creditAnsModel.CustId, accountNumber, prop, prop2, sanction, out _error);
                #endregion

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return valid;
        }

        private bool VerifyCustomerAge(decimal MinHPage, DateTime DOB)
        {
            bool valid = true;
            if (CalculateAge(DOB) < MinHPage)
            {
                valid = false;
            }


            return valid;
        }

        private decimal CalculateAge(DateTime dtDOB)
        {
            int y = DateTime.Today.Year - dtDOB.Year;
            int m = DateTime.Today.Month - dtDOB.Month;
            int d = DateTime.Today.Day - dtDOB.Day;
            if (d < 0) m--;
            if (m < 0) y--;

            return Convert.ToDecimal(y);
        }

        private void SetScoreResult(string country, string accountNo, string accountType, string customerID, DateTime dateProp,
                                    short branchNo, ref bool referDeclined, out string newBand, out string refCode, out decimal points,
                                    out decimal credit, out string result, out string bureauFailure, int user, out string referralReasons, out string Error)
        {

            string err = string.Empty;

            TransactionRepository ObjTrans = new TransactionRepository();

            ObjTrans.ScoreProposal(country, accountNo, accountType, customerID, dateProp,
                     Convert.ToInt16(branchNo), ref referDeclined, out newBand,
                     out refCode, out points, out credit, out result, out bureauFailure, user, out referralReasons, out Error);

            if (accountType == AT.ReadyFinance || accountType == AT.StoreCard)
            {
                ObjTrans.SetCreditLimit(customerID, credit, out err);
            }

        }

        private bool SaveProposalStage2(CreditAnswerModel creditAnsModel, DataSet _stage2DataA1, string accountNumber)
        {
            bool valid = true;
            //int refNo = 0;
            TransactionRepository ObjTrans = new TransactionRepository();
            try
            {
                bool complete = true;

                #region save proposal table
                DataRow r;
                if (_stage2DataA1.Tables[TN.Proposal].Rows.Count == 0)
                {
                    r = _stage2DataA1.Tables[TN.Proposal].NewRow();
                    _stage2DataA1.Tables[TN.Proposal].Rows.Add(r);
                }
                else
                    r = _stage2DataA1.Tables[TN.Proposal].Rows[0];

                //DataRow r = _stage2DataA1.Tables[TN.Proposal].Rows[0];
                r[CN.PAddress1] = creditAnsModel.Q4;
                r[CN.PAddress2] = string.Empty; //custDetails.Q5;//
                r[CN.NewComment] = Convert.ToDecimal(0.0);// custDetails.Q27;//Mandatory
                r[CN.EmployeeName] = creditAnsModel.Q47;//New
                #endregion

                #region save references
                // Clear the old references
                _stage2DataA1.Tables[TN.References].Clear();

                r = _stage2DataA1.Tables[TN.References].NewRow();
                r[CN.RefFirstName] = creditAnsModel.Q28;
                r[CN.RefLastName] = string.Empty; //custDetails.Q29;//Mandatory
                r[CN.RefRelation] = string.Empty;//custDetails.Q30;//Mandatory
                r[CN.RefAddress1] = string.Empty; //custDetails.Q31;//Mandatory
                r[CN.RefPhoneNo] = creditAnsModel.Q32;//Mandatory
                r[CN.NewComment] = string.Empty; //custDetails.Q34;//
                r[CN.DateChange] = _dateProp; //custDetails.Q33;//
                r[CN.YrsKnown] = 0;
                r[CN.RefAddress2] = string.Empty;
                r[CN.RefCity] = string.Empty;
                r[CN.RefPostCode] = string.Empty;
                r[CN.RefWAddress1] = string.Empty;
                r[CN.RefWAddress2] = string.Empty;
                r[CN.RefWCity] = string.Empty;
                r[CN.RefWPostCode] = string.Empty;
                r[CN.RefDialCode] = string.Empty;
                r[CN.RefWDialCode] = string.Empty;
                r[CN.RefWPhoneNo] = string.Empty;
                r[CN.RefMDialCode] = string.Empty;
                r[CN.RefMPhoneNo] = string.Empty;
                r[CN.RefDirections] = string.Empty;
                r[CN.RefComment] = string.Empty;
                r[CN.EmpeeNoChange] = 0;
                _stage2DataA1.Tables[TN.References].Rows.Add(r);

                if (_stage2DataA2 != null)
                {
                    r = _stage2DataA2.Tables[TN.Customer].Rows[0];
                    r[CN.DeliveryArea] = string.Empty; //custDetails.Q7;//Mandatory
                }
                #endregion

                if (valid)
                {
                    ObjTrans.SaveProposalStage2(creditAnsModel.CustId, accountNumber, _stage2DataA1, _stage2DataA2, complete, out _error);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return valid;
        }

        public string StripCurrency(string field)
        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);
            //Thread.CurrentThread.CurrentCulture = new CultureInfo(Config.Culture);
            return this.StripCurrency(field, Thread.CurrentThread.CurrentCulture);
        }

        public string StripCurrency(string field, CultureInfo c)
        {
            /* this method needs to cope with all possible negative currency patterns */
            var num = c.NumberFormat;
            string separator = num.CurrencyGroupSeparator;
            string currency = num.CurrencySymbol;
            field = field.Replace(currency, "");
            field = field.Replace(separator, "");
            //int i = 0;

            switch (num.CurrencyNegativePattern)
            {
                case 0:
                    field = field.Replace("(", "-").Replace(")", "");	/* ($n) */
                    break;
                case 3:
                    field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");	/* $n-	*/
                    break;
                case 4:
                    field = field.Replace("(", "-").Replace(")", "");	/* (n$) */
                    break;
                case 6:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n-$  */
                    break;
                case 7:
                    field = "-" + field.Replace("-", "");					/* n$-	*/
                    break;
                case 10:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n $-	*/
                    break;
                case 11:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* $ n-	*/
                    break;
                case 13:
                    field = field = field.IndexOf("-") == -1 ? field : "-" + field.Replace("-", "");					/* n- $ */
                    break;
                case 14:
                    field = field.Replace("(", "-").Replace(")", "");	/* ($ n) */
                    break;
                case 15:
                    field = field.Replace("(", "-").Replace(")", "");	/* (n $) */
                    break;
                default:
                    break;
            }
            return field.Trim();
        }

        public JResponse GetCustomerCreditSummary(string CustId)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            CustomerCreditSummary objCustomerCreditSummary = new CustomerCreditSummary();
            objCustomerCreditSummary = ObjTrans.GetCustomerCreditSummary(CustId);
            if (objCustomerCreditSummary._CustomerCreditSummary != null)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objCustomerCreditSummary._CustomerCreditSummary);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = objCustomerCreditSummary.Message;//Need to create resource file.
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = objCustomerCreditSummary.Message;//Need to create resource file.
            }
            return objJResponse;
        }

        public JResponse GetUserTransactions(UserTransactionInputModel objUserTransactionInputModel)
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            UserTransactions objUserTransactions = new UserTransactions();
            string message = ObjTrans.GetUserTransactions(objUserTransactionInputModel, out objUserTransactions);
            if (objUserTransactions != null && objUserTransactions.transactions != null && objUserTransactions.transactions.Count > 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objUserTransactions);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = message;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = message;//Need to create resource file.
            }
            return objJResponse;
        }

        public decimal MoneyStrToDecimal(string moneyString)
        {
            return MoneyStrToDecimal(moneyString, decimalPlaces);
        }

        public decimal MoneyStrToDecimal(string moneyString, string decimalPlaces)
        {
            // Overload to allow a different number of decimal places to the Country format
            // Convert to decimal allowing currency symbol and blank
            moneyString = StripCurrency(moneyString.Trim());
            if (IsStrictNumeric(moneyString) && moneyString.Trim().Length > 0)
            {
                decimal moneyValue = Convert.ToDecimal(moneyString);
                // Reformat again in case the user entered more decimal places than the currency format
                string moneyReformatted = moneyValue.ToString(decimalPlaces);
                return Convert.ToDecimal(StripCurrency(moneyReformatted));
            }
            else
            {
                // Return zero instead of blank
                return 0.0M;
            }
        }

        public bool IsStrictNumeric(string text)
        {
            //Regex reg = new Regex("^[0-9.]*$");
            // (M49,M104) DSR 10/4/03 - Parse a decimal number
            // JPJ - decimal separator is not always '.' sometimes it is ',' (indonesia)
            string decimalPoint = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            //Regex reg = new Regex("(^([+-][0-9]*|[0-9]*)\\.[0-9]+$)|(^([+-][0-9]*|[0-9]*)$)");
            Regex reg = new Regex("(^([+-][0-9]*|[0-9]*)\\" + decimalPoint + "[0-9]+$)|(^([+-][0-9]*|[0-9]*)$)");
            return reg.IsMatch(text);
        }

        public void LoadAnswers(AnswerModel ansModel, CreditAnswerModel creditAnsModel)
        {
            creditAnsModel.CustId = ansModel.custId;

            for (int i = 0; i < ansModel.answers.Count; i++)
            {
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q1) == 0)//Date of Birth
                    creditAnsModel.Q1 = GetCultDateTime(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q4) == 0)//Home Address
                    creditAnsModel.Q4 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q9) == 0)//Marital status
                    creditAnsModel.Q9 = Convert.ToChar(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q10) == 0)//Dependents
                    creditAnsModel.Q10 = Convert.ToInt32(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q11) == 0)//Nationality
                    creditAnsModel.Q11 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q12) == 0)//Occupation
                    creditAnsModel.Q12 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q13) == 0)//Pay frequency
                    creditAnsModel.Q13 = Convert.ToChar(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q14) == 0)//Telephone Number (Work)
                    creditAnsModel.Q14 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q15) == 0)//Current employment start date
                    creditAnsModel.Q15 = GetCultDateTime(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q19) == 0)//Toatl net income
                    creditAnsModel.Q19 = Convert.ToDecimal(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q28) == 0)//Contacts
                {
                    string[] ContactDetails = ansModel.answers[i].answer.ToString().Split(';');
                    if (ContactDetails.Length > 1)
                    {
                        creditAnsModel.Q28 = Convert.ToString(ContactDetails[0]);
                        creditAnsModel.Q32 = Convert.ToString(ContactDetails[1]);
                    }
                    else
                    {
                        creditAnsModel.Q28 = string.Empty;
                        creditAnsModel.Q32 = Convert.ToString(ContactDetails[0]);
                    }

                }
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q42) == 0)// Id proof
                {
                    creditAnsModel.Q42 = System.Convert.FromBase64String(ansModel.answers[i].answer);
                    creditAnsModel.Q53 = Convert.ToString(ansModel.answers[i].filetype);
                }
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q43) == 0)//Address proof
                {
                    creditAnsModel.Q43 = System.Convert.FromBase64String(ansModel.answers[i].answer);
                    creditAnsModel.Q54 = Convert.ToString(ansModel.answers[i].filetype);
                }
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q44) == 0)//upload income proof
                {
                    creditAnsModel.Q44 = System.Convert.FromBase64String(ansModel.answers[i].answer);
                    creditAnsModel.Q55 = Convert.ToString(ansModel.answers[i].filetype);
                }
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q46) == 0)//Gender
                    creditAnsModel.Q46 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q47) == 0)//Employer name
                    creditAnsModel.Q47 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q48) == 0)//Work Address
                    creditAnsModel.Q48 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q49) == 0)//Rent or own
                    creditAnsModel.Q49 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q50) == 0)//years at current address
                    creditAnsModel.Q50 = Convert.ToInt32(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q51) == 0)//Total living expenses
                    creditAnsModel.Q51 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q52) == 0)//Total loan expenses
                    creditAnsModel.Q52 = Convert.ToString(ansModel.answers[i].answer);
                if (string.Compare(ansModel.answers[i].questionId.ToString(), CreditApproval.Q53) == 0)//upload income proof
                {
                    creditAnsModel.Q56 = System.Convert.FromBase64String(ansModel.answers[i].answer);
                    creditAnsModel.Q57 = Convert.ToString(ansModel.answers[i].filetype);
                }
            }

        }

        public DateTime GetCultDateTime(string dateString)
        {
            DateTime dateVal = DateTime.MinValue;
            string[] formats = { "M/d/yyyy", "d/M/yyyy", "M-d-yyyy", "d-M-yyyy", "d-MMM-yy", "d-MMMM-yyyy", };
            for (int i = 0; i < formats.Length; i++)
            {
                DateTime date;
                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    dateVal = date;
                    break;
                }
            }
            return dateVal;
        }

        public decimal GetAdditionalExpendicture(decimal netIncome, decimal NetIncomePercentage)
        {
            decimal additionalExpendicture = netIncome * NetIncomePercentage / 100;
            return additionalExpendicture;
        }

        public JResponse getDocumentStatus()
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository objTransaction = new TransactionRepository();
            List<CreditDcoumentStatus> objCDS = new List<CreditDcoumentStatus>();

            objCDS = objTransaction.CreditDocStatus();
            if (objCDS != null && objCDS.Count > 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objCDS);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = string.Empty;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "Record not available";
            }
            return objJResponse;
        }

        public JResponse TPCreateRFAccount(string CustId, TPTransactionConfirm TPTransactionConfirm)
        {

            CashLoanDetails det = null;
            bool rescore = false;
            bool reOpenS1 = false;
            bool reOpenS2 = false;
            string Error = string.Empty;
            string Country = string.Empty;
            short Branch;
            DateTime _propDate;
            string fileUploadError = string.Empty;

            JResponse objJResponse = new JResponse();
            TransactionRepository ObjTrans = new TransactionRepository();
            CreditApp objCrApp = new CreditApp();
            UploadDownloadDocument objUdd = new UploadDownloadDocument();

            // Check the previous account is exist or not
            CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
            dtCountryMaintenanceModel = ObjTrans.GetCountryMaintenceDetails(CustId);
            if (dtCountryMaintenanceModel != null && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.Message) && !string.IsNullOrWhiteSpace(dtCountryMaintenanceModel.StatusCode))
            {
                if (!dtCountryMaintenanceModel.StatusCode.Equals("200"))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = dtCountryMaintenanceModel.Message;//Need to create resource file.
                    return objJResponse;
                }
            }

            //Read Account Count and Vendor Count
            DataSet ds = ObjTrans.CheckAccount(CustId, TPTransactionConfirm.storeId);
            int AccountCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
            int VendorCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            if (VendorCount > 0)
            {
                if (AccountCount > 0)
                {

                    CreditAnswerModel creditAnsModel = new CreditAnswerModel();
                    TPLoadAnswers(CustId, creditAnsModel);

                    //Read Country maintenance data from DB.
                    DataSet DtCountryMaintenance = dtCountryMaintenanceModel.DtCountryMaintenance;
                    DataRow[] foundTemplateRow;

                    //Read Country and branch code from DB.
                    Country = (string)DtCountryMaintenance.Tables[0].Rows[0]["countrycode"];
                    Branch = (short)DtCountryMaintenance.Tables[0].Rows[0]["origbr"];

                    try
                    {
                        objCrApp = ObjTrans.CreateRFAccount(Country, Branch, creditAnsModel.CustId, 11111, false, ref det, out rescore, out reOpenS1, out reOpenS2,
                        out Error);
                    }
                    catch (Exception ex)
                    {
                        objJResponse.Result = string.Empty;
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = ex.Message;
                        return objJResponse;

                    }

                    string acctNo = objCrApp.accountNumber;


                    if (!string.IsNullOrEmpty(objCrApp.accountNumber))
                    {

                        decimal credit = creditAnsModel.Q59;
                        string result = String.Empty;
                        string refCode = String.Empty;
                        string newBand = String.Empty;
                        string targetFolderPath = string.Empty;
                        string idProof_TargetFileName = string.Empty;
                        string idProof_UploadedFileName = string.Empty;
                        string AddressProof_TargetFileName = string.Empty;
                        string AddressProof_UploadedFileName = string.Empty;
                        string IncomeProof_TargetFileName = string.Empty;
                        string IncomeProof_UploadedFileName = string.Empty;
                        string idProofSavedFileName = string.Empty;
                        string AddressProofSavedFileName = string.Empty;
                        string incomeProofSavedFileName = string.Empty;

                        objCrApp.accountNumber = acctNo.Replace("-", "");

                        //Read Stage1 proposal data
                        prop = ObjTrans.GetProposalStage1(creditAnsModel.CustId, objCrApp.accountNumber);

                        _propDate = ObjTrans.GetProposalDate(objCrApp.accountNumber, creditAnsModel.CustId);

                        //Folder path for saving the images/pdf.
                        targetFolderPath = string.Format("{0}\\", Path.Combine(System.Configuration.ConfigurationManager.AppSettings["UploadDocumentTargetFolderPath"], CustId, objCrApp.accountNumber));
                        string targetFileName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(creditAnsModel.Q53))
                        {
                            string ext = Path.GetExtension(creditAnsModel.Q53);
                            targetFileName = string.Format("IdProof1_{0}_{1}{2}", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"), ext);

                            UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                            CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                            objCustCreditDocument.AccountNumber = objCrApp.accountNumber;
                            objCustCreditDocument.CustId = CustId;
                            objCustCreditDocument.FileName = targetFileName;
                            objCustCreditDocument.FolderPath = targetFolderPath;
                            objCustCreditDocument.IsThirdParty = true;
                            string message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                            if (!message.Equals("No user found"))
                            {
                                if (!Directory.Exists(targetFolderPath))
                                    Directory.CreateDirectory(targetFolderPath);
                                File.Copy(creditAnsModel.Q53, Path.Combine(targetFolderPath, targetFileName));
                            }
                        }
                        else
                        {
                            fileUploadError = "Id proof 1 document";
                        }

                        if (!string.IsNullOrWhiteSpace(creditAnsModel.Q54))
                        {
                            string ext = Path.GetExtension(creditAnsModel.Q54);
                            targetFileName = string.Format("AddressProof_{0}_{1}{2}", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"), ext);

                            UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                            CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                            objCustCreditDocument.AccountNumber = objCrApp.accountNumber;
                            objCustCreditDocument.CustId = CustId;
                            objCustCreditDocument.FileName = targetFileName;
                            objCustCreditDocument.FolderPath = targetFolderPath;
                            objCustCreditDocument.IsThirdParty = true;
                            string message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                            if (!message.Equals("No user found"))
                            {
                                if (!Directory.Exists(targetFolderPath))
                                    Directory.CreateDirectory(targetFolderPath);
                                File.Copy(creditAnsModel.Q54, Path.Combine(targetFolderPath, targetFileName));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(fileUploadError))
                                fileUploadError += ", Address proof document";
                            else
                                fileUploadError = "Address proof document";
                        }

                        if (!string.IsNullOrWhiteSpace(creditAnsModel.Q55))
                        {
                            string ext = Path.GetExtension(creditAnsModel.Q55);
                            targetFileName = string.Format("IncomeProof_{0}_{1}{2}", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"), ext);

                            UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                            CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                            objCustCreditDocument.AccountNumber = objCrApp.accountNumber;
                            objCustCreditDocument.CustId = CustId;
                            objCustCreditDocument.FileName = targetFileName;
                            objCustCreditDocument.FolderPath = targetFolderPath;
                            objCustCreditDocument.IsThirdParty = true;
                            string message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                            if (!message.Equals("No user found"))
                            {
                                if (!Directory.Exists(targetFolderPath))
                                    Directory.CreateDirectory(targetFolderPath);
                                File.Copy(creditAnsModel.Q55, Path.Combine(targetFolderPath, targetFileName));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(fileUploadError))
                                fileUploadError += "and Income proof document";
                            else
                                fileUploadError = "Income proof document";
                        }
                        //Id proof 2
                        if (!string.IsNullOrWhiteSpace(creditAnsModel.Q57))
                        {
                            string ext = Path.GetExtension(creditAnsModel.Q57);
                            targetFileName = string.Format("IdProof2_{0}_{1}{2}", CustId, DateTime.Now.ToString("yyyyMMddHHmmss"), ext);

                            UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                            CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                            objCustCreditDocument.AccountNumber = objCrApp.accountNumber;
                            objCustCreditDocument.CustId = CustId;
                            objCustCreditDocument.FileName = targetFileName;
                            objCustCreditDocument.FolderPath = targetFolderPath;
                            objCustCreditDocument.IsThirdParty = true;
                            string message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                            if (!message.Equals("No user found"))
                            {
                                if (!Directory.Exists(targetFolderPath))
                                    Directory.CreateDirectory(targetFolderPath);
                                File.Copy(creditAnsModel.Q57, Path.Combine(targetFolderPath, targetFileName));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(fileUploadError))
                                fileUploadError += ", Id proof 2 document";
                            else
                                fileUploadError = "Id proof 2 document";
                        }

                        if (!string.IsNullOrWhiteSpace(fileUploadError))
                            fileUploadError += " does not exists.";

                        //For Auto approve account
                        ObjTrans.CompleteReferralStage(CustId, objCrApp.accountNumber, _propDate,
                            "Auto Approved", "", true, false, false, Branch, credit, Country, out Error);

                        //For Auto approve account
                        ObjTrans.SaveScoreHist(CustId, DateTime.Now, null, null, Convert.ToSingle(credit), "", Credential.UserId, "", objCrApp.accountNumber, out Error);

                        //Update vendor code and Create line item 
                        ObjTrans.TPInsertLineItem(CustId, TPTransactionConfirm.loanAmount, TPTransactionConfirm.numberOfInstallments, TPTransactionConfirm.storeId, objCrApp.accountNumber, Branch);

                    }

                    if (!string.IsNullOrEmpty(objCrApp.accountNumber))
                    {
                        //Mail Body
                        string htmlBody = File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TPMailContent"]));
                        htmlBody = htmlBody.Replace("@CustName", prop.Tables[TN.Customer].Rows[0]["FirstName"].ToString() + ' ' + prop.Tables[TN.Customer].Rows[0]["LastName"].ToString());
                        htmlBody = htmlBody.Replace("@AccountNo", objCrApp.accountNumber);
                        htmlBody = htmlBody.Replace("@Note", fileUploadError);

                        foundTemplateRow = DtCountryMaintenance.Tables[2].Select("TemplateName='TPCreditApproval'");

                        //Mail Subject
                        string subject = Convert.ToString(foundTemplateRow[0]["MailSubject"]);
                        subject = subject.Replace("@AccountNo", objCrApp.accountNumber);

                        MailTemplate MT = new MailTemplate();
                        MT.To = Convert.ToString(foundTemplateRow[0]["MailTo"]);
                        MT.Cc = Convert.ToString(foundTemplateRow[0]["MailCC"]);
                        MT.body = htmlBody;
                        MT.Subject = subject;

                        objCrApp.MailContent = MT;
                        objJResponse.Result = JsonConvert.SerializeObject(objCrApp);
                        objJResponse.Status = true;
                        objJResponse.StatusCode = (int)HttpStatusCode.OK;
                        objJResponse.Message = "Record saved successfully.";
                    }
                    else
                    {
                        objJResponse.Result = string.Empty;
                        objJResponse.Status = false;
                        objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                        objJResponse.Message = "Account not registered";
                    }
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    objJResponse.Message = "Register your account through the credit app questions.";
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "Please select vaild store.";
            }
            return objJResponse;

        }

        private bool TPSaveS1(String CustId, DataSet prop, string accountNumber, CreditAnswerModel creditAnsModel)
        {
            bool valid = true;
            try
            {
                bool sanction = false;
                #region applicant 1
                #region information from the customer tables
                DataRow r = prop.Tables[TN.Customer].Rows[0];

                r[CN.DOB] = creditAnsModel.Q1;//New
                r[CN.Title] = creditAnsModel.Q2;//Mandatory
                r[CN.MaritalStatus] = creditAnsModel.Q9;//New
                r[CN.Dependants] = creditAnsModel.Q10;//New
                r[CN.Nationality] = creditAnsModel.Q11;//New
                r[CN.DateIn] = creditAnsModel.Q60;
                r[CN.PrevDateIn] = _dateProp;//custDetails.Q18;//
                r[CN.Address1] = creditAnsModel.Q4;//New
                r[CN.PrevResidentialStatus] = creditAnsModel.Q49;// String.Empty;
                r[CN.PropertyType] = string.Empty;
                r[CN.Sex] = creditAnsModel.Q46;//Sex//new


                #endregion

                #region information from the bank tables
                if (prop.Tables[TN.Bank].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Bank].NewRow();
                    prop.Tables[TN.Bank].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Bank].Rows[0];

                r[CN.CustomerID] = CustId;
                r[CN.BankAccountOpened] = _dateProp;// custDetails.Q25;//Mandatory
                r[CN.Code] = string.Empty; //custDetails.Q24;//Mandatory

                #endregion

                #region information from the employment tables

                if (prop.Tables[TN.Employment].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Employment].NewRow();
                    prop.Tables[TN.Employment].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Employment].Rows[0];

                r = prop.Tables[TN.Employment].Rows[0];
                r[CN.DateEmployed] = creditAnsModel.Q15;//Mandatory//Need Default value
                r[CN.PrevDateEmployed] = _dateProp;//custDetails.Q16;
                r[CN.EmploymentStatus] = creditAnsModel.Q17;
                r[CN.PayFrequency] = creditAnsModel.Q13;
                r[CN.WorkType] = creditAnsModel.Q12; //"OT";//Default value Other.
                if (String.Compare(creditAnsModel.Q19.ToString(), "") == 0)
                    r[CN.AnnualGross] = DBNull.Value;
                else
                    r[CN.AnnualGross] = 12 * Convert.ToDouble((StripCurrency(creditAnsModel.Q19.ToString())));


                #endregion

                #region supplementary information which will go in the proposal table
                //there may or may not be a row in the proposal table
                if (prop.Tables[TN.Proposal].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Proposal].NewRow();
                    prop.Tables[TN.Proposal].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Proposal].Rows[0];

                r[CN.Occupation] = creditAnsModel.Q12;//New
                r[CN.AdditionalIncome] = creditAnsModel.Q20;//Mandatory
                r[CN.Commitments1] = creditAnsModel.Q21;
                //r[CN.Commitments2] = creditAnsModel.Q52;//Default
                //r[CN.Commitments3] = creditAnsModel.Q51;//custDetails.Q27;//Mandatory
                r[CN.MonthlyIncome] = creditAnsModel.Q19;//creditAnsModel.Q19;//Mandatory
                r[CN.MaritalStatus] = creditAnsModel.Q9;//New
                r[CN.Dependants] = creditAnsModel.Q10;//New
                r[CN.Nationality] = creditAnsModel.Q11;//New
                r[CN.DateProp] = _dateProp;//Default value

                r[CN.PrevEmpMM] = Convert.ToDateTime(creditAnsModel.Q15).Month;
                r[CN.PrevEmpYY] = Convert.ToDateTime(creditAnsModel.Q15).Year;
                r[CN.OtherPayments] = Convert.ToDecimal(0.0);
                if (!string.IsNullOrWhiteSpace(creditAnsModel.Q58))
                    r[CN.CCardNo1] = creditAnsModel.Q58;
                r[CN.CCardNo2] = string.Empty;
                r[CN.CCardNo3] = string.Empty;
                r[CN.CCardNo4] = string.Empty;
                r[CN.AdditionalExpenditure1] = Convert.ToDecimal(creditAnsModel.Q22);
                r[CN.AdditionalExpenditure2] = Convert.ToDecimal(creditAnsModel.Q23);
                r["PurchaseCashLoan"] = false;
                r[CN.RFCategory] = 1;
                r[CN.DistanceFromStore] = 0;
                r[CN.TransportType] = string.Empty;

                if (String.Compare(creditAnsModel.Q52.ToString(), "") == 0)
                    r[CN.Commitments2] = DBNull.Value;
                else
                    r[CN.Commitments2] = MoneyStrToDecimal(creditAnsModel.Q52);

                r[CN.Commitments3] = MoneyStrToDecimal(creditAnsModel.Q51);

                #endregion

                if (prop.Tables[TN.Agreements].Rows.Count == 0)
                {
                    r = prop.Tables[TN.Agreements].NewRow();
                    prop.Tables[TN.Agreements].Rows.Add(r);
                }
                else
                    r = prop.Tables[TN.Agreements].Rows[0];

                r[CN.PaymentMethod] = "W";

                TransactionRepository ObjTrans = new TransactionRepository();

                ObjTrans.SaveProposal(CustId, accountNumber, prop, prop2, sanction, out _error);
                #endregion

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return valid;
        }

        private bool TPSaveS2(String CustId, DataSet _stage2DataA1, string accountNumber, CreditAnswerModel creditAnsModel)
        {
            bool valid = true;
            TransactionRepository ObjTrans = new TransactionRepository();
            try
            {
                bool complete = true;

                #region save proposal table
                DataRow r;
                if (_stage2DataA1.Tables[TN.Proposal].Rows.Count == 0)
                {
                    r = _stage2DataA1.Tables[TN.Proposal].NewRow();
                    _stage2DataA1.Tables[TN.Proposal].Rows.Add(r);
                }
                else
                    r = _stage2DataA1.Tables[TN.Proposal].Rows[0];


                r[CN.PAddress1] = creditAnsModel.Q4;
                r[CN.PAddress2] = string.Empty; //custDetails.Q5;//
                r[CN.NewComment] = Convert.ToDecimal(0.0);// custDetails.Q27;//Mandatory
                r[CN.EmployeeName] = creditAnsModel.Q47;//New
                #endregion

                #region save references
                // Clear the old references
                _stage2DataA1.Tables[TN.References].Clear();

                r = _stage2DataA1.Tables[TN.References].NewRow();
                r[CN.RefFirstName] = creditAnsModel.Q28;
                r[CN.RefLastName] = string.Empty; //custDetails.Q29;//Mandatory
                r[CN.RefRelation] = string.Empty;//custDetails.Q30;//Mandatory
                r[CN.RefAddress1] = string.Empty; //custDetails.Q31;//Mandatory
                r[CN.RefPhoneNo] = creditAnsModel.Q32;//Mandatory
                r[CN.NewComment] = string.Empty; //custDetails.Q34;//
                r[CN.DateChange] = _dateProp; //custDetails.Q33;//
                r[CN.YrsKnown] = 0;
                r[CN.RefAddress2] = string.Empty;
                r[CN.RefCity] = string.Empty;
                r[CN.RefPostCode] = string.Empty;
                r[CN.RefWAddress1] = string.Empty;
                r[CN.RefWAddress2] = string.Empty;
                r[CN.RefWCity] = string.Empty;
                r[CN.RefWPostCode] = string.Empty;
                r[CN.RefDialCode] = string.Empty;
                r[CN.RefWDialCode] = string.Empty;
                r[CN.RefWPhoneNo] = string.Empty;
                r[CN.RefMDialCode] = string.Empty;
                r[CN.RefMPhoneNo] = string.Empty;
                r[CN.RefDirections] = string.Empty;
                r[CN.RefComment] = string.Empty;
                r[CN.EmpeeNoChange] = 0;
                _stage2DataA1.Tables[TN.References].Rows.Add(r);

                if (_stage2DataA2 != null)
                {
                    r = _stage2DataA2.Tables[TN.Customer].Rows[0];
                    r[CN.DeliveryArea] = string.Empty; //custDetails.Q7;//Mandatory
                }
                #endregion

                if (valid)
                {
                    ObjTrans.SaveProposalStage2(CustId, accountNumber, _stage2DataA1, _stage2DataA2, complete, out _error);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return valid;
        }

        public void TPLoadAnswers(string CustID, CreditAnswerModel creditAnsModel)
        {
            //Read existing Account information
            TransactionRepository ObjTrans = new TransactionRepository();
            DataSet ds = ObjTrans.GetRFAccountInformation(CustID);

            creditAnsModel.CustId = CustID;


            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                creditAnsModel.Q59 = Convert.ToDecimal(ds.Tables[0].Rows[0]["RFCreditLimit"]);
            }
            if (ds != null && ds.Tables != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows != null && ds.Tables[1].Rows.Count > 0)
            {
                string fullFilePath = string.Empty;
                DataRow[] proofDocuments = ds.Tables[1].Select("ProofType='IdProof1'");
                //Id proof 1
                if (proofDocuments != null && proofDocuments.Length > 0)
                {
                    fullFilePath = Convert.ToString(proofDocuments[0]["FullPath"]);
                    if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
                        creditAnsModel.Q53 = fullFilePath;
                }
                proofDocuments = ds.Tables[1].Select("ProofType='Address'");
                //Address proof
                if (proofDocuments != null && proofDocuments.Length > 0)
                {
                    fullFilePath = Convert.ToString(proofDocuments[0]["FullPath"]);
                    if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
                        creditAnsModel.Q54 = fullFilePath;
                }
                proofDocuments = ds.Tables[1].Select("ProofType='Income'");
                //Income proof
                if (proofDocuments != null && proofDocuments.Length > 0)
                {
                    fullFilePath = Convert.ToString(proofDocuments[0]["FullPath"]);
                    if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
                        creditAnsModel.Q55 = fullFilePath;
                }

                proofDocuments = ds.Tables[1].Select("ProofType='IdProof2'");
                //Income proof
                if (proofDocuments != null && proofDocuments.Length > 0)
                {
                    fullFilePath = Convert.ToString(proofDocuments[0]["FullPath"]);
                    if (!string.IsNullOrWhiteSpace(fullFilePath) && File.Exists(fullFilePath))
                        creditAnsModel.Q57 = fullFilePath;
                }

            }
        }

        public decimal GetLivingExpences(decimal Netincome, decimal loan, decimal netIncome)
        {
            //Calculate living expence using the NetIncome and Loan Expence
            decimal livingExpences = ((Netincome * netIncome / 100)) + loan;

            return livingExpences;
        }

        public JResponse UpdateTransaction(UpdateTransactionQueryString modelUpdateTransactionQueryString, UpdateTransactionBody modelUpdateTransactionBody)
        {
            JResponse objJResponse = new JResponse();

            try
            {
                UpdateTransactionResult objUTR = new UpdateTransactionResult();
                TransactionRepository ObjTrans = new TransactionRepository();
                string Error = string.Empty;
                string htmlBody = string.Empty;
                string subject = string.Empty;
                int result = 0;

                CountryMaintenanceModel dtCountryMaintenanceModel = new CountryMaintenanceModel();
                dtCountryMaintenanceModel = ObjTrans.GetCountryMaintenceDetails(modelUpdateTransactionQueryString.custId);
                DataSet DtCountryMaintenance = dtCountryMaintenanceModel.DtCountryMaintenance;
                DataRow[] foundTemplateRow;

                //Mail Body
                foundTemplateRow = DtCountryMaintenance.Tables[2].Select("TemplateName='UpdateTransaction'");
                htmlBody = File.ReadAllText(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["UpdateTransaction"]));
                htmlBody = htmlBody.Replace("@AccountNo", modelUpdateTransactionQueryString.acctNo);

                //Mail Subject
                subject = Convert.ToString(foundTemplateRow[0]["MailSubject"]);
                subject = subject.Replace("@AccountNo", modelUpdateTransactionQueryString.acctNo);

                //Accept transaction
                if (String.Compare(modelUpdateTransactionBody.status, UpdateTransactionConstants.Accepted) == 0)
                {

                    result = ObjTrans.ClearProposal(modelUpdateTransactionQueryString.acctNo, out Error);
                    htmlBody = htmlBody.Replace("@Status", UpdateTransactionConstants.Accepted);

                    if (result == 0)
                    {
                        objUTR.success = true;
                        objUTR.messages = "Transaction Updated Successfully.";
                        objJResponse.StatusCode = (int)HttpStatusCode.OK;
                        objJResponse.Message = "Transaction Updated successfully.";
                    }
                    else
                    {
                        objUTR.success = false;
                        objUTR.messages = "Transaction Not Updated successfully.";
                        objJResponse.StatusCode = (int)HttpStatusCode.OK;
                        objJResponse.Message = "Transaction Not Updated successfully.";
                    }
                }

                //Rejected transaction
                else if (String.Compare(modelUpdateTransactionBody.status, UpdateTransactionConstants.Rejected) == 0)
                {
                    htmlBody = htmlBody.Replace("@Status", UpdateTransactionConstants.Rejected);
                    objUTR.success = true;
                    objUTR.messages = "Transaction Updated Successfully.";
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Transaction Updated successfully.";


                }

                //Reverted transaction
                else if (String.Compare(modelUpdateTransactionBody.status, UpdateTransactionConstants.Reverted) == 0)
                {

                    htmlBody = htmlBody.Replace("@Status", UpdateTransactionConstants.Reverted);
                    objUTR.success = true;
                    objUTR.messages = "Transaction Updated Successfully.";
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Transaction Updated successfully.";

                }

                MailTemplate MT = new MailTemplate();
                MT.To = Convert.ToString(foundTemplateRow[0]["MailTo"]);
                MT.Cc = Convert.ToString(foundTemplateRow[0]["MailCC"]);
                MT.body = htmlBody;
                MT.Subject = subject;

                objUTR.MailContent = MT;


                objJResponse.Result = JsonConvert.SerializeObject(objUTR);
            }
            catch (Exception ex)
            {

                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "Record Not Updated successfully.";

            }
            return objJResponse;
        }

        public JResponse getEmailContract()
        {
            JResponse objJResponse = new JResponse();
            TransactionRepository objTransaction = new TransactionRepository();
            List<CutomerContract> objEmlCntr = new List<CutomerContract>();

            string path = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TPMailContract"]);
            string imageLink = ConfigurationManager.AppSettings.Get("UploadDocumentTargetFolderPath");
            imageLink = imageLink + "\\LogoImage.PNG";
            objEmlCntr = objTransaction.EmailContract(path, imageLink);
            if (objEmlCntr != null && objEmlCntr.Count > 0)
            {
                objJResponse.Result = JsonConvert.SerializeObject(objEmlCntr);
                objJResponse.Status = true;
                objJResponse.StatusCode = (int)HttpStatusCode.OK;
                objJResponse.Message = string.Empty;
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "Record not available";
            }
            return objJResponse;
        }

        public JResponse UpdateContractNotificationStatus(ContractNotificationStatus modelContractNotificationStatus)
        {
            string obj = XmlObjectSerializer.Serialize<ContractNotificationStatus>(modelContractNotificationStatus);

            JResponse objJResponse = new JResponse();
            TransactionRepository objCustomer = new TransactionRepository();
            List<string> Result = objCustomer.UpdateContractNotificationStatus(obj);

            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Status = true;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = "Email Notification updated successfully";
                }
                else
                {
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }
    }
}




