using System;
using STL.DAL;
using STL.Common;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.OperandTypes;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.SanctionStages;
using System.Diagnostics;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using Blue.Cosacs.Repositories;
using Blue.Cosacs;
using Blue.Cosacs.Shared;
using STL.Common.Constants;
using Blue.Cosacs.BLL.Equifax;
using System.Collections.Generic;
using Blue.Cosacs.BLL.EquifaxRule;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BProposal.
	/// </summary>
	public class BProposal : CommonObject
	{
		public scoreLimits ScoreLimits;

		private string _operandName = "";
		public string OperandName
		{
			get {return _operandName;}
			set {_operandName = value;}
		}

		private string _operandValue = "";
		public string OperandValue
		{
			get {return _operandValue;}
			set {_operandValue = value;}
		}

		private decimal _points = 0;
		public decimal Points
		{
			get {return _points;}
			set {_points = value;}
		}

		private string _referReasons = "";
		public string ReferReasons
		{
			get {return _referReasons;}
			set {_referReasons = value;}
		}

		private string _rejectReasons = "";
		public string RejectReasons
		{
			get {return _rejectReasons;}
			set {_rejectReasons = value;}
		}
		
		private string _propReason = "";
		public string PropReason
		{
			get {return _propReason;}
			set {_propReason = value;}
		}

		private string _propResult = "";
		public string PropResult
		{
			get {return _propResult;}
			set {_propResult = value;}
		}

        public decimal BureauCode
        {
            get { return (decimal)Country[CountryParameterNames.TransactEnabled]; }
        }

        public bool ContactBayCorp
        {
            get { return (BureauCode == 2 || BureauCode == 4); }
        }

        public bool ContactDPGroup
        {
            get { return (BureauCode == 3 || BureauCode == 4); }
        }

        public string FindSecondApplicant(string customerID, string accountNo, string relation)
		{
			DAccount acct = new DAccount();
			DCustomer cust = new DCustomer();
			string two = customerID;
			if(relation!="H")
			{				
				if(accountNo.Length!=0)
				{
					acct = new DAccount();
					if(acct.SoleOrJoint(accountNo, relation)==0)
					{
						two = acct.JointCustomerID;
					}
				}
				else
				{
					if(cust.SoleOrJoint(customerID, relation)==0)
					{
						two = cust.LinkedCustomer;
					}
				}
			}
			return two;
		}

        private class Parameters
        {

            public float UpliftPercentage{ get; set; }
            public bool AcceptedSinceSCReferral { get; set; }
            public bool AcceptedSinceSCReferralChecked { get; set; } 
            public bool ReferNoPhoneCustomers { get; set; }
            public bool HasMobilePhone { get; set; }
            public bool isExistingCustomer { get; set; }
            public string WorstCurrentEver { get; set; }
            public string WorstSettledEver { get; set; }
            public string MinimumWorstStatustoCheck { get; set; }
            //public bool 
            public SqlTransaction trans { get; set; }
            public SqlConnection conn { get; set; }
            public string AccountNo { get; set; }
            public string ruletype { get; set; }
            public string CustomerId { get; set; }
        }
		/// <summary>
		/// This method retrieves the information required to populate the 
		/// sanction screen for a new proposal. Must draw data from the following 
		/// sources:
		///		customer 
		///		bank
		///		employment
		///		accounts
		///	This method will be called once for each applicant
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		public DataSet GetProposalStage1(string customerID, string accountNo, string relation)
		{
			//This method can be used for returning proposal data for 
			//a spouse or joint applicant also
			DataSet ds = null;
			bool status = true;
			string joint = FindSecondApplicant(customerID, accountNo, relation);
			if(relation!="H")				//cater for the circumstance when there
			{								//is no second applicant with the 
				if(customerID != joint)		//required relationship
					customerID = joint;
				else
					status = false;
			}

			if(status)
			{
				ds = new DataSet();
				DCustomer cust = new DCustomer();
				cust.GetCustomerDetails(null, null, customerID);
				cust.GetCustomerHomeAddress(customerID);
				DataTable dtCust = cust.GetRow(TN.Customer);

                //uat478 rdb 15/07/08 get work address details also
                cust.GetCustomerWorkAddress(customerID);
                DataTable dtWork = cust.GetRow("WorkAddress");

				DBank bank = new DBank();
				bank.GetAccountDetails(customerID, accountNo);
				DataTable dtBank = bank.GetRow(TN.Bank);

				DEmployment emp = new DEmployment();
				emp.GetEmployment(customerID);
				DataTable dtEmployment = emp.GetRow(TN.Employment);

				DAccount acct = new DAccount();
				acct.GetStage1AccountSummary(customerID);
				DataTable dtAccountTotals  = new DataTable(TN.AccountTotals);
				dtAccountTotals.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CurrentAccounts, Type.GetType("System.Int32")),
																	  new DataColumn(CN.SettledAccounts, Type.GetType("System.Int32"))});
				dtAccountTotals.Rows.Add(new Object[] { acct.CurrentAccounts,
														  acct.SettledAccounts});

				DataTable dtAccounts = acct.Stage1;

				//if there is already a proposal for this cust/account 
				//then populate the data. If not just create a blank table 
				//with the right columns to store the data when it's entered.
				DProposal prop = new DProposal();
				prop.GetProposal(accountNo, customerID);
				DataTable dtProposal = prop.GetStage1Row(TN.Proposal);

				DAgreement agree = new DAgreement();
				agree.GetAgreement(null,null,accountNo, 1);
				DataTable dtAgreement = agree.AgreementList;

                //CR 835 Get the residential and financial data from the additional details table
                DataTable dtAdditionalFinancial     = cust.GetCustomerAdditionalDetailsFinancial(customerID);
                DataTable dtAdditionalResidential   = cust.GetCustomerAdditionalDetailsResidential(customerID);
				
                ds.Tables.AddRange(new DataTable[] { dtCust, dtBank, dtEmployment, dtAccountTotals, dtAccounts, dtProposal, dtAgreement, dtAdditionalFinancial, dtAdditionalResidential,dtWork });
			}
			return ds;
		}

		/// <summary>
		/// This method retrieves the information required to populate the 
		/// Stage 2 sanction screen. Must draw data from the following 
		/// sources:
		///		Proposal 
		///		ProposalRef
		///		Employment
		///	This method will be called once for each applicant
		/// </summary>
		/// <param name="customerID"></param>
		/// <returns></returns>
		/// 
		public DataSet GetProposalStage2(string customerID, DateTime dtDateProp, string accountNo, string relation)
		{
			//This method can also be used for returning proposal data for 
			//a spouse or joint applicant

			string tmpCustID = customerID;
			customerID = FindSecondApplicant(customerID, accountNo, relation);

			DataSet ds = new DataSet();

			DCustomer cust = new DCustomer();
			cust.GetCustomerDetails(null, null,customerID);
			
			if(customerID == tmpCustID)
				cust.GetCustomerHomeAddress(customerID);
			else
				cust.GetCustomerWorkAddress(customerID);

			DataTable dtCust = cust.GetRow(TN.Customer);			

			DEmployment emp = new DEmployment();
			emp.GetEmployment(customerID);
			DataTable dtEmployment = emp.GetRow(TN.Employment);

			DProposal prop = new DProposal();
			prop.GetProposal(accountNo, customerID);
			DataTable dtProposal = prop.GetStage2Row(TN.Proposal);

			DReference refs = new DReference();
			refs.GetReferences(accountNo, TN.References);

			ds.Tables.AddRange(new DataTable[] { dtCust, dtEmployment, dtProposal, refs.referenceList });
			return ds;
		}

		/// <summary>
		/// This method retrieves the list of previous references for one Customer 
		/// </summary>
		/// <param name="custId"></param>
		/// <returns></returns>
		/// 
		public DataSet GetReferenceList(string custId)
		{
			DReference refs = new DReference();
			refs.GetReferenceList(custId, TN.References);

			DataSet ds = new DataSet();
			ds.Tables.AddRange(new DataTable[] {refs.referenceList});
			return ds;
		}


		public DataSet LoadProposalFlags(string accountNo, string custid, DateTime dateprop, out bool holdProp, out string currentStatus)
		{
			DataSet ds = new DataSet();
			DProposalFlag pf = new DProposalFlag();
			pf.Load(accountNo, custid, dateprop, out holdProp, out currentStatus);
			ds.Tables.Add(pf.ProposalFlags);
			return ds;
		}

		/*public DataSet LoadDAProposalFlags ()
		{
			DataSet ds= new DataSet();
			DProposalFlag DApf = new DProposalFlag();
			DApf.LoadDAProposalFlags();
			ds.Tables.Add(DApf.ProposalFlags);
			return ds;
		}*/

		/// <summary>
		/// This is a little function to log any data columns that contain
		/// DBNull. This is to try and track down invalid cast errors
		/// </summary>
		/// <param name="r"></param>
		private void LogDBNulls(DataRow r)
		{
			string obj = "";
			int i=0;
			foreach (object o in r.ItemArray)
			{
				if(o is DBNull)
				{
					string tmp = r.Table.Columns[i].ColumnName + " = " + o.GetType().Name;
					obj += tmp += Environment.NewLine;
				}
				i++;
			}
			if(obj.Length>0)
			{
				obj = r.Table.TableName + Environment.NewLine + obj;
				logMessage(obj, User.ToString(), EventLogEntryType.Warning);
			}
		}

        public void StoreCardQualified(SqlConnection conn, SqlTransaction trans, string customerId, int points, string scorecard, out bool? storeCardApproved, DateTime rundate  )
        {
            storeCardApproved  = false;
            //Blue.Cosacs.StoreCardApplyExtraRules storeRules = new Blue.Cosacs.StoreCardApplyExtraRules(conn, trans);
            //    storeRules.ExecuteNonQuery(customerId, points, scorecard , out storeCardApproved, rundate);
            
            if (storeCardApproved == null) // could use the ??? null coalescing operator but would be unclear.
                    storeCardApproved = false;
            
            
        }


		/// <summary>
		/// This method will save the proposal details entered by the user
		/// into the proposal table. The source tables for the data will 
		/// also be updated.
		/// </summary>
		/// <param name="conn">SqlConnection which hosts the transaction</param>
		/// <param name="trans">The database transaction for the updates</param>
		/// <param name="customerID">Main applicants customer ID</param>
		/// <param name="accountNo">Account number to be sanctioned (if there is one)</param>
		/// <param name="ds">DataSet containing all the data to be saved</param>
		/// <param name="sanction">Bool telling us whether to sanction the proposal or not</param>
		public void Save(SqlConnection conn, 
			SqlTransaction trans, 
			string customerID,
			string accountNo,
			DataSet app1,
			DataSet app2,
			bool sanction)
		{
			/* ===================================================================
			 * Two datasets are passed into the method. One for each applicant.
			 * Each contain fields for updating the live tables and the proposal
			 * table. 
			 *	1)	call GetCustomerDetails
			 *	2)	call GetCustomerHomeAddress
			 *	3)	overwrite the customer object members with 
			 *		values from the input data table
			 *	4)	call UpdateCustomerDetails
			 * 
			 *	5)	call GetBankDetails
			 *	6)	overwrite the bank object members with values
			 *		from the input data table
			 *	7)	call UpdateBankDetails
			 * 
			 *	8)	call GetEmploymentDetails
			 *	9)	overwrite the employment object members with values
			 *		from the input data table
			 *	10)	call UpdateEmploymentDetails
			 * 
			 *	11)	call GetProposal
			 *	12)	overwrite the proposal object members with values
			 *		from all the relevant datatables
			 *	13)	call UpdateProposal
			 *	14)	repeat 1)-13) for applicant 2 if not null
			 *======================================================================*/
			
			//1)Update the customer tables for applicant one
			DataRow drCust1 = null;
			DataRow drCust2 = null;
			DataRow drEmp1 = null;
			DataRow drEmp2 = null;
			DataRow drBank1 = null;
			DataRow drBank2 = null;
			DataRow drProp1 = null;
			DataRow drProp2 = null;
			DataRow drAgree = null;


			DCustomer cust1 = null;
			DCustomer cust2 = null;

			DBank bank1 = null;
			DBank bank2 = null;

			DEmployment emp1 = null;
			DEmployment emp2 = null;

			DProposal prop = null;

			DAgreement agree = null;

			//bool newProp; // = false;
		
			//Retrieve custoemr data
			drCust1 = app1.Tables[TN.Customer].Rows[0];
			cust1 = new DCustomer();
			cust1.GetCustomerDetails(conn, trans, customerID);
			cust1.GetCustomerHomeAddress(customerID);
			LogDBNulls(drCust1);

			if(app2!=null)
			{
				drCust2 = app2.Tables[TN.Customer].Rows[0];
				cust2 = new DCustomer();
				cust2.GetCustomerDetails(conn, trans, (string)drCust2[CN.CustomerID]);
				LogDBNulls(drCust2);
			}

			//retrieve bank data
			drBank1 = app1.Tables[TN.Bank].Rows[0];
			bank1 = new DBank();
			bank1.GetAccountDetails(customerID, accountNo);
			LogDBNulls(drBank1);

			if(app2!=null)
			{
				drBank2 = app2.Tables[TN.Bank].Rows[0];
				bank2 = new DBank();
				bank2.GetAccountDetails((string)drBank2[CN.CustomerID], accountNo);
				LogDBNulls(drBank2);
			}

			//Retrieve employment data
			drEmp1 = app1.Tables[TN.Employment].Rows[0];
			emp1 = new DEmployment();
			emp1.GetEmployment(customerID);
			LogDBNulls(drEmp1);

			if(app2!=null)
			{
				drEmp2 = app2.Tables[TN.Employment].Rows[0];
				emp2 = new DEmployment();
				emp2.GetEmployment((string)drEmp2[CN.CustomerID]);
				LogDBNulls(drEmp2);
			}

			//Retrieve proposal data
			drProp1 = app1.Tables[TN.Proposal].Rows[0];
			prop = new DProposal();
			prop.GetProposal(accountNo, customerID);
            //if(prop.DateProp == DateTime.MinValue.AddYears(1899))
            //    newProp=true;
			LogDBNulls(drProp1);

			//Update customer data
			cust1.CustID = (string)drCust1[CN.CustomerID];
			cust1.FirstName = (string)drCust1[CN.FirstName];
			cust1.Name = (string)drCust1[CN.LastName];
			cust1.Sex = (string)drCust1[CN.Sex];
			cust1.Ethnicity = (string)drCust1[CN.Ethnicity];
			cust1.MoreRewardsNo = (string)drCust1[CN.MoreRewardsNo];
			cust1.EffectiveDate = (DateTime)drCust1[CN.EffectiveDate];
			cust1.IDType = (string)drCust1[CN.IDType];
			cust1.DateBorn = (DateTime)drCust1[CN.DOB];
			cust1.DateIn = (DateTime)drCust1[CN.DateIn];
			cust1.ResidentialStatus = (string)drCust1[CN.ResidentialStatus];
			cust1.PropertyType = (string)drCust1[CN.PropertyType];
			cust1.PrevDateIn = (DateTime)drCust1[CN.PrevDateIn];
			cust1.PrevResidentialStatus = (string)drCust1[CN.PrevResidentialStatus];
			cust1.MonthlyRent = drCust1[CN.MonthlyRent];
            
            //Following added for CR 835
            cust1.MaritalStatus = (string)drProp1[CN.MaritalStatus];
            cust1.Dependants = (int)drProp1[CN.Dependants];
            cust1.Nationality = (string)drProp1[CN.Nationality];

            cust1.Save(conn, trans, customerID);
			cust1.SaveHomeAddress(conn, trans, customerID);

            //CR 835 (saving a proposal also saves the data in the additional residential table
            cust1.CustID = customerID;
            cust1.SaveCustomerAdditionalDetailsResidential(conn, trans);
			
            if(app2!=null)
			{
				cust2.CustID = (string)drCust2[CN.CustomerID];
				cust2.Title = (string)drCust2[CN.Title];
				cust2.FirstName = (string)drCust2[CN.FirstName];
				cust2.Name = (string)drCust2[CN.LastName];
				cust2.Sex = (string)drCust2[CN.Sex];
				cust2.MoreRewardsNo = (string)drCust2[CN.MoreRewardsNo];
				cust2.EffectiveDate = (DateTime)drCust2[CN.EffectiveDate];
				cust2.IDType = (string)drCust2[CN.IDType];
				cust2.DateBorn = (DateTime)drCust2[CN.DOB];
				cust2.Alias = (string)drCust2[CN.Alias];
				cust2.Save(conn, trans, (string)drCust2[CN.CustomerID]);
			}

			//update the bank data			
			bank1.CustomerID = customerID;
			bank1.BankAccountNo = (string)drBank1[CN.BankAccountNo];
			bank1.BankCode = (string)drBank1[CN.BankCode];
			bank1.DateOpened = (DateTime)drBank1[CN.BankAccountOpened];
			bank1.Code = (string)drBank1[CN.Code];
			bank1.IsMandate = (bool)drBank1[CN.IsMandate];
			bank1.DueDayId = (int)drBank1[CN.DueDayId];
			bank1.BankAccountName = (string)drBank1[CN.BankAccountName];
			bank1.Save(conn, trans, customerID, accountNo);

			if(app2!=null)
			{			
				bank2.CustomerID = (string)drBank2[CN.CustomerID];
				bank2.DateOpened = (DateTime)drBank2[CN.BankAccountOpened];
				bank2.Save(conn, trans, (string)drBank2[CN.CustomerID], accountNo);
			}

			//Update employment data
			emp1.CustomerID = customerID;
			emp1.DateEmployed = (DateTime)drEmp1[CN.DateEmployed];
            //CR 866 Changed CN.Occupation to CN.WorkType
			emp1.WorkType = (string)drEmp1[CN.WorkType];
			emp1.EmploymentStatus = (string)drEmp1[CN.EmploymentStatus];
			emp1.PayFrequency = (string)drEmp1[CN.PayFrequency];
			emp1.AnnualGross = drEmp1[CN.AnnualGross];
			emp1.PersDialCode = (string)drEmp1[CN.PersDialCode];
			emp1.PersTel = (string)drEmp1[CN.PersTel];
			emp1.PrevDateEmployed = (DateTime)drEmp1[CN.PrevDateEmployed];

            //CR 866 Adding additional fields
            emp1.JobTitle = drEmp1[CN.JobTitle].ToString();
            emp1.EducationLevel = drEmp1[CN.EducationLevel].ToString();
            emp1.Industry = drEmp1[CN.Industry].ToString();
            emp1.Organisation = drEmp1[CN.Organisation].ToString();
            //CR 866 End additional fields
            
            emp1.Save(conn, trans, customerID);

			if(app2!=null)
			{				
				emp2.CustomerID = (string)drEmp2[CN.CustomerID];
				emp2.DateEmployed = (DateTime)drEmp2[CN.DateEmployed];
				emp2.EmploymentStatus = (string)drEmp2[CN.EmploymentStatus];
				emp2.AnnualGross = drEmp2[CN.AnnualGross];
                //CR 866 Change CN.Occupation to CN.WorkType
                emp2.WorkType = (string)drEmp2[CN.WorkType];

				emp2.Save(conn, trans, (string)drEmp2[CN.CustomerID]);
			}

			//update the proposal details			
			prop.DateProp = (DateTime)drProp1[CN.DateProp];
            if (drProp1[CN.NewS1Comment].ToString().Trim().Length > 0)
            {
                // Get Employee name to audit comments
                DEmployee employee = new DEmployee();
                employee.GetEmployeeDetails(conn, trans, this.User);
                prop.S1Comment = employee.EmployeeName + " (" + this.User + ") - " + DateTime.Now + " :\n\n" + (string)drProp1[CN.NewS1Comment] + "\n\n" + (string)drProp1[CN.S1Comment];
            }
            else
                if (drProp1[CN.S1Comment] != DBNull.Value)              // #3484 jec 27/09/11
                {
                    prop.S1Comment = (string)drProp1[CN.S1Comment];
                }
                else prop.S1Comment = "";

			prop.MaritalStatus = (string)drProp1[CN.MaritalStatus];
            prop.IsSpouseWorking = Convert.ToBoolean(drProp1[CN.IsSpouseWorking]);
			prop.Dependants = (int)drProp1[CN.Dependants];
			prop.Nationality = (string)drProp1[CN.Nationality];
			prop.PreviousEmploymentMM = (int)drProp1[CN.PrevEmpMM];
			prop.PreviousEmploymentYY = (int)drProp1[CN.PrevEmpYY];
			prop.AdditionalIncome = drProp1[CN.AdditionalIncome];
			prop.OtherPayments = drProp1[CN.OtherPayments];
			prop.CreditCardNo1 = (string)drProp1[CN.CCardNo1];
			prop.CreditCardNo2 = (string)drProp1[CN.CCardNo2];
			prop.CreditCardNo3 = (string)drProp1[CN.CCardNo3];
			prop.CreditCardNo4 = (string)drProp1[CN.CCardNo4];
			prop.Commitments1 = drProp1[CN.Commitments1];
			prop.Commitments2 = drProp1[CN.Commitments2];
			prop.Commitments3 = drProp1[CN.Commitments3];
			prop.EmployeeNoChanged = this.User;
			if(emp1.AnnualGross==DBNull.Value)
				prop.MonthlyIncome = DBNull.Value;
			else
				prop.MonthlyIncome = Convert.ToDecimal(emp1.AnnualGross) / 12;
			prop.BankAccountType = bank1.Code;
			prop.AdditionalExpenditure1 = drProp1[CN.AdditionalExpenditure1];
			prop.AdditionalExpenditure2 = drProp1[CN.AdditionalExpenditure2];

            prop.PurchaseCashLoan = Convert.ToBoolean(drProp1["PurchaseCashLoan"]);

			//------DSR 22 Oct 2002 - Fixes for UAT J7 and J11

			DateTime DateToday = DateTime.Today;

			DateDiff EmpPeriod = new DateDiff(emp1.DateEmployed, DateToday);
			prop.YearsCurrentEmployment = EmpPeriod.DiffYY;

			DateDiff PrevEmpPeriod = new DateDiff(emp1.PrevDateEmployed, emp1.DateEmployed);
			prop.PreviousEmploymentYY = PrevEmpPeriod.DiffYY;
			prop.PreviousEmploymentMM = PrevEmpPeriod.DiffMM;

			DateDiff CurAddrsPeriod = new DateDiff(cust1.DateIn, DateToday);
			prop.YearsCurrentAddress = (short)CurAddrsPeriod.DiffYY;

			DateDiff PrevAddrsPeriod = new DateDiff(cust1.PrevDateIn, cust1.DateIn);
			prop.YearsPreviousAddress = (short)PrevAddrsPeriod.DiffYY;
			prop.PreviousAddressYY = PrevAddrsPeriod.DiffYY;
			prop.PreviousAddressMM = PrevAddrsPeriod.DiffMM;

			DateDiff BankAcctPeriod = new DateDiff(bank1.DateOpened, DateToday);
			prop.YearsBankAccountHeld = (short)BankAcctPeriod.DiffYY;

			//------DSR 22 Oct 2002 - End of fixes for UAT J7 and J11

			prop.RFCategory = (short)drProp1[CN.RFCategory];
			prop.PreviousResidentialStatus = (string)drCust1[CN.PrevResidentialStatus];

			/* jpj extra stuff to complete the snapshot */
			prop.EmploymentStatus = emp1.EmploymentStatus;
			prop.Occupation = emp1.WorkType;
			prop.PayFrequency = emp1.PayFrequency;
			prop.DateEmpStart = emp1.DateEmployed;
			prop.DatePEmpStart = emp1.PrevDateEmployed;
			prop.EmploymentTelNo = emp1.PersTel;
			prop.EmploymentDialCode = emp1.PersDialCode;
			prop.BankCode = bank1.BankCode;
			prop.BankAccountNo = bank1.BankAccountNo;
			prop.BankAccountOpened = bank1.DateOpened;

            //CR 866 Adding additional proposal fields
            prop.JobTitle = emp1.JobTitle;
            prop.EducationLevel = emp1.EducationLevel;
            prop.Industry = emp1.Industry;
            prop.Organisation = emp1.Organisation;
            prop.TransportType = drProp1[CN.TransportType].ToString();
            prop.DistanceFromStore = Convert.ToInt16(drProp1[CN.DistanceFromStore]);
            //End CR 866 

            //CR 835 Save additional financial details in the customer additional details table
            prop.DueDayId = bank1.DueDayId;
            prop.BankAccountName = bank1.BankAccountName;
            prop.PaymentMethod = (string)app1.Tables[TN.Agreements].Rows[0][CN.PaymentMethod];
            prop.CustomerID = customerID;
            prop.SaveCustomerAdditionalDetailsFinancial(conn, trans);

            prop.DateIn = (DateTime)drCust1[CN.DateIn];
            prop.ResidentialStatus = (string)drCust1[CN.ResidentialStatus];
            prop.PropertyType = (string)drCust1[CN.PropertyType];
            

			if(app2!=null)
			{
				//update the proposal table with applicat2 data
				drProp2 = app2.Tables[TN.Proposal].Rows[0];
				prop.A2AdditionalIncome = drProp2[CN.AdditionalIncome2];
				if(emp2.AnnualGross==DBNull.Value)
					prop.A2MonthlyIncome = DBNull.Value;
				else
					prop.A2MonthlyIncome = Convert.ToDecimal(emp2.AnnualGross) / 12;
				prop.A2Relation = (string)drProp2[CN.A2Relation];
				LogDBNulls(drProp2);
			}
			else
			{
				prop.A2Relation = "";
			}

            if (prop.Points == null)            // #3484 jec 27/09/11
                prop.Points = 0;

			prop.Save(conn, trans, customerID, accountNo);		

			drAgree = app1.Tables[TN.Agreements].Rows[0];
			agree = new DAgreement();
			agree.Populate(conn, trans, accountNo, 1);
            if (agree.AccountNumber == string.Empty)
            {
                agree.AccountNumber = accountNo;
                agree.AgreementNumber = 1;
                agree.AgreementTotal = 0;
                agree.AgreementDate = DateTime.Now;
                agree.CashPrice = 0;
                agree.Deposit = 0;
                agree.User = this.User;
            }
			agree.PaymentMethod = (string)drAgree[CN.PaymentMethod];
            agree.DateChange = DateTime.Now;                //#14392
            agree.EmployeeNumChange = this.User;            //#17343
			agree.Save(conn, trans);

	
			//write prop flag records if this is a new proposal
			/* it will never be a new prop because it will have 
			 * been created when the account was tied to the customer
			 * therefore this is pointless - JPJ
			if(newProp)
			{
				DProposalFlag propFlag = new DProposalFlag();
				propFlag.OrigBr = 0;
				propFlag.CustomerID = customerID;
				propFlag.DateProp = prop.DateProp;
				propFlag.EmployeeNoFlag = this.User;

				propFlag.CheckType = "S1";
				propFlag.Save(conn, trans);

				propFlag.CheckType = "S2";
				propFlag.Save(conn, trans);

				propFlag.CheckType = "DC";
				propFlag.Save(conn, trans);
                propFlag.AddDA(conn, trans);
			}
			*/
		}


		/// <summary>
		/// This method will save the proposal details entered by the user
		/// into the proposal table. The source tables for the data will 
		/// also be updated.
		/// </summary>
		/// <param name="conn">SqlConnection which hosts the transaction</param>
		/// <param name="trans">The database transaction for the updates</param>
		/// <param name="customerID">Main applicants customer ID</param>
		/// <param name="accountNo">Account number to be sanctioned (if there is one)</param>
		/// <param name="ds">DataSet containing all the data to be saved</param>
		public void SaveStage2(	SqlConnection conn, 
								SqlTransaction trans, 
								string customerID,
								string accountNo,
								DataSet app1,
								DataSet app2,
								bool complete)
		{
			/* ===================================================================
			 * Two datasets are passed into the method. One for each applicant.
			 * Each contain fields for updating the live tables and the proposal
			 * table. 
			 *  1)	call GetEmployment
			 *  2)	overwrite employment StaffNo member
			 *  3)  call UpdateEmployment
			 *	4)	call GetProposal
			 *	5)	overwrite the proposal object members with values
			 *		from all the relevant datatables
			 *	6)	call UpdateProposal
			 *  7)	delete from ProposalRef
			 *  8)  insert into ProposalRef
			 *	9)	repeat 1)-6) for applicant 2 if not null
			 *======================================================================*/
			
			//1)Update the customer tables for applicant one
			DataRow drEmp1 = null;
			DataRow drEmp2 = null;
			DataRow drProp = null;
			DataRow cust = null;

			DEmployment emp1 = null;
			DEmployment emp2 = null;

			DProposal prop = new DProposal();
			DProposal prop2 = new DProposal();

			DReference propRef = new DReference();
		
			//Retrieve employment data
			drEmp1 = app1.Tables[TN.Employment].Rows[0];
			emp1 = new DEmployment();
			emp1.GetEmployment(customerID);

			if(app2!=null)
			{
				drEmp2 = app2.Tables[TN.Employment].Rows[0];
				emp2 = new DEmployment();
				emp2.GetEmployment((string)drEmp2[CN.CustomerID]);
			}

			//Retrieve proposal data
			drProp = app1.Tables[TN.Proposal].Rows[0];
			prop.GetProposal(accountNo, customerID);

			//Update employment data
			emp1.StaffNo = (string)drEmp1[CN.StaffNo];
			emp1.Save(conn, trans, customerID);

			if(app2!=null)
			{				
				emp2.StaffNo = (string)drEmp2[CN.StaffNo];
				emp2.PersDialCode = (string)drEmp2[CN.PersDialCode];
				emp2.PersTel = (string)drEmp2[CN.PersTel];
				emp2.Department = (string)drEmp2[CN.Department];
				emp2.Save(conn, trans, (string)drEmp2[CN.CustomerID]);
			}

			//update the proposal details			
			prop.SpecialPromo		= (string)drProp[CN.SpecialPromo];
			prop.PreviousAddress1	= (string)drProp[CN.PAddress1];
			prop.PreviousAddress2	= (string)drProp[CN.PAddress2];
			prop.PreviousCity		= (string)drProp[CN.PCity];
			prop.PreviousPostCode	= (string)drProp[CN.PPostCode];
			prop.EmployerName		= (string)drProp[CN.EmployeeName];
			prop.EmployerDept		= (string)drProp[CN.EmpDept];
			prop.EmployerAddress1	= (string)drProp[CN.EAddress1];
			prop.EmployerAddress2	= (string)drProp[CN.EAddress2];
			prop.EmployerCity		= (string)drProp[CN.ECity];
			prop.EmployerPostCode	= (string)drProp[CN.EPostCode];
			prop.NoOfReferences		= (int)drProp[CN.NoOfRef];
			prop.VehicleRegistration= (string)drProp[CN.VehicleRegistration];

			if (drProp[CN.NewComment].ToString().Trim().Length > 0)
			{
				// Get Employee name to audit comments
				DEmployee employee = new DEmployee();
				employee.GetEmployeeDetails(conn, trans, this.User);
				prop.S1Comment = employee.EmployeeName + " (" + this.User + ") - " + DateTime.Now + " :\n\n" + (string)drProp[CN.NewComment] + "\n\n" + (string)drProp[CN.S1Comment];
			}
			else
				prop.S1Comment = (string)drProp[CN.S1Comment];

			prop.Save(conn, trans, customerID, accountNo);

			if(app2!=null)
			{
				cust = app2.Tables[TN.Customer].Rows[0];
				SaveAddresses(conn, trans, (string)cust[CN.CustomerID], cust);
			}

			// delete the current set of references
			propRef.ClearReferences(conn, trans, accountNo);
			// save the new set of references
			propRef.referenceList = app1.Tables[TN.References];
			propRef.User = this.User;
			propRef.SaveReferences(conn, trans, accountNo);

			if (complete)
			{
				//Set stage 2 to complete
				DProposalFlag propFlag = new DProposalFlag();
				propFlag.OrigBr = 0;
				propFlag.CustomerID = customerID;
				propFlag.DateProp = prop.DateProp;
				propFlag.DateCleared = DateTime.Now;
				propFlag.EmployeeNoFlag = this.User;
				propFlag.CheckType = "S2";
				propFlag.Save(conn, trans, accountNo);
			}
	
		}	// End of SaveStage2


		public decimal DummySanction(SqlConnection conn, SqlTransaction trans, 
			string accountNo, string customerID, DateTime timeStamp)
		{
			//Write dummy proposal record
			DProposal prop = new DProposal();
			prop.DateProp = timeStamp;
			prop.PropResult = "A";
			prop.AppStatus = "A";
			prop.Save(conn, trans, customerID, accountNo);

			//write prop flag records
			DProposalFlag propFlag = new DProposalFlag();
			propFlag.OrigBr = 0;
			propFlag.CustomerID = customerID;
			propFlag.DateProp = timeStamp;
			propFlag.DateCleared = timeStamp;
			propFlag.EmployeeNoFlag = 99999;

			propFlag.CheckType = "S1";
			propFlag.Save(conn, trans, accountNo);

			propFlag.CheckType = "S2";
			propFlag.Save(conn, trans, accountNo);

			propFlag.CheckType = "DC";
			propFlag.Save(conn, trans, accountNo);

			//write prop result record
			DProposalResult propResult = new DProposalResult();
			propResult.Save(conn, trans, accountNo);

			//Update the customer record
			DCustomer cust = new DCustomer();
			decimal limit = 5000;
			cust.SetCreditLimit(conn, trans, customerID, limit, "A");

			return limit;
		}

		public void Score(SqlConnection conn,
			SqlTransaction trans, 
			string country, 
			string accountNo, 
			string accountType, 
			string customerID, 
			DateTime dateProp,
			short branchNo,
            out string newBand,
			out string refCode, 
			out decimal score,
			out decimal RFLimit,
			int user,
			out string result, 
            out string bureauFailure,
            ref bool referDeclined,
            out string referralReasons) //IP - 14/03/11 - #3314 - CR1245 
		{	

			branchNo = Convert.ToInt16(accountNo.Substring(0,3));
			DBranch b = new DBranch();
			b.Populate(conn, trans, branchNo);
			DScoring s = new DScoring();
			s.Country = country;
			s.Region = b.Region;
            DProposal DProp = new DProposal();
            char scorecard; char scorecardtouse;
            referralReasons = string.Empty; //IP - 14/03/11 - #3314 - CR1245

            DProp.ScoreTypetoUse(conn,trans,accountNo,customerID,out scorecard);
            if (scorecard == 'P' || scorecard == 'E') // Parallel run  //add equifax condition
            {
                if(scorecard == 'P')
                { 
                scorecardtouse = 'A';
                s.scoretype = 'A'; // do applicant first time
                }
                else //add equifax condition
                {
                    scorecardtouse = 'C';
                    s.scoretype = 'C';
                }
            }
            else
            {// Can be Behavioural or Applicant
                scorecardtouse = scorecard;
                s.scoretype = scorecard;
                //s.scoretype = 'B';
            }
                s.scoretype = scorecardtouse;
                s.GetRules();
                Score(conn, trans, country, accountNo, accountType,
                    customerID, dateProp, s.RulesTable, b.Region, out newBand, out refCode, out score, out RFLimit,
                    user, out result, out bureauFailure, ref referDeclined, scorecardtouse, s.scoretype, out referralReasons); //IP - 14/03/11 - #3314 - CR1245 - returning referral reasons
            if (scorecard == 'P' || scorecard == 'E') //add equifax score condition
            {
                string pband = ""; string prefcode = ""; decimal pscore = 0;
                string presult = ""; bool rdeclined = false; decimal prflimit = 0;
                char parallelScoreUsed = scorecard;  ////add equifax score condition
                if (scorecard == 'P')
                { 
                s.scoretype = 'B'; // now for parallel run do behavioural
                scorecard = 'B'; // now for parallel run do behavioural
                }
                else //add equifax score condition
                {
                    s.scoretype = 'D'; 
                    scorecard = 'D'; 
                }
                scorecardtouse = scorecard;
                s.GetRules();
                Score(conn, trans, country, accountNo, accountType,
                    customerID, dateProp, s.RulesTable, b.Region, out pband, out prefcode, out pscore, out prflimit,
                    user, out presult, out bureauFailure, ref rdeclined, scorecardtouse, parallelScoreUsed, out referralReasons); // but pass in Parallel option //IP - 14/03/11 - #3314 - CR1245 - returning referral reasons
                // don't use result so removing outs...
            }

            AccountRepository ar = new AccountRepository();

            //IP - 24/02/11 - #2807 - CR1090 - Qualification for Waiver - if the score > minimum score for qualification
            var instalWaiverMinScore = Country.GetCountryParameterValue<int>(CountryParameterNames.InstalWaiverMinScore);

            if (instalWaiverMinScore > 0 && score >= instalWaiverMinScore)
                ar.UpdateFirstInstalmentWaiver(conn, trans, accountNo);
            
            // check for instant credit if HP account
            ar.InstantCreditDACheck(accountNo, user, conn, trans); //IP - 03/03/11 - #3255 - Added user
           
		}

		// rdb - creating a collection to hold scores
		// and parameter to get around this very dodgy recursive call
		private ArrayList _scoreList = new ArrayList();
		bool BayCorpFail = false;
		bool DPGroupFail = false;
		bool ScoreCalledRecursively = false;
        /// <summary>
        /// Scores the account either Scorecard A for applicant , R for rescore, B for behavioural, P for Parallel
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="country"></param>
        /// <param name="accountNo"></param>
        /// <param name="accountType"></param>
        /// <param name="customerID"></param>
        /// <param name="dateProp"></param>
        /// <param name="rules"></param>
        /// <param name="branchRegion"></param>
        /// <param name="newBand"></param>
        /// <param name="refCode"></param>
        /// <param name="score"></param>
        /// <param name="RFLimit"></param>
        /// <param name="user"></param>
        /// <param name="result"></param>
        /// <param name="bureauFailure"></param>
        /// <param name="referDeclined"></param>
        /// <param name="scorecard"></param>
        private void Score(SqlConnection conn,
                            SqlTransaction trans,
                            string country,
                            string accountNo,
                            string accountType,
                            string customerID,
                            DateTime dateProp,
                            DataTable rules,
                            string branchRegion,
                            out string newBand,
                            out string refCode,
                            out decimal score,
                            out decimal RFLimit,
                            int user,
                            out string result,
                            out string bureauFailure,
                            ref bool referDeclined,
                            char scorecard,// A for applicant , B for behavioural
                            char scoretype, //P for parallel run R for behavioural rescore - if either we don't save details to main table. 
                            out string referralReasons
                            )
        {
            referDeclined = false;
            bool referReject = false;
            bool nextReferReject = false;
            bool missingNode = false;
            BCreditBureau creditBureau = new BCreditBureau();
            DReferral dReferral = new DReferral();
            result = null;
            newBand = "";
            referralReasons = string.Empty;

            //Retrieve all the data for this customer account combination 
            refCode = "";
            score = 0;
            decimal Equifax_score = 0;
            RFLimit = 0;
            bureauFailure = "";
            DataRow r = null;
            const string spendLimitRefCode = "SL";

            //update the datelastscored field for the customer
            DCustomer cust = new DCustomer();
            cust.Cache = this.Cache;  // 68359 IP/RD 05/07/06
            cust.CustID = customerID;
            // Date last scored is written to database 
            cust.SetDateLastScored(conn, trans, DateTime.Now);

            DateTime dateScored = DateTime.Now;

            DScoring s = new DScoring();
            s.AccountNumber = accountNo;
            s.CustomerID = customerID;
            s.Country = country;
            DataTable dt = s.GetScoreDetails(conn, trans);
            DataSet dsSC = s.GetScoreDetailsForScoreCard(conn, trans);

            // DataTable SC = new DataTable();
            foreach (DataRow row in dt.Rows)
                r = row;

            if (r == null)
            {
                throw new STLException(GetResource("M_NOSCORINGINFO", new object[] { accountNo }));
            }

            XmlDocument doc = new XmlDocument();
            foreach (DataRow row in rules.Rows)
            {
                doc.LoadXml((string)row[CN.RulesXML]);
            }

            try
            {
                ScoreLimits.DeclineScore = Convert.ToInt16(doc.DocumentElement.Attributes[Tags.DeclineScore].Value);

                ScoreLimits.ReferScore = Convert.ToInt16(doc.DocumentElement.Attributes[Tags.ReferScore].Value);


                if (Convert.ToString(scoretype) == "A" || Convert.ToString(scoretype) == "B" || Convert.ToString(scoretype) == "P")
                {
                    score += Convert.ToInt16(doc.DocumentElement.Attributes[Tags.InterceptScore].Value);  //IP - 05/10/12 - #11405 - CR11404
                }

            }
            catch (NullReferenceException)
            {
                /* we have no scoring limits set for this country,
                 * therefore if this is not an RF account we cannot
                 * proceed */
                if (accountType != AT.ReadyFinance)
                    throw new STLException(GetResource("M_NOSCORINGLIMITS"));
                //throw new Exception("No scoring limits set, unable to evaluate score.");				
            }

            if (ContactBayCorp || ContactDPGroup)	/* baycorp enabled */
            {
                try
                {
                    ScoreLimits.BureauMinimum = Convert.ToInt16(doc.DocumentElement.Attributes[Tags.BureauMinimum].Value);
                    ScoreLimits.BureauMaximum = Convert.ToInt16(doc.DocumentElement.Attributes[Tags.BureauMaximum].Value);
                }
                catch (NullReferenceException)
                {
                    /* we have no bureau limits set for this country,
                     * therefore we cannot determine whether to contact the
                     * credit bureau or not */
                    throw new STLException(GetResource("M_NOBUREAULIMITS"));
                }
            }

            /* process the scoring rules first so that we know the score and the RF Limit */
            XmlNodeList scoringRules = doc.SelectNodes("//Rule[@Type = 'S']");

            StringBuilder sbOperandName = new StringBuilder();
            StringBuilder sbOperandValue = new StringBuilder();
            StringBuilder sbPoints = new StringBuilder();
            var Parms = new Parameters();
            Parms.AccountNo = accountNo;
            Parms.conn = conn;
            Parms.CustomerId = customerID;
            Parms.trans = trans; Parms.ruletype = "Points";
            string acctType = (string)r["account type"];
            //below are equifax code
            if (Convert.ToBoolean(Country[CountryParameterNames.IsOldScoreRunWithEquifax]) || (Convert.ToString(scoretype) == "C" || Convert.ToString(scoretype) == "D"))
            {
                EquifaxRule objEquifax = new EquifaxRule();
                decimal equifaxintercept = doc.DocumentElement.Attributes[Tags.InterceptScore].Value == null ? 0 : Convert.ToDecimal(doc.DocumentElement.Attributes[Tags.InterceptScore].Value);
                score = objEquifax.EquifaxgetScore(customerID, accountNo, acctType, equifaxintercept, dsSC, scoretype, country, branchRegion, scoringRules);
            }

            if (Convert.ToString(scoretype) == "A" || Convert.ToString(scoretype) == "B" || Convert.ToString(scoretype) == "P")//OLD Score card
            {
                foreach (XmlNode rule in scoringRules)
                {
                    if (((acctType == AT.ReadyFinance || acctType == AT.StoreCard) && rule.Attributes[Tags.ApplyRF].Value == Boolean.TrueString) ||
                        (acctType != AT.Cash && acctType != AT.Special && acctType != AT.ReadyFinance && acctType != AT.StoreCard && rule.Attributes[Tags.ApplyHP].Value == Boolean.TrueString)) //Acct Type Translation DSR 29/9/03
                    {
                        EvaluateRule(r, rule, Parms);
                        if (rule.Attributes[Tags.State].Value == Boolean.TrueString)
                        {
                            score += Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                            Points = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                        }
                        else
                            Points = 0;

                        if (Points != 0)
                        {
                            if (OperandName.Contains("|"))
                            {
                                OperandName = OperandName.Replace("|", " ");
                            }
                            sbOperandName.Append(OperandName);
                            sbOperandName.Append("|");

                            if (OperandValue.Contains("|"))
                            {
                                OperandValue = OperandValue.Replace("|", " ");
                            }
                            sbOperandValue.Append(OperandValue);
                            sbOperandValue.Append("|");

                            sbPoints.Append(Points.ToString());
                            sbPoints.Append("|");
                        }
                    }
                }
            }


            string oldband = "";
            decimal oldlimit = 0;
            if ((bool)Country[CountryParameterNames.LoggingEnabled] || (scorecard == 'P' || scorecard == 'E'))
            {
                string operandName = sbOperandName.ToString();
                string operandValue = sbOperandValue.ToString();
                string points = sbPoints.ToString();
                
                    //save score details to the Scoring Details table to enable
                    //credit staff to see what values are being scored against
                    s.SaveScoreDetails(conn, trans, customerID, accountNo,
                       dateScored, operandName, operandValue, points);              

            }

            /* convert the points tally into a credit limit if RF */
            if (accountType == AT.ReadyFinance || accountType == AT.StoreCard)
            {
                short branchNo = Convert.ToInt16(accountNo.Substring(0, 3));
                DBranch b = new DBranch();
                if (branchRegion == null)
                {
                    b.Populate(conn, trans, branchNo);
                    s.Region = b.Region;
                }
                else
                {
                    s.Region = branchRegion;
                }
                s.Country = country;
                s.CustomerID = customerID;
                s.DateProp = dateProp;
                s.Score = score;
                s.scoretype = scorecard;
                s.GetRFCreditLimit(conn, trans);

                cust.GetBasicCustomerDetails(conn, trans, customerID, accountNo, "H");
                oldband = cust.scoringBand;
                oldlimit = cust.RFLimit;
                // DSR 28/9/04 OldRFCreditLimit is not the manual override value.
                // Always use the value returned which already includes any override.
                //if(cust.OldRFCreditLimit!=0)	/* if there has been an override in the past */
                //	RFLimit = cust.OldRFCreditLimit;
                //else

                if (s.CreditLimit == 0 && score > 0 && (bool)Country[CountryParameterNames.SetMinRFLimit])
                    s.GetMinimumRFCreditLimit(conn, trans);

                RFLimit = s.CreditLimit;
                bool parrallelscore = false;
                if ((string)Country[CountryParameterNames.BehaviouralScorecard] == "P" || (string)Country[CountryParameterNames.BehaviouralScorecard] == "E")  //add equifax condition
                {
                    parrallelscore = true;
                }
                if (!parrallelscore && (scoretype != 'R' || (bool)Country[CountryParameterNames.BehaveApplyEodImmediate]))
                // don't save details to normal tables if parallel run or Rescore and not applying immediately
                {
                    cust.SetCreditLimit(conn, trans, customerID, RFLimit, Convert.ToString(scorecard));
                    //cust.SetCreditLimit(conn, trans, customerID, RFLimit, "A");

                    BCustomer bCust = new BCustomer();
                    bCust.Cache = this.Cache;

                    bCust.SetAvailableSpend(conn, trans, customerID);
                    if (cust.StoreCardApproved)
                        cust.CustomerUpdateStoreCardLimit(conn, trans, customerID);
                    //hhjj
                }
            }

            /* add the RF limit to the list of customer operands in case there
             * are any referral rules which operate on it */
            r["RF Spending Limit"] = RFLimit;
            r["score"] = score;

            Parms.ruletype = "Refer";

            /* delete any previous referral rules that may have been stored from 
             * previous scorings of this proposal */
            dReferral.DeleteReferralRules(conn, trans, customerID, dateProp);

            /* next process the referral rules
             * AA changing this so that it is storing referral reasons
             * against the proposal table rather than referral rules */
            int referralCounter = 0;
            DProposal prop = new DProposal();
            // clear the reason code at the start of the scoring process.
            prop.Reason = "";
            prop.Points = Convert.ToInt16(score); /* 68596 */
            bool? storeCardApproved = cust.StoreCardApproved;

            if (accountType == AT.StoreCard || (accountType == AT.ReadyFinance && (bool)(Country[CountryParameterNames.StoreCardEnabled])))
            {//apply extra rules to check if customer qualifies for storecard
             //trans.Commit(); // XXHH todo remove when transaction issue fixed
             //if (accountNo == "700010641061" || customerID == "AN111152")
             //{
             //    var storeRules = new Blue.Cosacs.StoreCardQualify(conn, trans)
             //    {
             //        CommandTimeout = 0 //timeout is infinity
             //    }; 
             //    storeRules.ExecuteDataSet(customerID, (int)prop.Points, scorecard.ToString(), out storeCardApproved, DateTime.Now);

                //    if (storeCardApproved == null) // could use the ??? null coalescing operator but would be unclear.
                //        storeCardApproved = false;
                //}
                //storeCardApproved = true; 
                //  conn.Open();
                //trans=conn.BeginTransaction(); //todo remove ...

            }

            if ((RFLimit < Convert.ToDecimal(Country[CountryParameterNames.MinStoreCardLimit])
                || storeCardApproved == false) && accountType == AT.StoreCard)
            {

                referReject = true;
                prop.Reason = "PT";
                PropReason = "PT";
                RejectReasons += "PT";
                referralCounter++;
            }
            else
            {
                PropReason = "";
            }

            if ((RFLimit < Convert.ToDecimal(Country[CountryParameterNames.MinStoreCardLimit]) && accountType == AT.StoreCard) //#18071
                    || (storeCardApproved == false && accountType == AT.StoreCard))
            {

                referReject = true;
                prop.Reason = "PT";
                PropReason = "PT";
                RejectReasons += "PT";
                referralCounter++;
            }
            else
            {
                PropReason = "";

            }



            /* setting the reason code only */
            if (accountType == AT.ReadyFinance)
            {
                if (RFLimit == 0)
                {
                    referReject = true;
                    PropReason = "PT";
                    RejectReasons += "PT";
                    referralCounter++;
                }
                else
                {
                    PropReason = "";
                }
            }
            else
            if (accountType == AT.HP)    /* for normal HP accounts */
            {
                if (prop.Points < ScoreLimits.DeclineScore || prop.Points < ScoreLimits.ReferScore)
                {
                    PropReason = "PT";
                    RejectReasons += " PT";
                    referralCounter++;
                }
                else
                {
                    PropReason = "";
                }
            }
            int ReferReferralCounter = 0;
            XmlNodeList refRules = doc.SelectNodes("//Rule[@Type = 'R']");
            foreach (XmlNode rule in refRules)
            {
                if (((string)r["account type"] == "R" && rule.Attributes[Tags.ApplyRF].Value == Boolean.TrueString) ||
                    ((string)r["account type"] != AT.Cash && (string)r["account type"] != AT.Special && (string)r["account type"] != AT.ReadyFinance && rule.Attributes[Tags.ApplyHP].Value == Boolean.TrueString)) //Acct Type Translation DSR 29/9/03
                {
                    EvaluateRule(r, rule, Parms);

                    if (rule.Attributes[Tags.State].Value == Boolean.TrueString)
                    {

                        if (!Convert.ToBoolean(rule.Attributes[Tags.ReferDeclined].Value))
                            ReferReferralCounter++; // its a referral rule rather than a refer decline rule
                        /* save the rule to the ReferralRules table */
                        referralCounter++;
                        switch (referralCounter)
                        {
                            case 1:
                                PropReason = rule.Attributes[Tags.Result].Value;
                                break;

                            case 2:
                                prop.Reason2 = rule.Attributes[Tags.Result].Value;
                                break;

                            case 3:
                                prop.Reason3 = rule.Attributes[Tags.Result].Value;
                                break;

                            case 4:
                                prop.Reason4 = rule.Attributes[Tags.Result].Value;
                                break;
                            case 5:
                                prop.Reason5 = rule.Attributes[Tags.Result].Value;
                                break;
                            case 6:
                                prop.Reason6 = rule.Attributes[Tags.Result].Value;
                                break;
                            default:
                                break;
                        }


                        //dReferral.WriteReferralRule(conn, trans, customerID, dateProp, rule.Attributes[Tags.Result].Value);

                        if (!referDeclined)
                        {
                            try
                            {
                                referDeclined = Convert.ToBoolean(rule.Attributes[Tags.ReferDeclined].Value);
                            }
                            catch (NullReferenceException) { }
                        }

                        try
                        {
                            missingNode = false;
                            nextReferReject = Convert.ToBoolean(rule.Attributes[Tags.RuleRejects].Value);
                        }
                        catch (NullReferenceException) { missingNode = true; }

                        refCode = refCode.Length == 0 ? rule.Attributes[Tags.Result].Value : "XX";

                        if (!missingNode && nextReferReject)
                        {
                            RejectReasons += " " + rule.Attributes[Tags.Result].Value;
                            referReject = true;
                        }
                        else
                            ReferReasons += " " + rule.Attributes[Tags.Result].Value;
                    }
                }
            }

            //Always refer account if customer has a negative 
            //available spend limit
            if (accountType == AT.ReadyFinance || accountType == AT.StoreCard)
            {
                cust.GetRFLimit(conn, trans, customerID, "");
                if (cust.RFAvailable < 0)
                {
                    referralCounter++;
                    switch (referralCounter)
                    {
                        case 1:
                            PropReason = spendLimitRefCode;
                            break;
                        case 2:
                            prop.Reason2 = spendLimitRefCode;
                            break;
                        case 3:
                            prop.Reason3 = spendLimitRefCode;
                            break;
                        case 4:
                            prop.Reason4 = spendLimitRefCode;
                            break;
                        case 5:
                            prop.Reason5 = spendLimitRefCode;
                            break;
                        case 6:
                            prop.Reason6 = spendLimitRefCode;
                            break;
                        default:
                            break;
                    }

                    refCode = spendLimitRefCode;
                    ReferReasons += " " + spendLimitRefCode;
                }
            }

            //Set stage 1 to complete and write the result to the proposal table
            DProposalFlag propFlag = new DProposalFlag();
            propFlag.OrigBr = 0;
            propFlag.CustomerID = customerID;
            propFlag.DateProp = dateProp;
            propFlag.DateCleared = DateTime.Now;
            propFlag.EmployeeNoFlag = this.User;
            propFlag.CheckType = "S1";
            propFlag.Save(conn, trans, accountNo);


            prop.DateProp = dateProp;
            prop.CustomerID = customerID;
            prop.AccountNo = accountNo;
            // rdb 28/09/2007 get highest score

            prop.Points = Convert.ToInt16(score);
            //prop.Reason = refCode;			
            // Now just setting the result - the reason should already be populate
            if (accountType == AT.ReadyFinance)
            {
                if (RFLimit == 0 || referReject)
                {
                    if (!referDeclined) //fail unless refer declined rule kicks in. 
                    {     //changed made by dipti on 23/08/18
                        if (scorecard == 'B' || scorecard == 'D') //
                        {
                            RFLimit = 0;
                        }
                        PropResult = "X";
                    }
                    else
                        PropResult = "R";
                    //prop.Reason = refCode.Length==0 ? "PT":"XX";
                    //IP - 21/07/09 - UAT(697) - commented out below line as was incorrectly adding PT to reasons when rejected on another rule.
                    //should be ok for when RFLimit == 0 as this is set to 'PT' further up.
                    //RejectReasons += " PT";   
                }
                else
                {
                    if (refCode != "" && ReferReferralCounter > 0)
                        PropResult = "R";
                    else // make sure no propreason added to database
                    {
                        PropReason = "";
                        PropResult = "A";
                        refCode = ""; // 71019 SC Get rid of decline refer reason if not referred.
                        ReferReasons = "";
                    }
                }
            }
            else	/* for normal HP accounts */
            {
                if (prop.Points < ScoreLimits.DeclineScore)
                {
                    if (referDeclined && !referReject)
                    {
                        PropResult = "R";
                    }
                    else
                    {
                        if (scorecard == 'B' || scorecard == 'D') //
                        {
                            RFLimit = 0;
                        }
                        PropResult = "X";
                        //	prop.Reason = refCode.Length==0?"PT":"XX";
                        //	rejectReasons += " PT";
                    }
                }
                else
                {
                    if (prop.Points < ScoreLimits.ReferScore)
                    {
                        if (referReject)
                        {
                            PropResult = "X";
                            PropReason = refCode;
                        }
                        else
                        {
                            PropResult = "R";
                            //		prop.Reason = refCode.Length==0?"PT":"XX";
                            //		referReasons += " PT";
                        }
                    }
                    else /* accepted on points, may still be referred on
						  * referral rules */
                    {
                        if (referReject)
                        {
                            if (scorecard == 'B' || scorecard == 'D') //
                            {
                                RFLimit = 0;
                            }
                            PropResult = "X";
                            //		prop.Reason = refCode;
                        }
                        else
                        {
                            if (ReferReferralCounter > 0) //account was referred as opposed to hitting a refer reject rule
                                PropResult = "R";
                            else
                                PropResult = "A";
                            //= refCode.Length == 0 ? "A" : "R";
                        }
                    }
                }
            }

            /* all normal scoring is done now - this is where we check against bureau
             * limits to see if we need to contact the bureau.  */
            #region SingBureau
            if (ContactBayCorp || ContactDPGroup)
            {
                // new error handling with forced timeouts


                if (prop.Points > ScoreLimits.BureauMinimum &&
                    prop.Points < ScoreLimits.BureauMaximum)
                {
                    // original code always uses DPGroup, try BayCorp also
                    DateTime lastRequestBay = creditBureau.GetLastRequest(conn, trans, customerID, STL.Common.Constants.CreditBureau.CreditBureau.Baycorp);
                    DateTime lastRequestDP = creditBureau.GetLastRequest(conn, trans, customerID, STL.Common.Constants.CreditBureau.CreditBureau.DPGroup);

                    bool newBayRequired = ContactBayCorp
                        && ((TimeSpan)(DateTime.Now - lastRequestBay)).Days > (decimal)Country[CountryParameterNames.CreditScanInterval]
                        && !BayCorpFail;
                    bool newDPRequired = ContactDPGroup
                        && ((TimeSpan)(DateTime.Now - lastRequestDP)).Days > (decimal)Country[CountryParameterNames.CreditScanInterval]
                        && !DPGroupFail;

                    if (newBayRequired || newDPRequired)
                    //if( ((TimeSpan)(DateTime.Now - creditBureau.GetLastRequest(conn, trans, customerID,STL.Common.Constants.CreditBureau.CreditBureau.DPGroup))).Days > (decimal)Country[CountryParameterNames.CreditScanInterval] )
                    {

                        //CR 843 Call the second credit bureau
                        //Exception error = null;

                        if (newDPRequired)
                        {
                            // todo add to config
                            int numTries = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bureau2NumTrys"]);
                            while (numTries > 0)
                            {
                                try
                                {
                                    CreditBureau.CreditBureauMessenger2 creditBureau2 = new CreditBureau.CreditBureauMessenger2();
                                    creditBureau2.ConsumerEnquiry(conn, trans, accountNo, customerID);
                                    DPGroupFail = false;
                                    numTries = 0;
                                }
                                catch (Exception ex)
                                {
                                    DPGroupFail = true;
                                    numTries--;
                                    /* we don't want to rollback in this case, just report the error */
                                    bureauFailure = ex.Message;
                                    logMessage(ex.Message + Environment.NewLine + ex.StackTrace, User.ToString(), EventLogEntryType.Error);
                                }
                            }
                        }
                        //END CR 843 
                        if (newBayRequired)
                        {
                            // todo add to config
                            int numTries = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bureau1NumTrys"]);
                            while (numTries > 0)
                            {
                                try
                                {
                                    /* contact the credit bureau */
                                    creditBureau.ConsumerEnquiry(conn, trans, accountNo, customerID);
                                    BayCorpFail = false;
                                    numTries = 0;
                                }
                                catch (CreditBureauException ex)
                                {
                                    BayCorpFail = true;
                                    numTries--;
                                    /* we don't want to rollback in this case, just report the error */
                                    bureauFailure = ex.Message;
                                    logMessage(ex.Message + Environment.NewLine + ex.StackTrace, User.ToString(), EventLogEntryType.Error);
                                }
                            }
                        }

                        //CR 843 throw an error
                        //if (error != null)
                        //	throw new CreditBureauException("The following error occurred connecting to DPGroup: " + error.Message, error);
                        //End CR 843

                        ///recurse - this will never loop because the last scored date
                        ///will be updated                        

                        // rdb -  ignore the claims of the developer above that this will never loop,                    
                        // if a call to a credit bureau fails because the connection is down
                        // it will loop continously until the Cosacs web service times out
                        // what prevents it looping in this case is setting dpgroupfail and baycorpfail


                        PropReason = "";
                        PropResult = "";
                        ReferReasons = "";
                        RejectReasons = "";
                        // rdb 28/09/07 this is not a suitable way to proceed
                        // as when we reach this code: prop.WriteResult(conn, trans);
                        // we correctly save our new score from the bureau from this call to Score
                        // however when this original call exist it then saved the non bureau Score over the top of it
                        // instead lets factor out the code above that rescores the account and call that here
                        // to generate the updated score
                        // we will then use which ever score is worse


                        ScoreCalledRecursively = true;
                        Score(conn, trans, country, accountNo,
                            accountType, customerID,
                            dateProp, rules, branchRegion, out newBand, out refCode,
                            out score, out RFLimit, user, out result, out bureauFailure, ref referDeclined, 'A', scoretype, out referralReasons); //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons
                        ScoreCalledRecursively = false;
                    }

                }
            }
            #endregion SingBureau
            //update account status unless for behavioural rescore or for Parallel run. 
            if ((scoretype != 'P'|| scoretype != 'E') && scoretype != 'R') //Add eqifax condition
            {
                DAccount acct = new DAccount();
                acct.UpdateStatus(conn, trans, accountNo);
            }
            if (ReferReasons.Length > 0)
                ReferReasons = ReferReasons.Insert(0, "Referral: ");
            prop.ProposalNotes = DateTime.Now.ToString() + " Scored : " + Convert.ToString(scorecard) +
                ReferReasons + " " + RejectReasons /*+ " New Band:" + newBand*/ + Environment.NewLine + Environment.NewLine;

            if (RejectReasons.Length > 0)
                RejectReasons = RejectReasons.Insert(0, "Rejection: ");

            prop.Reason = PropReason;
            prop.PropResult = PropResult;

            // rdb if both failed dont add
            if (!(ScoreCalledRecursively && BayCorpFail && DPGroupFail))
            {
                _scoreList.Add(prop);

            }

            string AccountBand;
            this.DetermineBands(conn, trans, accountNo, customerID, out AccountBand, out newBand, scorecard.ToString(), (int)prop.Points);


            if ((scoretype == 'P' && scorecard == 'B')|| (scoretype == 'E' && scorecard == 'D')) //if Parallell and Behavioural   //Add equifax condition
            {
                prop.WriteResultforBS(conn, trans, newBand, scoretype, RFLimit, oldband, oldlimit);
            }


            //  we do not neccesarily want to save the worst, if we have one from burea then use that
            // a bit of a workaround again but we know there will only be one item in the collection
            // if we are calling the bureau, or if there is no call to the bureau
            // so only save if count is one
            if (_scoreList.Count == 1)
            {
                //IP - CR916- 10/04/08 - Set the user for DProposal
                //which was not being set previously, which is needed
                //to write to the 'ProposalAudit' table.

                prop.EmployeeNoChanged = this.User;

                if (scoretype == 'R' && !String.IsNullOrEmpty(newBand)) // if ReScore write anyway so can come into 
                {
                    prop.WriteResultforBS(conn, trans, newBand, scoretype, RFLimit, oldband, oldlimit);
                }

                if ((scoretype != 'P'|| scoretype != 'E') && (scoretype != 'R' || (bool)Country[CountryParameterNames.BehaveApplyEodImmediate]))// expect to be removing the 'R' clause based on user feedback.  //add equifax condition
                {
                    if (PropResult != "X")
                    {
                        DCustomer Dcust = new DCustomer();
                        if (newBand.Length > 0)
                        {
                            char band = newBand.ToCharArray()[0];
                            Dcust.CustomerSaveBand(conn, trans, customerID, band); // check not part of transactions  
                        }
                    }


                    if (scoretype == 'R') // annual ReScore set to accepted -because don't want these showing as referred when already delivered. 
                        prop.PropResult = "A";
                    prop.WriteResult(conn, trans, newBand, scorecard, AccountBand);

                }

                result = PropResult;



                //prop.WriteResult(conn, trans);


                //if referred, set the referral flag and write a referral record except for rescore and parallel run
                if (PropReason.Length != 0 && PropResult == "R" && scoretype != 'R' && (scoretype != 'P'|| scoretype != 'E'))//add equifax condition
                {
                    propFlag.DateCleared = DateTime.MinValue.AddYears(1899);
                    propFlag.CheckType = "R";
                    if ((scorecard != 'P'|| scorecard != 'E') && scorecard != 'R') //add equifax condition
                    {
                        propFlag.Save(conn, trans, accountNo);
                    }

                    DReferral referral = new DReferral();
                    referral.CustomerID = customerID;
                    referral.DateProp = dateProp;
                    referral.EmployeeNo = this.User;
                    referral.DateReferral = DateTime.Now;
                    if (scoretype != 'P' || scoretype != 'E') //add equifax condition
                    {
                        referral.Update(conn, trans);
                    }
                }

                /* if the proposal has been approved or declined 
                 * then remove any referral flag which may be outstanding */
                if (PropResult != "R")
                {
                    propFlag.CheckType = "R";
                    if (scorecard != 'P' || scoretype != 'E') //add equifax condition
                    {
                        propFlag.Delete(conn, trans, accountNo);
                    }
                }

                string reason = "";
                if (scoretype == 'R')
                {
                    reason = "Auto Rescore";
                }
                else
                {
                    reason = "Score";
                }

                if (scorecard != 'P' || scoretype != 'E') //add equifax condition
                {
                    this.SaveScoreHist(conn, trans, customerID, dateProp, scorecard, prop.Points,
                                       Convert.ToSingle(RFLimit), newBand, user, reason, accountNo);
                }
                /* prior to scoring the customer may have been marked 
                 * as RF inactive. If this is an RF score then any 
                 * expiry flag should be removed from the customer */
                if (accountType == AT.ReadyFinance && (scoretype != 'P'|| scoretype != 'E') && scoretype != 'R')
                {
                    cust.RemoveCodeFromCustomer(conn, trans, customerID, "REX");
                }
            }

            //IP - 14/03/11 - #3314 - CR1245 - Return the referral reasons if any that the account has been referred on to be displayed in the referral popup
            AccountRepository a = new AccountRepository();
            referralReasons = a.GetReferralDescriptions(conn, trans, prop.Reason, prop.Reason2, prop.Reason3, prop.Reason4, prop.Reason5, prop.Reason6);

            //trans.Commit(); //xxhh remove when miguel resolved
        }
        // end of day rescore 
        public void ReScore(SqlConnection conn,
			SqlTransaction trans, 
            char scoretype, 
			string country, 
			string accountNo, 
			string accountType, 
			string customerID, 
			DateTime dateProp,
			DataSet rules,
			out string refCode, 
			out decimal score,
			out decimal RFLimit,
			out string result,
			out string bureauFailure,
            out string newBand)
		{
            newBand = "";
            bool referDeclined = false;
            string referralReasons = string.Empty;  //IP - 14/03/11 - #3314 - CR1245
          
            //
            DataTable ScoringRules = null;
            char scorecard ;
            scorecard = 'A'; ///By Default A set  for remove complie time error


            if (scoretype == 'B' || scoretype == 'A')
            { 
                if (scoretype == 'B')
            {
                DProposal DProp = new DProposal();
                DProp.ScoreTypetoUse(conn, trans, accountNo, customerID, out scoretype);
                if (scoretype == 'P' || scoretype == 'B') //so for rescore if Behavioural or Parallel chosen mark down as rescore.
                {
                    scoretype = 'R';
                    scorecard = 'B';
                    if (rules.Tables.Count > 1)
                        ScoringRules = rules.Tables[1];

                }
                else
                { // customer does not qualify for behavioural rescore as account settled
                    bureauFailure = "";
                    result = "";
                    RFLimit = 0;
                    score = 0;
                    refCode = "";
                    return;
                }
            }
            else //standard applicant.
            {
                scorecard = 'A';
                scoretype = 'A';
                if (rules.Tables.Count > 0)
                    ScoringRules = rules.Tables[0];
            }
             }

            //Add Equifax Score condition 
            if (scoretype == 'D' || scoretype == 'C')
            {
                if (scoretype == 'D')
                {
                    DProposal DProp = new DProposal();
                    DProp.ScoreTypetoUse(conn, trans, accountNo, customerID, out scoretype);
                    if (scoretype == 'E' || scoretype == 'D') //so for rescore if Behavioural or Parallel chosen mark down as rescore.
                    {
                        scoretype = 'R';
                        scorecard = 'D';
                        if (rules.Tables.Count > 1)
                            ScoringRules = rules.Tables[1];

                    }
                    else
                    { // customer does not qualify for behavioural rescore as account settled
                        bureauFailure = "";
                        result = "";
                        RFLimit = 0;
                        score = 0;
                        refCode = "";
                        return;
                    }
                }
                else //standard applicant.
                {
                    scorecard = 'C';
                    scoretype = 'C';
                    if (rules.Tables.Count > 0)
                        ScoringRules = rules.Tables[0];
                }

               }
            //End Equifax condition
            Score(conn, trans, country, accountNo,
               accountType, customerID, dateProp, ScoringRules, null, out newBand,
               out refCode, out score, out RFLimit, 0, out result,
               out bureauFailure, ref referDeclined, scorecard, scoretype, out referralReasons); //IP - 14/03/11 - #3314 - CR1245 - Returning referral reasons


        }

        public void ManualRefer(SqlConnection conn, SqlTransaction trans, string accountNo, string customerID, DateTime dateProp, bool isManualRefer, bool cashLoan = false)
		{
			DProposal prop = new DProposal();
			prop.User = this.User;
			prop.SetManualRefer(conn, trans, customerID, accountNo, dateProp, isManualRefer, cashLoan);

			DStatus stat = new DStatus();
			string status = stat.Unsettle(conn, trans, accountNo, DateTime.Now, User);
			DAccount acct = new DAccount(conn, trans, accountNo);
			acct.CurrentStatus = status;
			acct.Save(conn, trans);	
		
			// Adding a call to UpdateStatus as this could be an account converted to HP from
			// RF.  Need to check the status as account could have been manually referred
			// from the New Account Screen, and as the current status has just been updated 
			// by the Manual Refer process need to make sure status = U if deposit/instal
			// has not been paid.

			acct.UpdateStatus(conn, trans, accountNo);

            //IP - #3895 - 02/11/11 - Referred Accounts
            DReferral referral = new DReferral();
            referral.CustomerID = customerID;
            referral.DateProp = dateProp;
            referral.EmployeeNo = this.User;
            referral.DateReferral = DateTime.Now;
            referral.Update(conn, trans);
		}

		private bool EvaluateRule(DataRow details, XmlNode rule, Parameters Parms)
		{
/*#if(DEBUG)
			logMessage("Evaluating rule: "+rule.Attributes[Tags.RuleName].Value, "DEBUG", EventLogEntryType.Information);
#endif*/
			if((bool)Country[CountryParameterNames.LoggingEnabled])
			{
				//logMessage("Evaluating rule: "+rule.Attributes[Tags.RuleName].Value, "DEBUG", EventLogEntryType.Information);
				
				//need the operand name to save to the Scoring Details table.
				OperandName = rule.Attributes[Tags.RuleName].Value;
			}
            
			//loop through all the clauses and evaluate each one
			int i=0;
			bool [] results = new bool[2];	//storage for the results of the sub clauses
			string lo = "";
			bool result = false;

			foreach (XmlNode clause in rule.ChildNodes)
			{
				if(clause.Name!=Elements.LogicalOperator)	//ignore logical operators
				{
					results[i] = EvaluateClause(details, clause,ref Parms);
					i++;
				}
				if(clause.Name==Elements.LogicalOperator)
					lo = clause.Attributes[Tags.Operator].Value;
			}

			switch (lo)
			{
				case "AND":	result = results[0]&&results[1]?true:false;		
					break;
				case "OR":	result = results[0]||results[1]?true:false;
					break;
				default:	result = results[0];
					break;
			}
			rule.Attributes[Tags.State].Value = result.ToString();
			return result;
		}

        

		private bool EvaluateClause(DataRow details, XmlNode clause,ref Parameters Parms)
		{
			//check a clause and set it to true or false accordingly
			string op1 = "";	//operand 1
			string op2 = "";	//operand 2
			string co = "";		//comparison operator	
			string ot = "";		//operand type
			bool result = false;

			//if it's not a simple clause, we need to evaluate each of
			//if's child clauses
			if(clause.Attributes[Tags.Type].Value=="C")
			{
				int i=0;
				bool [] results = new bool[2];	//storage for the results of the sub clauses
				string lo = "";
				foreach(XmlNode child in clause.ChildNodes)
				{
					if(child.Name==Elements.Clause)		//exclude logical operator nodes
					{
						results[i] = EvaluateClause(details, child,ref Parms);
						i++;
					}
					if(child.Name==Elements.LogicalOperator)
						lo = child.Attributes[Tags.Operator].Value;
				}
				switch (lo)
				{
					case "AND":	result = results[0]&&results[1]?true:false;		
						break;
					case "OR":	result = results[0]||results[1]?true:false;
						break;
					default:
						break;
				}
			}
			else		//it's a simple clause and we can evaluate it directly
			{	
				//pick out the operands and operator from the clause node
				foreach(XmlNode child in clause.ChildNodes)
				{
					switch (child.Name)
					{
						case Elements.Operand1:	op1 = child.Attributes[Tags.Operand].Value;	
							ot = child.Attributes[Tags.Type].Value;
							break;
						case Elements.Operand2: op2 = child.Attributes[Tags.Operand].Value;
							//OperandValue = op2;
							break;
						case Elements.ComparisonOperator: co = child.Attributes[Tags.Operator].Value;
							break;
						default:
							break;
					}
				}

                var AccountR = new AccountRepository();
				//evaluate the clause in different ways depending on the type of
				//the first operand.
				switch (ot)
				{
					case OT.FreeText:	
					OperandValue = Convert.ToString(details[op1]);
                    if (op1 == "Worst Current Ever") //allow for automated increase spending limit if customer good enough.  
                    {
                        if (OperandValue != "N") // so not new customer
                            Parms.isExistingCustomer = true;
                    }
					switch(co)
					{
						case "=":	result = (Convert.ToString(details[op1])).Trim()==op2.Trim()?true:false;
							break;
						case "!=":	result = (Convert.ToString(details[op1])).Trim()!=op2.Trim()?true:false;
							break;
						case "LIKE": 
							if((Convert.ToString(details[op1])).Length < op2.Length)
								result = false;
							else
								result = (Convert.ToString(details[op1])).Substring(0, op2.Length)==op2?true:false;
							break;
						case "!LIKE":
							if((Convert.ToString(details[op1])).Length < op2.Length)
								result = true;
							else
								result = (Convert.ToString(details[op1])).Substring(0, op2.Length)==op2?false:true;
							break;
						default:
							break;
					}
                    if (op1 == "Worst Settled Ever" && Parms.WorstSettledEver == null)
                        Parms.WorstSettledEver = OperandValue;
                    if (op1 == "Worst Current Ever" && Parms.WorstCurrentEver== null )
                        Parms.WorstCurrentEver = OperandValue;
                    
                    if ((op1 == "Worst Settled Ever" || op1 =="Worst Current Ever") && Parms.ruletype == "Refer" && result==true )
                    {
                        if (Parms.MinimumWorstStatustoCheck == null || Convert.ToInt16(Parms.MinimumWorstStatustoCheck) > Convert.ToInt16(op2.Trim()))
                        { //want to get the minimum status to refer
                            Parms.MinimumWorstStatustoCheck = op2.Trim();
                            Parms.AcceptedSinceSCReferralChecked = false; 
                        }
                        AcceptedSinceSCReferralCheck(ref Parms);
                        if (Parms.AcceptedSinceSCReferral) 
                            result = false;
                    }
                    if (Parms.HasMobilePhone == false && 
                        Convert.ToString(details["Mobile Phone Y/N"]) == "Y")
                        Parms.HasMobilePhone = true;


                    if ((op1 == "Has work phone Y/N" || op1 == "Has home phone Y/N") && Parms.ruletype == "Refer"
                        && co == "=" && Parms.HasMobilePhone)
                    {
                        if (Parms.isExistingCustomer && (bool)Country[CountryParameterNames.ReferExistingCustomersWithoutHomeandWorkPhonesButwithMobiles]==false) // do not check this rule if existing customer
                        {
                            result = false;
                        }
                        if (!(bool)Country[CountryParameterNames.ReferNewCustomersWithoutHomeandWorkPhonesButwithMobiles] && !Parms.isExistingCustomer)
                        {
                            result = false;
                        }
                    }

					break;

					case OT.Optional:
					OperandValue = Convert.ToString(details[op1]);
					switch(co)
					{
						case "=":	result = (Convert.ToString(details[op1])).Trim()==op2.Trim()?true:false;
							break;
						case "!=":	result = (Convert.ToString(details[op1])).Trim()!=op2.Trim()?true:false;
							break;
						default:
							break;
					}
						break;
					case OT.Decimal:
					OperandValue = Convert.ToDecimal(details[op1]).ToString();
                    decimal opvalue = Convert.ToDecimal(op2);
                    if (op1 == "RF Spending Limit" && Convert.ToDecimal(Country[CountryParameterNames.MaxSpendLimitRefer]) != opvalue)
                    {
                        CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue(Parms.conn, Parms.trans);
                        CMSV.ExecuteNonQuery("MaxSpendLimitRefer", Convert.ToString(opvalue));
                        Country[CountryParameterNames.MaxSpendLimitRefer] = op2;
                    }
                    
    

                    if (op1 == "RF Spending Limit" && Parms.ruletype=="Refer" && (co ==">" || co==">=") ) //allow for automated increase spending limit if customer good enough. 
                    {
                        if (Convert.ToDecimal(OperandValue) > opvalue) //check any uplift percentage
                             op2 = Convert.ToString(opvalue +
                                 opvalue*AccountR.UpliftPercentage(Parms.AccountNo, Parms.conn, Parms.trans)/100);   
                        else
                            {
                                var Pup = new ProposalUpdateUpliftPercent(Parms.conn, Parms.trans);
                                Pup.ExecuteNonQuery(Parms.AccountNo, Parms.CustomerId, 0);
                            }
                    }

                    

					switch(co)
					{
                        case "=": result = Convert.ToDecimal(OperandValue) == Convert.ToDecimal(op2) ? true : false;
							break;
                        case "!=": result = Convert.ToDecimal(OperandValue) != Convert.ToDecimal(op2) ? true : false;
							break;
                        case "<": result = Convert.ToDecimal(OperandValue) < Convert.ToDecimal(op2) ? true : false;
							break;
                        case ">": result = Convert.ToDecimal(OperandValue) > Convert.ToDecimal(op2) ? true : false;
							break;
                        case "<=": result = Convert.ToDecimal(OperandValue) <= Convert.ToDecimal(op2) ? true : false;
							break;
                        case ">=": result = Convert.ToDecimal(OperandValue) >= Convert.ToDecimal(op2) ? true : false;
							break;
						default:
							break;
					}
                    if (op1 == "Monthly Expenses(ex accom)" && Parms.ruletype == "Refer" &&  co == "<" &&
                        !(bool)Country[CountryParameterNames.MinExpenseReferforExistingCustomer] ) // do not check this rule if existing customer
                        if (Parms.isExistingCustomer)
                            result = false;
						break;
					case OT.Numeric:
					OperandValue = Convert.ToInt32(details[op1]).ToString();
                    if (op1 == "Number of settled accounts" || op1 == "No of Current Accounts") //allow for automated increase spending limit if customer good enough.  Agreement Total(- deposit)
                    {
                        if (Convert.ToInt32(OperandValue) > 0)
                            Parms.isExistingCustomer = true;
                    }
					switch(co)
					{
						case "=":	result = Convert.ToInt32(details[op1])==Convert.ToInt32(op2)?true:false;
							break;
						case "!=":	result = Convert.ToInt32(details[op1])!=Convert.ToInt32(op2)?true:false;
							break;
						case "<":	result = Convert.ToInt32(details[op1])<Convert.ToInt32(op2)?true:false;
							break;
						case ">":	result = Convert.ToInt32(details[op1])>Convert.ToInt32(op2)?true:false;
							break;
						case "<=":	result = Convert.ToInt32(details[op1])<=Convert.ToInt32(op2)?true:false;
							break;
						case ">=":	result = Convert.ToInt32(details[op1])>=Convert.ToInt32(op2)?true:false;
							break;
						default:
							break;
					}
						break;
					default:
						break;
				}
			}
          

			//Update the clause node to reflect whether it is true or false
			clause.Attributes[Tags.State].Value = result.ToString();
			return result;
		}

        private void AcceptedSinceSCReferralCheck(ref Parameters Parms)
        {
            if (Parms.WorstCurrentEver != null & Parms.WorstSettledEver != null && !Parms.AcceptedSinceSCReferralChecked)
            {
                if ((bool)Country[CountryParameterNames.StatExistsRefer])
                {
                    var ACNA = new AcctCheckNewafterBadStatus(Parms.conn, Parms.trans);
                    byte? AcceptedAccountAfterBadStatus = 0;
                    string worstCurrentStatus = Parms.WorstCurrentEver; string worstSettled = Parms.WorstSettledEver;
                    if (Convert.ToInt16(worstCurrentStatus) < Convert.ToInt16(Parms.MinimumWorstStatustoCheck))
                        worstCurrentStatus = worstSettled; //we are doing this because we don't want to check the date of good status codes so just set the worst current to the worst settled.
                    if (Convert.ToInt16(worstSettled) < Convert.ToInt16(Parms.MinimumWorstStatustoCheck))
                        worstSettled = worstCurrentStatus; //as above.... 

                    ACNA.ExecuteNonQuery(Parms.AccountNo, Parms.CustomerId, worstCurrentStatus,worstSettled, out AcceptedAccountAfterBadStatus);
                    if (AcceptedAccountAfterBadStatus == 1)
                        Parms.AcceptedSinceSCReferral = true;
                    else
                        Parms.AcceptedSinceSCReferral = false;
                }
                Parms.AcceptedSinceSCReferralChecked = true; //only check once.
            }

        }
		public DataSet GetReferralData(string customerID, string accountNo, DateTime dateProp, string countryCode)
		{
			DataSet ds = new DataSet();
			DReferral refer = new DReferral();
			DCreditBureau creditBureau = null;
			DCreditBureauDefaults creditBureauDefaults = null;
			DataTable dtCreditBureau = null;
			DataTable dtCreditBureauDefaults = null;
			DataTable dtReferralSummary = null;

			///retrieve the data used in scoring
			DataTable dtScoringDetails = null;
			DScoring s = new DScoring();
			s.AccountNumber = accountNo;
			dtScoringDetails = s.GetScoreDetails();

            if (ContactBayCorp || ContactDPGroup)
			{
				/* me may have bureau data to return also */
				creditBureau = new DCreditBureau();
				creditBureauDefaults = new DCreditBureauDefaults();
				dtCreditBureau  = creditBureau.Get(customerID);

                /* we need to transform the response xml into html */
                string xml = "";
                XmlDocument doc = null;
                BCreditBureau cb = new BCreditBureau();

                foreach(DataRow r in dtCreditBureau.Rows)
				{
					if(ContactBayCorp)
					{
					xml = (string)r[CN.ResponseXML];
					doc = new XmlDocument();
                    //CR 843 Added this check for xml data
                    if (!xml.Equals(string.Empty))
                    {
                        doc.LoadXml(xml);
                        r[CN.ResponseXML] = cb.Transform(doc, STL.Common.Constants.CreditBureau.CreditBureau.Baycorp);
                    }
					}

                    if (ContactDPGroup)
                    {
                        //CR 843
                        //Second xml field
                        xml = (string)r[CN.ResponseXML2];
                        if (!xml.Equals(string.Empty))
                        {
                            doc = new XmlDocument();
                            doc.LoadXml(xml);
                            r[CN.ResponseXML2] = cb.Transform(doc, STL.Common.Constants.CreditBureau.CreditBureau.DPGroup);
                        }
					}
                        //End CR 843
                    }
				dtCreditBureauDefaults = creditBureauDefaults.Get(customerID);
			}

			refer.CustomerID = customerID;
			refer.AccountNo = accountNo;
			refer.DateProp = dateProp;

			DataSet referralSet = refer.GetReferralData();
			DataTable referralData = new DataTable(TN.ReferralData);
			if (referralSet.Tables.Contains(TN.ReferralData))
			{
				referralData = referralSet.Tables[TN.ReferralData];
				referralSet.Tables.Remove(TN.ReferralData);
			}
			DataTable referralAudit = new DataTable(TN.ReferralAudit);
			if (referralSet.Tables.Contains(TN.ReferralAudit))
			{
				referralAudit = referralSet.Tables[TN.ReferralAudit];
				referralSet.Tables.Remove(TN.ReferralAudit);
			}

			dtReferralSummary = refer.GetReferralRules(null, null, customerID, dateProp);


			// Code below is needed for new Referral screen based on E-Cosacs
			// Underwriter's screen.
			//DProposalResult prop = new DProposalResult();
			//prop.GetProposalRefDetails(accountNo);

            if (ContactBayCorp || ContactDPGroup)
				ds.Tables.AddRange(new DataTable[] {referralData, referralAudit, dtCreditBureau, dtCreditBureauDefaults, dtScoringDetails, dtReferralSummary}) ;
			else
				ds.Tables.AddRange(new DataTable[] {referralData, referralAudit, dtScoringDetails, dtReferralSummary /*prop.ProposalRef*/}) ;
			return ds;
		}

		public DataSet GetDocConfirmationData(string customerID, string accountNo, DateTime dateProp)
		{
			DataSet ds = new DataSet();

			DEmployment emp = new DEmployment();
			emp.GetEmployment(customerID);

			DProposal prop = new DProposal();
			prop.CustomerID = customerID;
			prop.AccountNo = accountNo;
			prop.DateProp = dateProp;
			ds.Tables.AddRange(new DataTable[]{prop.GetDocConfirmationData(), prop.GetPreviousDocConfirmationData(), emp.GetRow(TN.Employment)});
			return ds;
		}

		public void CompleteReferralStage(SqlConnection conn, SqlTransaction trans, 
											string customerID, 
											string accountNo,
											DateTime dateProp,
											string newNotes, string notes, bool approved, 
											bool rejected, bool reOpen, int branch,
											decimal creditLimit, string countryCode)
		{
			DProposalFlag propFlag = new DProposalFlag();
			propFlag.CheckType = "R";
			propFlag.CustomerID = customerID;
			propFlag.DateProp = dateProp;
			propFlag.DateCleared = DateTime.Now;
			propFlag.EmployeeNoFlag = this.User;
			propFlag.Save(conn, trans, accountNo);

			string propResult = approved==true?"A":"D";

			if (newNotes.Trim().Length > 0)
			{
				// Get Employee name to audit comments
				DEmployee employee = new DEmployee();
				employee.GetEmployeeDetails(conn, trans, this.User);
				notes = employee.EmployeeName + " (" + this.User + ") - "+ DateTime.Now + " :\n\n" + newNotes + "\n\n" + notes;
			}

			DProposal prop = new DProposal();
			// Save the result
			prop.SetPropResult(conn, trans, customerID, accountNo, dateProp, propResult, notes);
			// Save the audit record
			prop.AuditPropResult(conn, trans, customerID, accountNo, dateProp, this.User);

			BAccount ac = new BAccount();
			DAccount acct = new DAccount();
			DCustomer cust = new DCustomer();

			cust.GetBasicCustomerDetails(conn, trans, customerID, accountNo, "H");			

			if(rejected)
				cust.SetCreditLimit(conn, trans, customerID, 0, "A");
			else
			{
				if(cust.RFLimit != creditLimit)		/* limit has been overridden */
					cust.SetOverrideLimit(conn, trans, customerID, creditLimit);
				else
					cust.SetCreditLimit(conn, trans, customerID, creditLimit, "A");
			}
			
			if(reOpen && !rejected)
			{
				// If previously rejected account, write record to FACT
				ac.FactTransCancel(conn, trans, accountNo, branch, true, DateTime.Now);
				acct.ResetAgrmnTotal(conn, trans, accountNo);
			}
			else
				// DSR 24/02/03 Update the account status
				acct.UpdateStatus(conn, trans, accountNo);

            if ((bool)Country[CountryParameterNames.PrizeVouchersActive] && approved)
            {
                DAgreement agreement = new DAgreement(conn, trans, accountNo, 1);
                BCustomer customer = new BCustomer();

                customer.User = this.User;
                customer.IssueAdditionalPrizeVouchers(conn, trans, accountNo, agreement.CashPrice, 0);
            }
		}

		public void SaveReferralNotes(SqlConnection conn, SqlTransaction trans, 
			string customerID,
			string accountNo,
			DateTime dateProp,
			string newNote,
			decimal creditLimit,
			string countryCode)
		{
			if (newNote.Trim().Length > 0)
			{
				// Get existing notes to prefix with the new note
				string notes = "";
				DataSet ds = GetReferralData(customerID, accountNo, dateProp, countryCode);

				foreach (DataTable dt in ds.Tables)
					if (dt.TableName == TN.ReferralData)
						foreach (DataRow r in dt.Rows)
							notes = (string)r[CN.PropNotes];

				// Get Employee name to audit comments
				DEmployee employee = new DEmployee();
				employee.GetEmployeeDetails(conn, trans, this.User);
				notes = employee.EmployeeName + " (" + this.User + ") - " + DateTime.Now + " :\n\n" + newNote + "\n\n" + notes;

				// Save all the notes
				DProposal prop = new DProposal();
				prop.SaveReferralNotes(conn, trans, customerID, accountNo, dateProp, notes, creditLimit);
			}
		}

        public void SaveProposalNotes(SqlConnection conn, SqlTransaction trans, 
            string customerID,
            string accountNo,
            DateTime dateProp,
            string notes)
        {
            DProposal prop = new DProposal();
            prop.SaveProposalNotes(conn,trans,customerID,accountNo,dateProp,notes);
        }


		public DataTable GetProposalsToRescore(char scoretype)
		{
			DProposal prop = new DProposal();
			return prop.GetProposalsToRescore(scoretype);
		}

		public void SaveDocConfirmation(SqlConnection conn, SqlTransaction trans, DataSet propData, bool complete, string accountnumber)
		{
			DProposal prop = new DProposal();
			DProposalFlag pf = new DProposalFlag();

            string acctno = string.Empty;

			foreach(DataTable dt in propData.Tables)
			{
				if(dt.TableName==TN.DocConfirmation)
				{
					foreach(DataRow r in dt.Rows)
					{
                        acctno = (string)r[CN.AccountNo];
						prop.AccountNo = (string)r[CN.AccountNo];
                        if (acctno == "") //not sure why but intermittent error where account number not passed in correctly
                        {
                            acctno = accountnumber;
                            prop.AccountNo = accountnumber;
                        }
                        pf.CustomerID = prop.CustomerID = (string)r[CN.CustomerID];
						pf.DateProp = prop.DateProp = (DateTime)r[CN.DateProp];
						prop.ProofOfAddress = (string)r[CN.ProofOfAddress];
						prop.ProofOfID = (string)r[CN.ProofOfID];
						prop.ProofOfIncome = (string)r[CN.ProofOfIncome];
                        prop.ProofOfBank = Convert.ToString(r[CN.ProofOfBank]);             //IP - 14/12/10 - Store Card
						prop.DCText1 = (string)r[CN.DCText1];
						prop.DCText2 = (string)r[CN.DCText2];
						prop.DCText3 = (string)r[CN.DCText3];
                        prop.ProofOfBankTxt = Convert.ToString(r[CN.ProofOfBankTxt]);       //IP - 14/12/10 - Store Card
                        prop.EmployeeNoChanged = this.User;
						prop.SaveDocConfirmation(conn, trans);
					}
				}
			}

			if(complete && acctno != string.Empty)
			{			
				pf.CheckType = "DC";
				pf.DateCleared = DateTime.Now;
				pf.EmployeeNoFlag = this.User;
				pf.Save(conn, trans, acctno);
			}


            if (acctno != String.Empty)
            {
                AccountRepository AccountR = new AccountRepository();
                AccountR.InstantCreditDACheck(acctno, User, conn, trans);           //#13821 - Method already clears proposal if criteria met and submits booking

                //if (AccountR.InstantCreditDACheck(acctno, User, conn, trans)) //IP - 03/03/11 - #3255 - Added User
                    //new DAgreement(conn, trans, acctno, 1) { User = Users.ICAutoDA }.ClearProposal(conn, trans, acctno, "AUTO");
            }

			
		}

		public void GetUnclearedStage(SqlConnection conn, SqlTransaction trans, string accountNo, ref string newAccount, ref string checkType, ref DateTime dateProp, ref string propResult,ref int points)
		{
			DProposal prop = new DProposal();
			prop.GetUnclearedStage(conn, trans, accountNo, ref newAccount, ref checkType, ref dateProp, ref propResult,ref points);
		}
						 
		public void ClearFlag(SqlConnection conn, 
							  SqlTransaction trans,
							  string custID, 
							  string chkType, 
							  DateTime dateProp,
							  bool reOpen, string acctno)
		{
			DProposalFlag propFlag = new DProposalFlag();
			propFlag.OrigBr = 0;
			propFlag.CustomerID = custID;
			propFlag.DateProp = dateProp;
			
			if(!reOpen)
				propFlag.DateCleared = DateTime.Now;
			
			propFlag.EmployeeNoFlag = this.User;
			propFlag.CheckType = chkType;
			propFlag.Save(conn, trans, acctno);
		}
		/*
		public int GetScoreLimits()
		{
			Function = "BScoring::GetScoreLimits";
			DataSet ds = new DataSet();	
			data = new DScoring();
			data.GetScoreLimits();
			ds.Tables.Add();		

			DataRow row = data.Limits.Rows[0];
			ScoreLimits = new scoreLimits();

			ScoreLimits.AcceptScore = (short)row["AcceptScore"];
			ScoreLimits.ReferScore =  (short)row["ReferScore"];
			return 0;
		}
		*/
		public struct scoreLimits
		{
			public short ReferScore;
			public short DeclineScore;
			public short BureauMinimum;
			public short BureauMaximum;
		}

		public void UnClearFlag(SqlConnection conn, SqlTransaction trans, string accountNo, 
								string checkType, bool changeStatus, int user)
		{
			DProposalFlag pf = new DProposalFlag();
			pf.UnClearFlag(conn, trans, accountNo, checkType, changeStatus,user);
			if(checkType==SS.S1)
			{
                var delTot = 0m;                                             //IP - 28/06/11 - 5.13 - LW73619 - #3751
                BDelivery del = new BDelivery();                            //IP - 28/06/11 - 5.13 - LW73619 - #3751
                delTot =  del.DeliveryGetTotal(conn, trans, accountNo);     //IP - 28/06/11 - 5.13 - LW73619 - #3751

				BAgreement agree = new BAgreement();
				agree.Populate(conn, trans, accountNo, 1);

                if (agree.AgreementTotal != delTot)                        //IP - 28/06/11 - 5.13 - LW73619 - #3751 - Only want to set HoldProp if there are outstanding deliveries.
                {
                    agree.HoldProp = "Y";
                    agree.Save(conn, trans);
                }
			}
		}

		public DataSet GetReferralSummaryData(string accountNo, 
												string customerID, 
												string accountType, 
												DateTime dateProp, 
												out XmlNode lineItems)
		{
			DataSet data = new DataSet();
			BItem item = new BItem();
			lineItems = item.GetLineItems(null, null, accountNo, accountType, 
											(string)Country[CountryParameterNames.CountryCode], 1);

			DProposal prop = new DProposal();
			data.Tables.Add(prop.GetReferralSummaryData(accountNo, customerID, accountType, dateProp));

			return data;
		}



		public void SpendLimitReferral(SqlConnection conn, SqlTransaction trans, 
										string accountNo, string customerID, 
										DateTime dateProp, string newNote,
										decimal creditLimit, string countryCode)
		{
			//Get the current date prop for this account - 68963 
			DProposal prop = new DProposal();
			prop.GetDatePropForAccount(conn, trans, accountNo, customerID, ref dateProp);

			SaveReferralNotes(conn, trans, customerID, accountNo, dateProp, newNote, creditLimit, countryCode);
			ManualRefer(conn, trans, accountNo, customerID, dateProp, false);
		}
			
		private void SaveAddresses(SqlConnection conn, SqlTransaction trans, 
										string custID, DataRow addressRow)
		{
			DCustomer cust = new DCustomer();

			DataTable dt = new DataTable("Addresses");
            dt.Columns.AddRange(new DataColumn[]{	new DataColumn("AddressType"),
													new DataColumn("Address1"),
													new DataColumn("Address2"),
													new DataColumn("Address3"),
													new DataColumn("PostCode"),
													new DataColumn(CN.DeliveryArea),
													new DataColumn("Notes"),
													new DataColumn("EMail"),
													new DataColumn("DialCode"),
													new DataColumn("PhoneNo"),
													new DataColumn("Ext"),
													new DataColumn("DateIn", Type.GetType("System.DateTime")),
													new DataColumn("NewRecord", Type.GetType("System.Boolean")),
                                                    new DataColumn("Zone"),
                                                    new DataColumn("DELTitleC"),
                                                    new DataColumn("DELFirstname"),
                                                    new DataColumn("DELLastname")
        });

            var latitudeColumn = new DataColumn("Latitude", Type.GetType("System.Double")); // Address Standardization CR2019 - 025
            latitudeColumn.AllowDBNull = true;
            var longitudeColumn = new DataColumn("Longitude", Type.GetType("System.Double")); // Address Standardization CR2019 - 025
            longitudeColumn.AllowDBNull = true;
            dt.Columns.Add(latitudeColumn);
            dt.Columns.Add(longitudeColumn);

            DataRow row = dt.NewRow();
			row["AddressType"] = "W";
			row["Address1"] = (string)addressRow[CN.Address1];
			row["Address2"] = (string)addressRow[CN.Address2];
			row["Address3"] = (string)addressRow[CN.Address3];
			row["PostCode"] = (string)addressRow[CN.PostCode];
			row[CN.DeliveryArea] = (string)addressRow[CN.DeliveryArea];
			row["Notes"] = "";
			row["EMail"] = "";
			row["DialCode"] = "";
			row["PhoneNo"] = "";
			row["Ext"] = "";
			row["DateIn"] = DateTime.Today;
			row["NewRecord"] = false;
            row[CN.Zone] = "";
            row["DELTitleC"] = "";
            row["DELFirstname"] = "";
            row["DELLastname"] = "";
            row["Latitude"] = DBNull.Value; // Address Standardization CR2019 - 025
            row["Longitude"] = DBNull.Value; // Address Standardization CR2019 - 025

            cust.SaveAddress(conn, trans, custID, this.User, row);
		}

		public void SetPotentialCreditLimit(SqlConnection conn,
			SqlTransaction trans, 
			string country, 
			string accountNo, 
			string customerID, 
			DateTime dateProp,
			DataTable rules,
			out decimal RFLimit)
	{
			decimal score = 0;

			BCreditBureau creditBureau = new BCreditBureau();

			DataRow r = null;

			DCustomer cust = new DCustomer();
            var Parms = new Parameters();
            Parms.ruletype = "Score"; Parms.conn = conn; Parms.trans = trans;
            Parms.AccountNo = accountNo;

			DScoring s = new DScoring();
			s.AccountNumber = accountNo;
			DataTable dt = s.GetScoreDetails(conn, trans);
			foreach(DataRow row in dt.Rows)
				r = row;

			if(r == null)
			{
				throw new STLException(GetResource("M_NOSCORINGINFO", new object [] {accountNo}));
			}		

			XmlDocument doc = new XmlDocument();
			foreach (DataRow row in rules.Rows)
			{
				doc.LoadXml((string)row[CN.RulesXML]);
			}

			/* process the scoring rules first so that we know the score and the RF Limit */
			XmlNodeList scoringRules = doc.SelectNodes("//Rule[@Type = 'S']");
			foreach(XmlNode rule in scoringRules)
			{
				if((string)r["account type"]!=AT.Cash && (string)r["account type"]!=AT.Special && (string)r["account type"]!=AT.ReadyFinance && rule.Attributes[Tags.ApplyHP].Value==Boolean.TrueString)
				{
					EvaluateRule(r, rule,Parms);

					if(rule.Attributes[Tags.State].Value == Boolean.TrueString)
					{
						score += Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
						Points = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
					}
					else
						Points = 0;

					if((bool)Country[CountryParameterNames.LoggingEnabled] && Points != 0)
					{
						//save score details to the Scoring Details table to enable
						//credit staff to see what values are being scored against
						s.SaveScoreDetails(conn, trans, customerID, accountNo, 
							DateTime.Now, OperandName, OperandValue, Points);
					}				
				}
			}

			short branchNo = Convert.ToInt16(accountNo.Substring(0,3));
			DBranch b = new DBranch();
			b.Populate(conn, trans, branchNo);
			s.Country = country;
			s.CustomerID = customerID;
			s.DateProp = dateProp;
			s.Score = score;
			s.Region = b.Region;
			s.GetPotentialRFCreditLimit(conn, trans);

			cust.CustID = customerID;

			cust.SetPotentialCalcDate(conn, trans, DateTime.Now, score);

			RFLimit = s.CreditLimit;

			cust.SetCreditLimit(conn, trans, customerID, s.CreditLimit, "P");
		}	
		
		public DataTable GetNonRFProposals()
		{
			DProposal prop = new DProposal();
			return prop.GetNonRFProposals();
		}
        //LoadBSCustomers(string category, int runno )
        /// <summary>
        /// Loads Behavioural Scoring from either end of day rescore or for Parallel Run.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="runno"></param>
        /// <returns></returns>
        public DataSet LoadBSCustomers(string category,
                                int runno)
        {
            DataSet ds = new DataSet();
            DProposal p = new DProposal();
            ds.Tables.Add(p.LoadBSCustomers(category, runno));
            return ds;
        }

        private void GetScoreType()
        {
                    
        }

        public void SaveScoreHist(SqlConnection conn,
            SqlTransaction trans, string CustomerID, DateTime dateprop, char? scorecard, short? points , float creditlimit,
            string scoringband, int user, string reasonchanged, string AccountNo
            )
        {
            DProposal Prop = new DProposal();
            Prop.CustomerID = CustomerID;
            Prop.DateProp = dateprop;
            Prop.Points = points;
            Prop.creditawarded = creditlimit;
            Prop.ScoringBand =  scoringband;
            Prop.EmployeeNoChanged = user;
            Prop.AccountNo = AccountNo;
            Prop.SaveScoreHist(conn,trans,reasonchanged,scorecard);

        }

        public void ApplyBSRescore(SqlConnection conn,
    SqlTransaction trans, string CustomerID, int user, int runno
    )
        {
            DProposal Prop = new DProposal();
            
            
            Prop.ApplyBSRescore(conn, trans, CustomerID, 0, user); 

        }


        public void ApplyLatestBSRescoreForRun(SqlConnection conn,    SqlTransaction trans    )
        {
            DProposal Prop = new DProposal();

            Prop.ApplyLatestBSRescoreForRun(conn, trans);

        }

        public DataSet LoadScoreHistforCustomer(string CustomerId)
        {
            DataSet ds = new DataSet();
            DProposal prop = new DProposal();
            prop.CustomerID = CustomerId;
            ds.Tables.Add(prop.LoadScoreHistforCustomer());
            return ds;
        }
     
        
        /// <summary>
        /// Determines the customer and account band to bring back for accounts that have been scored. 
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <param name="Custid"></param>
        /// <param name="AccountBand"></param>
        /// <param name="CustomerBand"></param>
        /// <param name="ScoreCard"></param>
        /// <param name="points"></param>
        public void DetermineBands(SqlConnection conn, SqlTransaction trans, string AccountNo, string Custid, out string AccountBand, out string CustomerBand, string ScoreCard, int points)
        {
            if (AccountNo.Length > 3 && AccountNo[3] == '0' && Custid != String.Empty)
            {
                DataSet DS = this.DataforBands(AccountNo, Custid, conn, trans);
                this.DetermineBandsfromData(DS, out AccountBand, out CustomerBand, ScoreCard, points);
            }
            else
            {
                CustomerBand = string.Empty;
                AccountBand = string.Empty;
            }
        }



        private DataSet DataforBands(string AccountNo, string Custid, SqlConnection conn, SqlTransaction trans)
        {

            DProposal Dprop = new DProposal();
            Dprop.AccountNo = AccountNo;
            Dprop.CustomerID = Custid;
            return Dprop.ScoreBandLoadData(conn, trans);
        }


        private void DetermineBandsfromData(DataSet DS, out string AccountBand, out string CustomerBand,
            string ScoreCard, int points)
        {
            if (ScoreCard == null)
                ScoreCard = string.Empty;

            

            DateTime DatelastOverride;
            bool EverAuthorised = false;
            AccountBand = "";
            CustomerBand = "";
            try
            {
                DataRow ScoreRow = null;

                DateTime DateProp = DateTime.MinValue;
                if (DS.Tables[0].Rows.Count > 0)
                {
                    ScoreRow = DS.Tables[0].Rows[0];
                    DateProp = (DateTime)ScoreRow["DateProp"];

                }
                else
                    DateProp = DateTime.Now;
                //This contains the earliest date for when bands were introduced. Accounts preceding this did not have bands
                DataRow DateRow = DS.Tables[2].Rows[0];
                DateTime StartDate = (DateTime)DateRow["startdate"];

                //if (DateProp > StartDate) //otherwise band could be applied
                //{

                    // default band is what is stored on the databse.
                    AccountBand = (string)ScoreRow["IPBand"];
                    //if we are not just scored then 0 sent in then use points stored on database 
                    if (points == 0)
                        points = Convert.ToInt32(ScoreRow["points"]);
                    // get existing band from database
                    CustomerBand = (string)ScoreRow["CustomerBand"];

                    DatelastOverride = (DateTime)ScoreRow["DateLastOverride"];
                    DateTime DateLastScored = (DateTime)ScoreRow["datelastscored"];

                    // Scorecard can be passed in if rescoring else use database.
                    if (ScoreCard == String.Empty)
                        ScoreCard = (string)ScoreRow["ScoreCard"];


                    // if manually override band occurred since last scored use this
                    if (DatelastOverride > DateLastScored && (string)ScoreRow["ManualBand"] != "")
                    {
                        CustomerBand = (string)ScoreRow["ManualBand"];
                    }
                    else
                    {
                        //Filter based on points
                        DataView DV = DS.Tables[1].DefaultView;
                        DV.RowFilter = " ScoreType = '" + ScoreCard + "' AND PointsFrom <= " + points.ToString() + " AND PointsTo >= " + points.ToString();

                        DV.Sort = "DateImported DESC";
                        if (DV.Count > 0)
                            CustomerBand = (string)DV[0]["Band"];

                    }

                    DateTime DateDel = (DateTime)ScoreRow["DateDel"];
                    int EmployeeAuthorised = (int)ScoreRow["EmployeeAuthorised"];
                    if (DateDel.Year > 1900 || EmployeeAuthorised > 0)
                        EverAuthorised = true;   // if delivered or ever authorised then don't change

                    if (CustomerBand == String.Empty)
                        CustomerBand = (string)Country[CountryParameterNames.TermsTypeBandDefault];

                    if (!EverAuthorised && DateProp > StartDate) // Ipband wont change if authorised so set it if not.  
                        AccountBand = CustomerBand;
                //}

            }
            catch (Exception)
            {
                throw;
            }

            //ScoreRow[""];
            //ScoreRow[""];

            //if (((string)addressRow[CN.AddressType]).Trim() == drpDeliveryAddress.Text.Trim())

        }



        // todo add to config
        public BProposal()
        {

        }
    }


}
