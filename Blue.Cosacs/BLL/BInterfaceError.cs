using System;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ItemTypes;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BInterfaceError.
	/// </summary>
	public class BInterfaceError : CommonObject
	{
		public BInterfaceError(SqlConnection conn, SqlTransaction trans, string _interface, int runno, DateTime errorDate,
								string errorText, string severity)
		{
            DInterfaceError ie = new DInterfaceError();	
			ie.Interface = _interface;
			ie.RunNumber = runno;
			ie.ErrorDate = errorDate;
			ie.ErrorText = errorText;
			ie.Severity = severity;
			ie.Write(conn, trans);
		}

        public DataSet GetInterfaceError(string eodInterface, int runno, DateTime startdate)
		{
			DataSet ds = new DataSet();
			DInterfaceError ifError = new DInterfaceError();
			ifError.Interface = eodInterface;
			ifError.RunNumber = runno;
            ifError.StartDate = startdate;      //jec 06/04/11
			ifError.GetInterfaceError();
			ds.Tables.Add(ifError.Control);

			return ds;
		}

		public BInterfaceError()
		{
		}
	}
}
