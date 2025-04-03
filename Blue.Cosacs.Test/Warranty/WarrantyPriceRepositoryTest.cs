using Blue.Cosacs.Test.Warranty.SqlScripts;
using Blue.Cosacs.Warranty.Repositories;
using NUnit.Framework;
using StructureMap;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Blue.Cosacs.Test.Warranty
{
    [TestFixture]
    public class WarrantyPriceRepositoryTest
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        [TestFixtureSetUp]
        public virtual void Setup()
        {
            ObjectFactory.Initialize(p => p.AddRegistry(new Registry()));
        }

        //[Test]
        public void CreateWarrantyPriceTestValues()
        {
            SqlScriptsRunner.ExecuteScript(connectionString, "WarrantyPriceRepositoryTestScript.sql");

            Assert.True(true);

            SqlScriptsRunner.ExecuteScript(connectionString, "WarrantyPriceRepositoryTestScript-Drop.sql");
        }

        //[Test]
        public void RunScenarioNo1()
        {
            // Scenario 1
            // Bulk edit and increase the cost price by 10 % with effective date 23-01-2014.
            // Nothing would change since there is already a price set for 23rd for War1.
            // There is no price on War 2 which can be changed.

            SqlScriptsRunner.ExecuteScript(connectionString, "WarrantyPriceRepositoryTestScript.sql");

            LoadWarrantyPriceTestData(1, 2014);

            SqlScriptsRunner.ExecuteScript(connectionString, "WarrantyPriceRepositoryTestScript-Drop.sql");

            Assert.True(true);
        }

        private void LoadWarrantyPriceTestData(int scenario, int year)
        {
            SqlScriptsRunner.RunStoredProcedure(connectionString,
                "[Warranty].[WarrantyPriceRepositoryBulkInsertTestScript]",
                new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Scenario", Value = scenario },
                    new SqlParameter() { ParameterName = "@SampleYear", Value = year.ToString() }
                });
        }
    }
}
