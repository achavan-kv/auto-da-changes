using Blue.Transactions;
using System.Data.Entity;

namespace Blue.Cosacs.Credit
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
            ScoreCardModelBuilder.Build(modelBuilder);
        }
    }
}
