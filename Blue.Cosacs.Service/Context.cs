using System;
using System.Data.Entity;
using Blue.Transactions;
using StructureMap;

namespace Blue.Cosacs.Service
{
    public partial class Context : ContextBase
    {
        public static ReadScope<Context> Read()
        {
            return ObjectFactory.GetInstance<ReadScope<Context>>();
        }

        public static WriteScope<Context> Write()
        {
            return new WriteScope<Context>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<InvoiceSearchView>().HasKey(t => new { t.InvoiceNumber, t.branch, t.ItemId });
            modelBuilder.Entity<CustTelView>().HasKey(t => t.custid);
            modelBuilder.Entity<TechnicianAvailableRequestView>().HasKey(t => t.RequestId);
            modelBuilder.Entity<TechnicianAllocatedRequestView>().HasKey(t => t.RequestId);
            modelBuilder.Entity<TechnicianNameView>().HasKey(t => t.UserId);
            modelBuilder.Entity<CountryView>().HasKey(t => t.countrycode);
            modelBuilder.Entity<TechnicianUser>().HasKey(k => k.UserId);
            modelBuilder.Entity<SummaryPrintView>().HasKey(t => t.RequestId);
            modelBuilder.Entity<PublicHolidayView>().HasKey(t => t.Date);
            modelBuilder.Entity<StockView>().HasKey(t => new { t.ItemNumber, t.Location });
            modelBuilder.Entity<ExchangeRateView>().HasKey(t => t.RateId);
            modelBuilder.Entity<BranchLookup>().HasKey(t => t.branchno);
            modelBuilder.Entity<SummaryView>().HasKey(t => new { t.ServiceRequestNo, t.Acctno });
            modelBuilder.Entity<WarrantyBandView>().HasKey(t => new { t.ItemID, t.refcode });
            modelBuilder.Entity<AdminTechProfileUsersView>().HasKey(t => t.UserId);
            modelBuilder.Entity<HistoryView>().HasKey(t => t.Id);
            modelBuilder.Entity<TechnicianPaymentsView>().HasKey(t => t.RequestId);
            modelBuilder.Entity<TechnicianPaymentsTotCostView>().HasKey(t => t.TechnicianId);
            modelBuilder.Entity<RequestSlotTimesView>().HasKey(t => t.RequestId);
            modelBuilder.Entity<ChargeToView>().HasKey(t => t.Type);

            modelBuilder.Entity<RequestPartView>().HasKey(t => t.id);


            modelBuilder.Entity<PrimaryChargeToView>().HasKey(t => t.ResolutionPrimaryCharge);

            modelBuilder.Entity<ReceiptBranchDetailsView>().HasKey(t => t.BranchNo);
            modelBuilder.Entity<WarrantyTotalRequestsView>().HasKey(t => t.WarrantyId);
            modelBuilder.Entity<ItemsWithoutWarrantyView>().HasKey(t => new { t.CustomerAccount, t.CustomerId, t.ItemId, t.StockLocation });
            modelBuilder.Entity<ItemsByInvoiceNoSearchView>().HasKey(t => new
            {
                t.InvoiceNumber, t.StockLocation, t.ItemId, t.ManufacturerWarrantyContractNo,
                t.DeliveryDate, t.AccountNo, t.SoldBy
            });

            modelBuilder.Entity<RequestPart>().Property(p => p.CostPrice).HasPrecision(19, 3);
            modelBuilder.Entity<RequestPart>().Property(p => p.Price).HasPrecision(19, 3);
            modelBuilder.Entity<RequestPart>().Property(p => p.TaxAmount).HasPrecision(19, 3);

            modelBuilder.Entity<Charge>().Property(p => p.Cost).HasPrecision(19, 3);
            modelBuilder.Entity<Charge>().Property(p => p.Value).HasPrecision(19, 3);
            modelBuilder.Entity<Charge>().Property(p => p.Tax).HasPrecision(19, 3);

        }
    }
}
