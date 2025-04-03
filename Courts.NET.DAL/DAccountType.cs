using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Deals with data access to the accttype table
	/// </summary>
	public class DAccountType : DALObject
	{
		DataTable _accounttypes;

		/// <summary>
		/// returns the various account types
		/// </summary>
		/// <param name="parms"></param>
		/// <returns></returns>
		public int GetAccountTypes(XmlNode parms)
		{
			try
			{
				_accounttypes = new DataTable(TN.AccountType);

				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@branchCode", SqlDbType.SmallInt);
				parmArray[0].Value = Convert.ToInt16(parms.FirstChild.NextSibling.Attributes[Tags.Value].Value);

				result = this.RunSP("DN_AccountGetTypesSP", parmArray, _accounttypes);
				if(result==0)
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

		public DataTable AccountTypes
		{
			get
			{
				return _accounttypes;
			}
		}
		public DAccountType()
		{

		}
	}
}
