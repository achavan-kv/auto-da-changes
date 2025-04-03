using System;
using System.Collections.Generic;
using System.Text;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Configuration;
using STL.BLL.BrokerFinancial;
//using STL.BLL.BrokerFinancialWS;
using IBM.WMQ;


namespace STL.BLL.BrokerFinancial
{
    public class BBrokerFinancialExport : CommonObject
    {
        public BBrokerFinancialExport()
        {

        }

        //IP - CR946 - Extract the data to export to the Broker.
        public DataSet GetBrokerExtractData(SqlConnection conn, SqlTransaction trans)
        {
            DataSet brokerData = new DataSet();
            DBrokerExtract d = new DBrokerExtract();

            try
            {

                brokerData = d.GetBrokerExtractData(conn, trans);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return brokerData;
        }

        public void ExportBrokerData(int runNo, DataTable brokerDT)
        {
            try
            {
                //string url;
                string extractedBrokerData;


                //Call method that creates the CSV and creates the string to send
                //accross to the Oracle/MQ
                extractedBrokerData = this.createBrokerCSV(runNo, brokerDT);

                //Call the method to send the broker data, passing in the data to send
                //and the URL to gain access to the web service.
                if ((bool)Country[CountryParameterNames.MQEnabled])
                    this.SendBrokerData(extractedBrokerData);



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //Method that creates a CSV file of the Broker Financial Data and creates
        //a string that will be used to send the data to the URL supplied.
        private string createBrokerCSV(int runNo, DataTable brokerDT)
        {

            string fileName = "BrokerXRun" + runNo.ToString() + ".csv";
            string outputFile = Path.Combine((string)Country[CountryParameterNames.OracleBExportLocn] + "\\", fileName);


            StringBuilder sb = new StringBuilder();

            try
            {

                //Create the column headings
                //foreach (DataColumn dc in brokerDT.Columns)
                //{

                //    sb.Append(dc);
                //    if (brokerDT.Columns.IndexOf(dc) != brokerDT.Columns.Count - 1)
                //    {
                //        sb.Append(",");
                //    }

                //}

                //sb.AppendLine();

                //Create a row for each line of data (comma seperated)
                foreach (DataRow dr in brokerDT.Rows)
                {
                    foreach (DataColumn dc in brokerDT.Columns)
                    {
                        sb.Append(dr[dc].ToString());
                        if (brokerDT.Columns.IndexOf(dc) != brokerDT.Columns.Count - 1)
                            sb.Append(",");

                    }
                    sb.AppendLine();
                }
                //sb.Append(fileName); //CR 1036 Broker Extract Changes - want file name appended in file...     
                // AA 2 - aug -removing as Broker threw an exception on receipt of the file
                sb.AppendLine();
                FileInfo fi = new FileInfo(outputFile);
                FileStream fs = fi.OpenWrite();
                byte[] blob = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(blob, 0, blob.Length);
                fs.Close();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            //Return the string that holds the broker data to be sent accross to the URL.
            return Convert.ToString(sb);
        }

        public void ExportBrokerData(string brokerData)
        {
            if ((bool)Country[CountryParameterNames.MQEnabled])
            {
                this.SendBrokerData(brokerData);
            }
        }

        //Method which will send the broker data extract to MQ Message Service.
        private void SendBrokerData(string brokerData)
        {
            //int Status;
            
            try
            {
                logMessage("Connecting to MQ", "99999", System.Diagnostics.EventLogEntryType.Information);

                MQAdapter Queue1 = new MQAdapter(MessagingType.Sync);

                // Need the name of the MQManager which is gotten from the eod.net.config/app.config file.  
                // Will be the same for all countries, but not for test installations hence better in the app.config
                string MQManager = "";
                MQManager = ConfigurationManager.AppSettings["MQManager"];

                string ResponseQueueName = "";
                ResponseQueueName = ConfigurationManager.AppSettings["ResponseQueueName"];

                if (ResponseQueueName != "")
                {
                    Queue1.OutQueName = ResponseQueueName;
                }

                try
                {
                    Queue1.SendRequestToMQ(brokerData, MQManager);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}
