using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DFollUpAlloc.
	/// </summary>
	public class DFollUpAlloc : DALObject
	{
		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set {_acctno = value;}
		}

		private int _allocno = 0;
		public int AllocNo
		{
			get{return _allocno;}
		}

		private int _empeeno = 0;
		public int EmployeeNo
		{
			get{return _empeeno;}
			set {_empeeno = value;}
		}

		private DateTime _dateAlloc;
		public DateTime DateAllocated
		{
			get{return _dateAlloc;}
			set {_dateAlloc = value;}
		}

		private DateTime _dateDealloc;
		public DateTime DateDeallocated
		{
			get{return _dateDealloc;}
		}

		private decimal _allocArrears = 0;
		public decimal AllocatedArrears
		{
			get{return _allocArrears;}
		}

		private double _bailfee = 0;
		public double BailiffFee
		{
			get{return _bailfee;}
		}

		private string _allocPrtFlag = "";
		public string AllocPrintFlag
		{
			get{return _allocPrtFlag;}
		}

		private int _empeenoAlloc = 0;
		public int EmployeeNoAllocated
		{
			get{return _empeenoAlloc;}
		}

		private int _empeenoDealloc = 0;
		public int EmployeeNoDeallocated
		{
			get{return _empeenoDealloc;}
		}

		private DataTable _allocAccounts = null;
		public DataTable AllocAccounts
		{
			get{return _allocAccounts;}
		}

        private DataTable _allocHistory = null;
        public DataTable AllocHistory
        {
            get 
            {
                return _allocHistory;
            }
        }

		public void Populate(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@allocno", SqlDbType.Int);
				parmArray[1].Value = 0;
				parmArray[2] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[2].Value = 0;
				parmArray[3] = new SqlParameter("@datealloc", SqlDbType.DateTime);
				parmArray[4] = new SqlParameter("@datedealloc", SqlDbType.DateTime);
				parmArray[5] = new SqlParameter("@allocarrears", SqlDbType.Money);
				parmArray[5].Value = 0;
				parmArray[6] = new SqlParameter("@bailfee", SqlDbType.Float);
				parmArray[6].Value = 0;
				parmArray[7] = new SqlParameter("@allocprtflag", SqlDbType.NChar,1);
				parmArray[7].Value = "";
				parmArray[8] = new SqlParameter("@empeenoalloc", SqlDbType.Int);
				parmArray[8].Value = 0;
				parmArray[9] = new SqlParameter("@empeenodealloc", SqlDbType.Int);
				parmArray[9].Value = 0;

				foreach(SqlParameter p in parmArray)
					p.Direction = ParameterDirection.Output;

				parmArray[0].Direction = ParameterDirection.Input;
						
				this.RunSP(conn,trans,"DN_FollUpAllocGetSP", parmArray);

				if(parmArray[1].Value!=DBNull.Value)
					_allocno = (int)parmArray[1].Value;	
				if(parmArray[2].Value!=DBNull.Value)
					_empeeno = (int)parmArray[2].Value;	
				if(parmArray[3].Value!=DBNull.Value)
					_dateAlloc = (DateTime)parmArray[3].Value;	
				if(parmArray[4].Value!=DBNull.Value)
					_dateDealloc = (DateTime)parmArray[4].Value;	
				if(parmArray[5].Value!=DBNull.Value)
					_allocArrears = (decimal)parmArray[5].Value;	
				if(parmArray[6].Value!=DBNull.Value)
					_bailfee = (double)parmArray[6].Value;	
				if(parmArray[7].Value!=DBNull.Value)
					_allocPrtFlag = (string)parmArray[7].Value;	
				if(parmArray[8].Value!=DBNull.Value)
					_empeenoAlloc = (int)parmArray[8].Value;	
				if(parmArray[9].Value!=DBNull.Value)
					_empeenoDealloc = (int)parmArray[9].Value;	

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetFollowUpHistory(string accountNo)
        {
            _allocHistory = new DataTable("AllocHistory");

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
                parmArray[0].Value = accountNo;
						
                this.RunSP("DN_FollUpAllocHistoryGetSP", parmArray, _allocHistory);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

		public void DeAllocate(SqlConnection conn, SqlTransaction trans ,
			string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[1].Value =this.User;
				this.RunSP("DN_FollUpAllocdeallocateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	


        //IP - 08/10/09 - UAT(909)
        //public bool Save(SqlConnection conn, SqlTransaction trans ,
        //    string accountNo, int EmployeeNo, bool checkMaxAccounts)
        public void Save(SqlConnection conn, SqlTransaction trans,
            string accountNo, int EmployeeNo)
		{
			//bool rtnValue = false;
            
            try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = EmployeeNo;
				parmArray[2] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[2].Value = this.User;
						
				this.RunSP(conn, trans, "DN_FollUpAllocAddSP", parmArray);

                //if (checkMaxAccounts == true)
                //{
                //    //--CR 852 Checking courtsperson.MaxAccounts value
                //    DataTable dt = new DataTable();
                //    parmArray = new SqlParameter[1];
                //    parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
                //    parmArray[0].Value = EmployeeNo;
                //    this.RunSP(conn, trans, "CM_GetEmpeeAllocDetail", parmArray, dt);

                //    if (dt.Rows.Count > 0)
                //    {
                //        if (Convert.ToInt32(dt.Rows[0][CN.MaxAccounts]) == 0)
                //            rtnValue = true; //MaxAccounts is not set
                //        else if (Convert.ToInt32(dt.Rows[0][CN.AllocCount]) > Convert.ToInt32(dt.Rows[0][CN.MaxAccounts]))
                //            rtnValue = false; //AllocCount exceeds MaxAccounts
                //        else
                //            rtnValue = true;
                //    }
                //}
                //else
                //{
                //    rtnValue = true;
                //} 
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
            
            //return rtnValue;
		}

		public int GetAllocatedCourtsPerson(string accountNo,ref int empNo, ref string empType, ref string empName)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = -1;
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@empeetype", SqlDbType.NVarChar, 30);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@empeename", SqlDbType.NVarChar, 20);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;
						
				result = this.RunSP("DN_FollUpAllocGetEmpeeNoSP", parmArray);

				if(parmArray[1].Value!=DBNull.Value)
					empNo = (int)parmArray[1].Value;
				if(parmArray[2].Value!=DBNull.Value)
					empType = (string)parmArray[2].Value;
				if(parmArray[3].Value!=DBNull.Value)
					empName = (string)parmArray[3].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public void AutoDeallocate(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
						
				RunSP(conn, trans, "DN_FollUpAllocAutoDeallocateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

		public void GetAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch)
		{
			_allocAccounts = new DataTable(TN.Allocations);

			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
				parmArray[1] = new SqlParameter("@branchOrAcctFilter", SqlDbType.VarChar, 12);
				parmArray[1].Value = branchOrAcctFilter;
				parmArray[2] = new SqlParameter("@acctnosearch", SqlDbType.SmallInt);
				parmArray[2].Value = acctNoSearch;
						
				RunSP("DN_TelephoneActionLoadSP", parmArray, _allocAccounts);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

      public void GetStrategyAccountsAllocated(int empeeNo, string branchOrAcctFilter, short acctNoSearch, string strategy)
      {
         _allocAccounts = new DataTable(TN.Allocations);

         try
         {
            parmArray = new SqlParameter[4];
            parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
            parmArray[0].Value = empeeNo;
            parmArray[1] = new SqlParameter("@branchOrAcctFilter", SqlDbType.VarChar, 12);
            parmArray[1].Value = branchOrAcctFilter;
            parmArray[2] = new SqlParameter("@acctnosearch", SqlDbType.SmallInt);
            parmArray[2].Value = acctNoSearch;
            parmArray[3] = new SqlParameter("@strategy", SqlDbType.VarChar, 3);
            parmArray[3].Value = strategy;

            RunSP("DN_TelephoneActionLoadStrategyAccountsSP", parmArray, _allocAccounts);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

      public DataSet GetWorklistAccounts(int empeeNo, string worklist,bool viewTop500) //IP - 12/11/09 UAT5.2 (882) - added control to either return top 500 or all accounts
      {
         DataSet ds = new DataSet();
         try{
            parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
                parmArray[1] = new SqlParameter("@viewTop500", SqlDbType.Bit);
                parmArray[1].Value = viewTop500;
                parmArray[2] = new SqlParameter("@worklist", SqlDbType.VarChar, 10);
                parmArray[2].Value = worklist;
            //RunSP("DN_TelephoneActionLoadNewSP",parmArray, ds);
                RunSP("DN_TelephoneActionLoadAccountsSP", parmArray, ds); //NM & IP 
            

         }
         catch(SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return ds;
      }

      public DataTable GetLineItemsForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar,12);
            parmArray[0].Value = acct;
            RunSP("GetLineItemsForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;
      }

      public DataTable GetTransactionsForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            RunSP("GetTransactionsForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;
      }

      public DataTable GetStrategiesForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            RunSP("GetStrategiesForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;      
      }

      public DataTable GetWorklistsForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            RunSP("GetWorklistsForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;      
      }

      public DataTable GetLettersForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            RunSP("GetLettersForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;      
      }

      public DataTable GetSMSForAWorklistAccount(string acct)
      {
         DataTable dt = new DataTable();
         try
         {
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
            parmArray[0].Value = acct;
            RunSP("GetSMSForAWorklistAccountSP", parmArray, dt);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
         return dt;      
      }

      public DataSet GetFollupAllocForActionSheet(string acctNo, int empeeNo)
      {
          DataSet ds = new DataSet();

          try
          {
              parmArray = new SqlParameter[2];
              parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
              parmArray[0].Value = empeeNo;
              parmArray[1] = new SqlParameter("@acctNo", SqlDbType.VarChar, 12);
              parmArray[1].Value = acctNo;

              this.RunSP("CM_GetFollupAllocForActionSheet", parmArray, ds);
          }
          catch (SqlException ex)
          {
              LogSqlException(ex);
              throw ex;
          }
          return ds;
      }

		public void getNewAllocations(int empeeNo)
		{
			_allocAccounts = new DataTable(TN.Allocations);

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
						
				RunSP("DN_FollUpAllocGetNewSP", parmArray, _allocAccounts);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

		public void UpdateDateAlloc(SqlConnection conn, SqlTransaction trans, int empeeNo)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = empeeNo;
						
				RunSP(conn, trans, "DN_FollUpAllocUpdateDateAllocSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

		public void LoadAllocationsForReprint(int months)
		{
			_allocAccounts = new DataTable(TN.Allocations);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = this.EmployeeNo;
				parmArray[1] = new SqlParameter("@months", SqlDbType.Int);
				parmArray[1].Value = months;
						
				RunSP("DN_FollUpAllocGetForRePrintSP", parmArray, _allocAccounts);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

		public void LoadAllocationDetails()
		{
			_allocAccounts = new DataTable(TN.Allocations);

			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = this.EmployeeNo;
				parmArray[1] = new SqlParameter("@datealloc", SqlDbType.DateTime);
				parmArray[1].Value = this.DateAllocated;
						
				RunSP("DN_FollUpAllocGetCustDetailsSP", parmArray, _allocAccounts);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

		public void UpdateAllocForReprint(SqlConnection conn,SqlTransaction trans,bool batch)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[1].Value = this.EmployeeNo;
				parmArray[2] = new SqlParameter("@datealloc", SqlDbType.DateTime);
				parmArray[2].Value = this.DateAllocated;
				parmArray[3] = new SqlParameter("@batch", SqlDbType.Bit);
				parmArray[3].Value = batch;
						
				RunSP(conn, trans, "DN_FollUpAllocUpdateForReprintSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}	
		}

        //IP - 08/10/09 - UAT(909) - Method which checks if accounts can be re-allocated to the selected employee.
        public bool CheckCanReallocate(SqlConnection conn, SqlTransaction trans, int countAcctsToRealloc, int EmployeeNo, ref int noCanAlloc)
        {
            bool canReallocate = false;

            try
            {
                
                    //--CR 852 Checking courtsperson.MaxAccounts value
                    DataTable dt = new DataTable();
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@empeeNo", SqlDbType.Int);
                    parmArray[0].Value = EmployeeNo;
                    this.RunSP(conn, trans, "CM_GetEmpeeAllocDetail", parmArray, dt);

                    if (dt.Rows.Count > 0)
                    {
                        if (countAcctsToRealloc <= Convert.ToInt32(dt.Rows[0][CN.MaxAccounts]) - Convert.ToInt32(dt.Rows[0][CN.AllocCount])
                            || Convert.ToInt32(dt.Rows[0][CN.MaxAccounts]) == 0)
                        {
                            canReallocate = true;
                        }
                    }

                    //Return the no of accounts that can be allocated to the employee.
                    noCanAlloc = Convert.ToInt32(dt.Rows[0][CN.MaxAccounts]) - Convert.ToInt32(dt.Rows[0][CN.AllocCount]);
                    
                    //If negative set to 0
                    if (noCanAlloc < 0)
                    {
                        noCanAlloc = 0;
                    }
              
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return canReallocate;
        }

		public DFollUpAlloc()
		{
		}

        public DataTable WorklistCustomerGetAllAccountsSP(string acct)
        {
            DataTable dt = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acct;
                RunSP("WorklistCustomerGetAllAccountsSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }
	}
}
