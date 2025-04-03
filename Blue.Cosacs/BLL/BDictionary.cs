using System;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using STL.Common;
using STL.DAL;
using STL.Common.Constants.ColumnNames;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BDictionary.
	/// </summary>
	public class BDictionary : CommonObject
	{
		public BDictionary()
		{
		}

		public DataSet GetDictionary(string culture)
		{
			DataSet ds = new DataSet();
			DDictionary d = new DDictionary();
			d.GetDictionary(culture);
			ds.Tables.Add(d.Dictionary);
			return ds;
		}

		public void SaveDictionary(SqlConnection conn, SqlTransaction trans, string culture, DataSet ds)
		{
			DDictionary d = new DDictionary();
			//d.DeleteDictionary(conn, trans, culture);
            foreach(DataTable dt in ds.Tables)
			{
				DataTable mod = dt.GetChanges(DataRowState.Modified);
				if(mod!=null)
				{
					foreach(DataRow r in mod.Rows)
					{
						if(r[CN.Key]==DBNull.Value)
						{
							if(r[CN.Translation]==DBNull.Value)
								r[CN.Translation] = "";

							d.Write(conn, trans,
								(string)r[CN.Culture],
								(string)r[CN.English],
								(string)r[CN.Translation]);
						}
						else
						{
							d.Write(conn, trans,
								(string)r[CN.Culture],
								(string)r[CN.Key],
								(string)r[CN.Translation]);
						}
					}
				}
			}
		}
	}
}
