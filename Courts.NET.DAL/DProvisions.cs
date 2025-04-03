using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using STL.Common;
using System.Data;
using System.Data.SqlClient;
using System;

namespace STL.DAL
{
    public class DProvisions : DALObject
    {
        Database db;
        public DProvisions()
        {
            db = DatabaseFactory.CreateDatabase();

        }

        public void SaveProvisions(DataTable ProTable)
        {
            using (SqlConnection conn = new SqlConnection(Connections.Default))
            {
                conn.Open();

                SqlCommand sqlcmb = new SqlCommand("Truncate Table Provisions", conn);
                sqlcmb.ExecuteNonQuery();

                SqlDataAdapter Da = new SqlDataAdapter("select * from provisions", conn);
                SqlCommandBuilder cmb = new SqlCommandBuilder(Da);
                Da.Update(ProTable);
            }

        }

        public List<ProvisionsItem> LoadProvisions()
        {
            List<ProvisionsItem> items = new List<ProvisionsItem>();

            DbCommand comm = db.GetStoredProcCommand("ProvisionsGetAllSP");
            var dr = db.ExecuteReader(comm);

            while (dr.Read())
            {
                
                ProvisionsItem item = new ProvisionsItem();
                item.Acctype = Convert.ToChar(dr["Acctype"]);
                item.StatusName = dr["StatusName"].ToString();
                item.StatusLower = Convert.ToInt32(dr["StatusLower"]);
                item.StatusUpper = Convert.ToInt32(dr["StatusUpper"]); 
                item.MonthsName = dr["MonthsName"].ToString();
                item.MonthsLower = Convert.ToInt32(dr["MonthsLower"]);
                item.MonthsUpper = Convert.ToInt32(dr["MonthsUpper"]);
                item.Provision = Convert.ToDecimal(dr["Provision"]);
                items.Add(item);
            }
            return items;
        }
    
    }
}
