using System.Data.Entity;
using Blue.Transactions;

namespace Blue.Cosacs.Stock
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
            modelBuilder.Entity<Branch>().HasKey(t => new { t.branchno });
            modelBuilder.Entity<StockItemView>().HasKey(t => new { t.ItemID, t.origbr });
            modelBuilder.Entity<StockProductCountView>().HasKey(t => new { t.Id });
            modelBuilder.Entity<KitParentPricesView>().HasKey(t => new { t.ItemID });
            modelBuilder.Entity<StockItemViewRelations>().HasKey(t => new { t.Department, t.Category, t.Class });
            modelBuilder.Entity<StockItemWarrantyView>().HasKey(t => new { t.id});
        }
    }
}
