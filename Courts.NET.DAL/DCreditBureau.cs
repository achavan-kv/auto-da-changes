using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DCreditBureau.
	/// </summary>
	public class DCreditBureau : DALObject
	{
		public DCreditBureau()
		{
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="custid">string</param>
		/// <returns>DataTable</returns>
		/// 
		public DataTable Get (string custid)
		{
			DataTable dt = new DataTable(TN.CreditBureau);

			try
			{
				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;				 
				
				this.RunSP("DN_CreditBureauSelectSP", parmArray, dt);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		/// <summary>
		/// GetLastRequest
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="datelast">DateTime</param>
		/// <returns>DateTime</returns>
		/// 
		public DateTime GetLastRequest (SqlConnection conn, 
										SqlTransaction trans, string custid, string source)
		{			
			DateTime datelast = DateTime.MinValue.AddYears(1899);
			
			try
			{
            parmArray = new SqlParameter[3];

            parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
            parmArray[0].Value = custid;

            parmArray[1] = new SqlParameter("@source", SqlDbType.NChar, 1);
            parmArray[1].Value = source;

            parmArray[2] = new SqlParameter("@datelast", SqlDbType.DateTime);
            parmArray[2].Value = datelast;
            parmArray[2].Direction = ParameterDirection.Output;

            this.RunSP(conn, trans, "DN_CreditBureauGetLastRequestSP", parmArray);

            if (parmArray[2].Value != DBNull.Value)
               datelast = (DateTime)parmArray[2].Value;
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return datelast;
		}

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="scoredate">DateTime</param>
		/// <param name="responsexml">string</param>
		/// <param name="lawsuittimesincelast">int</param>
		/// <param name="lawsuits">int</param>
		/// <param name="lawsuits12months">int</param>
		/// <param name="lawsuitsavgvalue">double</param>
		/// <param name="lawsuitstotalvalue">double</param>
		/// <param name="bankruptcies">int</param>
		/// <param name="bankruptcies12months">int</param>
		/// <param name="bankruptciesavgvalue">double</param>
		/// <param name="bankruptciestotalvalue">double</param>
		/// <param name="previousenquiries">int</param>
		/// <param name="previousenquiriestotalvalue">double</param>
		/// <param name="previousenquiriesavgvalue">double</param>
		/// <param name="previousenquiries12months">int</param>
		/// <param name="previousenquiriesavgvalue12months">double</param>
		/// <returns>void</returns>
		/// 
		public void Save (SqlConnection conn, SqlTransaction trans, 
						string custid, DateTime scoredate, 
						string responsexml, short lawsuittimesincelast, 
						short bankruptciestimesincelast, 
						short lawsuits, short lawsuits12months, short lawsuits24months,
						decimal lawsuitsavgvalue, decimal lawsuitstotalvalue, 
						short bankruptcies, short bankruptcies12months, short bankruptcies24months,
						decimal bankruptciesavgvalue, decimal bankruptciestotalvalue, 
						short previousenquiries, decimal previousenquiriestotalvalue, 
						decimal previousenquiriesavgvalue, short previousenquiries12months,
                        decimal previousenquiriesavgvalue12months, string source) //CR 843 Added source
		{
			try
			{
                //CR 843 Added new parameter
				parmArray = new SqlParameter[21];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 40);
				parmArray[0].Value = custid;
				
				parmArray[1] = new SqlParameter("@scoredate", SqlDbType.DateTime);
				parmArray[1].Value = scoredate;
				
				parmArray[2] = new SqlParameter("@responsexml", SqlDbType.NText);
				parmArray[2].Value = responsexml;
				
				parmArray[3] = new SqlParameter("@lawsuittimesincelast", SqlDbType.SmallInt);
				parmArray[3].Value = lawsuittimesincelast;
				
				parmArray[4] = new SqlParameter("@bankruptciestimesincelast", SqlDbType.SmallInt);
				parmArray[4].Value = bankruptciestimesincelast;

				parmArray[5] = new SqlParameter("@lawsuits", SqlDbType.SmallInt);
				parmArray[5].Value = lawsuits;
				
				parmArray[6] = new SqlParameter("@lawsuits12months", SqlDbType.SmallInt);
				parmArray[6].Value = lawsuits12months;

				parmArray[7] = new SqlParameter("@lawsuits24months", SqlDbType.SmallInt);
				parmArray[7].Value = lawsuits24months;
				
				parmArray[8] = new SqlParameter("@lawsuitsavgvalue", SqlDbType.Money);
				parmArray[8].Value = lawsuitsavgvalue;
				
				parmArray[9] = new SqlParameter("@lawsuitstotalvalue", SqlDbType.Money);
				parmArray[9].Value = lawsuitstotalvalue;
				
				parmArray[10] = new SqlParameter("@bankruptcies", SqlDbType.SmallInt);
				parmArray[10].Value = bankruptcies;
				
				parmArray[11] = new SqlParameter("@bankruptcies12months", SqlDbType.SmallInt);
				parmArray[11].Value = bankruptcies12months;

				parmArray[12] = new SqlParameter("@bankruptcies24months", SqlDbType.SmallInt);
				parmArray[12].Value = bankruptcies24months;
				
				parmArray[13] = new SqlParameter("@bankruptciesavgvalue", SqlDbType.Money);
				parmArray[13].Value = bankruptciesavgvalue;
				
				parmArray[14] = new SqlParameter("@bankruptciestotalvalue", SqlDbType.Money);
				parmArray[14].Value = bankruptciestotalvalue;
				
				parmArray[15] = new SqlParameter("@previousenquiries", SqlDbType.SmallInt);
				parmArray[15].Value = previousenquiries;
				
				parmArray[16] = new SqlParameter("@previousenquiriestotalvalue", SqlDbType.Money);
				parmArray[16].Value = previousenquiriestotalvalue;
				
				parmArray[17] = new SqlParameter("@previousenquiriesavgvalue", SqlDbType.Money);
				parmArray[17].Value = previousenquiriesavgvalue;
				
				parmArray[18] = new SqlParameter("@previousenquiries12months", SqlDbType.SmallInt);
				parmArray[18].Value = previousenquiries12months;
				
				parmArray[19] = new SqlParameter("@previousenquiriesavgvalue12months", SqlDbType.Money);
				parmArray[19].Value = previousenquiriesavgvalue12months;

                //CR 843 Added this parameter
                parmArray[20] = new SqlParameter("@source", SqlDbType.NChar, 1);
                parmArray[20].Value = source;
                //End CR 843
				
				this.RunSP(conn, trans, "DN_CreditBureauSaveSP", parmArray);
	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}