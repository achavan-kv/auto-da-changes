using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using System.Data;
using DapperLite;

namespace Blue.Cosacs.StockCountApp
{
    public class UserRepository : Repository
    {      
        public UserModel Get()
        {
            using (var conn = GetConnection())
            {
                return conn.Query<UserModel>("SELECT * FROM [User]").FirstOrDefault();               
            }
        }

        public void Save(string username)
        {
            using (var conn = GetConnection())
            {
                using (var trans = conn.BeginTransaction())
                {
                    var user = new UserModel() { Id = username, ModifiedDate = DateTime.Now };
                    ClearAll();
                    conn.Insert(trans, "User", user);                    
                    trans.Commit();
                }
            }           
        }

        public void ClearAll()
        {
            using (var conn = GetConnection())
            {
                conn.Execute("DELETE FROM [User]");
            }
        }
    }
}
