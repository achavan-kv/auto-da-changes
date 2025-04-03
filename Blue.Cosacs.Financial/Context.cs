using System.Data.Entity;
using Blue.Transactions;

namespace Blue.Cosacs.Financial
{
    public partial class Context : ContextBase
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountryMaintenanceView>().HasKey(p => p.ParameterID);
        }

        public static ReadScope<Context> Read()
        {
            return new ReadScope<Context>();
        }

        public static WriteScope<Context> Write()
        {
            return new WriteScope<Context>();
        }
    }
}
