using System;
using System.Collections;
using System.Data;
using STL.Common.Constants.ColumnNames;
using System.Diagnostics;
using System.Text;


namespace STL.Common
{
	public enum CountryParameterType
	{
		Text,
		DropDown,
		CheckBox,
		Numeric,
		DateTime,
		MultiText
	}

	public class CountryParameterCollection : Hashtable
	{
		public const string CacheKey = "CountryParameters";
		public const string CachedDatasetKey = "CountryParametersDataset";
		public static string CountryCacheKey(string countryCode)
		{
			return CacheKey + countryCode;
		}

		public CountryParameterCollection()
		{
		}

		public static event EventHandler<CountryParamEventArgs> CountryParamNotFound;

		public static void OnCountryParamNotFound(string CountryParamName)
		{
			if (CountryParamNotFound != null)
			{
				CountryParamNotFound(null, new CountryParamEventArgs(CountryParamName));
			}
		}

		public CountryParameterCollection(DataTable dt)
		{
		 int i = 0;
		 try
		 {
			foreach (DataRow r in dt.Rows)
			{
			   i = dt.Rows.IndexOf(r);
			   CountryParameterType t = CountryParameterType.Text;
			   switch ((string)r[CN.Type])
			   {
				  case "text": t = CountryParameterType.Text;
					 break;
				  case "checkbox": t = CountryParameterType.CheckBox;
					 break;
				  case "datetime": t = CountryParameterType.DateTime;
					 break;
				  case "dropdown": t = CountryParameterType.DropDown;
					 break;
				  case "numeric": t = CountryParameterType.Numeric;
					 break;
				  case "MultiText": t = CountryParameterType.MultiText;
					 break;
				  default:
					 break;
			   }
			   CountryParameter p = new CountryParameter((int)r[CN.ParameterID],
				  (string)r[CN.CountryCode],
				  (string)r[CN.ParameterCategory],
				  (string)r[CN.name],
				  r[CN.Value],
				  t, (int)r[CN.Precision],
				  (string)r["CodeName"]);
			   this[(string)r["CodeName"]] = p;
			}
		 }
		 catch (Exception ex)
		 {
			System.Diagnostics.Debug.Write(ex.Message + dt.Rows[i][CN.Type].ToString() + dt.Rows[i][CN.ParameterID].ToString());
		 }
		}

		
public object this[string key]
		{

			get
			{
				object o = null;

				CountryParameter c = (CountryParameter)base[key];

				if (c == null)
				{
					OnCountryParamNotFound(key);
				}
				else
				{
					switch (c.Type)
					{
						case CountryParameterType.CheckBox: o = Convert.ToBoolean(c.Value);
							break;
						case CountryParameterType.DateTime: o = Convert.ToDateTime(c.Value);
							break;
						case CountryParameterType.Numeric: o = Convert.ToDecimal(c.Value);
							break;
						default: o = Convert.ToString(c.Value);
							break;
					}
				}
				return o;
			}

			set
			{
				if (value is CountryParameter)
					base[key] = value;
			}
		}
		   


		public string this[string key, string property]
		{
			get
			{
				string propertyValue = "";

				CountryParameter c = (CountryParameter)base[key];
				switch(property)
				{
					case CN.ParameterCategory:
						propertyValue = c.ParameterCategory;
						break;
					case CN.name:
						propertyValue = c.Name;
						break;
					case CN.Precision:
						propertyValue = Convert.ToString(c.Precision);
						break;
					default:
						break;
				}
				return propertyValue;
			}

			//set
			//{
			//    if(value is CountryParameter)
			//        base[key] = value;
			//}
		}

		public event Action<Exception> GetCountryParameterValueException;

		public T GetCountryParameterValue<T>(string parameterName)
		{
			try
			{
				Type type = typeof(T);

				if (type == typeof(string))
					return (T)this[parameterName];
				else if (type == typeof(bool))
					return (T)Convert.ChangeType(bool.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(int))
					return (T)Convert.ChangeType(decimal.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(uint))
					return (T)Convert.ChangeType(decimal.Parse(this[parameterName].ToString().ToString()), type);
				else if (type == typeof(short))
					return (T)Convert.ChangeType(decimal.Parse(this[parameterName].ToString().ToString()), type);
				else if (type == typeof(ushort))
					return (T)Convert.ChangeType(decimal.Parse(this[parameterName].ToString().ToString()), type);
				else if (type == typeof(decimal))
					return (T)Convert.ChangeType(decimal.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(float))
					return (T)Convert.ChangeType(float.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(double))
					return (T)Convert.ChangeType(double.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(sbyte))
					return (T)Convert.ChangeType(sbyte.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(byte))
					return (T)Convert.ChangeType(byte.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(char))
					return (T)Convert.ChangeType(char.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(long))
					return (T)Convert.ChangeType(long.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(ulong))
					return (T)Convert.ChangeType(ulong.Parse(this[parameterName].ToString()), type);
				else if (type == typeof(DateTime))
					return (T)Convert.ChangeType(DateTime.Parse(this[parameterName].ToString()), type);
				else
					return (T)this[parameterName];
			}
			catch (InvalidCastException ex)
			{
				OnCountryParamNotFound(parameterName);
				throw ex;
			}
			catch (FormatException ex)
			{
				OnCountryParamNotFound(parameterName);
				throw ex;
			}
			catch (OverflowException)
			{
				OnCountryParamNotFound(parameterName);

				return default(T);
			}
			catch (Exception)
			{
				OnCountryParamNotFound(parameterName);

				return default(T);
			}
		}
	}

	/// <summary>
	/// Class to represent a row in the CountryMaintenance table
	/// </summary>
	public class CountryParameter
	{
		private int _parameterID = 0;
		public int ParameterID
		{
			get{ return _parameterID;}
		}

		private string _countryCode = "";
		public string CountryCode
		{
			get{ return _countryCode;}
		}

		private string _parmCategory = "";
		public string ParameterCategory
		{
			get{ return _parmCategory;}
		}

		private string _name = "";
		public string Name
		{
			get{ return _name;}
		}

		private object _value = "";
		public object Value
		{
			get{ return _value;}
		}

		private CountryParameterType _type = CountryParameterType.Text;
		public CountryParameterType Type
		{
			get{ return _type;}
		}

		private int _precision = 0;
		public int Precision
		{
			get{ return _precision;}
		}

		private string _codeName = "";
		public string CodeName
		{
			get{ return _codeName;}
		}

		public CountryParameter(int parameterID,
			string countryCode,
			string parmCategory,
			string name,
			object val,
			CountryParameterType type,
			int precision,
			string codeName )
		{
			this._parameterID = parameterID;
			this._countryCode = countryCode;
			this._parmCategory = parmCategory;
			this._name = name;
			this._value = val;
			this._type = type;
			this._precision = precision;
			this._codeName = codeName;
		}

		public CountryParameter()
		{
			
		}

		public enum StatementPrintTypes
		{
			LaserPrinter = 0,
			ReceiptPrinter = 1
		}
	}

	public class CountryParameterNames
	{
		public static string OrigBr = "origbr";
		public static string AffinityMaxTerm = "affinitymaxterm";
		public static string BCPpath = "BCPpath";                   //IP - 21/03/11 - #3352
		public static string CountryCode = "countrycode";
		public static string CountryName = "countryname";
		public static string HOBranchNo = "hobranchno";
		public static string ChequeDays = "cheqdays";
		public static string LetterDays = "letterdays";
		public static string LetterPorw = "letterporw";
		public static string TaxType = "taxtype";
		public static string TaxRate = "taxrate";
		public static string TaxName = "taxname";
		//public static string ServPcent = "servpcent";
		public static string DateRun = "daterun";
		public static string DateWeek1 = "dateweek1";
		public static string WeekNo = "weekno";
		public static string BatchControlNo = "batchctrlno";
		public static string BailFee = "bailfee";
		public static string BailPcent = "bailpcent";
		public static string ReceiptsPerBook = "rcptsperbook";
		public static string WeekLst400 = "weeklst400";
		public static string TaxInvType = "taxinvtype";
		public static string AgreementTaxType = "agrmttaxtype";
		public static string TermsTypeBandDefault = "TermsTypeBandDefault";
		public static string TermsTypeBandEnabled = "TermsTypeBandEnabled";
		public static string TierPCEnabled = "TierPCEnabled";
		public static string ClubAverageStatus = "ClubAverageStatus";
		public static string WarningAverageStatus = "WarningAverageStatus";
		public static string WarningLeadDays = "WarningLeadDays";
		public static string ConsecutiveInstalments = "ConsecutiveInstalments";
		public static string CreditExpiryMonths = "CreditExpiryMonths";
		public static string MinClubLen = "MinClubLen";
		public static string ReminderDays = "ReminderDays";
		public static string Tier1CashLen = "Tier1CashLen";
		public static string Tier1CashSpend = "Tier1CashSpend";
		public static string Tier1CashMaintainLen = "Tier1CashMaintainLen";
		public static string Tier1CashMaintainSpend = "Tier1CashMaintainSpend";
		public static string Tier1CreditLen = "Tier1CreditLen";
		public static string Tier2CashLen = "Tier2CashLen";
		public static string Tier2CashSpend = "Tier2CashSpend";
		public static string Tier2CashMaintainLen = "Tier2CashMaintainLen";
		public static string Tier2CashMaintainSpend = "Tier2CashMaintainSpend";
		public static string Tier2CreditLen = "Tier2CreditLen";
		public static string Tier2Discount = "Tier2Discount";
		public static string Tier2DiscountItemNumber = "Tier2DiscountItemNumber";
		public static string DateLastScore = "datelastscor";
		public static string MinHPage = "minhpage";
		public static string MaxHPage = "maxhpage";
		public static string DateLastCalc = "datelastcalc";
		public static string DailyExport = "dailyexport";
		public static string GlobalDeliveryPcent = "globdelpcent";
		public static string OneMonthDelivery = "onemonthdel";
		public static string DeliveryDays = "deldays";
		public static string ArrearsOnPay = "arrearsonpay";
		public static string TotalPaymentCommission = "totalpaymentcommission";
		public static string CurStatusCommission = "curstatuscommission";
		public static string SystemOpen = "systemopen";
		public static string Timing = "timing";
		public static string AddToMin = "addtomin";
		public static string AddToTerm = "addtoterm";
		public static string AdminFee = "adminfee";
		public static string AutoDownSC5 = "autodownsc5";
		public static string CashInterestRate = "cashintrate";
		public static string DateChargesStart = "datechargesstart";
		public static string LastChargesWeekNo = "lastchargesweekno";
		public static string LastYearLastWeekNo = "lastyrlastweekno";
		public static string LettersGap = "lettersgap";
		public static string PercentAddTo1 = "percentaddto1";
		public static string PercentAddTo2 = "percentaddto2";
		public static string PercentAddTo3 = "percentaddto3";
		public static string SmallBalance = "smallbalance";
		public static string DutyFree = "dutyfree";
		public static string LoyaltyCard = "loyaltycard";
		public static string LockTimeout = "locktimeout";
		public static string RebatePcent = "rebpcent";
		public static string DeliverySlots = "deliveryslots";
		public static string DefaultDeliveryNoteBranch = "defdelnotebranch";
		public static string MinPeriod = "minperiod";
		public static string DDLeadTime = "ddleadtime";
		public static string DDFee = "ddfee";
		public static string DDEnabled = "ddenabled";
		public static string DDBankAccountNo = "ddbankacctno";
		public static string HighStatus = "highstatus";
		public static string PaymentMethod = "paymentmethod";
		public static string AgrgPrint = "agrgprint";
		public static string AgrTimePrint = "agrtimeprint";
		public static string Print90 = "print90";
		public static string ServicePrint = "serviceprint";
		public static string NoCents = "nocents";
		public static string DDGenNo = "ddgenno";
		public static string DDServiceType = "ddservicetype";
		public static string DDIDCodeFrom = "ddidcodefrom";
		public static string DDIDCodeTo = "ddidcodeto";
		public static string DDBankBranch = "ddbankbranch";
		public static string DDBankAccountName = "ddbankacctname";
		public static string DDBankAccountType = "ddbankaccttype";
		public static string DDTest = "ddtest";
		public static string VarStamp = "varstamp";
		public static string NoTaxCopies = "notaxcopies";
		public static string NoAgrCopies = "noagrcopies";
		public static string AgrPrintType = "agrprinttype";
		public static string DDMaxRejections = "ddmaxrejections";
		public static string DDFeeOverride = "ddfeeoverride";
		public static string FixedDateFirst = "fixeddatefirst";
		public static string TransactEnabled = "transactenabled";
		public static string TransactURL = "transacturl";
		public static string AllowZeroStock = "allowzerostock";
		public static string CODDefault = "coddefault";
		public static string DecimalPlaces = "decimalplaces";
		public static string SanctionMinYears = "sanctionminyears";
		public static string AutomaticWarrantyNo = "mandwarrantyno";
		public static string ServicePrintDP = "serviceprintdp";
		public static string ManualRefer = "manualrefer";
		public static string LaserPrintTax = "laserprinttax";
		public static string PrintWarrantyContract = "printwarrantycontract";
		public static string CODPercentage = "codpercentage";
		public static string NonInterestItem = "noninterestitem";
		public static string PrintCreditNote = "printcreditnote";
		public static string WarrantyCustCopy = "warrantycustcopy";
		public static string WarrantyCreditCopy = "warrantycreditcopy";
		public static string WarrantyHOCopy = "warrantyhocopy";
		public static string DefaultDelDays = "defaultdeldays";
		public static string PercentToPay = "percenttopay";
		public static string Securitisation = "securitisation";
		public static string NoRFSummary = "norfsummary";
		public static string NoRFDetails = "norfdetails";
		public static string CreditScanInterval = "creditscaninterval";
		public static string SecureRefunds = "securerefunds";
		public static string WarrantyStylesheet = "warrantystylesheet";
		public static string MinReferences = "minreferences";
		public static string WarrantyCreditDays = "creditwarrantygrace";
		public static string PrintScheduleOfPayments = "printscheduleofpayments";
		public static string PrintSPAscheduleOfPayments = "printSPAscheduleofpayments";     //UAT1012   jec
		public static string CancellationRejectionCode = "cancellationcode";
		public static string PrintToolBar = "printtoolbar";
		public static string GiftVoucherAccount = "giftvoucheraccount";
		public static string DefaultVoucherExpiry = "defaultvoucherexpiry";
		public static string BailiffCommissionEqualsFee = "bcommissionequalsfee";
		public static string PayWholeUnits = "paywholeunits";
		public static string OpenCashDrawerForCredit = "opencashtillforcredit";
		public static string CaptureLandlordDetails = "capturelandlorddetails";
		public static string CancellationNotes = "cancellationnotes";
		public static string AllowSafeDeposits = "allowsafedeposits";
		public static string CreditCardsElectronic = "creditcardselectronic";
		public static string DebitCardsElectronic = "debitcardselectronic";
		public static string CreditCardsConsolidated = "creditcardsconsolidated";
		public static string DebitCardsConsolidated = "debitcardsconsolidated";
		public static string AdminChargeItem = "adminitemno";
		public static string InsuranceChargeItem = "insitemno";
		public static string AuditDataPeriod = "auditdataperiod";
		public static string AgreementCustomerCopies = "customeragreementcopies";
		public static string AgreementBranchCopies = "branchagreementcopies";
		public static string RestrictRepossessions = "restrictrepossessions";
		public static string AllowCashAndGoCheques = "cashandgocheques";
		public static string AllowDAUnpaid = "allowdaunpaid";
		public static string CustomerIDMask = "custidmask";
		public static string DiscountPercentage = "discountpcent";
		public static string AutoPaymentHoliday = "autopaymentholiday";
		public static string PaymentCardPrompt = "paymentcardprompt";
		public static string PrintRFCreditLimit = "printrfcreditlimit";
		public static string LoggingEnabled = "loggingenabled";
		public static string TallymanServerDB = "tallymanserverdb";
		public static string LinkToTallyman = "tmlink";
		public static string CurrencySymbolForPrint = "currencysymbolforprint";
		public static string SetMinRFLimit = "setminrflimit";
		public static string DoPotentialSpend = "dopotentialspend";
		public static string CreditLetterMonths = "creditlettermonths";
		public static string MinSpendAmount = "minspendamount";
		public static string NumAccts = "numaccts";
		public static string AutoReturnCodes = "autoreturncodes";
		public static string DepositUniqueReference = "deposituniqueref";
		public static string AddToRate = "addtorate";
		public static string DelNoteCancel = "delnotecancel";
		public static string PasswordChangeDays = "passwordchangedays";
		public static string PasswordMinLength = "PasswordMinLength";
		public static string WarrantyCustomerDetails = "warrantycustomerdetails";
		public static string ReplacementCredit = "replacementcredit";
		public static string AffinityStockSales = "affinitystocksales";
		public static string AssociatedProductsCredit = "associatedproductscredit";
		public static string AssociatedProductsCash = "associatedproductscash";
		public static string WarrantyExpiryPromptDays = "warrantyexpirypromptdays";
		public static string WarrantyExpiryLetter = "warrantyexpiryletter";
		public static string WarrantyExpiryMaxPrompt = "warrantyexpirymaxprompt";
        public static string ActivePrompDays = "activepromptdays";                          //#16017
		public static string IRPeriod1 = "irperiod1";
		public static string IRPeriod2 = "irperiod2";
		public static string IRPeriod3 = "irperiod3";
		public static string DisplayScore = "displayscore";
		public static string PrintCharges = "printcharges";
		public static string PrintRFIndicator = "printrfindicator";
		public static string MonthsAllocated = "monthsallocated";
		public static string AllowDNAddedToLoad = "allowdnaddedtoload";
		public static string OnePickListPerTruck = "onepicklistpertruck";
		public static string OutOfStockAuth = "outofstockauth";
		public static string CheckRegion = "checkregion";
		public static string WarehouseTime = "warehousetime";
		public static string StockLockingQty = "stocklockingqty";
		public static string WarrantyValidity = "warrantyvalidity";
		public static string WarrantyCancelDays = "warrantycanceldays";
		public static string DisplayAssembly = "displayassembly";
		public static string ReqDelDateFilter = "reqdeldatefilter";
		public static string PrintPicklist = "printpicklist";
		public static string DisplayDelNoteBranch = "displaydelnotebranch";
		public static string CreditWarrantyDays = "creditwarrantydays";
		public static string doFinancialInterface = "dofinancialinterface";
		public static string fact2000driveandddirectory = "FACT2000driveandDirectory";
		public static string FACT2000ProgramDirectory = "FACT2000ProgramDirectory";
		public static string SystemDrive = "systemdrive";
		public static string lettersDailyorWeekly = "lettersdailyorweekly";
		public static string ValuePerVoucher = "valuepervoucher";
		public static string PreviousRepair = "PreviousRepair";                   //IP - 25/01/11 - Sprint 5.9 - #2252
		public static string PrizeVouchersActive = "prizevouchersactive";
		//public static string EmployChgManApprov = "EmployChgManApprove";        //CR907 jec 01/08/07
		//public static string ExistAccountLength = "ExistAccountLength";         //CR907 jec 01/08/07
		//public static string HighSettStat2Yr = "HighSettStat2Yr";               //CR907 jec 01/08/07
		//public static string MaxAgrmtTotal = "MaxAgrmtTotal";                   //CR907 jec 01/08/07
		//public static string MaxArrearsLevel = "MaxArrearsLevel";               //CR907 jec 01/08/07
		//public static string MinCredScore = "MinCredScore";                     //CR907 jec 01/08/07
		//public static string ResidenceChgManApprove = "ResidenceChgManApprove"; //CR907 jec 01/08/07
		//public static string SettledCashMonths = "SettledCashMonths";           //CR907 jec 01/08/07
		//public static string SettledCredMonths = "SettledCredMonths";           //CR907 jec 01/08/07
		//public static string MaxLoanAmount = "MaxLoanAmount";                   // CR906 rdb 10/08/07
		//public static string LoanMinCredScore = "LoanMinCredScore";             // CR906 rdb 10/08/07
		//public static string LoanRFpcAvail = "LoanRFpcAvail";                   // CR906 rdb 10/08/07
		public static string OracleBExportLocn = "OracleBExportLocn";                   // CR1036 AA 14/12/09
		public static string ReturnedChequePeriod = "returnedchequeperiod";              //CR 543 Peter Chong [25-Sep-2006]
		public static string ReturnedChequeNumberAllowed = "numberallowedreturncheques"; //CR 543 Peter Chong [25-Sep-2006]
		public static string RetainCommMonths = "retainCommMonths";              //CR 36 jec 05/10/06
		public static string SpiffReselection = "SPIFFreselection";              //CR 36 jec 05/10/06
		public static string SpiffPerBranch = "SPIFFperBranch";              //CR1035 jec 20/07/09
		public static string SpiffSugestion = "SPIFFsugestion";              //CR 36 jec 05/10/06
		public static string MaxCommRate = "maxcommrate";              //CR 36 jec 05/10/06
		public static string MaxSpiffValue = "maxspiffvalue";              //CR 36 jec 05/10/06
		public static string ComPerAccType = "ComPerAccType";              //CR 36 jec 22/06/07
		public static string ServiceReplacement = "ServiceReplacement";
		public static string ServiceLocation = "ServiceLocation";
		public static string ServiceBER = "ServiceBER";
		public static string ServiceBERCostPricePCent = "ServiceBERCostPricePCent"; //IP - 24/01/11 - Sprint 5.9 - #2232
		public static string ServiceDeposit = "ServiceDeposit";
		public static string ServiceLabourRate = "ServiceLabourRate";
		public static string ServiceMatrix = "ServiceMatrix";
		public static string ServiceLabourMarkUp = "ServiceLabourMarkUp";
		public static string ServicePartsMarkUp = "ServicePartsMarkUp";
		public static string ServiceInternal = "ServiceInternal";
		public static string ServiceWarranty = "ServiceWarranty";
		public static string ServiceStockAccount = "ServiceStockAccount";
		public static string ServiceItemPartsCourts = "ServiceItemPartsCourts";
		public static string ServiceItemPartsOther = "ServiceItemPartsOther";
		public static string ServiceItemLabour = "ServiceItemLabour";
		public static string ServiceRepossession = "ServiceRepossession";
		public static string ServiceResolutionFault = "ServiceResolutionFault"; //UAT 453
		public static string ServiceCashGo = "ServiceCash&Go"; //CR949/958
		public static string ServiceActionRequired = "ServiceActionRequired"; // CR 949/958
		public static string SRAccountName = "SRAcctName"; //CR1030
		public static string SpaMaxLowerInstals = "SpaMaxLowerInstals"; // CR 976
		public static string IRCashRF = "IRCashRF";
		public static string WarrantySESPrompts = "WarrantySESPrompts"; //CR478
		public static string WarrantyDays = "warrantydays"; //CR478
		public static string EnablePhotoPrinting = "EnablePhotoPrinting";//CR855
		public static string StoreCustomerSignature = "StoreCustomerSignature";//CR855
		public static string PhotoDirectory = "PhotoDirectory";//CR855
		public static string SignatureDirectory = "SignatureDirectory";//CR855
		public static string RescoreMonths = "rescoremonths";//CR868
		public static string RescoreStatus = "rescorestatus";//CR868
		public static string EODRescoreMonths = "eodrescoremonths";//CR868
		public static string CancelDelNoteIfFailed = "CancelDelNoteIfFailed";   //UAT351 IP 01/11/07
		public static string SLRebateCalculationRuleDate = "SLRebateCalculationRuleDate";           //CR938 jec 03/04/08
		public static string FinancialURL = "financialurl"; //IP - CR946
		public static string MaxReminderDaysInAdv = "MaxReminderDaysInAdv"; //NM & IP - 05/01/09 - CR976
		public static string IncInsinServAgrPrint = "IncInsinServAgrPrint"; //AA - CR1005
		public static string DeleteDNFromLoad = "DeleteDNFromLoad"; //IP - 05/01/09 - CR929 & 974
		public static string MQEnabled = "MQEnabled"; //IP - 05/01/09 - CR929 & 974
		public static string SRAwaitCustPayment = "SRAwaitCustPayment"; //CR 1024 (NM 23/04/2009)
		public static string SRBatchPrintOutFormat = "SRBatchPrintOutFormat"; //CR 1024 (NM 23/04/2009)
		public static string SRClsdTabBeforeResTab = "SRClsdTabBeforeResTab"; //CR 1024 (NM 23/04/2009)
		public static string SRBatchPrintCopies = "SRBatchPrintCopies"; //CR 1024 (NM 23/04/2009)
		public static string SRRepairEstimate = "SRRepairEstimate"; //CR 1024 (NM 23/04/2009)
		public static string SRSupAllocation = "SRSupAllocation"; //CR 1024 (NM 23/04/2009)        
		public static string IdentRepEqualsDNStockBranch = "IdentRepEqualsDNStockBranch";
		public static string CourtsDealerName = "CourtsDealerName";     //jec 05/06/09
		public static string NonCourtsDealerName = "NonCourtsDealerName";     //jec 05/06/09
		public static string SRPrintShowWrntyAvailable = "SRPrintShowWrntyAvailable"; //NM - UAT 693    
		public static string BlockCashGoInRefundCorrection = "BlockCashGoInRefCorr"; //IP - 08/02/10 - CR1037 - Malaysia Enhancements (CR1072)
		public static string blockcreditmonths = "blockcreditmonths";  
		public static string IntalmentRounding = "instalmentrounding"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string ServiceChargeRounding = "roundServiceCharge"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyScheme = "LoyaltyScheme"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyMembershipFee = "LoyaltyMembershipFee"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyCashThreshold = "LoyaltyCashThreshold"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyMembershipPeriod = "LoyaltyMembershipPeriod"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyVoucherPeriod = "LoyaltyVoucherPeriod"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyRollOverPeriod = "LoyaltyRollOverPeriod"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyIncludeWarranty = "LoyaltyIncludeWarranty"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string LoyaltyMaxJoinRejects = "LoyaltyMaxJoinRejects"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		// public static string LoyaltyReviseScreen = "LoyaltyReviseScreen"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string EnableGRTnotes = "EnableGRTnotes"; //IP - 08/02/10 - Malaysia Enhancements (CR1072 merged from 4.3)
		public static string ManualDAFOCAccts = "ManualDAFOCAccts"; //IP/JC - 16/02/10 - CR1048 4.10 (REF:3.1.36) - FOC - CR1072 
		public static string BehaviouralScorecard = "BehaviouralScorecard"; //SC - 16/02/10 - CR1034 //IP - 09/04/10 - CR1034 - Removed put back AA
		public static string BehaveApplyEodImmediate = "BehaveApplyEodImmediate";
		public static string TaxInvKitDisc = "TaxInvKitDisc"; //IP - 19/02/10 - CR1072 - LW 69807 - Printing Fixes from 4.3 - Merge
		public static string ThirdPartyDeliveriesWarehouse = "3PLWarehouse"; //IP - 23/02/10 - CR1072 - Malaysia 3PL for Version 5.2
		public static string ScheduledDeliveries = "ScheduleDel"; //jec - 01/03/10 - CR1072 - Malaysia 3PL for Version 5.2
		public static string FinTotalsIncludedinDeliveriesExport = "FinTotinDelExport"; //Malaysia want this...
		public static string MaxExceedCRLimit = "MaxExceedCRLimit";     //jec 10/08/10 CR1113
		public static string MinCRLimitRef = "MinCRLimitRef";     //jec 10/08/10 CR1113
		public static string IC_EmployChgManApprove = "IC_EmployChgManApprove";
		public static string IC_ExistAccountLength = "IC_ExistAccountLength";
		public static string IC_HighSettStat2Yr = "IC_HighSettStat2Yr";
		public static string IC_HighStatusTimeFrame = "IC_HighStatusTimeFrame";
		public static string IC_MaxAgrmtTotal = "IC_MaxAgrmtTotal";
		public static string IC_MaxArrearsLevel = "IC_MaxArrearsLevel";
		public static string IC_MinCredScore = "IC_MinCredScore";
		public static string IC_ResidenceChgManApprove = "IC_ResidenceChgManApprove";
		public static string IC_SettledCredMonths = "IC_SettledCredMonths";
		public static string IC_ReferralMonths = "IC_ReferralMonths";
		public static string IC_AddressCheckScore = "IC_AddressCheckScore";
		public static string IC_EmploymentCheckScore = "IC_EmploymentCheckScore";
		public static string IC_ReviseMonths = "IC_ReviseMonths";
		public static string CL_EmployChgManApprove = "CL_EmployChgManApprove";
		public static string CL_ExistAccountLength = "CL_ExistAccountLength";
		public static string CL_HighSettStat2Yr = "CL_HighSettStat2Yr";
		public static string CL_HighStatusTimeFrame = "CL_HighStatusTimeFrame";
		public static string CL_LoanMinCredScore = "CL_LoanMinCredScore";
		public static string CL_LoanRFpcAvail = "CL_LoanRFpcAvail";
		public static string CL_MaxArrearsLevel = "CL_MaxArrearsLevel";
		public static string CL_ResidenceChgManApprove = "CL_ResidenceChgManApprove";
		public static string CL_SettledCredMonths = "CL_SettledCredMonths";
		public static string CL_MaxLoanAmount = "CL_MaxLoanAmount";
        public static string CL_NewCustMaxLoanAmount = "CL_NewCustMaxLoanAmount";
        public static string CL_RecentCustMaxLoanAmount = "CL_RecentCustMaxLoanAmount";
        public static string CL_StaffCustMaxLoanAmount = "CL_StaffCustMaxLoanAmount";
        public static string CL_EnablePurposeDropDown = "CL_EnablePurposeDropDown";         //#19337 - CR18568
        public static string CL_EarlySettPenaltyPeriod = "CL_EarlySettPenaltyPeriod";       //#19425 - CR18938
        public static string CL_NewOutstandingCalculation = "CL_NewOutstandingCalculation";       //#CLA - RD - Flag to Enable or disable New Outstanding Calculation for CLA
        public static string showFactEmpNo = "showFactEmpNo";
		public static string TaxReprintHeader = "TaxReprintHeader";
		public static string ZoneAddresses = "ZoneAddresses";     //jec 20/08/10 CR1084
		public static string AssignSuccessBailiff = "AssignSuccessBailiff";     //jec 20/08/10 CR1084
		public static string SuccessBailiffMonths = "SuccessBailiffMonths";     //jec 20/08/10 CR1084
		public static string PopUpAdditInfo = "PopUpAdditInfo";     //jec 20/08/10 CR1084
		public static string PopUpCustomerLeftAddr = "PopUpCustomerLeftAddr";     //jec 20/08/10 CR1084
		public static string PopUpAcctsInArrears = "PopUpAcctsInArrears";     //jec 20/08/10 CR1084
		public static string PopUpPhoneOutOfService = "PopUpPhoneOutOfService";     //jec 20/08/10 CR1084
		public static string EposLicense = "EposLicense";
		public static string genFeeStandingOrder = "genFeeStandingOrder";           //IP - 16/09/10 - CR1092 - COASTER to CoSACS Enhancements
		public static string TaxInvoiceItemsPerPage = "TaxInvoiceItemsPerPage";
		public static string StoreCardEnabled = "StoreCardEnabled";
		public static string StoreCardPercent = "StorecardPercent";
		public static string StoreCardPaymentPercent = "StoreCardPaymentPercent";
		public static string StoreCardMagStripeReaderName = "StoreCardMagStripeReaderName";
		public static string SCLatePaymentFees = "SCLatePaymentFees";
		public static string STMinBalanceforFee = "STMinBalanceforFee";
		public static string STAnnualFee = "SCAnnualFee";
		public static string StoreCardMinPayment = "StoreCardMinPayment";
		public static string STStatementFee = "SCStatementFee";
		public static string STReplacementFee = "SCReplacementFee";
		public static string StoreCardCheckQual = "StoreCardCheckQual";
		public static string ItemPriceFromLocation = "pricefromlocn";
		public static string MinStoreCardLimit = "MinStoreCardLimit";
		public static string MaxItemValStoreCard = "MaxItemValStoreCard";           //IP - 02/12/10 - Store Card
		public static string StoreCardActivate = "StoreCardActivate";
		public static string StoreCardPasswordforManualEntry = "SCManualCardOverride";
		public static string StoreCardMaxNoJointCards = "StoreCardMaxNoJointCards";
		public static string StoreCardDefaultCardMonths = "StoreCardDefaultCardMonths";
		public static string ValuePerVoucherSCardInterestFreeDays = "SCardInterestFreeDays";
		public static string InstalChgAcct = "InstalChgAcct";
		public static string ReferExistingCustomersWithoutHomeandWorkPhonesButwithMobiles = "MobPhoneRefer";
		public static string ReferNewCustomersWithoutHomeandWorkPhonesButwithMobiles = "MobPhoneReferNew";
		public static string StatExistsRefer = "StatExistsRefer";
		public static string WorstStatusPeriod = "WorstStatusPeriod";
		public static string MinExpenseReferforExistingCustomer = "MinExpenseRefer";
		public static string InstalWaiverMinScore = "InstalWaiverMinScore";
		public static string MaxSpendLimitRefer = "MaxSpendLimitRefer";
		public static string ReasonsReferPopup = "ReasonsReferPopup";  //IP - 23/03/11 - CR1245 
		public const string NoOfDaysSinceAction = "NoOfDaysSinceAction";
		public const string StoreCardIssueCardPreAppr = "StoreCardIssueCardPreAppr"; //IP - 05/05/11
		public const string StoreCardStatementFrequency = "StoreCardStatementFrequency"; //IP - 05/05/11
		public const string SCardInterestFreeDays = "SCardInterestFreeDays"; //IP - 05/05/11
		public static string RIABCpath = "RIABCpath";   //RI jec 11/04/11
		public static string RIABCpathRepo = "RIABCpathRepo";   //RI jec 11/04/11
		public static string RIDTFpath = "RIDTFpath";   //RI jec 11/04/11
		public static string RIDTFpathRepo = "RIDTFpathRepo";   //RI jec 11/04/11
		public static string RIKITpath = "RIKITpath";   //RI jec 11/04/11
		public static string RIKITpathRepo = "RIKITpathRepo";   //RI ip 26/08/11
		public static string RIOHQYpath = "RIOHQYpath";   //RI jec 11/04/11
		public static string RIOHQYpathRepo = "RIOHQYpathRepo";   //RI jec 11/04/11
		public static string RIOutboundDirectory = "RIOutboundDirectory";   //RI jec 11/04/11
		public static string RIPODYpath = "RIPODYpath";   //RI jec 11/04/11
		public static string RIQTYpath = "RIQTYpath";   //RI jec 11/04/11
		public static string RIQTYpathRepo = "RIQTYpathRepo";  
		public static string RIRPOpath = "RIRPOpath";   //RI jec 11/04/11
		public static string RISARpath = "RISARpath";   //RI jec 11/04/11
		public static string RISARpathRepo = "RISARpathRepo";   //RI jec 11/04/11
		public static string RICTXpath = "RICTXpath";   //RI jec 15/06/11
		public static string RICTXpathRepo = "RICTXpathRepo";   //RI jec 15/06/11
		public static string RIDTFMSGQArgument = "RIDTFMSGQArgument";
		public static string RIDTFRepoMSGQArgument = "RIDTFRepoMSGQArgument";        
		public static string RIQTYMSGQArgument = "RIQTYMSGQArgument";
		public static string RIQTYRepoMSGQArgument = "RIQTYRepoMSGQArgument";  
		public static string RISARMSGQArgument = "RISARMSGQArgument";      
		public static string RISARRepoMSGQArgument = "RISARRepoMSGQArgument";
		public static string RIRPOMSGQArgument = "RIRPOMSGQArgument";        

		public static string RIFTEBatchScriptPath = "RIFTEBatchScriptPath";
		public static string DisplayAssemblyOptions = "DisplayAssemblyOptions";             //IP - 18/06/11 - CR1212 - RI - #3960
		public static string RIInterfaceOptions = "RIInterfaceOptions";                     //IP - 22/06/11 - CR1212 - RI - #3987
		public static string RIDispCourtsCode = "RIDispCourtsCode";                         //IP - 22/06/11 - CR1212 - RI - #3987
		public static string StockOnOrderAuth = "StockOnOrderAuth";                         //IP - 06/07/11 - RI - #3974
		public static string ComRepoItem = "ComRepoItem";                         // RI jec - 07/07/11
		public static string RIDispCatAsDept = "RIDispCatAsDept";                           //IP - 25/07/11 - CR1254 - RI - #4036
		public static string CL_MaxPctRFavail = "CL_MaxPctRFavail";   //CR1232 jec 16/09/11
		public static string CL_MinLoanAmount = "CL_MinLoanAmount";   //CR1232 jec 16/09/11
		public static string CL_AccountType = "CL_AccountType";         //CR1232 jec 19/09/11
		public static string CL_AddressMonths = "CL_AddressMonths";         //CR1232 jec 20/09/11
		public static string MaxShortageDaily = "MaxShortageDaily";         //CR1234 jec 07/12/11
		public static string MaxShortageWeekly = "MaxShortageWeekly";         //CR1234 jec 07/12/11
		public static string MaxShortageMonthly = "MaxShortageMonthly";         //CR1234 jec 07/12/11
		public static string MaxShortageYearly = "MaxShortageYearly";         //CR1234 jec 07/12/11
		public static string CashierTotalsAccount = "CashierTotalsAccount";         //CR1234  
		public static string CashierTotalsWOFrequency = "CashierTotalsWOFrequency";         //CR1234  
		public static string TaxInvPrintAfterPayment = "TaxInvPrintAfterPayment";         //IP - 18/06/12 - #9448 - CR1239
        public static string CL_Amortized = "CL_Amortized";   //For amortized cash loan country level parameter
        public static string CL_TaxRateApplied = "CL_TaxRateApplied"; //For tax rate application in amortized cash loan country level parameter
		public static string IsOldScoreRunWithEquifax = "IsOldScoreRunWithEquifax";//for run both old and new Equifax score country level parameter



        public static string PasswordExpireDays = "PasswordExpireDays";
		public static string MinRequiredPasswordLength = "MinRequiredPasswordLength";
		public static string MinRequiredNonalphanumericCharacters = "MinRequiredNonalphanumericCharacters";    
        public static string RepoDelUnitPrice = "RepoDelUnitPrice";         //jec 18/07/12 - #10406
        public static string StoreCardMaxDaysEODRun = "StoreCardMaxDaysEODRun"; //#12341 - CR11571
        public static string DisplayExpressDelivery = "DisplayExpressDelivery"; //#12232 - CR12249
	    public static string InstallationStockAccount = "InstallationStockAccount"; //#12116
        public static string OnlineDistCentre = "OnlineDistCentre"; // #13889
        public static string OnlineSBandIntRate = "OnlineSBandIntRate"; // #13889
        public static string OnlineTermsType = "OnlineTermsType"; // #13889
        public static string OnlineTermsLength = "OnlineTermsLength"; // #13889
        public static string ISOCountryCode = "ISOCountryCode"; // #13890
        public static string MinFreeMonthIR = "MinFreeMonthIR"; // #17287
        public static string DelayNewIRW = "DelayNewIRW";       // #17287
        public static string CreditMinPrice = "CreditMinPrice"; // #17287
		public static string EnableMMI = "EnableMMI";
        public static string AbilitySetFirstPaymentDate = "AbilitySetFirstPaymentDate";
        public static string ReviseCashLoanDisbursLimits = "ReviseCashLoanDisbursLimits"; // Cash Limit
		public static string ApplyNewDIChanges = "ApplyNewDispIncomeChanges";
        public static string RentFactor = "SpouseRentFactor";
		
        public static class Printing
		{
			public static string ReceiptMinimumHeight = "ReceiptMinimumHeight";
			public static string PrintAutomaticMiniStatement = "PrintAutomaticMiniStatement";
			public static string TransactionsForAutomaticMiniStatement = "TransactsForAutoMiniStatement";
			public static string PrintAvailableSpendOnMiniStatement = "PrintAvailSpendOnMiniStatement";
			public static string DefaultToThermalPrinter = "DefaultToThermalPrinter";
			public static string BusinessTitle = "BusinessTitle";
			public static string BusinessRegNo = "BusinessRegNo";
			public static string CashNGoFooter = "CashNGoFooter";
			public static string TaxInvoiceFooter = "TaxInvoiceFooter";
			public static string StatementFooter = "StatementFooter";
			public static string ReceiptDateFormat = "ReceiptDateFormat";
			public static string PaymentFooter = "PaymentFooter";
			public static string CashNGoReceiptTitle = "CashNGoReceiptTitle";
			public static string PaymentReceiptTitle = "PaymentReceiptTitle";
			public static string TaxInvoiceTitle = "TaxInvoiceTitle";
			public static string DisplayCashNGoReceiptTitle = "DisplayCashNGoReceiptTitle";
			public static string DisplayPaymentReceiptTitle = "DisplayPaymentReceiptTitle";
			public static string DisplayTaxInvoiceTitle = "DisplayTaxInvoiceTitle";
			public static string TaxNumber = "TaxNumber";
			public static string PaymentReceiptDisplayVATNumber = "PaymentReceiptDisplayVATNumber";
			public static string CashNGoDisplayVATNumber = "CashNGoDisplayVATNumber";
			public static string TaxInvoiceDisplayVATNumber = "TaxInvoiceDisplayVATNumber";
			public static string StatementDisplayVATNumber = "StatementDisplayVATNumber";
			public static string StatementDisplayAvailableSpend = "StatementDisplayAvailableSpend";
			public static string StatementTitle = "StatementTitle";
			public static string CashNGoNoOfCopies = "CashNGoNoOfCopies";
			public static string CashNGoDisplaySignature = "CashNGoDisplaySignature";
			public static string CashNGoSignatureText = "CashNGoSignatureText";
			public static string CashNGoOrigianlText = "CashNGoOrigianlText";
			public static string CashNGoCopyText = "CashNGoCopyText";
			public static string PaymentReceiptDisplayFooter = "PaymentReceiptDisplayFooter";
			public static string CashNGoDisplayFooter = "CashNGoDisplayFooter";
			public static string TaxInvoiceDisplayFooter = "TaxInvoiceDisplayFooter";
			public static string DisplayTaxNumberLabel = "DisplayTaxNumberLabel";
			public static string DisplayBusinessRegistrationLabel = "DisplayBusinessRegLabel";
			public static string BusinessRegistrationLabel = "BusinessRegistrationLabel";
			public static string DisplayBusinessRegistrationNumber = "DisplayBusinessRegNo";
			public static string DisplayBusinessTaxNumber = "DisplayBusinessTaxNumber";
			public static string TaxNumberLabel = "TaxNumberLabel";
			public static string StatementPrintType = "StatementPrintType";
			public static string SCardReceiptDisplayFooter = "SCardReceiptDisplayFooter";               //IP - 11/01/11 - Store Card
			public static string SCardReceiptDisplayTitle = "SCardReceiptDisplayTitle";                 //IP - 11/01/11 - Store Card
			public static string SCardReceiptTitle = "SCardReceiptTitle";                               //IP - 11/01/11 - Store Card
			public static string SCardReceiptFooter = "SCardReceiptFooter";                             //IP - 11/01/11 - Store Card
			public static string SCardReceiptDisplaySignature = "SCardReceiptDisplaySignature";         //IP - 11/01/11 - Store Card
			public static string SCardReceiptSignatureText = "SCardReceiptSignatureText";               //IP - 11/01/11 - Store Card
			public static string SCardReceiptCopies = "SCardReceiptCopies";                             //IP - 13/01/11 - Store Card

		}
	}

	public class CountryParamEventArgs : EventArgs
	{
		public string CountryParamName { get; set; }

		public CountryParamEventArgs(string CountryParamName)
		{
			this.CountryParamName = CountryParamName;
		}
	}
}
