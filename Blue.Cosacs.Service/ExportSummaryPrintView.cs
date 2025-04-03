using System;
using Blue.Cosacs.Service;
using FileHelpers;

[DelimitedRecord(",")]
public sealed class ExportSummaryPrintView 
{
    public Int32 RequestId;
    public string Status;
    public string CreatedOn;
    public Int32 DaysOutstanding;
    public string LastUpdatedOn;
    public string CustomerChargeAcct;
    public string DepositPaid;
    public string ChargeAcctCancel;
    

    public static explicit operator ExportSummaryPrintView(SummaryPrintView value)
    {
        return new ExportSummaryPrintView()
        {
            RequestId = value.RequestId,
            Status = value.Status,
            CreatedOn = DateToUIShortString(value.CreatedOn),
            DaysOutstanding = Convert.ToInt32(value.DaysOutstanding),
            LastUpdatedOn = DateToUIShortString(Convert.ToDateTime(value.LastUpdatedOn)),
            CustomerChargeAcct = value.CustomerChargeAcct,
            DepositPaid = value.DepositPaid,
            ChargeAcctCancel = value.ChargeAcctCancel
        };
    }

    private static string DateToUIShortString(DateTime value)
    {
        return value.ToString("g", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
    }
}