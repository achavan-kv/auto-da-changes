using Blue.Transactions;
using StructureMap;
using System.Data.Entity;

namespace Blue.Cosacs.Payments
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
            modelBuilder.Entity<ActiveExchangeRates>().HasKey(p => new { p.id });
        }
    }
}
