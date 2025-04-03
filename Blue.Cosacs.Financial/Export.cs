using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Financial
{
    public class Export
    {
        public Export()
        {
            this.RunNumber = 0;
            HasBeenMarkAsExported = false;
        }

        private class ExportData
        {
            public List<Transaction> Transactions
            {
                get;
                set;
            }

            private short runNumber;
            public short RunNumber
            {
                get
                {
                    return this.runNumber;
                }
                set
                {
                    this.runNumber = Convert.ToInt16(value + 1);
                }
            }

            public string CountryCode
            {
                get;
                set;
            }

            public string ExportDrive
            {
                get;
                set;
            }
        }

        private int RunNumber
        {
            get;
            set;
        }

        private bool HasBeenMarkAsExported
        {
            get;
            set;
        }

        public string Run(int runNo)
        {
            var sb = new StringBuilder();
            var data = this.GetDataTransactions();

            using (var writer = new StringWriter(sb))
            {
                TransactionExport.WriteToWriter(data.Transactions, writer, runNo, data.CountryCode);
                this.RunNumber = runNo;

                if (!string.IsNullOrEmpty(data.ExportDrive))
                {
                    this.SendToFileSystem(sb.ToString(), data.ExportDrive, runNo);
                    this.MarkAsExported();
                }

                writer.Close();
            }

            return sb.ToString();
        }

        public void MarkAsExported()
        {
            if (this.HasBeenMarkAsExported)
            {
                return;
            }

            if (this.RunNumber == 0)
            {
                throw new Exception("Export has not been run yet");
            }

            using (var ctx = Context.Write())
            {
                var sp = new MarkTransactionsAsExported();
                sp.ExecuteNonQuery(this.RunNumber);
                ctx.Complete();
            }
            this.HasBeenMarkAsExported = true;
        }

        private void SendToFileSystem(string valuesToExport, string filePath, int runNumber)
        {
            var fileName = string.Format("FinTrans{0}.csv", runNumber);

            using (var writer = System.IO.File.CreateText(Path.Combine(filePath, fileName)))
            {
                writer.Write(valuesToExport);
                writer.Flush();
                writer.Close();
            }
        }

        //private void SendToQueue(string valuesToExport)
        //{
        //    var bf = new BBrokerFinancialExport();
        //    bf.ExportBrokerData(valuesToExport);
        //}

        private ExportData GetDataTransactions()
        {
            var returnValue = new ExportData();
            const string SystemDrive = "systemdrive";

            using (var ctx = new Context())
            {
                returnValue.CountryCode = new GetCountryCode().ExecuteScalar().ToString();

                var rn = ctx.Transaction
                    .Max(p => p.RunNo);

                if (!rn.HasValue)
                {
                    rn = new short?(0);
                }

                returnValue.ExportDrive = ctx.CountryMaintenanceView
                    .Where(p => p.CodeName == SystemDrive)
                    .Select(p => p.Value)
                    .FirstOrDefault();

                returnValue.RunNumber = rn.Value;
                returnValue.Transactions = ctx.Transaction.SqlQuery("exec GetFinTransDataSP")
                    .ToList();
            }

            return returnValue;
        }
    }
}
