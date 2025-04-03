using System.Data.Entity;
using Blue.Transactions;
using StructureMap;

namespace Blue.Cosacs.NonStocks
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
            modelBuilder.Entity<NonStockPromotionsView>().HasKey(p => new { p.RowKey });
        }
    }
}
