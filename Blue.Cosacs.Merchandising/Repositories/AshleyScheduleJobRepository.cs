using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface IAshleyScheduleJobRepository
    {
        void CreateAutoPO();
        void RunnSSISPackage();
        void ImportSaleDataFromFile();
        void ReadAsnXML();
        void ExportPOXML();
        void ExportStockCSV();
    }
    public class AshleyScheduleJobRepository : IAshleyScheduleJobRepository
    {
        
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 18/02/2019
        /// CR     : #Ashley
        /// Details: Auto Create PO JOB
        /// </summary>
        /// <returns></returns>
        /// 
        public void CreateAutoPO()
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("CreateAutoPO");
            }
        }


        public void ReadAsnXML()
        {

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@name";
            parameter.Value = "ReadAsnXML";
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("EXEC AshleyJob @name", parameter);
            }
        }


        public void ExportPOXML()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@name";
            parameter.Value = "ExportPOXML";
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("EXEC AshleyJob @name", parameter);
            }
        }


        public void ExportStockCSV()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@name";
            parameter.Value = "ExportStockCSV";
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("EXEC AshleyJob @name", parameter);
            }
        }


        public void ImportSaleDataFromFile()
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@name";
            parameter.Value = "ImportSaleData";
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("EXEC AshleyJob @name", parameter);
            }
        }


        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 22/05/2019
        /// CR     : #Ashley
        /// Details: Demo
        /// </summary>
        /// <returns></returns>
        public void RunnSSISPackage()
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("SSISPackageRunning");
            }
        }
    }
}
