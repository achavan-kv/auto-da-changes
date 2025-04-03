using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DDictionary.
	/// </summary>
	public class DDictionary : DALObject
	{
		private DataTable _dictionary = null;
		public DataTable Dictionary
		{
			get{return _dictionary;}
			set{_dictionary = value;}
		}

		public void GetDictionary(string culture)
		{
			try
			{
				_dictionary = new DataTable();
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@culture", SqlDbType.NVarChar,10);
				parmArray[0].Value = culture;

				this.RunSP("DN_DictionaryGetSP", parmArray, _dictionary);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void DeleteDictionary(SqlConnection conn, SqlTransaction trans, string culture)
		{
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@culture", SqlDbType.NVarChar,10);
				parmArray[0].Value = culture;

				this.RunSP(conn, trans, "DN_DictionaryDeleteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void Write(SqlConnection conn, SqlTransaction trans, string culture, string english, string translation)
		{
			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@culture", SqlDbType.NVarChar,10);
				parmArray[0].Value = culture;
				parmArray[1] = new SqlParameter("@english", SqlDbType.NVarChar, 300);
				parmArray[1].Value = english;
				parmArray[2] = new SqlParameter("@translation", SqlDbType.NText);
				parmArray[2].Value = translation;

				this.RunSP(conn, trans, "DN_DictionaryWriteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DDictionary()
		{
		}
	}
}
