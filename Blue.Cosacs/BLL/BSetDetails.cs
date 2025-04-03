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
    /// Business Logic Layer for SetDetails
    /// </summary>
    public class BSetDetails : CommonObject
    {
        public DataSet GetSetDetailsForSetName(string setName, string tName)
        {
            DSetDetails dSetDetails = new DSetDetails();
            return dSetDetails.GetSetDetailsForSetName(setName,tName);
        }

        public void Insert(SqlConnection conn, SqlTransaction trans, 
            string setName, string data, string tName)
        {
            DSetDetails dSetDetails = new DSetDetails();
            dSetDetails.SetName = setName;
            dSetDetails.Data = data;
            dSetDetails.TName = tName;
            dSetDetails.Insert(conn,trans);
        }

        public void Delete(SqlConnection conn, SqlTransaction trans, 
            string setName, string tName)
        {
            DSetDetails dSetDetails = new DSetDetails();
            dSetDetails.SetName = setName;
            dSetDetails.TName = tName;
            dSetDetails.Delete(conn,trans);
        }

		public void InsertBranch(SqlConnection conn, SqlTransaction trans, 
			string setName, string tName, string branchNo)
		{
			DSetDetails dSetDetails = new DSetDetails();
			dSetDetails.SetName = setName;
			dSetDetails.TName = tName;
			dSetDetails.branchNo = branchNo;
			dSetDetails.InsertBranch(conn,trans);
		}

		public void DeleteBranch(SqlConnection conn, SqlTransaction trans, 
			string setName, string tName)
		{
			DSetDetails dSetDetails = new DSetDetails();
			dSetDetails.SetName = setName;
			dSetDetails.TName = tName;
			dSetDetails.DeleteBranch(conn,trans);
		}

        public CategoryItem GetCategoryItem(string categoryCode)
        {
            DSetDetails dSetDetails = new DSetDetails();
            return dSetDetails.GetCategoryItem(categoryCode);
        }

        public BSetDetails()
        {
        }
    }
}
