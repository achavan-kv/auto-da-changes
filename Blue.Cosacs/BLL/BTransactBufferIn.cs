using System;
using STL.Common;

namespace STL.BLL
{
	/// <summary>
	/// This class will handle all access to the transact INPUT buffer.
	/// Access will be controlled using properties so that the 
	/// buffer is encapsulated and presents as a normal object with 
	/// member variables.
	/// </summary>
	public class BTransactBufferIn : StringBuffer
	{

		public BTransactBufferIn()
		{
			ClearBuffer(2100);
		}

		#region input buffer properties

		#region applicant one properties
		public string AppType
		{
			get {return ExtractString(Offset.AppType, Length.AppType);	}
			set	{Append(Offset.AppType, Length.AppType, value);	}
		}

		public string NoApps
		{
			get	{return ExtractString(Offset.NoApps, Length.NoApps);}
			set	{Append(Offset.NoApps, Length.NoApps, value);}
		}

		public string Country
		{
			get	{return ExtractString(Offset.Country, Length.Country);}
			set	{Append(Offset.Country, Length.Country, value);	}
		}

		public string AccType
		{
			get	{return ExtractString(Offset.AccType, Length.AccType);}
			set	{Append(Offset.AccType, Length.AccType, value);	}
		}

		public string AccNumber
		{
			get	{return ExtractString(Offset.AccNumber, Length.AccNumber);	}
			set	{Append(Offset.AccNumber, Length.AccNumber, value);	}
		}

		public DateTime AppDate
		{
			get	{return ExtractDate(Offset.AppDate, Length.AppDate);}
			set	{Append(Offset.AppDate, Length.AppDate, value);	}
		}

		public string Bureau
		{
			get	{return ExtractString(Offset.Bureau, Length.Bureau);	}
			set	{Append(Offset.Bureau, Length.Bureau, value);	}
		}
	
		public string IDNum
		{
			get	{return ExtractString(Offset.IDNum, Length.IDNum);	}
			set	{Append(Offset.IDNum, Length.IDNum, value);	}
		}
	
		public string IDType
		{
			get	{return ExtractString(Offset.IDType, Length.IDType);	}
			set	{Append(Offset.IDType, Length.IDType, value);	}
		}

		public string Title
		{
			get	{return ExtractString(Offset.Title, Length.Title);	}
			set	{Append(Offset.Title, Length.Title, value);	}
		}

		public string Forename
		{
			get	{return ExtractString(Offset.Forename, Length.Forename);	}
			set	{Append(Offset.Forename, Length.Forename, value);	}
		}

		public string MiddleName
		{
			get	{return ExtractString(Offset.MiddleName, Length.MiddleName);	}
			set	{Append(Offset.MiddleName, Length.MiddleName, value);	}
		}

		public string Surname
		{
			get	{return ExtractString(Offset.Surname, Length.Surname);	}
			set	{Append(Offset.Surname, Length.Surname, value);	}
		}

		public string Fullname
		{
			get	{return ExtractString(Offset.Fullname, Length.Fullname);	}
			set	{Append(Offset.Fullname, Length.Fullname, value);	}
		}

		public string Alias
		{
			get	{return ExtractString(Offset.Alias, Length.Alias);	}
			set	{Append(Offset.Alias, Length.Alias, value);	}
		}

		public string PrevSurname
		{
			get	{return ExtractString(Offset.PrevSurname, Length.PrevSurname);	}
			set	{Append(Offset.PrevSurname, Length.PrevSurname, value);	}
		}

		public string Address1
		{
			get	{return ExtractString(Offset.Address1, Length.Address1);	}
			set	{Append(Offset.Address1, Length.Address1, value);	}
		}

		public string Address2
		{
			get	{return ExtractString(Offset.Address2, Length.Address2);	}
			set	{Append(Offset.Address2, Length.Address2, value);	}
		}

		public string Address3
		{
			get	{return ExtractString(Offset.Address3, Length.Address3);	}
			set	{Append(Offset.Address3, Length.Address3, value);	}
		}

		public string Address4
		{
			get	{return ExtractString(Offset.Address4, Length.Address4);	}
			set	{Append(Offset.Address4, Length.Address4, value);	}
		}

		public string PostCode
		{
			get	{return ExtractString(Offset.PostCode, Length.PostCode);	}
			set	{Append(Offset.PostCode, Length.PostCode, value);	}
		}

		public int AddressYear
		{
			get	{return ExtractInt32(Offset.AddressYear, Length.AddressYear);	}
			set	{Append(Offset.AddressYear, Length.AddressYear, value);	}
		}

		public int AddressMonth
		{
			get	{return ExtractInt32(Offset.AddressMonth, Length.AddressMonth);	}
			set	{Append(Offset.AddressMonth, Length.AddressMonth, value);	}
		}

		public string ResStatus
		{
			get	{return ExtractString(Offset.ResStatus, Length.ResStatus);	}
			set	{Append(Offset.ResStatus, Length.ResStatus, value);	}
		}

		public string PAddress1
		{
			get	{return ExtractString(Offset.PAddress1, Length.PAddress1);	}
			set	{Append(Offset.PAddress1, Length.PAddress1, value);	}
		}

		public string PAddress2
		{
			get	{return ExtractString(Offset.PAddress2, Length.PAddress2);	}
			set	{Append(Offset.PAddress2, Length.PAddress2, value);	}
		}

		public string PAddress3
		{
			get	{return ExtractString(Offset.PAddress3, Length.PAddress3);	}
			set	{Append(Offset.PAddress3, Length.PAddress3, value);	}
		}

		public string PAddress4
		{
			get	{return ExtractString(Offset.PAddress4, Length.PAddress4);	}
			set	{Append(Offset.PAddress4, Length.PAddress4, value);	}
		}

		public string PPostCode
		{
			get	{return ExtractString(Offset.PPostCode, Length.PPostCode);	}
			set	{Append(Offset.PPostCode, Length.PPostCode, value);	}
		}

		public int PAddressYear
		{
			get	{return ExtractInt32(Offset.PAddressYear, Length.PAddressYear);	}
			set	{Append(Offset.PAddressYear, Length.PAddressYear, value);	}
		}

		public int PAddressMonth
		{
			get	{return ExtractInt32(Offset.PAddressMonth, Length.PAddressMonth);	}
			set	{Append(Offset.PAddressMonth, Length.PAddressMonth, value);	}
		}

		public string PResStatus
		{
			get	{return ExtractString(Offset.PResStatus, Length.PResStatus);	}
			set	{Append(Offset.PResStatus, Length.PResStatus, value);	}
		}

		public string PropType
		{
			get	{return ExtractString(Offset.PropType, Length.PropType);	}
			set	{Append(Offset.PropType, Length.PropType, value);	}
		}

		public decimal MonthlyRent
		{
			get	{return ExtractDecimal(Offset.MonthlyRent, Length.MonthlyRent);	}
			set	{Append(Offset.MonthlyRent, Length.MonthlyRent, value);	}
		}

		public string Tel1
		{
			get	{return ExtractString(Offset.Tel1, Length.Tel1);	}
			set	{Append(Offset.Tel1, Length.Tel1, value);	}
		}

		public string Tel2
		{
			get	{return ExtractString(Offset.Tel2, Length.Tel2);	}
			set	{Append(Offset.Tel2, Length.Tel2, value);	}
		}

		public string MobileTel
		{
			get	{return ExtractString(Offset.MobileTel, Length.MobileTel);	}
			set	{Append(Offset.MobileTel, Length.MobileTel, value);	}
		}

		public DateTime DOB
		{
			get	{return ExtractDate(Offset.DOB, Length.DOB);}
			set	{Append(Offset.DOB, Length.DOB, value);	}
		}

		public int Age
		{
			get	{return ExtractInt32(Offset.Age, Length.Age);	}
			set	{Append(Offset.Age, Length.Age, value);	}
		}

		public string MaritalStat
		{
			get	{return ExtractString(Offset.MaritalStat, Length.MaritalStat);	}
			set	{Append(Offset.MaritalStat, Length.MaritalStat, value);	}
		}

		public int Dependants
		{
			get	{return ExtractInt32(Offset.Dependants, Length.Dependants);	}
			set	{Append(Offset.Dependants, Length.Dependants, value);	}
		}

		public string Nationality
		{
			get	{return ExtractString(Offset.Nationality, Length.Nationality);	}
			set	{Append(Offset.Nationality, Length.Nationality, value);	}
		}

		public string Ethnicity
		{
			get	{return ExtractString(Offset.Ethnicity, Length.Ethnicity);	}
			set	{Append(Offset.Ethnicity, Length.Ethnicity, value);	}
		}

		public string PrivClub
		{
			get	{return ExtractString(Offset.PrivClub, Length.PrivClub);	}
			set	{Append(Offset.PrivClub, Length.PrivClub, value);	}
		}

		public string Sex
		{
			get	{return ExtractString(Offset.Sex, Length.Sex);	}
			set	{Append(Offset.Sex, Length.Sex, value);	}
		}

		public string EmploymentStatus
		{
			get	{return ExtractString(Offset.EmploymentStatus, Length.EmploymentStatus);	}
			set	{Append(Offset.EmploymentStatus, Length.EmploymentStatus, value);	}
		}

		public int EmploymentYears
		{
			get	{return ExtractInt32(Offset.EmploymentYears, Length.EmploymentYears);	}
			set	{Append(Offset.EmploymentYears, Length.EmploymentYears, value);	}
		}

		public int EmploymentMonths
		{
			get	{return ExtractInt32(Offset.EmploymentMonths, Length.EmploymentMonths);	}
			set	{Append(Offset.EmploymentMonths, Length.EmploymentMonths, value);	}
		}

		public string Occupation
		{
			get	{return ExtractString(Offset.Occupation, Length.Occupation);	}
			set	{Append(Offset.Occupation, Length.Occupation, value);	}
		}

		public string EmploymentTel1
		{
			get	{return ExtractString(Offset.EmploymentTel1, Length.EmploymentTel1);	}
			set	{Append(Offset.EmploymentTel1, Length.EmploymentTel1, value);	}
		}

		public string EmploymentTel2
		{
			get	{return ExtractString(Offset.EmploymentTel2, Length.EmploymentTel2);	}
			set	{Append(Offset.EmploymentTel2, Length.EmploymentTel2, value);	}
		}

		public int PEmploymentYears
		{
			get	{return ExtractInt32(Offset.PEmploymentYears, Length.PEmploymentYears);	}
			set	{Append(Offset.PEmploymentYears, Length.PEmploymentYears, value);	}
		}

		public int PEmploymentMonths
		{
			get	{return ExtractInt32(Offset.PEmploymentMonths, Length.PEmploymentMonths);	}
			set	{Append(Offset.PEmploymentMonths, Length.PEmploymentMonths, value);	}
		}

		public string Bank
		{
			get	{return ExtractString(Offset.Bank, Length.Bank);	}
			set	{Append(Offset.Bank, Length.Bank, value);	}
		}

		public decimal NetMonthlyIncome
		{
			get	{return ExtractDecimal(Offset.NetMonthlyIncome, Length.NetMonthlyIncome);	}
			set	{Append(Offset.NetMonthlyIncome, Length.NetMonthlyIncome, value);	}
		}
	
		public decimal AddMonthlyIncome
		{
			get	{return ExtractDecimal(Offset.AddMonthlyIncome, Length.AddMonthlyIncome);	}
			set	{Append(Offset.AddMonthlyIncome, Length.AddMonthlyIncome, value);	}
		}

		public string PayFrequency
		{
			get	{return ExtractString(Offset.PayFrequency, Length.PayFrequency);	}
			set	{Append(Offset.PayFrequency, Length.PayFrequency, value);	}
		}

		public string TypeAccount
		{
			get	{return ExtractString(Offset.TypeAccount, Length.TypeAccount);	}
			set	{Append(Offset.TypeAccount, Length.TypeAccount, value);	}
		}

		public int BankYears
		{
			get	{return ExtractInt32(Offset.BankYears, Length.BankYears);	}
			set	{Append(Offset.BankYears, Length.BankYears, value);	}
		}

		public int BankMonths
		{
			get	{return ExtractInt32(Offset.BankMonths, Length.BankMonths);	}
			set	{Append(Offset.BankMonths, Length.BankMonths, value);	}
		}

		public decimal OtherCommitments
		{
			get	{return ExtractDecimal(Offset.OtherCommitments, Length.OtherCommitments);	}
			set	{Append(Offset.OtherCommitments, Length.OtherCommitments, value);	}
		}
		#endregion

		#region applicant two properties
		public string A2IDNum
		{
			get	{return ExtractString(Offset.A2IDNum, Length.A2IDNum);	}
			set	{Append(Offset.A2IDNum, Length.A2IDNum, value);	}
		}
	
		public string A2IDType
		{
			get	{return ExtractString(Offset.A2IDType, Length.A2IDType);	}
			set	{Append(Offset.A2IDType, Length.A2IDType, value);	}
		}

		public string A2Title
		{
			get	{return ExtractString(Offset.A2Title, Length.A2Title);	}
			set	{Append(Offset.A2Title, Length.A2Title, value);	}
		}

		public string A2Forename
		{
			get	{return ExtractString(Offset.A2Forename, Length.A2Forename);	}
			set	{Append(Offset.A2Forename, Length.A2Forename, value);	}
		}

		public string A2MiddleName
		{
			get	{return ExtractString(Offset.A2MiddleName, Length.A2MiddleName);	}
			set	{Append(Offset.A2MiddleName, Length.A2MiddleName, value);	}
		}

		public string A2Surname
		{
			get	{return ExtractString(Offset.A2Surname, Length.A2Surname);	}
			set	{Append(Offset.A2Surname, Length.A2Surname, value);	}
		}

		public string A2Fullname
		{
			get	{return ExtractString(Offset.A2Fullname, Length.A2Fullname);	}
			set	{Append(Offset.A2Fullname, Length.A2Fullname, value);	}
		}

		public string A2Alias
		{
			get	{return ExtractString(Offset.A2Alias, Length.A2Alias);	}
			set	{Append(Offset.A2Alias, Length.A2Alias, value);	}
		}

		public string A2PrevSurname
		{
			get	{return ExtractString(Offset.A2PrevSurname, Length.A2PrevSurname);	}
			set	{Append(Offset.A2PrevSurname, Length.A2PrevSurname, value);	}
		}

		public string A2Address1
		{
			get	{return ExtractString(Offset.A2Address1, Length.A2Address1);	}
			set	{Append(Offset.A2Address1, Length.A2Address1, value);	}
		}

		public string A2Address2
		{
			get	{return ExtractString(Offset.A2Address2, Length.A2Address2);	}
			set	{Append(Offset.A2Address2, Length.A2Address2, value);	}
		}

		public string A2Address3
		{
			get	{return ExtractString(Offset.A2Address3, Length.A2Address3);	}
			set	{Append(Offset.A2Address3, Length.A2Address3, value);	}
		}

		public string A2Address4
		{
			get	{return ExtractString(Offset.A2Address4, Length.A2Address4);	}
			set	{Append(Offset.A2Address4, Length.A2Address4, value);	}
		}

		public string A2PostCode
		{
			get	{return ExtractString(Offset.A2PostCode, Length.A2PostCode);	}
			set	{Append(Offset.A2PostCode, Length.A2PostCode, value);	}
		}

		public int A2AddressYear
		{
			get	{return ExtractInt32(Offset.A2AddressYear, Length.A2AddressYear);	}
			set	{Append(Offset.A2AddressYear, Length.A2AddressYear, value);	}
		}

		public int A2AddressMonth
		{
			get	{return ExtractInt32(Offset.A2AddressMonth, Length.A2AddressMonth);	}
			set	{Append(Offset.A2AddressMonth, Length.A2AddressMonth, value);	}
		}

		public string A2PAddress1
		{
			get	{return ExtractString(Offset.A2PAddress1, Length.A2PAddress1);	}
			set	{Append(Offset.A2PAddress1, Length.A2PAddress1, value);	}
		}

		public string A2PAddress2
		{
			get	{return ExtractString(Offset.A2PAddress2, Length.A2PAddress2);	}
			set	{Append(Offset.A2PAddress2, Length.A2PAddress2, value);	}
		}

		public string A2PAddress3
		{
			get	{return ExtractString(Offset.A2PAddress3, Length.A2PAddress3);	}
			set	{Append(Offset.A2PAddress3, Length.A2PAddress3, value);	}
		}

		public string A2PAddress4
		{
			get	{return ExtractString(Offset.A2PAddress4, Length.A2PAddress4);	}
			set	{Append(Offset.A2PAddress4, Length.A2PAddress4, value);	}
		}

		public string A2PPostCode
		{
			get	{return ExtractString(Offset.A2PPostCode, Length.A2PPostCode);	}
			set	{Append(Offset.A2PPostCode, Length.A2PPostCode, value);	}
		}

		public int A2PAddressYear
		{
			get	{return ExtractInt32(Offset.A2PAddressYear, Length.A2PAddressYear);	}
			set	{Append(Offset.A2PAddressYear, Length.A2PAddressYear, value);	}
		}

		public int A2PAddressMonth
		{
			get	{return ExtractInt32(Offset.A2PAddressMonth, Length.A2PAddressMonth);	}
			set	{Append(Offset.A2PAddressMonth, Length.A2PAddressMonth, value);	}
		}

		public decimal A2MonthlyRent
		{
			get	{return ExtractDecimal(Offset.A2MonthlyRent, Length.A2MonthlyRent);	}
			set	{Append(Offset.A2MonthlyRent, Length.A2MonthlyRent, value);	}
		}

		public string A2Tel1
		{
			get	{return ExtractString(Offset.A2Tel1, Length.A2Tel1);	}
			set	{Append(Offset.A2Tel1, Length.A2Tel1, value);	}
		}

		public string A2Tel2
		{
			get	{return ExtractString(Offset.A2Tel2, Length.A2Tel2);	}
			set	{Append(Offset.A2Tel2, Length.A2Tel2, value);	}
		}

		public string A2MobileTel
		{
			get	{return ExtractString(Offset.A2MobileTel, Length.A2MobileTel);	}
			set	{Append(Offset.A2MobileTel, Length.A2MobileTel, value);	}
		}

		public DateTime A2DOB
		{
			get	{return ExtractDate(Offset.A2DOB, Length.A2DOB);}
			set	{Append(Offset.A2DOB, Length.A2DOB, value);	}
		}

		public int A2Age
		{
			get	{return ExtractInt32(Offset.A2Age, Length.A2Age);	}
			set	{Append(Offset.A2Age, Length.A2Age, value);	}
		}

		public string A2EmploymentStatus
		{
			get	{return ExtractString(Offset.A2EmploymentStatus, Length.A2EmploymentStatus);	}
			set	{Append(Offset.A2EmploymentStatus, Length.A2EmploymentStatus, value);	}
		}

		public int A2EmploymentYears
		{
			get	{return ExtractInt32(Offset.A2EmploymentYears, Length.A2EmploymentYears);	}
			set	{Append(Offset.A2EmploymentYears, Length.A2EmploymentYears, value);	}
		}

		public int A2EmploymentMonths
		{
			get	{return ExtractInt32(Offset.A2EmploymentMonths, Length.A2EmploymentMonths);	}
			set	{Append(Offset.A2EmploymentMonths, Length.A2EmploymentMonths, value);	}
		}

		public string A2Occupation
		{
			get	{return ExtractString(Offset.A2Occupation, Length.A2Occupation);	}
			set	{Append(Offset.A2Occupation, Length.A2Occupation, value);	}
		}

		public decimal A2NetMonthlyIncome
		{
			get	{return ExtractDecimal(Offset.A2NetMonthlyIncome, Length.A2NetMonthlyIncome);	}
			set	{Append(Offset.A2NetMonthlyIncome, Length.A2NetMonthlyIncome, value);	}
		}
	
		public decimal A2AddMonthlyIncome
		{
			get	{return ExtractDecimal(Offset.A2AddMonthlyIncome, Length.A2AddMonthlyIncome);	}
			set	{Append(Offset.A2AddMonthlyIncome, Length.A2AddMonthlyIncome, value);	}
		}

		public int A2BankYears
		{
			get	{return ExtractInt32(Offset.A2BankYears, Length.A2BankYears);	}
			set	{Append(Offset.A2BankYears, Length.A2BankYears, value);	}
		}

		public int A2BankMonths
		{
			get	{return ExtractInt32(Offset.A2BankMonths, Length.A2BankMonths);	}
			set	{Append(Offset.A2BankMonths, Length.A2BankMonths, value);	}
		}

		public decimal A2OtherCommitments
		{
			get	{return ExtractDecimal(Offset.A2OtherCommitments, Length.A2OtherCommitments);	}
			set	{Append(Offset.A2OtherCommitments, Length.A2OtherCommitments, value);	}
		}
		#endregion

		#region calculated fields properties
		public string ProdCat
		{
			get	{return ExtractString(Offset.ProdCat, Length.ProdCat);	}
			set	{Append(Offset.ProdCat, Length.ProdCat, value);	}
		}

		public string ProdCode
		{
			get	{return ExtractString(Offset.ProdCode, Length.ProdCode);	}
			set	{Append(Offset.ProdCode, Length.ProdCode, value);	}
		}

		public int MaxProd
		{
			get	{return ExtractInt32(Offset.MaxProd, Length.MaxProd);	}
			set	{Append(Offset.MaxProd, Length.MaxProd, value);	}
		}

		public int NumPurch
		{
			get	{return ExtractInt32(Offset.NumPurch, Length.NumPurch);	}
			set	{Append(Offset.NumPurch, Length.NumPurch, value);	}
		}

		public decimal TotalValue
		{
			get	{return ExtractDecimal(Offset.TotalValue, Length.TotalValue);	}
			set	{Append(Offset.TotalValue, Length.TotalValue, value);	}
		}

		public decimal LoanAmount
		{
			get	{return ExtractDecimal(Offset.LoanAmount, Length.LoanAmount);	}
			set	{Append(Offset.LoanAmount, Length.LoanAmount, value);	}
		}

		public int Term
		{
			get	{return ExtractInt32(Offset.Term, Length.Term);	}
			set	{Append(Offset.Term, Length.Term, value);	}
		}

		public decimal Deposit
		{
			get	{return ExtractDecimal(Offset.Deposit, Length.Deposit);	}
			set	{Append(Offset.Deposit, Length.Deposit, value);	}
		}

		public decimal Finance
		{
			get	{return ExtractDecimal(Offset.Finance, Length.Finance);	}
			set	{Append(Offset.Finance, Length.Finance, value);	}
		}

		public decimal AgreementTotal
		{
			get	{return ExtractDecimal(Offset.AgreementTotal, Length.AgreementTotal);	}
			set	{Append(Offset.AgreementTotal, Length.AgreementTotal, value);	}
		}

		public decimal MonthlyInstalment
		{
			get	{return ExtractDecimal(Offset.MonthlyInstalment, Length.MonthlyInstalment);	}
			set	{Append(Offset.MonthlyInstalment, Length.MonthlyInstalment, value);	}
		}

		public decimal DepositPercentage
		{
			get	{return ExtractDecimal(Offset.DepositPercentage, Length.DepositPercentage);	}
			set	{Append(Offset.DepositPercentage, Length.DepositPercentage, value);	}
		}

		public decimal InstalmentPercentage
		{
			get	{return ExtractDecimal(Offset.InstalmentPercentage, Length.InstalmentPercentage);	}
			set	{Append(Offset.InstalmentPercentage, Length.InstalmentPercentage, value);	}
		}

		public string Insurance
		{
			get	{return ExtractString(Offset.Insurance, Length.Insurance);	}
			set	{Append(Offset.Insurance, Length.Insurance, value);	}
		}

		public string PayMethod
		{
			get	{return ExtractString(Offset.PayMethod, Length.PayMethod);	}
			set	{Append(Offset.PayMethod, Length.PayMethod, value);	}
		}

		public string Location
		{
			get	{return ExtractString(Offset.Location, Length.Location);	}
			set	{Append(Offset.Location, Length.Location, value);	}
		}

		public string WorstCurrent
		{
			get	{return ExtractString(Offset.WorstCurrent, Length.WorstCurrent);	}
			set	{Append(Offset.WorstCurrent, Length.WorstCurrent, value);	}
		}

		public string StatMostRecentCurrent
		{
			get	{return ExtractString(Offset.StatMostRecentCurrent, Length.StatMostRecentCurrent);	}
			set	{Append(Offset.StatMostRecentCurrent, Length.StatMostRecentCurrent, value);	}
		}

		public string WorstSettled
		{
			get	{return ExtractString(Offset.WorstSettled, Length.WorstSettled);	}
			set	{Append(Offset.WorstSettled, Length.WorstSettled, value);	}
		}

		public string StatMostRecentSettled
		{
			get	{return ExtractString(Offset.StatMostRecentSettled, Length.StatMostRecentSettled);	}
			set	{Append(Offset.StatMostRecentSettled, Length.StatMostRecentSettled, value);	}
		}

		public string StatLargestSettled
		{
			get	{return ExtractString(Offset.StatLargestSettled, Length.StatLargestSettled);	}
			set	{Append(Offset.StatLargestSettled, Length.StatLargestSettled, value);	}
		}

		public string SizeLargestSettled
		{
			get	{return ExtractString(Offset.SizeLargestSettled, Length.SizeLargestSettled);	}
			set	{Append(Offset.SizeLargestSettled, Length.SizeLargestSettled, value);	}
		}

		public decimal ExisitingInstal
		{
			get	{return ExtractDecimal(Offset.ExisitingInstal, Length.ExisitingInstal);	}
			set	{Append(Offset.ExisitingInstal, Length.ExisitingInstal, value);	}
		}

		public decimal ExistingBalance
		{
			get	{return ExtractDecimal(Offset.ExistingBalance, Length.ExistingBalance);	}
			set	{Append(Offset.ExistingBalance, Length.ExistingBalance, value);	}
		}

		public int NumCurrent
		{
			get	{return ExtractInt32(Offset.NumCurrent, Length.NumCurrent);	}
			set	{Append(Offset.NumCurrent, Length.NumCurrent, value);	}
		}

		public int NumSettled
		{
			get	{return ExtractInt32(Offset.NumSettled, Length.NumSettled);	}
			set	{Append(Offset.NumSettled, Length.NumSettled, value);	}
		}

		public string HiStatCurrAccountNow
		{
			get	{return ExtractString(Offset.HiStatCurrAccountNow, Length.HiStatCurrAccountNow);	}
			set	{Append(Offset.HiStatCurrAccountNow, Length.HiStatCurrAccountNow, value);	}
		}

		public string HiStatSettAccountNow
		{
			get	{return ExtractString(Offset.HiStatSettAccountNow, Length.HiStatSettAccountNow);	}
			set	{Append(Offset.HiStatSettAccountNow, Length.HiStatSettAccountNow, value);	}
		}

		public decimal WeightAvCurrAccountNow
		{
			get	{return ExtractWDecimal(Offset.WeightAvCurrAccountNow, Length.WeightAvCurrAccountNow);	}
			set	{AppendW(Offset.WeightAvCurrAccountNow, Length.WeightAvCurrAccountNow, value);	}
		}

		public decimal WeightAvSettAccountNow
		{
			get	{return ExtractWDecimal(Offset.WeightAvSettAccountNow, Length.WeightAvSettAccountNow);	}
			set	{AppendW(Offset.WeightAvSettAccountNow, Length.WeightAvSettAccountNow, value);	}
		}

		public decimal WeightAvCurrAccountEver
		{
			get	{return ExtractWDecimal(Offset.WeightAvCurrAccountEver, Length.WeightAvCurrAccountEver);	}
			set	{AppendW(Offset.WeightAvCurrAccountEver, Length.WeightAvCurrAccountEver, value);	}
		}

		public decimal WeightAvSettAccountEver
		{
			get	{return ExtractWDecimal(Offset.WeightAvSettAccountEver, Length.WeightAvSettAccountEver);	}
			set	{AppendW(Offset.WeightAvSettAccountEver, Length.WeightAvSettAccountEver, value);	}
		}

		public string A2WorstCurrent
		{
			get	{return ExtractString(Offset.A2WorstCurrent, Length.A2WorstCurrent);	}
			set	{Append(Offset.A2WorstCurrent, Length.A2WorstCurrent, value);	}
		}

		public string A2StatMostRecentCurrent
		{
			get	{return ExtractString(Offset.A2StatMostRecentCurrent, Length.A2StatMostRecentCurrent);	}
			set	{Append(Offset.A2StatMostRecentCurrent, Length.A2StatMostRecentCurrent, value);	}
		}

		public string A2WorstSettled
		{
			get	{return ExtractString(Offset.A2WorstSettled, Length.A2WorstSettled);	}
			set	{Append(Offset.A2WorstSettled, Length.A2WorstSettled, value);	}
		}

		public string A2StatMostRecentSettled
		{
			get	{return ExtractString(Offset.A2StatMostRecentSettled, Length.A2StatMostRecentSettled);	}
			set	{Append(Offset.A2StatMostRecentSettled, Length.A2StatMostRecentSettled, value);	}
		}

		public string A2StatLargestSettled
		{
			get	{return ExtractString(Offset.A2StatLargestSettled, Length.A2StatLargestSettled);	}
			set	{Append(Offset.A2StatLargestSettled, Length.A2StatLargestSettled, value);	}
		}

		public string A2SizeLargestSettled
		{
			get	{return ExtractString(Offset.A2SizeLargestSettled, Length.A2SizeLargestSettled);	}
			set	{Append(Offset.A2SizeLargestSettled, Length.A2SizeLargestSettled, value);	}
		}

		public decimal A2ExisitingInstal
		{
			get	{return ExtractDecimal(Offset.A2ExisitingInstal, Length.A2ExisitingInstal);	}
			set	{Append(Offset.A2ExisitingInstal, Length.A2ExisitingInstal, value);	}
		}

		public decimal A2ExistingBalance
		{
			get	{return ExtractDecimal(Offset.A2ExistingBalance, Length.A2ExistingBalance);	}
			set	{Append(Offset.A2ExistingBalance, Length.A2ExistingBalance, value);	}
		}

		public int A2NumCurrent
		{
			get	{return ExtractInt32(Offset.A2NumCurrent, Length.A2NumCurrent);	}
			set	{Append(Offset.A2NumCurrent, Length.A2NumCurrent, value);	}
		}

		public int A2NumSettled
		{
			get	{return ExtractInt32(Offset.A2NumSettled, Length.A2NumSettled);	}
			set	{Append(Offset.A2NumSettled, Length.A2NumSettled, value);	}
		}

		public int NumApps
		{
			get	{return ExtractInt32(Offset.NumApps, Length.NumApps);	}
			set	{Append(Offset.NumApps, Length.NumApps, value);	}
		}

		public string Rejects
		{
			get	{return ExtractString(Offset.Rejects, Length.Rejects);	}
			set	{Append(Offset.Rejects, Length.Rejects, value);	}
		}

		public int A2NumApps
		{
			get	{return ExtractInt32(Offset.A2NumApps, Length.A2NumApps);	}
			set	{Append(Offset.A2NumApps, Length.A2NumApps, value);	}
		}

		public string A2Rejects
		{
			get	{return ExtractString(Offset.A2Rejects, Length.A2Rejects);	}
			set	{Append(Offset.A2Rejects, Length.A2Rejects, value);	}
		}

		#endregion

		#endregion 

		/// <summary>
		/// This structure contains all the offset definitions
		/// </summary>
		private struct Offset
		{
		#region Applicant One Offsets
			public const int AppType = 9;
			public const int NoApps = 10;
			public const int Country = 11;
			public const int AccType = 13;
			public const int AccNumber = 14;
			public const int AppDate = 26;
			public const int Bureau = 34;
			public const int Filler = 35;
			public const int IDNum = 85;
			public const int IDType = 115;
			public const int Title = 116;
			public const int Forename = 131;
			public const int MiddleName = 161;
			public const int Surname = 187;
			public const int Fullname = 222;
			public const int Alias = 282;
			public const int PrevSurname = 307;
			public const int Address1 = 342;
			public const int Address2 = 368;
			public const int Address3 = 394;
			public const int Address4 = 420;
			public const int PostCode = 446;
			public const int AddressYear = 456;
			public const int AddressMonth = 458;
			public const int ResStatus = 460;
			public const int PAddress1 = 461;
			public const int PAddress2 = 487;
			public const int PAddress3 = 513;
			public const int PAddress4 = 539;
			public const int PPostCode = 565;
			public const int PAddressYear = 575;
			public const int PAddressMonth = 577;
			public const int PResStatus = 579;
			public const int PropType = 580;
			public const int MonthlyRent = 581;
			public const int Tel1 = 595;
			public const int Tel2 = 603;
			public const int MobileTel = 616;
			public const int DOB = 636;
			public const int Age = 644;
			public const int MaritalStat = 647;
			public const int Dependants = 648;
			public const int Nationality = 650;
			public const int Ethnicity = 652;
			public const int PrivClub = 653;
			public const int Sex = 654;
			public const int Filler2 = 655;
			public const int EmploymentStatus = 695;
			public const int EmploymentYears = 696;
			public const int EmploymentMonths = 698;
			public const int Occupation = 700;
			public const int EmploymentTel1 = 702;
			public const int EmploymentTel2 = 710;
			public const int PEmploymentYears = 723;
			public const int PEmploymentMonths = 725;
			public const int Filler3 = 727;
			public const int Bank = 777;
			public const int NetMonthlyIncome = 783;
			public const int AddMonthlyIncome = 797;
			public const int PayFrequency = 811;
			public const int TypeAccount = 812;
			public const int BankYears = 813;
			public const int BankMonths = 815;
			public const int OtherCommitments = 817;
			public const int Filler4 = 831;
		#endregion
		
		#region Applicant Two Offsets
			public const int A2IDNum = 881;
			public const int A2IDType = 911;
			public const int A2Title = 912;
			public const int A2Forename = 927;
			public const int A2MiddleName = 957;
			public const int A2Surname = 983;
			public const int A2Fullname = 1018;
			public const int A2Alias = 1078;
			public const int A2PrevSurname = 1103;
			public const int A2Address1 = 1138;
			public const int A2Address2 = 1164;
			public const int A2Address3 = 1190;
			public const int A2Address4 = 1216;
			public const int A2PostCode = 1242;
			public const int A2AddressYear = 1252;
			public const int A2AddressMonth = 1254;
			public const int A2PAddress1 = 1256;
			public const int A2PAddress2 = 1282;
			public const int A2PAddress3 = 1308;
			public const int A2PAddress4 = 1334;
			public const int A2PPostCode = 1360;
			public const int A2PAddressYear = 1370;
			public const int A2PAddressMonth = 1372;
			public const int A2MonthlyRent = 1374;
			public const int A2Tel1 = 1388;
			public const int A2Tel2 = 1396;
			public const int A2MobileTel = 1409;
			public const int A2DOB = 1429;
			public const int A2Age = 1437;
			public const int Filler5 = 1440;
			public const int A2EmploymentStatus = 1481;
			public const int A2EmploymentYears = 1482;
			public const int A2EmploymentMonths = 1484;
			public const int A2Occupation = 1486;
			public const int Filler6 = 1488;
			public const int A2NetMonthlyIncome = 1508;
			public const int A2AddMonthlyIncome = 1522;
			public const int A2BankYears = 1536;
			public const int A2BankMonths = 1538;
			public const int A2OtherCommitments = 1540;
			public const int Filler7 = 1554;
		#endregion

		#region Calculated Field Offsets
			public const int ProdCat = 1604;
			public const int ProdCode = 1606;
			public const int MaxProd = 1614;
			public const int NumPurch = 1616;
			public const int TotalValue = 1618;
			public const int LoanAmount = 1632;
			public const int Term = 1646;
			public const int Deposit = 1648;
			public const int Finance = 1662;
			public const int AgreementTotal = 1676;
			public const int MonthlyInstalment = 1690;
			public const int DepositPercentage = 1704;
			public const int InstalmentPercentage = 1710;
			public const int Insurance = 1716;
			public const int PayMethod = 1717;
			public const int Location = 1718;
			public const int Filler8 = 1719;
			public const int WorstCurrent = 1769;
			public const int StatMostRecentCurrent = 1770;
			public const int WorstSettled = 1771;
			public const int StatMostRecentSettled = 1772;
			public const int StatLargestSettled = 1773;
			public const int SizeLargestSettled = 1774;
			public const int ExisitingInstal = 1775;
			public const int ExistingBalance = 1789;
			public const int NumCurrent = 1803;
			public const int NumSettled = 1805;
			public const int BehaviourScore1 = 1807;
			public const int BehaviourScore2 = 1810;
			public const int BehaviourScore3 = 1813;
			public const int HiStatCurrAccountNow = 1816;
			public const int HiStatSettAccountNow = 1817;
			public const int WeightAvCurrAccountNow = 1818;
			public const int WeightAvSettAccountNow = 1821;
			public const int WeightAvCurrAccountEver = 1824;
			public const int WeightAvSettAccountEver = 1827;
			public const int Filler9 = 1830;
			public const int A2WorstCurrent = 1860;
			public const int A2StatMostRecentCurrent = 1861;
			public const int A2WorstSettled = 1862;
			public const int A2StatMostRecentSettled = 1863;
			public const int A2StatLargestSettled = 1864;
			public const int A2SizeLargestSettled = 1865;
			public const int A2ExisitingInstal = 1866;
			public const int A2ExistingBalance = 1880;
			public const int A2NumCurrent = 1894;
			public const int A2NumSettled = 1896;
			public const int A2BehaviourScore1 = 1898;
			public const int A2BehaviourScore2 = 1901;
			public const int A2BehaviourScore3 = 1904;
			public const int Filler10 = 1907;
			public const int NumApps = 1951;
			public const int Rejects = 1953;
			public const int Filler11 = 1954;
			public const int A2NumApps = 2004;
			public const int A2Rejects = 2006;
			public const int Filler12 = 2007;
		#endregion
		}

		/// <summary>
		/// This structure contains all the field Lengths
		/// </summary>
		private struct Length
		{
		#region Applicant one lengths
			public const int AppType = 1;
			public const int NoApps = 1;
			public const int Country = 2;
			public const int AccType = 1;
			public const int AccNumber = 12;
			public const int AppDate = 8;
			public const int Bureau = 1;
			public const int Filler = 50;
			public const int IDNum = 30;
			public const int IDType = 1;
			public const int Title = 15;
			public const int Forename = 30;
			public const int MiddleName = 26;
			public const int Surname = 35;
			public const int Fullname = 60;
			public const int Alias = 25;
			public const int PrevSurname = 35;
			public const int Address1 = 26;
			public const int Address2 = 26;
			public const int Address3 = 26;
			public const int Address4 = 26;
			public const int PostCode = 10;
			public const int AddressYear = 2;
			public const int AddressMonth = 2;
			public const int ResStatus = 1;
			public const int PAddress1 = 26;
			public const int PAddress2 = 26;
			public const int PAddress3 = 26;
			public const int PAddress4 = 26;
			public const int PPostCode = 10;
			public const int PAddressYear = 2;
			public const int PAddressMonth = 2;
			public const int PResStatus = 1;
			public const int PropType = 1;
			public const int MonthlyRent = 14;
			public const int Tel1 = 8;
			public const int Tel2 = 13;
			public const int MobileTel = 20;
			public const int DOB = 8;
			public const int Age = 3;
			public const int MaritalStat = 1;
			public const int Dependants = 2;
			public const int Nationality = 2;
			public const int Ethnicity = 1;
			public const int PrivClub = 1;
			public const int Sex = 1;
			public const int Filler2 = 49;
			public const int EmploymentStatus = 1;
			public const int EmploymentYears = 2;
			public const int EmploymentMonths = 2;
			public const int Occupation = 2;
			public const int EmploymentTel1 = 8;
			public const int EmploymentTel2 = 13;
			public const int PEmploymentYears = 2;
			public const int PEmploymentMonths = 2;
			public const int Filler3 = 50;
			public const int Bank = 6;
			public const int NetMonthlyIncome = 14;
			public const int AddMonthlyIncome = 14;
			public const int PayFrequency = 1;
			public const int TypeAccount = 1;
			public const int BankYears = 2;
			public const int BankMonths = 2;
			public const int OtherCommitments = 14;
			public const int Filler4 = 50;
		#endregion
		
		#region Applicant two lengths
			public const int A2IDNum = 30;
			public const int A2IDType = 1;
			public const int A2Title = 15;
			public const int A2Forename = 30;
			public const int A2MiddleName = 26;
			public const int A2Surname = 35;
			public const int A2Fullname = 60;
			public const int A2Alias = 25;
			public const int A2PrevSurname = 35;
			public const int A2Address1 = 26;
			public const int A2Address2 = 26;
			public const int A2Address3 = 26;
			public const int A2Address4 = 26;
			public const int A2PostCode = 10;
			public const int A2AddressYear = 2;
			public const int A2AddressMonth = 2;
			public const int A2PAddress1 = 26;
			public const int A2PAddress2 = 26;
			public const int A2PAddress3 = 26;
			public const int A2PAddress4 = 26;
			public const int A2PPostCode = 10;
			public const int A2PAddressYear = 2;
			public const int A2PAddressMonth = 2;
			public const int A2MonthlyRent = 14;
			public const int A2Tel1 = 8;
			public const int A2Tel2 = 13;
			public const int A2MobileTel = 20;
			public const int A2DOB = 8;
			public const int A2Age = 3;
			public const int Filler5 = 41;
			public const int A2EmploymentStatus = 1;
			public const int A2EmploymentYears = 2;
			public const int A2EmploymentMonths = 2;
			public const int A2Occupation = 2;
			public const int Filler6 = 20;
			public const int A2NetMonthlyIncome = 14;
			public const int A2AddMonthlyIncome = 14;
			public const int A2BankYears = 2;
			public const int A2BankMonths = 2;
			public const int A2OtherCommitments = 14;
			public const int Filler7 = 50;
		#endregion

		#region calculated field lengths
			public const int ProdCat = 2;
			public const int ProdCode = 8;
			public const int MaxProd = 2;
			public const int NumPurch = 2;
			public const int TotalValue = 14;
			public const int LoanAmount = 14;
			public const int Term = 2;
			public const int Deposit = 14;
			public const int Finance = 14;
			public const int AgreementTotal = 14;
			public const int MonthlyInstalment = 14;
			public const int DepositPercentage = 6;
			public const int InstalmentPercentage = 6;
			public const int Insurance = 1;
			public const int PayMethod = 1;
			public const int Location = 1;
			public const int Filler8 = 50;
			public const int WorstCurrent = 1;
			public const int StatMostRecentCurrent = 1;
			public const int WorstSettled = 1;
			public const int StatMostRecentSettled = 1;
			public const int StatLargestSettled = 1;
			public const int SizeLargestSettled = 1;
			public const int ExisitingInstal = 14;
			public const int ExistingBalance = 14;
			public const int NumCurrent = 2;
			public const int NumSettled = 2;
			public const int BehaviourScore1 = 3;
			public const int BehaviourScore2 = 3;
			public const int BehaviourScore3 = 3;
			public const int HiStatCurrAccountNow = 1;
			public const int HiStatSettAccountNow = 1;
			public const int WeightAvCurrAccountNow = 3;
			public const int WeightAvSettAccountNow = 3;
			public const int WeightAvCurrAccountEver = 3;
			public const int WeightAvSettAccountEver = 3;
			public const int Filler9 = 30;
			public const int A2WorstCurrent = 1;
			public const int A2StatMostRecentCurrent = 1;
			public const int A2WorstSettled = 1;
			public const int A2StatMostRecentSettled = 1;
			public const int A2StatLargestSettled = 1;
			public const int A2SizeLargestSettled = 1;
			public const int A2ExisitingInstal = 14;
			public const int A2ExistingBalance = 14;
			public const int A2NumCurrent = 2;
			public const int A2NumSettled = 2;
			public const int A2BehaviourScore1 = 3;
			public const int A2BehaviourScore2 = 3;
			public const int A2BehaviourScore3 = 3;
			public const int Filler10 = 44;
			public const int NumApps = 2;
			public const int Rejects = 1;
			public const int Filler11 = 50;
			public const int A2NumApps = 2;
			public const int A2Rejects = 1;
			public const int Filler12 = 94;
		#endregion

		}

	}
}
