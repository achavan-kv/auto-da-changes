using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;

namespace STL.BLL
{
    /// <summary>
    /// Business Logic Layer for Sets
    /// </summary>
    public class BSets : CommonObject
    {
        public DataSet GetSets(string setName,string tName)
        {
            DataSet ds = new DataSet();
            DSets dSets = new DSets();
            dSets.GetSets(setName,tName);
            ds.Tables.Add(dSets.SetsData);
            return ds;
        }

		public DataSet GetSetsForTName(string tName)
		{
			// Set contents returned that are NOT split by branch
			DataSet ds = new DataSet();
			DSets dSets = new DSets();
			dSets.GetSetsForTName(tName);
			ds.Tables.Add(dSets.SetsData);
			return ds;
		}

		public DataSet GetSetsForTNameBranch(string tName, short branchNo)
		{
			// Set contents returned for the specified branch
			DataSet ds = new DataSet();
			DSets dSets = new DSets();
			dSets.GetSetsForTNameBranch(tName, branchNo);
			ds.Tables.Add(dSets.SetsData);
			return ds;
		}

		public DataSet GetSetsForTNameBranchAll(string tName)
		{
			// Set lists returned for all branches, to be filtered
			// on one branch at a time by a view at the client
			DataSet ds = new DataSet();
			DSets dSets = new DSets();
			dSets.GetSetsForTNameBranchAll(tName);
			ds.Tables.Add(dSets.SetsData);
			return ds;
		}

		public void Save(SqlConnection conn, SqlTransaction trans, 
            string setName, int empeeNo, string tName, string columnType,string setDescript,decimal value)      // #13691
        {
            DSets dSets = new DSets();
            dSets.SetName = setName;
            dSets.SetDescript = setDescript;
            dSets.EmployeeNo = empeeNo;
            dSets.TName = tName;
            dSets.ColumnType = columnType;
            dSets.Value= value;                 // #13691
            dSets.Save(conn,trans);
        }

        public void Delete(SqlConnection conn, SqlTransaction trans, 
            string setName, string tName)
        {
            DSets dSets = new DSets();
            dSets.SetName = setName;
            dSets.TName = tName;
            dSets.Delete(conn,trans);
        }

        public BSets()
        {
        }
    }
}
