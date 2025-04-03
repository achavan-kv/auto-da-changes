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
	/// Summary description for DMethodOfPayment.
	/// </summary>
	public class DMethodOfPayment : DALObject
	{
		private DataTable _mop;

		public int GetMOPSummary()
		{
			try
			{
				_mop = new DataTable(TN.MethodOfPayment);

				result = this.RunSP("DN_MethodOfPaymentSummaryGetSP", _mop);
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

		public DataTable MOP
		{
			get
			{
				return _mop;
			}
		}
		public DMethodOfPayment()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
