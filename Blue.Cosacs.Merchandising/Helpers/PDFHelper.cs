using System;

namespace Blue.Cosacs.Merchandising.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Blue.Cosacs.Merchandising.Models;

    using Microsoft.Reporting.WebForms;

    public static class PdfHelper
    {
        public static MemoryStream CreatePdfMemoryStream(PurchaseOrderViewModel purchaseOrder, SupplierModel supplier, LocationModel receivingLocation)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            var report = new LocalReport
            {
                ReportEmbeddedResource = "Blue.Cosacs.Merchandising.ReportTemplates.PurchaseOrder.rdlc"
            };

            var rds = new ReportDataSource { Name = "PurchaseOrderViewModel", Value = new List<PurchaseOrderViewModel> { purchaseOrder } };
            report.DataSources.Add(rds);

            var rds2 = new ReportDataSource { Name = "PurchaseOrderProducts", Value = purchaseOrder.Products };
            report.DataSources.Add(rds2);

            var rds3 = new ReportDataSource { Name = "Supplier", Value = new List<SupplierModel> { supplier } };
            report.DataSources.Add(rds3);

            var rds4 = new ReportDataSource { Name = "ReceivingLocation", Value = new List<LocationModel> { receivingLocation } };
            report.DataSources.Add(rds4);

            var pdfBytes = report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
            return new MemoryStream(pdfBytes);
        }
    }
}
