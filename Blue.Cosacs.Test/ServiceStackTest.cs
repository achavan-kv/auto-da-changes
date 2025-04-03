using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using System.Threading;
using System.IO;
using Blue.Cosacs.Shared.Net;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class ServiceStackTest
    {
        //[Test]
        public void HelloWorldJsonTest()
        {
            const string pathFormat = "..\\..\\..\\Blue.Cosacs.Ws.Host.Console\\bin\\{0}\\Blue.Cosacs.Ws.Host.Console.exe";

            var proc1 = System.Diagnostics.Process.Start(File.Exists(string.Format(pathFormat, "Debug")) ?
                string.Format(pathFormat, "Debug") : string.Format(pathFormat, "Release"));
            Thread.Sleep(20000);

            try
            {
                Call<HelloWorldRequest2, HelloWorldResponse>(new HelloWorldRequest2 { Text = "Hello Mike!" }, DoSuccess, DoError);

                var start = DateTime.Now;

                while (true)
                {
                    Thread.Sleep(500);
                    if (error)
                        Assert.Fail("Error calling web service...");
                    if (success)
                        break;
                    if (DateTime.Now.Subtract(start).TotalSeconds > 180)
                        Assert.Fail("Timeout...");
                }
            }
            finally
            {
                proc1.Kill();
            }
        }

        private void Call<TRequest, TResponse>(TRequest request, Shared.Action<TResponse> success, Shared.Action<TResponse, Exception> error)
        {
            var type = typeof(TRequest);
            var name = string.Format("{0}.{1}", type.Namespace, type.Name);
            var url = "/json/syncreply/" + name;
            new JsonServiceClient("http://DOG:82").Call<TRequest, TResponse>(url, request, success, error);
        }

        private void DoSuccess(HelloWorldResponse response)
        {
            Assert.AreEqual("Hello Mike!", response.EchoText);
            success = true;
        }

        private void DoError(HelloWorldResponse response, Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            error = true;
        }

        private bool error, success;

        //[Test]
        public void DataSetJsonTest()
        {
            const string pathFormat = "..\\..\\..\\Blue.Cosacs.Ws.Host.Console\\bin\\{0}\\Blue.Cosacs.Ws.Host.Console.exe";

            var proc1 = System.Diagnostics.Process.Start(File.Exists(string.Format(pathFormat, "Debug")) ?  
                string.Format(pathFormat, "Debug") : string.Format(pathFormat, "Release"));
            Thread.Sleep(2000);

            try
            {
                var error = false;
                var success = false;

                var req = new DataSetSampleRequest { MyString = "Give me my data set!" };

                Call<DataSetSampleRequest, DataSetSampleResponse>(req, res => success = true, (res, ex) => error = true);
                Call<DataSetSampleRequest, DataSetSampleResponse>(req, res => success = true, (res, ex) => error = true);
                Call<DataSetSampleRequest, DataSetSampleResponse>(req, res => success = true, (res, ex) => error = true);
                Call<DataSetSampleRequest, DataSetSampleResponse>(req, res => success = true, (res, ex) => error = true);

                var start = DateTime.Now;

                while (true)
                {
                    Thread.Sleep(500);
                    if (error)
                        Assert.Fail("Error calling web service...");
                    if (success)
                        break;
                    if (DateTime.Now.Subtract(start).TotalSeconds > 180)
                        Assert.Fail("Timeout...");
                }
            }
            finally
            {
                proc1.Kill();
            }
        }

       // [Test] reinstate??
        public void DataSetJsonSerializationTest()
        {
            var set = new DataSet();
            using (var c = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
            {
                c.Open();
                new SqlDataAdapter("SELECT TOP 5 * FROM DimCustomer", c).Fill(set);
            }

            var sw = new StringWriter();
            var serializer = Shared.Net.JsonServiceClient.CreateSerializer();
            serializer.Serialize(sw, set);
            Console.WriteLine(sw);


            // read
            var set2 = (DataSet)serializer.Deserialize(new StringReader(sw.ToString()), typeof(DataSet));

            Assert.AreEqual(set.Tables.Count, set2.Tables.Count, "!= table count");
            Assert.AreEqual(set.Tables[0].Columns.Count, set2.Tables[0].Columns.Count, "!= col count");
            Assert.AreEqual(set.Tables[0].Rows.Count, set2.Tables[0].Rows.Count, "!= row count");

            for (var i = 0; i < set.Tables[0].Rows.Count; i++)
            {
                var r1 = set.Tables[0].Rows[i];
                var r2 = set2.Tables[0].Rows[i];

                for (var j = 0; j < set.Tables[0].Columns.Count; j++)
                {
                    Assert.AreEqual(r1[j], r2[j], string.Format("row {0} col {1} differ", i, j));
                }
            }
        }
    }
}
