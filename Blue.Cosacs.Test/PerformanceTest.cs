using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Configuration;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class PerformanceTest
    {
        //[Test]
        public void CommandTimeoutTest()
        {
            var cs = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            using (var c = new SqlConnection(cs))
            {
                c.Open();
                var cmd = new SqlCommand("UPDATE acct SET dateacctopen = dateacctopen", c);
                cmd.CommandTimeout = 3; // 10 seconds
                cmd.ExecuteNonQuery();
            }
        }
    }
}
