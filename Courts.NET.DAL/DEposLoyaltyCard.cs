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
	/// Summary description for DEposLoyaltyCard.
	/// </summary>
	public class DEposLoyaltyCard : DALObject
	{
		public DEposLoyaltyCard()
		{
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="transrefno">int</param>
		/// <param name="datetrans">DateTime</param>
		/// <param name="acctno">string</param>
		/// <param name="morerewardsno">string</param>
		/// <param name="agreementno">int</param>
		/// <returns>void</returns>
		/// 
		public void Save (SqlConnection conn, SqlTransaction trans, 
							int transrefno, DateTime datetrans, 
							string acctno, string morerewardsno, int agreementno)
		{
			try
			{
				parmArray = new SqlParameter[5];
				
				parmArray[0] = new SqlParameter("@transrefno", SqlDbType.Int);
				parmArray[0].Value = transrefno;
				
				parmArray[1] = new SqlParameter("@datetrans", SqlDbType.DateTime);
				parmArray[1].Value = datetrans;
				
				parmArray[2] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[2].Value = acctno;
				
				parmArray[3] = new SqlParameter("@morerewardsno", SqlDbType.NVarChar, 16);
				parmArray[3].Value = morerewardsno;
				
				parmArray[4] = new SqlParameter("@agreementno", SqlDbType.Int);
				parmArray[4].Value = agreementno;
				 
				if(conn!=null && trans!=null)
					this.RunSP(conn, trans, "DN_EposLoyaltyCardInsertSP", parmArray);
				else
					this.RunSP("DN_EposLoyaltyCardInsertSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}