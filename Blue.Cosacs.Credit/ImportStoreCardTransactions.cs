using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

[DelimitedRecord(",")]
public sealed class ImportStoreCardTransactions
{
    public Int64 CardNumber;
    public decimal TransactionAmount;

    [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
    public DateTime TransactionDate;

    [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]
    public DateTime ExportDate;
   
}