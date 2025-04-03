using System;
using STL.DAL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Categories;
using System.Data;
using System.Data.SqlClient;
using STL.Common.PrivilegeClub;

namespace STL.BLL
{
	/// <summary>
	/// Maintains the code table
	/// </summary>
	public class BCode : CommonObject
	{
		DCode code = null;

		/// <summary>
		/// Returns all codes and code categories
		/// </summary>
		/// <returns>DataSet of two DataTables</returns>
		public DataSet GetAllCodesAndCategories()
		{
			DataSet ds = new DataSet();
			code.GetAllCodesAndCategories();
			ds.Tables.Add(code.Codes.Copy()); 
			ds.Tables.Add(code.Categories.Copy()); 
			return ds;
		}

		public int Delete(SqlConnection conn, SqlTransaction trans, 
			string Code, string Category)
		{
			if ((bool)Country[CountryParameterNames.TierPCEnabled]
				&& Category == CAT.CustomerCode1
				&& (Code == PCCustCodes.Tier1 || Code == PCCustCodes.Tier2))
			{
				// Cannot delete the Tier1/2 Privilege Club customer codes
				throw new STLException(GetResource("M_PRIVCLUBCODEDELETE", new object [] {PCCustCodes.Tier1, PCCustCodes.Tier2}));
			}
			else return code.Delete(conn, trans, Code, Category);
		}

		public int Update(SqlConnection conn, SqlTransaction trans, DataSet Changes)
		{
			int status = 0;

			foreach(DataRow row in Changes.Tables[0].Rows)
			{
				int SortOrder = 0;
				string Code = "";
				string Category = "";
				string OldCode = "";
				string OldCategory = "";
				string CodeDescript = "";
				string Reference = "";
			    string Additional = "";
                string Additional2 = "";                                //IP - 07/12/11 - CR1234
                bool IsMmiApplicable = false;

				if (row[CN.Code] != DBNull.Value)
					Code = (string)row[CN.Code];

				if (row[CN.OldCode] != DBNull.Value)
					OldCode = (string)row[CN.OldCode];

				if (row[CN.Category] != DBNull.Value)
					Category = (string)row[CN.Category];

				if (row[CN.OldCategory] != DBNull.Value)
					OldCategory = (string)row[CN.OldCategory];

				if (row[CN.CodeDescript] != DBNull.Value)
					CodeDescript = (string)row[CN.CodeDescript];

				if (row[CN.SortOrder] != DBNull.Value)
					SortOrder = Convert.ToInt32(row[CN.SortOrder]);

				if (row[CN.Reference] != DBNull.Value)
					Reference = (string)row[CN.Reference];

                if (row[CN.Additional] != DBNull.Value)
                    Additional = (string)row[CN.Additional];

                if (row[CN.Additional2] != DBNull.Value)            //IP - 07/12/11 - CR1234
                    Additional2 = (string)row[CN.Additional2];

                if (row[CN.MmiApplicable] != DBNull.Value)            //IP - 07/12/11 - CR1234
                    IsMmiApplicable = (bool)row[CN.MmiApplicable];

                status = code.Update(conn, trans, Code, Category, OldCode, OldCategory, CodeDescript, SortOrder, Reference,Additional, Additional2, IsMmiApplicable);        //IP - 07/12/11 - CR1234
				
				if (status != 0)
					break;
			}

			return status;			
		}

		public BCode()
		{
			code = new DCode();
		}
	}
}



