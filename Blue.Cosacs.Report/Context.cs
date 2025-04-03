using System.Data.Entity;
using Blue.Transactions;

namespace Blue.Cosacs.Report
{
    public partial class Context : ContextBase
    {
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
