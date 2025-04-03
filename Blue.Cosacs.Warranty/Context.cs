using System.Data.Entity;
using Blue.Transactions;
using StructureMap;

namespace Blue.Cosacs.Warranty
{
    public partial class Context : ContextBase
    {
        public static ReadScope<Context> Read()
        {
            var returnValue = StructureMap.ObjectFactory.TryGetInstance<ReadScope<Context>>();

            if (returnValue == null)
            {
                returnValue = new ReadScope<Context>();
            }

            return returnValue;
        }

        public static WriteScope<Context> Write()
        {
            return new WriteScope<Context>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Warranty>().Property(w => w.TaxRate).HasPrecision(4, 2);
            modelBuilder.Entity<BranchLookup>().HasKey(w => w.branchno);
            modelBuilder.Entity<PriceCalcView>().HasKey(p => p.Id);
            modelBuilder.Entity<WarrantyReturnSearchView>().HasKey(p => new { p.WarrantyReturnId, p.TagId });
            modelBuilder.Entity<WarrantyPromotionsView>().HasKey(p => new { p.RowKey });
        }
    }
}
