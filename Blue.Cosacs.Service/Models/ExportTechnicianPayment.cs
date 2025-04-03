using System;
using FileHelpers;

namespace Blue.Cosacs.Service.Models
{
    [DelimitedRecord(",")]
    public class ExportTechnicianPayment
    {
        public int RequestId { get; set; }
        public string AllocatedOn { get; set; }
        public decimal Labour { get; set; }
        public decimal PartsOther { get; set; }
        public decimal Total { get; set; }
        public string State { get; set; }

        public static explicit operator ExportTechnicianPayment(TechnicianPaymentsView value)
        {
            return new ExportTechnicianPayment()
            {
                RequestId = value.RequestId,
                AllocatedOn = DateToUIShortString(Convert.ToDateTime(value.AllocatedOn)),
                Labour = value.Labour.HasValue ? value.Labour.Value : 0,
                PartsOther = value.PartsOther.HasValue ? value.PartsOther.Value : 0,
                Total = value.Total.HasValue ? value.Total.Value : 0,
                State = GetState(value.State)
            };
        }

        private static string DateToUIShortString(DateTime value)
        {
            return value.ToString("g", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        }

        private static string GetState(string s)
        {
            switch (s)
            {
                case "P":
                    return "Paid";
                case "D":
                    return "Deleted";
                case "H":
                    return "On Hold";
                default:
                    return "Pending";
            }
        }
    }
}
