using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DReferral.
	/// </summary>
	public class DReferral : DALObject
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

		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private DateTime _dateprop = DateTime.MinValue.AddYears(1899);
		public DateTime DateProp
		{
			get{return _dateprop;}
			set{_dateprop = value;}
		}

		private string _refresult = "";
		public string ReferralResult
		{
			get{return _refresult;}
			set{_refresult = value;}
		}

		private int _empeeno = 0;
		public int EmployeeNo
		{
			get{return _empeeno;}
			set{_empeeno = value;}
		}

		private DateTime _dateref = DateTime.MinValue.AddYears(1899);
		public DateTime DateReferral
		{
			get{return _dateref;}
			set{_dateref = value;}
		}

		public void Update(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[0].Value = this.OrigBr;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				if(this.DateProp==DateTime.MinValue.AddYears(1899))
					parmArray[2].Value = DBNull.Value;
				else
					parmArray[2].Value = this.DateProp;
				parmArray[3] = new SqlParameter("@reflresult", SqlDbType.NChar, 1);
				parmArray[3].Value = this.ReferralResult;
				parmArray[4] = new SqlParameter("@empeeno", SqlDbType.Int);
				parmArray[4].Value = this.EmployeeNo;
				parmArray[5] = new SqlParameter("@datereferral", SqlDbType.DateTime);
				parmArray[5].Value = this.DateReferral;

				result = this.RunSP(conn, trans, "DN_ReferralUpdateSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DataSet GetReferralData()
		{
			DataSet ds = new DataSet();
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[2].Value = this.DateProp;

				this.RunSP("DN_ReferralGetDataSP", parmArray, ds);

				if (ds.Tables.Count > 1)
				{
					ds.Tables[0].TableName = TN.ReferralData;
					ds.Tables[1].TableName = TN.ReferralAudit;
				}

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return ds;
		}


		public DReferral()
		{
		}

		/// <summary>
		/// DeleteReferralRules
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="dateprop">DateTime</param>
		/// <returns>void</returns>
		/// 
		public void DeleteReferralRules (SqlConnection conn, SqlTransaction trans, 
						string custid, DateTime dateprop)
		{
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[1].Value = dateprop;
				 
				
				this.RunSP(conn, trans, "DN_ReferralRulesDeleteSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// WriteReferralRule
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="dateprop">DateTime</param>
		/// <param name="code">string</param>
		/// <returns>void</returns>
		/// 
		public void WriteReferralRule (SqlConnection conn, SqlTransaction trans, 
							string custid, DateTime dateprop, string code)
		{
			try
			{
				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[1].Value = dateprop;
				
				parmArray[2] = new SqlParameter("@code", SqlDbType.NVarChar, 10);
				parmArray[2].Value = code;
				 
				
				this.RunSP(conn, trans, "DN_ReferralRuleWriteSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		/// <summary>
		/// GetReferralRules
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="dateprop">DateTime</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable GetReferralRules (SqlConnection conn, 
											SqlTransaction trans, 
											string custid, 
											DateTime dateprop)
		{
			DataTable dt = new DataTable(TN.ReferralRules);

			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[1].Value = dateprop;
				 
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_ReferralRulesGetSP", parmArray, dt);
				else
					this.RunSP("DN_ReferralRulesGetSP", parmArray, dt);				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}
	}
}