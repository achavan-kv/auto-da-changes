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
    public class SettingsRepository : Repository
    {      
        public SettingsModel Get()
        {
            using (var conn = GetConnection())
            {
                return conn.Query<SettingsModel>("SELECT * FROM [Settings]").FirstOrDefault();               
            }
        }

        public void Save(string host)
        {
            using (var conn = GetConnection())
            {
                using (var trans = conn.BeginTransaction())
                {
                    var user = new SettingsModel() { Id = 1, Host = host };
                    if (Get() == null)
                    {
                        conn.Insert(trans, "Settings", user);
                    }
                    else
                    {
                        conn.Update(trans, "Settings", user);
                    }
                    trans.Commit();
                }
            }           
        }       
    }
}
