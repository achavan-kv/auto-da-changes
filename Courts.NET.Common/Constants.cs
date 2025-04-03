using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;


/* This file defines static strings for all column names used
 * in the application
 */

namespace STL.Common.Constants.ColumnNames
{

    // Column widths - should be extended for all hard coded column widths
    public struct CW
    {
        public const int CustomerID = 20;
        public const int DELFirstname = 20;
        public const int DELLastname = 20;
        public const int DELTitleC = 20;
        public const int AccountNo = 12;
        public const int Address1 = 50;
        public const int Address2 = 50;
        public const int Address3 = 50;
        public const int PostCode = 10;
        public const int DeliveryArea = 8;
        public const int ResStatus = 1;
        public const int PropType = 4;
        public const int AddressType = 2;
        public const int Email = 60;
        public const int DialCode = 8;
        public const int Phone = 20;
        public const int Ext = 6;
        public const int AddressNotes = 1000;
        public const int ItemNo = 18;                       //IP - 06/07/11 - CR1254 - RI - #3994 - changed from 8
        public const int Zone = 4;          //CR1084   
        public const int agrmtno = 10;  //CR 2018-13: To get Agreementno


        // CR866a These are all lookups
        public const int JobTitle = 4;
        public const int Organisation = 4;
        public const int Industry = 4;
        public const int EducationLevel = 4;
        public const int TransportType = 4;

    }

    //struct used for datatable column name constants
    public static class CN
    {
        #region MMI
        public const string Label = "Label";
        public const string FromScore = "FromScore";
        public const string ToScore = "ToScore";
        public const string MmiPercentage = "MmiPercentage";

        public const string MmiThresholdPercentage = "MmiThresholdPercentage";
        public const string IsMmiActive = "IsMmiActive";

        public const string MmiApplicable = "MmiApplicable";
        public const string MmiApplicableLength = "MmiApplicableLength";
        public const string MmiApplicableText = "MmiApplicableText";
        public const string MmiLimit = "MmiLimit";
        #endregion MMI

        public const string TotalAmountReceived = "TotalAmountReceived";
        public const string TotalCash = "TotalCash";
        public const string TotalCheque = "TotalCheque";
        public const string TotalPaymentPaid = "TotalPaymentPaid";
        public const string CashChange = "CashChange";
        public const string ChequeChange = "ChequeChange";

        public const string EmpeeNo = "EmpeeNo";
        public const string FtNotes = "FtNotes";
        public const string CashierTotalId = "CashierTotalId";
        public const string PostedToMambu = "PostedToMambu";
        public const string MambuError = "MambuError";
        public const string AlreadyAdded = "AlreadyAdded";
        public const string IsMambuAccount = "IsMambuAccount";// aandanan
        public const string A2CustomerID = "A2CustomerID";
        public const string A2Relation = "A2Relation";
        public const string Account = "Account";
        public const string AccountNo = "Account No.";
        public const string AccountNumber = "acctno";
        public const string AccountNumber2 = "AccountNumber";
        public const string AccountNumber3 = "Account_Number";
        public const string AccountStatus = "currstatus";
        public const string AccountStatus2 = "Current Status";
        public const string AccountType = "AccountType";
        public const string AccountType2 = "Account Type";
        public const string AcctCat = "acctcat";
        public const string AcctCatDesc = "acctcatdesc";
        public const string AcctLoaded = "AcctLoaded";
        public const string acctno = "acctno";
        public const string AcctNo = "AcctNo";
        public const string AcctNoBegin = "acctnobegin";
        public const string AcctNoLength = "acctnolength";
        public const string AccountNoCompany = "accountNoCompany";
        public const string AcctType = "accttype";
        public const string Action = "Action";
        public const string ActionCode = "ActionCode";
        public const string ActionDescription = "ActionDescription";
        public const string ActionNo = "ActionNo";
        public const string Actioned = "Actioned";    //#10535
        public const string ActionRequired = "ActionRequired";
        public const string ActionTaken = "ActionTaken";            //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string ActionValue = "ActionValue";
        public const string ActiveText = "ActiveText";
        public const string ActiveTextColor = "ActiveTextColor";
        public const string ActivityID = "Activity_ID";
        public const string ActivityName = "Activity_Name";
        public const string Actual = "Actual";
        public const string ActualCost = "ActualCost";
        public const string ActualWarrantySales = "Actual Warranty Sales";
        public const string ADComment = "ADComment";
        public const string Additional = "Additional";
        public const string Additional2 = "Additional2";           //IP - 07/12/11 - CR1234
        public const string AdditionalLength = "AdditionalLength"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string Additional2Length = "Additional2Length"; //IP - 07/12/11 - CR1234
        public const string AdditionalHeaderText = "AdditionalHeaderText"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string Additional2HeaderText = "Additional2HeaderText"; //IP - 07/12/11 - CR1234
        public const string AdditionalCost = "AdditionalCost";
        public const string AdditionalExpenditure1 = "AdditionalExpenditure1";
        public const string AdditionalExpenditure2 = "AdditionalExpenditure2";
        public const string AdditionalIncome = "AdditionalIncome";
        public const string AdditionalIncome2 = "AdditionalIncome2";
        public const string AdditionalLimit = "AdditionalLimit";
        public const string AdditionalPercent = "AdditionalPercent";
        public const string DELFirstname = "DELFirstname";//KD
        public const string DELLastname = "DELLastname";//KD
        public const string DELTitle = "DELTitle";
        public const string Address1 = "Address1";
        public const string Address2 = "Address2";
        public const string Address3 = "Address3";
        public const string AddressPC = "AddressPC";
        public const string AddressType = "AddressType";
        public const string AddTo = "addto";
        public const string AddType = "AddType";
        public const string AdminPcent = "adminpcent";
        public const string AdminFee = "admin fee";
        public const string ADReqd = "ADReqd";
        public const string AdtnlLbrCostEstimate = "AdtnlLbrCostEstimate"; // CR 1024 (NM 29/04/2009)
        public const string AIGClaim = "EW Claim";      //CR1030 jec
        public const string Affinity = "Affinity";
        public const string AffinityFormatDate = "AffinityFormatDate";
        public const string Age = "age";
        public const string AgreementDate = "dateagrmt";
        public const string AgreementDueDate = "AgreementDueDate";
        public const string AgreementNo = "AgreementNo";
        public const string AgreementNum = "Agreement No.";  //CR36
        public const string AgreementPrint = "agreementprint";
        public const string AgreementPrintDesc = "agreementprintdesc";
        public const string AgreementText = "agrtext";
        public const string AgreementTotal = "AgreementTotal";
        public const string AgreementTotal2 = "Agreement Total";
        public const string AgrmtNo = "AgrmtNo";
        public const string AgrmtTotal = "agrmtTotal";
        public const string Alias = "Alias";
        public const string AllocArrears = "allocarrears";
        public const string AllocatedFloat = "allocatedfloat";
        public const string AllocCount = "alloccount";
        public const string AllocEmpeeName = "allocempeename";
        public const string AllocNo = "AllocNo";
        public const string AllocPercent = "allocpercent";
        public const string AllocPrtFlag = "allocprtflag";
        public const string AllowCancel = "AllowCancel";                //IP - 10/02/11 - Sprint 5.10 - #2978
        public const string AllowReuse = "AllowReuse";
        public const string AllowWriteOff = "AllowWriteOff";            //IP - 08/02/11 - Sprint 5.10 - #2977
        public const string AllowTransfer = "AllowTransfer";            //IP - 20/02/12 - #9633 - CR1234
        public const string AllowReopenS1 = "AllowReopenS1";                  //#10477
        public const string Amount = "Amount";
        public const string AmountDue = "Amount Due";
        public const string AmtCommPaidOn = "amtcommpaidon";
        public const string AnnualGross = "AnnualGross";
        public const string AnswerNo = "AnswerNo";
        public const string AnswerYes = "AnswerYes";
        public const string ApplicationStatus = "ApplicationStatus";
        public const string ApplicationType = "ApplicationType";
        public const string ApplicantSpendFactorInPercent = "ApplicantSpendFactorInPercent";
        public const string ApprovalDate = "ApprovalDate";
        public const string APR = "apr";
        public const string ArrangementAmount = "Arrangement Amount"; //IP & JC - 12/01/09 - CR976
        public const string Archived = "archived";
        public const string Arrears = "Arrears";
        public const string ArrearsExCharges = "arrearsexcharges";
        public const string ArrearsGroup = "arrearsgroup";
        public const string ArrearsLevel = "Arrears Level";
        public const string ArrearsLevel2 = "arrearslevel";
        public const string AS400BranchNo = "as400branchno";
        public const string Ascii = "Ascii";
        public const string AssemblyRequired = "assemblyrequired";
        public const string Assigned = "assigned";
        public const string AssociatedItem = "AssociatedItem";
        public const string AssociatedItemID = "associateditemid";          // RI
        public const string AssociatedItemIndicator = "associateditemindicator";
        public const string Attachmentpct = "Attachment %";
        public const string AvailableCredit = "AvailableCredit";
        public const string AvailableSpend = "AvailableSpend";
        public const string AvailableStock = "availablestock";
        public const string AvgRunTime = "AvgRunTime";
        public const string avvtt = "avvtt";
        public const string AgreementInvoiceNumber = "AgreementInvoiceNumber";
        public const string BailFee = "bailfee";
        public const string BailiffCommissionEqualsFee = "bcommissionequalsfee";
        public const string BailiffFee = "bailifffee";
        public const string BalAfterRepo = "balanceafterrepo";
        public const string Balance = "Balance";
        public const string BalBeforeRepo = "balancebeforerepo";
        public const string BalExclInt = "balexclint";
        public const string Band = "Band";
        public const string Bank = "Bank";
        public const string BankAccountName = "BankAccountName";
        public const string BankAccountNo = "BankAccountNo";
        public const string BankAccountNo2 = "bankacctno";
        public const string BankAccountOpened = "BankAccountOpened";
        public const string BankAccountType = "BankAccountType";
        public const string BankAddress1 = "bankaddr1";
        public const string BankAddress2 = "bankaddr2";
        public const string BankAddress3 = "bankaddr3";
        public const string BankPostCode = "bankpocode";
        public const string BankBranchNo = "BankBranchNo";
        public const string BankCode = "bankcode";
        public const string BankedValue = "bankedvalue";
        public const string BankName = "bankname";
        public const string BankOrder = "BankOrder";
        public const string Bankruptcies = "bankruptcies";
        public const string Bankruptcies12Months = "bankruptcies12months";
        public const string Bankruptcies24Months = "bankruptcies24months";
        public const string BankruptciesAvgValue = "bankruptciesavgvalue";
        public const string BankruptciesTimeSinceLast = "bankruptciestimesincelast";
        public const string BankruptciesTotalValue = "bankruptciestotalvalue";
        public const string BatchNo = "batchno";
        public const string BatchHeaderId = "BatchHeaderId";
        public const string BatchHeaderIdBegin = "BatchHeaderIdBegin";
        public const string BatchHeaderIdLength = "BatchHeaderIdLength";
        public const string BatchHeaderHasTotal = "BatchHeaderHasTotal";
        public const string BatchHeaderMoneyBegin = "BatchHeaderMoneyBegin";
        public const string BatchHeaderMoneyLength = "BatchHeaderMoneyLength";
        public const string BatchTrailerId = "BatchTrailerId";
        public const string BatchTrailerIdBegin = "BatchTrailerIdBegin";
        public const string BatchTrailerIdLength = "BatchTrailerIdLength";
        public const string BDWBalance = "bdwbalance";
        public const string BDWCharges = "bdwcharges";
        public const string BERCheck = "BERCheck";
        //public const string BehaviouralScoring = "BehaviouralScoring"; //IP - 08/04/10 - CR1034 - Removed
        public const string Blank = "Blank";
        public const string BookingType = "BookingType";
        public const string BookingID = "BookingID";            // #10230
        public const string BranchAddress1 = "branchaddr1";
        public const string BranchAddress2 = "branchaddr2";
        public const string BranchAddress3 = "branchaddr3";
        public const string BranchName = "branchname";
        public const string BranchNo = "branchno";
        public const string Branch = "Branch";      // jec CR1035
        public const string BranchNumber = "branch number";
        public const string BranchPostCode = "branchpocode";
        public const string BranchSet = "Branch Set";
        public const string BranchSplit = "branchsplit";
        public const string BranchSplitBalancing = "branchsplitbalancing";
        public const string Brand = "Brand";                                        //IP - 20/09/11 - RI - #8220 - CR8201  
        public const string BuffBranchNo = "BuffBranchNo";
        public const string BuffNo = "BuffNo";
        public const string CalculatedFee = "CalculatedFee";
        public const string CallsPerDay = "CallsPerDay";
        public const string CancellationTotal = "CancellationTotal";
        public const string CancelDesc = "CancelDesc";
        public const string CancellationRejectionCode = "CancellationRejectionCode";
        public const string Cancelled = "cancelled";
        public const string CancelReason = "CancelReason";
        public const string CardName = "CardName";                  //IP - 06/01/11 - Store Card
        public const string CardNumber = "CardNumber";              //IP - 06/01/11 - Store Card
        public const string CardPrinted = "CardPrinted";
        public const string CardType = "cardtype";
        public const string CarrierNumber = "carrierNumber";
        public const string CashBackAmount = "cashbackamount";
        public const string CashBackMonth = "cashbackmonth";
        public const string CashBackPc = "cashbackpc";
        public const string CashLoan = "cashLoan";
        public const string CashLoanBlocked = "CashLoanBlocked";    //IP - 03/11/11 - CR1232 - Cash Loans
        public const string CashierName = "CashierName";            //IP - 17/05/12 - CR1239
        public const string CashierEmpeeNo = "CashierEmpeeNo";      //IP - 17/05/12 - CR1239
        public const string CashPrice = "CashPrice";
        public const string CashValue = "cashvalue";
        public const string CatDescript = "catdescript";
        public const string Category = "Category";
        public const string CCardNo1 = "CCardNo1";
        public const string CCardNo2 = "CCardNo2";
        public const string CCardNo3 = "CCardNo3";
        public const string CCardNo4 = "CCardNo4";
        public const string Change = "Change";                      //IP - 21/05/12 - #10145 - CR1239
        public const string ChangeAmt = "ChangeAmt";                      //IP - 21/05/12 - #10145 - CR1239
        public const string CashChangeAmt = "CashChangeAmt";
        public const string ChequeChangeAmt = "ChequeChangeAmt";
        public const string ChangedBy = "ChangedBy";
        public const string Charges = "Charges";
        public const string ChargeTo = "ChargeTo";
        public const string ChargeToAcct = "ChargeToAcct";      // CR1030 jec
        public const string ChargeAcctWrittenOff = "ChargeAcctWrittenOff";  //IP - 09/02/11 - Sprint 5.10 - #2977
        public const string ChargeToChangedBy = "ChargeToChangedBy";
        public const string ChargeToCustomer = "ChargeToCustomer";
        public const string ChargeToMake = "ChargeToMake";
        public const string ChargeToModel = "ChargeToModel";
        public const string ChargeType = "ChargeType";
        public const string CheckType = "checktype";
        public const string ChequeColln = "chequecolln";
        public const string ChequeClearance = "chequeClearance";
        public const string ChequeNo = "chequeno";
        public const string Claim = "Claim"; //CR 822 [PC] 29-Sep-2006
        public const string ClaimNumber = "claimnumber";
        public const string Class = "Class";        //CR1212 jec
        public const string ClearedBy = "clearedby";

        public const string CMAcctno = "acctno";
        public const string CMEmpeeno = "empeeno";
        public const string CMSolicitorNo = "solicitorNo";
        public const string CMAuctionProceeds = "auctionProceeds";
        public const string CMAuctionDate = "auctionDate";
        public const string CMAuctionAmount = "auctionAmount";
        public const string CMCourtDeposit = "courtDeposit";
        public const string CMCourtAmount = "courtAmount";
        public const string CMCourtDate = "courtDate";
        public const string CMCaseClosed = "caseClosed";
        public const string CMMentionDate = "mentionDate";
        public const string CMMentionCost = "mentionCost";
        public const string CMPaymentRemittance = "paymentRemittance";
        public const string CMJudgement = "judgement";
        public const string CMLegalAttachmentDate = "legalAttachmentDate";
        public const string CMLegalInitiatedDate = "legalInitiatedDate";
        public const string CMDefaultedDate = "defaultedDate";
        public const string CMUserNotes = "userNotes";
        public const string CMInitiatedDate = "initiatedDate";
        public const string CMFullOrPartClaim = "fullOrPartClaim";
        public const string CMInsAmount = "insAmount";
        public const string CMInsType = "insType";
        public const string CMIsApproved = "isApproved";
        public const string CMFraudInitiatedDate = "fraudInitiatedDate";
        public const string CMIsResolved = "isResolved";
        public const string CMTRCInitiatedDate = "traceInitiatedDate";

        public const string Code = "code";
        public const string CodeDescript = "codedescript";
        public const string CodeDescription = "codedescript";
        public const string CodeHeaderText = "CodeHeaderText"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string CodeName = "CodeName";
        public const string CodeLength = "CodeLength"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string CODPercentage = "codpercentage";
        public const string CollectionFee = "collectionfee";
        public const string CollectionPercent = "collectionpercent";
        public const string CollectionReason = "collectionreason";
        public const string CollectionType = "collecttype";
        public const string ColourName = "ColourName";
        public const string Column_Name = "column_name"; // CR 976
        public const string Comment = "Comment";
        public const string Comments = "Comments";
        public const string Commission = "Commission";
        public const string CommissionAmount = "CommissionAmount";
        public const string Commission_Amount = "Commission Amount";    //jec 03/07/08 UAT442
        public const string CommissionDays = "CommissionDays";
        public const string CommissionPercent = "commnpercent";
        public const string Commission_Percent = "Commission %";        //jec 03/07/08 UAT442
        public const string CommissionTotal = "CommissionTotal";
        public const string CommissionType = "CommissionType";
        public const string Commission_Type = "Commission Type";
        public const string Commitments1 = "Commitments1";
        public const string Commitments2 = "Commitments2";
        public const string Commitments3 = "Commitments3";
        public const string Committed = "Committed"; //66669
        public const string CommnDue = "commndue";
        public const string CommSetVal = "CommSetVal";              //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string ComponentID = "ComponentID";
        public const string ComponentNo = "ComponentNo";
        public const string Condition = "Condition";
        public const string ConditionCode = "ConditionCode";
        public const string ConfirmLetter = "ConfirmLetter";
        public const string Consent = "Consent";
        public const string ContractNo = "contractno";
        public const string Corrected = "Corrected";
        public const string Cost = "Cost";
        public const string CostPrice = "CostPrice"; //CR802
        public const string Country = "Country";
        public const string CountryCode = "countrycode";
        public const string CountType1 = "counttype1";
        public const string CountType2 = "counttype2";
        public const string CountValue = "countvalue";
        public const string Courts_Code = "Courts Code";    //IP - 08/07/11 - CR1254 - RI 
        public const string CourtsCode = "CourtsCode";
        public const string CourtsVoucher = "courtsVoucher";
        public const string CreatedBy = "createdby";
        public const string CreateRFAccounts = "CreateRFAccounts";      //CR903 jec
        public const string CreateHPAccounts = "CreateHPAccounts";      //CR903 jec
        public const string CreateStoreAccounts = "CreateStoreAccounts";
        public const string CreateCashAccounts = "CreateCashAccounts";      //CR903 jec
        public const string CreatedByName = "createdbyname";
        public const string CreditBlocked = "CreditBlocked";                //IP - 28/04/10 - UAT(983) UAT5.2
        public const string CreditFee = "CreditFee";                //IP - 28/04/10 - UAT(983) UAT5.2
        public const string CreditLimit = "creditlimit";
        public const string CreditScanInterval = "CreditScanInterval";
        public const string CreditWarrantyDays = "creditwarrantydays";
        public const string CroffNo = "croffno";
        public const string Culture = "Culture";
        public const string CurAcctNo = "CurAcctNo";
        public const string CurAmount = "CurAmount";
        public const string CurBankAcctName = "CurBankAcctName";
        public const string CurBankAcctNo = "CurBankAcctNo";
        public const string CurBankBranchNo = "CurBankBranchNo";
        public const string CurBankCode = "CurBankCode";
        public const string CurConsent = "CurConsent";
        public const string CurDDPending = "CurDDPending";
        public const string CurDueDayId = "CurDueDayId";
        public const string CurEndDate = "CurEndDate";
        public const string CurRejectAction = "CurRejectAction";
        public const string Currency = "Currency";
        public const string CurrentAccounts = "CurrentAccounts";
        public const string CurrentBand = "CurrentBand";
        public const string CurrentStatus = "Current Status";
        public const string CurrentStep = "currentstep";
        public const string CurrStatus = "CurrStatus";
        public const string CurStartDate = "CurStartDate";
        public const string CurWorst = "CurWorst";
        public const string cusaddr1 = "cusaddr1";
        public const string cusaddr2 = "cusaddr2";
        public const string cusaddr3 = "cusaddr3";
        public const string cusnotes = "cusnotes";
        public const string DELLastName = "DELLastName";

        public const string DELTitleC = "DELTitleC";
        public const string cuspocode = "cuspocode";
        public const string Latitude = "Latitude"; // Address Standardization CR2019 - 025
        public const string Longitude = "Longitude"; // Address Standardization CR2019 - 025
        public const string CustID = "custid";
        public const string CosacsAcctNo = "CosacsAcctNo";
        public const string CustomerAddress = "CustomerAddress";
        public const string Customer = "Customer";
        public const string CustomerCollected = "CustomerCollected";      //CR 949/958
        public const string CustomerID = "CustomerID";
        public const string CustomerID2 = "Customer ID";
        public const string CustomerID3 = "CustomerIC_Number";
        public const string CustomerName = "CustomerName";
        public const string CycleToNextFlag = "CycleToNextFlag"; //NM & IP - 29/12/08 - CR976
        public const string CustomerDebitAmount = "Customer Debit Amount";
        public const string Damaged = "damaged";
        public const string Data = "data";
        public const string Date = "Date"; //CR 802
        public const string DateAcctLttr = "dateacctlttr";
        public const string DateAcctOpen = "dateacctopen";
        public const string DateAction = "DateAction";
        public const string DateAdded = "DateAdded";
        public const string DateAlloc = "datealloc";
        public const string DateAllocated = "DateAllocated";
        public const string DateAuthorised = "dateauthorised";
        public const string DateBegin = "datebegin";
        public const string DateChange = "DateChange";
        public const string DateChanged = "DateChanged";
        public const string DateCleared = "datecleared";
        public const string DateClosed = "DateClosed";
        public const string DateClosedOrig = "DateClosedOrig";
        public const string DateCollected = "DateCollected";
        public const string DateCommissionCalculated = "DateCommissionCalculated";
        public const string DateDealloc = "datedealloc";
        public const string DateDel = "datedel";
        public const string DateDelivered = "Date Delivered";
        public const string DateDelPlan = "DateDelPlan";
        public const string DateDeposit = "datedeposit";
        public const string DateDiscon = "DateDiscon";
        public const string DateDNPrinted = "datednprinted";
        public const string DateDue = "Date Due";
        public const string DateEffective = "DateEffective";
        public const string DateEmployed = "DateEmployed";
        public const string DateEmpStart = "DateEmpStart";
        public const string DateExpires = "expires";
        public const string DateExpiry = "DateExpiry";
        public const string DateFinish = "datefinish";
        public const string DateFirst = "DateFirst";
        public const string DateFormat = "dateformat";
        public const string DateFrom = "DateFrom";
        public const string DateIn = "DateIn";
        public const string DateInAddress = "dateinaddress";
        public const string DateIssued = "dateissued";
        public const string DateLastChanged = "DateLastChanged";
        public const string DateLastPaid = "datelastpaid";
        public const string DateLast = "datelast";
        public const string DateLength = "datelength";
        public const string DateLogged = "DateLogged";
        public const string DateLoggedOrig = "DateLoggedOrig";
        public const string DateLoggedStr = "DateLoggedStr";
        public const string DateMoved = "DateMoved";
        public const string DateNextDue = "datenextdue";
        public const string DateNextPaymentDue = "datenextpaymentdue";
        public const string DateOpened = "Date Opened";
        public const string DateOpened1 = "DateOpened"; //IP - 09/06/08 - Account Status screen
        public const string DateOut = "DateOut";
        public const string DatePayment = "DatePayment";
        public const string DatePCChange = "datepcchange";
        public const string DatePEmpStart = "DatePEmpStart";
        public const string DatePlanDel = "DatePlanDel";
        public const string DatePresented = "datepresent";
        public const string DatePrinted = "dateprinted";
        public const string DatePromised = "DatePromised";
        public const string DateProp = "DateProp";
        public const string DateRemoved = "DateRemoved";
        public const string DateReOpened = "DateReopened";
        public const string DateReqDel = "DateReqDel";
        public const string DateResolved = "DateResolved";
        public const string DateStart = "datestart";
        public const string DateStatChge = "datestatchge";
        public const string DateTaken = "datetaken";
        public const string DateTelAdd = "DateTelAdd";
        public const string DateTo = "DateTo";
        public const string DateTrans = "DateTrans";
        public const string DCText1 = "DCText1";
        public const string DCText2 = "DCText2";
        public const string DCText3 = "DCText3";
        public const string DDPending = "DDPending";
        public const string DeallocEmpeeName = "deallocempeename";
        public const string DebitAccount = "debitaccount";
        public const string DecimalPlaces = "DecimalPlaces";
        public const string Decision = "Decision";
        public const string DefaultChargeTo = "DefaultChargeTo";
        public const string DefaultDelDays = "DefaultDelDays";
        public const string DefaultDeposit = "defaultdeposit";
        public const string Defaults = "defaults";
        public const string DefaultsBalance = "defaultsbalance";
        public const string DefaultsExMotor = "defaultsexmotor";
        public const string DefaultsExMotorBalance = "defaultsexmotorbalance";
        public const string DefaultPrintLocation = "defaultPrintLocation"; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public const string DefaultTerm = "defaultterm";
        public const string DefaultVoucherExpiry = "defaultvoucherexpiry";
        public const string DeferredIntMonths = "deferredintmonths";
        public const string DeferredMonths = "deferredmonths";
        public const string Deleted = "Deleted";
        public const string Delivered = "Delivered";      //CR 949/958
        public const string Deliverer = "Deliverer";
        public const string DeliveredIndicator = "DeliveredIndicator";
        public const string DeliverNonStocks = "delnonstocks";
        public const string DeliveryAddress = "deliveryaddress";
        public const string DeliveryAmount = "DeliveryAmount";
        public const string DeliveryArea = "deliveryarea";
        public const string DeliveryDate = "DeliveryDate";
        public const string DeliveryDamage = "DeliveryDamage";
        public const string DeliveryFlag = "DeliveryFlag";
        public const string DeliveryNoteNumber = "DeliveryNoteNumber";
        public const string DeliveryProcess = "DeliveryProcess";
        public const string DeliverySlot = "deliveryslot";
        public const string DeliveryTotal = "DeliveryTotal";    // jec 03/07/08 UAT442
        public const string DelNoteBranch = "delnotebranch";
        public const string DelAmount = "Delivery Amount";      // jec 03/07/08 UAT442
        public const string DelOrColl = "DelOrColl";
        public const string DelCol = "Del/Col";
        public const string Delimiter = "Delimiter";                                      //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string DelimitedNoOfCols = "DelimitedNoOfCols";                      //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string DelimitedAcctNoColNo = "DelimitedAcctNoColNo";                //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string DelimitedDateColNo = "DelimitedDateColNo";                    //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string DelimitedMoneyColNo = "DelimitedMoneyColNo";                  //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string DelQty = "DelQty";
        public const string DelToFact = "deltofact";
        public const string DelType = "deltype";
        public const string Department = "Department";
        public const string Dependants = "Dependants";
        public const string DependantsFromProposal = "DependantsFromProposal";
        public const string DependnetSpendFactorInPercent = "DependnetSpendFactorInPercent";
        public const string Deposit = "Deposit";
        public const string DepositAmount = "DepositAmount";
        public const string DepositID = "depositid";
        public const string DepositIsPercentage = "DepositIsPercentage";
        public const string DepositPaid = "DepositPaid";
        public const string DepositScreenLocked = "depositscreenlocked";
        public const string DepositType = "deposittype";
        public const string DepositTypeCode = "deposittypecode";
        public const string DepositValue = "depositvalue";
        public const string descr1 = "descr1";
        public const string descr1_en = "descr1_en";
        public const string descr2 = "descr2";
        public const string descr2_en = "descr2_en";
        public const string Description = "description";
        public const string Description1 = "Description1";        // RI
        public const string Description2 = "Description2";        // RI
        public const string DescriptionHeaderText = "DescriptionHeaderText"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string DescriptionLength = "DescriptionLength"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string DHLInterfaceDate = "DHLInterfaceDate";      //UAT114 21/04/10
        public const string DialCode = "DialCode";
        public const string Difference = "Difference";
        public const string Directions = "Directions";
        public const string Discount = "Discount";
        //CR 866 Added distance from store
        public const string DistanceFromStore = "DistanceFromStore";
        public const string DOB = "DOB";
        public const string DoDefault = "dodefault";
        public const string DoNextRun = "donextrun";
        public const string DoNotSecuritise = "DoNotSecuritise";
        public const string DriverName = "drivername";
        public const string DropDownName = "DropDownName";
        public const string DueDate = "DueDate";
        public const string DueDay = "DueDay";
        public const string DueDayId = "DueDayId";
        public const string Duration = "Duration";
        public const string DutyFree = "dutyfree";
        public const string EAddress1 = "EAddress1";
        public const string EAddress2 = "EAddress2";
        public const string ECity = "ECity";
        //CR 866 Added education level
        public const string EducationLevel = "EducationLevel";
        public const string EffDate = "EffDate";
        public const string EffDay = "EffDay";
        public const string Effect = "Effect";
        public const string EffectiveDate = "EffectiveDate";
        public const string ElectricalLimit = "eleclimit";
        public const string Electrical = "Electrical";      //CR1030 jec 01/02/11
        public const string Email = "Email";
        public const string EmpDept = "EmpDept";
        public const string EmpeeNoAuth = "empeenoauth";
        public const string EmpeeNoChange = "EmpeeNoChange";
        public const string EmpeeNoSale = "empeenosale";
        public const string EmpeeNoStat = "empeenostat";
        public const string Employee = "Employee";
        public const string EmployeeName = "EmployeeName";
        public const string EmployeeNo = "EmpeeNo";
        public const string EmployeeNoEntered = "empeenoentered";
        public const string EmployeeNoSpa = "EmpeeNoSpa";
        public const string EmployeeType = "EmpeeType";
        public const string Employer = "Employer";
        public const string EmploymentDialCode = "EmploymentDialCode";
        public const string EmploymentStatus = "EmploymentStatus";
        public const string EmploymentTelNo = "EmploymentTelNo";
        public const string EmpyrAddr1 = "EmpyrAddr1";
        public const string EmpyrAddr2 = "EmpyrAddr2";
        public const string EmpyrAddr3 = "EmpyrAddr3";
        public const string EmpyrName = "EmpyrName";
        public const string EmpyrPOCode = "EmpyrPOCode";
        public const string EndDate = "EndDate";
        public const string English = "English";
        public const string EPostCode = "EPostCode";
        public const string Error = "Error";
        public const string ErrorDate = "ErrorDate";
        public const string ErrorText = "ErrorText";
        public const string Ethnicity = "Ethnicity";
        public const string Event = "Event";
        public const string ExchangeContractNo = "ExchangeContractNo";
        public const string ExcelGen = "ExcelGen";
        public const string ExchangeRate = "Rate";
        public const string Exit = "Exit";
        public const string ExpectedReturnDate = "ExpectedReturnDate";
        public const string Expenses = "expenses";
        public const string ExpirationYear = "ExpirationYear";        //IP - 06/01/11 - Store Card
        public const string ExpirationMonth = "ExpirationMonth";        //IP - 06/01/11 - Store Card
        public const string ExpiredPortion = "ExpiredPortion";        //CR1094 jec
        public const string Express = "Express";                      //IP - 07/06/12 - #10229 - Warehouse & Deliveries
        public const string OddPayment = "Odd Payment"; //IP & JC - CR976
        public const string ExtNo = "ExtnNo";
        public const string ExtendTerm = "ExtendTerm"; //IP & JC - CR976
        public const string ExtWarranty = "ExtWarranty";
        public const string ExtWarrantyOrig = "ExtWarrantyOrig";
        public const string FACT2000BranchLetter = "FACT2000BranchLetter";
        public const string FACTEmployeeNo = "FACTEmployeeNo";
        public const string FACTTranType = "TranType";
        public const string FailureReason = "FailureReason";    // CR949/958
        public const string FailureBookingId = "FailureBookingId";    //#10535
        public const string FailedQty = "FailedQty";    // #10221
        public const string FalseStep = "FalseStep";
        public const string Fault = "Fault"; // UAT 453
        public const string FeesAndInterest = "feesandinterest";
        public const string FileName = "filename";
        public const string FileNameDate = "FileNameDate";
        public const string FinalDec = "FinalDec";
        public const string FinalInstal = "finalinstal";
        public const string FinalPayDate = "FinalPayDate"; //IP & JC - CR976
        public const string FirstPayDate = "FirstPayDate"; //IP & JC - CR976
        public const string FirstName = "FirstName";
        public const string FirstName2 = "First Name";
        public const string DELFirstName = "DELFirstName";
        public const string FoodLoss = "FoodLoss";
        public const string FoodLossVal = "Food Loss Value";
        public const string FootNotes = "FTNotes";
        public const string ForDeposit = "fordeposit";
        public const string Forecast = "Forecast";
        public const string ForeignTender = "foreigntender";
        public const string FreeInstalment = "FreeInstalment";
        public const string FreezeIntAdmin = "FreezeIntAdmin"; //IP & JC - CR976
        public const string FromMonth = "frommonth";
        public const string FTNotes = "ftnotes";
        public const string FullOrPartTime = "FullOrPartTime";
        public const string FullRebateDays = "fullrebatedays";
        public const string FurnitureLimit = "furnlimit";
        public const string Furniture = "Furniture";      //CR1030 jec 02/01/11
        public const string FYW = "FYW";
        public const string MAN = "MAN"; //rm CR1051 change FYW to MAN
        public const string FYWarranty = "FYWarranty";
        public const string MANWarranty = "MANWarranty"; //rm CR1051 change FYW to MAN
        public const string FYWtic = "FYWtic";
        public const string GiftVoucherAccount = "giftvoucheraccount";
        public const string GoodsOnLoan = "GoodsOnLoan";
        public const string GoodsOnLoanCollected = "GoodsOnLoanCollected";
        public const string GrtCreatedOn = "GrtCreatedOn"; //IP - 16/02/10 - CR1072 - CR1048 - 3.1.8 (ref:3.1.46 & 3.1.47) from 4.3
        public const string GrtCreatedBy = "GrtCreatedBy"; //IP - 16/02/10 - CR1072 - CR1048 - 3.1.8 (ref:3.1.46 & 3.1.47) from 4.3
        public const string HasTotalled = "hastotalled";
        public const string HasTrailer = "hastrailer";
        public const string HeaderWarehouseNo = "HeaderWarehouseNo";
        public const string HeaderIdBegin = "HeaderIdBegin";            //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string HeaderIdLength = "HeaderIdLength";          //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string HeaderId = "HeaderId";                      //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string HeadLine = "headline";
        public const string HiAllocated = "hiallocated";
        public const string HiAllowed = "hiallowed";
        public const string HiBuffNo = "hibuffno";
        public const string HiddenStatus = "HiddenStatus"; //Only used in SR as a hidden field
        public const string HiRefNo = "hirefno";
        public const string HissN = "hissn";
        public const string HiStatus = "HighstStatus";
        public const string History = "History";      //UAT 367
        public const string HitRate = "HitRate";
        public const string HoldProp = "holdprop";
        public const string HomeDialCode = "HomeDialCode"; //NM & IP - 29/12/08 - CR976
        public const string HourlyRate = "HourlyRate";
        public const string Hours = "Hours";
        public const string HoursFrom = "HoursFrom";
        public const string HoursTo = "HoursTo";
        public const string HPValue = "hpvalue";
        public const string ID = "id";
        public const string IDType = "IDType";
        public const string IncludeInCashierTotals = "includeincashiertotals";
        public const string IncludeINGFT = "includeingft";
        public const string IncludeWarranty = "includewarranty";
        public const string Income = "income";
        public const string IncWarrantyText = "IncWarrantyText";
        //CR 866 Added Industry
        public const string Industry = "Industry";
        public const string InsIncluded = "insincluded";
        public const string InsIncludedText = "insincludedtext";
        public const string InsPcent = "inspcent";
        public const string InstalAmount = "Instal Amount";
        public const string Installation = "Installation";              //IP - 23/02/11 - Sprint 5.10 - #3133
        public const string InstallationNo = "InstallationNo";
        public const string InstallmentAmount = "InstallmentAmount";
        public const string InstallmentDueDate = "InstallmentDueDate";
        public const string InstallmentPaidAmount = "InstallmentPaidAmount";
        public const string Instalment = "Instalamount";
        public const string Instalment2 = "instalment";
        public const string InstalmentNumber = "instalmentnumber";
        public const string InstalOrder = "instalorder";
        public const string InstalPreDel = "instalpredel";
        public const string InstantReplace = "InstantReplace";
        public const string InstantReplaceOrig = "InstantReplaceOrig";
        public const string Instructions = "Instructions";
        public const string IntChargesApplied = "intchargesapplied";
        public const string IntChargesCumul = "intchargescumul";
        public const string IntChargesDue = "intchargesdue";
        public const string Interest = "Interest";
        public const string Interface = "interface";
        public const string InterfaceAccount = "interfaceaccount";
        public const string InterfaceBalancing = "interfacebalancing";
        public const string InterfaceSecAccount = "interfacesecaccount";
        public const string InterfaceSecBalancing = "interfacesecbalancing";
        public const string Internal = "Internal";
        public const string IntRate = "intrate";
        public const string IntRateFrom = "intratefrom";
        public const string IntRateTo = "intrateto";
        public const string InvoiceNo = "InvoiceNo";
        public const string IsActive = "isactive";
        public const string IsAM = "IsAM";
        public const string IsBatch = "IsBatch";            //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string IsCashierFloat = "IsCashierFloat";
        public const string IsDelimited = "IsDelimited";            //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string IsDeposit = "isdeposit";
        public const string IsInterest = "IsInterest";              //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
        public const string IsLoan = "isLoan";
        public const string IsMandate = "IsMandate";
        public const string IsReversed = "IsReversed";
        public const string IsWorklist = "IsWorklist"; //NM & IP - 06/01/09 - CR976
        public const string IsBailiff = "isBailiff";
        public const string IsTelephoneCaller = "isTelephoneCaller";
        public const string Item1 = "Item1";  //CR36
        public const string Item2 = "Item2";  //CR36
        public const string Item3 = "Item3";  //CR36
        public const string Item4 = "Item4";  //CR36
        public const string Item5 = "Item5";  //CR36    
        public const string ItemDescr1 = "itemdescr1";
        public const string ItemDescr2 = "itemdescr2";
        public const string ItemDescription = "ItemDescription";    //CR802
        public const string ItemId = "ItemId";    //CR1212 - RI
        public const string ItemID = "ItemID";    //CR1212 - RI
        public const string ItemIUPC = "itemiupc"; //CR1212 - RI
        public const string ItemNo = "ItemNo";
        public const string ItemNotes = "itemnotes";
        public const string ItemPrice = "itemprice";
        public const string ItemSuppText = "ItemSuppText";
        public const string ItemText = "Itemtext";  //CR36
        public const string ItemType = "ItemType";
        public const string ItemValue = "ItemValue";                //CR802
        public const string IsNonCourts = "IsNonCourts";            //#12116
        public const string IsSpouseWorking = "IsSpouseWorking";
        public const string JobTitle = "JobTitle";
        public const string Key = "Key";
        public const string KitNo = "kitno";
        public const string LabourCost = "LabourCost";
        public const string LabourLimit = "LabourLimit";
        public const string LabourPercent = "LabourPercent";
        public const string LargestAgreement = "largestagreement";
        public const string LaserPrintTax = "LaserPrintTax";
        public const string LastName = "LastName";
        public const string LastName2 = "Last Name";
        public const string LastTransDate = "LastTransDate";            //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
        public const string Lawsuits = "lawsuits";
        public const string Lawsuits12Months = "lawsuits12months";
        public const string Lawsuits24Months = "lawsuits24months";
        public const string LawsuitsAvgValue = "lawsuitsavgvalue";
        public const string LawsuitsTotalValue = "lawsuitstotalvalue";
        public const string LawsuitTimeSinceLast = "lawsuittimesincelast";
        public const string LbfOrdval = "lbfordval";
        public const string LbfPrice = "lbfprice";
        public const string LbfQuantity = "lbfquantity";
        public const string LbrCostEstimate = "LbrCostEstimate"; // CR 1024 (NM 29/04/2009)
        public const string Length = "length";
        public const string LetterCode = "lettercode";
        public const string Limit = "Limit";
        public const string LimitType = "LimitType";
        public const string LinkedWarranty = "LinkedWarranty"; //#17290
        public const string LineItemId = "lineItemId";    //IP - 12/06/12 - #10357 - Warehouse & Deliveries
        public const string LineWareHouseNo = "LineWareHouseNo";
        public const string LinkedSR = "LinkedSR";  //UAT 367
        public const string Loaded = "Loaded";
        public const string LoadNo = "LoadNo";
        public const string LocalChange = "localchange";
        public const string LocalTender = "LocalTender";
        public const string Locked = "Locked";      //CR802
        public const string LockedBy = "LockedBy";
        public const string LoggedBy = "loggedby";
        public const string LoggedIn = "loggedin";
        public const string LongestAgreement = "longestagreement";
        public const string LostCommission = "Lost Commission";
        public const string LostSalesValue = "Lost Sales Value";        //UAT219 jec
        public const string LstCommn = "lstcommn";
        public const string MachineLoggedOn = "MachineLoggedOn";
        public const string MaidenName = "maidenname";
        public const string MandateId = "MandateId";
        public const string MandWarrantyNo = "mandwarrantyno";
        public const string Manual = "Manual";      //UAT856 jec 18/09/09
        public const string ManualName = "ManualName";
        public const string ManuFacturerMonths = "ManuFacturerMonths";        //CR1094 jec
        public const string ManualRefer = "ManualRefer";
        public const string MaritalStatus = "MaritalStatus";
        public const string MaxBal = "MaxBal";                //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MaxMnthsArrears = "MaxMnthsArrears"; //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MaxTerm = "MaxTerm";
        public const string MaxValue = "maxvalue";
        public const string MaxValColl = "MaxValColl";         //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MaxNoWarrantableSales = "Max No of Warrantable Sales";
        public const string MaxWarrantySalesValue = "Max Warranty Sales Value";
        public const string MaxValueWarrantableSales = "Max Value of Warrantable Sales";
        public const string ActualWarrantySalesValue = "Actual Warranty Sales Value";
        public const string MaxAccounts = "maxAccounts";
        public const string MaxJobs = "maxJobs"; // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.																																							
        public const string MinAccounts = "minAccounts";
        public const string MinBal = "MinBal";                //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MinDelDate = "MinDeliveryDate";
        public const string MinimumIncome = "MinimumIncome";
        public const string MaximumIncome = "MaximumIncome";
        public const string MinMnthsArrears = "MinMnthsArrears"; //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MinNotesLength = "MinNotesLength"; //NM & IP - 29/12/08 - CR976
        public const string MinPeriod = "MinPeriod";
        public const string MinReferences = "minreferences";
        public const string MinTerm = "MinTerm";
        public const string MinValue = "minvalue";
        public const string MinValColl = "MinValColl";         //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string MobileDialCode = "MobileDialCode"; //NM & IP - 29/12/08 - CR976
        public const string MinApplicationScore = "MinApplicationScore"; //IP - 9/12/10 - Store Card
        public const string MinBehaviouralScore = "MinBehaviouralScore"; //IP - 9/12/10 - Store Card
        public const string MinMthsAcctHistX = "MinMthsAcctHistX"; //IP - 9/12/10 - Store Card
        public const string MinMthsAcctHistY = "MinMthsAcctHistY"; //IP - 26/04/11 - Store Card - Feature 3000
        public const string MaxCurrMthsInArrs = "MaxCurrMthsInArrs"; //IP - 9/12/10 - Store Card
        public const string MaxNoCustForApproval = "MaxNoCustForApproval"; //IP - 10/05/11 - Store Card
        public const string MaxPrevMthsInArrsX = "MaxPrevMthsInArrsX"; //IP - 9/12/10 - Store Card
        public const string MaxPrevMthsInArrsY = "MaxPrevMthsInArrsY"; //IP - 26/04/11 - Store Card - Feature 3000
        public const string PcentInitRFLimit = "PcentInitRFLimit"; //IP - 9/12/10 - Store Card //IP - 26/04/11 - Renamed


        public const string MobileNo = "mobileno";
        public const string ModelNo = "ModelNo";
        public const string MoneyBegin = "moneybegin";
        public const string MoneyLength = "moneylength";
        public const string MoneyPoint = "moneypoint";
        public const string Month = "month";
        public const string MonthlyIncome = "MonthlyIncome";
        public const string MonthlyInstal = "monthlyinstal";
        public const string MonthlyRent = "MonthlyRent";
        public const string MonthName = "MonthName";
        public const string MonthsInArrears = "MonthsInArrears";
        public const string MonthsIntFree = "mthsintfree";
        public const string MoreRewardsNo = "MoreRewardsNo";
        public const string Mpr = "mpr";
        public const string MultipleQuantity = "MultipleQuantity";
        public const string name = "name";
        public const string Name = "Name";  //CR633 jec 20/06/06
        public const string Nationality = "Nationality";
        public const string NearestDueDate = "NearestDueDate";
        public const string NegativeRef = "NegativeRef";
        public const string NetPayment = "NetPayment";
        public const string NewAgreementTotal = "newagreementtotal";
        public const string NewBuffNo = "newbuffno";
        public const string NewCODflag = "NewCODflag";
        public const string NewComment = "NewComment";
        public const string NewCustId = "NewCustId";
        public const string NewDateFirst = "newdatefirst";
        public const string NewDeposit = "NewDeposit";
        public const string NewInstalment = "newinstalment";
        public const string NewInstalNo = "newinstalno";
        public const string NewItem = "NewItem";                //IP - 15/02/11 - Sprint 5.10 - #3151
        public const string NewName = "NewName";
        public const string NewWarrantyID = "newwarrantyid";    //IP - 16/06/11 - CR1212 - RI - #3941
        public const string NewPCType = "NewPCType";
        public const string NewS1Comment = "NewS1Comment";
        public const string NewServiceCharge = "newservicecharge";
        public const string NextStep = "NextStep";
        public const string NextStepFalse = "NextStepFalse";
        public const string NextStepTrue = "NextStepTrue";
        public const string NoArrearsLetters = "noarrearsletters";
        public const string NoOfCalls = "NoOfCalls";              //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string NoOfDaysSinceAction = "NoOfDaysSinceAction";                //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string NoOfWarrantySales = "No Of Warranty Sales";
        public const string NoFees = "NoFees";
        public const string NoCancelled = "No Of Warranties Cancelled";
        public const string NoRepossessed = "No Of Warranties Repossessed";
        public const string NonCourts = "NonCourts";
        public const string NoOfRemainInstals = "NoOfRemainInstals"; //IP & JC - CR976
        public const string NoOfIns = "NoOfIns"; //IP & JC - 14/01/09 - CR976
        public const string NonInterestItem = "noninterestitem";
        public const string NoOfOpenSRs = "noOfOpenSRs";  // UAT 380
        public const string NoOfRef = "NoOfRef";
        public const string NoOfPrompts = "NoOfPrompts";
        public const string NoRFDetails = "NoRFDetails";
        public const string NoRFSummary = "NoRFSummary";
        public const string Notes = "Notes";
        public const string NotifiedBy = "NotifiedBy";
        public const string NotLike = "notlike"; // CR 976
        public const string NumCashSettled = "nocashsettled";
        public const string NumCurrent = "nocurrent";
        public const string NumInArrears = "noinarrears";
        public const string NumOfDependents = "NumOfDependents";
        public const string NumReturnedCheques = "noreturnedcheques";
        public const string NumRFAccounts = "norfaccounts";
        public const string NumSettled = "nosettled";
        public const string Occupation = "Occupation";
        public const string OldAgreementTotal = "oldagreementtotal";
        public const string OldCategory = "oldcategory";
        public const string OldCODflag = "OldCODflag";
        public const string OldCode = "oldcode";
        public const string OldCustId = "OldCustId";
        public const string OldDeposit = "olddeposit";
        public const string OldInstalment = "oldinstalment";
        public const string OldInstalNo = "oldinstalno";
        public const string OldName = "OldName";
        public const string OldPCType = "oldpctype";
        public const string OldServiceCharge = "oldservicecharge";
        public const string OperandAllowable = "OperandAllowable";
        public const string Operand = "Operand";
        public const string OperandName = "OperandName";
        public const string OperandOptions = "OperandOptions";
        public const string OperandType = "OperandType";
        public const string Operator = "Operator";
        public const string Operator1 = "Operator1";
        public const string Operator2 = "Operator2";
        public const string OptionCategory = "OptionCategory";
        public const string OptionCode = "OptionCode";
        public const string OptionListName = "OptionListName";
        public const string Or = "Or";
        public const string Or_Clause = "or_clause"; // CR 976
        public const string OrClause = "OrClause";
        public const string OrdVal = "OrdVal";
        public const string OrderedQuantity = "OrderedQuantity";    // #10221
        //CR 866 Added organisation
        public const string Organisation = "Organisation";
        public const string OrigBR = "origbr";
        public const string OrigBuffno = "OrigBuffno"; //IP - 20/04/10 - UAT(107) UAT5.2
        public const string OrigItemItemID = "OrigItemItemID";  //IP - 20/06/11 - CR1212 - RI - #4042
        public const string OriginalItem = "originalitem";
        public const string OriginalLocation = "originallocation";
        public const string OrigMonth = "OrigMonth";
        public const string OtherPayments = "OtherPayments";
        public const string OutstandingBalance = "Outstanding Bal";
        public const string OutstandingBalance2 = "Outstanding Balance";
        public const string OutstBal = "OutstBal";
        public const string Overage = "Overage";
        public const string Override = "Override";
        public const string P1 = "P1";
        public const string P10 = "P10";
        public const string P11 = "P11";
        public const string P12 = "P12";
        public const string P2 = "P2";
        public const string P3 = "P3";
        public const string P4 = "P4";
        public const string P5 = "P5";
        public const string P6 = "P6";
        public const string P7 = "P7";
        public const string P8 = "P8";
        public const string P9 = "P9";
        public const string PAddress1 = "PAddress1";
        public const string PAddress2 = "PAddress2";
        public const string PAddress3 = "PAddress3";
        public const string PaidAndTaken = "PaidAndTaken";
        public const string PaidPcent = "paidpcent";
        public const string ParameterCategory = "ParameterCategory";
        public const string ParameterID = "ParameterID";
        public const string ParentID = "ParentID";              //IP - 15/06/10 - CR1083 - Collection Commissions
        public const string ParentItemId = "ParentItemId";  //CR1212 - RI
        public const string ParentItemNo = "parentitemno";
        public const string ParentLocation = "parentlocation";
        public const string PartDescription = "PartDescription";
        public const string PartID = "PartID";                  //IP - 01/07/11 - CR1254 - RI - #3994
        public const string PartNo = "PartNo";
        public const string PartsDate = "PartsDate";
        public const string PartLimit = "PartLimit";
        public const string PartPercent = "PartPercent";
        public const string PartType = "PartType";
        public const string Password = "password";
        public const string PayCode = "PayCode";
        public const string PayFrequency = "PayFrequency";
        public const string Payment = "Payment";
        public const string PaymentAmt = "PaymentAmt";          //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
        public const string PaymentCardLine = "paymentcardline";
        public const string PaymentHoliday = "paymentholidays";
        public const string PaymentHolidayArrears = "paymentholidayarrears";
        public const string PaymentHolidayMin = "paymentholidaymin";
        public const string PaymentId = "PaymentId";
        public const string PaymentMethod = "Payment Method";
        public const string Paymentmethod = "paymentmethod";	//jec used in PaymentFileDefn
        public const string PaymentType = "PaymentType";
        public const string PayMethod = "PayMethod";
        public const string PayMethodDescription = "PayMethodDescription";
        public const string PayMethodText = "PayMethodText";
        public const string PayWholeUnits = "PayWholeUnits";
        public const string PcentArrearsColl = "PcentArrearsColl";            //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string PcentCommOnArrears = "PcentCommOnArrears";        //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string PcentCommOnAmtPaid = "PcentCommOnAmtPaid";              //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string PcentCommOnFee = "PcentCommOnFee";                //IP - 30/06/10 - CR1083 - Collection Commissions
        public const string PcentOfCalls = "PcentOfCalls";                    //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string PcentOfWorklist = "PcentOfWorklist";              //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string PCity = "PCity";
        public const string PClubTier1 = "PClubTier1";
        public const string PClubTier2 = "PClubTier2";
        public const string PDeliveryArea = "pdeliveryarea";
        public const string Percentage = "Percentage";  //CR36
        public const string PercentageCash = "PercentageCash";  //CR36
        public const string PercentagePaid = "paidpcent";
        public const string PercentToPay = "PercentToPay";
        public const string Period = "Period"; //IP & JC - CR976
        public const string PeriodEnd = "PeriodEnd";
        public const string PersDialCode = "PersDialCode";
        public const string PersTel = "PersTel";
        public const string Permissionid = "Permissionid";
        public const string Permissionid1 = "Permissionid1";
        public const string Phone = "Phone";
        public const string PhoneNo = "phoneno";
        public const string Picked = "picked";
        public const string PickListBranch = "picklistbranch";
        public const string PicklistNumber = "Picklistnumber";
        public const string Picture = "picture";
        public const string PointsFrom = "PointsFrom";
        public const string PointsTo = "PointsTo";
        public const string PolicyRule1 = "PolicyRule1";
        public const string PolicyRule2 = "PolicyRule2";
        public const string PolicyRule3 = "PolicyRule3";
        public const string PolicyRule4 = "PolicyRule4";
        public const string PolicyRule5 = "PolicyRule5";
        public const string PolicyRule6 = "PolicyRule6";
        public const string Position = "Position";
        public const string PostCode = "PostCode";
        public const string PPostCode = "PPostCode";
        public const string Precision = "precision";
        public const string PrevAddMM = "PrevAddMM";
        public const string PrevAddYY = "PrevAddYY";
        public const string Premium = "Premium";
        public const string PrevDateEmployed = "PrevDateEmployed";
        public const string PrevDateIn = "PrevDateIn";
        public const string PrevDateOut = "PrevDateOut";
        public const string PrevEmpMM = "PrevEmpMM";
        public const string PrevEmpYY = "PrevEmpYY";
        public const string PreviousCosts = "PreviousCosts";
        public const string PreviousCostsEW = "PreviousCostsEW";                                            //IP - 16/09/11 - #8153 - UAT29
        public const string PreviousEnquiries = "previousenquiries";
        public const string PreviousEnquiries12Months = "previousenquiries12months";
        public const string PreviousEnquiriesAvgValue = "previousenquiriesavgvalue";
        public const string PreviousEnquiriesAvgValue12Months = "previousenquiriesavgvalue12months";
        public const string PreviousEnquiriesTotalValue = "previousenquiriestotalvalue";
        public const string PreviouslyDeposited = "previouslydeposited";
        public const string PrevPropertyType = "PrevPropertyType";
        public const string PrevResidentialStatus = "PrevResidentialStatus";
        public const string Price = "price";
        public const string PrintCreditNote = "PrintCreditNote";
        public const string Printed = "Printed";
        public const string PrintedBy = "printedby";
        public const string PrintLocn = "PrintLocn";   //CR 949/958
        public const string PrintScheduleOfPayments = "printscheduleofpayments";
        public const string PrintToolBar = "PrintToolBar";
        public const string PrintWarrantyContract = "PrintWarrantyContract";
        public const string PrivilegeClub = "PrivilegeClub";
        public const string PrivilegeClubDesc = "PrivilegeClubDesc";
        public const string ProdCat = "ProdCat";
        public const string Product_Category = "Product Category";                 //IP - 25/07/11 - CR1254 - RI - #4036
        public const string ProdCode = "ProdCode";
        public const string ProdDescription = "ProdDescription";
        public const string ProdDescription2 = "ProdDescription2"; //SC 70524 - Include prod desc
        public const string ProdStatus = "ProdStatus";       //CR1094 jec 09/12/10
        public const string Product = "Product";
        public const string ProductCode = "ProductCode";
        public const string Product_Code = "Product Code";
        public const string ProductGroup = "ProductGroup";      //CR1212 jec
        public const string ProductQuantity = "Product Quantity";
        public const string ProductDeliveryDate = "ProductDeliveryDate";
        public const string ProductTotal = "ProductTotal";
        public const string ProductType = "ProductType";        //CR1094 jec
        public const string ProductCommissionTotal = "ProductCommissionTotal";      //CR1035
        public const string PromotionId = "PromotionId";      //Merchandising
        public const string ProofOfAddress = "ProofOfAddress";
        public const string ProofOfBank = "ProofOfBank";                            //IP - 14/12/10 - Store Card
        public const string ProofOfBankTxt = "ProofOfBankTxt";                      //IP - 14/12/10 - Store Card
        public const string ProfitMargin = "Profit Margin";
        public const string ProofOfID = "ProofOfID";
        public const string ProofOfIncome = "ProofOfIncome";
        public const string PropertyType = "PropertyType";
        public const string PropNotes = "propnotes";
        public const string PropResult = "propresult";
        public const string ProvisionAmount = "provisionamount";
        public const string PurchaseDate = "PurchaseDate";
        public const string PurchaseOrderNumber = "purchaseordernumber";
        public const string QtyAvailable = "QtyAvailable";                      //IP - 07/07/11 - RI - #4037
        public const string Qtydiff = "Qtydiff";
        public const string QtyBooked = "QtyBooked";                              // #10230
        public const string QualifyingCode = "QualifyingCode";
        public const string Quantity = "Quantity";
        public const string QuantityAfter = "quantityafter";
        public const string QuantityBefore = "quantitybefore";
        public const string QuantityDelivered = "QuantityDelivered";
        public const string QuantityOrdered = "QuantityOrdered";
        public const string Query = "Query"; // CR 976
        public const string Question = "Question";
        public const string Rate = "rate";
        public const string RateType = "RateType";
        public const string RateTypeDesc = "ratetypedesc";
        public const string Ratio = "Ratio";
        public const string RatioFee = "RatioFee";
        public const string RatioPay = "RatioPay";
        public const string ReadOnly = "ReadOnly";
        public const string Reason = "reason";
        public const string Reason2 = "reason2";
        public const string Reason3 = "reason3";
        public const string Reason4 = "reason4";
        public const string Reason5 = "reason5";
        public const string ReAssignCode = "ReAssignCode"; //CR1030 jec
        public const string ReAssignedBy = "ReAssignedBy"; //CR1030 jec
        public const string ReasonCode = "ReasonCode"; //IP & JC - CR976
        public const string ReasonCodeDesc = "ReasonCodeDesc";
        public const string Rebate = "Rebate";
        public const string RebateAfter12Mths = "rebateafter12mths";
        public const string RebateTotal = "RebateTotal";
        public const string Rebatepcent = "Rebate%";
        public const string RebateWithin12Mths = "rebatewithin12mths";
        public const string ReceiptDate = "receiptdate";
        public const string ReceiptNo = "receiptno";
        public const string ReceiveDate = "ReceiveDate";
        public const string ReceivedDate = "ReceivedDate";
        public const string RecordLine = "RecordLine";
        public const string RefAcctNo = "RefAcctNo";
        public const string RefAcctOpen = "RefAcctOpen";
        public const string RefAddress1 = "RefAddress1";
        public const string RefAddress2 = "RefAddress2";
        public const string RefCity = "RefCity";
        public const string RefCode = "RefCode";
        public const string RefComment = "RefComment";
        public const string RefDialCode = "RefDialCode";
        public const string RefDirections = "RefDirections";
        public const string Reference = "reference";
        public const string ReferenceLength = "ReferenceLength"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string ReferenceHeaderText = "ReferenceHeaderText"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string ReferenceMandatory = "referencemandatory";
        public const string ReferenceUnique = "referenceunique";
        public const string ReferralReason = "ReferralReason";      //CR1084 jec
        public const string ReferralReason2 = "ReferralReason2";      //CR1084 jec
        public const string ReferralReason3 = "ReferralReason3";      //CR1084 jec
        public const string ReferralReason4 = "ReferralReason4";      //CR1084 jec
        public const string ReferralReason5 = "ReferralReason5";      //CR1084 jec
        public const string ReferralReason6 = "ReferralReason6";      //CR1084 jec
        public const string RefFirstName = "RefFirstName";
        public const string RefinDeposit = "RefinDeposit"; //CR976
        public const string RefLastName = "RefLastName";
        public const string RefMDialCode = "RefMDialCode";
        public const string RefMPhoneNo = "RefMPhoneNo";
        public const string RefNo = "RefNo";
        public const string RefNoMandatory = "referencemandatory";
        public const string RefNoUnique = "referenceunique";
        public const string RefPhoneNo = "RefPhoneNo";
        public const string RefPostCode = "RefPostCode";
        public const string RefRelation = "RefRelation";
        public const string RefRelationText = "RefRelationText";
        public const string Refund = "Refund";
        public const string RefundType = "RefundType";
        public const string RefundPercentage = "RefundPercentage";        //CR1094 jec
        public const string RefWAddress1 = "RefWAddress1";
        public const string RefWAddress2 = "RefWAddress2";
        public const string RefWCity = "RefWCity";
        public const string RefWDialCode = "RefWDialCode";
        public const string RefWPhoneNo = "RefWPhoneNo";
        public const string RefWPostCode = "RefWPostCode";
        public const string Region = "region";
        public const string RejectAction = "RejectAction";
        public const string RejectActionStr = "RejectActionStr";
        public const string RejectCode = "RejectCode";
        public const string RejectCount = "RejectCount";
        public const string Released = "released";
        public const string RemainInstalAmt = "RemainInstalAmt"; //IP & JC - CR976
        public const string ReminderDateTime = "ReminderDateTime"; //NM & IP - CR976
        public const string RemovedBy = "RemovedBy"; //IP - 18/11/09 - CR929 & 974 - Audit
        public const string RenewalContractNo = "renewalcontractno";
        public const string RenewalWarrantyNo = "renewalwarrantyno";
        public const string RepairDate = "RepairDate";
        public const string RepairEstimate = "RepairEstimate";
        public const string RepairedHome = "RepairedHome";         //CR 949/958
        public const string RepaymentPcent = "repaymentpcent";
        public const string RepaymentPcentCurrent = "repaymentpcentcurrent";
        public const string Replacement = "Replacement";
        public const string ReplacementStatus = "ReplacementStatus";
        public const string ReplacementMarker = "ReplacementMarker";
        public const string RepoPercentage = "RepoPercentage";  // RI
        public const string RepoPercentageCash = "RepoPercentageCash";  // RI
        public const string RepossessionTotal = "RepossessionTotal";
        public const string RepossessionDate = "Repossession Date";
        public const string ReposDate = "ReposDate";         //IP - 30/09/10 - CR1107 - Writeoff Review Enhancements
        public const string RepossArrears = "repossarrears";
        public const string RepossPercent = "reposspercent";
        public const string RepoItem = "RepoItem";      // jec 
        public const string RepoValue = "repovalue";
        public const string RepPercent = "reppercent";
        public const string RequiredSalesValue = "Required Sales Value";        //UAT219 jec
        public const string ResidentialStatus = "ResidentialStatus";
        public const string Resolution = "Resolution";
        public const string ResolutionChangedBy = "ResolutionChangedBy";
        public const string ResolutionDate = "ResolutionDate";
        public const string ResolutionDescription = "ResolutionDescription";
        public const string ResponseXML = "responsexml";
        //CR 843 Added Responsexml2
        public const string ResponseXML2 = "responsexml2";
        public const string Result = "result";
        public const string Retailer = "Retailer";      //CR1030 jec
        public const string RetItemNo = "RetItemNo";
        public const string RetItemId = "RetItemId";    //IP - 17/05/11 - CR1212 - CR1212 - #3627
        public const string RetStockLocn = "RetStockLocn";
        public const string ReturnCode = "ReturnCode";
        public const string ReturnedChequeAuthorisedBy = "returnedChequeAuthorisedBy";
        public const string ReturnDate = "ReturnDate";
        public const string ReturnLetter = "ReturnLetter";
        public const string ReturnWarehouse = "retwarehouse";
        public const string RetVal = "RetVal";
        public const string Reversible = "reversible";
        public const string ReviewDate = "ReviewDate"; //IP & JC - CR976
        public const string Revised = "Revised";
        public const string RFAvailable = "RFAvailable";
        public const string RFBlock = "RFBlock";
        public const string RFCardSeqNo = "RFCardSeqNo";
        public const string RFCategory = "RFCategory";
        public const string RFCreditLimit = "RFCreditLimit";
        public const string RiskCat = "RiskCat";
        public const string RiskCategory = "riskcategory";
        public const string RuleName = "RuleName";     //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string RulesXML = "RulesXML";
        public const string RunCloseAC = "runcloseac";
        public const string RunCloseBalance = "runclosebalance";
        public const string RunDate = "rundate";
        public const string RunEnd = "runend";
        public const string RunMovement = "runmovement";
        public const string Runno = "runno";
        public const string ReRunNo = "ReRunNo";        //jec 05/04/11
        public const string RunOpenAC = "runopenac";
        public const string RunOpenBalance = "runopenbalance";
        public const string RunStart = "runstart";
        public const string S1Comment = "S1Comment";
        public const string S2Comment = "S2Comment";
        public const string SafeValue = "safevalue";
        public const string SalesPerson = "salesperson";
        public const string SalesPersonName = "SalesPersonName";                                //IP - 17/05/12 - #9447 - CR1239
        public const string SalesTax = "salestax";
        public const string SanctionMinYears = "SanctionMinYears";
        public const string SanctionStage = "SanctionStage";
        public const string SavedType = "SavedType";
        public const string SCInterfaceAccount = "SCInterfaceAccount";                          //IP - 11/04/12 - CR9863 - #9885
        public const string SCInterfaceBalancing = "SCInterfaceBalancing";                      //IP - 11/04/12 - CR9863 - #9885
        public const string ScheduledDate = "ScheduledDate";  // 68440 RD 24/08/06
        public const string ScheduledQty = "scheduledqty";
        public const string Score = "score";
        public const string ScoreDate = "scoredate";
        public const string ScoreHPbefore = "ScoreHPbefore"; //CR903
        public const string ScoringBand = "ScoringBand";
        public const string ScoringCard = "ScoringCard"; //CR1034
        public const string Screen = "screen";
        public const string ScriptName = "scriptname";
        public const string SecureRefunds = "SecureRefunds";
        public const string Securitisation = "securitisation";
        public const string Securitised = "securitised";
        public const string SecuritisedValue = "securitisedtotal";
        public const string SegmentDate = "Date";
        public const string SegmentID = "Segment_ID";
        public const string SegmentName = "Segment_Name";
        public const string SegmentName2 = "SegmentName";                //IP - 09/09/10 - CR1107 - Writeoff Review screen Enhancements
        public const string SegmentUser = "User_id";
        public const string Selected = "Selected";
        public const string SeparateIns = "separateins";
        public const string Sequence = "sequence";
        public const string SerialNo = "SerialNo";
        public const string ServiceBranchNo = "ServiceBranchNo";
        public const string ServiceCharge = "servicechg";
        public const string ServiceChargeAcctNo = "ChargeAcctNo";       //IP - 26/03/12 - #8842 - LW73943 - Merged from current
        public const string ServiceChargePC = "ServiceCharge";
        public const string ServiceEvaluation = "ServiceEvaln";
        public const string ServiceLocn = "ServiceLocn";
        public const string ServicePrintDP = "serviceprintdp";
        public const string ServiceRepairCentre = "ServiceRepairCentre";
        public const string ServiceRequestNo = "ServiceRequestNo";
        public const string ServiceRequestNoStr = "ServiceRequestNoStr";
        public const string ServiceType = "ServiceType";
        public const string SetDescript = "setdescript";
        public const string SetName = "setname";
        public const string SetNumber = "SetNumber";
        public const string Settled = "Settled";
        public const string SettledAccounts = "SettledAccounts";
        public const string SettlementFigure = "SettlementFigure";
        public const string SetWorst = "SetWorst";
        public const string Severity = "severity";
        public const string Sex = "Sex";
        public const string Shortage = "Shortage";
        public const string Slot = "Slot";
        public const string SlotDate = "SlotDate";
        public const string SlotNo = "SlotNo";
        public const string SMSName = "SMSName";
        public const string SMSText = "SMSText";
        public const string SOA = "SOA";
        public const string SoftScript = "SoftScript";
        public const string SoftScriptDate = "SoftScriptDate";
        public const string SortOrder = "sortorder";
        public const string SortOrderLength = "SortOrderLength"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string SortOrderHeaderText = "SortOrderHeaderText"; //IP - 30/10/09 - CoSACS Improvement - Code Maintenance
        public const string Source = "source";
        public const string SPIFFTotal = "SPIFFTotal";
        public const string SpaInstal = "SpaInstal";
        public const string SpecialPromo = "SpecialPromo";
        public const string SpecialValue = "specialvalue";
        public const string StaffNo = "StaffNo";
        public const string StaffType = "StaffType"; //IP - 03/02/10 - CR1072 - 3.1.8
        public const string Stage1Complete = "Stage1Complete";
        public const string StartDate = "StartDate";
        public const string Status = "Status";
        public const string StatusCode = "StatusCode";
        public const string Status1 = "Status1"; //IP - 19/05/08
        public const string Status2 = "Status2"; //IP - 19/05/08
        public const string Status3 = "Status3"; //IP - 19/05/08
        public const string StatusDescription = "StatusDescription";
        public const string STCAmount = "stcamount";
        public const string STCPc = "stcpc";
        public const string Step = "Step";
        public const string StepActionType = "StepActionType";
        public const string StockAvailable = "stockavailable";
        public const string StockItemNo = "stockitemno";
        public const string StockLocn = "StockLocn";
        public const string StockStatus = "stockstatus";
        public const string StoreType = "StoreType"; //CR903     jec
        public const string StoreCard = "Storecard";    //IP - 20/02/12 - #9423 - CR8262
        public const string StoreCardAcctNo = "StoreCardAcctNo";    //IP - 06/05/11 - Store Card - Feature - #3004
        public const string StoreCardNo = "storeCardNo";
        public const string ScoreCardType = "ScoreCardType";
        public const string StoreCardLimit = "StoreCardLimit";              //IP - 11/01/11 - Store Card
        public const string StoreCardAvailable = "StoreCardAvailable";      //IP - 11/01/11 - Store Card
        public const string StoreCardValue = "storecardvalue";              //IP - 17/02/12 - #9423 - CR8262
        public const string Strategy = "Strategy";
        public const string Style = "Style";                                //IP - 20/09/11 - RI - #8220 - CR8201                        
        public const string SubmitDate = "SubmitDate";
        public const string SubClass = "SubClass";        //CR1212 jec
        public const string SundryCredit = "SundryCredit";
        public const string SuperShield = "supershield";
        public const string Supplier = "Supplier";
        public const string SupplierCode = "SupplierCode"; //CR 1024 (NM 23/04/2009)	
        public const string SupplierNo = "supplierno";
        public const string SysRecommend = "SysRecommend";
        public const string SystemTotal = "systemtotal";
        public const string SystemValue = "SystemValue";
        public const string TallymanAcct = "TallymanAcct";  //CR1072 UAT12 jec 16/03/10
        public const string TaxAmt = "TaxAmt";
        public const string TaxAmtAfter = "taxamtafter";  // 67977 RD 22/02/06
        public const string TaxAmtBefore = "taxamtbefore"; // 67977 RD 22/02/06
        public const string TaxExempt = "TaxExempt";
        public const string TaxRate = "TaxRate";
        public const string TaxType = "TaxType";
        public const string TCCode = "TCCode";
        public const string TechnicianId = "TechnicianId";
        public const string TechnicianName = "TechnicianName";
        public const string TechnicianReport = "TechnicianReport"; // CR 1024 (NM 29/04/2009)
        public const string TechComments = "Comments"; //CR802 JH 04/01/2007
        public const string TelHome = "TelHome";
        public const string TelLocation = "TelLocn";
        public const string TelMobile = "TelMobile";
        public const string TenderAmt = "TenderAmt";
        public const string CashAmt = "CashAmt";
        public const string ChequqeAmt = "ChequqeAmt";
        public const string UsedCashAmt = "UsedCashAmt";
        public const string UsedChequqeAmt = "UsedChequqeAmt";
        public const string Homephone = "Homephone"; //IP - 22/07/08 - CR951
        public const string Cellphone = "Cellphone"; //IP - 22/07/08 - CR951
        public const string TelNo = "telno";
        public const string TelNoExt = "ext";
        public const string TelNoHome = "hometel";
        public const string TelNoWork = "worktel";
        public const string TelWork = "TelWork";
        public const string TermsType = "termstype";
        public const string TermsTypeCode = "termstypecode";
        public const string Threshold = "Threshold";
        public const string TillID = "tillid";
        public const string TimeFrameDays = "TimeFrameDays";                //IP - 25/06/10 - CR1083 - Collection Commissions
        public const string TimeOpen = "timeopen";
        public const string TimeReqDel = "TimeReqDel";
        public const string Title = "Title";
        public const string TMPercentage = "Reference";
        public const string TMChargeFee = "Reference";
        public const string ToDelete = "ToDelete";
        public const string ToFollowAmount = "ToFollowAmount";
        public const string ToMonth = "tomonth";
        public const string ToPay = "topay";
        public const string Total = "Total";
        public const string ToolTipText = "ToolTipText"; //IP - 09/11/09 - CoSACS Improvement - Code Maintenance
        public const string TotalAgreements = "TotalAgreements";
        public const string TotalAllInstalments = "TotalAllInstalments";
        public const string TotalAmount = "TotalAmount";
        public const string TotalAmountDue = "Total Amount Due";
        public const string TotalArrears = "TotalArrears";
        public const string TotalBalance = "TotalBalance";
        public const string TotalBalances = "TotalBalances";
        public const string TotalCost = "TotalCost";
        public const string TotalCount = "TotalCount";
        public const string TotalCredit = "TotalCredit";
        public const string TotalCommission_Percent = "Total Commission %";        //jec 17/07/09 UAT441
        public const string TotalCurrentInstalments = "totalcurrentinstalments";
        public const string TotalDeliveredInstalments = "TotalDeliveredInstalments";
        public const string TotalDue = "TotalDue";
        public const string TotalItems = "totalitems";
        public const string TotalSales = "Total Sales";
        public const string TrailerBegin = "trailerbegin";
        public const string TrailerLength = "trailerlength";
        public const string TrailerIdBegin = "TrailerIdBegin";                      //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string TrailerIdLength = "TrailerIdLength";                    //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string TrailerId = "TrailerId";                                //IP - 13/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string TransactionDate = "TransactionDate";
        public const string TransactionDate2 = "Trans Date";                        //IP - 20/02/12 - #9423 - CR8262       
        public const string TranSchedNo = "transchedno";
        public const string TranSchedNoBranch = "transchednobranch";
        public const string TransferReference = "transferref";                      //IP - 16/02/12 - #9632 - CR1234
        public const string TransferredToSafe = "transferredtosafe";
        public const string TransitNotes = "TransitNotes";
        public const string Translation = "Translation";
        public const string TransportCost = "TransportCost";// CR 1024 (NM 29/04/2009)
        public const string TransportCostEstimate = "TransportCostEstimate"; // CR 1024 (NM 29/04/2009)
        //CR 866 Added Transport Type 
        public const string TransportType = "TransportType";
        public const string TransPrinted = "TransPrinted";
        public const string TransRefNo = "TransRefNo";
        public const string TransTypeCode = "TransTypeCode";
        public const string TransValue = "TransValue";
        public const string TruckID = "truckid";
        public const string Type = "Type";
        public const string Unaccounted = "Unaccounted";
        public const string UndeliveredFlag = "undeliveredflag";
        public const string Unicode = "Unicode";
        public const string UnitPrice = "UnitPrice";
        public const string UnitPriceHP = "UnitPriceHP";    //CR1094 jec 09/12/10
        public const string UnitPriceCash = "UnitPriceCash";    //CR1094 jec 09/12/10
        public const string UnitPriceDutyFree = "UnitPriceDutyFree";    //CR1094 jec 09/12/10
        public const string Updated = "Updated";    //UAT 367
        public const string UpliftCommissionRate = "UpliftCommissionRate";
        public const string Uplift_Commission_pcRate = "Uplift Commission %Rate";
        public const string UserTotal = "usertotal";
        public const string UserValue = "UserValue";
        public const string UWComment = "UWComment";
        public const string UWName = "UWName";
        public const string UWResult = "UWResult";
        public const string Userid = "Userid";
        public const string Userid1 = "Userid1";
        //CR 802 Add Vacation dates for Technicians
        public const string VacationStartDate = "UnavailableStartDate";
        public const string VacationEndDate = "UnavailableEndDate";
        public const string Value = "value";
        public const string ValueOfProduct = "Value of Product";
        public const string ValueAfter = "valueafter";
        public const string ValueBefore = "valuebefore";
        public const string ValueControlled = "valueControlled";
        public const string ValueRepossession = "Value Of Warranties Repossessed";
        public const string ValueCancelled = "Value of Warranties Cancelled";
        public const string ValueOfArrears = "valueofarrears";
        public const string VanNo = "VanNo";
        public const string VehicleRegistration = "vehicleregistration";
        public const string VoucherNo = "VoucherNumber";
        public const string VoucherReference = "voucherReference";
        public const string VoucherAuthorisedBy = "voucherAuthorisedBy";
        public const string AuthorisedBy = "AuthorisedBy";
        public const string WarehouseNo = "warehouseno";
        public const string WarehouseRegion = "warehouseregion";
        public const string waritemno = "waritemno";
        public const string WarItemID = "WarItemID";                //IP - 09/05/12 - #9608 - CR8520
        public const string Warrantable = "warrantable";
        public const string WarrantiesSold = "Warranties Sold";     //UAT219 jec
        public const string WarrantyCollection = "WarrantyCollection";
        public const string Warranty_Courts_Code = "Warranty Courts Code";    //IP - 08/07/11 - CR1254 - RI 
        public const string WarrantyCreditCopy = "WarrantyCreditCopy";
        public const string WarrantyCustCopy = "WarrantyCustCopy";
        public const string WarrantyCode = "Warranty Code";
        public const string WarrantyReturnCode = "Warranty Return Code";
        public const string WarrantyReturnLocation = "Warranty Return Location";
        public const string WarrantyRetailValue = "Warranty Retail Value";
        public const string WarrantyCostPrice = "Warranty Cost Price";
        public const string WarrantyCost = "Warranty Cost";
        public const string WarrantyDescr1 = "warrantydesc1";
        public const string WarrantyDescr2 = "warrantydesc2";
        public const string WarrantyFulfilled = "warrantyfulfilled";
        public const string WarrantyHOCopy = "WarrantyHOCopy";
        public const string WarrantyId = "WarrantyId";
        public const string WarrantyLength = "warrantylength";
        public const string WarrantyLocation = "warrantylocn";
        public const string WarrantyNo = "WarrantyNo";
        public const string WarrantyLocn = "WarrantyLocn";
        public const string WarrantyMonths = "WarrantyMonths";        //CR1094 jec
        public const string WarrantyPrice = "warrantyprice";
        public const string WarrantyStylesheet = "warrantystylesheet";
        public const string WarrantyTotal = "WarrantyTotal";        //CR1035
        public const string WholeFee = "WholeFee";
        public const string WorkDialCode = "WorkDialCode"; //NM & IP - 29/12/08 - CR976
        public const string WorkList = "WorkList";
        public const string WorkListAction = "WorkListAction";
        //CR 866 Added Work Type [PC]
        public const string WorkType = "WorkType";
        public const string WorstCurrent = "worstcurrent";
        public const string WorstSettled = "worstsettled";
        public const string Year = "Year";
        public const string YrsKnown = "YrsKnown";
        public const string Zone = "Zone";
        public const string BehaviouralScoring = "BehaviouralScoring";
        public const string Provision = "Provision %";
        public const string Provision_status = "Status Codes";
        public const string Provision_Months = "Months in Arrears";
        public const string DateCurrAddress = "DateCurrAddress";
        public const string CurrentResStatus = "CurrentResStatus";

        //For amortization schedule tab
        public const string PaymentNum = "PaymentNum";
        public const string PaymentDate = "PaymentDate";
        public const string BeginningBalance = "BeginningBalance";
        public const string InstalAmt = "InstalAmt";
        public const string Principal = "Principal";
        public const string AmortizedInterest = "AmortizedInterest";
        public const string EndingBalance = "EndingBalance";
        public const string LatePmtFee = "LatePmtFee";
        public const string PenaltyFee = "PenaltyFee";
        public const string AdminFees = "AdminFee";
        public const string TotalInstalment = "TotalInstalment";

        public const string PaymentAmount = "PaymentAmount";
        public const string TenderedAmount = "TenderedAmount";
        public const string TenderedAccountAmt = "TenderedAccountAmt";
        public const string ChangeAmount = "ChangeAmount";
        public const string Success = "Success";

    }
}


namespace STL.Common.Constants.TableNames
{
    //struct used for datatable name constants
    public struct TN
    {
        public const string AccountCodes = "AccountCodes";
        public const string AcctDaHistory = "AcctDaHistory"; //IP - 04/02/10 - CR1072 - 3.1.9
        public const string AccountDetails = "AccountDetails";
        public const string AccountFlags = "AccountFlags";
        public const string AccountNumbers = "AccountNumbers";
        public const string Accounts = "Accounts";
        public const string AccountTotals = "AccountTotals";
        public const string AccountType = "AccountType";
        public const string AcctNoCtrl = "AcctNoCtrl";
        public const string AcctSelectionAction = "AcctSelectionAction";
        public const string AcctSelectionAllocation = "AcctSelectionAllocation";
        public const string AcctSelectionArrears = "AcctSelectionArrears";
        public const string AcctSelectionCodes = "AcctSelectionCodes";
        public const string AcctSelectionLetter = "AcctSelectionLetter";
        public const string AcctSelectionPoints = "AcctSelectionPoints";
        public const string AcctSelectionStatus = "AcctSelectionStatus";
        public const string Actions = "Actions";
        public const string Activities = "Activities";
        public const string AddressHistory = "AddressHistory";
        public const string AddressType = "AddressType";
        public const string Agreements = "Agreements";
        public const string AgreementTypes = "AgreementTypes";
        public const string AgreementAudit = "AgreementAudit";
        public const string Allocations = "Allocations";
        public const string AllStaff = "AllStaff";
        public const string Arrangements = "Arrangements";
        public const string App1 = "App1";
        public const string App2 = "App2";
        public const string ApplicationType = "ApplicationType";
        public const string ArrearsAccounts = "ArrearsAccounts";
        public const string Available = "Available";
        public const string BailiffCommission = "BailiffCommission";
        public const string Bank = "Bank";
        public const string BankAccountType = "BankAccountType";
        public const string BankBranches = "BankBranches";
        public const string BranchNumber = "BranchNumber";
        public const string BranchDetails = "BranchDetails";
        public const string BranchDeposits = "BranchDeposits";
        public const string BasicDetails = "BasicDetails";
        public const string CashLoanDisbursementMethods = "CashLoanDisbursementMethods";
        public const string CallReminders = "CallReminders"; //NM & IP - CR976
        public const string CancelReasons = "CancelReasons";
        public const string STCancelReasons = "STCancelReasons";
        public const string CardPrintType = "CardPrintType";
        public const string CashLoanPurpose = "CashLoanPurpose";        //#19337 - CR18568
        public const string CashAndGoPayments = "CashAndGoPayments"; //IP - 14/10/09 - CR1048 - 3.1.2.5 -- //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public const string CashierBreakdown = "CashierBreakdown";
        public const string CashierDeposits = "CashierDeposits";
        public const string CashierTotals = "CashierTotals";
        public const string CashierByBranch = "CashierByBranch";
        public const string Categories = "Categories";  //CR1094 jec 09/12/10
        public const string ChargeToAnalysis = "ChargeToAnalysis";
        public const string ChosenEntryConditions = "ChosenEntryConditions";
        public const string ChosenExitConditions = "ChosenExitConditions";
        public const string ChosenSteps = "ChosenSteps";
        public const string ChosenActions = "ChosenActions";
        public const string Code = "code";
        public const string CodeCat = "codecat";
        public const string CollectionCommissionActions = "CollectionCommissionActions";          //IP - 15/06/10 - CR1083 - Collection Commissions
        public const string CollectionCommission = "CollectionCommission";          //IP - 10/06/10 - CR1083 - Collection Commissions
        public const string CollectionType = "CollectionType";
        public const string CollectionActions = "CollectionActions";                //IP - 09/06/10 - CR1083 - Collection Commissions
        public const string CommStaff = "CommStaff";        // jec 14/06/07 
        public const string SalesCommStaff = "SalesCommStaff";
        public const string ConfigurationName = "ConfigurationName";
        public const string Control = "Control";
        public const string CorrectionReasons = "CorrectionReasons";
        public const string Countries = "Countries";
        public const string CountryParameters = "CountryParameters";
        public const string CreditBureau = "CreditBureau";
        public const string CreditBureauDefaults = "CreditBureauDefaults";
        public const string CreditCard = "CreditCard";
        public const string Currency = "Currency";
        public const string Customer = "Customer";
        public const string CustomerSR = "CustomerSR";
        public const string CustomerCurrent = "CustomerCurrent";
        public const string CustomerAccounts = "CustomerAccounts";
        public const string CustomerAdditionalDetailsFinancial = "CustomerAdditionalDetailsFinancial";
        public const string CustomerAdditionalDetailsResidential = "CustomerAdditionalDetailsResidential";
        public const string CustomerAddresses = "CustomerAddresses";
        public const string CustomerAudit = "CustomerAudit";
        public const string CustomerCodes = "CustomerCodes";
        public const string CustomerIdFormats = "CustomerIdFormats";
        public const string CustomerRelationship = "CustomerRelationship";
        public const string CLPayMethods = "CLPayMethods";
        public const string Data = "Data";
        public const string DDDueDate = "DDDueDate";
        public const string DDGiroDates = "DDGiroDates";
        public const string DDMandate = "DDMandate";
        public const string DDPaymentExtra = "DDPaymentExtra";
        public const string DDRejection = "DDRejection";
        public const string Deliveries = "Deliveries";
        public const string DeliveryArea = "DeliveryArea";
        public const string DeliveryLineItems = "DeliveryLineItems";
        public const string DeliveryNotificationAudit = "DeliveryNotificationAudit"; //IP - 18/11/09 - CR929 & 974 - Audit
        public const string DelNoteRemoveReason = "DelNoteRemoveReason";
        public const string DelNoteDeleteReason = "DelNoteDeleteReason"; //IP - 18/02/09 - CR929 & 974
        public const string DepositBanks = "DepositBanks";
        public const string Deposits = "Deposits";
        public const string Discounts = "Discounts";
        public const string DiscountLinks = "DiscountLinks";
        public const string DocConfirmation = "DocConfirmation";
        public const string DueDay = "DueDay";
        //CR 866 Added EducationLevels 
        public const string EducationLevels = "EducationLevels";
        public const string EmployeeTypes = "EmployeeTypes";
        public const string Employee = "Employee";
        public const string Employment = "Employment";
        public const string EmploymentStatus = "EmploymentStatus";
        public const string EndPeriods = "EndPeriods";
        public const string EODConfigurations = "EODConfigurations";
        public const string EODOptions = "EODOptions";
        public const string EthnicGroup = "EthnicGroup";
        public const string ExchangeRates = "ExchangeRates";
        public const string FinTrans = "FinTrans";
        public const string FinancialExportTotals = "FinancialExportTotals";
        public const string FraudDetails = "FraudDetails"; //NM & IP - 08/01/09 - CR976
        public const string Gender = "Gender";
        public const string GeneralTransactionReasons = "GeneralTransactionReasons";
        public const string GeneralTransactions = "GeneralTransactions";
        public const string GRTReason = "GRTReason";
        public const string HPPayMethods = "HPPayMethods";
        public const string IDSelection = "IDSelection";
        //CR 866b Added Industries
        public const string Industries = "Industries";
        public const string IncompleteCredits = "IncompleteCredits";
        public const string InstalPlanAudit = "InstalPlanAudit";
        public const string InstantCreditFlags = "InstantCreditFlags";
        public const string InsuranceDetails = "InsuranceDetails"; //NM & IP - 08/01/09 - CR976
        public const string InsuranceTypes = "InsuranceTypes"; //NM & IP - 08/01/09 - CR976
        public const string InstallationPrimaryCharge = "InstallationPrimaryCharge";
        public const string InstallationItemCat = "InstallationItemCat";
        public const string InterestRateTypes = "InterestRateTypes";
        public const string InterestSettled = "InterestSettled";
        public const string InterestUnsettled = "InterestUnsettled";
        public const string IntRateHistory = "IntRateHistory";
        public const string Items = "Items";
        //CR 866 Added JobTitles 
        public const string JobTitles = "JobTitles";
        public const string LegalDetails = "LegalDetails"; //NM & IP - 08/01/09 - CR976
        public const string Letter = "Letter";
        public const string AdhocLetter = "AdhocLetter";
        public const string LineItem = "LineItem";
        public const string LineItemAudit = "LineItemAudit";
        public const string LinkedSpiffs = "LinkedSpiffs";
        public const string ManualCodes = "ManualCodes";
        public const string MaritalStatus = "MaritalStatus";
        public const string MethodOfPayment = "MethodOfPayment";
        public const string MmiMatrix = "MmiMatrix";
        public const string MonitorBookings = "MonitorBookings";
        public const string MonitorDeliveries = "MonitorDeliveries";
        public const string Nationality = "Nationality";
        public const string NonCourtsProducts = "NonCourtsProducts";
        public const string NonDeposits = "NonDeposits";
        public const string NonRFProposals = "NonRFProposals";
        public const string NonStockList = "NonStockList";
        //CR 866b Added Organisations 	
        public const string Organisations = "Organisations";
        public const string Occupation = "Occupation";
        public const string Operands = "Operands";
        public const string EquifaxOperands = "EquifaxOperands"; //Equifax Score Card ,Added by Nilesh
        public const string FlagCustomerStatus = "FlagCustomerStatus"; //Equifax Score Card ,Added by Nilesh
        public const string MobileNumber = "MobileNumber"; //Equifax Score Card ,Added by Nilesh
        public const string Operators = "Operators";
        public const string PartList = "PartList";
        public const string PayFrequency = "PayFrequency";
        public const string PaymentCard = "PaymentCard";
        public const string PaymentHolidays = "PaymentHolidays";
        public const string Payments = "Payments";
        public const string PayMethod = "PayMethod";
        public const string PaymentOption = "PaymentOption";
        public const string PayMethodList = "PayMethodList";
        public const string Picklist = "Picklist";
        public const string PrevDocConf = "PrevDocConf";
        public const string PriceIndexMatrix = "PriceIndexMatrix";
        public const string PolicyRules = "PolicyRules";
        public const string ProductCategories = "ProductCategories";
        public const string ProofOfAddress = "ProofOfAddress";
        public const string ProofOfBank = "ProofOfBank";                    //IP - 14/12/10 - Store Card
        public const string ProofOfID = "ProofOfID";
        public const string ProofOfIncome = "ProofOfIncome";
        public const string ProofOfIncomeEmployed = "ProofOfIncomeEmployed";
        public const string ProofOfIncomeSelfEmployed = "ProofOfIncomeSelfEmployed";
        public const string PropertyType = "PropertyType";
        public const string Proposal = "Proposal";
        public const string ProposalFlags = "ProposalFlags";
        public const string ProposalRef = "ProposalRef";
        public const string Rates = "Rates";
        public const string ReadyAssistTerms = "ReadyAssistTerms";          //#18604 - CR15594
        public const string Reasons = "Reasons";
        public const string RebatesByBranch = "RebatesByBranch";
        public const string RebatesAsAt = "RebatesAsAt";
        public const string RebatesTotals = "RebatesTotals";
        public const string References = "References";
        public const string ReferralAudit = "ReferralAudit";
        public const string ReferralCodes = "ReferralCodes";
        public const string ReferralData = "ReferralData";
        public const string ReferralRules = "ReferralRules";
        public const string RefRelation = "RefRelation";
        public const string RefundReasons = "RefundReasons";
        public const string Report = "Report";
        public const string ReportA = "ReportA";
        public const string ReportB = "ReportB";
        public const string ReportC = "ReportC";
        public const string ReportD = "ReportD";
        public const string ReScore = "ReScore";
        public const string Residential = "Residential";
        public const string ResidentialStatus = "ResidentialStatus";
        public const string Retailers = "Retailers";        //CR1030 jec
        public const string ReturnedCheques = "ReturnedCheques";
        public const string ReturnReasons = "ReturnReasons";
        public const string RFDetails = "RFDetails";
        public const string RFTransactions = "RFTransactions";
        public const string Rules = "Rules";
        public const string SalesCommission = "SalesCommission";  //CR36
        public const string SalesCommissionRates = "SalesCommissionRates";  //CR36
        public const string SalesCommissionMultiSPIFFRates = "SalesCommissionMultiSPIFFRates";  //CR36
        public const string SalesStaff = "SalesStaff";
        public const string LogonHistory = "LogonHistory";
        public const string SanctionStages = "SanctionStages";
        public const string Schedules = "Schedules";
        public const string ScoringDetails = "ScoringDetails";
        public const string ScoringMatrix = "ScoringMatrix";
        public const string Screens = "Screens";
        public const string Segments = "Segments";
        public const string Selected = "Selected";
        public const string SelectionAction = "SelectionAction";
        public const string ServiceActionRequired = "ServiceActionRequired"; //CR 949/958
        public const string ServiceChargeTo = "ServiceChargeTo";
        public const string ServiceChargeToAuthorisation = "ServiceChargeToAuthorisation";
        public const string ServiceCustomerInteraction = "ServiceCustomerInteraction"; //CR802 [PC]
        public const string ServiceCustomerInteractionType = "ServiceCustomerInteractionType"; //CR802 [PC]
        public const string ServiceDeliverer = "ServiceDeliverer";
        public const string ServiceEvaluation = "ServiceEvaluation";
        public const string ServiceFoodLoss = "ServiceFoodLoss";
        public const string ServiceLocation = "ServiceLocation";
        public const string ServiceMake = "ServiceMake";
        public const string ServiceModel = "ServiceModel";
        public const string ServicePartType = "ServicePartType";
        public const string ServiceResolution = "ServiceResolution";
        public const string ServiceRetailer = "ServiceRetailer";        //CR1030 jec
        public const string ServiceRequest = "ServiceRequest";
        public const string ServiceRequestAudit = "ServiceRequestAudit";
        public const string ServiceRequestAuditUpdate = "ServiceRequestAuditUpdate";
        public const string ServiceRequestNo = "ServiceRequestNo";
        public const string ServiceTechnician = "ServiceTechnician";
        public const string ServiceTechnicianPayments = "ServiceTechnicianPayments";
        public const string ServiceTechniciansForPaymentScreen = "ServiceTechniciansForPaymentScreen"; //CR802
        public const string ServiceZone = "ServiceZone";
        public const string SetsData = "SetsData";
        public const string SetDetailsData = "SetDetailsData";
        public const string SetBranchData = "SetBranchData";
        public const string SoftScript = "SoftScript";
        public const string Source = "Source";	//jec
        public const string SourceOfAttraction = "SourceOfAttraction";
        public const string SpaDetails = "SpaDetails";
        public const string SpaHistory = "SpaHistory";
        public const string Spiffs = "Spiffs";
        public const string Stage1 = "Stage1";
        public const string StockLocation = "StockLocation";
        public const string StorderProcess = "StorderProcess";
        public const string StorderControl = "StorderControl";	//jec
        public const string StorderDelimiters = "StorderDelimiters";	//IP - 16/09/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string Strategies = "Strategies";
        public const string StrategyActions = "StrategyActions"; //CR 852
        public const string StoreCardContactMethod = "StoreCardContactMethod";
        public const string StoreCardCancelReasons = "StoreCardCancelReasons";

        public const string StoreCardPaymentMethod = "StoreCardPaymentMethod";
        public const string StoreCardPaymentOption = "StoreCardPaymentOption";
        public const string StoreCardSecurityQuestion = "StoreCardSecurityQuestion";
        public const string StoreCardQualRules = "StoreCardQualRules";                  //IP - 9/12/10 - Store Card
        public const string StoreCardStatus = "StoreCardStatus";

        public const string Technician = "Technician";	//pc
        public const string TechnicianByZone = "TechnicianByZone";	//pc
        public const string TechnicianDiary = "TechnicianDiary";
        public const string TechnicianMaxAndCurrJobs = "TechnicianMaxAndCurrJobs"; // Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.																																														  
        public const string TelephoneHistory = "TelephoneHistory";
        public const string TempReceipt = "TempReceipt";
        public const string TermsType = "TermsType";
        public const string TermsTypeAccountType = "TermsTypeAccountType";
        public const string TermsTypeBand = "TermsTypeBand";
        public const string TermsTypeBandList = "TermsTypeBandList";
        public const string TermsTypeFreeInstallments = "TermsTypeFreeInstallments";
        public const string TermsTypeVariableRates = "TermsTypeVariableRates";
        public const string TermsTypeMatrix = "TermsTypeMatrix";
        public const string TermsTypeMaxTerm = "TermsTypeMaxTerm";
        public const string TermsTypeMinTerm = "TermsTypeMinTerm";
        public const string TermsTypeLength = "TermsTypeLength";
        public const string Title = "Title";
        public const string TNameBranch = "branch";
        public const string TNameDeliveryArea = "deliveryarea";
        public const string TNameEmployee = "employee";
        public const string TNameTransType = "transtype";
        public const string TraceDetails = "TraceDetails"; //jec - 02/06/09 - CR976
        public const string Transactions = "Transactions";
        public const string TransTypes = "TransTypes";
        public const string Transport = "Transport";
        public const string TransportData = "TransportData";
        //CR 866 Added Transport Type 
        public const string TransportTypes = "TransportTypes";
        public const string UnicodeToAscii = "UnicodeToAscii";
        public const string UserFunctions = "UserFunctions";
        public const string UserTypes = "UserTypes";
        public const string WarrantyList = "WarrantyList";
        public const string WarrantyCollectionReason = "WarrantyCollectionReason"; // CR 822 [Peter Chong] 02-10-2006        
        public const string WorkList = "WorkList";
        public const string WorkListActions = "WorkListActions";
        public const string WorkListEmployeeTypes = "WorkListEmployeeTypes";
        public const string WriteOffCodes = "WriteOffCodes";
        public const string TransferReasons = "TransferReasons";
        public const string GiftVoucherOther = "GiftVoucherOther";
        public const string WriteOffCategories = "WriteOffCategories";
        public const string CountryParameterCategories = "CountryParameterCategories";
        public const string TaxTypes = "TaxTypes";
        public const string TaxInvoiceFormats = "TaxInvoiceFormats";
        public const string ServicePercentFormats = "ServicePercentFormats";
        public const string CashDrawerReasons = "CashDrawerReasons";
        public const string CashierOutstandingIncome = "CashierOutstandingIncome";
        public const string WarrantyCategories = "WarrantyCategories";
        public const string Warranties = "Warranties";
        public const string Worklists = "Worklists";
        public const string SummaryControl = "SummaryControl";
        public const string VariableInstal = "VariableInstal";
        public const string TTOverview = "TTOverview";
        public const string AcctDelStatus = "AcctDelStatus"; //IP - 19/05/08 - Account Status Screen 
        public const string OracleExport = "OracleExport";
        public const string ScoreCards = "ScoreCards";
        public const string DepositWaiver_Category = "DepositWaiver_Category";
        public const string DepositWaiver_Level = "DepositWaiver_Level";
        public const string DepositWaiver_Product = "DepositWaiver_Product";
        public const string DepositWaiver_SubClass = "DepositWaiver_SubClass";          //IP - 27/07/11 - RI - #4415
        public const string BandLimitChange = "BandLimitChange";
        public const string ReassignReason = "ReassignReason";      //CR1094 jec
        public const string RIInterfaceOptions = "RIInterfaceOptions";      //IP - 22/06/11 - CR1212 - RI - #3987
        public const string CashLoanAccountTypes = "CashLoanAccountTypes";      //CR1232 jec 19/09/11

        public const string AmortizedSchedule = "AmortizedSchedule";     //Table name to  display amortization schedule
        public const string Villages = "Villages";  //Address Standardization CR2019 - 025
    }
}

namespace STL.Common.Constants.TabPageNames
{
    //struct used for TabPage name constants
    public struct TPN
    {
        public const string AllocationHistory = "tpAllocation";
    }
}


namespace STL.Common.Constants.Categories
{
    // Category Codes
    public struct CAT
    {
        public const string AccountCode1 = "AC1";
        public const string AccountCode2 = "AC2";
        public const string AcctSelectAction = "ASA";
        public const string AcctSelectionAllocation = "AST";
        public const string AcctSelectionArrears = "ASR";
        public const string AcctSelectionCode = "ASC";
        public const string AcctSelectionLetter = "ASL";
        public const string AcctSelectionPoints = "ASP";
        public const string AcctSelectionStatus = "ASS";
        public const string AdditionalLetter = "LT2";
        public const string BailiffCommission = "BC1";
        public const string BankAccountType = "BA2";
        public const string BankActTypeCRef = "BA1";
        public const string BusinessPremises1 = "BP1";
        public const string BusinessPremises2 = "BP2";
        public const string CancellationCode = "CN2";
        public const string CardPrintType = "CPT";
        public const string CorrectionReason = "FT ";
        public const string Correction1 = "FT1";
        public const string Correction2 = "FT2";
        public const string CreditCardType = "CCT";
        public const string Currency = "CU1";
        public const string CustomerAddressCode = "CA1";
        public const string CustomerCode1 = "CC1";
        public const string CustomerCode2 = "CC2";
        public const string CustomerTelCode = "CT1";
        public const string DelNoteRemoveReason = "DS1";
        public const string DelNoteDeleteReason = "DND"; //IP - 18/02/09 - CR929 & 974
        public const string EmployeeType = "ET1";
        public const string EmploymentStatus = "ES2";
        public const string EmployStatusCRef = "ES1";
        public const string EthnicGroupCRef = "EG1";
        public const string FinalDecisnCRef = "FD1";
        public const string FintransPayMethod = "FPM";
        public const int FPMForeignCurrency = 100;
        public const string FintransReasonCode = "RC2";
        public const string FollowUpAction = "FUP";
        public const string FullOrPartTime1 = "FP1";
        public const string FullOrPartTime2 = "FP2";
        public const string IdTypeCRef = "IT1";
        public const string InsProductCRef = "IP1";
        public const string InstallationItem = "INST";
        public const string KitItemCatDiscount = "KCD";
        public const string LetterCode = "LT1";
        public const string LetterCodeAdditional = "LT2";
        public const string LocationCRef = "LO1";
        public const string MaritalStatCRef = "MS1";
        public const string MaritalStatus = "MS2";
        public const string MethodOfPayment = "MOP";
        public const string MoneyTransfer1 = "REM";
        public const string MoneyTransfer2 = "XFR";
        public const string Nationality = "NA2";
        public const string NationalityCRef = "NT1";
        public const string OccupationCRef = "WT1";
        public const string OverrideReasCRef = "OV1";
        public const string PayFrequency = "PF2";
        public const string PaymentFreqCRef = "PF1";
        public const string PayMethodCRef = "PM1";
        public const string ProductCatElectrical = "PCE";
        public const string ProductCatFurniture = "PCF";
        public const string ProductCatOther = "PCO";
        public const string ProductCatWarehouse = "PCW";
        public const string ProductCatDelivery = "PCD";
        public const string ProofAddressCRef = "PAD";
        public const string ProofIDCRef = "PID";
        public const string ProofIncomeCRef = "PIN";
        public const string PropertyTypeCRef = "PT1";
        public const string ProposalHold = "PH2";
        public const string ReassignReason = "SRREASON";     //CR1030 jec
        public const string RefundReason = "RF1";
        public const string RelationshipCRef = "RL1";
        public const string ResStatusCRef = "RS1";
        public const string SanctionReason = "SN1";
        public const string SanctionReferralReason = "RR1";
        public const string SanctionResult = "SR1";
        public const string SanctionStages = "PH1";
        public const string ServiceActionRequired = "SRSERVACT";
        public const string ServiceChargeTo = "SRCHARGE";
        public const string ServiceCustomerInteraction = "SRCUSTINT";
        public const string ServiceDeliverer = "SRDELIVERER";
        public const string ServiceEvaluation = "SREVALN";
        public const string ServiceLocation = "SRSERVLCN";
        public const string ServiceMake = "SRPRODUCT";
        public const string ServiceModel = "SRSUPPLIER";
        public const string ServiceResolution = "SRRESOLVE";
        public const string ServiceRetailer = "SRRETAILER";     //CR1030 jec
        public const string ServiceZone = "SRZONE";
        public const string SoftScript = "SRSCRIPT";
        public const string SundryCharges = "SCH";
        public const string SourceOfAttraction = "SOA";
        public const string SPACode = "SP2";
        public const string TemporaryOrPerm1 = "TP1";
        public const string TemporaryOrPerm2 = "TP2";
        public const string Title = "TTL";
        public const string TaxRates = "TXR";
        public const string UserAuthorisation = "UA1";
        public const string Warranty = "WAR";
        public const string WorkType = "WT2";


    }
}

namespace STL.Common.Constants.SanctionStages
{
    // Sanction Stages
    public struct SS
    {
        public const string S1 = "S1";
        public const string S2 = "S2";
        public const string AD = "AD";
        public const string R = "R";
        public const string DC = "DC";
    }
    // Proposal Results
    public struct PR
    {
        public const string Accepted = "A";
        public const string Referred = "R";
        public const string Rejected = "X";
        public const string Decline = "D";
    }
}

namespace STL.Common.Constants.CountryCodes
{
    public struct CC
    {
        public const string Indonesia = "I";
    }
}

namespace STL.Common.Constants.OperandTypes
{
    public struct OT
    {
        public const string FreeText = "free";
        public const string Numeric = "numeric";
        public const string Decimal = "float";
        public const string Optional = "option";
    }
}

namespace STL.Common.Constants.AccountTypes
{
    public class AT
    {
        public const string Cash = "C";
        public const string HP = "H";
        public const string Special = "S";
        public const string BuyNowPayLater = "B";
        public const string ReadyFinance = "R";
        public const string GoodsOnLoan = "L";
        public const string AddTo = "M";
        public const string StoreCard = "T";
        public const string CashSR = "SRC";         //jec 14/01/11 

        public const string BuyNowPayLater4 = "D";
        public const string BuyNowPayLater5 = "E";
        public const string BuyNowPayLater2 = "F";
        public const string BuyNowPayLater1 = "G";

        public static bool IsCashType(string type)
        {
            type = type.Trim().ToUpper();
            return (type == AT.Cash ||
                type == AT.Special ||
                type == AT.CashSR ||        //jec 14/01/11
                type == AT.GoodsOnLoan);
        }

        public static bool IsCreditType(string type)
        {
            type = type.Trim().ToUpper();
            return (type == AT.HP ||
                type == AT.AddTo ||
                type == AT.ReadyFinance ||
                type == AT.StoreCard ||
                type == AT.BuyNowPayLater ||
                type == AT.BuyNowPayLater1 ||
                type == AT.BuyNowPayLater2 ||
                type == AT.BuyNowPayLater4 ||
                type == AT.BuyNowPayLater5);
        }
    }
}
// Instant Credit Approval          CR907  jec 31/07/07
namespace STL.Common.Constants.InstantCredit
{
    public struct IC
    {
        public static readonly string Approved = "Y";
        public static readonly string Declined = "N";
        public static readonly string Granted = "G";
    }
}


namespace STL.Common.Constants.AccountCodes
{
    public class AC
    {
        public const string FreeInstalment = "FREE";
    }
}

namespace STL.Common.Constants.CustomerTypes
{
    public class BDW
    {
        public static readonly string Recover = "BDWRECOVER";
        public static readonly string SecuritisedRecover = "BDWSECRECOVER";

        public static bool IsBDW(string type)
        {
            type = type.Trim().ToUpper() + "1234567890123";
            return (type.Substring(0, 10) == Recover ||
                type.Substring(0, 13) == SecuritisedRecover);
        }

        public static bool IsBDWSecuritised(string type)
        {
            type = type.Trim().ToUpper() + "1234567890123";
            return (type.Substring(0, 13) == SecuritisedRecover);
        }

    }
}

namespace STL.Common.Constants.TermsTypes
{
    public class TT
    {
        public static readonly string WarrantyOnCredit = "WC";
        public static readonly string PTWarranty = "PT";

        public static bool IsPaidAndTaken(string termsType)
        {
            return (termsType == WarrantyOnCredit || termsType == PTWarranty);
        }
    }
}

namespace STL.Common.Constants.EmployeeTypes
{
    public struct ET
    {
        public static readonly string Bailiff = "B";
        public static readonly string Telephone = "T";
    }
}

namespace STL.Common.Constants.CodeReference
{
    public struct ECT
    {
        // Employee Commission Type defined by the reference number
        // on Employee Type code maintenance.
        // Pay commission only after a previous action allows commission
        public static readonly short CommissionOnAction = 2;
        // Always pay commission
        public static readonly short CommissionAlways = 3;
    }
}

namespace STL.Common.Constants.RateTypes
{
    public struct RT
    {
        public static readonly string Fixed = "F";
        public static readonly string Variable = "V";
    }
}

namespace STL.Common.Constants.Giro
{
    public struct DDPaymentType
    {
        // Payment Type
        public const string Normal = "N";
        public const string Fee = "F";
        public const string Extra = "E";
        public const string Represent = "R";
    }

    public struct DDStatus
    {
        // Mandate Status (Pending, Active and Terminated are
        // derived from the dates and are stored as Current)
        public const string Pending = "P";
        public const string Active = "A";
        public const string Terminated = "T";
        public const string Current = "C";
        public const string History = "H";
        public const string Deleted = "D";

    }

    public struct DDCancel
    {
        // Mandate cancel reasons
        public const string Init = "I";
        public const string NotApproved = "N";
        public const string TwoRejections = "T";
        public const string MaxRejections = "M";
        public const string CancelledReject = "C";
        public const string UserCancelled = "U";
        public const string Settled = "S";
    }

    public struct DDLetter
    {
        // Letter codes (2 chars max)
        public const string Cancel = "GX";
        public const string Return = "GR";
    }

    public struct DDRejection
    {
        // Rejection Actions
        public const string Init = "I";
        public const string NotRepresent = "N";
        public const string Represent = "R";
        public const string Cancel = "C";
    }

    public struct DDFileName
    {
        // File names
        public const string ServerPath = @"D:\cosdata\giro\";
        public const string Normal = "dd";
        public const string Extra = "dd";
        public const string Represent = "dd";
        public const string Fee = "dd";
        public const string Reject = "ddreject";
        public const string RejectFormat = "ddrejectformat";
        public const string Processed_Reject = "ddrej_p";
    }

    public struct DDFileSuffix
    {
        // File suffixes
        public const string Normal = ".pay";
        public const string Extra = ".ext";
        public const string Represent = ".rep";
        public const string Fee = ".fee";
        public const string Reject = ".ddx";
        public const string Format = ".fmt";
        public const string Tmp = ".~tmp";
    }

    public struct DDErrorFileName
    {
        // File names for error log messages
        public const string Normal = "Normal";
        public const string Extra = "Extra";
        public const string Fee = "Fee";
        public const string Represent = "Representation";
    }

    public struct DDRecordLength
    {
        // Record lengths in bank files (including newline char in rejections file)
        public const int LineLengthMax = 80;
        public const int LineLengthPay = 80;
        public const int LineLengthRej = 80;
    }

    public struct DDHeaderField
    {
        /* Static header fields in bank files */
        public const string FieldA = "G";
        public const string FieldB = "150";
        public const string FieldG = "K";
        public const string FieldH = "********";
    }

    public struct DDHeaderFieldLen
    {
        /* Header field lengths in bank files */
        public const int BankAcctNo = 9;
        public const int PayCode = 3;
        public const int CreationDate = 12;
        public const int BACSDate = 6;
        public const int RecordCount = 5;
        public const int TotalAmount = 10;
    }

    public struct DDFieldLen
    {
        // Detail field lengths in bank files
        public const int RejectReason = 1;
        public const int CourtsAcctNo = 12;
        public const int BankAcctName = 20;
        public const int BankCode = 4;
        public const int BankBranchNo = 3;
        public const int BankAcctNo = 16;
        public const int Amount = 10;
        public const int RejectReason2 = 2;
        public const int RejectCode = 3;
    }

    public struct DDFormat
    {
        // Field template for the numeric fields in the bank files.
        // This is used with $DDHL_RecordCount, $DDHL_TotalAmount and $DDFL_Amount.
        // The template should be as long as the longest of these fields.
        public const string Numeric = "0000000000";

        // Field template for bank file delete records
        public const string Delete = "DELETE";

    }

    public struct DDEnable
    {
        // EOD Payment processing enable window in days
        public const int PaymentWindow = 4;
    }

}

//CR 843 Added constants for credit bureaus.
namespace STL.Common.Constants.CreditBureau
{
    public struct CreditBureau
    {
        public const string Baycorp = "B";
        public const string DPGroup = "D";
        public const string Unknown = "";
    }
}
//End CR 843 Change

namespace STL.Common.Constants.FTransaction
{
    // Transaction Types
    public struct TransType
    {
        public const string AddTo = "ADD";
        public const string AdminCharge = "ADM";
        public const string AnnualServiceRecoveryCredit = "ASR";
        public const string AnnualServiceRecoveryCash = "ASC";
        public const string CashLoan = "CLD";                 //IP - 11/10/11 - #3921 - CR1232
        public const string Correction = "COR";
        public const string ElecWarrantyRecovery = "CRE";
        public const string FurnWarrantyRecovery = "CRF";
        public const string GiroExtra = "DDE";
        public const string GiroNormal = "DDN";
        public const string GiroRepresent = "DDR";
        public const string GiroFeeRaised = "DDF";
        public const string GiroFeePaid = "DDG";
        public const string Delivery = "DEL";
        public const string CreditFee = "FEE";
        public const string GoodsReturn = "GRT";
        public const string Interest = "INT";
        public const string InsClaimTransferredToRecovery = "IPR"; //IP - 16/04/09 - CR971 - Archiving
        public const string InsClaimAfterWriteoff = "IPY"; //IP - 16/04/09 - CR971 - Archiving
        public const string InstallationElectrical = "INE";
        public const string InstallationFurniture = "INF";
        public const string TakeonTransfer = "JLX";
        public const string Payment = "PAY";
        public const string ReadyAssistRecoveryCredit = "RAR";     //#18607 - CR15594
        public const string ReadyAssistRecoveryCash = "RRC";     //#19267 - CR15594
        public const string Redelivery = "RDL";
        public const string Rebate = "REB";
        public const string Refund = "REF";
        public const string Repossession = "REP";
        public const string RepoTransferredToRecovery = "RPR"; //IP - 15/04/09 - CR971 - Archiving
        public const string RepoAfterWriteoff = "RPY"; //IP - 15/04/09 - CR971 - Archiving
        public const string RefinanceDep = "RFD";      // CR976 Refinance Deposit jec
        public const string Refinance = "RFN";      // CR976 Refinance Balance jec
        public const string Repossession2 = "RPO";
        public const string Return = "RET";
        public const string SundryCreditTransfer = "SCX";
        public const string CashiersShortageAdjustment = "CAS";             //IP - 14/02/12 - #8819 - CR1234
        public const string Transfer = "XFR";
        public const string Update = "UPD";
        public const string Shortage = "SHO";
        public const string ShortageWriteoff = "SWO";
        public const string Overage = "OVE";
        public const string CreditAdjustment = "ADJ";
        public const string InsuranceClaim = "INS";
        public const string BadDebtWriteOff = "BDW";
        public const string BadDebtUnearnedFinanceIncome = "BDU";
        public const string TraceFee = "TRC";
        public const string Safe = "SAF";
        public const string GiftVoucherJournal = "JGF";
        public const string DebtPayment = "DPY";
        public const string OldAddToReversal = "ADX";
        public const string FreeInstalment = "MKT";
        public const string ReturnedChequeAuthorisation = "RTA"; //CR 543 Added Peter Chong [26-Sep-2006]
        public const string InsuranceWarrantyRefund = "INW";     //CR 822 Added Peter Chong [28-Sep-2006]
        // uat376 rdb BDW Reversal
        public const string BadDebtWriteOffReversal = "BDR";
        public const string StoreCardPayment = "SCT";
        public const string StoreCardRefund = "STR";
        public const string StoreCardAnnualFee = "STA";
        public const string StoreCardCardReplaceFee = "SRF";
        public const string StoreCardCardStatementFee = "SSF";
        public const string StoreCardInterest = "INT";

        public const string WriteOffService = "WOS";        //IP - 08/02/11 - Sprint 5.10 - #2977 //IP - 15/02/11 - Sprint 5.10 - #2977 changed from SDW to WOS

        #region CL Amortization Outstanding Balnce Calculation
        /*
         * CR       : #CL Amortization Outstanding Balnce Calculation
         * Date     : 25/July/2019
         * Author   : Rahul D, Zensar
         * Details  : Add new Trans type codes for Cash loan Account
         */
        public const string CLAdminReversal = "ADC";
        public const string CLCreditBalance = "CLC";
        public const string CLBDRecovey = "CLR";
        public const string CLWriteOff = "CLW";
        public const string CLAdminFee = "CLA";
        public const string CLInsuranceClaim = "INC";
        public const string CLPenaltyInterest = "CLP";
        public const string CLLateFeeReversal = "CLL";
        public const string CLServiceChargeCorrection = "SCC";
        #endregion
    }
    // Payment Methods
    public static class PayMethod
    {
        public const short Cash = 1;
        public const short Cheque = 2;
        public const short CreditCard = 3;
        public const short DebitCard = 4;
        public const short StandingOrder = 5;
        public const short WirelessTransfer = 6;
        public const short ATM = 7;
        public const short GiftVoucher = 8;
        public const short StoreCard = 13;
        public const short USdollars = 100;
        public const short USCheque = 102;                  //IP - 23/05/12 - #10154
        public const short ElectronicBankTransfer = 84;

        //New CL Pay Methods Mambu
        public const short BSCash = 211;
        public const short BSCheque = 212;
        public const short BSDebitCard = 213;
        public const short BSCreditCard = 233;

        public const short BSUSDCash = 221;
        public const short BSCADCash = 231;
        public const short BSGBPCash = 241;
        public const short BSUSDCheque = 222;
        public const short BSCADCheque = 232;
        public const short BSGBPCheque = 242;
        public const short BSUSDCreditCard = 223;

        public const short BSNCBATM = 214;
        public const short BSBNSATM = 217;

        public const short BSWirelessTransfer = 215;
        public const short BSStandingOrder = 225;

        public const short BSGiftVoucher = 218;
        public const short ePaymentBNS = 220;
        public const short BSElectronicBankTransfer = 226;

        public const short BSCashBNS = 251;
        public const short BSChequeBNS = 252;
        public const short BSUSDCashBNS = 261;
        public const short BSUSDCreditCardBNS = 243;
        public const short BSCADCashBNS = 271;
        public const short BSGBPCashBNS = 281;
        public const short BSWirelessTransferBNS = 219;
        public const short BSStandingOrderBNS = 216;
        public const short BSGiftVoucherBNS = 228;
        public const short BSDebitCardBNS = 253;
        public const short BSCreditCardBNS = 263;
        public const short BSElectronicBankTransferBNS = 224;

        //New HP Pay Methods Mambu
        public const short UNCASHNCB = 501;
        public const short UNCHEQUENCB = 502;
        public const short UNCASHBNS = 511;
        public const short UNCHEQUEBNS = 512;
        public const short UNCardNCB = 523;
        public const short UNDebitCardNCB = 533;
        public const short UNCreditCardNCB = 543;

        public const short UNUSDCashNCB = 521;
        public const short UNUSDCashBNS = 531;
        public const short UNCADCashNCB = 591;
        public const short UNCADCashBNS = 541;
        public const short UNGBPCashNCB = 551;
        public const short UNGBPCashBNS = 561;
        public const short UNKYDCashNCB = 571;
        public const short UNKYDCashBNS = 581;
        //public const short UNUSDChequeNCB = 502;
        public const short UNUSDChequeBNS = 582;
        //public const short UNCADChequeNCB = 512;
        public const short UNCADChequeBNS = 522;
        public const short UNGBPChequeNCB = 532;
        public const short UNGBPChequeBNS = 592;
        public const short UNKYDChequeNCB = 542;
        public const short UNKYDChequeBNS = 552;
        public const short UNStaffChequesNCB = 562;
        public const short UNStaffChequesBNS = 572;
        public const short UNUSDCreditCardNCB = 503;
        public const short UNUSDCreditCardBNS = 513;

        public const short UNATMNCB = 504;
        public const short UNATMBNS = 505;

        public const short UNStandingOrderNCB = 514;

        public const short UNWirelessTransferNCB = 515;
        public const short UNWirelessTransferBNS = 516;

        public const short UNJMMBNCB = 507;
        public const short UNJMMBBNS = 508;

        public const short UNInternetbillBNS = 517;

        public const short UNGiftVoucherNCB = 518;

        public const short UNJNTransactionsCash = 491;
        public const short UNJNTransactionsCard = 553;
        //public const short UNJNTransactionsNCB = 509;
        //public const short UNJNTransactionsBNS = 519;

        public const short UNePaymentBNS = 510;
        
        //New Cash Loan Pay methods
        public const short BSNEWATM = 317;
        public const short BSNEWCash = 351;
        public const short BSNEWCheque = 352;
        public const short BSNEWCreditCard = 363;
        public const short BSNEWDebitCard = 353;
        public const short BSNEWElectronicBankTransfer = 324;
        public const short BSNEWGiftVoucher = 328;
        public const short BSNEWStandingOrder = 316;
        public const short BSNEWWirelessTransfer = 319;

        public static bool IsPayMethod(int curPayMethod, int payMethod)
        {
            // Only compare the last digit
            //int leftDigits = (int)(curPayMethod / 10) * 10;
            int leftDigits = curPayMethod > 100 ? (int)(curPayMethod / 10) * 10 : 0;    //IP- 23/05/12 - #10173
            int lastDigit = curPayMethod - leftDigits;

            return (lastDigit == (payMethod == PayMethod.USdollars ? 0 : payMethod));            // #10158 reinstate & allow for US$ code not being 101 jec 22/05/12
            //return (lastDigit == (payMethod - leftDigits)); //IP - 29/11/10 
        }

        public static bool IsForeignCurrencyPayMethod(int curPayMethod)
        {
            if ((curPayMethod >= 100 && curPayMethod <= 200) || (curPayMethod == PayMethod.UNUSDCashNCB) || (curPayMethod == PayMethod.UNUSDCashBNS)
                                    || (curPayMethod == PayMethod.UNCADCashNCB) || (curPayMethod == PayMethod.UNCADCashBNS)
                                    || (curPayMethod == PayMethod.UNGBPCashNCB) || (curPayMethod == PayMethod.UNGBPCashBNS)
                                    || (curPayMethod == PayMethod.UNKYDCashNCB) || (curPayMethod == PayMethod.UNKYDCashBNS)
                                    || (curPayMethod == PayMethod.UNJNTransactionsCash)
                                    || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                    || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                    || (curPayMethod == PayMethod.UNGBPChequeNCB) || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                    || (curPayMethod == PayMethod.UNKYDChequeNCB) || (curPayMethod == PayMethod.UNKYDChequeBNS)
                                    || (curPayMethod == PayMethod.UNStaffChequesNCB) || (curPayMethod == PayMethod.UNStaffChequesBNS)
                                    || (curPayMethod == PayMethod.UNUSDCreditCardNCB) || (curPayMethod == PayMethod.UNUSDCreditCardBNS)
                                    || (curPayMethod == PayMethod.UNJNTransactionsCard) || (curPayMethod == PayMethod.BSUSDCash) || (curPayMethod == PayMethod.BSCADCash) || (curPayMethod == PayMethod.BSGBPCash)
                                    || (curPayMethod == PayMethod.BSUSDCheque) || (curPayMethod == PayMethod.BSCADCheque) || (curPayMethod == PayMethod.BSGBPCheque)
                                    || (curPayMethod == PayMethod.BSUSDCreditCard) || (curPayMethod == PayMethod.BSUSDCashBNS) || (curPayMethod == PayMethod.BSUSDCreditCardBNS)
                                    || (curPayMethod == PayMethod.BSCADCashBNS) || (curPayMethod == PayMethod.BSGBPCashBNS))
                return true;
            else
                return false;
        }


        public static bool IsPayByCash(short curPayMethod)
        {
            return ((curPayMethod == PayMethod.UNCASHNCB) || (curPayMethod == PayMethod.UNCASHBNS)
                                        || (curPayMethod == PayMethod.UNUSDCashNCB) || (curPayMethod == PayMethod.UNUSDCashBNS)
                                        || (curPayMethod == PayMethod.UNCADCashNCB) || (curPayMethod == PayMethod.UNCADCashBNS)
                                        || (curPayMethod == PayMethod.UNGBPCashNCB) || (curPayMethod == PayMethod.UNGBPCashBNS)
                                        || (curPayMethod == PayMethod.UNKYDCashNCB) || (curPayMethod == PayMethod.UNKYDCashBNS)
                                        || (curPayMethod == PayMethod.UNJNTransactionsCash) || (curPayMethod == PayMethod.BSCash)
                                        || (curPayMethod == PayMethod.BSCashBNS) || (curPayMethod == PayMethod.BSUSDCash)
                                 || (curPayMethod == PayMethod.BSUSDCashBNS) || (curPayMethod == PayMethod.BSCADCash) || (curPayMethod == PayMethod.BSCADCashBNS)
                                 || (curPayMethod == PayMethod.BSGBPCash) || (curPayMethod == PayMethod.BSGBPCashBNS) || (curPayMethod == PayMethod.Cash)
                                 || (curPayMethod == PayMethod.BSNEWCash));
        }

        public static bool IsPayByCheque(short curPayMethod)
        {
            return ((curPayMethod == PayMethod.UNUSDChequeBNS)
                               || (curPayMethod == PayMethod.UNUSDChequeBNS)
                               || (curPayMethod == PayMethod.UNGBPChequeNCB) || (curPayMethod == PayMethod.UNUSDChequeBNS)
                               || (curPayMethod == PayMethod.UNKYDChequeNCB) || (curPayMethod == PayMethod.UNKYDChequeBNS)
                               || (curPayMethod == PayMethod.UNStaffChequesNCB) || (curPayMethod == PayMethod.UNStaffChequesBNS)
                               || (curPayMethod == PayMethod.UNCHEQUEBNS) || (curPayMethod == PayMethod.UNCHEQUENCB) || (curPayMethod == PayMethod.BSCheque)
                               || (curPayMethod == PayMethod.BSChequeBNS) || (curPayMethod == PayMethod.BSUSDCheque)
                            || (curPayMethod == PayMethod.BSCADCheque) || (curPayMethod == PayMethod.BSGBPCheque) || (curPayMethod == PayMethod.Cheque)
                            || (curPayMethod == PayMethod.USCheque)
                            || (curPayMethod == 122) || (curPayMethod == 132) || (curPayMethod == 142) || (curPayMethod == 152)
                            || (curPayMethod == 162) || (curPayMethod ==PayMethod.BSNEWCheque));
        }

        public static bool IsPayByCard(short curPayMethod)
        {
            return ((curPayMethod == PayMethod.UNUSDCreditCardNCB) || (curPayMethod == PayMethod.UNUSDCreditCardBNS)
                             || (curPayMethod == PayMethod.UNCardNCB) || (curPayMethod == PayMethod.UNDebitCardNCB)
                             || (curPayMethod == PayMethod.UNCreditCardNCB) || (curPayMethod == PayMethod.UNJNTransactionsCard)
                             || (curPayMethod == PayMethod.BSUSDCreditCard) || (curPayMethod == PayMethod.BSUSDCreditCardBNS)
                          || (curPayMethod == PayMethod.BSDebitCard) || (curPayMethod == PayMethod.BSDebitCardBNS) || (curPayMethod == PayMethod.BSCreditCard)
                          || (curPayMethod == PayMethod.BSCreditCardBNS) || (curPayMethod == PayMethod.CreditCard) || (curPayMethod == PayMethod.DebitCard)
                          || (curPayMethod == PayMethod.BSNEWCreditCard) || (curPayMethod == PayMethod.BSNEWDebitCard));
        }

        public static bool IsPayByGift(short curPayMethod)
        {
            return ((curPayMethod == PayMethod.UNGiftVoucherNCB) || (curPayMethod == PayMethod.GiftVoucher)
                                || (curPayMethod == PayMethod.BSGiftVoucher) || (curPayMethod == PayMethod.BSGiftVoucherBNS)
                                || (curPayMethod == PayMethod.BSNEWGiftVoucher));
        }

        public static bool IsPayByForeign(short curPayMethod)
        {
            return ((curPayMethod == PayMethod.UNUSDCashNCB) || (curPayMethod == PayMethod.UNUSDCashBNS)
                                || (curPayMethod == PayMethod.UNCADCashNCB) || (curPayMethod == PayMethod.UNCADCashBNS)
                                || (curPayMethod == PayMethod.UNGBPCashNCB) || (curPayMethod == PayMethod.UNGBPCashBNS)
                                || (curPayMethod == PayMethod.UNKYDCashNCB) || (curPayMethod == PayMethod.UNKYDCashBNS)
                                || (curPayMethod == PayMethod.UNJNTransactionsCash)
                                || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                || (curPayMethod == PayMethod.UNGBPChequeNCB) || (curPayMethod == PayMethod.UNUSDChequeBNS)
                                || (curPayMethod == PayMethod.UNKYDChequeNCB) || (curPayMethod == PayMethod.UNKYDChequeBNS)
                                || (curPayMethod == PayMethod.UNStaffChequesNCB) || (curPayMethod == PayMethod.UNStaffChequesBNS)
                                || (curPayMethod == PayMethod.UNUSDCreditCardNCB) || (curPayMethod == PayMethod.UNUSDCreditCardBNS)
                                || (curPayMethod == PayMethod.UNJNTransactionsCard) || (curPayMethod == PayMethod.BSUSDCash) || (curPayMethod == PayMethod.BSUSDCashBNS) || (curPayMethod == PayMethod.BSCADCash)
                            || (curPayMethod == PayMethod.BSCADCashBNS) || (curPayMethod == PayMethod.BSGBPCash) || (curPayMethod == PayMethod.BSGBPCashBNS)
                            || (curPayMethod == PayMethod.BSUSDCheque) || (curPayMethod == PayMethod.BSCADCheque)
                            || (curPayMethod == PayMethod.BSGBPCheque) || (curPayMethod == PayMethod.BSUSDCreditCard)
                            || (curPayMethod == PayMethod.BSUSDCreditCardBNS) || (curPayMethod == PayMethod.USdollars) || (curPayMethod == PayMethod.USCheque)
                            || (curPayMethod == 103) || (curPayMethod == 111) || (curPayMethod == 121) || (curPayMethod == 122) || (curPayMethod == 131)
                            || (curPayMethod == 132) || (curPayMethod == 141) || (curPayMethod == 142) || (curPayMethod == 151) || (curPayMethod == 152)
                            || (curPayMethod == 162) || (curPayMethod == 101));
        }
    }

    public enum PayMethods
    {
        Cash = 1,
        Cheque = 2,
        CreditCard = 3,
        DebitCard = 4,
        StandingOrder = 5,
        WirelessTransfer = 6,
        ATM = 7,
        GiftVoucher = 8,
        StoreCard = 13,
        USdollars = 100,                 //IP - 22/05/12 - #10154
        USCheque = 102                   //IP - 23/05/12 - #10154
    }

    //Collection Types
    public struct CollectionType
    {
        public static readonly string Repossession = "R";
        public static readonly string Whole = "W";
        public static readonly string Part = "P";
    }

    // Foot Notes
    public struct FootNote
    {
        public static readonly string ImmediateDelivery = "IMDL";
        public static readonly string AddToDelivery = "ADDL";
        public static readonly string Automatic = "AUTO";
        public static readonly string AddtoCreditReversal = "CREV";
        public static readonly string AddtoDebitReversal = "DREV";
        public static readonly string NonStockCollection = "NSCO";
        public static readonly string CancelCollectionNote = "CCNT";
        public static readonly string FreeInstalment = "FREE";
    }

    // Sources
    public struct Source
    {
        public static readonly string CoSACS = "COSACS";
    }

    //IP - 16/05/12 - #9447 - CR1239
    public struct AuditType
    {
        public static readonly string CashAndGoReturnPrint = "CashAndGoReturnPrint";
        public static readonly string CashAndGoPrint = "CashAndGoPrint";
        public static readonly string CashAndGoSingleReprint = "SingleReprint";
        public static readonly string CashAndGoBulkReprint = "BulkReprint";

    }

}

namespace STL.Common.Constants.AccountHolders
{
    public struct Holder
    {
        public static readonly string Main = "H";
        public static readonly string Joint = "J";
        public static readonly string Spouse = "S";
    }
}

namespace STL.Common.Constants.ScreenModes
{
    public struct SM
    {
        public const string New = "New";
        public const string View = "View";
        public const string Edit = "Edit";
    }
}

namespace STL.Common.Constants.ItemTypes
{
    public struct IT
    {
        public const string Stock = "Stock";
        public const string Component = "Component";
        public const string Kit = "Kit";
        public const string Warranty = "Warranty";
        public const string KitWarranty = "KitWarranty";
        public const string Discount = "Discount";
        public const string KitDiscount = "KitDiscount";
        public const string Affinity = "Affinity";
        public const string SundryCharge = "Sundry Charge";
        public const string Unknown = "Unknown";
        public const string Installation = "Installation";      //IP - 24/02/11 - #3130
        public const string ReadyAssist = "ReadyAssist";        //#18604 - CR15594
        public const string AssemblyCost = "AssemblyCost";
        public const string AnnualService = "AnnualService";
        public const string GenericService = "GenericService";
    }
}

namespace STL.Common.Constants.EOD
{
    public struct EODResult
    {
        public const string Pass = "P";
        public const string Fail = "F";
        public const string Warning = "W";
    }

    public struct CountType1
    {
        public const string Processed = "PROCESSED";
        public const string AccountsBF = "ACCOUNTS B/F";
        public const string BalanceBF = "BALANCE B/F";
        public const string AccountsCF = "ACCOUNTS C/F";
        public const string BalanceCF = "BALANCE C/F";
        public const string Delivery = "DELIVERY";
        public const string Receipt = "RECEIPT";
        public const string Adjustment = "ADJUSTMENT";
        public const string NewAccount = "NEWACCT";
        public const string ReOpened = "REOPENED";
        public const string Settled = "SETTLED";
        public const string InterestOnSettled = "INTONSETT";
        public const string InterestOnUnsettled = "INTONUNSETT";
        public const string SettledWithInterest = "SETTWITHINT";
        public const string TotalUnsettledInterest = "TOTUNINT";
    }

    public struct CountType2
    {
        public const string Cosacs = "COSACS";
        public const string Coaster = "COASTER";
        public const string Kit = "KIT";
    }
}

namespace STL.Common.Constants.Elements
{
    public struct Elements
    {
        public const string Account = "Account";
        public const string Purchase = "Purchase";
        public const string Agreement = "Agreement";
        public const string InstalPlan = "InstalPlan";
        public const string Items = "Items";
        public const string Item = "Item";
        public const string RelatedItem = "RelatedItems";
        public const string ContractNos = "ContractNos";
        public const string ContractNo = "ContractNo";
        public const string Rules = "Rules";
        public const string Rule = "Rule";
        public const string Clause = "Clause";
        public const string LogicalOperator = "LO";
        public const string ComparisonOperator = "CO";
        public const string Operand1 = "O1";
        public const string Operand2 = "O2";
        public const string Update = "UPDATE";
        public const string CurrentVersion = "CURRENT_VERSION";
        public const string Installer = "INSTALLER";
        public const string DropDowns = "DropDowns";
        public const string DropDown = "DropDown";
        public const string DropDownParmList = "DropDownParmList";
        public const string DropDownParm = "DropDownParm";
        public const string Recent = "recent";
        public const string RecentAccounts = "recentAccounts";
        public const string RecentCustomers = "recentCustomers";
        public const string Customer = "Customer";
        public const string ProductDescription = "ProductDescription";
        public const string Reason = "Reason";
        public const string OneForOneTimePeriod = "OneForOneTimePeriod";
        public const string DateReturn = "DateReturn";
        public const string PrinterProperties = "PrinterProperties";
        public const string Printer = "Printer";
        public const string Tray = "Tray";
    }
}

namespace STL.Common.Constants.Delivery
{
    public struct DelType
    {
        public const string Normal = "D";
        public const string Return = "R";
        public const string Collection = "C";
    }

    public struct DelTrans
    {
        public const string Normal = "DEL";
        public const string Repossession = "RPO";
        public const string Redelivery = "RDL";
        public const string Collection = "GRT";
    }
}

namespace STL.Common.Constants.StandingOrder
{
    public struct SOstatus
    {
        public const string Error = "E";
        public const string Settled = "S";
        public const string ToSettle = "W";
        public const string DateError = "D";
        public const string WrittenOff = "B";   //IP - 16/08/10 - CR1092 - COASTER to CoSACS Enhancements
        public const string AccountNotExists = "A";   //IP - 16/08/10 - CR1092 - COASTER to CoSACS Enhancements
    }
}

namespace STL.Common.Constants.ExchangeRate
{
    public struct RateFormat
    {
        public const string DecimalPlaces = "C3";
    }

    public struct RateStatus
    {
        public const string Current = "C";
        public const string History = "H";
        public const string Edit = "E";
    }
}

namespace STL.Common.Constants.Tags
{
    //struct used for XML tag constants
    public struct Tags
    {
        public const string Item = "Item";
        public const string Key = "Key";
        public const string Type = "Type";
        public const string Code = "Code";
        public const string Description1 = "Description1";
        public const string Description2 = "Description2";
        public const string Location = "Location";
        public const string Quantity = "Quantity";
        public const string UnitPrice = "UnitPrice";
        public const string CostPrice = "CostPrice";
        public const string Value = "Value";
        public const string ValueControlled = "ValueControlled";
        public const string RelatedItems = "RelatedItems";
        public const string DamagedStock = "DamagedStock";
        public const string SupplierCode = "SupplierCode";
        public const string CashPrice = "CashPrice";
        public const string HPPrice = "HPPrice";
        public const string DutyFreePrice = "DutyFreePrice";
        public const string AvailableStock = "AvailableStock";
        public const string DeliveryDate = "DeliveryDate";
        public const string DeliveryTime = "DeliveryTime";
        public const string BranchForDeliveryNote = "BranchForDeliveryNote";
        public const string ColourTrim = "ColourTrim";
        public const string ContractNumber = "ContractNumber";
        public const string ParentItemNo = "ParentItemNo";
        public const string TaxRate = "TaxRate";
        public const string DeliveredQuantity = "DeliveredQuantity";
        public const string PlannedDeliveryDate = "PlannedDeliveryDate";
        public const string CanAddWarranty = "CanAddWarranty";
        public const string DateDelivered = "DateDelivered";
        public const string DeliveryAddress = "DeliveryAddress";
        public const string DeliveryArea = "DeliveryArea";
        public const string DeliveryProcess = "DeliveryProcess";
        public const string AgreementDate = "AgreementDate";
        public const string AgreementTotal = "AgreementTotal";
        public const string DateChanged = "DateChanged";
        public const string DateDue = "DateDue";
        public const string DateFirst = "DateFirst";
        public const string DateFrom = "Date From"; //CR36
        public const string DateTo = "Date To";   //CR36
        public const string DeferredTerms = "DeferredTerms";
        public const string Deposit = "Deposit";
        public const string EmpNoChanged = "EmpNoChanged";
        public const string FinalInstalment = "FinalInstalment";
        public const string InstalAmount = "InstalAmount";
        public const string InstalFreq = "InstalFreq";
        public const string Instalno = "Instalno";
        public const string PaymentMethod = "PaymentMethod";
        public const string PurchaseNumber = "PurchaseNumber";
        public const string SOA = "SOA";
        public const string TermsType = "TermsType";
        public const string InstalTotal = "InstalTotal";
        public const string Employee = "Employee";
        public const string SalesPerson = "SalesPerson";
        public const string CODFlag = "CODFlag";
        public const string DelToFact = "DelToFact";
        public const string Operand = "Operand";
        public const string Operator = "Operator";
        public const string Result = "Result";
        public const string ClauseType = "ClauseType";
        public const string State = "State";
        public const string RuleName = "RuleName";
        public const string Country = "Country";
        public const string ApplyRF = "ApplyRF";
        public const string ApplyHP = "ApplyHP";
        public const string TableName = "TableName";
        public const string Name = "Name";
        public const string AccountNumber = "accountNumber";
        public const string CustomerID = "customerID";
        public const string ReferDeclined = "ReferDeclined";
        public const string ReferAccepted = "ReferAccepted";
        public const string DeclineScore = "DeclineScore";
        public const string ReferScore = "ReferScore";
        public const string QuantityDiff = "QuantityDiff";
        public const string ScheduledQuantity = "ScheduledQuantity";
        public const string RuleRejects = "RuleRejects";
        public const string TaxAmount = "TaxAmount";
        public const string ReturnItemNo = "ReturnItemNo";
        public const string ReturnLocation = "ReturnLocation";
        public const string FreeGift = "FreeGift";
        public const string BureauMinimum = "BureauMinimum";
        public const string BureauMaximum = "BureauMaximum";
        public const string ReferToBureau = "ReferToBureau";
        public const string AddTo = "AddTo";
        public const string Region = "Region";
        public const string ExpectedReturnDate = "ExpectedReturnDate";
        public const string NewImport = "NewImport";
        public const string FileName = "FileName";
        public const string QtyOnOrder = "QtyOnOrder";
        public const string PurchaseOrder = "PurchaseOrder";
        public const string LeadTime = "LeadTime";
        public const string Assembly = "Assembly";
        public const string Damaged = "Damaged";
        public const string ProductCategory = "ProductCategory";
        public const string Deleted = "Deleted";
        public const string PurchaseOrderNumber = "PurchaseOrderNumber";
        public const string Percentage = "Percentage";  //CR36
        public const string ReplacementItem = "ReplacementItem";  //CR36
        public const string Category = "Category";
        public const string SPIFFItem = "SPIFFItem";
        public const string IsInsurance = "IsInsurance"; //CR1005
        public const string SparePartsCategory = "SparePartsCategory";
        public const string SortOrder = "SortOrder";
        public const string RefCode = "RefCode"; //IP - 28/01/10 - LW 72136
        //public const string ScoreType = "ScoreType"; //SC CR1034 Behavioural Scoring 15/02/2010 //IP - 08/04/10 - CR1034 - Removed
        public const string DhlInterfaceDate = "DhlInterfaceDate"; //jec Malaysia 3PL 05/03/2010
        public const string DhlPickingDate = "DhlPickingDate"; //jec Malaysia 3PL 05/03/2010
        public const string DhlDNNo = "DhlDNNo"; //jec Malaysia 3PL 05/03/2010
        public const string ItemRejected = "ItemRejected"; //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
        public const string ScoreType = "ScoreType"; //CR 1034
        public const string ModelNumber = "ModelNumber";
        public const string ColourName = "ColourName";      //CR1212 jec 21/04/11
        public const string Style = "Style";        //CR1212 jec 21/04/11
        public const string ItemId = "ItemId";        //CR1212 jec 21/04/11
        public const string ParentItemId = "ParentItemId";
        public const string SalesBrnNo = "SalesBrnNo";      //IP - 23/05/11 - CR1212 - RI - #3651
        public const string RepoItem = "RepoItem";      // RI 
        public const string Class = "Class";            // IP - 27/07/11 - RI - #4415
        public const string SubClass = "SubClass";      // IP - 27/07/11 - RI - #4415
        public const string Brand = "Brand";            // IP - 19/09/11 - RI - #8218 - CR8201 
        public const string Express = "Express";        // IP - 06/06/12 - #10229
        public const string InterceptScore = "InterceptScore";  //IP - 4/10/12 - #11405 - CR11404
        public const string LineItemId = "LineItemId";      //#13716 - CR12949
        public const string ReadyAssist = "ReadyAssist";    //#13716 - CR12949
        public const string WarrantyType = "WarrantyType";  //#17883            //#15888
        public const string AdditionalTaxRates = "AdditionalTaxRates"; //BCX : This is used for LUX tax for curacao
        public const string Description = "Description";
        public const string Amount = "Amount";
        public const string IsAmortized = "IsAmortized";
    }
}

namespace STL.Common.Constants.Enums
{
    public enum Return
    {
        Success,
        Fail,
        DuplicateAccount,
        NotFound,
        Deadlock
    }
    public enum DropDown
    {
        TermsType,
        SourceOfAttraction,
        AllStaff,
        SalesStaff,
        MethodOfPayment,
        AccountType,
        StockLocations,
        BranchNumbers,
        CustomerCodes,
        AccountCodes,
        UserTypes,
        UserFunctions,
        CustomerTitles,
        CustomerRelationship,
        AddressType,
        IDSelection,
        MaritalStatus,
        EthnicGroup,
        Nationality,
        PropertyType,
        ResidentialStatus,
        EmploymentStatus,
        Occupation,
        PayFrequency,
        Banks,
        BankAccountType,
        ApplicationType,
        DDDueDate,
        ReferralCode,
        Countries,
        ProductCategory
    }
    //public enum OneForOneTimePeriod
    //{
    //	LessThanThirtyDays,
    //	LessThanSixMonths,
    //	LessThanTwoYears,
    //	GreaterThanTwoYears,
    //	WarrantyExpired
    //}
    public enum OneForOneTimePeriod
    {
        // The time periods are consecutive, but the length of
        // each period is dynamic and set by country parameters.
        IRPeriod1,
        IRPeriod2,
        IRPeriod3,
        IRPeriod4,
        // The length of a warranty is determined by the warranty
        // and may not be related to the periods above
        // (i.e. it could expire before the end of all the periods.)
        WarrantyExpired
    }

}

namespace STL.Common.Constants.Values
{
    public struct VL
    {
        public const int Plus200 = 200;
        public const int Minus200 = -200;
        public const int MaxInstallmentTerms = 60;
    }

    public static class Address
    {
        public const string Address1 = "Address1";
        public const string Address2 = "Address2";
        public const string Address3 = "Address3";
        public const string PostCode = "PostCode";
        public const string AddressType = "AddressType";
        public const string Home = "H";
        public const string Work = "W";
        public const string Delivery = "D";
    }
}

//IP - 01/10/08 - Special Arrangements (Credit Collections)
//Struct to be passed in as a parameter to the method
//that retrieves the account details and displays them on the 'Special Arrangements' screen.
namespace STL.Common.Structs
{
    public struct SPAAccountDetails
    {
        public decimal Outstbal;
        public decimal Arrears;
        public decimal Instalamount;
        public DateTime DateAcctOpen;       //CR976
        public int PercentPaid;             //CR976
        public DateTime FinalPayDate;       //CR976
        public decimal Interest;            //CR976     
        public decimal Admin;               //CR976
        public string AcctType;             //CR976
        public string Period;               //CR976
        public int NumInstal;               //CR976
        public decimal OddPayment;          //CR976
        public DateTime FirstPayDate;       //CR976
        public decimal ArrangementAmount;   //CR976
        public string AccountNo;
        public int Term;                    //CR976
        public int MaxTerm;                 //CR976
        public int CurrInstNo;              //CR976
        public string TermsType;               //CR976
        public decimal RefinDeposit;        //CR976
        public decimal ServPcent;        //CR976
        public decimal CashPrice;        //CR976
        public int DueDay;        //CR976
        public decimal InsPcent;           //IP - 29/04/10 - UAT(983) UAT5.2
        public decimal AdminPcent;         //IP - 29/04/10 - UAT(983) UAT5.2
        public decimal Rebate;             //IP - 30/04/10 - UAT(983) UAT5.2
        public int MinTerm;                //IP - 22/09/10 - UAT(1017)UAT5.2
        public string ScoringBand;         //IP - 23/09/10 - UAT(1017)UAT5.2
    }
}
namespace STL.Common.Static
{
    public class Messages
    {
        private static readonly Hashtable _list;
        public static Hashtable List
        {
            get
            {
                //if (_list == null)
                //    CreateHashTable();
                return _list;
            }
            //set
            //{
            //    if (_list == null)
            //        CreateHashTable();
            //    _list = value;
            //}
        }

        static Messages()
        {
            _list = CreateHashTable();
        }

        private static Hashtable CreateHashTable()
        {
            var _list = new Hashtable();

            // Misc that someone could not be arsed to put into a category
            #region Misc
            _list.Add("AccountNo", "Account No");
            _list.Add("Address1", "Address 1");
            _list.Add("Address2", "Address 2");
            _list.Add("EnterName", "Enter a Name");
            _list.Add("From", "From");
            _list.Add("NoSetSpecified", "No Set Selected");
            _list.Add("AllValues", "All");
            _list.Add("AnyValue", "Any");
            _list.Add("Name", "Name");
            _list.Add("Postcode", "Postcode");
            _list.Add("TestString", "This is an English Message");
            _list.Add("Main", "Main");
            _list.Add("Maintenance", " Maintenance");
            _list.Add("Yes", "Yes");
            _list.Add("No", "No");
            _list.Add("Delivery", "Delivery");
            _list.Add("Collection", "Collection");
            _list.Add("Redelivery", "Redelivery");
            _list.Add("GrandTotal", "Grand Total --->");
            _list.Add("BranchTotal", "Branch Total --->");
            _list.Add("LOYALTY_MEMDIGITCHECK", "The membership number entered is not a valid number. Please check the number and enter again."); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("LOYALTY_DATESTARTEARLY", "The membership start date entered is too early."); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("LOYALTY_MEMBERCHECK", "No membership type selected!"); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("DOBCUSTOMER", "Note: Customer's age out of range for credit."); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("Once", "Once");
            _list.Add("Daily", "Daily");
            _list.Add("Weekly", "Weekly");
            _list.Add("Monthly", "Monthly");
            _list.Add("Country", "Country");      // CR931 jec 04/04/08
            _list.Add("AllBranches", "All Branches");        // CR931 jec 04/04/08
            _list.Add("Not Scheduled", "Not Scheduled"); //IP - 04/03/08 - Livewire (69582)

            #endregion
            //
            // Labels
            //
            #region Labels
            _list.Add("L_HOME", "H");
            _list.Add("L_WORK", "W");
            _list.Add("L_MOBILE", "M");
            _list.Add("L_MOBILE2", "M2");
            _list.Add("L_MOBILE3", "M3");
            _list.Add("L_MOBILE4", "M4");

            _list.Add("L_DMOBILE", "DM"); // Address Standardization CR2019 - 025
            _list.Add("L_D1MOBILE", "D1M"); // Address Standardization CR2019 - 025
            _list.Add("L_D2MOBILE", "D2M"); // Address Standardization CR2019 - 025
            _list.Add("L_D3MOBILE", "D3M"); // Address Standardization CR2019 - 025

            //IP - 16/03/11 - #3317 - CR1245 - labels for additional work numbers
            _list.Add("L_WORK2", "W2");
            _list.Add("L_WORK3", "W3");
            _list.Add("L_WORK4", "W4");

            _list.Add("L_DEL", "D");
            _list.Add("L_ALL", "ALL");
            _list.Add("L_STAFF", "Staff Members");

            _list.Add("L_SELECTA", "Select a ");
            _list.Add("L_SELECTSET", "Select an Existing Set");
            _list.Add("L_REQDELDATE", "Required Delivery Date Between");

            _list.Add("L_PASS", "PASS");
            _list.Add("L_WARNING", "PASS(W)");
            _list.Add("L_FAIL", "FAIL");
            _list.Add("L_SERVICEREQUEST", "SR No");         //UAT1021 jec
            #endregion
            //
            // Messages
            //
            #region Messages
            _list.Add("M_ABOUTEXTRAPAYMENTS", "Giro Extra Payments\n\nThis screen will list all giro accounts with an active mandate that\nare in arrears. When the user has confirmed consent with the customer, the\nuser will tick the relevant row in the tablefield. The user will be able to\nenter a payment amount against a row that has been ticked. All rows that\nhave been ticked will be included in the next payment file to be submitted\nto the bank. Until the next payment file is created the user will be able\nto amend the rows to change the amounts and to add or remove the ticks.\nOnce a payment file has been created, the ticked rows will be removed from\nthe list so that they cannot be amended after submission.\n\nThe user will only be able to enter a payment amount on a row that has been\nticked. If a tick is removed then the frame will clear this amount.\n\nThe initial population of the tablefield will set the 'Giro Pending' column\naccording to the stored data. When the 'Extra Payment' column is changed,\nthe frame will update the corresponding 'Giro Pending' amount.\n");
            _list.Add("M_ABOUTREJECTIONS", "Giro Rejections\n\nThis screen will list all giro accounts with a rejected payment.\nFor each account you may choose to not re-present; re-present or\ncancel the mandate. Each payment selected to re-present will be included\nin the next re-presentation file to be submitted to the bank. Mandates\nto be cancelled will be cancelled by the next Giro Housekeeping EOD run.\nUntil then you will be able to amend the rejection option.\n\nOnce an EOD run has completed and a re-presentation file has been created,\nthe cancelled and re-presentation rows will no longer appear in the list.");
            _list.Add("M_ACCOUNTLOCKED", "The account {0} for customer {1} is already locked and cannot be selected");
            _list.Add("M_ACCOUNTNAMEVALIDATION", "A name cannot contain a comma.");
            _list.Add("M_ACCOUNTNOFAILED", "Unable to generate new account number");
            _list.Add("M_ACCOUNTNOTSCORED", "Country Parameter 'TermsTypeBandDefault' must be set or credit scoring must be done before adding items");
            _list.Add("M_ACCOUNTSETTLED", "Account settled.  You cannot cancel a settled account");
            _list.Add("M_ACCOUNTSLISTED", " Account(s) listed.");
            _list.Add("M_ACCOUNTSZERO", "0 Accounts listed.");
            _list.Add("M_ACTIONSHEET", "Action sheet");
            _list.Add("M_ADDCUSTDETAILS", "You must add customer details.\\Would you like to go to the Customer Search screen?");
            _list.Add("M_ADDPAPER", "Add paper to the receipt printer");
            _list.Add("M_ADDNEWPAYMENTCARD", "Please add a new payment card and click OK to continue printing.");
            _list.Add("M_ADDRESSBLANK1", "AWAITING CUSTOMER");
            _list.Add("M_ADDRESSBLANK2", "ADDRESS DETAILS");
            _list.Add("M_ADDTODEPOSIT", "\n\nThe Deposit has been set to {0} which is the minimum deposit allowed for the new Cash Price.");
            _list.Add("M_ADDTORESULT", "The Outstanding Balance on existing accounts will be consolidated. This will increase the Cash Price of account number {0} to {1}, the Agreement Total to {2} and the new Monthly Instalment will be {3} given {4} instalments payable.");
            _list.Add("M_ADDTORESULTREFER", "The Outstanding Balance on existing accounts will be consolidated. This will increase the Cash Price of account number {0} to {1}, the Agreement Total to {2} and the new Monthly Instalment will be {3} given {4} instalments payable.\n\nHowever the new Agreement EXCEEDS the RF Credit Limit of {5} by {6}. You have the option to REFER this account.");
            _list.Add("M_ADDTORESULTREFERINFO", "An Add-To requested onto account {0} requires the current RF Credit Limit of {1} to be increased by {2} to a new limit of at least {3}.");
            _list.Add("M_ADDWARRANTY", "Please enter a contract number for each warranty you require (max [{0}])");
            _list.Add("M_AFFINITYMAXTERM", "The maximum Agreement Term for Affinity items is {0} months");
            _list.Add("M_AGREEMENT", "Agreement");
            _list.Add("M_AGREEMENTSUMMARY", "Balance payable by {0} instalments of {1} and a final instalment of {2}");
            _list.Add("M_AGREEMENTSUMMARYVARIABLE", "Balance payable by {0} instalments of {1}");
            _list.Add("M_AGREEMENTSUMMARYVARIABLEFOLLOW", "followed by {0} instalments of {1}");
            _list.Add("M_AGREEMENTSUMMARYVARIABLEFINAL", "and a final instalment of {0}");
            _list.Add("M_AGREEMENTSUMMARYGUYANA", "The balance to be paid by monthly rents of {0} payable on the,      ,day of each         commencing on the      day of                 payable at the owners place of business");
            _list.Add("M_ALLBRANCHES", "All Branches");
            _list.Add("M_ALLOCATIONINFUTURE", "Date Allocated in Future -not allowed");
            _list.Add("M_ALIGNDELDATE", "Item out of stock - delivery date moved \\Do you wish to align required delivery date of other ordered items to this item?");
            _list.Add("M_ALREADYLINKED", "Account already linked to this customer.");
            _list.Add("M_ALREADYLOGGEDIN", "User {0} is already logged on. \\This may result in unpredictable behaviour. \\Proceed?");
            _list.Add("M_ALREADYTRANSFERRED", "This transaction has already been transferred. Please select a different transaction to transfer");                 //IP - 21/02/12 - #9633 - CR1234
            _list.Add("M_AMOUNTDUE", "Amount Due");
            _list.Add("M_ANNUALSERVICECONTRACTDEBIT", "Customers account will be debited {0} for the used portion of the Annual Service Contract");
            _list.Add("M_ARRANGEMENTAMOUNT", "The arrangment amounts entered must total the total arrangement instalment amount"); //IP & JC - 12/01/09 - CR976
            _list.Add("M_APPLICANTTOOYOUNG", "Applicant's age is below the minimum required for credit application.");
            _list.Add("M_AREYOUSURE", "Quantity greater than 10, Are you sure?");
            _list.Add("M_ASSOCIATEDWARRANTY", "Note: Item {0} has the following associated warranties: \\\\  {1}");
            _list.Add("M_ASSOCIATEDDISCOUNT", "Note: Item {0} has the following associated discounts: \\\\  {1} \\\\  These will also be collected.");
            _list.Add("M_BALANCECHECK", "Balance must be 0 before settling");
            _list.Add("M_BALANCEFROMPREV", "Balance from previous account");
            _list.Add("M_BALANCEGRTMILLION", "Balance/value > 1 million cr/dr");
            _list.Add("M_BANDORLOYALTYREQUIRED", "Scoring Band or Loyalty Club must be entered");
            _list.Add("M_BANDREQUIRED", "Scoring Band must be entered");
            _list.Add("M_BANKDEPOSITMETHOD", "The bank deposit must be positive for this pay method");
            _list.Add("M_BANKDEPOSITSETUP", "There are currently no bank deposits setup. Please setup bank deposits to continue."); //IP - 05/05/09 - Livewire (71112)
            _list.Add("M_BANKDETAILS", "Bank Details");
            _list.Add("M_BANKDETAILSGIRO", "Bank Details - Use Giro Mandate screen to edit mandate fields");
            _list.Add("M_BDWINVALIDSCREEN", "You are attempting to use a written off account in the wrong context."); //IP - 22/04/08 - UAT364(v.5.1)
            _list.Add("M_BRANCHBUFF", "Please enter both a Branch Number and a Buff Number");
            _list.Add("M_BRANCHDETAILSSAVED", "Branch details have been saved");   //CR903 jec
            _list.Add("M_BOOKINGSLISTED", " Booking(s) listed.");
            _list.Add("M_BOOKINGSZERO", "0 Bookings listed.");
            _list.Add("M_BULKPRINTRECEIPTS", "You have chosen to print {0} invoices. Do you wish to proceed?");      //IP - 11/05/12 - #9609 - CR8520
            _list.Add("M_CANCELAUTH", "Authorisation is needed to cancel a warranty after the cancellation period.");
            _list.Add("M_CANCELLED", "You have cancelled without saving data.  Default print settings will be used");
            _list.Add("M_CANCELSCHEDULE", "Outstanding schedules must be deleted before account can be cancelled.");
            _list.Add("M_CANCELSUCCESSFUL", "Account has been successfully cancelled.");
            _list.Add("M_CANCELSUCCESSFUL2", "{0} Account(s) successfully cancelled.");
            _list.Add("M_CANCELUNSUCCESS", "Account(s) NOT cancelled."); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_CANNOTCANCELSTORECARD", "This Store Card has a balance of {0} and therefore cannot be cancelled"); //IP - 13/04/12 - #9920 - UAT (150)
            _list.Add("M_CANNOTADDENTRYCONDITION", "This condition cannot be added as it is currently used as a Step/Exit condition"); //IP - 02/12/09 - UAT5.2 (929)
            _list.Add("M_CANNOTADDSTEPCONDITION", "This condition cannot be added as it is currently used as a Entry/Exit condition"); //IP - 02/12/09 - UAT5.2 (929)
            _list.Add("M_CANNOTADDEXITCONDITION", "This condition cannot be added as it is currently used as a Entry/Step condition"); //IP - 02/12/09 - UAT5.2 (929)
            _list.Add("M_CANNOTDEACTIVATE", "There are {0} strategies connected to this strategy. These are {1}. Until they have all been removed from this strategy you cannot deactivate this strategy."); //IP - UAT(520)
            _list.Add("M_CANNOTREMOVEADDTO", "Add To items can only be removed by reversing the add to");
            _list.Add("M_CANNOTREMOVEITEM", " Cannot reduce/remove,item has been delivery authorised");     //IP - 28/05/12 - #9877 - Warehouse & Deliveries Integration
            _list.Add("M_CANNOTREMOVEWARRANTY", "Warranty items must be collected before they can be removed");
            _list.Add("M_CANNOTENTERPAIDTAKEN", "Please enter a courts account which is not a Paid and Taken or Special account."); //IP - 31/07/09 - UAT(741)
            _list.Add("M_CANNOTENTERSPECIAL", "Please enter a courts account which is not a Special account.");
            _list.Add("M_CANTADDITEMS", "You cannot add items in the collection screen.");
            _list.Add("M_CANTADDWARRANTY", "Warranties cannot be added to this item.");
            _list.Add("M_CANTCHANGEKIT", "Cannot change composition of a kit. Whole kit will be removed.\\Proceed?");
            _list.Add("M_CANTFINDADDEDTO", "Unable to perform reversal. \\Cannot determine account added to.");
            _list.Add("M_CANTINCREASEQTY", "You cannot increase the quantity when returning an item.");
            _list.Add("M_CANTOPENCALC", "Unable to find/open calc.exe");
            _list.Add("M_CANTSAVEACCOUNT", "Unable to save account. Close screen anyway?");
            _list.Add("M_CANTSAVECUST", "Unable to save customer. Close screen anyway?");
            _list.Add("M_CANTTRANSFERTOSAMEACCT", "You cannot transfer to the same account.");
            _list.Add("M_CANTTRANSFERTOOVERAGE", "You cannot transfer the entered amount as this will result in the Overage account being in debit");                     //IP - 15/02/12 - #8819 - CR1234
            _list.Add("M_CANTTRANSFERTOSHORTAGE", "You cannot transfer the entered amount as this will result in the Shortage account being in credit");                   //IP - 15/02/12 - #8819 - CR1234
            _list.Add("M_CARDPRINT", "For the type of Customer Card chosen the row number must be from 1 to {0}");
            _list.Add("M_CASHBACKPERCTOOHIGH", "The cash back percentage specified must not be more than {0}%");
            _list.Add("M_CASHIERDEPOSITMISSINGINFO", "Please enter a unique lodgement number for each banking transaction");
            _list.Add("M_CASHIERMATCH", "Cashier Totals match Cashier Deposits");
            //_list.Add("M_CASHIERNOTMATCH", "Cashier Totals do not match Cashier Deposits");
            _list.Add("M_CASHIERNOTMATCH", "Cashier Totals do not match cashier deposits for the following payment method(s): \\\\{0}");
            _list.Add("M_CASHIERTOTALAUTH", "Authorisation is required to save unreconciled cashier totals. Please enter the appropriate credentials.");
            _list.Add("M_CASHIERTOTALSEMPTY", "There are no Cashier Totals to save");
            _list.Add("M_CASHIERTOTALSMATCH", "Totals match entered amount");
            _list.Add("M_CASHIERTOTALSPAYMETHOD", "Your total does not match for the following pay method(s) \\\\{0}");
            _list.Add("M_CASHIERSLISTED", " Cashier(s) listed.");
            _list.Add("M_CASHIERSZERO", "0 Cashiers listed.");
            _list.Add("M_CASHTILLAUTH", "Authorisation is required to open the cash till. Please enter the appropriate credentials.You will then be prompted to enter a reason.");
            _list.Add("M_CASHLOANADMINCHARGE", "Authorisation is required to change / waive the Admin Charge");
            _list.Add("M_CASHLOANAMOUNTCHANGED", "Authorisation is required to change the Loan Amount");
            _list.Add("M_CHANGECHARGETO", "Authorisation is required to change the Primary Charge To. Please enter the appropriate credentials.");
            _list.Add("M_CHANGELABOURCOST", "Authorisation is required to change/enter the Labour Cost. Please enter the appropriate credentials."); // CR 949/958
            _list.Add("M_CHANGEESTIMATES", "Authorisation is required to change the Estimate Costs. Please enter the appropriate credentials.");
            _list.Add("M_CHANGERESOLUTION", "Authorisation is required to change the type of Resolution. Please enter the appropriate credentials.");
            _list.Add("M_CHEQUECLEARANCE", "The item\\\\    {0} : {1} \\\\has a delivery date earlier than the cheque clearance date of {2}.\\\\Do you wish to continue?");
            _list.Add("M_CHQCLEARED", "Cheque not cleared - are you sure ?");
            _list.Add("M_CHQCLEARED2", "Cheque not cleared - unable to authorise");
            _list.Add("M_CHRONODATES", "The {0} Date must be on or before the {1} Date");
            _list.Add("M_CLOSEALLWINDOWS", "You must close all open windows\nbefore logging off to avoid loss of  data.");
            _list.Add("M_CLOSEALLWINDOWSEOD", "You must close all open windows\nbefore closing CoSACS.");    //IP - 20/09/10 - UAT5.2 Log - UAT(1004)
            _list.Add("M_COLLECTION", "COLLECTION: ");
            _list.Add("M_COLLECTIONNOTE", "Collection Note");
            _list.Add("M_COLLECTIONNOTEUPPERCASE", "COLLECTION NOTE");   //UAT 198 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_OPTIONALREADYSELECTED", "This option has already been selected for a different field. Please selected another.");   //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            _list.Add("M_COMMCONNECTFAIL", "Unable to connect to receipt printer");
            _list.Add("M_COMMENTSFORCUSTOMER", "Comments for Customer");
            _list.Add("M_COMMISSIONLASTRUN", "Commissions have been run in the last {0} days. Frequently re-running commissions can lead to higher amounts being paid for some categories\\\\\\\\Press OK to start the EOD process, or CANCEL to abort it.");
            _list.Add("M_COMMISSIONNEGATIVE", "Attempted to save Commission as a Credit");
            _list.Add("M_COMMISSIONRATEEXCEEDED", "Commision % rate exceeds maximum allowed of {0}%");  //CR36 jec 05/10/06
            _list.Add("M_COMMISSIONVALUEEXCEEDED", "Commision value exceeds maximum allowed of {0}");  //CR36 jec 23/11/06
            _list.Add("M_COMMISSIONEXISTS", "A commission rate for this item already exists for this date");      //CR36
            _list.Add("M_COMPONENTEXISTS", "Component with key: {0}|{1} already exists on this account.");
            _list.Add("M_CONFIGERROR", "Unable to read config file. \\Exiting application.");
            _list.Add("M_CONFIGURATION", "This is a new installation so you must complete configuration information");
            _list.Add("M_CONFIGURESETTINGS", "Please configure all settings before saving");
            //68458 (jec 23/08/06)  _list.Add("M_CONFIRMPAYMENT", "Confirm customer will be paying the used portion of the warranty?");
            _list.Add("M_CONFIRMPAYMENT", "The contract is over 30 days old. Only a part of the premium will be credited to the customers account."); //68458
            _list.Add("M_CONFIRMPAYMENTUNDER30", "Only a part of the premium will be credited to the customers account."); //UAT 349 JH 30/10/2007
            _list.Add("M_CONFIRMSCHEDREMOVAL", "Are you sure you do not want to add new items to the schedule?");
            _list.Add("M_CONFIRMSALECLOSEWIHOUTSAVE", "Are you sure you want to close this sale without saving?");
            _list.Add("M_CONFIRMWARRANTY", "This warranty belongs to product {0}.  \\Please confirm you wish to collect the warranty only, without the parent item?");
            _list.Add("M_COPYREFERENCES", "You may select {0} more references to copy");
            _list.Add("M_CORRECTIONAUTH", "Authorisation required to process a correction on a payment that is more than one day old.");
            _list.Add("M_CREATENEWSR", "Are you sure you want to create a new Service Request against this item?"); //CR802 Jez Hemans 02/02/2007
            _list.Add("M_CREDITBLOCKED", "Unable to create RF account. \\Customer's credit is blocked.");
            _list.Add("M_CUSTCREDBLOCKED", "Customer's credit is blocked. To Extend Term please unblock credit");         //IP - 28/04/10 - UAT(983) UAT5.2
            _list.Add("M_CREDITFEE", "Authorisation is required to set the credit fee to zero. Please enter the appropriate credentials.");
            _list.Add("M_ManualCard", "Authorisation is required to be able to enter the manual Card");
            _list.Add("M_CREDITUNBLOCKED", "Customer's credit has been unblocked.");
            _list.Add("M_CREDREPOSS", "Credits from repossessions cannot exceed goods Delivered/Repossed/Agreement total");
            _list.Add("M_CULTURERESTART", "You must restart the application for new language settings to take effect");
            _list.Add("M_CUSTIDEXISTS", "New Customer ID already exists");
            _list.Add("M_CUSTNOACCOUNTS", "Customer has no accounts to sanction");
            _list.Add("M_CUSTOMERDATACHANGED", "The highlighted customer data fields have changed. Do you wish to update these fields to their current values? \n\nIf you choose not to do so now the next time you load this service request you will be promted again.");
            _list.Add("M_CUSTOMERSEARCH", "You will now be directed to the customer\\search screen to select a credit sanctioned customer");
            _list.Add("M_CUSTOMERBLACKLISTED", "Unable to create account. \\Please contact credit department."); //jec 07/07/10 UAT1066 
            _list.Add("M_CUSTOMERLEFTADDR", "You are receiving this pop up because this customer has a 'Left Address' active action on their account. \n\n Ask the customer to go to the Credit Department to update their address details before completing the transaction."); //jec 06/09/10 UAT1084 //IP - 12/10/10 - UAT5.4 - UAT(55)
            _list.Add("M_CUSTOMEROTHERARREARS", "You are receiving this pop up because this customer has other accounts in arrears. \n\n Kindly remind the customer of this the click OK to continue with the transaction."); //jec 06/09/10 UAT1084 
            _list.Add("M_CUSTOMERADDITINFO", "You are receiving this pop up because this customer has a 'Additional Information' active action on their account. \n\n Clicking OK will direct you to the Customer Details screen to update the outstanding fields before continuing with the transaction."); //jec 06/09/10 UAT1084
            _list.Add("M_CUSTOMERTELNOTINSERVICE", "You are receiving this pop up because this customer has a 'Telephone out of service/Invalid number' active action on their account. \n\n Clicking OK will direct you to the Customer Details screen to update the telephone number fields before continuing with the transaction."); //jec 06/09/10 UAT1084 
            _list.Add("M_CUSTOMERWITHNOSCORINGBAND", "There is no scoring band calculated for customer. \n\n Please configure scoring band for customer.");
            _list.Add("M_DATEDUEAUTH", "Authorisation is required when setting date due to be 2 months after delivery. Please enter the appropriate credentials.");
            _list.Add("M_DATEDUEINVALID", "Date due cannot be before delivery date.");
            _list.Add("M_DATEMUSTBEFUTURE", "Date must be in the future");      //CR36
            _list.Add("M_DATETOLATER", "Date To must be later than Date From"); //CR36
            _list.Add("M_DATEOUTOFRANGE", "Dates must fall into the overall date range selected in the overview section.");
            _list.Add("M_DATEOVERLAP", "Date ranges must not overlap");
            _list.Add("M_DDFILEEXISTS", "A new {0} Payments file was not generated because the file {1} already exists");
            _list.Add("M_DDFILENOTCREATED", "There were no due {0} Payments found, so the {0} Payments file {1} was not created");
            _list.Add("M_DDPAYMENTSDISABLED", "Payments Processing is currently disabled. The next available run date is between {0} and {1} for the due date of {2}");
            _list.Add("M_DDREJECTIONSDISABLED", "The current date is too close to a due date to run Rejections Processing. Rejections Processing has been disabled from {0} to {1}. The next available run date is {2}");
            _list.Add("M_DDREJECTIONSNOTEXISTS", "No rejections file found at {0}");
            _list.Add("M_DDREJECTIONSPROCESSED", "The new Rejections file {0} was not processed because the processed Rejections file {1} was found. CHECK THESE FILES ARE NOT DUPLICATES");
            _list.Add("M_DECIMALNOTVALID", "Decimal quantities are not allowed for this product");  //68500 jec 12/09/06
            _list.Add("M_DECIMALINVALID", "Decimals are not allowed for this commission rate");  //CR36 enhancement jec 21/06/07
            _list.Add("M_DELETECONFIRM", "Are you sure you want to delete this record?");
            _list.Add("M_DELETEDITEM", "Authorisation is required to sell deleted products.");
            _list.Add("M_DELETEDITEMRETURN", "Authorisation is required to return deleted products."); //69647
            _list.Add("M_DELETERULE", "Are you sure you want to delete this Rule?");            //IP - 11/06/10 - CR1083 - Collection Commissions
            _list.Add("M_DELETESET", "Are you sure you want to delete this Set?");
            _list.Add("M_DELETESMS", "Are you sure you want to delete this SMS?");
            _list.Add("M_DELETEWARRANTIES", "Do you want to remove attached warranty/warranties also?");
            _list.Add("M_DELETEWORKLIST", "Are you sure you want to delete this Work List?");
            _list.Add("M_DELETEEXISTINGSTRATEGY", "Are you sure you want to delete this Strategy?"); //IP - UAT(514)
            _list.Add("M_DELEXCEEDSAGREEMENT", "Delivery amount for account {0} \\exceeds agreement total - delivery not processed.");
            _list.Add("M_DELEXCEEDSAGREEMENTACCTS", "Delivery amount for the following accounts: {0} \\exceeds agreement total."); //IP - 04/02/08 Livewire: (69461)
            _list.Add("M_DELIVERLINEITEMS", "Delivering line items ...");
            _list.Add("M_DELIVERYAREADAY", "Day of delivery is not in this Delivery Area");
            _list.Add("M_DELIVERYAREAINFO", "The day of the Required Delivery Date {0} is not delivered by Delivery Area {1}.\n\nYou should set the Required Delivery Date to one of the days listed below.");
            _list.Add("M_DELIVERYNOTE", "Delivery Note");
            _list.Add("M_DELIVERYNOTEUPPERCASE", "DELIVERY NOTE");   //UAT 198 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_DELIVERYNOTECANCEL", "Supersedes previous number {0} printed on {1} now cancelled.");
            _list.Add("M_DELIVERIESLISTED", " Deliveries listed.");
            _list.Add("M_DELIVERIESZERO", "0 Deliveries listed.");
            _list.Add("M_DELIVERSCHEDULE", "This will deliver all remaining items on this schedule\n\nAre you sure?");
            _list.Add("M_DEPOSITAMOUNTPAID", "The amount {0} has already been paid on account {1} that covers the required deposit amount of {2}");
            _list.Add("M_DEPOSITMUSTBEPOSITIVE", "A Deposit must be positive");
            _list.Add("M_DEPOSITPAID", "Deposit Paid");
            _list.Add("M_DEPOSITPCENTRANGE", "Deposit percentage must be between 0 and 100%");  //  (jec 67941)
            _list.Add("M_DEPOSITREQUIRED", "Deposit Required");
            //_list.Add("M_DEPOSITTOOHIGH", "Deposit/Disbursement value of {0} is greater than amount available for deposit of {1}");
            _list.Add("M_DEPOSITTOOHIGH", "Deposit/Disbursement value is greater than amount available for deposit."); //IP - (70635) - 28/01/09 - Changed so amount available for deposit not displayed in message.
            _list.Add("M_REFUNDEXCEEDSDEPOSIT", "Refund value is greater than amount available for deposit.");
            _list.Add("M_REFUNDEXCEEDSARREARS", "Refund value is greater than arrears.");
            _list.Add("M_DETAILSMISSING", "You must enter all details before printing agreement.");
            _list.Add("M_DIFFERENTALLOCATION", "The receipt {0} is allocated to employee {1},\\but the account is allocated to {2}.\\\\Are you sure you want to use this receipt?");
            _list.Add("M_DISCOUNTAUTH", "Authorisation is required to discount more than {0}% of an item.");
            _list.Add("M_DISCOUNTMONTHSPASSEDAUTH", "Authorisation is required for a discount that has been applied less than {0} months ago.");
            _list.Add("M_DISCOUNTAUTHNOTLINKED", "Authorisation is required when adding a discount not linked to an item.");
            _list.Add("M_DISCOUNTINVALID", "Please enter a valid Discount code.");
            _list.Add("M_DISCOUNTTOOHIGH", "Discount cannot exceed the value of the item being discounted.");
            _list.Add("M_DISCOUNTTYPE", "No items on account for discount of this type. \\Please choose a discount that matches.");
            _list.Add("M_DUPLICATECODE", "Code {0} is duplicated by code {1}. The same code might be entered with leading zeroes.");
            _list.Add("M_DUPLICATECUSTID", "This Customer ID already exists.");
            _list.Add("M_DUPLICATEROW", "Duplicate row");
            _list.Add("M_DUPLICATEPART", "Duplicate part entered");     // RI jec
            _list.Add("M_EDITWARRANTY", "You cannot directly edit a warranty item.");
            _list.Add("M_EMPORACCTREQUIRED", "You must enter either an employee or an account number");
            _list.Add("M_EMPREQUIRED", "You must enter an employee"); // UAT 18 New error message required for Bailiff screen in 5.2
            _list.Add("M_ENTERCHEQUNO", "You must enter either a cheque number or a bank account number.");
            _list.Add("M_PAYMENTFILEDEFNIDLENGTH", "The ID length does not match the length specified.");   //IP - 13/09/10 - CR1092 - COASTER to CoSACS Enhancements
            _list.Add("M_ENTERMANDATORY", "Enter mandatory field");
            _list.Add("M_ENTERMANDATORYNOTES", "Notes must be entered to save GRT"); //CR1048 jec //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_SELECTMANDATORY", "Select mandatory field");
            _list.Add("M_ENTERATLEASTONE", "Enter at least one of the labour or parts fields");  //UAT 386 At least one parts/labour field must have an entry
            _list.Add("M_ENTERREASON", "Please enter a reason for all differences.");
            _list.Add("M_CLOSEDDATEBEFORERESOLVED", "Please enter a closed that is later than resolved date");
            _list.Add("M_ENTERTRANSFERREASON", "Please enter a transfer reason");
            _list.Add("M_ENTERVALIDVALUE", "Value must be 'Y' or 'N'");
            _list.Add("M_ENTRYLISTED", " Entry listed.");
            _list.Add("M_ENTRIESLISTED", " Entries listed.");
            _list.Add("M_ENTRIESZERO", "0 Entries listed.");
            _list.Add("M_EODBACKUP", "Unable to save configuration as not enough space on disk to perform backup.");
            _list.Add("M_EODBACKUPPATH", "Unable to save configuration as the backup path for {0} is incorrect.");   // jec 08/02/07
            _list.Add("M_EODCONTROL", "You have entered an invalid value for one or more EOD options.");
            _list.Add("M_EPSONOPOS4.NetINFO", "You must download and install EPSON OPOS ADK for .NET from www.epson-pos.com.\nPlease connect your printer before installing the software.\n\nFor printers other than EPSON, please contact support."); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_EODDELETECONFIG", "Select configuration to be deleted.");
            _list.Add("M_EODDELETEDEFAULT", "Default configuration cannot be deleted.");
            _list.Add("M_EODINVALIDFREQUENCY", "Choose a valid frequency.");
            _list.Add("M_EODINVALIDOPTION", "Add at least one option.");
            _list.Add("M_EODRUNCOMPLETE", "EOD Of Day Process Completed");
            _list.Add("M_EODRUNINFO", "You must select at least one option in order to run the End Of Day process.");
            _list.Add("M_EODSSAVEDEFAULT", "Please enter a new configuration name as the default configuration cannot be changed.");
            _list.Add("M_EODFACTFILEEXISTS", "The following file exists on the FACT installation and should be removed before running end of day: ");
            _list.Add("M_EODSYSTEMOPEN", "CoSACS is still Open.  You must close CoSACS and ensure any \\current users are logged off before starting the End Of Day Batch.");
            _list.Add("M_EODWARNING", "IMPORTANT!!!  Please check that :-' \\\\1) THERE ARE NO USERS CURRENTLY ACCESSING THE DATABASE.\\\\2) The interface selections are valid for this EOD run.\\\\\\\\Press OK to start the EOD process, or CANCEL to abort it.");
            _list.Add("M_ERROR", "Error");
            _list.Add("M_ERRORSRETRY", "There are errors. Would you like to correct them or cancel?");
            _list.Add("M_ESTIMATEREQUIRED", "ESTIMATE REQUIRED");
            _list.Add("M_EXCEEDSSAFEBALANCE", "There are insufficient funds in the safe. The safe has {0} and you wish to take out {1}.\\\\Reduce your amount by {2} or add {2} to the safe.");
            _list.Add("M_EXPIREDVOUCHERDAUTH", "Authorisation is required to redeem an expired gift voucher.");
            _list.Add("M_EXPIRYPAST", "Expiry date must be in the future.");
            _list.Add("M_EXTENDEDWARRANTY", "Authorisation is required to remove an extended warranty.");
            _list.Add("M_EXCEPTIONONITEM", "Please note that there is an outstanding exception on an item which is awaiting to be resolved in the Failed Deliveries and Collections screen"); //#10535
            _list.Add("M_ODDPAYMENTAMT", "The odd payment amounts entered must total the total odd payment amount"); //IP & JC - 12/01/09 - CR976
            _list.Add("M_EXTRAPAYMENTSESSION", "The extra payment for account {0} ({1})\nhas NOT been updated because it has been changed by another session.\n\nDo you wish to continue with the rest of the list?");
            _list.Add("M_FACTACCTNO", "Fact2000 Corresponding Account Number must be four digits long.");
            _list.Add("M_FAULTYPRODUCT", "Faulty Product Note");
            _list.Add("M_FILEEXISTS", "A file of this name already exists.");
            _list.Add("M_FLOATMUSTBENEGATIVE", "A Float must be negative");
            _list.Add("M_GOTODEPOSITSSCREEN", "You must now complete cashier deposits");
            _list.Add("M_NUMERICVALUE", "Please enter a numeric value"); //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions
            _list.Add("M_NONNUMERICVALUE", "Please enter a non-numeric value"); //NM 13/08/09 - UAT(5.2) - 759
            _list.Add("M_GREATERTHANZERO", "Please enter an amount greater than zero"); //NM & IP - 07/01/09 - CR976 - Extra Telephone Actions
            _list.Add("M_GREATERTHANZERO2", "Please enter a value greater than zero"); //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            _list.Add("M_HISTORICBEFOREEND", "The End Date cannot be changed to before {0}");
            _list.Add("M_HISTORICBEFORESTART", "The Start Date cannot be changed to before {0}");
            _list.Add("M_HISTORICCHANGE", "A historic Start Date cannot be changed");
            _list.Add("M_HISTORICNEW", "A historic End Date cannot be changed to a new historic date");
            _list.Add("M_HISTORICALSR", "This service request is historical and a more recent one exists for this item.");  //UAT 398
            _list.Add("M_HIALLOCATEDTOOLARGE", "The value for 'Highest Allocated' must not be greater than 'Highest Allowed'");
            _list.Add("M_ICAPPROVED", "Customer has been granted Instant Credit");
            _list.Add("M_IMPORTRULESOK", "New rules successfully imported and saved to the database.");
            _list.Add("M_INCOMPLETECARDNO", "The card number is incomplete");
            _list.Add("M_INFORMATION", "Information");
            _list.Add("M_INSTALMAX", "The number of installments can not be greater than 60");
            _list.Add("M_INSTRESUPDATED", "Resolution Successfully Updated");
            _list.Add("M_INTERACTIONFUTUREDATE", "Interaction date in future is not allowed");
            _list.Add("M_INTERESTACCOUNTS", "Please wait for interest accounts to load ...");
            _list.Add("M_INUSEONTHISACCOUNT", "Contract number {0} is already in use on this account");
            _list.Add("M_INUSEONOTHERACCOUNT", "Contract number {0} is already in use on another account");
            //lw69699 RDB 30/04/08 only accounts with status 6 should have the possibility of a BDW
            //_list.Add("M_INVALIDBDW", "Account must be in status code 5 or 6 for BDW"); //  69125 SC 09/07/07
            _list.Add("M_INVALIDVALUE", "Invalid value entered");
            _list.Add("M_INVALIDBANKREFERENCENO", "Bank Reference Number must be alphanumeric");
            _list.Add("M_INVALIDBDW", "Account must be  status code 6 for BDW"); //  69125 SC 09/07/07
            _list.Add("M_INVALIDBDWC", "Account must not be in credit for BDW"); // 68473 RD 06/09/06 // 69125 JH reference to account status 6 removed
            _list.Add("M_INVALIDCLWAccStatus", "Account must be  status code 6 for CLW"); //  69125 SC 09/07/07
            _list.Add("M_INVALIDCLWC", "Account must not be in credit for CLW"); // 68473 RD 06/09/06 // 69125 JH reference to account status 6 removed
            _list.Add("M_INVALIDBDWD", "Account must be settled with outstanding balance equal to zero");  // 68473 RD 06/09/06
            _list.Add("M_INVALIDBRANCH", "Configured branch number ({0}) is not valid.\\Please change and retry.");
            _list.Add("M_INVALIDCANCELCODE", "Invalid cancellation reason.  Please select a valid reason.");
            _list.Add("M_INVALIDCODE", "New status code invalid");
            _list.Add("M_INVALIDCONFIGURATION", "Enter valid configuration.");
            _list.Add("M_INVALIDCOUNTRY", "No country parameters for country code {0}. \\\\Check country code is correct.");
            _list.Add("M_INVALIDCREDIT", "This transaction will put account into credit.");
            _list.Add("M_INVALIDCUSTID", "Warning: Legacy Customer Id or invalid format");
            _list.Add("M_INVALIDDATE", "Date must not be > date of proposal");
            _list.Add("M_INVALIDDATEDEL", "The date of delivery cannot be before the date the account was opened.");
            _list.Add("M_INVALIDFIELDS", "Some fields are invalid.  Please correct before saving.");
            _list.Add("M_INVALIDEMAIL", "Email Address must contain @.  Please correct before saving.");    //UAT84 jec 21/10/10
            _list.Add("M_INVALIDFILENAME", "The file name must be of type .jpg");
            _list.Add("M_INVALIDIMAGEFILE", "The file you have selected is of an invalid format");
            _list.Add("M_INVALIDPHOTODIRECTORY", "The directories for customer photographs and signatures need to be set in Country Maintenance");
            _list.Add("M_INVALIDRDL", "Account must be in SC6");
            _list.Add("M_INVALIDSCHEDULECHANGE", "Unable to confirm change to schedule as the stock location has change. \\Item must be removed from existing schedule and rescheduled.");
            _list.Add("M_INVALIDSETNAME", "You must enter a valid name for the set you wish to save");
            _list.Add("M_INVALIDSALESPERSON", "Employee no for Salesperson must be between 1 and 9999");
            _list.Add("M_INVALIDSALESPERSONNO", "This Employee no is not a valid Salesperson");
            _list.Add("M_INVALIDTIMEFORMAT", "Times must be written in 24 hour format (HH:MM).");
            _list.Add("M_INVALIDWORKLIST", "Enter valid Work List.");
            _list.Add("M_INVERTFAIL", "Unable to invert receipt printer.");
            _list.Add("M_IRPERIOD1", "Manufacturer warranty");
            _list.Add("M_IRPERIOD2", "Warranty remains in place - Courts pay cost of replacement");
            _list.Add("M_IRPERIOD3", "Warranty terminates - Courts pay cost of replacement");
            _list.Add("M_IRPERIOD4", "Warranty terminates - EW pay cost of replacement");       //CR1030 jec
            _list.Add("M_IRPERIOD5", "Warranty has expired");
            _list.Add("M_ITEMSCHEDULED", "Item has been scheduled for delivery or is about to be delivered.");
            _list.Add("M_KITFAIL", "Unable to add kit.");
            _list.Add("M_LANDLORDREQUIRED", "Landlord reference is required because residential\\status is rented. Must change one of the current references.");
            _list.Add("M_LENGTHALREADYTHERE", "Length option already present");
            _list.Add("M_NOBETWEENMINMAX", "Length options must be between min and max");
            _list.Add("M_DEFAULTBETWEENMINMAX", "Default months must be between min and max");
            _list.Add("M_BLANKTERMSTYPECODE", "Terms type code must not be blank");
            _list.Add("M_BLANKTERMSTYPEDESC", "Terms type description must not be blank");
            _list.Add("M_BLANKSTORETYPE", "Store Type must be selected.");
            _list.Add("M_NOPAYMENTHOLIDAYWITHINTFREE", "Can't have payment holidays with an interest free account - ensure Payment Holidays is 'No' or Full Rebate If Paid Within is 0.");
            _list.Add("M_LINKEDCANNOTBEHOLDER", "Linked customer cannot be the same as the holder");
            _list.Add("M_LINKEDSPIFFDESCR", "A description must be entered for this Linked Spiff");    // CR36
            _list.Add("M_LOADINGDATA", "Loading data ...");
            _list.Add("M_LOCKEDACCOUNTS", "There are {0} accounts already locked for customer {1}.\\Details of each lock are in the account list.\\\\You can try reloading this customer, or contacting the sales person holding each lock.");
            _list.Add("M_LOCKINGACCOUNT", "Locking account ...");
            _list.Add("M_MANDATEACTIVE", "This mandate is currently active.\\\\Are you sure you want to change the details?");
            _list.Add("M_MANDATEAPPROVED", "This mandate has been approved and can only be updated by Head Office");
            _list.Add("M_MANDATECHANGED", "You have changed one or more of the fields:\\" +
                "    - Account number;\\" +
                "    - Due day;\\" +
                "    - Bank code;\\" +
                "    - Bank branch number;\\" +
                "    - Bank account number;\\" +
                "    - Bank account name;\\" +
                "    - Termination date.\\" +
                "If you continue a history record will be created and the new\\" +
                "details will have {0}\\and {1}");
            _list.Add("M_MANDATEDASHSPACE", "The following fields must not contain a dash or a space:\\" +
                "    - Bank Branch Number;\\" +
                "    - Bank Account Number.");
            _list.Add("M_MANDATEDELETED", "This mandate has been deleted at the bank and cannot be updated.\\You can create a new mandate application by pressing YES.\\If you press YES then a new mandate application form must be submitted to the bank.\\\\Do you want to create a new mandate?");
            _list.Add("M_MANDATEEXISTS", "There is an existing mandate for the account {0}\\" +
                "with {1} and {2}\\\\If you continue the previous mandate will be given\\" +
                "the End Date {3}\\The new mandate will have {4} and {5}.\\\\" +
                "Are you sure you want to end the existing mandate and replace\\" +
                "it with this mandate ?");
            _list.Add("M_MANDATEMANDATORY", "The following fields are mandatory and must be completed:\\" +
                "    - Due Day;\\" +
                "    - Bank Code;\\" +
                "    - Bank Branch Number;\\" +
                "    - Bank Account Number;\\" +
                "    - Bank Account Name.");
            _list.Add("M_MANDATEREINSTATED", "You have changed the end date on a mandate that has\\" +
                "already been cancelled. If you continue the mandate\\" +
                "will be re-instated with the new end date and the\\" +
                "reject count will be reset to zero.\\\\" +
                "Do you wish to continue?");
            _list.Add("M_MANDATERETURNED", "There is already a Return Date on this mandate of {0}.\\\\Are you sure you want to send a Return Mandate Letter ?");
            _list.Add("M_MANDATORYFIELD", "Mandatory field {0} must be entered");
            _list.Add("M_MANDATORYZONE", "A valid Collection Zone must be selected");       //CR1084
            _list.Add("M_MANDATORYDELAREA", "A valid Delivery Area must be selected");       // #12226
            _list.Add("M_MANUALACCTTYPE", "Please enter manual {0} account number");
            _list.Add("M_MANUALREFER", "The credit application has been declined.\\\\Do you want to Manually Refer?");
            _list.Add("M_MATRIXSAVED", "Matrix successfully saved.");
            _list.Add("M_MATRIXWRONGCOUNTRY", "Import file did not contain a scoring matrix for this country.");
            _list.Add("M_MATRIXTTWRONGCOUNTRY", "Import file did not contain a Terms Type matrix for this country.");
            _list.Add("M_MAXBAL", "Maximum balance cannot be less than Minimum balance");    //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MAXTERMS", "Term (Months) can not be more than {0} months.");
            _list.Add("M_MAXVALCOLL", "Maximum value collected cannot be less than Minimum value collected");    //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MAXMNTHSARRS", "Maximum months in arrears cannot be less than Minimum months in arrears"); //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MAXCLAUSES", "Rule already has maximum number of clauses.\\Further conditions must be expressed as composite clauses.");
            _list.Add("M_MAXWARRANTY", "The maximum number of warranties has been purchased for this item.");
            _list.Add("M_MAXINSTALLATIONS", "The maximum number of installations has been added for this item."); //IP - 23/02/11 - Sprint 5.10 - #3133
            _list.Add("M_MINREFERENCES", "At least {0} References must be entered");
            _list.Add("M_CREDITMINVALUE", "Total cash price is less than minimum value for credit");       // #17287
            _list.Add("M_MINBAL", "Minimum balance cannot be greater than Maximum balance");    //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MINVALCOLL", "Minimum value collected cannot be greater than Maximum value collected");    //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MINMNTHSARRS", "Minimum months in arrears cannot be greater than Maximum months in arrears");    //IP - 09/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MISSINGADDRESS", "You must add the following address types for this customer: \\\\{0}");
            _list.Add("M_MISSINGCOMMISSION", "Missing commission value for Product category {0}. Please re-enter");   //CR36
            _list.Add("M_MMINOTCALC", "MMI value is still not calculated for this customer.\n\nAccount should Sanctioned before accepting Cash Loan.");
            _list.Add("M_MISSINGADDRESSWARNING", "The following delivery address type(s) are not known for this customer: \\\\{0}");
            _list.Add("M_MULTIPLECOLLCOMMNRATES", "You have entered more than one commission rate. Are you sure this is correct?");            //IP - 16/06/10 - CR1083 - Collection Commissions
            _list.Add("M_MULTIPLESRQUANTITY", "Item {0} already has {1} open Service Requests against a sold quantity of {2}.\n\nIf you continue an additional Service Request will be opened with a new sequence number.");
            _list.Add("M_MSPOSDOWNLOADERROR", "Error downloading MS POS for .Net components. Please check error log"); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_MUSTDEPOSIT", "Outstanding income must be deposited before other functions can be used.");
            _list.Add("M_MUSTDEPOSITOUTSTANDING", "Outstanding income must be deposited before a float can be entered.");
            _list.Add("M_MUSTREFRESHCASHIER", "The Cashier Totals have changed.\n\nThis Cashier may be logged in elsewhere.\n\nThe Cashier Deposits screen will be refreshed.");
            _list.Add("M_NEGATIVEAGRTOTAL", "This sales order would cause a negative Agreement Total of {0}.\n\nYou may need to remove a discount item.");
            _list.Add("M_NEGATIVEGOODSRETURN", "This Goods Return would cause a negative Agreement Total of {0}.\n\nYou may need to remove a discount item as well.");
            _list.Add("M_NEGATIVEINCOME", "Customer's disposable income is less than zero.");
            _list.Add("M_NOASSOCIATEDITEMS", "There are no associated items to add for this item."); //IP - 25/02/11 - Sprint 5.11 - #
            _list.Add("M_NOACCOUNT", "No accounts associated with this customer.");
            _list.Add("M_NOACCOUNTDATA", "No data retuned for this account number");
            _list.Add("M_NOACCOUNTFORDEL", "The account details could not be loaded - Delivery not processed.");
            _list.Add("M_NOACCOUNTSELECTED", "No account selected.");
            _list.Add("M_NOAMTENTERED", "Please enter an amount.");
            _list.Add("M_NOCHARGETOINTERNAL", "The Internal Account to post Charge To transactions is not defined in Country Parameters.\n\nService Request {0}");
            _list.Add("M_NOCHARGETOWARRANTY", "The Warranty (EW) Account to post Charge To transactions is not defined in Country Parameters.\n\nService Request {0}");     //CR1030 jec
            _list.Add("M_NOCHARGETOSUPPLIER", "The Supplier account could not be found to post Charge To transaction.\n\nService Request {0}, Supplier Code '{1}'");
            _list.Add("M_NOCHARGETODELIVERER", "Failed to create Deliverer cash account to post Charge To transaction.\n\nService Request {0}, Deliverer Code '{1}'");
            _list.Add("M_NOCHARGETOCUSTOMER", "Failed to create Customer cash account to post Charge To transaction.\n\nService Request {0}, Customer Id '{1}'");
            _list.Add("M_NOCHARGETOINTSTALL", "The Intstallation Account to post Charge To transactions is not defined in Country Parameters.");   //CR1030 jec 02/02/11
            _list.Add("M_NOSTOCKACCOUNT", "The Stock Inventory Account to post Courts Parts used is not defined in Country Parameters.\n\nService Request {0}");
            _list.Add("M_NOINSTSTOCKACCOUNT", "The Stock Inventory Account to post Courts Parts used is not defined in Country Parameters.\n\nInstallation No {0}");        //#12166
            _list.Add("M_NOLANDLORDDETAILS", "Applicant residential status is rented.\\\\You must enter their landlord as a reference");
            _list.Add("M_NOBDWACCOUNT", "No bad debt write-off account found for branch {0}");
            _list.Add("M_NOBUREAULIMITS", "No credit bureau scoring parameters have been set. Unable to continue.");
            _list.Add("M_NOCASHTILLHISTORY", "There are no cash till history records for this employee");
            _list.Add("M_NOCLAUSES", "You have not entered any clauses");
            _list.Add("M_NOCUSTOMERFORPRINT", "Account must have an associated customer to print agreement.");
            _list.Add("M_NOEXCHANGERATE", "There are no Exchange Rates defined for Foreign Currency Payments.\\\\Foreign Currency amounts cannot be calculated.");
            _list.Add("M_NOGIFTMETHOD", "This account is securitised and cannot be paid with a Gift Voucher");
            _list.Add("M_NOGIFTVOUCHERACCOUNT", "The gift voucher account {0} could not be found.");
            _list.Add("M_NOGOODSTORETURN", "There are no items in the list of goods to return");
            _list.Add("M_NOLINES", "All items have been collected, do you wish to cancel this account?");
            _list.Add("M_NONARREARS", "This is a non-arrears strategy and can not be deactivated.");
            _list.Add("M_NONNUMERIC", "Contains non-numeric characters");
            _list.Add("M_NOTALLDELIVERED", "Error loading delivery details for branch no: {0} and buffno: {1}.\nRemaining Deliveries on this Schedule have not been processed.");
            _list.Add("M_NOTRANSACTION", "No transaction will take place until you return to this screen and print a receipt.");
            _list.Add("M_NOSTOCKAUTH", "Authorisation is required to sell items not in stock or not on order.");
            _list.Add("M_OLDDELIVERYDATE", "The Delivery Date {0} is over three weeks old\n\nAre you sure you want to Notify Deliveries with this date ?");
            _list.Add("M_CORRECTBDW", "This is a BDW Payment and can only be corrected against the written off account");
            _list.Add("M_CORRECTEDPAY", "This Payment already has a correction");
            _list.Add("M_CORRECTTODAY", "This Payment is not dated today and can not be corrected");
            _list.Add("M_NOCASHLOANTERMSTYPE", "There are no Terms Types setup for {0} Cash Loans. Please setup a new Terms Type before continuing.");
            _list.Add("M_NONSTOCKSEXISTS", "Account cannot be cancelled.  Non-Stock deliveries exist.");
            _list.Add("M_NOOVERAGESACCOUNT", "There is no account set up to record the overage");
            _list.Add("M_NOPAIDANDTAKEN", "There is no paid and taken account set up");
            _list.Add("M_NOPAYMENTCARDINFO", "No payment card information retrieved.");
            _list.Add("M_NOPAYMENTSELECTED", "No payment has been selected. Cannot process correction.");
            _list.Add("M_NODPYNOLINK", "No DPY transaction has been selected on the BDW account.\n\nThe refund cannot be linked to a customer account.\n\nAre you sure you want to continue?");
            _list.Add("M_NODPYPAYMENTSELECTED", "No transaction has been selected on the BDW account.\n\nCannot process refund.");
            _list.Add("M_NOPHOTO", "No photograph exists for this customer.");
            _list.Add("M_NORECEIVABLEACCOUNT", "No Receivable account could be found to write shortage to for this cashier");
            _list.Add("M_NORFCREDIT1", "Customer has no available ready finance credit");
            _list.Add("M_NORFCREDIT", "This customer has not been awarded any ready finance credit. What would you like to do with the account that was sanctioned ({0})?");
            _list.Add("M_NOSCORINGINFO", "Unable to retrieve scoring information for this account.");
            _list.Add("M_NOSCORINGLIMITS", "No scoring limits set, unable to evaluate score.");
            _list.Add("M_NOSIGNATURE", "No signature exists for this customer.");
            _list.Add("M_NOSTOCK", "No available stock for this product at this location. \\Unable to proceed.");
            _list.Add("M_NOSTOCKOPTION", "No available stock for this product at this location. Continue?");
            _list.Add("M_NOSUCHACCOUNT", "Account does not exist.");
            _list.Add("M_NOSUCHCUSTOMER", "Customer does not exist.");
            _list.Add("M_NOSUCHSERVICEREQUEST", "Service Request does not exist.");
            _list.Add("M_NOSUCHSERVICEREQUESTBOOKING", "Service Request {0} not found or not suitable to book for Estimate/Repair.");
            _list.Add("M_NOSUNDRYACCOUNT", "No sundry credit account could be found");
            _list.Add("M_NOSUNDRYACCOUNTBRANCH", "No sundry credit account could be found for branch {0}");
            _list.Add("M_NOTAFFINITY", "Affinity items cannot be added for this terms type.");
            _list.Add("M_NOTCANCELLED", "Reversal cannot be processed as account is not cancelled.");
            _list.Add("M_NOPICKLISTTOPRINT", "There are no 'released' items to print.");
            _list.Add("M_NOTUNIQUE", "Reference is not unique");
            _list.Add("M_NOTVALIDFORCASH", "Not valid for cash accounts.");
            _list.Add("M_NOWARRANTIES", "There are no warranties for this product");
            _list.Add("M_NOWARRANTY", "There is no warranty cover for product {0}");
            _list.Add("M_NUMERIC", "Please enter a number using the numeric characters 0 to 9");
            _list.Add("M_NUMERICREQUIRED", "You must enter a numeric value in the {0} field");
            _list.Add("M_NUMBERACCOUNTPRINT", "Printing Manual Account Numbers ...");
            _list.Add("M_NUMBERACCOUNTPRINTED", "Manual Account Numbers printed");
            _list.Add("M_NUMBERCONTRACTPRINT", "Printing Manual Contract Numbers ...");
            _list.Add("M_NUMBERCONTRACTPRINTED", "Manual Contract Numbers printed");
            _list.Add("M_ONEFORONEREPLACEMENT", "Instant Replacement Note");
            _list.Add("M_ONECHARCODEONLY", "Only one character can be entered for the code in this category");
            _list.Add("M_ONLYAFFINITY", "Only affinity products can be added for this terms type.");
            _list.Add("M_OPOSDOWNLOADING", "Slip Printer controls are not installed\\Download will now commence.\\Wait for installation to complete and then retry.");
            _list.Add("M_OPOSDOWNLOADERROR", "Error downloading OPOS components. Please check error log"); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("M_ORDERSLISTED", " Order(s) listed.");
            _list.Add("M_ORDERSZERO", "0 Orders listed.");
            _list.Add("M_OUTOFSTOCK", "Special Order: User: {0}. Item not in stock, not on order");
            _list.Add("M_OUTSTANDINGADJUSTMENT", "Account cannot be cancelled.  Warranty adjustment must be paid for.");
            _list.Add("M_OUTSTANDINGDELIVERIESS", "Account cannot be cancelled.  Merchandise must be collected first.");
            _list.Add("M_OUTSTANDINGPAYMENTS", "There are outstanding payments on the account.  Do you wish to:");
            _list.Add("M_OUTSTANDINGSAFEDEPOSITS", "Outstanding deposits to the safe of {0} must be \\reversed in the cashier deposits screen before cashier \\totals can be saved for this cashier.");
            _list.Add("M_OUTSTANDINGTRANSACTIONS", "Account cannot be cancelled.  Financial Transactions outstanding.");
            _list.Add("M_ODDPAYMENT", "Odd payment amount must not be negative");
            _list.Add("M_OVERAGE", "An overage of {0} will be posted to account {1}.");
            _list.Add("M_OVERAGEREVERSE", "An overage of {0} will be posted to account {1}.\\\\The last shortage for the same cashier was for the same amount.\\Do you want to reverse the last shortage with this overage ?");
            _list.Add("M_OVERLAPPINGPERIOD", "Another rate has the same start date.\n\nPlease remove or amend that rate first.");
            _list.Add("M_PAIDTAKENAUTH", "Authorisation is required to revise a Paid and Taken account. Please enter the appropriate credentials.");
            _list.Add("M_PARTIALLYDELIVERED", "Cannot delete because item {0} has been at least partially delivered.");
            _list.Add("M_PARTSAFTERREPAIR", "The Parts Expected date cannot be after the Repair Date");
            _list.Add("M_PARTSDATEISSUE", "A Parts Expected Date of {0} has already been set for this service request that is after this repair date.");
            _list.Add("M_PARTSAVELIST", "Not all of your changes were saved due to another\nsession editing the same {0}.\n\nYou will need to check whether you need to make further changes.");
            _list.Add("M_PAYMENTCARDDUEDATE", "Deliveries outstanding - \\\\do you wish to update the payment card for instalment and due date?");
            _list.Add("M_PERCENTEXCEEDSSERVICE", "The Insurance% must not be more than the Service%");
            _list.Add("M_PERCENTEXCEEDSSERVICEBAND", "The Insurance% must not be more than the Service% for band {0}");
            _list.Add("M_PCENTORVALUE", "Only percentage OR value may be entered - not both");    //CR36
            _list.Add("M_PCENTANDVALUE", "A percentage OR value must be entered - both can not be 0");    //CR36
            _list.Add("M_PAYMENTNEGATIVE", "Attempted to save Payment as a Debit");
            _list.Add("M_PERCENTRANGE", "Percentage must be between 0 and 100");
            _list.Add("M_PICKLISTDEL", "Delivery Notes To Be Picked");
            _list.Add("M_PICKLISTCOLL", "Collection Notes To Be Picked");
            _list.Add("M_PREVIOUSPERIOD", "The Start Date cannot be on or before the start of the previous period.\nPlease change or remove the previous period first.");
            _list.Add("M_POSITIVENUM", "Please enter a positive number");
            _list.Add("M_POSWHOLENUM", "Please enter a positive whole number");
            _list.Add("M_PREEXISTING", "Deliveries exist on this pre-existing account.  Cannot cancel this account");
            _list.Add("M_PRINTERCLAIMFAIL", "Printer.Claim failed: rc = {0}");
            _list.Add("M_PRINTERCLOSED", "Printer is not open");
            _list.Add("M_PRINTERCLOSEFAIL", "Printer.Close failed: rc = {0}");
            _list.Add("M_PRINTERFEEDFAIL", "Printer.Feed failed: rc = {0}");
            _list.Add("M_PRINTERFLUSHFAIL", "Printer.Flush failed: rc = {0}");
            _list.Add("M_PRINTERINITFAIL", "Printer.Init failed: rc = {0}");
            _list.Add("M_PRINTERINVERTFAIL", "Printer.Invert failed: rc = {0}");
            _list.Add("M_PRINTERNARROWFAIL", "Printer.Narrow failed: rc = {0}");
            _list.Add("M_PRINTEROPENFAIL", "Printer.Open failed: rc = {0}");
            _list.Add("M_PRINTEROPENDRAWERFAIL", "Printer.Open drawer failed: rc = {0}");
            _list.Add("M_PRINTERPRINTFAIL", "Printer.PrintImmediate failed: rc = {0}");
            _list.Add("M_PRINTCARDNEW", "Printing new Payment Card for Account {0} ...");
            _list.Add("M_PRINTCARDUPDATE", "Printing updates on Payment Card ...");
            _list.Add("M_PRINTCREDITNOTE", "Would you like to print a credit note?");
            _list.Add("M_PRINTNEWPAYMENTCARD", "Please insert new payment card and press OK");
            _list.Add("M_PRIVCLUBCODEDELETE", "Privilege Club customer codes '{0}' and '{1}' cannot be deleted");
            _list.Add("M_PRIZEVOUCHER", "Prize Vouchers");
            _list.Add("M_PRODUCTINVALID", "Invalid Product Code");
            _list.Add("M_PRODUCTCLASSLEN", "Product Class must be 2 or 3 characters.");         // RI
            _list.Add("M_PRODUCTSUBCLASSLEN", "Product SubClass must be 3 to 5 characters.");         // RI
            _list.Add("M_PRODUCTSKULEN", "Product SKU must not be > 8 characters.");       // RI
            _list.Add("M_PRODUCTLEN", "Product must not be > 12 characters.");       // RI 
            _list.Add("M_PRODUCTNONCOURTS", "NON-Courts Product Code");
            _list.Add("M_PRODUCTNONSTOCK", "The product you have entered is not a Stock Item. You cannot enter Spiffs for a nonstock item.");    //CR36
            _list.Add("M_PRODUCTNOTEXIST", "The {0} {1} you have entered does not exist. Do you want to continue?");    //CR36
            _list.Add("M_TWOPRODUCTSREQUIRED", "At least the first two Products must be entered for Linked Spiffs");      //CR36
            _list.Add("M_DUPLICATEPRODUCT", "A duplicate Product has been entered");      //CR36
            _list.Add("M_PRODUCTNOTSTOCK", "The Product Code must be for a stock item type");
            _list.Add("M_PRODUCTNOTSPAREPART", "The Product Code must be for a spare part"); // Required for selecting a spare part for a Service Request JH 08/11/2007
            _list.Add("M_PCCODEADDREMOVE", "Privilege Club customer codes '{0}' and '{1}' cannot be added or removed");
            _list.Add("M_PCVALIDATECLUBAVERAGESTATUS", "Club Average Status should be a decimal value from 1 to 2 and not less than Warning Average Status");
            _list.Add("M_PCVALIDATECONSECUTIVEINSTALMENTS", "Consecutive Instalments should be an integer value from zero upwards");
            _list.Add("M_PCVALIDATECREDITEXPIRYMONTHS", "Credit Expiry Months should be an integer value from one upwards");
            _list.Add("M_PCVALIDATEMINCLUBLEN", "Min Club Length should be an integer value from one upwards but not greater than {0} or {1} Credit Length");
            _list.Add("M_PCVALIDATEREMINDERDAYS", "Days between Invitation & Reminder should be an integer from one upwards");
            _list.Add("M_PCVALIDATETIERCASHLEN", "{0} Cash Length should be an integer value from one upwards and not less than {0} Cash Maintain Length");
            _list.Add("M_PCVALIDATETIERCASHMAINTAINSPEND", "{0} Cash Maintain Spend should be an integer value from one up to the value of {0} Cash Spend");
            _list.Add("M_PCVALIDATETIERCASHSPEND", "{0} Cash Spend should be an integer value from one upwards and not less than {0} Cash Maintain Spend");
            _list.Add("M_PCVALIDATETIERCREDITLEN", "{0} Credit Length should be an integer value from Min Club Length upwards");
            _list.Add("M_PCVALIDATETIERDISCOUNT", "{0} Discount % should be a decimal from 0 to 100");
            _list.Add("M_PCVALIDATETIERDISCOUNTITEMNUMBER", "{0} Discount Item Number should be a non blank string up to eight characters");
            _list.Add("M_PCVALIDATEWARNINGAVERAGESTATUS", "Warning Average Status should be a decimal value from 1 to 2 and not greater than Club Average Status");
            _list.Add("M_PCVALIDATEWARNINGLEADDAYS", "Warning Lead Days should be an integer value from one upwards");
            _list.Add("M_PHOTOSAVED", "Photograph successfully saved.");
            _list.Add("M_QUANTITYTOOLOW", "Quantity must be greater than zero.");
            _list.Add("M_RATESLISTED", " Rate(s) listed.");
            _list.Add("M_RATESZERO", "0 Rates listed.");
            _list.Add("M_READYASSISTTERM", "The Ready Assist Term Length must be {0}"); //#18604 - CR15594
            _list.Add("M_READYASSISTDEBIT", "Customers account will be debited {0} for the used portion of the Ready Assist"); //#18604 - CR15594
            _list.Add("M_REALLOCATIONFAILED", "The number of accounts selected exceeds the number of accounts which can be allocated to this employee.\n The number of accounts that can be allocated is {0}"); //IP - 08/10/09 - UAT(909)
            _list.Add("M_RETURNCODEREQUIREDFORWARRANTY", "Please enter a return code for each selected warranty");
            _list.Add("M_RETURNQUANTITY", "Return quantity cannot be larger than the delivered quantity");
            _list.Add("M_RETURNQUANTITYZERO", "Return quantity must be greater than zero");
            _list.Add("M_REASONCHANGEDELDATE", "The delivery date for item {0} on Delivery Note {1} / {2} will be changed");
            _list.Add("M_REASONDELETEDN", "Delivery Note {0} / {1} will be deleted from the load and all items will be deleted from the account"); //IP - 18/02/09 - CR929 & 974
            _list.Add("M_REASONREMOVEDELNOTE", "ALL items on Delivery Note {0} / {1} will be removed from the delivery");
            _list.Add("M_REASONREMOVEDELNOTEITEM", "The item {0} on Delivery Note {1} / {2} will be removed from the delivery");
            _list.Add("M_REBATEPAID", "Rebate has been added to the account.\\\\Balance should be zero or in credit.");
            _list.Add("M_REBATENEGATIVE", "Attempted to save Rebate as a Debit");
            _list.Add("M_RECEIPTNOTALLOCATED", "The receipt {0} is not allocated");
            _list.Add("M_RECEIPTPAID", "The receipt {0} already has a payment");
            _list.Add("M_RECEIPTPRINTERNOPAPER", "Please insert paper into the receipt printer");
            _list.Add("M_REENTERQUANTITY", "Re-enter quantity");
            _list.Add("M_REFUNDAUTH", "Authorisation is required to process a refund.");
            _list.Add("M_REFUNDMAXALLOC", "Authorisation is required to override maximum allocation.");
            _list.Add("M_REFUNDTOOHIGH", "Refund amount cannot exceed the total value of payments of {0}.");
            _list.Add("M_REINSERTPAYMENTCARD", "Please re-insert payment card to continue printing transactions");
            _list.Add("M_REJECTIONSESSION", "The reject action for account {0} ({1})\nhas NOT been updated because it has been changed by another session.\n\nDo you wish to continue with the rest of the list?");
            _list.Add("M_RELEASE", "Cannot Release - Hold flags still outstanding");
            _list.Add("M_REMOVEAFFINITY", "You must remove Affinity items before you can\\add a non-affinity item to this account.");
            _list.Add("M_REMOVENONAFFINITY", "You must remove non-Affinity items before you\\can add an affinity item to this account.");
            _list.Add("M_REPLACEITEM", "Do you want to replace the existing item with this code and location?");
            _list.Add("M_REPLACECASHANDGOITEM", "Do you want to replace the existing item with this item?"); //IP - 15/02/10 - LW - 71860
            _list.Add("M_REPO", "This account has been repossessed.");
            _list.Add("M_REPRINTVOUCHERS", "Authorisation is required to reprint prize vouchers.");
            _list.Add("M_REQDELIVERYDATE", "The earliest Required Delivery Date allowed is the date\\this account was opened on {0}");
            _list.Add("M_REQUIREACCOUNTNO", "Please enter an Account Number");
            _list.Add("M_REQUIREBANK", "Please select a Bank");
            _list.Add("M_REQUIREBANKREFNO", "Please enter a Bank Reference Number");
            _list.Add("M_REQUIREBANKACCOUNTTYPE", "Please select a Bank Account Type");
            _list.Add("M_REQUIREBANKBRANCH", "Please enter Bank Branch");
            _list.Add("M_REQUIREBANKACCOUNTNO", "When paying by cheque the Bank Account Number must be entered");
            _list.Add("M_REQUIREBANKACCOUNTNOEBT", "When paying by Electronic Bank Transfer the Bank Account Number must be entered"); //Used for Electronic Bank Transfer
            _list.Add("M_REQUIREBANKREFERENCE", "Please enter Bank Reference");
            _list.Add("M_REQUIREBANKACCOUNTNAME", "Please enter Name on Account");
            _list.Add("M_REMOVEEBTDETAILS", "Are you sure you wish to remove Electronic Banking Details?");
            _list.Add("M_REQUIRECUSTID", "Please enter a Customer Id");
            _list.Add("M_REQUIREPAYMETHOD", "The payment method must be entered for a payment");
            _list.Add("M_REQUIREAGREEMENTNO", "The Agreement Number is Required\\For Service Request Payments Please Enter The Service Request Number");
            _list.Add("M_REQUIRECARDTYPE", "When paying by card the type of card must be entered");
            _list.Add("M_REQUIRECHEQUENO", "The cheque or card number must be entered for this payment method");
            _list.Add("M_REQUIREPAYAMOUNT", "The payment amount must be entered for a payment");
            _list.Add("M_REQUIRETENDERED", "The Tendered Amount must be entered for a payment");
            _list.Add("M_REVERSEDEPOSIT", "Deposit has been interfaced to FACT. Do you want to post a reversal to FACT as well?");
            _list.Add("M_REVISEAWAITING", "You do not have the right to revise items awaiting scheduling.");
            _list.Add("M_REVISECASH", "This is a CASH account and cannot be revised");
            _list.Add("M_REVISECASHLOAN", "This is a Cash Loan account. Cash Loan accounts cannot be revised");  // #8489 jec 01/11/11
            _list.Add("M_REVISENOTALLOWED", "Changes that will result in a new delivery note number being generated\\should be made in the Change Order Details screen.");
            _list.Add("M_REVISESCHEDULED", "You do not have the right to revise scheduled items.");
            _list.Add("M_REVISESTORE", "You do not have the right to change StoreCard accounts");
            _list.Add("M_REVISINGSETTLED", "This is a settled account. Changes made may alter the status of the account.");
            _list.Add("M_RETSTOCKINVALID", "Return Item No entered not present on CoSACS - Cannot change Return Item No.");
            _list.Add("M_RETSTOCKNOTPRESENT", "Return Item/Stocklocn not present on CoSACS - Cannot return item");
            _list.Add("M_RETURNEXCEEDSDEL", "Return amount for account {0} \\exceeds delivery total - return not processed.");
            _list.Add("M_RETURNEXCEEDSDELIVERY", "{0} CR > DEL DR");
            _list.Add("M_RETURNVALUE", "Return value cannot be higher than the original value");
            _list.Add("M_REBATEAMOUNTTOOHIGH", "You cannot rebate more than the initial service charge (including previous rebates)");
            _list.Add("M_REVERSALAMOUNTTOOHIGH", "Amount cannot be greater than the total to reverse");
            _list.Add("M_REVERSECANCELSUCCESSFUL", "Cancellation has been sucessfully reversed.");
            _list.Add("M_RFBLOCKED", "Ready finance account is blocked. Access is read only.");
            _list.Add("M_RFCSVFILEEXISTS", "A new RF Card Print file was not generated because the file {0} already exists");
            _list.Add("M_RFCSVFILENOTCREATED", "There were no new RF records created and delivered, so the {0} RF Card Print file {1} was not created");
            _list.Add("M_RFACCOUNTLOCKED", "The RF account {0} for customer {1} is already locked.\\None of the delivered RF accounts can be selected.");
            _list.Add("M_RFLIMITEXCEEDED", "Cash total is higher than your RF credit limit will allow.");
            _list.Add("M_RFRESCOREREQUIRED", "RF account is inactive.\\Credit limit has been reset and account must be re-scored.");
            _list.Add("M_RFSTAGE1", "Sanction Stage 1 needs to be completed for this sub-agreement.");
            _list.Add("M_RFSTAGE2", "Sanction Stage 2 needs to be completed for this sub-agreement.");
            _list.Add("M_RFSUMMARY", "RF Summary");
            _list.Add("M_RFTERMS", "RF Terms and Conditions");
            _list.Add("M_ROWSRETURNED", " Row(s) returned.");
            _list.Add("M_RULEEXISTS", "Unable to add rule. This rule already exists");  //IP - 10/06/10 - CR1083 - Collection Commissions
            _list.Add("M_RULESWRONGCOUNTRY", "Import file does not contain rules for this country.\\Unable to import.");
            _list.Add("M_SAFENOTFOUND", "Floats can only be performed from the safe\\The 'safe' deposit type cannot be found.");
            _list.Add("M_SAFETOSAFE", "It is not possible for both source and destination to be 'Safe'");
            _list.Add("M_SALESSAVEDSUCCESSFUL", "Sales order has been saved successfully.");
            _list.Add("M_SALESSAVEDSUCCESSFULRF", "Sales order has been saved successfully.\n\nOrd/Invoice: {0}");
            _list.Add("M_SAVE", "Save Changes?"); //CR852
            _list.Add("M_SAVED", "Changes Saved"); //68989 20/06/07 SC
            _list.Add("M_SAVECHANGES", "Save changes before closing?");
            _list.Add("M_SAVECHANGES2", "You have changes that have not been saved. Would you like to save these changes before displaying the new selection?");
            _list.Add("M_SAVECHANGESTELEPHONE", "You have made changes to the telephone numbers that have not been saved. Would you like to save these changes?"); //NM & IP - 30/12/08 - CR976
            _list.Add("M_SAVECHECK", "Please make sure account has been selected and action code has been set.");
            _list.Add("M_SAVECLEAR", "Save changes before clearing?");
            _list.Add("M_SAVEDEPOSITS", "Do you wish to save the entries?");
            _list.Add("M_SAVINGACCOUNT", "Saving account ...");
            _list.Add("M_SAVINGPAYMENTS", "Saving payments ...");
            _list.Add("M_SAVINGTRANSACTION", "Saving transaction ...");
            _list.Add("M_SAVECOMMCHANGES", "You have {0} changes that have not been saved. Would you like to save these changes before continuing?");  //CR36
            _list.Add("M_SAVECOMMNOTALLOWED", "You must complete entry or clear entry before saving");  //CR36
            _list.Add("M_SAVETRANSACTIONTYPECHECK", "You do not have the user rights to save a transaction type.");
            _list.Add("M_SCHEDULELOCATION", "Load cannot contain items with different locations");
            _list.Add("M_SCHEDULEOFPAYMENTS", "Schedule of Payments");
            _list.Add("M_SCHEDULEREMOVE", "Account must be DA'ed before changes can be confirmed.");
            _list.Add("M_SEARCHACCOUNT", "Searching for account ...");
            _list.Add("M_SELECTDIFFERENTPAYMETHOD", "Please select a different payment method to continue"); //IP - 03/12/10 - Store Card
            _list.Add("M_SELECTCOLLCOMMNRULEACTION", "Atleast one action must be selected for a rule."); //IP - 09/07/10 - CR1083 - Collection Commissions
            _list.Add("M_SELECTCOLLCOMMNRULEACTION2", "You can not select ALL action with any other action."); //IP - 09/07/10 - CR1083 - Collection Commissions
            _list.Add("M_SEARCHTRANSACTIONS", "Searching for transactions ...");
            _list.Add("M_SELECTACTIONCODE", "Please select an action code");
            _list.Add("M_SELECTUSERROLE", "You must select a user role from the left hand window first");
            _list.Add("M_SELECTROWS", "Please select one or more rows from the list");
            _list.Add("M_SERVICEREQUESTALREADYBOOKED", "Service Request {0} cannot be booked because it is already booked for {1} on {2}.\n\nReview the Service Request to remove the current booking first.");
            _list.Add("M_SETTLETRANSFER", "The Settlement value for account {4} is {0} with a rebate of {1}.\n\nThe Transfer Value -{2} will place the account in CREDIT.\n\nDo you want the Transfer Value to be reduced to the Settlement amount ?\n\n(If you click 'No' then -{2} will be transferred and the account left {3} in CREDIT.)");
            _list.Add("M_SETTLEVALUE", "The Settlement value for account {1} is {0}");
            _list.Add("M_SETNAMEEXISTS", "A set already exists with this name, are you sure you want to overwrite it with your current selections?");
            _list.Add("M_SETUPADDITIONALLETTERS", "Please setup additional letters under Code Maintenance, category LT2"); //IP - 22/12/09 - UAT(946)
            _list.Add("M_SHORTAGE", "A shortage of {0} will be posted to account {1}.");
            _list.Add("M_SIGNATURESAVED", "Signature successfully saved.");
            _list.Add("M_SLIPCHECK", "Checking Slip Printer connected ...");
            _list.Add("M_SLIPCONNECT", "Slip printer not connected or switched off.\n\nClick 'Ignore' if you do NOT require a receipt/payment card to be printed?");
            _list.Add("M_SLIPNOCONNECT", "Slip Printer NOT connected");
            _list.Add("M_SLIPOK", "Slip Printer OK");
            _list.Add("M_SLIPPAPER", "Slip printer needs more paper.\n\nClick 'Ignore' if you do NOT require a receipt/payment card to be printed?");
            _list.Add("M_SLIPPAPEROUT", "Slip Printer paper out");
            _list.Add("M_SOFTSCRIPTCOMMENT", "SOFT SCRIPT COMPLETED ON ");
            _list.Add("M_SOFTSCRIPTSEPARATOR", "-----------------------------------------------------------------------------------------");
            _list.Add("M_SPACEORNULL", "Contains Space OR NULL Value");
            _list.Add("M_SPACONSOLIDATEDMSG", "An odd payment amount can only be entered against an account that has an arrangement amount entered."); //IP & JC - 13/01/09 - CR976
            _list.Add("M_SPADETAILSREQUIRED", "You must press the 'SPA' button and enter additional information before you can save a SPA action");
            _list.Add("M_STAGE2SAVED", "Stage 2 details successfully saved.");
            _list.Add("M_STARTDATEAFTEREND", "The Start Date cannot be after the End Date");
            _list.Add("M_STARTDATEPAST", "Date range modifications cannot be made with a start date that is less than today's date");
            _list.Add("M_STOCKITEMLOCKED", "Low Stock.  User {0} ({1}) has got this item currently locked");
            _list.Add("M_STOCKONORDERAUTH", "Authorisation is required to sell items on purchase order.");                           //IP - 06/07/11 - RI - #3974
            _list.Add("M_STOCKNOTPRESENT", "Item/Stocklocn not present on CoSACS - Cannot return item");
            _list.Add("M_STORETYPE", "You must select the Store Type");     //CR903  jec
            _list.Add("M_SCARDQUALMINMTHSHIST", "Minimum months account history should be less than the number of months to check in");   //IP - #10073 - 02/05/12
            _list.Add("M_SCARDQUALMAXPREVMTHSARRS", "Maximum previous months in arrears should be less than the number of months to check in");   //IP - #10073 - 02/05/12
            _list.Add("M_SYSTEMCLOSED", "CoSACS is currently CLOSED.  Please try again later.");
            _list.Add("M_T2NODISCOUNTCODE", "This customer is eligible for a loyalty discount, but the item code is not set up in Country Maintenance.\n\nThe loyalty discount cannot be added!\n\nYou should cancel your changes and wait for the loyalty discount code to be set up.");
            _list.Add("M_TAXINVOICE", "Tax Invoice");
            _list.Add("M_TENDERED", "The Tendered amount must be at least as big as the Payment amount");
            _list.Add("M_TECHNICIANEXISTS", "There is already a technician with this name.  Do you wish to overwrite these details?");
            _list.Add("M_TECHNICIANAPPLYTOALL", "Are you sure you wish to apply these details to all technicians?");
            _list.Add("M_TECHNICIANEXCEEDSCALLSPERDAY", "There are {0} technicians that have slots booked that exceed this amout of calls per day.\nThe technician with the greatest number of slots booked for a day is: {1}");
            _list.Add("M_TECHNICIANSLOTSEXCEEDED", "This technician has slots allocated and cannot be deleted.");
            _list.Add("M_TECHNICIANINACTIVE", "Are you sure you wish to make this technician inactive?");     //IP - 14/02/11 - Sprint 5.10 - #2975
            _list.Add("M_TECHNICIANACTIVATE", "Are you sure you wish to activate this technician?");          //IP - 14/02/11 - Sprint 5.10 - #2975
            _list.Add("M_TELEPHONEUPDATED", "Telephone numbers have been updated successfully");
            _list.Add("M_TEST", "this is {0} is a test {1} message with \a line break {2} in it.");
            _list.Add("M_TERMSTYPELEN", "Terms Type must be 2 characters.");
            _list.Add("M_TERMINATED", "Employee {0} can not be terminated as they have outstanding transactions to be totalled."); //cr1117
            _list.Add("M_TIMEAM", "AM");
            _list.Add("M_TIMEPM", "PM");
            _list.Add("M_TITLETEXT", "CoSaCS   {1} ({0})   Branch {2}   [{3}]");
            _list.Add("M_TOOMANYITEMS", "Only one replacement item (and optionally a discount) may be added.");
            _list.Add("M_TRANSACTIONSLISTED", " Transaction(s) listed.");
            _list.Add("M_TRANSACTIONSAVEDPRINT", "Transaction saved. Printing transaction ...");
            _list.Add("M_FTRANSACTIONTYPECHECK", "Invalid Transaction Code.  Please enter a valid code before saving.");
            _list.Add("M_TRANSACTIONTYPESAVE", "Interface and Corresponding account numbers should be blank or filled.");
            _list.Add("M_TRANSACTIONTYPETRANS", "Invalid Transaction Type.  Please enter a valid transaction type before saving.");
            _list.Add("M_TRANSFERSUCCESSFUL", "Transfer Completed Successfully.");
            _list.Add("M_SUNDRYNOVALUE", "There is no balance to transfer from Sundry Credit.");
            _list.Add("M_TRANSFERTOOHIGH", "The value that you want to transfer, {0}\\is greater than the available value, {1}\\\\This value can not be transfered.");
            _list.Add("M_TRANSPORTLISTED", " Transport entries listed.");
            _list.Add("M_TRANSPORTSCHEDULE", " Transport Schedule.");
            _list.Add("M_TRANSPORTZERO", "No Transport data found.");
            _list.Add("M_TTBANDOVERLAP", "The bands overlap or have a gap. Please check each points range and ensure the lowest starts at zero.");
            _list.Add("M_MMILEVELOVERLAP", "The level overlap or have a gap. Please check each points range and ensure the lowest starts at zero.");
            _list.Add("M_UNKNOWN", "Unknown");
            _list.Add("M_UNPAID", "Payment outstanding - Do you wish to continue ?");
            _list.Add("M_UNPAID2", "Payment outstanding - unable to authorise");
            _list.Add("M_UNPRINTEDTRANSACTIONS", "Unprinted transactions for account number {0}.\\\\Please enter payment card for printing at row number {1}.");
            _list.Add("M_UNSANCTIONEDRF", "Unable to create RF account. \\Existing RF account(s) must be sanctioned.");
            _list.Add("M_UPDATEAVAILABLE", "Application update available.\\\\Application will terminate and launch installer.");
            _list.Add("M_UPDATEERROR", "Problem with automatic updates. Please check config file settings.");
            _list.Add("M_UPDATEKIT", "You cannot update the predefined contents of a kit");
            _list.Add("M_VALIDEXTRA", "Each Extra Payment must be between zero and the Outstanding Balance");
            _list.Add("M_VALIDFEE", "Each Fee must be between zero and the Payment figure");
            _list.Add("M_VALIDPAYMENT", "Each Payment value must be greater than or equal to zero");
            _list.Add("M_VALIDPAYMENTFEE", "Each Fee must be between zero and the Payment figure,\\and each Payment value must be greater than or equal to zero.");
            _list.Add("M_VALIDRETITEM", "Returned waranty number is invalid.  Item must begin with 19 or XW");
            _list.Add("M_VALUEBETWEEN", "Enter a value from {0} to {1}");
            _list.Add("M_VALUETOOLARGE", "The value entered is too large");         //69571 jec 11/02/08
            _list.Add("M_VOUCHEREXISTS", "Voucher already exists");
            _list.Add("M_WARNING", "Warning");
            _list.Add("M_WARRANTYCONTRACT", "Warranty Contract");
            _list.Add("M_FREEWARRANTYCONTRACT", "Free Warranty Contract");
            _list.Add("M_WARRANTYEXPIRY", "{0} months following the manufacturer's warranty.");
            _list.Add("M_WARRANTYFULFILLED", "This warranty has been fulfilled and may not be collected");
            _list.Add("M_WARRANTYNOTPRESENT", "Returned Warranty Code/Stocklocn not present on CoSACS - Cannot return item");
            _list.Add("M_WORKLISTEMPLOYEE", "At least one employee type should be selected.");
            _list.Add("M_WRONGVERSION", "Client and Server versions are different.\\Please ensure the correct versions have been installed.");
            _list.Add("M_WRONGTIME", "Your Client computer has a different time and/or date to the Server.\\\\In order to log on the two machines must have the same time (to within 10 minutes).");
            _list.Add("M_ZEROROWS", "0 Row(s) returned.");
            _list.Add("M_CUSTID_EXISTS", "The Customer ID is taken, please use a different Customer ID");
            _list.Add("M_MISSINGPRINTPROPERTIES", "The following required print types have not been \\ fully specified. OK to proceed? \\\\ {0}");
            _list.Add("M_PASSWORDCHECKFAILED", "The new password entered has not been confirmed.");
            _list.Add("M_PASSWORDCHECKNUMERIC", "Password should be a numeric value.");
            _list.Add("M_PASSWORDCHECKSAME", "New password should not be the same as old password.");
            _list.Add("M_PASSWORDCHECKSAVE", "New password should not be blank or have leading spaces.");
            _list.Add("M_PASSWORDCHANGEFAILED", "Invalid password change.");
            _list.Add("M_PASSWORDCHANGEREQUIRESLOGIN", "Please log in to change your password.");
            _list.Add("M_PASSWORDCONFIRMMISMATCH", "The confirm password doesn't match the new password");
            _list.Add("M_PASSWORDMINLENGTH", "The password must be atleast {0} characters long");
            _list.Add("M_OLDPASSWORDCHECKFAILED", "The old password entered is incorrect.");
            _list.Add("M_SAVEPASSWORDCHECK", "Cannot save password as there are errors with Old/New Passwords entered.");
            _list.Add("M_NOHOMEADDRESS", "You must enter a home address.");
            _list.Add("M_PAYMENTTOOLOW", "Payment amount entered is not sufficient.");
            _list.Add("M_EXCELNOTFOUND", "Microsoft Excel not found. CSV file written to \\{0}.");
            _list.Add("M_DELDATEACCTOPENED", "Delivery Date cannot be before date account was opened.");
            _list.Add("M_DELDATEPAST", "Delivery Date must be in the past.");
            _list.Add("M_DELDATERIGHT", "You do not have the right to set delivery date to greater than a week.");
            _list.Add("M_DELDATEVALID", "Delivery Date must be within a valid range in the past.");
            _list.Add("M_COLLDATEPAST", "Collection Date must be in the future.");
            _list.Add("M_REFER", "Account has been Referred: {0}"); //IP - 14/03/11 - #3314 - CR1245 - Display the referral reasons in the referral popup
            _list.Add("M_ACCEPT", "Account has been Accepted.");
            _list.Add("M_REJECT", "Account has been Rejected: {0}"); //IP - 15/03/11 - #3314 - CR1245 - Display any rejection reasons plus referral reasons in the rejection popup
            _list.Add("M_ACCEPTRF", "Account has been Accepted with a Spending Limit of {0}.");
            _list.Add("M_CHANGEPASSWORD", "Your Password has expired.  Do you wish to change it now.");
            _list.Add("M_REPCRED", "Not all Repossession Credit/Replacement values have been entered.");
            _list.Add("M_DECISIONSCHECK", "Decision must be given before completing this stage.");
            _list.Add("M_SCORERESULT", "Credit Score Result: {0}  ");
            _list.Add("M_CASHIERTOTALS", "Cashier Totals");
            _list.Add("M_NOTOTALREVERSE", "There are no Cashier Totals under ten days old that can be reversed");
            _list.Add("M_INVALIDEMPNO", "Employee No. must be a valid numeric value");
            _list.Add("M_RFORHPINVALID", "If the 'Cash' Account Type is selected, you cannot also select 'Ready Finance' or 'Hire Purchase'");
            _list.Add("M_CASHINVALID", "If either of the 'Ready Finance' or 'Hire Purchase' Account Types are selected, you cannot also select 'Cash'");
            _list.Add("M_STCONLY", "If the 'STC' Account Type is selected, no other Account Type can be selected with it");
            _list.Add("M_WARRANTYRENEWALPROMPT", "Warranty renewal available on product: \n{0}\n Please ask customer to go to the sales persons for continued cover on items.");
            _list.Add("M_NOWARRANTYRENEWALPROMPT", "The customer has no available warranty renewals.");
            _list.Add("M_REALLOCATINGISSUEDTEMPRECEIPTS", "{0} of the temporary receipts that you wish to allocate have already been issued.");
            _list.Add("M_TEMPRECEIPTSALREADYAlLLOCATED", "Temporary receipts that you wish to allocate have already been allocated.");
            _list.Add("M_TEMPRECEIPTSALAlLLOCATED", "Temporary receipts {0} to {1} have successfully been allocate.");
            _list.Add("M_NOREPOSSREDEL", "No repossessions to process for the account.");
            _list.Add("M_INVALIDVOIDTEMPRECEIPT", "Cannot remove as receipt is already issued.");
            _list.Add("M_LOADEXISTS", "Load already exisits.");
            _list.Add("M_LOADREMOVECOMMREQUIRED", "A comment is required to remove the load from the schedule.");
            _list.Add("M_NEEDDNPRINTED", "Delivery note needs to be printed before notification");
            _list.Add("M_CHANGEDATE", "Change 'Date From' to add new commission rate");
            _list.Add("M_RATEENDED", "Commission rate ended - no changes allowed");
            _list.Add("M_NEWRATE", "A subsequent commission rate has been added - no changes allowed");
            _list.Add("M_MANDATORYANDINVISIBLEDISABLED", "A field cannot be set to mandatory and either invisible or disabled");
            _list.Add("M_INVISIBLEANDENABLED", "A field cannot be set to invisible and enabled");
            _list.Add("M_PRODCAT", "Product Category cannot be greater than 3 characters.");    // RI jec 08/07/11
            _list.Add("M_INSTALMENTAMT", "Please enter an instalment amount."); //IP - 03/10/08 - Special Arrangements (Credit Collections)
            _list.Add("M_ARRANGEMENTGREATERARREARS", "The arrangement amount cannot be greater than the current balance."); //IP - 03/10/08 - Special Arrangements (Credit Collections)
            _list.Add("M_MINNOTESLENGTH", "The minimum notes length required to successfully save the action is {0}"); //NM & IP - 29/12/08 - CR976
            _list.Add("M_MAXDAYSALLOWED", "The reminder date cannot be later {0} days from today."); //NM & IP - 05/01/09 - CR976
            _list.Add("M_LEGDETAILSREQUIRED", "Please enter legal details"); //NM & IP - 07/01/09 - CR976
            _list.Add("M_INSDETAILSREQUIRED", "Please enter insurance details"); //NM & IP - 07/01/09 - CR976
            _list.Add("M_FRDDETAILSREQUIRED", "Please enter fraud details"); //NM & IP - 07/01/09 - CR976
            _list.Add("M_INCORRECTDATE", "Date cannot be < todays date"); //IP & JC  CR976
            _list.Add("M_DATEWARNING", "Date moved to match stock availaibility"); //FA UAT 665
            _list.Add("M_PAID_AND_TAKEN_DISALLOWED", "This is a Paid and Taken Account, for Refunds and Corrections please use Cash and Go Returns screen");     //IP - 08/02/10 - CR1037 Merged - Malaysia Enhancements (CR1072)
            _list.Add("M_TOOMANYADDRESSES", "Too many addresses, please remove any of the existing addresses to add new one"); //uat(4.3) - 161 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            //_list.Add("M_SCOREAPPBANDWRONG", "Application Scoring band must be in A - H"); //CR1034 SC //IP - 09/04/10 - CR1034 - Removed
            //_list.Add("M_SCOREBHBANDWRONG", "Behavioural Scoring band must be in I - P"); //CR1034 SC //IP - 09/04/10 - CR1034 - Removed
            _list.Add("M_DUPLICATESERIALNO", "Model number and serial no. have already been entered on another item.");
            _list.Add("M_AmountPaid", "Please check the payment amount for errors.");
            _list.Add("M_CardError", "Please check the card entered for errors.");
            _list.Add("M_SCOREAPPBANDWRONG", "Application Scoring band must be in A - H"); //CR1034 SC
            _list.Add("M_SCOREBHBANDWRONG", "Behavioural Scoring band must be in 1 - 9"); //CR1034 SC
            _list.Add("M_SoreCardErrors", "Please check store card details for errors."); //IP - 02/12/10 - Store Card
            _list.Add("M_RUNSTORECARDSTATEMENTS", "It has been at least {0} days since Store Card Statements were last run. Do you wish to run this option ?");  //#12341 - CR11571
            _list.Add("M_WARRANTYREFUND", "Please note the refund amount has been adjusted to reflect the used portion of the warranty"); //#16607
            _list.Add("M_CASHLOANDISBURSEMENTPAYADMIN", "A payment of {0} must be processed for the Admin Charge");
            _list.Add("M_INVALIDCOORDINATE", "Please enter a valid co-ordinate detail i.e. Latitude,Longitude"); // Address Standardization CR2019 - 025
            _list.Add("M_CHECKVALIDATION", "Please check for matrix validation errors");
            _list.Add("M_PAYMENTVALIDATION", "Payment amount cannot be greater than settlement figure.");
            _list.Add("M_NOMULTIPAYMENT", "You can make payment for one account at a time.");
            _list.Add("M_VALIDDEPOSIT", "Deposit amount cannot be changed.");
            _list.Add("M_TENDER", "Please Enter Total Received Amount.");
            _list.Add("M_VALIDTENDER", "Please Enter Cash/Cheque Amount less than or equal to Total Received Amount.");
            _list.Add("M_USEDGIFTVOUCHER", "Voucher already associated with an account.");
            _list.Add("M_INVALIDDEPOSIT", "ERROR: Please enter the right DOWNPAYMENT AMOUNT {0} | Transaction Fail try again.");
            _list.Add("M_TENDEVSTOTAL", "Total Received Amount cannot be less than Total Amount.");
            _list.Add("M_USEDTOTALCASH", "Total of Cash Amount cannot be less than Used Amount.");
            _list.Add("M_USEDTOTALCHEQUE", "Total of Cheque Amount cannot be less than Used Amount.");
            _list.Add("M_SELPAYMETHOD", "Please select Payment Method.");
            _list.Add("M_CASHAMOUNT", "Please enter Cash Amount.");
            _list.Add("M_CHEQUEAMOUNT", "Please enter Cheque Amount.");
            #endregion
            //
            // Context menu items (Popups)
            //
            #region Context menu items (Popups)
            _list.Add("P_ACCOUNT_DETAILS", "Account Details");
            _list.Add("P_ADDTOACCOUNT", "Add To Account");
            _list.Add("P_ADDSPIFF", "Add Additional SPIFF");
            _list.Add("P_ADDFAILURE_NOTES", "Add Notes");       //#10358
            _list.Add("P_ALLOCATE_TECH", "Allocate Technician");
            _list.Add("P_ALLOCATE_SR", "Allocate SR");      //jec 19/01/11
            _list.Add("P_ALLOCATION_HISTORY", "Allocation History");
            _list.Add("P_ASSIGN_APP2", "Assign as second applicant");
            _list.Add("P_ASSOCIATE", "Associate with Account");
            _list.Add("P_AUDIT", "Audit Details");
            _list.Add("P_BOOK_TECH", "Book Technician");
            _list.Add("P_CANCEL_ACCOUNT", "Cancel Account");
            _list.Add("P_CANCEL_SERVICE_CASH_ACCT", "Cancel Service Cash Account"); //IP - 10/02/11 - Sprint 5.10 - #2978
            _list.Add("P_CANCEL_COLLECTION_NOTE", "Cancel Collection Notes");        // #13644
            _list.Add("P_CASH_GO_REPRINT", "Reprint Receipt");                      //IP - 08/05/12 - #9608 - CR8520
            _list.Add("P_CASH_GO_RETURN", "Cash and Go Return");
            _list.Add("P_CASHTILLHISTORY", "Cash Till History");
            _list.Add("P_CHANGE_LOCATION", "Change Item Location");
            _list.Add("P_CHANGE_ORDER_DETAILS", "Change Order Details");        // # 10327
            _list.Add("P_CLEARFLAG", "Clear Flag");
            _list.Add("P_CLEARPROP", "Clear Proposal");
            _list.Add("P_COMMENTS", "View Customer Notes/Comments");
            _list.Add("P_COPY", "Copy");
            _list.Add("P_NOCOPY", "Do not copy");
            _list.Add("P_CREATERF", "Create New RF Account");
            _list.Add("P_CUSTDETAILS", "Customer Details");
            _list.Add("P_CUST_INTERACT_DETAILS", "Customer Interaction Details");
            _list.Add("P_DELETE", "Delete");
            _list.Add("P_EDIT", "Edit");
            _list.Add("P_EDITCONTRACTNO", "Edit Contract Number");
            _list.Add("P_EDITRETURNDATE", "Edit Expected Return Date");
            _list.Add("P_EDITSANCTIONSTAGES", "Edit Sanction Stages");
            _list.Add("P_EDITWARRANTY", "Add/Edit Warranty");
            _list.Add("P_AddAssociated", "Add Associated/Installation");
            _list.Add("P_ENQUIREBY", "Enquire By...");
            _list.Add("P_ENTERLINEITEMS", "Enter Line Items");
            _list.Add("P_ERRORDATE", "Error Date");
            _list.Add("P_ERRORTEXT", "Error Text");
            _list.Add("P_FINANCIAL", "Financial Interface Report");
            _list.Add("P_FOLLOWUP", "Follow Up Action Details");
            _list.Add("P_INSTALLATION", "Installation");
            _list.Add("P_ITEMSONDELNOTE", "Line Items on Delivery Note");
            _list.Add("P_ITEMSFORACCOUNT", "Line Items for Account");
            _list.Add("P_LOCATION", "Location");
            _list.Add("P_MANDATE_DETAILS", "Mandate Details");
            _list.Add("P_PRINTPCARD", "Print Payment Card");
            _list.Add("P_PRINT_INVOICE", "Print Invoice");
            _list.Add("P_PRODUCT", "Product");
            _list.Add("P_REFERREJECTED", "Refer Rejected Accounts");
            _list.Add("P_UNARCHIVE", "Un-archive"); //IP - 30/01/09 - CR971
            _list.Add("P_CANCEL_REDELIVERY", "Cancel Re-Delivery");        // #13675
            _list.Add("P_REVERSEADDTO", "Reverse Add To");
            _list.Add("P_REVERSETRANSACTION", "Reverse Transaction");
            _list.Add("P_REMOVE", "Remove");
            _list.Add("P_REMOVE_DEPOSIT", "Service Request no longer requires deposit");
            _list.Add("P_REMOVEITEM", "Remove Item");
            _list.Add("P_REMOVESPIFF", "Remove SPIFF");
            _list.Add("P_REPRINT", "Re-print");
            _list.Add("P_REVERSEDEPOSIT", "Reverse safe transfer");
            _list.Add("P_REVISE_ACCOUNT", "Revise Account");
            _list.Add("P_REVISE_ORDER", "Revise Order");        // #10221            
            _list.Add("P_SANCTIONS1", "Sanction Stage 1");
            _list.Add("P_SANCTIONDC", "Document Confirmation");
            _list.Add("P_SERVICE_REQUEST", "Service Request");
            _list.Add("P_SEVERITY", "Severity");
            _list.Add("P_STORE_CARD", "Store Card");
            _list.Add("P_SUBMIT_BOOKING", "Submit Shipment");  //#15469      // #10221
            _list.Add("P_UNSETTLE", "Unsettle"); //IP - 30/01/09 - CR971
            _list.Add("P_VIEW", "View");
            _list.Add("P_VIEW_COMMISSIONS", "View Commissions");
            _list.Add("P_VIEWERRORS", "View Errors");
            _list.Add("P_VIEWPROP", "View Proposal");
            _list.Add("P_VIEWSCHEDULE", "View Schedule Details");
            _list.Add("P_VIEWSMS", "View SMS Text");
            _list.Add("P_VIEWSUMMARY", "View Summary");
            _list.Add("P_VIEWVALUES", "View Values");
            _list.Add("P_WARRANTY_REPLACE", "Instant Replacement");
            _list.Add("P_WRITEOFF_SERVICE_CASH_ACCT", "Write-Off Service Cash Account"); //IP - 08/02/11 - Sprint 5.10 - #2977
            #endregion
            //
            // Titles
            //
            #region Titles
            _list.Add("T_ACCUMCHARGESAPPLIED", "Accumulated Charges Applied");
            _list.Add("T_ACCTCAT", "Acct Category");
            _list.Add("T_ACCTCATDESC", "Acct Category Desc");
            _list.Add("T_ACCOUNTBALANCE", "Account Balance");
            _list.Add("T_ACCOUNTNO", "Account No");
            _list.Add("T_ACCTNO", "Account No");
            _list.Add("T_ACCOUNTNUMBER", "Account Number");
            _list.Add("T_ACCOUNTTYPE", "Acct Type");
            _list.Add("T_ACTION", "Action");
            _list.Add("T_ACTIONNO", "Action No");
            _list.Add("T_ACTIONCODE", "Action Code");
            _list.Add("T_ACTIONREQUIRED", "Action Required");  //CR 949/958
            _list.Add("T_ACTIONVALUE", "Action Value");
            _list.Add("T_ACTIVITYID", "Activity ID");
            _list.Add("T_ACTTYPERF", "RF");
            _list.Add("T_ACTTYPEHP", "HP");
            _list.Add("T_ACTTYPECASH", "CASH");
            _list.Add("T_ACTUAL", "Actual");
            _list.Add("T_ACTUALCOST", "Actual Cost");
            _list.Add("T_ACTUALSTOCK", "Actual Stock");
            _list.Add("T_ADDEDBY", "Added By");
            _list.Add("T_ADDITIONS", "AMENDED PICKLIST");
            _list.Add("T_ADDITIONAL", "Additional"); // IP - 09/11/09 - CoSACS Improvements
            _list.Add("T_ADDITIONAL2", "Additional2"); // IP - 07/12/11 - CR1234
            _list.Add("T_ADDITIONALCOST", "Additional Cost");
            _list.Add("T_ADDITIONALLIMIT", "Additional Value");
            _list.Add("T_ADDITIONALPERCENT", "Additional %");
            _list.Add("T_ADDRESS", "Address");
            _list.Add("T_ADDRESS1", "Address1");
            _list.Add("T_ADDRESS2", "Address2");
            _list.Add("T_ADDRESS3", "Address3");
            _list.Add("T_ADDTOPOTENTIAL", "Add To Potential");
            _list.Add("T_ADMINPERC", "Admin %");
            _list.Add("T_ADMINVALUE", "Admin Value");
            _list.Add("T_ADVANCE", "Advance");
            _list.Add("T_AGREEDUEDATE", "Agrmt And Due Date Changed");
            _list.Add("T_AGREEMENTNO", "Agreement No");
            _list.Add("T_AGREEREVISED", "Agreements Revised");
            _list.Add("T_AGREETOTAL", "Agreement Total");
            _list.Add("T_ALLOCACCOUNTS", "Allocated Accounts");
            _list.Add("T_ALLOCNO", "Alloc No");
            _list.Add("T_ALLOCATEDFLOAT", "Allocated Float");
            _list.Add("T_ALLOCATEDEMPLOYEE", "Allocated Employee");
            _list.Add("T_ALLOCATEDBY", "Allocated By");
            _list.Add("T_ALLOCPC", "Alloc %");
            _list.Add("T_ALLOWABLE", "Allowable");
            _list.Add("T_ALLOW_CHARGEACCT_CANCELLATION", "Allow Charge Account Cancellation");   //IP - 15/02/11 - Sprint 5.10 - #2977
            _list.Add("T_ALLOW_CHARGEACCT_WRITEOFF", "Allow Charge Account Writeoff");   //IP - 15/02/11 - Sprint 5.10 - #2977
            _list.Add("T_AMOUNT", "Amount: ");
            _list.Add("T_AMOUNT1", "Amount");
            _list.Add("T_AMOUNTDISBURSED", "Amount Disbursed: ");                       //IP - 12/10/11 - #3921 - CR1232
            _list.Add("T_AMOUNTBANKED", "Amount Banked");
            _list.Add("T_AMOUNTTENDERED", "Amount Tendered: ");
            _list.Add("T_AMOUNTTOSAFE", "Amount to Safe");
            _list.Add("T_AMT", "Amt");
            _list.Add("T_ARCHIVED", "Archived"); //IP - CR971
            _list.Add("T_ARRANGEMENTAMOUNT", "Arr Inst Amount"); //IP & JC - CR976
            _list.Add("T_ARREARS", "Arrears");
            _list.Add("T_ARREARSLEVEL", "Arrears Level");
            _list.Add("T_ARREARSEXCHARGES", "Arrears Excluding Charges");
            _list.Add("T_ARREARSREPOPC", "Arrears Repo %");
            _list.Add("T_ARRANGEMENTDATE", "Arrangement Date");
            _list.Add("T_ASSEMBLY", "Assembly");
            _list.Add("T_ASSIGNED", "Assigned");
            _list.Add("T_AVAILABLESPEND", "RF Available Spend");
            _list.Add("T_AVAILABLESPEND2", "Available Spend");
            _list.Add("T_AVAILSTOCK", "Available Stock");
            _list.Add("T_BAILFEE", "Bail Fee");
            _list.Add("T_BAILREVIEW", "Bailiff Review");
            _list.Add("T_BALAFTER", "Balance After Repo ");
            _list.Add("T_BALANCE", "Balance: ");
            _list.Add("T_BALANCE1", "Balance");
            _list.Add("T_BALANCE2", "Bal");
            _list.Add("T_BALANCETOTAL", "Balance Total");
            _list.Add("T_BALANCEEXCLINT", "Balance Excluding Interest ");
            _list.Add("T_BALBEFORE", "Balance Before Repo ");
            _list.Add("T_BAND", "Band");
            _list.Add("T_BANKORDER", "Bank Order");
            _list.Add("T_BANKNAME", "Bank Name");
            _list.Add("T_BANKACCTNO", "Bank Account No.");
            _list.Add("T_BDWBALANCE", "BDW Balance");
            _list.Add("T_BDWCHARGES", "BDW Charges");
            _list.Add("T_BOOKINGSBALANCE", "Balance");
            _list.Add("T_BOOKINGSCANCELLED", "Cancelled");
            _list.Add("T_BOOKINGSSALESPERSON", "Salesperson Name");
            _list.Add("T_BOOKINGSSALESPERSONID", "Salesperson ID");  // 67915 RD 15/03/06 Added to split 'Salesperson" into two columns: "Salesperson ID" and "Salesperson Name"
            _list.Add("T_BOOKINGSSALESTAX", "Sales Tax");
            _list.Add("T_BOOKINGSSUPERSHIELD", "SupaShield");
            _list.Add("T_BY", "By");
            _list.Add("T_BRANCH", "Branch");
            _list.Add("T_BRANCHNO", "Branch Number");               //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
            _list.Add("T_BRANCHADDRESS", "Branch Address");
            _list.Add("T_BUFFNO", "Delivery Note Number");
            _list.Add("T_BUFFNO_2", "Buff No");
            _list.Add("T_BUFFBRANCHNO", "Buff Branch No");
            _list.Add("T_CASHIERNAME", "Cashier Name");
            _list.Add("T_CASHIER", "Cashier:");                     //IP - 17/05/12 - #9447 - CR1239
            _list.Add("T_CATEGORY", "Category");
            _list.Add("T_CASHIERID", "Cashier ID:");
            _list.Add("T_CASHIEROVERAGE", "Overage");
            _list.Add("T_CASHIERSHORTAGE", "Shortage");
            _list.Add("T_CASHIERTOTAL", "Cashier Total");
            _list.Add("T_CASHLOAN", "Cash Loan");
            _list.Add("T_CASHPRICE", "Cash Price");
            _list.Add("T_CHANGEDBY", "Changed By");
            _list.Add("T_CHANGEGIVEN", "Change Given: ");
            _list.Add("T_CHARGES", "Interest");
            _list.Add("T_CHARGETO", "Charge To");
            _list.Add("T_CHARGETOPARTSCOURTS", "Parts - Courts");
            _list.Add("T_CHARGETOPARTSOTHER", "Parts - Other");
            _list.Add("T_CHARGETOPARTSTOTAL", "Parts - Total");
            _list.Add("T_CHARGETOTOTALLABOUR", "Total Labour");
            _list.Add("T_CHARGETOTOTALCOST", "Total Cost");
            _list.Add("T_CHEQUE", "Chq");
            _list.Add("T_CHEQUENO", "Cheque No");
            _list.Add("T_CHEQUENOFIN", "Cheque No/From To A/C"); //IP - 69305 - 23/11/2007
            _list.Add("T_CITY", "City");
            _list.Add("T_CLAIM", "Claim"); // CR 822 Added [PC] 29-Sep-2006 
            _list.Add("T_CLAIMNO", "Claim No");
            _list.Add("T_CLEAREDBY", "Cleared By");
            _list.Add("T_CODE", "Code");
            _list.Add("T_COLLECTED", "Collected");
            _list.Add("T_COLLDATE", "Collection Date");
            _list.Add("T_COLLECTIONPC", "Coll %");
            _list.Add("T_COLLECTIONREASON", "Collection Reason"); // CR 822 Added [PC] 03-Oct-2006 
            _list.Add("T_COMMISSIONDUE", "Commission Due");
            _list.Add("T_COMMISSIONPC", "Comm %");
            _list.Add("T_CONTRACTNO", "Contract No");
            _list.Add("T_COST", "Cost");
            _list.Add("T_COUNTRYCODE", "Country Code");
            _list.Add("T_COUNTRY", "Country");
            _list.Add("T_COUNTTYPE1", "Count Type 1");
            _list.Add("T_COUNTTYPE2", "Count Type 2");
            _list.Add("T_COUNTVALUE", "Count Value");
            _list.Add("T_COURTS", "Courts");
            _list.Add("T_COURTSCODE", "Courts Code");           //IP - 07/07/11 - CR1254 - RI - #4018
            _list.Add("T_CR", "CR");
            _list.Add("T_CREATECASH", "Create CASH Account");
            _list.Add("T_CREATEHP", "Create HP Account");
            _list.Add("T_CREATERF", "Create RF Account");
            _list.Add("T_CREATESTORE", "Create Store Card Account");
            _list.Add("T_CREATESUB", "Create sub-agreement");
            _list.Add("T_CREDITLIMIT", "Credit Limit");
            _list.Add("T_CURRENCY", "Currency");
            _list.Add("T_CUSTID", "Customer ID");
            _list.Add("T_CUSTOMER", "Customer");
            _list.Add("T_CUSTOMER_CHARGE_TO_ACCT", "Customer Charge To Account");   //IP - 10/02/11 - Sprint 5.10 - #2978
            _list.Add("T_CUSTSIGNATURE", "Customers Signature");    //IP - 07/01/11 - Store Card
            _list.Add("T_CUSTOMERNAME", "Customer Name");
            _list.Add("T_CUSTOMERNOTES", "Customer Notes");
            _list.Add("T_DAED", "DA'ed");
            _list.Add("T_DAEDBY", "DA'ed By");
            _list.Add("T_DAMAGESTOCK", "Damaged Stock");
            _list.Add("T_DATEADDED", "Date Added");
            _list.Add("T_DATE", "Date");
            _list.Add("T_DATEALLOC", "Date Allocated");
            _list.Add("T_DATEAUTH", "Date Authorised");
            _list.Add("T_DATEBOOKED", "Date Booked");
            _list.Add("T_DATECHANGED", "Date Changed");
            _list.Add("T_DATECLOSED", "Date Closed");
            _list.Add("T_DATEDEALLOC", "Date Deallocated");
            _list.Add("T_DATEDEL", "Date of Delivery");
            _list.Add("T_DATEDELIVERED", "(null)");
            _list.Add("T_DATEDELPLAN", "Planned Delivery Date");
            _list.Add("T_DATEDEPOSIT", "Date of Deposit");
            _list.Add("T_DATEDNPRINTED", "Date DN Printed");
            _list.Add("T_DATEDUE", "Date Due");
            _list.Add("T_DATEEXPIRY", "Date of Expiry");
            _list.Add("T_DATEFINISH", "Date Finish");
            _list.Add("T_DATEFIRST", "Date First");
            _list.Add("T_DATEFROM", "Date From");
            _list.Add("T_DATEISSUED", "Date Issued");
            _list.Add("T_DATELASTCHANGED", "Date Last Changed");
            _list.Add("T_DATELASTPAID", "Date Last Paid");
            _list.Add("T_DATELOGGED", "Date Logged");
            _list.Add("T_DATENEXTDUE", "Date next Charge/Letter due");
            _list.Add("T_DATEOPENED", "Date Opened");
            _list.Add("T_DATEORDERED", "Date Ordered");
            _list.Add("T_DATEPAID", "Date Paid");
            _list.Add("T_DATEPRINTED", "Date Printed");
            _list.Add("T_DATEPRINTED2", "Date Printed:");               //IP - 17/05/12 - #9447 - CR1239
            _list.Add("T_DATEREPRINTED", "Date Re-Printed:");           //IP - 09/05/12 - #9608 - CR8520
            _list.Add("T_REMOVEDDELETEDDATE", "Removed/Deleted Date"); //IP - 18/11/09 - CR929 & 974 - Audit
            _list.Add("T_DATEREVISED", "Date Revised");
            _list.Add("T_DATERUN", "Date Run");
            _list.Add("T_DATESENT", "Date Sent");
            _list.Add("T_DATESTART", "Date Start");
            _list.Add("T_DATESTATCHANGED", "Date Status changed");
            _list.Add("T_DATETAKEN", "Date Taken");
            _list.Add("T_DATETO", "Date To");
            _list.Add("T_DDPENDING", "Giro Pending");
            _list.Add("T_DDRANOTREPRESENT", "Not Represent");
            _list.Add("T_DDRAREPRESENT", "Represent");
            _list.Add("T_DDRACANCEL", "Cancel");
            _list.Add("T_DEALLOCATEDBY", "Deallocated By");
            _list.Add("T_DEBIT", "Debit");
            _list.Add("T_DEFAULTS", "No. of Defaults");
            _list.Add("T_DEFAULTSBAL", "Balance of Defaults");
            _list.Add("T_DEFAULTSEXMOTOR", "No. of Defaults (excluding motor)");
            _list.Add("T_DEFAULTSEXMOTORBAL", "Balance of Defaults (excluding motor)");
            _list.Add("T_DEFERREDTERMSAMOUNT", "Deferred Terms Amount");
            _list.Add("T_DELADDRESS", "Del Address"); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("T_REASONDELETEDN", "Delete Delivery Note"); //IP - 18/02/09 - CR929 & 974
            _list.Add("T_DELETEDITEM", "Deleted Product"); //69647
            _list.Add("T_DELIVERCOLLECT", "Delivery/Collection");
            _list.Add("T_DELIVERED", "Delivered");
            _list.Add("T_DELIVEREDQTY", "Delivered Quantity");
            _list.Add("T_DELIVERER", "Deliverer");
            _list.Add("T_DELIVERYADDRESS", "Delivery Address");
            _list.Add("T_DELIVERYAREA", "Delivery Area");
            _list.Add("T_DELIVERYAREAINFO", "Delivery Days for Delivery Area");
            _list.Add("T_DELIVERYDATE", "Delivery Date");
            _list.Add("T_DELIVERYINSTRUCTIONS", "Delivery Instructions");           //IP - 12/06/12 - #10345 - Warehouse & Deliveries
            _list.Add("T_DELIVERYPROCESS", "Delivery Process");
            _list.Add("T_DELIVERYSTATUS", "Delivery Status");
            _list.Add("T_DELNOTEBRANCH", "Del Note Branch");
            _list.Add("T_DELNOTENUMBER", "Delivery Note Number");
            _list.Add("T_DELIVERYLOCATION", "Delivery Location");      // #10401 jec 15/06/12
            _list.Add("T_DELNOTENO", "Del Note No");
            _list.Add("T_DELORCOLL", "Del or Coll");
            _list.Add("T_DELREQDEL", "Required Del Date");
            _list.Add("T_DEPARTMENT", "Department");                    //IP - 25/07/11 - CR1254 - RI - #4036         
            _list.Add("T_DEPOSIT", "Deposit Value");
            _list.Add("T_DEPOSITAMOUNT", "Deposit Amount");
            _list.Add("T_DEPOSITPAID", "Deposit Paid");
            _list.Add("T_DEP", "Deposit");
            _list.Add("T_DEPOSITTYPE", "Deposit Type");
            _list.Add("T_DESCRIPTION", "Description");
            _list.Add("T_DESCRIPTION2", "Description2");
            _list.Add("T_DHLInterfaceDate", "DHL Interface Date");      // Malaysia 3PL jec 05/03/10
            _list.Add("T_DHLPickingDate", "DHL Picking Date");      // Malaysia 3PL jec 05/03/10
            _list.Add("T_DHLDNNo", "DHL DN No.");      // Malaysia 3PL jec 05/03/10
            _list.Add("T_DIALCODE", "Dail Code");
            _list.Add("T_DIFFERENCE", "Difference");
            _list.Add("T_DISBURSEMENTTYPE", "Disbursement Type");      //IP - 8/12/11 - CR1234
            _list.Add("T_DISCOUNT", "Discount");
            _list.Add("T_DODEFAULT", "Do Default");
            _list.Add("T_DONEXTRUN", "Do Next Run");
            _list.Add("T_DRIVERNAME", "Driver Name");
            _list.Add("T_DUEDATE", "Due Date Changed");
            _list.Add("T_DURATION", "Duration");
            _list.Add("T_DUEDAY", "Due Day");
            _list.Add("T_EFFECTIVE", "Effective");
            _list.Add("T_ELECTRICALLIM", "Electrical Limit");
            _list.Add("T_EMAIL", "Email");
            _list.Add("T_EMPEENAME", "Employee Name");
            _list.Add("T_EMPEENO", "Employee No");
            // _list.Add("T_EMPEETYPE", "Employee Type");
            _list.Add("T_ENABLED", "Enabled");
            _list.Add("T_ENGLISHDESC1", "English Desc1");
            _list.Add("T_ENGLISHDESC2", "English Desc2");
            _list.Add("T_ESTIMATE", "Estimate");
            _list.Add("T_ESTIMATEDATE", "Estimate Date");
            _list.Add("T_EVENT", "Event");
            _list.Add("T_EXT", "Ext");
            _list.Add("T_EXTTERM", "Ext Term");
            _list.Add("T_EXPRESS", "Express Delivery");     //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            _list.Add("T_EXTWARRANTY", "EW");      //CR1030  jec 
            _list.Add("T_EXPSETTLEMENTDATE", "Exp. Settlement Date"); //IP & JC - CR976
            _list.Add("T_FINALARRANGEAMOUNT", "Final Arrangement Amount"); //IP & JC - CR976
            _list.Add("T_EXPIRYDATE", "Expiry Date");    //IP - 13/01/11 - Store Card
            _list.Add("T_EXTRAPAYMENTS", "Extra Payments");
            _list.Add("T_FACTEMPEENO", "FACT Employee No");
            _list.Add("T_FEE", "Fee");
            _list.Add("T_FINPAYDATE", "Final Payment Date"); //IP & JC - CR976
            _list.Add("T_FIRSTPAYDATE", "First Payment Date"); //IP & JC - CR976
            _list.Add("T_FIRSTNAME", "First Name");
            _list.Add("T_FOODLOSS", "Food Loss");
            _list.Add("T_FORDEPOSIT", "Amt Available for deposit");
            _list.Add("T_FORDECASTPERIOD", "Forecast At Prior Period End");
            _list.Add("T_FORDECASTYEAR", "Forecast At Year End");
            _list.Add("T_FRZINTADMIN", "Frz Int Admin");
            _list.Add("T_FROMMONTH", "From Month");
            _list.Add("T_FROMMONTHSHORT", "From");
            _list.Add("T_FROMTOACCTNO", "Cheque No/From To A/C"); //IP - 69305 - 23/11/2007
            _list.Add("T_FURNITURELIM", "Furniture Limit");
            _list.Add("T_FYW", "FYW");
            _list.Add("T_MAN", "MAN"); //rm CR1051 change FYW to MAN
            _list.Add("T_MANWarranty", "MAN Warranty");

            _list.Add("T_GIFTREFERENCE", "Gift Voucher Reference");
            _list.Add("T_GRTCREATEDON", "GRT Created On"); //IP - 16/02/10 - CR1072 - CR1048 - 3.1.8 (ref:3.1.46 & 3.1.47) from 4.3
            _list.Add("T_GRTCREATEDBY", "GRT Created By"); //IP - 16/02/10 - CR1072 - CR1048 - 3.1.8 (ref:3.1.46 & 3.1.47) from 4.3
            _list.Add("T_HASWARRANTY", "Has Warranty");
            _list.Add("T_HCMember", "HomeClub MemberNo"); //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            _list.Add("T_HIALLOCATED", "Highest Allocated");
            _list.Add("T_HIALLOWED", "Highest Allowed");
            _list.Add("T_HLDJNT", "Holder/Joint");
            _list.Add("T_HPPRICE", "HP Price");
            _list.Add("T_IMMEDIATE", "Immediate");
            _list.Add("T_INCLUDEINSURANCE", "Include Insurance");
            _list.Add("T_INCLUDEWARRANTY", "Include Warranty");
            _list.Add("T_INCOME", "Monthly Disposable Income");
            _list.Add("T_INSTALLATIONDATE", "Installation Date");
            _list.Add("T_INSTALLATIONNO", "Installation No");
            _list.Add("T_INTCHARGESDUE", "Weekly Charges Calculated");
            _list.Add("T_INTCHARGESCUMUL", "Accumulated Charges");
            _list.Add("T_INTERFACE", "Interface");
            _list.Add("T_INTERNAL", "Internal");
            _list.Add("T_INVOICE_NUMBER", "Invoice Number");
            _list.Add("T_INVOICENO", "Invoice No: ");
            _list.Add("T_INVOICENOCASHGO", "Invoice No");
            _list.Add("T_INVOICETOTAL", "Invoice Total");
            _list.Add("T_INSTAL", "Install Amount");
            _list.Add("T_INSTALLMENT", "Instalment");
            _list.Add("T_INSTALORDER", "Instalment Order");
            _list.Add("T_INSTALNO", "No. Of Instalments");
            _list.Add("T_INSTANTREPLACE", "Instant Replacement");
            _list.Add("T_INSPERC", "Insurance %");
            _list.Add("T_INSPC", "INS%");
            _list.Add("T_ITEM_DESCRIPTION", "Item Description");
            _list.Add("T_ITEM_DESCRIPTION2", "Item Description 2");
            _list.Add("T_ITEMID", "ItemID");                        //IP - 20/05/11 - CR1212 - RI - #3627
            _list.Add("T_ITEMNO", "Item No");
            _list.Add("T_LABOURCOST", "Labour Cost");
            _list.Add("T_LABOURLIMIT", "Labour Value");
            _list.Add("T_LABOURPERCENT", "Labour %");
            _list.Add("T_LASTCOMMN", "Last Commision Amount");
            _list.Add("T_LASTNAME", "Last Name");
            _list.Add("T_LASTTRANSDATE", "Last Transaction Date");          //IP - 09/09/10 - CR1107 - WriteOff Review screen Enhancements
            _list.Add("T_LASTUPDATEDBY", "Last Updated By");
            _list.Add("T_LENGTH", "Length");
            _list.Add("T_LOADNO", "Load No");
            _list.Add("T_LOCATION", "Location");
            _list.Add("T_LOCALAMOUNT", "Local Amount");
            _list.Add("T_LOCKEDBY", "Locked By");
            _list.Add("T_LOGGEDBY", "Logged By");
            _list.Add("T_LOYALTYCLUB", "Loyalty Club");
            _list.Add("T_MACHINENAME", "Machine Name");                     //IP - 14/01/11 - Store Card
            _list.Add("T_MANDATEACTIVE", "Mandate ACTIVE");
            _list.Add("T_MANDATETERMINATED", "Mandate Terminated");
            _list.Add("T_MANDATEDELETED", "Mandate DELETED");
            _list.Add("T_MANDATORY", "Mandatory");
            _list.Add("T_MANUALNAME", "Manual WO Employee No");
            _list.Add("T_MAINTENANCE", "Maintenance");
            _list.Add("T_MIN", "Min");
            _list.Add("T_MAX", "Max");
            _list.Add("T_MMIAPPLICABLE", "MMI Applicable");
            _list.Add("T_MODELNO", "Model No");
            _list.Add("T_MONTHSINARREARS", "Months In Arrears");        //IP - 09/09/10 - CR1107 - WriteOff Review screen Ehnancements
            _list.Add("T_MOBILEPHONE", "Mobile Phone");
            _list.Add("T_HOMEPHONE", "Home Phone"); //IP - 21/07/08 - CR951
            _list.Add("T_CELLPHONE", "Cell Phone"); //IP - 21/07/08 - CR951
            _list.Add("T_MOBILENO", "Mobile No");
            _list.Add("T_MONTH", "Month");
            _list.Add("T_MONTHSARREARS", "Months in Arrears");
            _list.Add("T_MOREREWARDSNO", "More rewards no: ");
            _list.Add("T_MOVEMENT", "Movement");
            _list.Add("T_NAME", "Name");
            _list.Add("T_NETPAYMENT", "Net Payment");
            _list.Add("T_NEWAGREEMENTTOTAL", "New Agreement Total");
            _list.Add("T_NEWDELNOTENO", "New Del Note No");
            _list.Add("T_NEWCOD", "New COD");
            _list.Add("T_NEWCUSTID", "New Cust Id");
            _list.Add("T_NEWDATEFIRST", "New Due Date");
            _list.Add("T_NEWDEPOSIT", "New Deposit");
            _list.Add("T_NEWINSTALMENT", "New Instalment");
            _list.Add("T_NEWINSTALNO", "New Instal No.");
            _list.Add("T_NEWNAME", "New Name");
            _list.Add("T_NEWSERVICECHARGE", "New Service Charge");
            _list.Add("T_NOOFINS", "No of Instalments"); //IP & JC - CR976
            _list.Add("T_NO.REPAIRS", "No. Repairs"); // Walkthrough request to replace Seq on Service Request screen JH 08/11/2007
            _list.Add("T_NOTES", "Notes");
            _list.Add("T_NOTIFIEDBY", "Notified By");
            _list.Add("T_NUMALLOCATED", "No. Of Allocated A/C's");
            _list.Add("T_OLDAGREEMENTTOTAL", "Old Agreement Total");
            _list.Add("T_OLDCOD", "Old COD"); // 69516 COD changes to be displayed
            _list.Add("T_OLDCUSTID", "Old Cust Id");
            _list.Add("T_OLDDEPOSIT", "Old Deposit");
            _list.Add("T_OLDINSTALMENT", "Old Instalment");
            _list.Add("T_OLDINSTALNO", "Old Instal No.");
            _list.Add("T_OLDNAME", "Old Name");
            _list.Add("T_OLDSERVICECHARGE", "Old Service Charge");
            _list.Add("T_ONAMOUNT", "On Amount");
            _list.Add("T_OUTBAL", "Outstanding Balance");
            _list.Add("T_PARTSDATE", "Parts Date");
            _list.Add("T_PARTLIMIT", "Part Value");
            _list.Add("T_PARTNO", "Part No");
            _list.Add("T_PARTPERCENT", "Part %");
            _list.Add("T_PARTSDESCRIPTION", "Parts Description");
            _list.Add("T_PARTTYPE", "Part Type");
            _list.Add("T_PAIDAMOUNT", "Paid Amount");
            _list.Add("T_PARENT_ITEM_NUMBER", "Parent ItemNumber");
            _list.Add("T_PARENT_ITEM", "Parent Item");            // RI
            _list.Add("T_PARENT_STOCK_LOCATION", "Parent Stock Location");
            _list.Add("T_PAYMENT", "Payment");
            _list.Add("T_PAYMENTTOTAL", "Payment Total");
            _list.Add("T_PAYMENTS", "Payments");
            _list.Add("T_PAYMETHOD", "Pay Method");
            _list.Add("T_PERCENTAGEPAID", "Percentage Paid");
            _list.Add("T_PERCENTAGEPAID2", "% Paid");
            _list.Add("T_PERCENTAGERATE", "% Rate");
            _list.Add("T_SPAPERIOD", "Period"); //IP & JC - CR976
            _list.Add("T_PERIOD", "Period End");
            _list.Add("T_PHONE", "Phone");
            _list.Add("T_PHONEHOME", "Phone Home");
            _list.Add("T_PHONENO", "Phone Number");
            _list.Add("T_PHONEWORK", "Phone Work");
            _list.Add("T_PICKED", "Picked");
            _list.Add("T_PICKLISTNO", "Pick List No");
            _list.Add("T_POINTSFROM", "Points From");
            _list.Add("T_POINTSTO", "Points To");
            _list.Add("T_POSTCODE", "Post Code");
            _list.Add("T_PREVDEPOSITED", "Amt previously deposited");
            _list.Add("T_PRICE", "Price");
            _list.Add("T_PRINTED", "Printed");
            _list.Add("T_PRINTEDBY", "Printed By");
            _list.Add("T_PRINTLOCN", "Print Location");  //CR 949/958
            _list.Add("T_PRIVILEGECLUB", "Privilege Club");
            _list.Add("T_PRODCODE", "Product Code");
            _list.Add("T_PRODSTATUS", "Product Status"); //UAT 770
            _list.Add("T_PRODDESC", "Product Description");
            _list.Add("T_PRODDESC2", "Product Description 2");
            _list.Add("T_PRODUCT", "Product");
            _list.Add("T_PRODUCTLEVEL", "Product Level");
            _list.Add("T_PRODUCTSKU", "Product SKU");       // RI
            _list.Add("T_PRODUCTCLASS", "Product Class");       // RI
            _list.Add("T_PRODUCTSUBCLASS", "Product SubClass");       // RI 
            _list.Add("T_PRODUCT1", "Product 1");   // CR36 
            _list.Add("T_PRODUCT2", "Product 2");   // CR36
            _list.Add("T_PRODUCT3", "Product 3");   // CR36
            _list.Add("T_PRODUCT4", "Product 4");   // CR36
            _list.Add("T_PRODUCT5", "Product 5");   // CR36
            _list.Add("T_PRODUCTCOMMISSIONTOTAL", "Product Commission Total");     //CR1035
            _list.Add("T_PURCHASEDATE", "Purchase Date");
            _list.Add("T_QUANTITY", "Quantity");
            _list.Add("T_QUANTITYAFTER", "Qty After");
            _list.Add("T_QUANTITYBEFORE", "Qty Before");
            _list.Add("T_QUANTITYONORDER", "Qty Available");
            _list.Add("T_RATE", "Rate");
            _list.Add("T_RATEDETAILS", "Rate Details (applies to ALL score bands - any change will add new rate for ALL bands)");
            _list.Add("T_RATETYPE", "Rate Type");
            _list.Add("T_REASON", "Reason");
            _list.Add("T_REASONCHANGEDELDATE", "Change Delivery Date");
            _list.Add("T_REASONREMOVEDELNOTE", "Remove Delivery Note");
            _list.Add("T_REASONREMOVEDELNOTEITEM", "Remove Delivery Note Item");
            _list.Add("T_REBATE", "Rebate");
            _list.Add("T_REBATEWITHIN12MTHS", "Rebate Due Within 12 Months");
            _list.Add("T_REBATEAFTER12MTHS", "Rebate Due After 12 Months");
            _list.Add("T_RECEIPTDATE", "Receipt Date");
            _list.Add("T_RECEIPTNO", "Receipt No");
            _list.Add("T_RECEIPTNUMBER", "Receipt Number");             //IP - 13/01/11 - Store Card
            _list.Add("T_REDELIVERY", "REDELIVERY:");
            _list.Add("T_REFERENCE", "Reference");
            _list.Add("T_REFERRAL", "Referral Reason");
            _list.Add("T_REFINDEP", "Refinance Deposit");
            _list.Add("T_REFNO", "Reference Number");
            _list.Add("T_REFNO1", "Ref No");
            _list.Add("T_REFNOSHORT", "RefNo");
            _list.Add("T_REFTOTAL", "Ref Total");
            _list.Add("T_REJECTCODE", "Rejection Code");
            _list.Add("T_REJCODE", "Rej. Code");
            _list.Add("T_REJECTIONS", "Rejections");
            _list.Add("T_RELATION", "Relation");
            _list.Add("T_REPLACE", "Replace ");
            _list.Add("T_REPLACEMENTINVOICE", "REPLACEMENT: ");
            _list.Add("T_REPLACEMENTSTATUS", "Replacement Status");
            _list.Add("T_RELEASED", "Released");
            _list.Add("T_REMINST", "Rem. Inst.");
            _list.Add("T_REMINSTAMT", "Rem. Inst. Amt.");
            _list.Add("T_REMOVEDDELETEDBY", "Removed/Deleted By"); //IP - 18/11/09 - CR929 & 974 - Audit
            _list.Add("T_REPAIR", "Repair");
            _list.Add("T_REPAIRCENTRE", "Repair Centre"); //CR 1024 
            _list.Add("T_REPAIRDATE", "Repair Date");
            _list.Add("T_REPAIRESTIMATE", "Repair Estimate");
            _list.Add("T_REPLACEMENT", "Replacement");
            _list.Add("T_REPOARREARS", "Arrears Before Repo");
            _list.Add("T_REPOPCENT", "Repo Percentage");
            _list.Add("T_REPOPC", "Repo %");
            _list.Add("T_REPOVALUE", "Repo Value");
            _list.Add("T_REPOSSESSIONDATE", "Repossession Date");          //IP - 30/09/10 - CR1107 - WriteOff Review screen Enhancements
            _list.Add("T_REPORTDBLIVE", "Report from LIVE database");
            _list.Add("T_REPORTDBREPORTING", "Report from reporting database");
            //_list.Add("T_REPRINT", "REPRINT");
            _list.Add("T_REPRINT", "REPRINT");
            _list.Add("T_REQDELDATE", "Req Date Delivery");
            _list.Add("T_REQDELTIME", "Req Time Delivery");
            _list.Add("T_RESOLUTION", "Resolution");
            _list.Add("T_RESOLUTIONDESCRIPTION", "Resolution Description");
            _list.Add("T_RESOLUTIONDATE", "Resolution Date");
            _list.Add("T_RESULT", "Result");
            _list.Add("T_RETITEM", "Return Item No");
            _list.Add("T_RETLOCN", "Return Stock Locn");
            _list.Add("T_RETURNCODE", "Return Code");
            _list.Add("T_RETURNINVOICE", "RETURN INVOICE: ");
            _list.Add("T_REVIEWDATE", "Review Date");
            _list.Add("T_REVISEDBY", "Revised By");
            _list.Add("T_RFCREDITLIMIT", "RF Credit Limit");    //CR633 jec
            _list.Add("T_RULENAME", "Rule Name");
            _list.Add("T_RULERESULT", "Rule Result");
            _list.Add("T_RULEWEIGHT", "Weight");               //Equifax ScoreCard CR
            _list.Add("T_RULETYPE", "Rule Type");
            _list.Add("T_RUNNO", "Run Number");
            _list.Add("T_SALESPERSON", "Sales Person: ");
            _list.Add("T_SALESEMPEENO", "Sales Emp. No");
            _list.Add("T_SALESPERSONNAME", "Sales Person Name: ");  //IP - 17/05/12 - #9447 - CR1239
            _list.Add("T_SCHEDULED", "Scheduled");
            _list.Add("T_SCHEDULEDBRANCH", "Scheduled Branch");
            _list.Add("T_SCHEDULEDDATE", "Scheduled Date");  // 68440 RD 24/08/06
            _list.Add("T_SCHEDULEDQTY", "Scheduled Qty");
            _list.Add("T_SCHEDULEDREPAIRDATE", "Scheduled Repair Date");    //CR1030 jec 10/01/11
            _list.Add("T_SCHEDULING", "Scheduling");
            _list.Add("T_SCORE", "Score");
            _list.Add("T_SECURITISEDVALUE", "A.S.");
            _list.Add("T_SEGMENTNAME", "Segment Name");         //IP - 09/09/10 - CR1107 - WriteOffReview screen Enhancements
            _list.Add("T_SERVICECHARGE", "Service Charge %");
            _list.Add("T_SERVCHARGE", "Serv Charge");
            _list.Add("T_SERVICECHARGE2", "Service Charge");     //IP - 09/09/10 - CR1107 - WriteOffReview screen Enhancements
            _list.Add("T_SERIALNO", "Serial No");
            _list.Add("T_SCPC", "SC%");
            _list.Add("T_SERVICECHARGEBANDS", "Service Charge (applies to THIS score band only)");
            _list.Add("T_SERVICECHARGESHORT", "%");
            _list.Add("T_SERVICELOCATION", "Service Location");
            _list.Add("T_SERVICEREQUESTNO", "SR No");
            _list.Add("T_SERVICETYPE", "Service Type");
            _list.Add("T_SETTLED", "Accounts Settled");
            _list.Add("T_SETTLEMENT", "Settlement");
            _list.Add("T_SEQUENCE", "Seq");
            _list.Add("T_SLOTDATE", "Slot Date"); //CR802 added 12/12/2006  JH
            _list.Add("T_SOFTSCRIPT", "Soft Script");
            _list.Add("T_FAULTSCOMMENTS", "Fault Comments"); //IP - 19/10/09 - CR1055
            _list.Add("T_SOLD_ON", "Sold On");
            _list.Add("T_SOURCE", "Source");
            _list.Add("T_SPIFFVALUE", "Spiff Value");
            _list.Add("T_SPIFFPERCENTAGE", "Spiff Percentage");
            _list.Add("T_STATUS", "Status");
            _list.Add("T_STATUSCODE", "Status Code");
            _list.Add("T_STAY", "Stay");
            _list.Add("T_STAFFTYPE", "Staff Type"); //IP - 03/02/10 - CR1072 - 3.1.8
            _list.Add("T_STOCK_STATUS", "Stock Status"); // 69417 SC 04/12/07
            _list.Add("T_STOCK", "Stock Available");
            _list.Add("T_STOCKITEMNO", "Stock Item No");
            _list.Add("T_STOCKLOCN", "Stock Location");
            _list.Add("T_STOCKONORDER", "Stock On Order");
            _list.Add("T_STOCKSTATUS", "Stock Status");
            _list.Add("T_STORECARDNAME", "Store Card Name");    //IP - 13/01/11 - Store Card
            _list.Add("T_STORECARDNUMBER", "Store Card Number");    //IP - 07/01/11 - Store Card
            _list.Add("T_STORECARDLIMIT", "Store Card Limit");    //IP - 07/01/11 - Store Card
            _list.Add("T_STORECARDAVAILABLE", "Store Card Available");    //IP - 07/01/11 - Store Card
            _list.Add("T_STORETYPE", "Store Type");
            _list.Add("T_STRATEGY", "Strategy");
            _list.Add("T_SUBTOTAL", "Sub Total");
            _list.Add("T_SUPPLIER", "Supplier");
            _list.Add("T_SUPPLIERCODE", "Supplier Code");
            _list.Add("T_SYSTEMVALUE", "System Value");
            _list.Add("T_TAKENBY", "Taken By");
            _list.Add("T_TAXAMT", "Tax");
            _list.Add("T_TAXAMTBEFORE", "Tax Before");
            _list.Add("T_TAXAMTAFTER", "Tax After");
            _list.Add("T_TAXINVOICE", "TAX INVOICE: ");
            _list.Add("T_TECHNICIAN", "Technician");
            _list.Add("T_TECHNICIANID", "Technician ID");
            _list.Add("T_TECHNICIANNAME", "Technician Name");
            _list.Add("T_TELACTION", "Telephone Actions");
            _list.Add("T_TELNO", "Telephone");
            _list.Add("T_TERMS", "Terms");
            _list.Add("T_TERMSTYPE", "Terms Type");
            _list.Add("T_THRESHOLD", "Threshold");
            _list.Add("T_TILLID", "Till ID");
            _list.Add("T_TIME", "Time");
            _list.Add("T_TIMEOPEN", "Time Opened");
            _list.Add("T_TIMEREQDEL", "Required Del Time");
            _list.Add("T_TITLE", "Title");
            _list.Add("T_TOMONTH", "To Month");
            _list.Add("T_TOMONTHSHORT", "To");
            _list.Add("T_TOPAY", "To Pay");
            _list.Add("T_TOTALAMT", "Total Amt");
            _list.Add("T_TOTALITEMS", "No. Items on Del Note");
            _list.Add("T_TOTALLED", "Totalled");
            _list.Add("T_TOTAL", "Total");
            _list.Add("T_TOTALCOST", "Total Cost");
            _list.Add("T_TOTALDUE", "Total Due");
            _list.Add("T_TRANS1", "Translation 1");
            _list.Add("T_TRANS2", "Translation 2");
            _list.Add("T_TRANSACTIONDATE", "Transaction Date");
            _list.Add("T_TRANSACTIONNO", "Transaction No");
            _list.Add("T_TRANSACTIONTYPE", "Transaction Type");
            _list.Add("T_TRANSACTIONS", "  transaction(s)");
            _list.Add("T_TRANSACTIONS1", "Transactions");
            //_list.Add("T_TRANSACTIONS","  transaction(s)");
            _list.Add("T_TRANSFERREF", "Transfer Ref");                               //IP - 16/02/12 - #9632 - CR1234
            _list.Add("T_TRANSFERREDTOSAFE", "Transferred To Safe");
            _list.Add("T_TRANSPICKLISTNO", "Transport Pick List No");
            _list.Add("T_TRANSPORTCOST", "Transport Cost");
            _list.Add("T_TRANSREFNO", "Trans Ref No");
            _list.Add("T_TRUCKID", "Truck Id");
            _list.Add("T_TYPE", "Type");
            _list.Add("T_UNDELIVEREDFLAG", "Undelivered Flag");
            _list.Add("T_UNEMPLOYED", "Unemployed");
            _list.Add("T_UNITPRICE", "Unit Price");
            _list.Add("T_UPDATED", "Last Updated");
            _list.Add("T_USERVALUE", "User Value");
            _list.Add("T_VALUE", "Value");
            _list.Add("T_VALUEAFTER", "Value After");
            _list.Add("T_VALUEBEFORE", "Value Before");
            _list.Add("T_VISIBLE", "Visible");
            _list.Add("T_VOUCHERNO", "Voucher No");
            _list.Add("T_WARRANTY_NO", "Warranty No");
            _list.Add("T_WARRANTYTOTAL", "Warranty Total");     //CR1035
            _list.Add("T_WEEKNO", "Week No");
            _list.Add("T_WORKLIST", "Worklist");
            _list.Add("T_WORKPHONE", "Work Phone");
            _list.Add("T_CODEDSCRIPT", "Description");
            _list.Add("T_SEGMENTID", "Segment ID");
            _list.Add("T_SEGMENTUSER", "User");
            _list.Add("T_SORTORDER", "Sort Order");
            _list.Add("T_STATUSDESC", "Status Description");
            _list.Add("T_SUBAGREEMENT", "Sub Agreement");
            _list.Add("T_DATEACCTLTTR", "Date Sent");
            _list.Add("T_LETTERCODE", "Letter Code");
            _list.Add("T_DELTEXT", "REQUIRED DELIVERY:");
            _list.Add("T_DELDUPLICATETEXT", "Check Dup delivery note already printed:");
            _list.Add("T_COLLTEXT", "COLLECTION:");
            _list.Add("T_CASHTEXT", "Cash Sale");
            _list.Add("T_CREDITTEXT", "Credit Sale");
            _list.Add("T_MADCASHTEXT", "Vente au");
            _list.Add("T_MADCREDITTEXT", "Vente a Credit");
            _list.Add("T_EXCELSAVE", "Save results to file");
            _list.Add("T_RUNSTART", "Run Start");
            _list.Add("T_RUNEND", "Run End");
            _list.Add("T_OPENACSTART", "Open A/C at Start");
            _list.Add("T_OPENACCLOSE", "Open A/C at Close");
            _list.Add("T_BALANCESTART", "Balance at Start");
            _list.Add("T_BALANCEEND", "Balance at Close");
            _list.Add("T_SMRYHPVALUE", "HP Total");
            _list.Add("T_SMRYCASHVALUE", "Cash Total");
            _list.Add("T_SMRYSPECIALVALUE", "Special Total");
            _list.Add("T_SMRYSTORECARDVALUE", "Store Card Total");                          //IP - 17/02/12 - #9423 - CR1234
            _list.Add("T_SMRYSOURCE", "Source");
            _list.Add("T_DateReposs", "Date Of Repossession");
            _list.Add("T_DELSLOT", "Delivery Slot");
            _list.Add("T_DELDATE", "Deliver Date");
            _list.Add("T_STORECARD", "Store Card");                                          //IP - 20/02/12 - #9423 - CR1234
                                                                                             //MMI Matrix
            _list.Add("T_FROMSCORE", "From Score");
            _list.Add("T_TOSCORE", "To Score");
            _list.Add("T_MMIPERCENTAGE", "MMI %");
            //Service failure report headings
            _list.Add("T_TOTALSOLD", "Total Sold");
            _list.Add("T_SALESQ1", "Sales Q1");
            _list.Add("T_SALESQ2", "Sales Q2");
            _list.Add("T_SALESQ3", "Sales Q3");
            _list.Add("T_SALESQ4", "Sales Q4");
            _list.Add("T_AFTER90DAYS", "After 90 Days");
            _list.Add("T_AFTER180DAYS", "After 180 Days");
            _list.Add("T_AFTER270DAYS", "After 270 Days");
            _list.Add("T_AFTER365DAYS", "After 365 Days");
            _list.Add("T_AFTER90DAYSCOUNT", "Count After 90");
            _list.Add("T_AFTER180DAYSCOUNT", "Count After 180");
            _list.Add("T_AFTER270DAYSCOUNT", "Count After 270");
            _list.Add("T_AFTER365DAYSCOUNT", "Count After 365");

            //Claims report headings
            _list.Add("T_CUSTOMERADDRESS", "Customer Address");
            _list.Add("T_PARTDESCRIPTION", "Part Description");
            _list.Add("T_RECEIVEDDATE", "Received Date");
            _list.Add("T_FYWTIC", "FYW");
            _list.Add("T_COMMENTS", "Comments");
            _list.Add("T_DATEPROMISED", "Date Promised");
            _list.Add("T_DATECOLLECTED", "Date Collected");
            _list.Add("T_TECHNICIANREPORT", "Technician Report");

            //Commission Report Headings
            _list.Add("T_COMMISSIONTYPE", "Commission Type");
            _list.Add("T_COMMISSIONAMOUNT", "Commission Amount");
            _list.Add("T_DELIVERYAMOUNT", "Delivery Amount");
            _list.Add("T_REBATETOTAL", "Rebate  Comm. Total");
            _list.Add("T_REPOSSESSIONTOTAL", "Repossession Comm. Total");
            _list.Add("T_PRODUCTTOTAL", "Product Total");
            _list.Add("T_COMMISSIONPERCENT", "Commission %");
            _list.Add("T_EMPLOYEENAME", "Employee Name");
            _list.Add("T_COMMISSIONTOTAL", "Commission Total");
            _list.Add("T_SPIFFTOTAL", "SPIFF Total");
            _list.Add("T_TRANSTYPECODE", "Trans Type");
            _list.Add("T_CANCELLATIONTOTAL", "Cancellation Comm. Total");

            //DataGrid Column Headings

            _list.Add("T_CreditPct", "Percentage for Credit");
            _list.Add("T_CashPct", "Percentage for Cash");
            _list.Add("T_Percentage", "Percentage");
            _list.Add("T_CreditPctRepo", "Repo Percent for Credit");      //RI jec 08/07/11
            _list.Add("T_CashPctRepo", "Repo Percent for Cash");
            _list.Add("T_PercentageRepo", "Repo Percentage");
            _list.Add("T_PctRepo", "Repo Percent");
            _list.Add("T_ValueRepo", "Repo Value");

            //For amortization schedule tab
            _list.Add("T_PaymentNum", "Payment No.");
            _list.Add("T_PaymentDate", "Payment Date");
            _list.Add("T_BeginningBalance", "Beginning Balance");
            _list.Add("T_PaymentAmt", "Scheduled Payment");
            _list.Add("T_Principal", "Principal");
            _list.Add("T_Interest", "Interest");
            _list.Add("T_EndingBalance", "Ending Balance");
            _list.Add("T_LatePmtFee", "Late Payment Fee");
            _list.Add("T_PenaltyFee", "Penalty Fee");
            _list.Add("T_AdminFee", "Admin Fee");
            _list.Add("T_TotalInstalment", "Total Instalment");
            _list.Add("T_NoOfDependents", "Number Of Dependents");
            _list.Add("T_DependentPerFactor", "Dependent Spend Factor('%')");
            _list.Add("T_MinimumIncome", "Min Income");
            _list.Add("T_MaximumIncome", "Max Income");
            _list.Add("T_ApplicantPerFactor", "Applicant Spend Factor('%')");

            _list.Add("T_PAYMENTAMOUNT", "Is Refund or Correction (Y/N)");
            //_list.Add("T_TENDEREDAMOUNT", "Tendered Amount");
            //_list.Add("T_CHANGEAMOUNT", "Change Amount");
            _list.Add("T_SUCCESS", "Is Success (Y/N)");
            _list.Add("T_DEPOSITR", "Is Deposit (Y/N)");
            _list.Add("T_ID", "ID");


            //_list.Add("T_PAYMENTID", "PaymentId");
            _list.Add("T_TOTALAMTREC", "TotalAmountReceived");
            _list.Add("T_TOTALCASH", "TotalCash");
            _list.Add("T_TOTALCHEQUE", "TotalCheque");
            _list.Add("T_CHANGE", "CashChange");
            _list.Add("T_TOTALPAYMENTPAID", "TotalPaymentPaid");
            _list.Add("T_DATES", "Date");


            _list.Add("T_ACCTNOS", "AcctNo");
            _list.Add("T_TRANSREFNOS", "TransRefNo");
            _list.Add("T_DATETRANS", "DateTrans");
            _list.Add("T_TRANSTYPECODES", "TransTypeCode");
            _list.Add("T_TRANSVALUE", "TransValue");
            _list.Add("T_PAYMETHODS", "PayMethod");
            _list.Add("T_EMPLOYEENAMES", "EmployeeName");
            _list.Add("T_EMPEENOS", "EmpeeNo");
            _list.Add("T_FTNOTES", "FtNotes");
            _list.Add("T_TRANSPRINTED", "TransPrinted");
            _list.Add("T_REFERENCES", "Reference");

            _list.Add("T_CHEQUENOS", "ChequeNo");
            _list.Add("T_CASHIERTOTALID", "CashierTotalId");
            _list.Add("T_PAYMENTAMOUNTS", "PaymentAmount");

            _list.Add("T_ISDEPOSITS", "IsDeposit");
            _list.Add("T_POSTEDTOMAMBU", "PostedToMambu");
            _list.Add("T_MAMBUERROR", "MambuError");

            




            #endregion
            //
            // Tool Tips
            //
            #region ToolTips
            _list.Add("TT_ACCOUNTDETAILS", "Account Details");
            _list.Add("TT_ACCOUNTSEARCH", "Account Search");
            _list.Add("TT_ACTIONFILTER", "Once a restriction has been selected two further options are shown these are: \\1) A List Of Actions. \\2) Action Taken Date Range");
            _list.Add("TT_ACTIONCOUNTFILTER", "Once a restriction been selected the user can select the number of actions to base the criteria on.");
            _list.Add("TT_ACTIVATE", "Activate Terms Type");
            _list.Add("TT_ADDCODE", "Add Code");
            _list.Add("TT_ADDRESSTYPE", "Choose address type \\Click add and edit the details or click remove to delete the address details");
            _list.Add("TT_ADDROW", "Add a new row to the list");
            _list.Add("TT_ALLOCATEDFILTER", "There are two options to chose from either allocated to a Courts member of staff or not.");
            _list.Add("TT_ARREARSFILTER", "Once an arrears limitation has been selected the user can select \\the limited value they want to base the criteria on.");
            _list.Add("TT_BETWEEN", "Between");
            _list.Add("TT_BALANCEFILTER", "Once a balance restriction has been selected the user can select \\a balance value to base the criteria on.");
            _list.Add("TT_BRANCHFILTER", "This is the Branch where the account is originally created.");
            _list.Add("TT_CASHIERBREAKDOWN", "Click for Cashier breakdown by transaction");
            _list.Add("TT_CARDNO", "Please enter the last four digits of the credit/debit card number or the full storecard Number");   //IP - 26/01/11 - Sprint 5.9 - #2785
            _list.Add("TT_CHARGESFILTER", "If checked exclude charges from arrears value.");
            _list.Add("TT_CLICKACTION", "Click on the Action column to change a Reject Action");
            _list.Add("TT_CODEFILTER", "Once a restriction has been selected a list of codes is given to select from.");
            _list.Add("TT_COMPLETE", "Complete");
            _list.Add("TT_COPYREFERENCES", "Copy References");
            _list.Add("TT_CREATEHISTORY", "If checked customer has moved -otherwise change in details to existing address");
            _list.Add("TT_DATEMOVEDFILTER", "Once a filter has been selected the user can select \\a date range to base the criteria on.");
            _list.Add("TT_DEACTIVATE", "De-activate terms type");
            _list.Add("TT_DELAREA", "Delivery Area Descriptions");      //#14796
            _list.Add("TT_EMPLOYEEFILTER", "If checked limit number of actions for the allocated employee\\ otherwise limit for all employees.");
            _list.Add("TT_ENTERLINEITEM", "Enter Line Item");
            _list.Add("TT_EXCHANGECALCULATOR", "Exchange Rate Calculator");
            _list.Add("TT_IFCHECKED", "If checked only this flag outstanding");
            _list.Add("TT_LETTERFILTER", "When the SL ? A Specific Letter option is selected two further options are shown these are: \\1) A List Of Different Letter Types To Select From. \\2) Sent Between Date Range.  \\IF the AL ? Any Letter is selected then only the second option above is shown.");
            _list.Add("TT_MULTIPAYMETHOD", "Add another pay method");
            _list.Add("TT_CLEARMULTIPAY", "Clear all pay methods");
            _list.Add("TT_NEWCUST", "New Customer");
            _list.Add("TT_NEXT", "Next 250 records");
            _list.Add("TT_PAYMENTLIST", "Payment List");
            _list.Add("TT_PREVIOUS", "Previous 250 records");
            _list.Add("TT_PRINTCARD", "Print updates on Payment Card");
            _list.Add("TT_PRINTRECEIPT", "Print Receipt");
            _list.Add("TT_PRIVLOYALCLUB", "Privilege/Loyalty Club");    //jec 05/12/07
            _list.Add("TT_RECORDDELIVERY", "Record Delivery");
            _list.Add("TT_REFRESH", "Refresh");
            _list.Add("TT_REFRESHFILTER", "Refresh Employee Allocation List.");
            _list.Add("TT_REMOVECODE", "Remove Code");
            _list.Add("TT_REMOVELINEITEM", "Remove Line Item");
            _list.Add("TT_REVERSAL", "A reversed entry cannot be deleted and is marked with a red icon. The reversal is marked in green and can be deleted.");
            _list.Add("TT_RFMAXSPEND", "Max spend total does not include non-stock values");
            _list.Add("TT_ROWCORRECTED", "Symbol marks rows already corrected");
            _list.Add("TT_ROWCURRENT", "Symbol marks rows that are current");
            _list.Add("TT_ROWEDIT", "Symbol marks rows you have added");
            _list.Add("TT_SAVE", "Save");
            _list.Add("TT_SERVICECHARGEUPDATE", "Update SC% rates for all active and future rates");
            _list.Add("TT_SCOREFILTER", "Once a limit credit score restriction has been selected the \\user can select the credit score number to base the criteria on.");
            _list.Add("TT_STATUSFILTER", "This is a status which an account can run into, this depends upon the time or arrears the account has accumulated.");
            _list.Add("TT_TICKAMOUNT", "Tick the row to enter an Extra Payment amount");
            _list.Add("TT_EDIT", "Edit details");
            _list.Add("TT_CLEAR", "Clear form");
            _list.Add("TT_DDRETURN", "Send Mandate Return Letter");
            _list.Add("TT_SEARCH", "Search");
            _list.Add("TT_TOTALINSTALMENTS", "Payable instalments for delivered accounts");
            _list.Add("TT_LINKCUSTOMER", "Link Customer To Account");
            _list.Add("TT_PRINTAGREEMENT", "Print Agr/Warranty");
            _list.Add("TT_VALIDATE", "Validate");
            _list.Add("TT_ADDEDFILTER", "Use date action added");	//jec 67902
            _list.Add("TT_DUEFILTER", "Use date action due");		//jec 67902
            _list.Add("TT_CUSTSEARCH", "Customer Search");
            _list.Add("TT_SERVICESEARCH", "Service Request Search");
            _list.Add("TT_STOCKSEARCH", "Right click for stock item search");
            _list.Add("TT_REALLOCATEWARNING", "If ticked, all accounts of the employee will be reallocated");
            _list.Add("TT_CODEORGROUP", "Filter by Product Code OR Product Group");             // RI

            #endregion

            return _list;
        }
    }
    public class Recent
    {
        private static StringCollection _accountlist = null;
        public static StringCollection AccountList
        {
            get
            {
                if (_accountlist == null)
                    _accountlist = new StringCollection();
                return _accountlist;
            }
            set
            {
                if (_accountlist == null)
                    _accountlist = new StringCollection();
                _accountlist = value;
            }
        }
        private static StringCollection _customerlist = null;
        public static StringCollection CustomerList
        {
            get
            {
                if (_customerlist == null)
                    _customerlist = new StringCollection();
                return _customerlist;
            }
            set
            {
                if (_customerlist == null)
                    _customerlist = new StringCollection();
                _customerlist = value;
            }
        }
        public static void AddAccount(string account)
        {
            Add(account, AccountList);
        }
        public static void AddCustomer(string customer)
        {
            Add(customer, CustomerList);
        }
        private static void Add(string account, StringCollection sc)
        {
            bool found = false;
            if (sc == null)
                sc = new StringCollection();

            foreach (string s in sc)
            {
                if (s == account)
                {
                    found = true;
                    break;
                }

            }
            if (!found)
                sc.Insert(0, account);
            else
            {
                sc.RemoveAt(sc.IndexOf(account));
                Add(account, sc);
            }

            if (sc.Count >= 11)
                sc.RemoveAt(10);
        }
    }

    public class Date
    {
        // Culture sensitive date for 1 Jan 1900
        public static readonly DateTime blankDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
        public static readonly string standardFormat = "ddd dd MMM yyyy";
        public static readonly DateTime newCLDate = Convert.ToDateTime("01/04/2023");
    }

    /// <summary>
    /// Provides functionality to read the config file
    /// Currently gets the WebService Url, branch code,
    /// country code and culture
    /// </summary>
    public class Config
    {
        static Config()
        {
            string pathToCoSACS = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Config)).CodeBase);
            pathToCoSACS = Path.GetDirectoryName(pathToCoSACS);

            EposExeDefaultPath = Path.Combine(pathToCoSACS, "EPOS\\Courts.NET.EPOS.exe");
        }

        private const string serviceUrlTag = "WebServiceURL";
        private const string countryCode = "CountryCode";
        private const string branchCode = "BranchCode";
        private const string culture = "Culture";
        private const string newInstall = "NewInstall";
        private const string testKey = "TestKey";
        private const string updatePath = "UpdatePath";
        private const string splashImage = "SplashImage";
        private const string agrPrinter = "AgrPrinter";
        private const string agrTray = "AgrTray";
        private const string invoicePrinter = "InvoicePrinter";
        private const string invoiceTray = "InvoiceTray";
        private const string summaryPrinter = "SummaryPrinter";
        private const string summaryTray = "SummaryTray";
        private const string collPrinter = "CollPrinter";
        private const string collTray = "CollTray";
        private const string cashDrawerID = "CashDrawerID";
        private const string receiptprintermodel = "ReceiptPrinterModel";
        private const string storetype = "StoreType";
        public const string eposExe = "EPOSexe";
        public const string thermalPrintingEnabled = "ThermalPrintingEnabled";
        public const string thermalPrinterName = "ThermalPrinterName";
        public static string EposExeDefaultPath;

        private static string _url;
        private static string _branch;
        private static string _country;
        private static string _culture;
        private static bool _newinstall;
        private static string _testkey;
        private static string _updatePath;
        private static string _splashImage;
        private static string _agrPrinter;
        private static string _agrTray;
        private static string _invoicePrinter;
        private static string _invoiceTray;
        private static string _summaryPrinter;
        private static string _summaryTray;
        private static string _collPrinter;
        private static string _collTray;
        private static string _cashDrawerID;
        private static string _receiptprintermodel;
        private static string _thermalPrinterName;
        private static bool _ThermalPrintingEnabled;
        private static string _storetype;
        private static string _eposExe;
        private static string _version;

        public static string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public static string AgrPrinter
        {
            get { return _agrPrinter; }
            set { _agrPrinter = value; }
        }
        public static string AgrTray
        {
            get { return _agrTray; }
            set { _agrTray = value; }
        }

        public static string InvoicePrinter
        {
            get { return _invoicePrinter; }
            set { _invoicePrinter = value; }
        }
        public static string InvoiceTray
        {
            get { return _invoiceTray; }
            set { _invoiceTray = value; }
        }

        public static string SummaryPrinter
        {
            get { return _summaryPrinter; }
            set { _summaryPrinter = value; }
        }
        public static string SummaryTray
        {
            get { return _summaryTray; }
            set { _summaryTray = value; }
        }

        public static string CollPrinter
        {
            get { return _collPrinter; }
            set { _collPrinter = value; }
        }
        public static string CollTray
        {
            get { return _collTray; }
            set { _collTray = value; }
        }

        public static string SplashImage
        {
            get { return _splashImage; }
            set { _splashImage = value; }
        }

        public static string UpdatePath
        {
            get { return _updatePath; }
            set { _updatePath = value; }
        }

        public static bool NewInstall
        {
            get { return _newinstall; }
            set { _newinstall = value; }
        }

        public static string TestKey
        {
            get { return _testkey; }
            set { _testkey = value; }
        }

        public static string Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        public static string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public static bool Connected { get; set; }

        public static string Server
        {
            get
            {

                return Url.Split('/')[2];
            }
        }
        public static string BranchCode
        {
            get { return _branch; }
            set { _branch = value; }
        }
        public static string CountryCode
        {
            get { return _country.Trim(); }             //IP - 18/04/12 - #9494
            set { _country = value; }
        }
        public static string CashDrawerID
        {
            get { return _cashDrawerID; }
            set { _cashDrawerID = value; }
        }

        public static string ReceiptPrinterModel
        {
            get { return _receiptprintermodel; }
            set { _receiptprintermodel = value; }
        }

        public static string ThermalPrinterName
        {
            get { return _thermalPrinterName; }
            set { _thermalPrinterName = value; }
        }

        public static string StoreType
        {
            get { return _storetype; }
            set { _storetype = value; }
        }

        public static bool ThermalPrintingEnabled
        {
            get { return _ThermalPrintingEnabled; }
            set { _ThermalPrintingEnabled = value; }
        }

        public static string EPOSexe
        {
            get { return _eposExe; }
            set { _eposExe = value; }
        }

        public static bool Read(Assembly asm)
        {
            bool status = true;

            try
            {

                _url = ConfigurationManager.AppSettings[serviceUrlTag];
                _country = ConfigurationManager.AppSettings[countryCode];
                //_branch = ConfigurationManager.AppSettings[branchCode];
                _culture = ConfigurationManager.AppSettings[culture];
                _newinstall = Convert.ToBoolean(ConfigurationManager.AppSettings[newInstall]);
                _testkey = ConfigurationManager.AppSettings[testKey];
                _updatePath = ConfigurationManager.AppSettings[updatePath];
                //_splashImage = ConfigurationManager.AppSettings[splashImage];
                _agrPrinter = ConfigurationManager.AppSettings[agrPrinter];
                _agrTray = ConfigurationManager.AppSettings[agrTray];
                _invoicePrinter = ConfigurationManager.AppSettings[invoicePrinter];
                _invoiceTray = ConfigurationManager.AppSettings[invoiceTray];
                _summaryPrinter = ConfigurationManager.AppSettings[summaryPrinter];
                _summaryTray = ConfigurationManager.AppSettings[summaryTray];
                _collPrinter = ConfigurationManager.AppSettings[collPrinter];
                _collTray = ConfigurationManager.AppSettings[collTray];

                //_cashDrawerID = ConfigurationManager.AppSettings[cashDrawerID] == null ? "" : ConfigurationManager.AppSettings[cashDrawerID];
                //_receiptprintermodel = ConfigurationManager.AppSettings[receiptprintermodel] == null ? "295" : ConfigurationManager.AppSettings[receiptprintermodel];
                //_thermalPrinterName = ConfigurationManager.AppSettings[thermalPrinterName];
                //_ThermalPrintingEnabled = ConfigurationManager.AppSettings[thermalPrintingEnabled] == null ? false : Convert.ToBoolean(ConfigurationManager.AppSettings[thermalPrintingEnabled]);// ==  true : Convert.ToBoolean(ConfigurationSettings.AppSettings[MSposinstalled]) == false; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
                //_storetype = ConfigurationManager.AppSettings[storetype] == null ? "" : ConfigurationManager.AppSettings[storetype];
                _eposExe = ConfigurationManager.AppSettings[eposExe] == null ? EposExeDefaultPath : ConfigurationManager.AppSettings[eposExe];

                //_thermalReceiptPrinterName = ConfigurationSettings.AppSettings[thermalReceiptPrinterName] == null ? "PosPrinter" : ConfigurationSettings.AppSettings[thermalReceiptPrinterName];
                //_useThermalPrinter = ConfigurationSettings.AppSettings[useThermalPrinter] == null ? false : Boolean.Parse(ConfigurationSettings.AppSettings[useThermalPrinter]);
                //_eposExe = ConfigurationSettings.AppSettings[eposExe] == null ? EposExeDefaultPath : ConfigurationSettings.AppSettings[eposExe];

                string path = asm.Location.Substring(0, asm.Location.IndexOf("Courts.NET.PL.exe"));
                path += "Recent.xml";

                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(fileInfo.FullName);
                    XmlNode recentAccounts = xml.DocumentElement.SelectSingleNode(Elements.RecentAccounts);
                    if (recentAccounts != null)
                    {
                        foreach (XmlNode account in recentAccounts.ChildNodes)
                        {
                            string acctNo = account.Attributes[Tags.AccountNumber].Value;
                            Recent.AccountList.Add(acctNo);
                        }
                    }
                    XmlNode recentCustomers = xml.DocumentElement.SelectSingleNode(Elements.RecentCustomers);
                    if (recentCustomers != null)
                    {
                        foreach (XmlNode customerID in recentCustomers.ChildNodes)
                        {
                            string custID = customerID.Attributes[Tags.CustomerID].Value;
                            Recent.CustomerList.Add(custID);
                        }
                    }
                }

            }
            catch (Exception)
            {
                status = false;
                //Nothing we can sensibly do here, can't log it 
                //because we may not have the web service url
                //allow the status to reflect what to do next
            }
            return status;
        }

        public static void SaveRecent(Assembly asm)
        {
            string path = asm.Location.Substring(0, asm.Location.IndexOf("Courts.NET.PL.exe"));
            path += "Recent.xml";

            //Load the config file into the XML DOM.
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<recent />");
            xml.DocumentElement.AppendChild(xml.CreateElement(Elements.RecentAccounts));
            xml.DocumentElement.AppendChild(xml.CreateElement(Elements.RecentCustomers));
            XmlNode recentAccounts = xml.DocumentElement.SelectSingleNode(Elements.RecentAccounts);
            if (recentAccounts != null)
            {
                recentAccounts.RemoveAll();

                foreach (string acctNo in Recent.AccountList)
                {
                    XmlNode node = xml.CreateElement(Elements.Account);
                    node.Attributes.Append(xml.CreateAttribute(Tags.AccountNumber));
                    node.Attributes[Tags.AccountNumber].Value = acctNo;
                    recentAccounts.AppendChild(node);
                }
            }
            XmlNode recentCustomers = xml.DocumentElement.SelectSingleNode(Elements.RecentCustomers);
            if (recentCustomers != null)
            {
                recentCustomers.RemoveAll();

                foreach (string custID in Recent.CustomerList)
                {
                    XmlNode node = xml.CreateElement(Elements.Customer);
                    node.Attributes.Append(xml.CreateAttribute(Tags.CustomerID));
                    node.Attributes[Tags.CustomerID].Value = custID;
                    recentCustomers.AppendChild(node);
                }
            }
            xml.Save(path);
        }

        public static void Save(Assembly asm)
        {
            FileInfo fileInfo = new FileInfo(asm.Location + ".config");

            if (!fileInfo.Exists)
            {
                throw new STLException("Missing config file" + asm.Location + ".config");
            }

            //Load the config file into the XML DOM.
            XmlDocument xml = new XmlDocument();
            xml.Load(fileInfo.FullName);

            foreach (XmlNode node in xml["configuration"]["appSettings"])
            {
                if (node.Name == "add")
                {
                    switch (node.Attributes["key"].Value)
                    {
                        case serviceUrlTag:
                            node.Attributes["value"].Value = Url;
                            break;
                        case countryCode:
                            node.Attributes["value"].Value = CountryCode;
                            break;
                        case branchCode:
                            node.Attributes["value"].Value = BranchCode;
                            break;
                        case newInstall:
                            node.Attributes["value"].Value = Boolean.FalseString;
                            break;
                        case testKey:
                            node.Attributes["value"].Value = TestKey;
                            break;
                        case updatePath:
                            node.Attributes["value"].Value = UpdatePath;
                            break;
                        case culture:
                            node.Attributes["value"].Value = Culture;
                            break;
                        case splashImage:
                            node.Attributes["value"].Value = SplashImage;
                            break;
                        case agrPrinter:
                            node.Attributes["value"].Value = AgrPrinter;
                            break;
                        case agrTray:
                            node.Attributes["value"].Value = AgrTray;
                            break;
                        case invoicePrinter:
                            node.Attributes["value"].Value = InvoicePrinter;
                            break;
                        case invoiceTray:
                            node.Attributes["value"].Value = InvoiceTray;
                            break;
                        case summaryPrinter:
                            node.Attributes["value"].Value = SummaryPrinter;
                            break;
                        case summaryTray:
                            node.Attributes["value"].Value = SummaryTray;
                            break;
                        default:
                            break;
                    }
                }
            }

            /* need to add cashDrawerID. The node may or may not already exists */
            XmlNode n = xml.SelectSingleNode("//add[@key='CashDrawerID']");
            if (n == null)
            {
                n = xml.CreateElement("add");
                n.Attributes.Append(xml.CreateAttribute("key"));
                n.Attributes.Append(xml.CreateAttribute("value"));
                n.Attributes["key"].Value = cashDrawerID;
                n.Attributes["value"].Value = CashDrawerID;
                xml["configuration"]["appSettings"].AppendChild(n);
            }
            else
            {
                n.Attributes["value"].Value = CashDrawerID;
            }

            /* need to add receiptprintermodel. The node may or may not already exists */
            XmlNode xn = xml.SelectSingleNode("//add[@key='ReceiptPrinterModel']");
            if (xn == null)
            {
                xn = xml.CreateElement("add");
                xn.Attributes.Append(xml.CreateAttribute("key"));
                xn.Attributes.Append(xml.CreateAttribute("value"));
                xn.Attributes["key"].Value = receiptprintermodel;
                xn.Attributes["value"].Value = ReceiptPrinterModel;
                xml["configuration"]["appSettings"].AppendChild(xn);
            }
            else
            {
                xn.Attributes["value"].Value = ReceiptPrinterModel;
            }

            /* need to add receiptprintermodel. The node may or may not already exists */
            XmlNode typeNode = xml.SelectSingleNode("//add[@key='StoreType']");
            if (typeNode == null)
            {
                typeNode = xml.CreateElement("add");
                typeNode.Attributes.Append(xml.CreateAttribute("key"));
                typeNode.Attributes.Append(xml.CreateAttribute("value"));
                typeNode.Attributes["key"].Value = storetype;
                typeNode.Attributes["value"].Value = StoreType;
                xml["configuration"]["appSettings"].AppendChild(typeNode);
            }
            else
            {
                typeNode.Attributes["value"].Value = StoreType;
            }


            XmlNode xndd = xml.SelectSingleNode("//add[@key='" + thermalPrinterName + "']");
            if (xndd == null)
            {
                xndd = xml.CreateElement("add");
                xndd.Attributes.Append(xml.CreateAttribute("key"));
                xndd.Attributes.Append(xml.CreateAttribute("value"));
                xndd.Attributes["key"].Value = thermalPrinterName;
                xndd.Attributes["value"].Value = ThermalPrinterName;
                xml["configuration"]["appSettings"].AppendChild(xndd);
            }
            else
            {
                xndd.Attributes["value"].Value = ThermalPrinterName;
            }

            XmlNode xmlNode = xml.SelectSingleNode("//add[@key='" + eposExe + "']");
            if (xmlNode == null)
            {
                xmlNode = xml.CreateElement("add");
                xmlNode.Attributes.Append(xml.CreateAttribute("key"));
                xmlNode.Attributes.Append(xml.CreateAttribute("value"));
                xmlNode.Attributes["key"].Value = eposExe;
                xmlNode.Attributes["value"].Value = EPOSexe;
                xml["configuration"]["appSettings"].AppendChild(xmlNode);
            }
            else
            {
                xmlNode.Attributes["value"].Value = EPOSexe;
            }

            xmlNode = xml.SelectSingleNode("//add[@key='" + thermalPrintingEnabled + "']");
            if (xmlNode == null)
            {
                xmlNode = xml.CreateElement("add");
                xmlNode.Attributes.Append(xml.CreateAttribute("key"));
                xmlNode.Attributes.Append(xml.CreateAttribute("value"));
                xmlNode.Attributes["key"].Value = thermalPrintingEnabled;
                xmlNode.Attributes["value"].Value = ThermalPrintingEnabled.ToString();
                xml["configuration"]["appSettings"].AppendChild(xmlNode);
            }
            else
            {
                xmlNode.Attributes["value"].Value = ThermalPrintingEnabled.ToString();
            }

            //Write out the new config file.
            xml.Save(fileInfo.FullName);
        }

        public static bool ReadForRebate(Assembly asm)
        {
            bool status = true;

            try
            {
                _url = ConfigurationManager.AppSettings[serviceUrlTag];
                _country = ConfigurationManager.AppSettings[countryCode];
                _branch = ConfigurationManager.AppSettings[branchCode];
                _culture = ConfigurationManager.AppSettings[culture];
                _newinstall = Convert.ToBoolean(ConfigurationManager.AppSettings[newInstall]);
                _testkey = ConfigurationManager.AppSettings[testKey];
                _updatePath = ConfigurationManager.AppSettings[updatePath];
                _splashImage = ConfigurationManager.AppSettings[splashImage];
                _agrPrinter = ConfigurationManager.AppSettings[agrPrinter];
                _agrTray = ConfigurationManager.AppSettings[agrTray];
                _invoicePrinter = ConfigurationManager.AppSettings[invoicePrinter];
                _invoiceTray = ConfigurationManager.AppSettings[invoiceTray];
                _summaryPrinter = ConfigurationManager.AppSettings[summaryPrinter];
                _summaryTray = ConfigurationManager.AppSettings[summaryTray];
                _collPrinter = ConfigurationManager.AppSettings[collPrinter];
                _collTray = ConfigurationManager.AppSettings[collTray];

                _cashDrawerID = ConfigurationManager.AppSettings[cashDrawerID] == null ? "" : ConfigurationManager.AppSettings[cashDrawerID];
                _receiptprintermodel = ConfigurationManager.AppSettings[receiptprintermodel] == null ? "295" : ConfigurationManager.AppSettings[receiptprintermodel];
                _storetype = ConfigurationManager.AppSettings[storetype] == null ? "" : ConfigurationManager.AppSettings[storetype];
            }
            catch (Exception)
            {
                status = false;
                //Nothing we can sensibly do here, can't log it 
                //because we may not have the web service url
                //allow the status to reflect what to do next
            }
            return status;
        }
    }

    //public struct Credential
    //{
    //    public static string Name { get; set; }
    //    public static string User { get; set; }
    //    public static int UserId { get; set; }
    //    public static string Password { get; set; }
    //    public static string Cookie { get; set; }
    //    public static string[] Roles { get; set; }
    //    private static List<int> Permissions { get; set; }

    //    /// <summary>
    //    /// Warning: do not use this if yu do not know what you are doing!
    //    /// </summary>
    //    public static void SetPermissions(int[] ids)
    //    {
    //        Permissions = new List<int>(ids);
    //        Permissions.Sort();
    //    }

    //    public static bool IsInRole(string role)
    //    {
    //        bool found = false;
    //        if (role == "N")			//if administrator
    //            found = true;
    //        else
    //        {
    //            foreach (string r in Roles)
    //            {
    //                if (r == role)
    //                {
    //                    found = true;
    //                    break;
    //                }
    //            }
    //        }
    //        return found;
    //    }

    //    public static bool HasPermission(System.Enum value)
    //    {
    //        return Permissions.BinarySearch((int)((object)value)) >= 0;
    //    }
    //}
}
namespace STL.Common.Printing.AgreementPrinting
{
    public struct BookMarks
    {
        public const string AcctNo = "AcctNo";
        public const string Name = "Name";
        public const string FirstName = "FirstName";
        public const string CustID = "CustID";
        public const string CurrDate = "Date";
        public const string Addr1 = "Addr1";
        public const string Addr2 = "Addr2";
        public const string Addr3 = "Addr3";
        public const string PCode = "PCode";
        public const string Goods = "Goods";
        public const string Deposit = "Deposit";
        public const string Extended = "Extended";
        public const string Charge = "Charge";
        public const string Balance = "Balance";
        public const string Total = "Total";
        public const string Instal = "Instal";
        public const string Qty = "Qty";
        public const string Description = "Descr";
        public const string Item = "Item";
        public const string Price = "Price";
        public const string TaxInv = "TaxInv";
        public const string SerialNo = "SerialNo";
        public const string TaxRate = "TaxRate";
        public const string TaxName = "TaxName";
        public const string Value = "Value";
        public const string TaxAmount = "TaxAmount";
        public const string Incl = "Incl";
        public const string InvTotal = "InvTotal";
        public const string TotalVal = "TotalVal";
        public const string TotalTax = "TotalTax";
        public const string Branch = "Branch";
        public const string BankAccNo = "BankAccNo";
        public const string BankName = "BankName";
        public const string BankTime = "BankTime";
        public const string CurrAddrTime = "CurrAddrTime";
        public const string Dependants = "Dependants";
        public const string DOB = "DOB";
        public const string EmpAddr = "EmpAddr";
        public const string EmpStatus = "EmpStatus";
        public const string EmpTime = "EmpTime";
        public const string HomeTel = "HomeTel";
        public const string IDNum = "IDNum";
        public const string MobTel = "MobTel";
        public const string MStatus = "MStatus";
        public const string MthlyIncome = "MthlyIncome";
        public const string Occu = "Occu";
        public const string PrevAddrTime = "PrevAddrTime";
        public const string PropType = "PropType";
        public const string Residence = "Residence";
        public const string SPEmpStatus = "SPEmpStatus";
        public const string SPOccu = "SPOccu";
        public const string WorkTel = "WorkTel";
        public const string Employer = "Employer";
        public const string SPLimit = "SPLimit";
        public const string AVLimit = "AVLimit";
        public const string DatePurch = "DatePurch";
        public const string InstlAmt = "InstlAmt";
        public const string NumInstl = "NumInstl";
        public const string DateLast = "DateLast";
        public const string TotMthInstl = "TotMthInstl";
        public const string MthPayDate = "MthPayDate";
        public const string EmpTel = "EmpTel";
        public const string LastInstlAmt = "LastInstlAmt";
        public const string MinPay = "MinPay";
        public const string MPR = "MPR";
        public const string PRName = "PRName";
        public const string CusNotes = "Notes";
        public const string User = "User";
        public const string UserName = "UserName";
        public const string CustNotes = "Notes";
        public const string BuffNo = "BuffNo";
        public const string BranchNo = "BranchNo";
        public const string TimeStamp = "TimeStamp";
        public const string StockLocation = "StockLocation";
        public const string DeliveryText = "DelText";
    }

    public struct InstalDetails
    {
        public const string InstalSummary = "Balance payable by num\ninstalments of insamnt and a \nfinal instalment of finamnt";
        public const string SingInstal = "PART II ";
        public const string JamInstal = "\nAgreed minimum payment minpay";
    }

    public struct Templates
    {
        public const string AgreementTemplate = "Agreement.dot";
        public const string LineTemplate = "LineItems.dot";
        public const string InvoiceTemplate = "TaxInvoice.dot";
        public const string TaxLineTemplate = "TaxInvItems.dot";
        public const string SummaryTemplate = "RFSummary.dot";
        public const string DeliveryNotes = "DeliveryNotes.dot";
        public const string DeliveryNotesCont = "DeliveryNotesCont.dot";
        public const string AgreementTemplate97 = "Agreement97.dot";
        public const string LineTemplate97 = "LineItems97.dot";
        public const string InvoiceTemplate97 = "TaxInvoice97.dot";
        public const string TaxLineTemplate97 = "TaxInvItems97.dot";
        public const string SummaryTemplate97 = "RFSummary97.dot";
        public const string DeliveryNotes97 = "DeliveryNotes97.dot";
    }

    public struct DocumentType
    {
        public const string Agreement = "A";
        public const string TaxInvoice = "T";
        public const string Warranty = "W";
        public const string DeliveryNote = "D";
    }

    public struct Document
    {
        public const int Agreement = 1;
        public const int TaxInvoice = 2;
        public const int Summary = 3;
        public const int MaxAccts = 4;
        //public const int ItemsPerPage = 5; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public const int ItemsPerPage = 6; //IP - 15/05/08 - UAT(166) //CR1048 changed from 7 to 6 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public const int ItemsPerSummary = 7;
        public const int CollNote = 8;
        public const int JournalRowsPerPage = 34;
        public const int PickListItems = 20;
        public const int DeliveryScheduleItems = 10;
        public const int DNItemsPerPage = 5; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        //public const int TaxInvoiceItemsPerPage = 4; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
    }

    public struct PrintSetUp
    {
        public static string AgrPrinterName = "";
        public static string AgrSource = "";
        public static string InvPrinterName = "";
        public static string InvSource = "";
        public static string SumPrinterName = "";
        public static string SumSource = "";
        public static string CollPrinterName = "";
        public static string CollSource = "";
    }

    public struct XMLTemplates
    {
        public static string StatementOfAccountXML =
            "<STATEMENT>" +
            "<PAGE>" +
            "<HEADER>" +
            "<COUNTRYNAME/>" +
            "<DATE/>" +
            "<CUSTOMERNAME/>" +
            "<ADDR1/>" +
            "<ADDR2/>" +
            "<ADDR3/>" +
            "<POSTCODE/>" +
            "<INSTALMENT/>" +
            "<DUEDATE/>" +
            "<ACCTNO/>" +
            "<OUTSTANDINGBAL/>" +
            "<AGREEMENTTOTAL/>" +
            "<ARREARS/>" +
            "</HEADER>" +
            "<TRANSACTIONS>" +
            "<TRANSACTION>" +
            "<DATE/>" +
            "<TRANSTYPE/>" +
            "<DEBIT/>" +
            "<CREDIT/>" +
            "<BALANCE/>" +
            "</TRANSACTION>" +
            "</TRANSACTIONS>" +
            "<LAST/>" +
            "</PAGE>" +
            "</STATEMENT>";

        public static string StoreCardStatementXML =
            "<STATEMENT>" + "<PAGE>" +
            "<HEADER>" + "<COUNTRYNAME/>" +
            "<DATE/>" + "<CUSTOMERNAME/>" +
            "<ADDR1/>" + "<ADDR2/>" +
            "<ADDR3/>" + "<POSTCODE/>" +
            "<DUEDATE/>" + "<ACCTNO/>" +
            "<OUTSTANDINGBAL/>" + "<ARREARS/>" +
            "</HEADER>" + "<TRANSACTIONS>" +
            "<TRANSACTION>" + "<DATE/>" +
            "<TRANSTYPE/>" + "<DEBIT/>" +
            "<CREDIT/>" + "<BALANCE/>" +
            "</TRANSACTION>" + "</TRANSACTIONS>" +
            "<LAST/>" + "</PAGE>" +
            "</STATEMENT>";

        #region ScheduleOfPaymentsXML
        public static string ScheduleOfPaymentsXML =
            "<SCHEDULEOFPAYMENTS>" +
            "<HEADER>" +
            "<DEALERNAME/>" +
            "<CUSTOMERNAME/>" +
            "<DATE/>" +
            "<ACCTNO/>" +
            "<APR/>" +
            "<NOINSTALMENTS/>" +
            "<FIRSTDATE/>" +
            "<LASTDATE/>" +
            "<CHARGEABLEPRICE/>" +
            "<BALANCE/>" +
            "<TOTAL/>" +
            "<DEPOSIT/>" +
            "<AGRDATE/>" +
            "<LINEITEMS>" +
            "<LINEITEM>" +
            "<ITEMNO/>" +
            "<DESCR/>" +
            "</LINEITEM>" +
            "</LINEITEMS>" +
            "</HEADER>" +
            "<INSTALMENTS>" +
            "<INSTALMENT>" +
            "<NO/>" +
            "<DUEDATE/>" +
            "<MONTHLYINSTALMENT/>" +
            "<MONTHLYCHARGE/>" +
            "<MONTHLYCAPITAL/>" +
            "<OPENINGBALANCE/>" +
            "<CLOSINGBALANCE/>" +
            "</INSTALMENT>" +
            "</INSTALMENTS>" +
            "<FOOTER>" +
            "<TOTAL/>" +
            "<CHARGETOTAL/>" +
            "<CAPITALTOTAL/>" +
            "</FOOTER>" +
            "</SCHEDULEOFPAYMENTS>";
        #endregion

        #region ScheduleOfArrangementPaymentsXML
        public static string ScheduleOfArrangementPaymentsXML =
            "<SCHEDULEOFPAYMENTS>" +
            "<HEADER>" +
            "<DEALERNAME/>" +
            "<CUSTOMERNAME/>" +
            "<DATE/>" +
            "<ACCTNO/>" +
            "<APR/>" +
            "<NOINSTALMENTS/>" +
            "<FIRSTDATE/>" +
            "<LASTDATE/>" +
            "<CHARGEABLEPRICE/>" +
            "<BALANCE/>" +
            "<TOTAL/>" +
            "<DEPOSIT/>" +
            "<AGRDATE/>" +
            "<LINEITEMS>" +
            "<LINEITEM>" +
            "<ITEMNO/>" +
            "<DESCR/>" +
            "</LINEITEM>" +
            "</LINEITEMS>" +
            "</HEADER>" +
            "<INSTALMENTS>" +
            "<INSTALMENT>" +
            "<NO/>" +
            "<DUEDATE/>" +
            "<MONTHLYINSTALMENT/>" +
            "<MONTHLYCHARGE/>" +
            "<MONTHLYCAPITAL/>" +
            "<TOTALPAYMENT/>" +
            "<CLOSINGBALANCE/>" +
            "</INSTALMENT>" +
            "</INSTALMENTS>" +
            "<FOOTER>" +
            "<TOTAL/>" +
            "<CHARGETOTAL/>" +
            "<CAPITALTOTAL/>" +
            "</FOOTER>" +
            "</SCHEDULEOFPAYMENTS>";
        #endregion
        #region CashierTotalsSummaryXML
        public static string CashierTotalsSummaryXML =
            "<CASHIERTOTALS>" +
            "<BRANCH/>" +
            "<DATEFROM/>" +
            "<DATETO/>" +
            "<PAYMETHODS>" +
            "<PAYMETHOD>" +
            "<NAME/>" +
            "<SYSTEMVALUE/>" +
            "<USERVALUE/>" +
            "<DEPOSITVALUE/>" +
            "<DIFFERENCEVALUE/>" +
            "<SECURITISEDVALUE/>" +
            "</PAYMETHOD>" +
            "</PAYMETHODS>" +
            "<SYSTEMTOTAL/>" +
            "<USERTOTAL/>" +
            "<DEPOSITTOTAL/>" +
            "<DIFFERENCETOTAL/>" +
            "<SECURITISEDTOTAL/>" +
            "</CASHIERTOTALS>";
        #endregion
        #region CashierTotalsXML
        public static string CashierTotalsXML =
            "<CASHIERTOTALS>" +
            "<BRANCH/>" +
            "<EMPLOYEE/>" +
            "<EMPLOYEENAME/>" +
            "<DATEFROM/>" +
            "<DATETO/>" +
            "<TOTALVALUE/>" +
            "<SUBTOTAL/>" +
            "<TRANSACTIONS>" +
            "<TRANSACTION>" +
            "<EMPLOYEE/>" +
            "<TRANSTYPE/>" +
            "<PAYMETHOD/>" +
            "<TRANSVALUE/>" +
            "<ACCTNO/>" +
            "<CUSTNAME/>" +
            "<CHEQUENO/>" +
            "<BANKCODE/>" +
            "<BANKACCTNO/>" +
            "</TRANSACTION>" +
            "</TRANSACTIONS>" +
            "<TOTALVALS>" +
            "<TOTAL>" +
            "<PAYMETHOD/>" +
            "<SYSTEMTOTAL/>" +
            "<USERTOTAL/>" +
            "<DIFFERENCE/>" +
            "<REASON/>" +
            "<DESCRIPTION/>" +
            "<DEPOSIT/>" +
            "</TOTAL>" +
            "</TOTALVALS>" +
            "</CASHIERTOTALS>";
        #endregion
        #region otherboringXML
        public static string CollectionNoteRequestXML =
            "<COLLECTIONNOTE>" +
            "<ACCTNO/>" +
            "<BRANCHNO/>" +
            "<USER/>" +
            "<CUSTOMERID/>" +
            "<BUFFNO/>" +
            "<COLLDATE/>" +
            "<CULTURE/>" +
            "<DELADDRESS/>" +
            "<LINEITEMS/>" +
            "</COLLECTIONNOTE>";

        public static string CollectionItemRequestXML =
            "<LINEITEM>" +
            "<QUANTITY/>" +
            "<ITEMNO/>" +
            "<DESC1/>" +
            "<DESC2/>" +
            "<PRICE/>" +
            "<NOTES/>" +
            "<GRTNOTES/>" + // UAT 158 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<SUPPLIER/>" +  //CR 1048 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "</LINEITEM>";
        #endregion
        #region moreboringXML
        public static string DeliveryNoteRequestXML =
            "<DELIVERYNOTE>" +
            "<ACCTNO/>" +
            "<BRANCHNO/>" +
            "<USER/>" +
            "<SALESUSER/>" +
            "<PRINTTEXT/>" +
            "<DELNOTENUM/>" +
            "<CULTURE/>" +
            "<DATEREQDEL/>" +
            "<TIMEREQDEL/>" +
            "<DELADDRESS/>" +
            "<LOCATION/>" +
            "<BUFFBRANCHNO/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<CUSTID/>" +
            "<CUSTOMERNAME/>" +
            "<ALIAS/>" +
            "</DELIVERYNOTE>";

        public static string WarrantyContractXML =
            "<CONTRACT>" +
            "<COPY/>" +
            "<TODAY/>" +
            "<CONTRACTNO/>" +
            "<ACCTNO1/>" +
            "<ACCTNO2/>" +
            "<ACCTNO3/>" +
            "<ACCTNO4/>" +
            "<ACCTNO5/>" +
            "<ACCTNO6/>" +
            "<ACCTNO7/>" +
            "<ACCTNO8/>" +
            "<ACCTNO9/>" +
            "<ACCTNO10/>" +
            "<ACCTNO11/>" +
            "<ACCTNO12/>" +
            "<ACCTNO/>" +
            "<TITLE/>" +
            "<FIRSTNAME/>" +
            "<LASTNAME/>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<POSTCODE/>" +
            "<HOMETEL/>" +
            "<WORKTEL/>" +
            "<ITEMNO/>" +
            "<WARRANTYNO/>" +
            "<STORENO/>" +
            "<BRANCHNAME/>" +
            "<SOLDBY/>" +
            "<SOLDBYNAME/>" +
            "<DATEOFPURCHASE/>" +
            "<ITEMDESC1/>" +
            "<ITEMDESC2/>" +
            "<WARRANTYDESC1/>" +
            "<WARRANTYDESC2/>" +
            "<MANUFACTURERWARRANTYLENGTH/>" +
            "<ITEMPRICE/>" +
            "<WARRANTYPRICE/>" +
            "<PLANNEDDELIVERY/>" +
            "<STARTOFEXTENDEDWARRANTY/>" +
            "<EXPIRYOFWARRANTY/>" +
            "<SPACE/>" +
            "<LAST/>" +
            "<WARRANTYCREDIT/>" +
            "<TERMSTYPE/>" +
            "<COUNTRYNAME/>" +
            "<CUSTOMERID/>" +
            "</CONTRACT>";

        public static string RFAccountXML =
            "<ACCOUNT>" +
            "<ACCTNO/>" +
            "<DATEOPEN/>" +
            "<MONTHLYINSTALMENT/>" +
            "<NOOFINSTALMENTS/>" +
            "<DATEOFFINALINSTAL/>" +
            "<ITEMS/>" +
            "</ACCOUNT>";

        public static string RFTermsXML =
            "<RFTERMS>" +
            "<ACCOUNTNO/>" +
            "<NAME/>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<POSTCODE/>" +
            "<LIMIT/>" +
            "<VALID/>" +
            "</RFTERMS>";

        public static string RFSummaryXML =
            "<RFSUMMARY>" +
            "<CURRENTDATE/>" +
            "<CUSTDETAILS>" +
            "<CUSTID/>" +
            "<TITLE/>" +
            "<FIRSTNAME/>" +
            "<LASTNAME/>" +
            "<DOB/>" +
            "<HOMETEL/>" +
            "<WORKTEL/>" +
            "<MOBILETEL/>" +
            "<PROPTYPE/>" +
            "<CURRENTADDRESS>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<POSTCODE/>" +
            "</CURRENTADDRESS>" +
            "<TIMEINCURRADDRESS/>" +
            "<PREVADDRESS>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<POSTCODE/>" +
            "</PREVADDRESS>" +
            "<OCCUPATION/>" +
            "<EMPLOYER/>" +
            "<EMPLOYMENTSTAT/>" +
            "<EMPLOYERTEL/>" +
            "<TIMECURREMPLOYMENT/>" +
            "<MARITALSTAT/>" +
            "<SPOUSE>" +
            "<EMPLOYMENTSTAT/>" +
            "<OCCUPATION/>" +
            "</SPOUSE>" +
            "<DEPENDENTS/>" +
            "<BANKNAME/>" +
            "<TIMEATBANK/>" +
            "<BANKACCTNO/>" +
            "<MONTHLYINCOME/>" +
                        // rdb adding Malysia Fields
                        "<GENDER/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<ETHNICITY/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<NATIONALITY/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<RESIDENTIALSTATUS/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<MORTGAGE/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<ADDITIONALINCOME/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<UTILITIES/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<LOANS/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<VEHICLEREG/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<EMPLOYERADDRESS/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "</CUSTDETAILS>" +
            "<ACCTDETAILS>" +
            "<RFACCTNO/>" +
            "<SPENDINGLIMIT/>" +
            "<AVAILABLESPEND/>" +
            "<VALIDFROM/>" +
            "<VALIDTO/>" +
            "<ACCOUNTS/>" +
            "<TOTALMONTHLYINSTALMENT/>" +
            "<MONTHLYDUEDATE/>" +
            "</ACCTDETAILS>" +
            "</RFSUMMARY>";

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to print the Invoice.
        public static string TaxInvoicePayMethodXML =
            "<PAYMETHOD>" +
            "<DESCRIPTION/>" +
            "<AMOUNT/>" +
            "<TOTALAMTPAID2/>" +
            "</PAYMETHOD>";

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to print the Invoice.
        public static string TaxInvoiceItemXML =
            "<LINEITEM>" +
            "<NUMBER/>" +
            "<INDEX/>" +
            "<TYPE/>" +
            "<ITEMNO/>" +
            "<QUANTITY/>" +
            "<UNITPRICE/>" +		/* always inclusive of tax */
            "<ORDERVALUE/>" +	/* always inclusive of tax */
            "<DESC/>" +
            "<DESC2/>" +
            "<TAXRATE/>" +
            "<TAXAMOUNT/>" +		/* paid on orderValue not unitPrice */
            "<TERMSTYPE/>" +		/* used to identified warranties on credit */
            "<CONTRACTNO/>" +	/* used for warranties */
            "<ORDERVALUEEXTAX/>" +
            "<SERIALNOS/>" +
            "<RELATED/>" +
            "<NOTES/>" +        // CR 1048 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
             "<ORDERVALUEFULL/>" +
            "<TAXAMOUNTFULL/>" +
            "<TAXAMOUNTFOOTER/>" +
            "<CATEGORY/>" +
            "<TRIM/>" +
            "<MODEL/>" +
            "</LINEITEM>";

        public static string TaxInvoiceXML =
            "<TAXINVOICE>" +
            "<CREDITNOTE>False</CREDITNOTE>" +
            "<HEADER>" +
            "<REPRINTCOPY/>" +
            "<CUSTOMERID/>" +
            "<REPRINT/>" +
            "<DATE/>" +
            "<NOW/>" +
             "<Title/>" +
            "<FIRSTNAME/>" +
            "<LASTNAME/>" +
             "<DELTitleC/>" +
             "<DELFirstName/>" +
            "<DELLastName/>" +
            "<ADDR1/>" +
            "<ADDR2/>" +
            "<ADDR3/>" +
            "<POSTCODE/>" +
            // 25/04/08 rdb adding phone fields
            "<HOMEPHONE/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<MOBILEPHONE/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<WORKPHONE/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<WORKEXT/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<ACCTNO/>" +
            "<TAXNAME/>" +
            "<BRANCHNAME/>" +
            "<BRANCHADDR1/>" +
            "<BRANCHADDR2/>" +
            "<BRANCHADDR3/>" +
            // 25/04/08 rdb adding branchtel
            "<BRANCHTELNO/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<SERIALNO/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<SERIALNO/>" +
            "<SALETEXT/>" +
            "<COUNTRY/>" +//CR 2018-13
            "<BRANCHCITY/>" +
            "<REGNO/>" +
            "<BRANCHNO/>" +
            "<INVOICENO/>" +
            "<INVOICEDATE/>" +
            "<CASHIER/>" +
            "<SALESMAN/>" +
            "<CUSTOMERNAME/>" +
            "<CUSTOMERADDR/>" +
            "<ACCTNO/>" +
            "<ACCTBLNC/>" +
            "<AVAILABLESPEND/>" +//CR 2018-13  
            "<COLLCTION/>" +
            "<BUFFNO/>" +
            "<USER/>" +
            "<USERNAME/>" +
            "</HEADER>" +
            "<LINEITEMS/>" +
             "<PAYMETHODS/>" +
            "<PAY1/>" +
            //"<PAY2/>" +
            "<PAY2>" +
            "<TOTALAMTPAID/>" +
            "</PAY2>" +
            //"<TOTALAMTPAID/>" +
            "<FOOTER>" +
            "<EXTOTAL/>" +
            "<INCTOTAL/>" +
            "<TAXTOTAL/>" +
            //"<PAYMETHOD/>" +//CR 2018-13
            //"<TOTALAMTPAID/>" +//CR 2018-13
            "</FOOTER>" +
            "<AGRMNTFOOTER>" +
            "<FIRSTINST/>" +
            "<FINALINST/>" +
            "<INSTALNO/>" +
            "<INTERESTRATE/>" +
            "<INSTALMENTS/>" +
            "<GOODSVAL/>" +
            "<DEPOSIT/>" +
            "<CREDIT/>" +
            "<DT/>" +
            "<BALANCE/>" +
            "<TOTAL/>" +
            "<NINETYDAYS/>" +
            "<SERVICEPRINT/>" +
            "<TOPAY/>" +
            "</AGRMNTFOOTER>" +
            "<LAST/>" +
             "<AGRMTNO/>" +//CR 2018-13
            "</TAXINVOICE>"
            ;
        #endregion
        public static string DeliveryNoteXML =
            "<DELIVERYNOTE>" + "<HEADER>" +
            "<ADDRESS1/>" + "<ADDRESS2/>" +
            "<ADDRESS3/>" + "<POSTCODE/>" +
            "<HOMETEL/>" + "<WORKTEL/>" +
            "<MOBILE/>" + "<DELTEL/>" +
            "<ACCTNO/>" + "<CUSTOMERNAME/>" +
            "<CUSTID/>" + //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "<ALIAS/>" + "<BRANCH/>" +
            "<BUFFNO/>" + "<DELTEXT/>" +
            "<LOCATION/>" +
            "<PRINTED/>" +
            "<PRINTEDBY/>" +
            "<PRINTTEXT/>" +
            "<DELDATE/>" +
            "<CANCELTEXT/>" +
            "<NOTETEXT/>" +  //UAT 198 //IP - 08/02/10 - Malaysia Enhancements (CR1072)
            "</HEADER>" +
            "<LINEITEMS/>" +
            "<FOOTER>" +
            "<COD/>" +
            "<ADDCHARGES/>" +
            "<PAYABLE/>" +
            "<USER/>" +
            "<USERNAME/>" +
            "<CUSTNOTES/>" +
            "</FOOTER>" +
            "<LAST/>" +
            "</DELIVERYNOTE>";

        public static string OneForOneReplacementNoteXML =
            "<ONEFORONEREPLACEMENTNOTE>" +
            "<HEADER>" +
            "<ACCTNO/>" +
            "<BRANCHNO/>" +
            "<BUFFNO/>" +
            "<USER/>" +
            "<USERNAME/>" +
            "<NOTES/>" +
            "<PRINTED/>" +
            "</HEADER>" +
            "<PRODUCTDETAILS>" +
            "<PRODUCTDESCRIPTION/>" +
            "<REASON/>" +
            //"<ONEFORONETIMEPERIOD/>" +    //#17290
            "<DATERETURN/>" +
            "<ITEMNO/>" +
            "</PRODUCTDETAILS>" +
            "</ONEFORONEREPLACEMENTNOTE>";

        public static string AgreementXML =
            "<AGREEMENT>" +
            "<HEADER>" +
            "<ACCTNO/>" +
            "<DATE/>" +
            "<NAME/>" +
            "<JOINTNAME/>" +
            "<ADDR1/>" +
            "<ADDR2/>" +
            "<ADDR3/>" +
            "<POSTCODE/>" +
            "<CUSTID/>" +
            "<HOMETEL/>" +
            "<WORKTEL/>" +
            "<MOBILE/>" +
            "<RELATIONSHIP/>" +
            "</HEADER>" +
            "<LINEITEMS>" +
            "</LINEITEMS>" +
            "<FOOTER>" +
            "<FIRSTINST/>" +
            "<FINALINST/>" +
            "<INSTALNO/>" +
            "<INTERESTRATE/>" +
            "<INSTALMENTS/>" +
            "<GOODSVAL/>" +
            "<DEPOSIT/>" +
            "<CREDIT/>" +
            "<DT/>" +
            "<BALANCE/>" +
            "<TOTAL/>" +
            "<NINETYDAYS/>" +
            "<SERVICEPRINT/>" +
            "<TOPAY/>" +
            "<INSURANCE/>" +
            "</FOOTER>" +
            "<CUSTOMER/>" +
            "<LAST/>" +
            "</AGREEMENT>";

        public static string ActionSheetXML =
            "<ACTIONSHEET>" +
                "<HEADER>" +
           "<STORETYPE/>" +
                    "<TITLE/>" +
                    "<FIRSTNAME/>" +
                    "<LASTNAME/>" +
                     "<PHOTO/>" +
                     "<SIGNATURE/>" +
                    "<ADDR1/>" +
                    "<ADDR2/>" +
                    "<ADDR3/>" +
                    "<POSTCODE/>" +
                    "<ACCTNO/>" +
                    "<DELIVERYADDR1/>" +
                    "<DELIVERYADDR2/>" +
                    "<DELIVERYADDR3/>" +
                    "<DELIVERYPCODE/>" +
                    "<NAME/>" +
                    "<WORKADDR1/>" +
                    "<WORKADDR2/>" +
                    "<WORKADDR3/>" +
                    "<WORKPCODE/>" +
                    "<HOMETEL/>" +
                    "<MOBILE/>" +
                    "<MOBILE2/>" +
                    "<MOBILE3/>" +
                    "<MOBILE4/>" +
                    "<DELIVERYTEL/>" +
                    "<WORKTEL/>" +
                    "<NOTES/>" +
                    "<RFCUSTOMER/>" +
                    "<PRIVILEGECUSTOMER/>" +
                    "<INSTRUCTIONS/>" +
                    "<ALIAS/>" +
                "</HEADER>" +
                "<DETAILS>" +
                    "<COLLECTOR/>" +
                    "<DATE/>" +
                    "<BALANCE/>" +
                    "<ARREARS/>" +
                    "<COSTS/>" +
                    "<TOTAL/>" +
                    "<INSTALMENT/>" +
                    "<DUEDATE/>" +
                    "<RETURNDATE/>" +
                    "<DEADLINEDATE/>" +
                    "<DELIVERYDATE/>" +
                    "<DATELASTPAID/>" +
                    "<INTEREST/>" +
                    "<ARREARSLEVEL/>" +
                    "<PRINTCHARGES/>" +
                "</DETAILS>" +
                "<LINEITEMS>" +
                    "<TOTALVALUE/>" +
                "</LINEITEMS>" +
                "<CUSTOMERS>" +
                "</CUSTOMERS>" +
                "<SERVICEREQUESTS>" +
                "</SERVICEREQUESTS>" +
                "<LAST/>" +
            "</ACTIONSHEET>";

        public static string JournalEnquiryXML =
            "<JOURNALENQUIRY>" +
            "<PAGE>" +
            "<HEADER>" +
            "<DATE/>" +
            "</HEADER>" +
            "<TRANSACTIONS>" +
            "<TRANSACTION>" +
            "<BRANCH/>" +
            "<EMPLOYEE/>" +
            "<DATETRANS/>" +
            "<REFNO/>" +
            "<ACCTNO/>" +
            "<TRANSTYPE/>" +
            "<TRANSVALUE/>" +
            "</TRANSACTION>" +
            "</TRANSACTIONS>" +
            "<LAST/>" +
            "</PAGE>" +
            "<TOTALS>" +
            "<TOTAL>" +
            "<TRANSTYPE/>" +
            "<TRANSVALUE/>" +
            "</TOTAL>" +
            "</TOTALS>" +
            "</JOURNALENQUIRY>";

        public static string RebateForecastRequestXML =
            "<REBATEFORECAST>" +
            "<CULTURE/>" +
            "<PERIODS/>" +
            "</REBATEFORECAST>";

        public static string RebatePeriodsRequestXML =
            "<PERIOD>" +
            "<LEVEL/>" +
            "<P1/>" +
            "<P2/>" +
            "<P3/>" +
            "<P4/>" +
            "<P5/>" +
            "<P6/>" +
            "<P7/>" +
            "<P8/>" +
            "<P9/>" +
            "<P10/>" +
            "<P11/>" +
            "<P12/>" +
            "</PERIOD>";

        public static string PickListXML =
            "<PICKLIST>" +
            "<HEADER>" +
           "<STORETYPE/>" +
            "<PICKTEXT/>" +
            "<PICKNUMBER/>" +
            "<BRANCHNAME/>" +
            "<BRANCH/>" +
            "<DELNOTEBRANCH/>" +
            "<PRINTED/>" +
            "<USERNAME/>" +
            "<USER/>" +
            "<DELTEXT/>" +
            "<CATEGORYHEADING/>" +                      //IP - 22/09/11 - RI - #8224 - CR8201
            "</HEADER>" +
            "<DELIVERYNOTES>" +
            "</DELIVERYNOTES>" +
            "<LAST/>" +
            "</PICKLIST>";

        public static string BatchPrintXML =
            "<BATCHPRINT>" +
            "<HEADER>" +
            "<NAME/>" +
            "<SERVICEREQUESTNO/>" +
           "<PRINTLOCN/>" +      //CR 949/958
           "<ACTION/>" +         //CR 949/958
            "<ACCTNO/>" +
            "<HOMETEL/>" +
            "<WORKTEL/>" +
            "<MOBILETEL/>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<ADDRESSPC/>" +
            "<DIRECTIONS/>" +
            "<INSTRUCTIONS/>" + //IP - 05/08/08 - UAT5.1 - UAT(516) - Require 'Special Instructions' to be printed.
            "<PRODCODE/>" +
            "<PRODDESCRIPTION/>" +
            "<COMMENTS/>" +
            "<SLOTDATE/>" +
            "<SLOT/>" +
            "<DATELOGGED/>" +
            "<PURCHASEDATE/>" +
            "<DEPOSIT/>" +
            "<BALANCE/>" +
            "<EW/>" +
            "<FYW/>" +
            "<SERIALNO/>" +
            "<SERVICEBRANCHNO/>" +
            "<CHARGETOCUSTOMER/>" +
            "</HEADER>" +
            "<PARTS/>" +
            "<LAST/>" +
            "<FOOTER>" +
            "<WARRANTABLE/>" +
            "</FOOTER>" +
            "</BATCHPRINT>";

        public static string BatchPrintParts =
            "<PART>" +
            "<PARTNO/>" +
            "<QUANTITY/>" +
            "<TYPE/>" +
            "</PART>";

        public static string DeliveryScheduleRequest =
            "<DELIVERYSCHEDULE>" +
            "<LOADNO/>" +
            "<BRANCHNO/>" +
            "<DATEDEL/>" +
            "<TRUCKID/>" +
            "<DRIVERNAME/>" +
            "<PRINTED/>" +
            "<USER/>" +
            "<CULTURE/>" +
            "</DELIVERYSCHEDULE>";

        public static string DeliveryScheduleXML =
            "<DELIVERYSCHEDULE>" +
            "<HEADER>" +
            "<BRANCH/>" +
            "<DELIVERYDATE/>" +
            "<LOADNO/>" +
            "<TRUCKID/>" +
            "</HEADER>" +
            "<CUSTOMERS>" +
            "<CUSTOMER>" +
            "<LOCN/>" +     //#3467 jec 07/04/11
            "<BUFFNO/>" +
            "<NAME/>" +
            "<ACCTNO/>" +
            "<LINEITEMS>" +
            "<LINEITEM>" +
            "<QUANTITY/>" +
            "<ITEMNO/>" +
            "<DESC/>" +
            "<PRICE/>" +
            "</LINEITEM>" +
            "</LINEITEMS>" +
            "</CUSTOMER>" +
            "</CUSTOMERS>" +
            "</DELIVERYSCHEDULE>";

        public static string FoodLossXML =
           "<FOODLOSSES>" +
           "<FOODLOSS>" +
           "<HEADER>" +
           "<CUSTOMERID/>" +
           "<DATE/>" +
           "<NOW/>" +
           "<FIRSTNAME/>" +
           "<LASTNAME/>" +
           "<ADDR1/>" +
           "<ADDR2/>" +
           "<ADDR3/>" +
           "<POSTCODE/>" +
           "<HOMEPHONE/>" +
           "<MOBILEPHONE/>" +
           "<WORKPHONE/>" +
           "<WORKEXT/>" +
           "<ACCTNO/>" +
           "<MODELNO/>" +
           "<CONTRACTNO/>" +
           "<BRANCHNAME/>" +
           "<BRANCHADDR1/>" +
           "<BRANCHADDR2/>" +
           "<BRANCHADDR3/>" +
           "<BRANCHTELNO/>" +
           "<USER/>" +
           "<USERNAME/>" +
           "</HEADER>" +
           "<ITEMS/>" +
           "<TECHNICIANNOTES/>" +
           "<TECHNICIANNAME/>" +
           "<FOOTER>" +
           "<TOTAL/>" +
           "</FOOTER>" +
           "</FOODLOSS>" +
           "</FOODLOSSES>";

        public static string InstallationBookingPrintXML =
            "<BOOKING>" +
            "<HEADER>" +
            "<ACCTNO/>" +
            "<AGREEMENTNO/>" +
            "<BRANCHNO/>" +
            "<PURCHASEDATE/>" +
            "<INSTNO/>" +
            "<INSTDATE/>" +
            "<SLOTS/>" +
            "<DELDATE/>" +
            "<ITEMNO/>" +
            "<ITEMDESC/>" +
            "<HASWARRANTY/>" +
            "<MANUFACTURER/>" +
            "<MODELNO/>" +
            "<SERIALNO/>" +
            "<CUSTNAME/>" +
            "<HOMETEL/>" +
            "<WORKTEL/>" +
            "<ADDRESS1/>" +
            "<ADDRESS2/>" +
            "<ADDRESS3/>" +
            "<POSTCODE/>" + //NM/IP - 07/03/11 - #3278
            "<DIRECTION/>" +
            "<TECHNICIAN/>" +
            "</HEADER>" +
            "<FOOTER/>" +
            "<LAST/>" +
            "</BOOKING>";
    }
}

namespace STL.Common.Printing.CustomerCard
{
    public struct CardType
    {
        public const string strLong = "Long (30 lines)";
        public const string strShort = "Short (27 lines)";
        public const int rowsLong = 30;
        public const int rowsShort = 27;
        public const string typeLong = "L";
        public const string typeShort = "S";
    }
}

namespace STL.Common.PrivilegeClub
{
    public struct PCCustCodes
    {
        public const string Tier1 = "TIR1";
        public const string Tier2 = "TIR2";
    }

    public struct PCTranslate
    {
        public const string Tier1 = "Tier1";
        public const string Tier2 = "Tier2";
    }
}

namespace STL.Common.ServiceRequest
{
    public struct StockRepair
    {
        public const string CustomerId = "COURTS";
    }

    public struct ServiceType
    {
        public const string All = "A";
        public const string Courts = "C";
        public const string CourtsBySR = "CSR";
        public const string NonCourts = "N";
        public const string NonCourtsBySR = "NSR";
        public const string Stock = "S";
        //CR 949/958
        public const string Cash_Go = "G";
    }

    public struct ServiceTypeText
    {
        public const string Courts = "Courts";
        public const string NonCourts = "Non-Courts";
        public const string Stock = "Stock";
        public const string Cash_Go = "Cash&Go";    //CR1030 jec
        public const string All = "All";    //CR1030 jec
    }

    public struct ServiceStatus
    {
        public const string NoSR = "";
        public const string New = "N";
        public const string Deposit = "D";
        public const string Estimate = "E";
        public const string BERReplacement = "B";
        public const string Allocation = "A";
        public const string Resolution = "R";
        public const string Closed = "C";
        public const string CommentUpdate = "U";
        // CR 949/958 New status "To Be Allocated" for after the Repair button has been clicked and before a technician has been allocated.
        public const string ToBeAllocated = "T";
        public const string TechnicianAllocated = "H"; //CR 1024 (NM 23/04/2009)
        public const string AllocatedToSupplier = "S"; //CR 1024 (NM 23/04/2009)
    }

    public struct ServiceStatusText
    {
        public const string New = "New";
        public const string Deposit = "Deposit Required";
        public const string Estimate = "Estimate Required";
        public const string BERReplacement = "BER Replacement";
        public const string Allocation = "Work In Progress";
        public const string Resolution = "Resolved";
        public const string Closed = "Settled";
        public const string CommentUpdate = "Comment Updated";
        // CR 949/958 New status "To Be Allocated" for after the Repair button has been clicked and before a technician has been allocated.
        public const string ToBeAllocated = "To Be Allocated";
        public const string TechnicianAllocated = "Technician Allocated"; //CR 1024 (NM 23/04/2009)
        public const string AllocatedToSupplier = "Allocated To Supplier"; //CR 1024 (NM 23/04/2009)
    }

    public struct ServiceChargeToAuthorisation
    {
        public const string NeverAllow = "X";
        public const string AuthorisationRequired = "A";
        public const string NoAuthorisationRequired = "N";
    }

    public struct ServiceChargeToAuthorisationText
    {
        public const string NeverAllow = "Never Allowed";
        public const string AuthorisationRequired = "Authorisation Required";
        public const string NoAuthorisationRequired = "No Authorisation Required";
    }

    public struct ServiceEvaluation
    {
        public const string NoFault = "NFF";
        public const string Misuse = "MIS";
        public const string NotCovered = "EVC";
        public const string WarrantyCover = "WAR";
        public const string CallAbandoned = "CAB";
    }

    public struct ServiceResolution
    {
        public const string NoFault = "NFF";
        public const string Misuse = "MIS";
        public const string NotCovered = "EVC";
        public const string Installation = "INS";
        public const string Mechanical = "MEC";
        public const string Electrical = "ELE";
        public const string Upholstery = "UPH";
        public const string Cosmetic = "COS";
        public const string BER = "BER";
        public const string SaveCall = "SAC";
    }

    public struct ServiceAnalysis
    {
        public const short PartsCourts = 1;
        public const short PartsOther = 2;
        public const short PartsTotal = 3;
        public const short LabourTotal = 4;
        public const short TotalCost = 5;
    }

    public struct ServiceAcct
    {
        public const string Internal = "I";
        public const string Warranty = "W";
        public const string Supplier = "S";
        public const string Deliverer = "D";
        public const string Customer = "C";
    }

    public struct ServiceChargeTo
    {
        public const string Internal = "INT";
        public const string Deliverer = "DEL";
        public const string Customer = "CUS";
        public const string Supplier = "SUP";
        public const string EW = "EW";     //CR1030 jec   
    }

    public struct ServiceMarkUp
    {
        public const decimal EW = 20;
    }

    public struct TechnicianBooking
    {
        public const string ServiceEstimate = "E";
        public const string ServiceRepair = "R";
        public const string Installation = "I";
    }

    public struct ServiceReport
    {
        public const string Unallocated = "Unallocated";
        public const string AwaitingDeposit = "Awaiting Deposit";
        public const string NotUpdatedSince = "Not Updated Since";
        public const string ByTechnician = "By Technician";
        public const string ByDateLodged = "By Date Logged";
        public const string AwaitingSpareParts = "Awaiting Spare Parts";
        public const string AwaitingPayment = "Awaiting Payment";
        public const string AwaitingEstimate = "Awaiting Estimate";
        public const string RepairOverdue = "Repair Overdue";        //CR1030 jec
        public const string ReassignTechnician = "Reassign Technician";        //CR1030 jec
        public const string ResoldItem = "Resold Item";        //CR1030 jec
        public const string FoodLoss = "Food Loss";        //CR1030 jec
        public const string RepairTotalExceeded = "Previous Repair Total Exceeded"; //CR1030 jec
    }

    public struct ServiceReplacementStatus
    {
        public const string Authorised = "A";
        public const string GoodsReturned = "G";
        public const string Delivered = "D";
    }


    public struct PaymentStatus
    {
        public const string AvailableForPayment = "";
        public const string OnHold = "H";
        public const string Deleted = "D";
        public const string Paid = "P";
    }

    public struct CustomerDetails
    {
        public string customerID;
        public string title;
        public string firstName;
        public string lastName;
        public string address1;
        public string address2;
        public string address3;
        public string postcode;
        public string directions;
        public string homePhone;
        public string workPhone;
        public string mobile;
        public short branchNo;
    }

}

namespace STL.Common.Collections
{
    public struct Actions
    {
        public const string SendSMS = "Send SMS";
        public const string SendLetter = "Send Letter";
        public const string SendToWorklist = "Send to Worklist";
        public const string SendToStrategy = "Send to Strategy";
        public const string Letter = "Letter";
        public const string SMS = "SMS";
        public const string Strategy = "Strategy";
        public const string Worklist = "Worklist";
    }

    public struct Operators
    {
        public const string Equal = "Equal to =";
        public const string GreaterThan = "Greater than >";
        public const string LessThan = "Less than <";
        public const string GreaterThanEqualTo = "Greater than or equal to >=";
        public const string LessThanEqualTo = "Less than or equal to <=";
        public const string NotEqualTo = "Not equal to <>";
        public const string Between = "Between";
    }

    public struct OperatorSigns
    {
        public const string Equal = "=";
        public const string GreaterThan = ">";
        public const string LessThan = "<";
        public const string GreaterThanEqualTo = ">=";
        public const string LessThanEqualTo = "<=";
        public const string NotEqualTo = "<>";
        public const string Between = "Between";
    }

    public struct ActivateName
    {
        public const string Activate = "Activate";
        public const string Deactivate = "De-activate";
    }

    public struct DropDownValues
    {
        public const string ExitStrategy = "Select an exit strategy";
        public const string Operator = "Select an operator";
        public const string PreviousStrategy = "Select a previous strategy";
    }
}

namespace STL.Common.Constants.ImportFiles
{
    public struct IF
    {
        public const string Product = "\\bmsfcprd.dat";
        public const string KitProduct = "\\bmsfckit.dat";
        public const string PromotionalPrice = "\\bmsfcprm.dat";
        public const string StockQty = "\\bmsfpstk.DAT";
        public const string PurchaseOrder = "\\bmsfpord.dat";
        public const string AssociatedProducts = "\\bmsfaprd.dat";

        public const string NonStockProduct = "nonstocks_prod.dat";
        public const string NonStockPromotionalPrice = "nonstocks_promo.dat";
        public const string NonStockAssociatedProducts = "nonstocks_prodAssoc.dat";
    }
}

namespace STL.Common.Constants.AuditSource
{
    public class AS
    {
        public static readonly string CancelAccount = "CancelAccount";
        public static readonly string GRTExchange = "GRTExchange";
        public static readonly string GRTExchangeRev = "GRTExchangeRev";
        public static readonly string GRTCancel = "GRTCancel";
        public static readonly string GRTCancelRev = "GRTCancelRev";
        public static readonly string NewAccount = "NewAccount";
        public static readonly string Revise = "Revise";
        //uat88 rdb 3/3/08 extra audit values required
        public static readonly string SDRemove = "SDRemove"; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public static readonly string Replacement = "Replacement"; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public static readonly string DiscountDelivered = "DisDel"; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public static readonly string WarrantyDelivered = "WarDel"; //IP - 08/02/10 - Malaysia Enhancements (CR1072)
        public static readonly string DeliveryNotification = "DelNotification"; //IP - 24/02/09 - CR929 & 974
        public static readonly string ChangeOrder = "ChangeOrder";  //IP - 27/05/11 - CR1212 - RI
    }

    //IP - 03/02/10 - CR1072 - 3.1.9
    public struct DASource
    {
        public static readonly string Manual = "Manual";
        public static readonly string Auto = "Auto";
    }
}

namespace STL.Common.Constants.StoreInfo
{
    public struct StoreType
    {
        public static readonly string Courts = "C";
        public static readonly string NonCourts = "N";
        public static readonly string All = "A";
    }
}
// added jec 22/11/07
namespace STL.Common.Constants.CountryParameters
{
    public struct Codename
    {
        public static readonly string MaxArrearsLevel = "MaxArrearsLevel";
        public static readonly string MaxExceedCRLimit = "MaxExceedCRLimit";        //CR1113 
        public static readonly string SRAcctName = "SRAcctName";        //CR1030

    }
}
namespace STL.Common.Constants
{
    public static class Users
    {
        public const int ICAutoDA = -113;
        public const int RIExport = -115;               //IP - 16/06/11 - CR1212 - RI
    }
}

namespace STL.Common.Constants.RItableNames
{

    // RI Table names  jec 11/04/11
    public struct RI
    {
        public static readonly string RICommittedStock = "RICommittedStock";
        public static readonly string RIDeliveryTransfers = "RIDeliveryTransfers";
        public static readonly string RIDeliveriesReturns = "RIDeliveriesReturns";
        public static readonly string RICommittedStockRepo = "RICommittedStockRepo";
        public static readonly string RIDeliveryTransfersRepo = "RIDeliveryTransfersRepo";
        public static readonly string RIDeliveriesReturnsRepo = "RIDeliveriesReturnsRepo";
        public static readonly string RIRepossessions = "RIRepossessions";
        public static readonly string RItemp_RawKitload = "RItemp_RawKitload";
        public static readonly string RItemp_RawPOload = "RItemp_RawPOload";
        public static readonly string RItemp_RawStkQtyload = "RItemp_RawStkQtyload";
        public static readonly string RItemp_RawStkQtyloadRepo = "RItemp_RawStkQtyloadRepo";
        public static readonly string RItemp_RawProductload = "RItemp_RawProductload";
        public static readonly string RItemp_RawProductloadRepo = "RItemp_RawProductloadRepo";
        public static readonly string RItemp_RawProductImport = "RItemp_RawProductImport";
        public static readonly string RItemp_RawProductImportRepo = "RItemp_RawProductImportRepo";
        public static readonly string RItemp_RawProductHeirarchy = "RItemp_RawProductHeirarchy";          // jec 15/06/11

    }
}


namespace STL.Common.Constants
{
    public static class ErrorMessages
    {
        public const string SanctionDropErrorText = "The previously selected value is no longer available. Please select another.";
    }
}

//IP - 06/05/11 - StoreCard - Feature - #3004
namespace STL.Common.Constants.StoreCard
{

    public struct StoreCardSource
    {
        public const string Preapproval = "PreApprove";
        public const string NewAccount = "NewApp";
        public const string Additional = "Additional";
        public const string Replacement = "Replacement";
    }
}

namespace STL.Common.Constants.CashLoans
{
    public struct CashLoanStatus
    {
        public const string Disbursed = "D";
        public const string Referred = "R";
        public const string Cancelled = "C";
        public const string PromissoryPrinted = "P";
        public const string LowAvailableSpend = "L";
    }

    public struct CashLoanBlockedStatus
    {
        public const string Blocked = "B";
        public const string UnBlocked = "U";
        public const string NotBlocked = "";
        public const string NotQualified = "NQ";
    }
}
//Equifax Score card type
namespace STL.Common.Constants.EquifaxScorecard
{
    public struct EquifaxScorecardStatus
    {
        public const string EquifaxApplicant = "C";
        public const string EquifaxBehavioural = "D";
    }
}


