using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BTransType.
	/// </summary>
	public class BTransType
	{
		public BTransType()
		{
		}

		public DataSet GetTranstypeByCode(string transTypeCode)
		{
			DTransType tt = new DTransType();
			tt.TransTypeCode = transTypeCode;
			tt.LoadTransTypes();
			DataSet ds = new DataSet();

			ds.Tables.Add(tt.TransTypes);
			return ds;
		}

		public void Save(SqlConnection conn, SqlTransaction trans,string transTypeCode, string description,
						short includeINGFT, string intfaceSecAccount, string intfaceAccount, 
						short branchSplit, short isDeposit, string intfaceBalancing, 
						short isMandatory, short isUnique, int user, 
						string intfaceSecBalancing, short branchSplitBalancing,
                        string scInterfaceAccount, string scInterfaceBalancing)                             //IP - 11/04/12 - CR9863 - #9885
		{
			DTransType tt = new DTransType();
			tt.TransTypeCode = transTypeCode;
			tt.Description = description;
			tt.IncludeINGFT = includeINGFT;
			tt.InterfaceSecAccount = intfaceSecAccount;
			tt.InterfaceAccount = intfaceAccount;
			tt.BranchSplit = branchSplit;
			tt.IsDeposit = isDeposit;
			tt.InterfaceBalancing = intfaceBalancing;
			tt.IsMandatory = isMandatory;
			tt.IsUnique = isUnique;
			tt.User = user;
			tt.InterfaceSecBalancing = intfaceSecBalancing;
			tt.BranchSplitBalancing = branchSplitBalancing;
            tt.SCInterfaceAccount = scInterfaceAccount;                                                      //IP - 11/04/12 - CR9863 - #9885
            tt.SCInterfaceBalancing = scInterfaceBalancing;                                                  //IP - 11/04/12 - CR9863 - #9885
			tt.Save(conn, trans);
		}

	}
}
