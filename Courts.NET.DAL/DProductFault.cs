using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DProductFault.
	/// </summary>
	public class DProductFault : DALObject
	{
		public void Save(SqlConnection conn, SqlTransaction trans, 
						string accountNo, int agreementNo,
						string itemNo, string returnItemNo,
						string notes, string reason, DateTime dateCollection,
						short elapsedMonths, int newBuffNo)
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
				parmArray[1] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[1].Value = agreementNo;
                //parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar,8);
                //parmArray[2].Value = itemNo;
                //parmArray[3] = new SqlParameter("@returnitemno", SqlDbType.NVarChar,8);
                //parmArray[3].Value = returnItemNo;
                parmArray[2] = new SqlParameter("@itemno", SqlDbType.NVarChar, 18);                             //IP - 01/08/11 - RI
                parmArray[2].Value = itemNo;
                parmArray[3] = new SqlParameter("@returnitemno", SqlDbType.NVarChar, 18);                       //IP - 01/08/11 - RI
                parmArray[3].Value = returnItemNo;
				parmArray[4] = new SqlParameter("@notes", SqlDbType.NVarChar,2000);
				parmArray[4].Value = notes;
				parmArray[5] = new SqlParameter("@reason", SqlDbType.NVarChar,3);
				parmArray[5].Value = reason;
				parmArray[6] = new SqlParameter("@datecollection", SqlDbType.DateTime);
				parmArray[6].Value = dateCollection;
				parmArray[7] = new SqlParameter("@elapsedmonths", SqlDbType.SmallInt);
				parmArray[7].Value = elapsedMonths;
				parmArray[8] = new SqlParameter("@newbuffno", SqlDbType.Int);
				parmArray[8].Value = newBuffNo;

				if(conn!=null && trans!=null)
					RunSP(conn, trans, "DN_ProductFaultsSaveSP", parmArray);
				else
					RunSP("DN_ProductFaultsSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DProductFault()
		{
		}
	}
}
