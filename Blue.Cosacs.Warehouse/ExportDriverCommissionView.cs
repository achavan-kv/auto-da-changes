using System;
using Blue.Cosacs.Warehouse;
using FileHelpers;

[DelimitedRecord(",")]
public sealed class ExportDriverCommissionView //: Blue.Cosacs.BaseImportFile<ExportDriverCommissionView>
{
    public Int32 Id;
    public Int32 ScheduleId;
    public Int64 StockBranch;
    public string ScheduleOn;
    public string DeliveredOn;
    public String PickingId;
    public Decimal LineTotal;
    public string ExportedOn;
    public Int32 ExportedBy;
    public Int32 DriverId;      
    public string DriverName;   

    public static explicit operator ExportDriverCommissionView(DriverCommissionView value)
    {
        return new ExportDriverCommissionView()
        {
            DeliveredOn = DateToUIShortString(value.DeliveredOn.Value),
            ExportedBy = value.ExportedBy.Value,
            ExportedOn = DateToUIShortString(value.ExportedOn.Value),
            Id = value.Id.Value,
            LineTotal = value.LineTotal.Value,
            PickingId = value.PickingId,
            ScheduleId = value.ScheduleId.Value,
            ScheduleOn = DateToUIShortString(value.ScheduleOn.Value),
            StockBranch = value.StockBranch.Value,
            DriverId = Convert.ToInt32(value.DriverId),                       
            DriverName = value.DriverName                                     
        };
    }

    private static string DateToUIShortString(DateTime value)
    {
        return value.ToString("g", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
    }
}