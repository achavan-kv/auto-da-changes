using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DCustomerImage.
	/// </summary>
	public class DCustomerImage : DALObject
	{
		public DCustomerImage()
		{ 
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <param name="custid">string</param>
		/// <returns>Byte[]</returns>
		/// 
		public Byte[] Get (string custid)
		{
			Byte[] image = null;
			try
			{
				DataTable dt = new DataTable();

				parmArray = new SqlParameter[1];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;
				 
				RunSP("DN_CustomerImagesGetSP", parmArray, dt);

				foreach(DataRow r in dt.Rows)
					image = (Byte[])r[CN.Picture];			
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return image;
		}

		public void Update (string custid, byte[] image, DateTime date)
		{
			try
			{
				DataTable dt = new DataTable();

				parmArray = new SqlParameter[3];
				
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custid;
				parmArray[1] = new SqlParameter("@picture", SqlDbType.Image);
				parmArray[1].Value = image;
				parmArray[2] = new SqlParameter("@date", SqlDbType.DateTime);
				parmArray[2].Value = date;
				 
				RunSP("DN_CustomerImagesUpdateSP", parmArray);	
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
	}
}