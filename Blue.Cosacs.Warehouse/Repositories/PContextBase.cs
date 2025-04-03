using Blue.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Warehouse.Repositories
{
    public partial class ContextBase : DbContextBase
    {
        public const string ConnectionStringName = "Default";
        public ContextBase()
            : base(ConnectionStringName)
        {
        }

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

}
