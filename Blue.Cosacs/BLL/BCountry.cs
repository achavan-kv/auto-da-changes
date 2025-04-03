using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.PrivilegeClub;
using Blue.Cosacs.Repositories;

namespace STL.BLL
{
	/// <summary>
	/// Retrieves country level parameters at startup.
	/// </summary>
	public class BCountry : CommonObject
	{
		/*
		private DCountry data;
		public parms Parms;
		private string _code = "";
		
		public string CountryCode
		{
			get{return _code;}
		}
		
		
		public DataSet GetDefaults(SqlConnection conn, SqlTransaction trans, string countryCode)
		{
			Function = "BCountry::GetDefaults";
			DataSet ds = new DataSet();	
			data = new DCountry();
			data.GetDefaults(conn, trans, countryCode);
			ds.Tables.Add(data.Table);
			_code = countryCode;
			
			return ds;
		}

		public DataSet GetDefaults(string countryCode)
		{
			Function = "BCountry::GetDefaults";
			DataSet ds = new DataSet();	
			data = new DCountry();
			data.GetDefaults(countryCode);
			ds.Tables.Add(data.Table);	
			_code = countryCode;
			
			return ds;
		}
		*/

		public BCountry()
		{
		}

		/*

		/// <summary>
		/// Alternative constructor which will authomatically
		/// populate itself with country parameters
		/// </summary>
		/// <param name="countryCode"></param>
		public BCountry(string countryCode)
		{
			_code = countryCode;
			Parms = new parms();
			data = new DCountry();
			data.GetDefaults(countryCode);
			DataRow row = data.Table.Rows[0];

			if(!Convert.IsDBNull(row["origbr"]))
				Parms.OrigBr = (short)row["origbr"];

			Parms.CountryCode = (string)row["countrycode"];
			Parms.CountryName = (string)row["countryname"];
			Parms.HOBranchNo = (short)row["hobranchno"];
			Parms.ChequeDays = (int)row["cheqdays"];
			Parms.LetterDays = (int)row["letterdays"];
			Parms.LetterPorw = (string)row["letterporw"];
			Parms.TaxType = (string)row["taxtype"];

			if(!Convert.IsDBNull(row["taxrate"]))
				Parms.TaxRate = (double)row["taxrate"];

			Parms.TaxName = (string)row["taxname"];
			Parms.ServPcent = (double)row["servpcent"];
			Parms.DateRun = (DateTime)row["daterun"];
			Parms.DateWeek1 = (DateTime)row["dateweek1"];
			Parms.WeekNo = (int)row["weekno"];
			Parms.BatchControlNo = (int)row["batchctrlno"];
			Parms.BailFee = (double)row["bailfee"];
			Parms.BailPcent = (double)row["bailpcent"];
			Parms.ReceiptsPerBook = (int)row["rcptsperbook"];
			Parms.WeekLst400 = (int)row["weeklst400"];
			Parms.TaxInvType = (short)row["taxinvtype"];
			Parms.AgreementTaxType = (string)row["agrmttaxtype"];

			if(!Convert.IsDBNull(row["datelastscor"]))
				Parms.DateLastScore = (DateTime)row["datelastscor"];
			
			if(!Convert.IsDBNull(row["minhpage"]))
				Parms.MinHPage = (short)row["minhpage"];

			if(!Convert.IsDBNull(row["maxhpage"]))
				Parms.MaxHPage = (short)row["maxhpage"];

			Parms.DateLastCalc = (DateTime)row["datelastcalc"];
			Parms.DailyExport = (int)row["dailyexport"];
			Parms.GlobalDeliveryPcent = (double)row["globdelpcent"];
			Parms.OneMonthDelivery = (short)row["onemonthdel"];
			Parms.DeliveryDays = (short)row["deldays"];
			Parms.ArrearsOnPay = (string)row["arrearsonpay"];
			Parms.SystemOpen = (string)row["systemopen"];
			Parms.Timing = (short)row["timing"];
			Parms.AddToMin = (decimal)row["addtomin"];
			Parms.AddToTerm = (short)row["addtoterm"];
			Parms.AdminFee = (decimal)row["adminfee"];
			Parms.AutoDownSC5 = (string)row["autodownsc5"];
			Parms.CashInterestRate = (double)row["cashintrate"];
			Parms.DateChargesStart = (DateTime)row["datechargesstart"];
			Parms.LastChargesWeekNo = (short)row["lastchargesweekno"];
			Parms.LastYearLastWeekNo = (short)row["lastyrlastweekno"];
			Parms.LettersGap = (short)row["lettersgap"];
			Parms.PercentAddTo1 = (double)row["percentaddto1"];
			Parms.PercentAddTo2 = (double)row["percentaddto2"];
			Parms.PercentAddTo3 = (double)row["percentaddto3"];
			Parms.SmallBalance = (decimal)row["smallbalance"];
			Parms.DutyFree = (string)row["dutyfree"];
			Parms.LoyaltyCard = (string)row["loyaltycard"];
			Parms.LockTimeout = (int)row["locktimeout"];
			Parms.RebatePcent = (double)row["rebpcent"];
			Parms.DeliverySlots = (short)row["deliveryslots"];
			Parms.DefaultDeliveryNoteBranch = (short)row["defdelnotebranch"];
			Parms.MinPeriod = (short)row["minperiod"];
			Parms.DDLeadTime = (short)row["ddleadtime"];
			Parms.DDFee = (decimal)row["ddfee"];
			Parms.DDEnabled = (short)row["ddenabled"];
			Parms.DDBankAccountNo = (string)row["ddbankacctno"];

			if(!Convert.IsDBNull(row["highstatus"]))
				Parms.HighStatus = (string)row["highstatus"];

			Parms.PaymentMethod = (string)row["paymentmethod"];
			Parms.AgrgPrint = (short)row["agrgprint"];
			Parms.AgrTimePrint = (short)row["agrtimeprint"];
			Parms.Print90 = (short)row["print90"];
			Parms.ServicePrint = (string)row["serviceprint"];
			Parms.NoCents = (short)row["nocents"];
			Parms.DDGenNo = (int)row["ddgenno"];
			Parms.DDServiceType = (string)row["ddservicetype"];
			Parms.DDIDCodeFrom = (string)row["ddidcodefrom"];
			Parms.DDIDCodeTo = (string)row["ddidcodeto"];
			Parms.DDBankBranch = (string)row["ddbankbranch"];
			Parms.DDBankAccountName = (string)row["ddbankacctname"];
			Parms.DDBankAccountType = (string)row["ddbankaccttype"];
			Parms.DDTest = (byte)row["ddtest"];
			Parms.VarStamp = (short)row["varstamp"];
			Parms.NoTaxCopies = (byte)row["notaxcopies"];
			Parms.NoAgrCopies = (byte)row["noagrcopies"];
			Parms.AgrPrintType = (string)row["agrprinttype"];
			Parms.DDMaxRejections = (short)row["ddmaxrejections"];
			Parms.DDFeeOverride = (byte)row["ddfeeoverride"];
			Parms.FixedDateFirst = (byte)row["fixeddatefirst"];
			Parms.TransactEnabled = (byte)row["transactenabled"];
			Parms.TransactURL = (string)row["transacturl"];
			Parms.AllowZeroStock = (string)row["allowzerostock"];
			Parms.CODDefault = (string)row["CODDefault"];
			Parms.DecimalPlaces = (string)row[CN.DecimalPlaces];
			Parms.SanctionMinYears = (short)row[CN.SanctionMinYears];
			Parms.AutomaticWarrantyNo = (byte)row[CN.MandWarrantyNo];
			Parms.PayWholeUnits = (string)row[CN.PayWholeUnits];
			Parms.BailiffCommissionEqualsFee = Convert.ToBoolean(row[CN.BailiffCommissionEqualsFee]);
			Parms.ManualRefer = Convert.ToBoolean(row[CN.ManualRefer]);
			Parms.LaserPrintTax = Convert.ToBoolean(row[CN.LaserPrintTax]);
			Parms.CODPercentage = Convert.ToDecimal(row[CN.CODPercentage]);
			Parms.NonInterestItem = (string)row[CN.NonInterestItem];
			Parms.PrintCreditNote = Convert.ToBoolean(row[CN.PrintCreditNote]);
			Parms.WarrantyCreditCopy = (short)row[CN.WarrantyCreditCopy];
			Parms.WarrantyCustCopy = (short)row[CN.WarrantyCustCopy];
			Parms.WarrantyHOCopy = (short)row[CN.WarrantyHOCopy];
			Parms.PercentToPay = (double)row[CN.PercentToPay];
			Parms.Securitisation = Convert.ToBoolean(row[CN.Securitisation]);
			Parms.CreditScanInterval = (short)row[CN.CreditScanInterval];
			Parms.SecureRefunds = (short)row[CN.SecureRefunds];
			Parms.WarrantyStylesheet = (string)row[CN.WarrantyStylesheet];
			Parms.CreditWarrantyDays = (short)row[CN.CreditWarrantyDays];
			Parms.PrintScheduleOfPayments = Convert.ToBoolean(row[CN.PrintScheduleOfPayments]);
			Parms.CancellationRejectionCode = (string)row[CN.CancellationRejectionCode];
			Parms.PrintToolBar = Convert.ToBoolean(row[CN.PrintToolBar]);
			Parms.GiftVoucherAccount = (string)row[CN.GiftVoucherAccount];
			Parms.DefaultVoucherExpiry = (short)row[CN.DefaultVoucherExpiry];
		}

		public BCountry(SqlConnection conn, SqlTransaction trans, string countryCode)
		{
			_code = countryCode;
			Parms = new parms();
			data = new DCountry();
			data.GetDefaults(conn, trans, countryCode);
			DataRow row = data.Table.Rows[0];

			if(!Convert.IsDBNull(row["origbr"]))
				Parms.OrigBr = (short)row["origbr"];

			Parms.CountryCode = (string)row["countrycode"];
			Parms.CountryName = (string)row["countryname"];
			Parms.HOBranchNo = (short)row["hobranchno"];
			Parms.ChequeDays = (int)row["cheqdays"];
			Parms.LetterDays = (int)row["letterdays"];
			Parms.LetterPorw = (string)row["letterporw"];
			Parms.TaxType = (string)row["taxtype"];

			if(!Convert.IsDBNull(row["taxrate"]))
				Parms.TaxRate = (double)row["taxrate"];

			Parms.TaxName = (string)row["taxname"];
			Parms.ServPcent = (double)row["servpcent"];
			Parms.DateRun = (DateTime)row["daterun"];
			Parms.DateWeek1 = (DateTime)row["dateweek1"];
			Parms.WeekNo = (int)row["weekno"];
			Parms.BatchControlNo = (int)row["batchctrlno"];
			Parms.BailFee = (double)row["bailfee"];
			Parms.BailPcent = (double)row["bailpcent"];
			Parms.ReceiptsPerBook = (int)row["rcptsperbook"];
			Parms.WeekLst400 = (int)row["weeklst400"];
			Parms.TaxInvType = (short)row["taxinvtype"];
			Parms.AgreementTaxType = (string)row["agrmttaxtype"];

			if(!Convert.IsDBNull(row["datelastscor"]))
				Parms.DateLastScore = (DateTime)row["datelastscor"];
			
			if(!Convert.IsDBNull(row["minhpage"]))
				Parms.MinHPage = (short)row["minhpage"];

			if(!Convert.IsDBNull(row["maxhpage"]))
				Parms.MaxHPage = (short)row["maxhpage"];

			Parms.DateLastCalc = (DateTime)row["datelastcalc"];
			Parms.DailyExport = (int)row["dailyexport"];
			Parms.GlobalDeliveryPcent = (double)row["globdelpcent"];
			Parms.OneMonthDelivery = (short)row["onemonthdel"];
			Parms.DeliveryDays = (short)row["deldays"];
			Parms.ArrearsOnPay = (string)row["arrearsonpay"];
			Parms.SystemOpen = (string)row["systemopen"];
			Parms.Timing = (short)row["timing"];
			Parms.AddToMin = (decimal)row["addtomin"];
			Parms.AddToTerm = (short)row["addtoterm"];
			Parms.AdminFee = (decimal)row["adminfee"];
			Parms.AutoDownSC5 = (string)row["autodownsc5"];
			Parms.CashInterestRate = (double)row["cashintrate"];
			Parms.DateChargesStart = (DateTime)row["datechargesstart"];
			Parms.LastChargesWeekNo = (short)row["lastchargesweekno"];
			Parms.LastYearLastWeekNo = (short)row["lastyrlastweekno"];
			Parms.LettersGap = (short)row["lettersgap"];
			Parms.PercentAddTo1 = (double)row["percentaddto1"];
			Parms.PercentAddTo2 = (double)row["percentaddto2"];
			Parms.PercentAddTo3 = (double)row["percentaddto3"];
			Parms.SmallBalance = (decimal)row["smallbalance"];
			Parms.DutyFree = (string)row["dutyfree"];
			Parms.LoyaltyCard = (string)row["loyaltycard"];
			Parms.LockTimeout = (int)row["locktimeout"];
			Parms.RebatePcent = (double)row["rebpcent"];
			Parms.DeliverySlots = (short)row["deliveryslots"];
			Parms.DefaultDeliveryNoteBranch = (short)row["defdelnotebranch"];
			Parms.MinPeriod = (short)row["minperiod"];
			Parms.DDLeadTime = (short)row["ddleadtime"];
			Parms.DDFee = (decimal)row["ddfee"];
			Parms.DDEnabled = (short)row["ddenabled"];
			Parms.DDBankAccountNo = (string)row["ddbankacctno"];

			if(!Convert.IsDBNull(row["highstatus"]))
				Parms.HighStatus = (string)row["highstatus"];

			Parms.PaymentMethod = (string)row["paymentmethod"];
			Parms.AgrgPrint = (short)row["agrgprint"];
			Parms.AgrTimePrint = (short)row["agrtimeprint"];
			Parms.Print90 = (short)row["print90"];
			Parms.ServicePrint = (string)row["serviceprint"];
			Parms.NoCents = (short)row["nocents"];
			Parms.DDGenNo = (int)row["ddgenno"];
			Parms.DDServiceType = (string)row["ddservicetype"];
			Parms.DDIDCodeFrom = (string)row["ddidcodefrom"];
			Parms.DDIDCodeTo = (string)row["ddidcodeto"];
			Parms.DDBankBranch = (string)row["ddbankbranch"];
			Parms.DDBankAccountName = (string)row["ddbankacctname"];
			Parms.DDBankAccountType = (string)row["ddbankaccttype"];
			Parms.DDTest = (byte)row["ddtest"];
			Parms.VarStamp = (short)row["varstamp"];
			Parms.NoTaxCopies = (byte)row["notaxcopies"];
			Parms.NoAgrCopies = (byte)row["noagrcopies"];
			Parms.AgrPrintType = (string)row["agrprinttype"];
			Parms.DDMaxRejections = (short)row["ddmaxrejections"];
			Parms.DDFeeOverride = (byte)row["ddfeeoverride"];
			Parms.FixedDateFirst = (byte)row["fixeddatefirst"];
			Parms.TransactEnabled = (byte)row["transactenabled"];
			Parms.TransactURL = (string)row["transacturl"];
			Parms.AllowZeroStock = (string)row["allowzerostock"];
			Parms.CODDefault = (string)row["CODDefault"];
			Parms.DecimalPlaces = (string)row[CN.DecimalPlaces];
			Parms.SanctionMinYears = (short)row[CN.SanctionMinYears];
			Parms.AutomaticWarrantyNo = (byte)row[CN.MandWarrantyNo];
			Parms.PayWholeUnits = (string)row[CN.PayWholeUnits];
			Parms.BailiffCommissionEqualsFee = Convert.ToBoolean(row[CN.BailiffCommissionEqualsFee]);
			Parms.ManualRefer = Convert.ToBoolean(row[CN.ManualRefer]);
			Parms.LaserPrintTax = Convert.ToBoolean(row[CN.LaserPrintTax]);
			Parms.CODPercentage = Convert.ToDecimal(row[CN.CODPercentage]);
			Parms.NonInterestItem = (string)row[CN.NonInterestItem];
			Parms.PrintCreditNote = Convert.ToBoolean(row[CN.PrintCreditNote]);
			Parms.WarrantyCreditCopy = (short)row[CN.WarrantyCreditCopy];
			Parms.WarrantyCustCopy = (short)row[CN.WarrantyCustCopy];
			Parms.WarrantyHOCopy = (short)row[CN.WarrantyHOCopy];
			Parms.PercentToPay = (double)row[CN.PercentToPay];
			Parms.Securitisation = Convert.ToBoolean(row[CN.Securitisation]);
			Parms.CreditScanInterval = (short)row[CN.CreditScanInterval];
			Parms.SecureRefunds = (short)row[CN.SecureRefunds];
			Parms.WarrantyStylesheet = (string)row[CN.WarrantyStylesheet];
			Parms.CreditWarrantyDays = (short)row[CN.CreditWarrantyDays];
			Parms.PrintScheduleOfPayments = Convert.ToBoolean(row[CN.PrintScheduleOfPayments]);
			Parms.CancellationRejectionCode = (string)row[CN.CancellationRejectionCode];
			Parms.PrintToolBar = Convert.ToBoolean(row[CN.PrintToolBar]);
			Parms.GiftVoucherAccount = (string)row[CN.GiftVoucherAccount];
			Parms.DefaultVoucherExpiry = (short)row[CN.DefaultVoucherExpiry];
		}

		public struct parms
		{
			public short OrigBr;
			public string CountryCode;
			public string CountryName;
			public short HOBranchNo;
			public int ChequeDays;
			public int LetterDays;
			public string LetterPorw;
			public string TaxType;
			public double TaxRate;
			public string TaxName;
			public double ServPcent;
			public DateTime DateRun;
			public DateTime DateWeek1;
			public int WeekNo;
			public int BatchControlNo;
			public double BailFee;
			public double BailPcent;
			public int ReceiptsPerBook;
			public int WeekLst400;
			public short TaxInvType;
			public string AgreementTaxType;
			public DateTime DateLastScore;
			public short MinHPage;
			public short MaxHPage;
			public DateTime DateLastCalc;
			public int DailyExport;
			public double GlobalDeliveryPcent;
			public short OneMonthDelivery;
			public short DeliveryDays;
			public string ArrearsOnPay;
			public string SystemOpen;
			public short Timing;
			public decimal AddToMin;
			public short AddToTerm;
			public decimal AdminFee;
			public string AutoDownSC5;
			public double CashInterestRate;
			public DateTime DateChargesStart;
			public short LastChargesWeekNo;
			public short LastYearLastWeekNo;
			public short LettersGap;
			public double PercentAddTo1;
			public double PercentAddTo2;
			public double PercentAddTo3;
			public decimal SmallBalance;
			public string DutyFree;
			public string LoyaltyCard;
			public int LockTimeout;
			public double RebatePcent;
			public short DeliverySlots;
			public short DefaultDeliveryNoteBranch;
			public int MinPeriod;
			public short DDLeadTime;
			public decimal DDFee;
			public short DDEnabled;
			public string DDBankAccountNo;
			public string HighStatus;
			public string PaymentMethod;
			public short AgrgPrint;
			public short AgrTimePrint;
			public short Print90;
			public string ServicePrint;
			public short NoCents;
			public int DDGenNo;
			public string DDServiceType;
			public string DDIDCodeFrom;
			public string DDIDCodeTo;
			public string DDBankBranch;
			public string DDBankAccountName;
			public string DDBankAccountType;
			public byte DDTest;
			public short VarStamp;
			public byte NoTaxCopies;
			public byte NoAgrCopies;
			public string AgrPrintType;
			public short DDMaxRejections;
			public byte DDFeeOverride;
			public byte FixedDateFirst;
			public byte TransactEnabled;
			public string TransactURL;
			public string AllowZeroStock;
			public string CODDefault;
			public string DecimalPlaces;
			public short SanctionMinYears;
			public byte AutomaticWarrantyNo;
			public string PayWholeUnits;
			public bool BailiffCommissionEqualsFee;
			public bool ManualRefer;
			public bool LaserPrintTax;
			public decimal CODPercentage;
			public string NonInterestItem;
			public bool PrintCreditNote;
			public short WarrantyCustCopy;
			public short WarrantyCreditCopy;
			public short WarrantyHOCopy;
			public double PercentToPay;
			public bool Securitisation;
			public short CreditScanInterval;
			public short SecureRefunds;
			public string WarrantyStylesheet;
			public short CreditWarrantyDays;
			public bool PrintScheduleOfPayments;
			public string CancellationRejectionCode;
			public bool PrintToolBar;
			public string GiftVoucherAccount;
			public short DefaultVoucherExpiry;
		}
		*/

		/// <summary>
		/// GetMaintenanceParameters
		/// </summary>
		/// <param name="country">string</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet GetMaintenanceParameters (SqlConnection conn, SqlTransaction trans, string country)
		{
			DataSet ds = new DataSet(); 
			DCountry da = new DCountry();
			DataTable dt = da.GetMaintenanceParameters(conn, trans, country);
			
			if(dt.Rows.Count > 0)
			{
				CountryParameterCollection c = new CountryParameterCollection(dt);
				Cache["Country"] = c;
                StockItemCache.Invalidate(new StockRepository().GetStockItemCache());
                
				// Check whether the Privilege Club Tier1/2 is enabled and if so translate to their scheme.
				this.FilterPrivilegeClub(dt);
				ds.Tables.Add(dt);			
			}
			else
				throw new STLException(GetResource("M_INVALIDCOUNTRY", new object[]{country}));

			return ds;
		}

		public void SaveCountryMaintenanceParameters(SqlConnection conn, SqlTransaction trans, 
			string countryCode, DataSet changes, int user)
		{
			DCountry country = new DCountry();
			foreach(DataTable dt in changes.Tables)
			{
				// Check whether the Privilege Club Tier1/2 is enabled and if so validate the parameters
				this.ValidatePrivilegeClub(dt);

				foreach(DataRow r in dt.Rows)
				{
					country.SaveCountryMaintenanceParameter(conn, trans, countryCode, 
						(int)r[CN.ParameterID],
						(string)r[CN.Value], user);
				}
			}
			///This next step will refresh the cached copy of the parameters
            //DataSet ds = GetMaintenanceParameters(conn, trans, countryCode);          // #13465
		}


		private void TranslatePrivilegeClub(out string tier1Translation, out string tier2Translation)
		{
			// Init to default names
			tier1Translation = PCTranslate.Tier1;
			tier2Translation = PCTranslate.Tier2;

			// Get the user translation for their scheme
			DCategory cat = new DCategory();
			cat.GetCustomerCodes();
			foreach (DataRow row in cat.CustCodes.Rows)
				if ((string)row[CN.Code] == PCCustCodes.Tier1)
					tier1Translation = (string)row[CN.Description];
				else if ((string)row[CN.Code] == PCCustCodes.Tier2)
					tier2Translation = (string)row[CN.Description];
		}


		private void FilterPrivilegeClub(DataTable dt)
		{
			// If Privilege Club Tier 1/2 is  enabled then need to translate
			// "Tier1" and "Tier2" to the user descriptions for their scheme.
			if (Country[CountryParameterNames.TierPCEnabled] != null && (bool)Country[CountryParameterNames.TierPCEnabled])
			{
				// Get the user translation for their scheme
				string tier1Translation;
				string tier2Translation;
				this.TranslatePrivilegeClub(out tier1Translation, out tier2Translation);

				// Update all of the parameter names and their descriptions 
				// in the Privilege Club category
				string tierCategory = Country[CountryParameterNames.TierPCEnabled, CN.ParameterCategory];
				foreach (DataRow row in dt.Rows)
					if ((string)row[CN.ParameterCategory] == tierCategory)
					{
						// Translate "Tier1" and "Tier2" strings for the users scheme
						row[CN.name] = ((string)row[CN.name]).Replace(PCTranslate.Tier1, tier1Translation);
						row[CN.name] = ((string)row[CN.name]).Replace(PCTranslate.Tier2, tier2Translation);
						row[CN.Description] = ((string)row[CN.Description]).Replace(PCTranslate.Tier1, tier1Translation);
						row[CN.Description] = ((string)row[CN.Description]).Replace(PCTranslate.Tier2, tier2Translation);
					}
			}
		}


		private void ValidatePrivilegeClub(DataTable paramList)
		{
			// Validate Parameter Category fro Privilege Club
			// Parameters are:
			//   ClubAverageStatus
			//   WarningAverageStatus
			//   WarningLeadDays
			//   ConsecutiveInstalments
			//   CreditExpiryMonths
			//   MinClubLen
			//   ReminderDays
			//   Tier1CashLen
			//   Tier1CashSpend
			//   Tier1CashMaintainLen
			//   Tier1CashMaintainSpend
			//   Tier1CreditLen
			//   Tier2CashLen
			//   Tier2CashSpend
			//   Tier2CashMaintainLen
			//   Tier2CashMaintainSpend
			//   Tier2CreditLen
			//   Tier2Discount
			//   Tier2DiscountItemNumber

			// Only do this if the Tier1/2 Privilege Club is enabled
			if ((bool)Country[CountryParameterNames.TierPCEnabled])
			{
				// Get the user translation for their scheme
				string tier1Translation;
				string tier2Translation;
				this.TranslatePrivilegeClub(out tier1Translation, out tier2Translation);

				string tierCategory = Country[CountryParameterNames.TierPCEnabled, CN.ParameterCategory];
				string paramValue = "";
				string refValue = "";
				string refValue2 = "";

				foreach (DataRow row in paramList.Rows)
					if ((string)row[CN.ParameterCategory] == tierCategory)
					{
						paramValue = (string)row[CN.Value];
						switch ((string)row[CN.CodeName])
						{
							case  "ClubAverageStatus": 
								// Club Average Status should be a decimal value from one to two
								// and not less than Warning Average Status
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.WarningAverageStatus);
								if (System.Convert.ToDecimal(paramValue) < 1 ||
									System.Convert.ToDecimal(paramValue) < System.Convert.ToDecimal(refValue) ||
									System.Convert.ToDecimal(paramValue) > 2)
									throw new STLException(GetResource("M_PCVALIDATECLUBAVERAGESTATUS"));
								break;

							case  "WarningAverageStatus": 
								// Warning Average Status should be a decimal value from one up to Club Average Status
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.ClubAverageStatus);
								if (System.Convert.ToDecimal(paramValue) < 1 || System.Convert.ToDecimal(paramValue) > System.Convert.ToDecimal(refValue))
									throw new STLException(GetResource("M_PCVALIDATEWARNINGAVERAGESTATUS"));
								break;

							case "WarningLeadDays":
								// Warning Lead Days should be an integer value from one upwards 
								if (System.Convert.ToInt32(paramValue) < 1)
									throw new STLException(GetResource("M_PCVALIDATEWARNINGLEADDAYS"));
								break;

							case "CreditExpiryMonths":
								// Credit Expiry Months should be an integer value from one upwards
								if (System.Convert.ToInt32(paramValue) < 1)
									throw new STLException(GetResource("M_PCVALIDATECREDITEXPIRYMONTHS"));
								break;

							case "MinClubLen":
								// Min Club Length should be an integer value from one upwards
								// and not more than Tier1 Credit Length
								// and not more than Tier2 Credit Length
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CreditLen);
								refValue2 = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier2CreditLen);
								if (System.Convert.ToInt32(paramValue) < 1 ||
									System.Convert.ToInt32(paramValue) > System.Convert.ToDecimal(refValue) ||
									System.Convert.ToInt32(paramValue) > System.Convert.ToDecimal(refValue2))
									throw new STLException(GetResource("M_PCVALIDATEMINCLUBLEN", new object [] {tier1Translation,tier2Translation}));
								break;

							case "ReminderDays":
								// Days between Invitation & Reminder should be an integer from one upwards
								if (System.Convert.ToInt32(paramValue) < 1)
									throw new STLException(GetResource("M_PCVALIDATEREMINDERDAYS"));
								break;

							case "Tier1CashLen":
								// Tier1 Cash Length should be an integer value from Tier1 Cash Maintain Length upwards
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CashMaintainLen);
								if (System.Convert.ToInt32(paramValue) < System.Convert.ToDecimal(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHLEN", new object [] {tier1Translation}));
								break;

							case "Tier1CashSpend":
								// Tier1 Cash Spend should be an integer value from Tier1 Cash Maintain Spend upwards 
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CashMaintainSpend);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) < System.Convert.ToDecimal(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHSPEND", new object [] {tier1Translation}));
								break;

							case "Tier1CashMaintainLen": 
								// Tier1 Cash Maintain Length should be an integer value from one up to the value of Tier1 Cash Length
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CashLen);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) > System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHMAINTAINLEN", new object [] {tier1Translation}));
								break;

							case "Tier1CashMaintainSpend": 
								// Tier1 Cash Maintain Spend should be an integer value from one up to the value of Tier1 Cash Spend
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CashSpend);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) > System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHMAINTAINSPEND", new object [] {tier1Translation}));
								break;

							case "Tier1CreditLen": 
								// Tier1 Credit Length should be an integer value from Min Club Length upwards
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.MinClubLen);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) < System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCREDITLEN", new object [] {tier1Translation}));
								break;

							case "Tier2CashLen": 
								// Tier2 Cash Length should be an integer value from Tier2 Cash Maintain Length upwards
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier2CashMaintainLen);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) < System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHLEN", new object [] {tier2Translation}));
								break;

							case "Tier2CashSpend": 
								// Tier2 Cash Spend should be an integer value from Tier1 Cash Spend upwards 
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier1CashSpend);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) < System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHSPEND", new object [] {tier2Translation}));
								break;

							case "Tier2CashMaintainLen": 
								// Tier2 Cash Maintain Length should be an integer value from one up to the value of Tier2 Cash Length
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier2CashLen);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) > System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHMAINTAINLEN", new object [] {tier2Translation}));
								break;

							case "Tier2CashMaintainSpend":  
								// Tier2 Cash Maintain Spend should be an integer value from one up to the value of Tier2 Cash Spend
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.Tier2CashSpend);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) > System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCASHMAINTAINSPEND", new object [] {tier2Translation}));
								break;

							case "Tier2CreditLen":
								// Tier2 Credit Length should be an integer value from Min Club Length upwards
								refValue = this.GetNewParamValue(paramList, tierCategory, CountryParameterNames.MinClubLen);
								if (System.Convert.ToInt32(paramValue) < 1 || System.Convert.ToInt32(paramValue) < System.Convert.ToInt32(refValue))
									throw new STLException(GetResource("M_PCVALIDATETIERCREDITLEN", new object [] {tier2Translation}));
								break;
							case "Tier2Discount":
								// Tier2 Discount % should be a decimal from 0 to 100
								if (System.Convert.ToDecimal(paramValue) < 0 || System.Convert.ToDecimal(paramValue) > 100)
									throw new STLException(GetResource("M_PCVALIDATETIERDISCOUNT", new object [] {tier2Translation}));
								break;

							case "Tier2DiscountItemNumber":
								// Tier2 Discount Item Number should be a non blank string up to eight characters
								if (paramValue.Trim().Length == 0 || paramValue.Trim().Length > 8)
									throw new STLException(GetResource("M_PCVALIDATETIERDISCOUNTITEMNUMBER", new object [] {tier2Translation}));
								break;
							default:
								break;
						}
					}
			}
		}

		private string GetNewParamValue (DataTable paramList, string category, string paramName)
		{
			string paramValue = "";
			foreach (DataRow row in paramList.Rows)
				if ((string)row[CN.ParameterCategory] == category && (string)row[CN.CodeName] == paramName)
					paramValue = (string)row[CN.Value];

			if (paramValue == "")
			{
				// This parameter has not been changed by the user
				// so load it from the country parameters instead
				paramValue = Convert.ToString(Country[paramName]);
			}

			return paramValue;
		}

		public void SetSystemStatus(SqlConnection conn, SqlTransaction trans, string countryCode, 
										string status)
		{
			DCountry c = new DCountry();
			c.SetSystemStatus(conn, trans, countryCode, status);
		}

		public string GetDataBaseVersion()
		{
			DCountry c = new DCountry();
			return c.GetDataBaseVersion();
		}

        public bool CheckConfig(char country, int branch)
        {
            DCountry c = new DCountry();
            return c.CheckConfig(country, branch);
        }

        public bool IsRIFileTransfer()
        {
            var c = new DCountry();

            c.GetDefaults(Convert.ToString(Country[CountryParameterNames.CountryCode]));

            return Convert.ToBoolean(c.Table.Rows[0]["RIFileTransfer"]);

        }
	}
}
