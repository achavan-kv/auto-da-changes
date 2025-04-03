
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using System.IO;

namespace Blue.Cosacs.Financial
{
    [DelimitedRecord(",")]
    internal class TransactionExport
    {
        public string CountryCode;
        public int RunNumber;
        public string FactAcct;
        public short BranchNo;
        public string TransType;
        public decimal Debit_CreditValues; 
        public string Date;
        private string Category = string.Empty;

        private static TransactionExport Convert(Transaction value, int runNumber, string countryCode)
        {
            return new TransactionExport()
            {
                FactAcct = value.Account,
                Debit_CreditValues = value.Amount.Value,
                BranchNo = value.BranchNo,
                Date = value.Date.ToString("yyyyMMdd"),
                TransType = value.Type,
                RunNumber = runNumber,
                CountryCode = countryCode
            };
        }

        public static void WriteToWriter(List<Transaction> records, TextWriter writer, int runNumber, string countryCode)
        {
            var valuesToExport = records
                .Select(p => Convert(p, runNumber, countryCode))
                .ToList();

            var engine = new FileHelperEngine<TransactionExport>();

            engine.WriteStream(writer, valuesToExport);
        }
    }
}
