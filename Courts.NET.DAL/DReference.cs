using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DReferences.
	/// </summary>
	public class DReference: DALObject
	{
		private DataTable _referenceList;

		public int GetReferences(string accountNumber, string name)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;

				_referenceList = new DataTable(name);
				result = this.RunSP("DN_ProposalRefGetSP", parmArray, _referenceList);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetReferenceList(string custId, string name)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[0].Value = custId;

				_referenceList = new DataTable(name);
				result = this.RunSP("DN_ProposalRefListSP", parmArray, _referenceList);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}


		public int ClearReferences(SqlConnection conn, SqlTransaction trans, string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				result = this.RunSP(conn, trans, "DN_ProposalRefClearSP", parmArray);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}


		public int SaveReferences(SqlConnection conn, SqlTransaction trans, string accountNumber)
		{
			int refNo = 0;

			try
			{
				foreach (DataRow r in _referenceList.Rows)
				{
					// Seem to need to re-declare for each loop to avoid error
					parmArray = new SqlParameter[24];
					parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
					parmArray[1] = new SqlParameter("@refno", SqlDbType.Int);
					parmArray[2] = new SqlParameter("@name", SqlDbType.NVarChar,30);
					parmArray[3] = new SqlParameter("@surname", SqlDbType.NVarChar,35);
					parmArray[4] = new SqlParameter("@relation", SqlDbType.NVarChar,4);
					parmArray[5] = new SqlParameter("@yrsknown", SqlDbType.Int);
					parmArray[6] = new SqlParameter("@address1", SqlDbType.NVarChar,26);
					parmArray[7] = new SqlParameter("@address2", SqlDbType.NVarChar,26);
					parmArray[8] = new SqlParameter("@city", SqlDbType.NVarChar,26);
					parmArray[9] = new SqlParameter("@postcode", SqlDbType.NVarChar,10);
					parmArray[10] = new SqlParameter("@waddress1", SqlDbType.NVarChar,26);
					parmArray[11] = new SqlParameter("@waddress2", SqlDbType.NVarChar,26);
					parmArray[12] = new SqlParameter("@wcity", SqlDbType.NVarChar,26);
					parmArray[13] = new SqlParameter("@wpostcode", SqlDbType.NVarChar,10);
					parmArray[14] = new SqlParameter("@telcode", SqlDbType.NVarChar,8);
					parmArray[15] = new SqlParameter("@tel", SqlDbType.NVarChar,13);
					parmArray[16] = new SqlParameter("@wtelcode", SqlDbType.NVarChar,8);
					parmArray[17] = new SqlParameter("@wtel", SqlDbType.NVarChar,13);
					parmArray[18] = new SqlParameter("@mtelcode", SqlDbType.NVarChar,8);
					parmArray[19] = new SqlParameter("@mtel", SqlDbType.NVarChar,13);
					parmArray[20] = new SqlParameter("@directions", SqlDbType.NVarChar,300);
					parmArray[21] = new SqlParameter("@comment", SqlDbType.NVarChar,300);
					parmArray[22] = new SqlParameter("@empeeno", SqlDbType.Int);
					parmArray[23] = new SqlParameter("@datechecked", SqlDbType.DateTime);

					parmArray[0].Value = accountNumber;

					refNo = refNo + 1;
					parmArray[1].Value = refNo;
					parmArray[2].Value = r[CN.RefFirstName];
					parmArray[3].Value = r[CN.RefLastName];
					parmArray[4].Value = r[CN.RefRelation];
					parmArray[5].Value = r[CN.YrsKnown];
					parmArray[6].Value = r[CN.RefAddress1];
					parmArray[7].Value = r[CN.RefAddress2];
					parmArray[8].Value = r[CN.RefCity];
					parmArray[9].Value = r[CN.RefPostCode];
					parmArray[10].Value = r[CN.RefWAddress1];
					parmArray[11].Value = r[CN.RefWAddress2];
					parmArray[12].Value = r[CN.RefWCity];
					parmArray[13].Value = r[CN.RefWPostCode];
					parmArray[14].Value = r[CN.RefDialCode];
					parmArray[15].Value = r[CN.RefPhoneNo];
					parmArray[16].Value = r[CN.RefWDialCode];
					parmArray[17].Value = r[CN.RefWPhoneNo];
					parmArray[18].Value = r[CN.RefMDialCode];
					parmArray[19].Value = r[CN.RefMPhoneNo];
					parmArray[20].Value = r[CN.RefDirections];
					
					if (r[CN.NewComment].ToString().Trim().Length > 0)
					{
						// Get Employee name to audit comments
						DEmployee employee = new DEmployee();
						employee.GetEmployeeDetails(conn, trans, this.User);
						parmArray[21].Value = employee.EmployeeName + " (" + this.User + ") - " + DateTime.Now + " : " + Environment.NewLine + r[CN.NewComment] + Environment.NewLine + Environment.NewLine + r[CN.RefComment];
					}
					else
						parmArray[21].Value = r[CN.RefComment];
					
					parmArray[22].Value = r[CN.EmpeeNoChange];
					parmArray[23].Value = r[CN.DateChange];

					result = this.RunSP(conn, trans, "DN_ProposalRefSaveSP", parmArray);
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}



		public DataTable referenceList
		{
			get 
			{
				return _referenceList;
			}

			set 
			{
				_referenceList = value;
			}
		}

		public new int User
		{
			get{return _user;}
			set{_user = value;}
		}

		public DReference()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
