using Blue.Transactions;
using System.Data.Entity;

namespace Blue.Cosacs.Merchandising
{
    public class ReportingContext : Context
    {
        public ReportingContext() : base("Report") { }

        public static ReadScope<ReportingContext> Read()
        {
            return StructureMap.ObjectFactory.TryGetInstance<ReadScope<ReportingContext>>() ?? new ReadScope<ReportingContext>();
        }
    }

    public partial class Context : ContextBase
    {
        public Context() : this("Default")
        {
        }

        public Context(string connectionString) : base(connectionString) { }

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
        }
    }
}
