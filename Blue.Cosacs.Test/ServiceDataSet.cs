using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Blue.Cosacs.Test
{
    public class DataSetSampleRequest
    {
        public string MyString;
    }

    public class DataSetSampleResponse 
    {
        public DataSet MyDataSet { get; set; }
    }

    public class DataSetSampleService : IService<DataSetSampleRequest>
    {
        public object Execute(DataSetSampleRequest request)
        {
            var set = new DataSet();
            using (var c = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
            {
                c.Open();
                new SqlDataAdapter("SELECT TOP 5 * FROM DimCustomer", c).Fill(set);
            }

            var start = DateTime.Now;
            for (var i = 0; i < 1; i++)
            {
                var sw = new StringWriter();
                Shared.Net.JsonServiceClient.CreateSerializer().Serialize(sw, set);
                var countJson = sw.ToString().Length;
                Console.WriteLine(sw);
                Console.WriteLine("Dataset in JSON is: {0} bytes", countJson);
            }
            Console.WriteLine(DateTime.Now.Subtract(start));

            start = DateTime.Now;
            for (var i = 0; i < 1; i++)
            {
                var sw = new StringWriter();
                set.WriteXml(sw, XmlWriteMode.DiffGram);
                var countXml = sw.ToString().Length;
                Console.WriteLine(sw);
                Console.WriteLine("Dataset in XML is: {0} bytes", countXml);
            }
            Console.WriteLine(DateTime.Now.Subtract(start));

            //Console.WriteLine("JSON is {0} % of XML in size", ((double)countJson) / ((double)countXml) * 100);

            return new DataSetSampleResponse { MyDataSet = set };
        }
    }
}
