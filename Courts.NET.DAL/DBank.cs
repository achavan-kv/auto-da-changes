using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DBank.
	/// </summary>
	public class DBank:DALObject
	{
		private DataTable _table;

		private string _bankacctno = "";
		public string BankAccountNo
		{
			get{return _bankacctno;}
			set{_bankacctno = value;}
		}
		private string _bankcode = "";
		public string BankCode
		{
			get{return _bankcode;}
			set{_bankcode = value;}
		}
		private DateTime _dateopened;
		public DateTime DateOpened
		{
			get{return _dateopened;}
			set{_dateopened = value;}
		}
		private string _code = "";
		public string Code
		{
			get{return _code;}
			set{_code = value;}
		}
		private string _custid = "";
		public string CustomerID
		{
			get{return _custid;}
			set{_custid = value;}
		}
		private bool _ismandate = false;
		public bool IsMandate
		{
			get{return _ismandate;}
			set{_ismandate = value;}
		}
		private int _duedayid = 0;
		public int DueDayId
		{
			get{return _duedayid;}
			set{_duedayid = value;}
		}
		private string _bankacctname = "";
		public string BankAccountName
		{
			get{return _bankacctname;}
			set{_bankacctname = value;}
		}
		private string _acctno = "";
		public string AccountNumber
		{
			get{return _acctno;}
			set{_acctno = value;}
		}
		private string _bankname = "";
		public string BankName
		{
			get{return _bankname;}
			set{_bankname = value;}
		}

		public DataTable GetRow(string name)
		{
			DataTable dt = new DataTable(name);
			dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustomerID),
													 new DataColumn(CN.BankAccountNo),
													 new DataColumn(CN.BankCode),
													 new DataColumn(CN.BankAccountOpened, Type.GetType("System.DateTime")),
													 new DataColumn(CN.Code),
													 new DataColumn(CN.IsMandate, Type.GetType("System.Boolean")),
													 new DataColumn(CN.DueDayId, Type.GetType("System.Int32")),
													 new DataColumn(CN.BankAccountName),
													 new DataColumn(CN.BankName)});
			dt.Rows.Add(new object[] { this.CustomerID, 
										 this.BankAccountNo,
										 this.BankCode,
										 this.DateOpened,
										 this.Code,
										 this.IsMandate,
										 this.DueDayId,
										 this.BankAccountName,
										 this.BankName});
			return dt;
		}

		public int GetBankCodes()
		{
			try
			{			
				_table = new DataTable(TN.Bank);
				result = this.RunSP("DN_CodesGetBankSP", _table);
				if(result == 0)
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

		public int GetAccountDetails(string customerID, string accountNo)
		{
			try
			{
				CustomerID = customerID;
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = accountNo;
				parmArray[2] = new SqlParameter("@bankacctno", SqlDbType.NVarChar, 20);
				parmArray[2].Value = this.BankAccountNo;
				parmArray[3] = new SqlParameter("@bankcode", SqlDbType.NVarChar, 6);
				parmArray[3].Value = this.BankCode;
				parmArray[4] = new SqlParameter("@dateopened", SqlDbType.DateTime);
				parmArray[4].Value = this.DateOpened;
				parmArray[5] = new SqlParameter("@code", SqlDbType.NChar, 1);
				parmArray[5].Value = this.Code;
				parmArray[6] = new SqlParameter("@ismandate", SqlDbType.SmallInt);
				parmArray[6].Value = this.IsMandate;
				parmArray[7] = new SqlParameter("@duedayid", SqlDbType.Int);
				parmArray[7].Value = this.DueDayId;
				parmArray[8] = new SqlParameter("@acctname", SqlDbType.NVarChar, 40);
				parmArray[8].Value = this.BankAccountName;
				parmArray[9] = new SqlParameter("@bankname", SqlDbType.NVarChar, 20);
				parmArray[9].Value = this.BankName;

				foreach(SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;
				parmArray[0].Direction = ParameterDirection.Input;
				parmArray[1].Direction = ParameterDirection.Input;

				result = this.RunSP("DN_BankAccountGetSP", parmArray);
				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[2].Value))
						this.BankAccountNo = (string)parmArray[2].Value;
					if(!Convert.IsDBNull(parmArray[3].Value))
						this.BankCode = (string)parmArray[3].Value;
					if(!Convert.IsDBNull(parmArray[4].Value))
						this.DateOpened = (DateTime)parmArray[4].Value;
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.Code = (string)parmArray[5].Value;
					if(!Convert.IsDBNull(parmArray[6].Value))
						this.IsMandate = Convert.ToBoolean(parmArray[6].Value);
					if(!Convert.IsDBNull(parmArray[7].Value))
						this.DueDayId = (int)parmArray[7].Value;
					if(!Convert.IsDBNull(parmArray[8].Value))
						this.BankAccountName = (string)parmArray[8].Value;
					if(!Convert.IsDBNull(parmArray[9].Value))
						this.BankName = (string)parmArray[9].Value;
					result = (int)Return.Success;
				}
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

		public int Save(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo)
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@bankacctno", SqlDbType.NVarChar, 20);
				parmArray[1].Value = this.BankAccountNo;
				parmArray[2] = new SqlParameter("@bankcode", SqlDbType.NVarChar,6);
				parmArray[2].Value = this.BankCode;
				parmArray[3] = new SqlParameter("@dateopened", SqlDbType.DateTime);
				parmArray[3].Value = this.DateOpened;
				parmArray[4] = new SqlParameter("@code", SqlDbType.NChar, 1);
				parmArray[4].Value = this.Code;
				parmArray[5] = new SqlParameter("@ismandate", SqlDbType.SmallInt);
				parmArray[5].Value = Convert.ToInt16(this.IsMandate);
				parmArray[6] = new SqlParameter("@duedayid", SqlDbType.Int);
				parmArray[6].Value = this.DueDayId;
				parmArray[7] = new SqlParameter("@acctname", SqlDbType.NVarChar, 40);
				parmArray[7].Value = this.BankAccountName;
				parmArray[8] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[8].Value = accountNo;
				
				result = this.RunSP(conn, trans, "DN_BankAccountSaveSP", parmArray);

				if(result == 0)
					result = (int)Return.Success;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public int GetBankDetails()
        {
            _table = new DataTable(TN.Bank);

            try
            {
                result = this.RunSP("DN_BankGetSP", _table);
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

        //Method to update details of a Bank
        public int UpdateBank(SqlConnection conn, SqlTransaction trans, string bankcode, string bankname, 
            string bankaddr1, string bankaddr2, string bankaddr3, string bankpocode)
        {
            int status = 0;

            try
            {
                parmArray = new SqlParameter[6];

                parmArray[0] = new SqlParameter("@bankcode", SqlDbType.VarChar, 6);
                parmArray[0].Value = bankcode;

                parmArray[1] = new SqlParameter("@bankname", SqlDbType.VarChar, 20);
                parmArray[1].Value = bankname;

                parmArray[2] = new SqlParameter("@bankaddr1", SqlDbType.VarChar, 26);
                parmArray[2].Value = bankaddr1;

                parmArray[3] = new SqlParameter("@bankaddr2", SqlDbType.VarChar, 26);
                parmArray[3].Value = bankaddr2;

                parmArray[4] = new SqlParameter("@bankaddr3", SqlDbType.VarChar, 26);
                parmArray[4].Value = bankaddr3;

                parmArray[5] = new SqlParameter("@bankpocode", SqlDbType.VarChar, 10);
                parmArray[5].Value = bankpocode;

                status = this.RunSP(conn, trans, "DN_BankUpdateSP", parmArray);
            }

            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

        //Method to delete Bank details
        public int DeleteBank(SqlConnection conn, SqlTransaction trans, string bankcode)
        {
            int status = 0;

            try
            {
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@bankcode", SqlDbType.VarChar, 6);
                parmArray[0].Value = bankcode;

                status = this.RunSP(conn, trans, "DN_BankDeleteSP", parmArray);  
            }

            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return status;
        }

		public DataTable Table
		{
			get 
			{
				return _table;
			}
		}
		public DBank()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
