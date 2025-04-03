using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DEmployee.
	/// </summary>
	public class DEmployee: DALObject
	{
		private short _branchNo = 0;
		private string _fACTEmpeeNo = "";
		private string _empName = "";
		private string _empType = "";
		private string _password ="";
		private int _maxrows = 0;
		private DateTime _datepasschge = DateTime.Today;
		private short _commissionType = 0;
		private DataTable _salesstaff = null;
        private DataTable _logonhistory = null;
        private DataTable _allstaff = null;
		private DataTable _employees = null;
        private DataTable _commstaff = null;
        private DataTable _salesCommStaff = null;
		
		public int GetStaffSummary(XmlNode parms)
		{
			try
			{
				_salesstaff = new DataTable(TN.SalesStaff);
				XmlNode branch = parms.FirstChild;
				XmlNode type = branch.NextSibling;

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[0].Value = Convert.ToInt32(branch.Attributes[Tags.Value].Value);
				parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[1].Value = type.Attributes[Tags.Value].Value;

				result = this.RunSP("DN_StaffSummaryGetSP", parmArray, _salesstaff);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}
	
        public int GetAllStaff()
        {
            try
            {
                _allstaff = new DataTable(TN.AllStaff);

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
                parmArray[0].Value = 0;
                parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
                parmArray[1].Value = " ";

                result = this.RunSP("DN_StaffSummaryGetSP", parmArray, _allstaff);
                if(result==0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }
        // Get all staff that have commission records - for Commission enquiry
        public int GetCommStaff(XmlNode parms)
        {
            try
            {
                _commstaff = new DataTable(TN.CommStaff);
                XmlNode branch = parms.FirstChild;
                XmlNode type = branch.NextSibling;

                parmArray = new SqlParameter[2];
               
                
                parmArray[0] = new SqlParameter("@branchNo", SqlDbType.Char);       //CR1035
                parmArray[0].Value = Convert.ToString(branch.Attributes[Tags.Value].Value);
parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
                
                parmArray[1].Value = type.Attributes[Tags.Value].Value;

                result = this.RunSP("StaffCommissionSummaryGetSP", parmArray, _commstaff);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        // Get all staff that have commission records
        public int GetSalesCommStaff(XmlNode parms)
        {
            try
            {
                _salesCommStaff = new DataTable(TN.SalesCommStaff);
                XmlNode branch = parms.FirstChild;
                XmlNode type = branch.NextSibling;

                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@branchNo", SqlDbType.Char);      
                parmArray[0].Value = Convert.ToString(branch.Attributes[Tags.Value].Value);
                parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); 

                parmArray[1].Value = type.Attributes[Tags.Value].Value;

                result = this.RunSP("StaffSalesCommissionSummaryGetSP", parmArray, _salesCommStaff);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DataTable Employees
		{
			get
			{
				return _employees;
			}
		}
		public DataTable SalesStaff
		{
			get
			{
				return _salesstaff;
			}
		}

        public DataTable AllStaff
        {
            get
            {
                return _allstaff;
            }
        }
        public DataTable CommStaff
        {
            get
            {
                return _commstaff;
            }
        }

        public DataTable SalesCommStaff
        {
            get
            {
                return _salesCommStaff;
            }
        }

		public int GetStaffDetails(int employeeNumber)
		{
		
			try
			{
				_employees = new DataTable();			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@employeeNumber", SqlDbType.Int);
				parmArray[0].Value = employeeNumber;

				result = this.RunSP("DN_CourtspersonGet", parmArray, _employees);
			
				if(result==0)
				{
					result = (int)Return.Success;
				}
				else
				{
					result = (int)Return.Fail;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}
		
		

		public int GetEmployeeDetails(SqlConnection conn, SqlTransaction trans, int employeeNumber)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = employeeNumber;
				parmArray[1] = new SqlParameter("@fACTEmpeeNo", SqlDbType.NVarChar,4);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@empeename", SqlDbType.NVarChar,20);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[4] = new SqlParameter("@maxrow", SqlDbType.NVarChar,12);
				parmArray[4].Value = "";
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@commissiontype", SqlDbType.SmallInt);
				parmArray[5].Value = this.commissionType;
				parmArray[5].Direction = ParameterDirection.Output;


				if(conn!=null && trans!=null)
					result = this.RunSP(conn, trans, "DN_EmployeeDetailsGetSP", parmArray);
				else
					result = this.RunSP("DN_EmployeeDetailsGetSP", parmArray);

				if(result>=0)
				{
					_fACTEmpeeNo = parmArray[1].Value.ToString();
					_branchNo = (short)parmArray[2].Value;
					_empName = (string)parmArray[3].Value;
					_maxrows = Convert.ToInt32(parmArray[4].Value);
					_commissionType = Convert.ToInt16(parmArray[5].Value);
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int SaveStaff (bool changePassword, short branchno, int empeeNo , string fACTEmpeeNo,
			string firstName, string lastName ,string employeeType, string password,
            string dutyFree, int maxRow, short loggedIn, bool defaultRate, Single upliftRate, int empeechange) //CR1117 courts person audit //IP - 22/03/11 - #3299 - Changed upliftRate from int to float
            
		{
			try
			{   
				parmArray = new SqlParameter[14];
				parmArray[0] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				parmArray[1] = new SqlParameter("@empeeNo", SqlDbType.Int);
				parmArray[1].Value = empeeNo;
				parmArray[2] = new SqlParameter("@fACTEmpeeNo", SqlDbType.NVarChar,4);
				parmArray[2].Value = fACTEmpeeNo;
				parmArray[3] = new SqlParameter("@firstName", SqlDbType.NVarChar, 50);
				parmArray[3].Value = firstName;
                parmArray[4] = new SqlParameter("@lastName", SqlDbType.NVarChar, 50);
                parmArray[4].Value = lastName;
				parmArray[5] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 20/05/08 - Credit Collections, changed from Char 1
				parmArray[5].Value = employeeType;
				parmArray[6] = new SqlParameter("@password", SqlDbType.NVarChar, 12);
				parmArray[6].Value = password;
				parmArray[7] = new SqlParameter("@maxRow", SqlDbType.Int);
				parmArray[7].Value = maxRow;
				parmArray[8] = new SqlParameter("@dutyFree", SqlDbType.Char, 1);
				parmArray[8].Value = dutyFree;
				parmArray[9] = new SqlParameter("@loggedin", SqlDbType.SmallInt);
				parmArray[9].Value = loggedIn;
				parmArray[10] = new SqlParameter("@defaultrate", SqlDbType.Bit);
				parmArray[10].Value = defaultRate;
                parmArray[11] = new SqlParameter("@upliftRate", SqlDbType.Float); //IP - 22/03/11 - #3299-LW73340- Changed upliftRate from int to float
                parmArray[11].Value = upliftRate;
                parmArray[12] = new SqlParameter("@empeeChange", SqlDbType.Int); //CR1117 empee audit
                parmArray[12].Value = empeechange;
                parmArray[13] = new SqlParameter("@changePassword", SqlDbType.Bit);
                parmArray[13].Value = changePassword;


				result = this.RunSP("DN_CourtspersonSaveSP", parmArray);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetSalesStaffByType(string employeeType, int branchno)
		{
			try
			{
				_salesstaff = new DataTable(TN.SalesStaff);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@employeeType", SqlDbType.NVarChar, 3);
				parmArray[0].Value = employeeType;
				parmArray[1] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[1].Value = branchno;

				result = this.RunSP("DN_EmployeeGetByTypeSP", parmArray, _salesstaff);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        //IP - Method that retrieves the Logon history for a selected employee
        public DataTable GetEmployeeLogonHistory(int empeeno)
        {
            try
            {
                _logonhistory = new DataTable(TN.LogonHistory);

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
                parmArray[0].Value = empeeno;

                result = this.RunSP("DN_EmployeeGetLogonHistorySP", parmArray, _logonhistory);
             
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            return _logonhistory; 
        }

		public int GetStaffandAllocationByType(int employeeType, int branchno)
		{
			try
			{
				_salesstaff = new DataTable(TN.SalesStaff);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@employeeType", SqlDbType.Int); //IP - 22/05/08 - Credit Collections - Need to cater for (3) character Employee Types.
				parmArray[0].Value = employeeType;
				parmArray[1] = new SqlParameter("@branchNo", SqlDbType.SmallInt);
				parmArray[1].Value = branchno;

				result = this.RunSP("DN_EmployeeGetAllocationByTypeSP", parmArray, _salesstaff);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int GetAllocatableStaffandTypes()
        {
            try
            {
                _employees = new DataTable();

                result = this.RunSP("CM_LoadAllocatetableStaffandTypesSP", _employees);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


		public void UpdateAllocCount(SqlConnection conn, SqlTransaction trans, int empeeNo, int count)
		{
			try
			{			
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@count", SqlDbType.Int);
				parmArray[1].Value = count;

				this.RunSP(conn, trans, "DN_StaffUpdateAllocCountSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SavePassword(SqlConnection conn, SqlTransaction trans, string password, int empeeNo)
		{
			try
			{			
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@password", SqlDbType.NVarChar, 12);
				parmArray[0].Value = password;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = empeeNo;

				this.RunSP(conn, trans, "DN_SetPassword", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public bool SetLoggedIn(string machineName, int employeeNo, bool loggedIn)
		{
			try
			{			
				parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@machineName", SqlDbType.VarChar, 32);
                parmArray[0].Value = machineName;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = employeeNo;
				parmArray[2] = new SqlParameter("@loggedin", SqlDbType.SmallInt);
				parmArray[2].Value = Convert.ToInt16(loggedIn);

				result = RunSP("DN_CourtsPersonSetLoggedInSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return Convert.ToBoolean(result);
		}

		public DEmployee()
		{

		}

		public short BranchNumber
		{
			get
			{
				return _branchNo;
			}
			set
			{
				_branchNo = value;
			}
		}

		public string EmployeeName
		{
			get
			{
				return _empName;
			}
			set
			{
				_empName = value;
			}
		}

		public string EmployeeType
		{
			get
			{
				return _empType;
			}
			set
			{
				_empType = value;
			}
		}

		public DateTime DatePassChge
		{
			get{return _datepasschge;}
			set{_datepasschge = value;}
		}

		public short commissionType
		{
			get{return _commissionType;}
			set{_commissionType = value;}
		}


		/// <summary>
		/// TerminateEmployee
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <returns>int</returns>
		/// 
		public int TerminateEmployee (SqlConnection conn, SqlTransaction trans, int empeeno, int empeechange) //CR1117 empee audit
		{
			int status = 0;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
                parmArray[1] = new SqlParameter("@empeechange", SqlDbType.Int);
                parmArray[1].Value = empeechange;
 
				 
				status = this.RunSP(conn, trans, "DN_EmployeeTerminateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return status;
		}

		/// <summary>
		/// GetDateLastAudit
		/// </summary>
		/// <param name="empeeno">int</param>
		/// <param name="dateLast">DateTime</param>
		/// <returns>DateTime</returns>
		/// 
		public DateTime GetDateLastAudit (int empeeno)
		{
			
			
			DateTime dateLast = DateTime.MinValue;
			
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;
				
				parmArray[1] = new SqlParameter("@dateLast", SqlDbType.DateTime);
				parmArray[1].Value = dateLast;
				parmArray[1].Direction = ParameterDirection.Output;
				 
				this.RunSP("DN_CourtsPersonGetDateLastAuditSP", parmArray);
	
				if(parmArray[1].Value!=DBNull.Value)
					dateLast = (DateTime)parmArray[1].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dateLast;
		}



		/// <summary>
		/// GetCashiersWithOutstandingPayments
		/// </summary>
		/// <param name="branchno">int</param>
		/// <returns>DataSet</returns>
		/// 
		public DataSet GetCashiersWithOutstandingPayments (short branchno)
		{
			DataSet ds = new DataSet();
			
			
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchno;
				 
				
				this.RunSP("DN_CourtsPersonGetOutstandingPaymentsSP", parmArray, ds);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return ds;
		}

		/// <summary>
		/// CashTillOpenLoadEmployee
		/// </summary>
		/// <param name="user">int</param>
		/// <returns>int</returns>
		/// 
		public int CashTillOpenLoadEmployee (int user, string tillid)
		{
			int status = 0;
			int empeeno = 0;

			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = user;

				parmArray[1] = new SqlParameter("@tillid", SqlDbType.VarChar, 16);
				parmArray[1].Value = tillid;

				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[2].Direction = ParameterDirection.Output;
				 
				status = this.RunSP("dn_cashtillopencheckempeeno", parmArray);

				if(DBNull.Value!=parmArray[2].Value)
					empeeno = (int)parmArray[2].Value;				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return empeeno;
		}

		public DataTable GetCashierOutstandingIncome (short branchNo)
		{
			DataTable dt = new DataTable(TN.CashierOutstandingIncome);
			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[0].Value = branchNo;
				 
				this.RunSP("DN_CashierOutstandingIncomeGetSP", parmArray, dt);			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		public DataTable GetCashierOutstandingIncomeByPayMethod (int empeeno, short branchno)
		{
			DataTable dt = new DataTable(TN.CashierOutstandingIncome);
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeno;

				parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
				parmArray[1].Value = branchno;
				 
				this.RunSP("DN_CashierOutstandingIncomeGetByPayMethodSP", parmArray, dt);			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			
			return dt;
		}

		public int LoadCommissionDetailsByType(string employeeType, int branchNo)
		{
			try
			{
				_salesstaff = new DataTable(TN.SalesStaff);

				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@branchno", SqlDbType.Int);
				parmArray[0].Value = branchNo;
				parmArray[1] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 3); //IP - 03/06/08 - Credit Collections - Need to cater for (3) character employee types.
				parmArray[1].Value = employeeType;

				result = this.RunSP("DN_EmployeeGetCommissionDetailsByTypeSP", parmArray, _salesstaff);
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}
	}
}
