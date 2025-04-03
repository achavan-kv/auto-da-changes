using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// This class provides data access to all country specific 
	/// variables. 
	/// </summary>
	public class DCountry : DALObject
	{
		private DataTable _table;
 
		/// <summary>
		/// This method returns a table containing all fields in the 
		/// country table for a specific country code
		/// </summary>
		/// <param name="countryCode">Country code</param>
		/// <returns>Either Success or NotFound</returns>
		public int GetDefaults(string countryCode)
		{
			try
			{
				_table = new DataTable("Country");

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[0].Value = countryCode;

				result = this.RunSP("DN_CountryGetOptionsSP", parmArray, _table);

				if(result<0)
				{
					result = (int)Return.NotFound;
				}
				else
				{
					result = (int)Return.Success;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetDefaults(SqlConnection conn, SqlTransaction trans, string countryCode)
		{
			try
			{
				_table = new DataTable("Country");

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[0].Value = countryCode;

				if(conn!=null&&trans!=null)
					result = this.RunSP(conn, trans, "DN_CountryGetOptionsSP", parmArray, _table);
				else
					result = this.RunSP("DN_CountryGetOptionsSP", parmArray, _table);

				if(result<0)
				{
					result = (int)Return.NotFound;
				}
				else
				{
					result = (int)Return.Success;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public DCountry()
		{

		}

		public DataTable Table
		{
			get
			{
				return _table;
			}
		}

		/// <summary>
		/// GetMaintenanceParameters
		/// </summary>
		/// <param name="country">string</param>
		/// <returns>DataSet</returns>
		/// 
		public DataTable GetMaintenanceParameters (SqlConnection conn, SqlTransaction trans, string country)
		{
			DataTable dt = new DataTable(TN.CountryParameters);		
			
			try
			{
                //parmArray = new SqlParameter[0]; //1];
				
				//parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
				//parmArray[0].Value = country;
				 
                //if(conn!=null && trans!=null)				
                //    RunSP(conn, trans, "DN_CountryMaintenanceGetParametersSP", parmArray, dt);
                //else
                //    RunSP("DN_CountryMaintenanceGetParametersSP", parmArray, dt);	

                GetCountryParams(dt);       // #13147
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw;
			}
			
			return dt;
		}

		public void SaveCountryMaintenanceParameter(SqlConnection conn, SqlTransaction trans,
													string countryCode, int parameterID, string val, int user)
		{
			try
			{
				parmArray = new SqlParameter[4];
				
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
				parmArray[0].Value = countryCode;

				parmArray[1] = new SqlParameter("@parameterid", SqlDbType.Int);
				parmArray[1].Value = parameterID;

				parmArray[2] = new SqlParameter("@value", SqlDbType.NVarChar, 1500);
				parmArray[2].Value = val;
				
				parmArray[3] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[3].Value = user;
				
				if(conn!=null && trans!=null)				
					RunSP(conn, trans, "DN_CountryMaintenanceSaveSP", parmArray);
				else
					RunSP("DN_CountryMaintenanceSaveSP", parmArray);	
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public string GetDataBaseVersion()
		{
			string dbVersion = "";
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@dbversion", SqlDbType.NChar, 20);
				parmArray[0].Value = "";
				parmArray[0].Direction = ParameterDirection.Output;
				
				this.RunSP("DN_CountryGetDBVersionSP", parmArray);

				if(parmArray[0].Value!=DBNull.Value)
					dbVersion = (string)parmArray[0].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dbVersion;
		}

		public void SetSystemStatus(SqlConnection conn, SqlTransaction trans, string countryCode, 
										string status)
		{
			try
			{
				parmArray = new SqlParameter[2];
				
				parmArray[0] = new SqlParameter("@status", SqlDbType.NChar, 6);
				parmArray[0].Value = status;
				parmArray[1] = new SqlParameter("@country", SqlDbType.NChar, 1);
				parmArray[1].Value = countryCode;

				RunSP(conn, trans, "DN_CountrySetSystemStatusSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public bool CheckConfig(char country, int branch)
        {
            parmArray = new SqlParameter[2];


            parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 1);
            parmArray[0].Value = country;
            parmArray[1] = new SqlParameter("@branch", SqlDbType.Int);
            parmArray[1].Value = branch;

            return ReturnBool("CheckConfig", parmArray);
        }
	}
}