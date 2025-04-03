using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
	/// <summary>
	/// Summary description for DProposalFlag.
	/// </summary>
	public class DProposalFlag : DALObject
	{
		private short _origbr = 0;
		public short OrigBr
		{
			get{return _origbr;}
			set{_origbr = value;}
		}
		private string _custid = "";
		public string CustomerID
		{
			get{return _custid;}
			set{_custid = value;}
		}
		private DateTime _dateprop;
		public DateTime DateProp
		{
			get{return _dateprop;}
			set{_dateprop = value;}
		}
		private string _checktype = "";
		public string CheckType
		{
			get{return _checktype;}
			set{_checktype = value;}
		}
		private DateTime _datecleared = DateTime.MinValue.AddYears(1899);
		public DateTime DateCleared
		{
			get{return _datecleared;}
			set{_datecleared = value;}
		}
		private int _empeeno = 0;
		public int EmployeeNoFlag
		{
			get{return _empeeno;}
			set{_empeeno = value;}
		}
		private string _currentStage = SS.S1;
		public string CurrentStage
		{
			get{return _currentStage;}
			set{_currentStage = value;}
		}

		private DataTable _propFlags = null;
		public DataTable ProposalFlags
		{
			get{return _propFlags;}
			set{_propFlags = value;}
		}

	
		public int Load(string accountNo, string customerID, DateTime dateProp, out bool holdProp, out string currentStatus)
		{
			try
			{
				_propFlags = new DataTable(TN.ProposalFlags);
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@holdprop", SqlDbType.NChar, 1);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@currentStatus", SqlDbType.NChar, 1);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[3].Value = customerID;
                parmArray[4] = new SqlParameter("@dateprop", SqlDbType.DateTime);
                parmArray[4].Value = dateProp;
				this.RunSP("DN_ProposalFlagGetSP", parmArray, _propFlags);
				holdProp = (string)parmArray[1].Value == "N"?false:true;
				currentStatus = (string)parmArray[2].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public void Save(SqlConnection conn, SqlTransaction trans, string acctno)
		{
			try
			{
				_propFlags = new DataTable(TN.ProposalFlags);
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
				parmArray[2].Value = this.DateProp;
				parmArray[3] = new SqlParameter("@checktype", SqlDbType.NVarChar, 4);
				parmArray[3].Value = this.CheckType;
				parmArray[4] = new SqlParameter("@datecleared", SqlDbType.DateTime);
				if(this.DateCleared==DateTime.MinValue.AddYears(1899))
					parmArray[4].Value = DBNull.Value;
				else
					parmArray[4].Value = this.DateCleared;
				parmArray[5] = new SqlParameter("@empeenopflg", SqlDbType.Int);
				parmArray[5].Value = this.EmployeeNoFlag;
                parmArray[6] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[6].Value = acctno;
				this.RunSP(conn, trans, "DN_ProposalFlagSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
// ip 11/10/07 updating proposalflag routine not used.  If reinstated add param acctno
//this procedure adds proposal flags which will be required to be cleared in DA
        //public void AddDA(SqlConnection conn, SqlTransaction trans)
        //{
        //    try
        //    {
        //        parmArray = new SqlParameter[4];
        //        parmArray[0] = new SqlParameter("@dateprop", SqlDbType.SmallDateTime);
        //        parmArray[0].Value = this.DateProp;
        //        parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
        //        parmArray[1].Value = this.CustomerID;
        //        parmArray[2] = new SqlParameter("@empeenopflg", SqlDbType.Int);
        //        parmArray[2].Value = this.EmployeeNoFlag;
        //        parmArray[3] = new SqlParameter("@acctno", SqlDbType.Char, 12);

        //        //parmArray[3].Value = this
        //        this.RunSP(conn, trans, "DN_ProposalFlagAddDA", parmArray);
        //    }
        //    catch(SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //}






		public void UnClearFlag(SqlConnection conn, SqlTransaction trans,
								string accountNo, string checkType, bool changeStatus,int @user)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@checkType", SqlDbType.NVarChar, 5);
				parmArray[1].Value = checkType;
				parmArray[2] = new SqlParameter("@changeStatus", SqlDbType.SmallInt);
				parmArray[2].Value = Convert.ToInt16(changeStatus);
				parmArray[3] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[3].Value = Convert.ToInt32(user);

				this.RunSP(conn, trans, "DN_ProposalFlagUnClearFlagSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void Delete(SqlConnection conn, SqlTransaction trans, string acctno)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@checktype", SqlDbType.NVarChar, 4);
				parmArray[0].Value = this.CheckType;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[1].Value = acctno;


				this.RunSP(conn, trans, "DN_ProposalFlagDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void ClearAll(SqlConnection conn, SqlTransaction trans, string acctno)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[0].Value = this.User;
				parmArray[1] = new SqlParameter("@datecleared", SqlDbType.DateTime);
				parmArray[1].Value = DateTime.Now;
                parmArray[2] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[2].Value = acctno;

				this.RunSP(conn, trans, "DN_ProposalFlagClearAllSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DProposalFlag()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
