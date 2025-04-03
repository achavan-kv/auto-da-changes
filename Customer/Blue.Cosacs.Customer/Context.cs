using System.Data.Entity;
using Blue.Transactions;
using StructureMap;

namespace Blue.Cosacs.Customer
{
    public partial class Context : ContextBase
    {
        public static ReadScope<Context> Read()
        {
            Database.SetInitializer<Context>(new DatabaseInitializer());

            var returnValue = StructureMap.ObjectFactory.TryGetInstance<ReadScope<Context>>();

            if (returnValue == null)
            {
                returnValue = new ReadScope<Context>();
            }

            return returnValue;
        }

        public static WriteScope<Context> Write()
        {
            Database.SetInitializer<Context>(new DatabaseInitializer());

            var returnValue = StructureMap.ObjectFactory.TryGetInstance<WriteScope<Context>>();

            if (returnValue == null)
            {
                returnValue = new WriteScope<Context>();
            }

            return returnValue;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CustomerSearchView>().HasKey(l => new { l.ID });
        }
    }
}
