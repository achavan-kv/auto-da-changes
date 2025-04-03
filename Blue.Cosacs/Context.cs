using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using StructureMap;
using Blue.Transactions;

namespace Blue.Cosacs
{
    public partial class Context : System.Data.Linq.DataContext
    {
        static Context()
        {
            //var dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            //database = DatabaseFactory.CreateDatabase(STL.DAL.Connections.DefaultName);
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[STL.DAL.Connections.DefaultName].ConnectionString;
            database = new SqlDatabase(connectionString);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Blue.Cosacs.Linq2Sql.xml"))
                xmlMappingSource = XmlMappingSource.FromStream(stream);
        }

        internal static Database Database()
        {
            return database;
        }

        private static readonly Database database;
        private static readonly XmlMappingSource xmlMappingSource;

        public static string ConnectionString
        {
            get { return STL.DAL.Connections.Default; }
        }

        private Context() : base(ConnectionString, xmlMappingSource) { }
        private Context(DbConnection connection) : base(connection, xmlMappingSource) { }

        public static Context Create(DbConnection connection = null, DbTransaction transaction = null)
        {
            Context context;

            if (connection == null)
            {
                context = new Context();
            }
            else
            {
                context = new Context(connection);
                if (transaction != null)
                {
                    context.Transaction = transaction;
                }
            }

            context.CommandTimeout = TimeoutManager.Instance.Timeout;

            return context;
        }

        public static void ExecuteTx(System.Action<Context, SqlConnection, SqlTransaction> dbAction, Context context = null, SqlConnection conn = null, SqlTransaction trans = null)
        {
            ExecuteTx<int>((ctx2, c2, t2) =>
                {
                    dbAction(ctx2, c2, t2);
                    return 0;
                }, context, conn, trans);
        }

        public static T ExecuteTx<T>(System.Func<Context, SqlConnection, SqlTransaction, T> dbAction, Context context = null, SqlConnection conn = null, SqlTransaction trans = null)
        {
            if (!(context == null && conn == null && trans == null || context != null && conn != null && trans != null || context == null && conn != null && trans != null))
                throw new ArgumentException("Invalid context creation. Specfied parameter combo is not valid.");

            var create = (context == null); // they are all null
            var ctxonly = (context == null && conn != null && trans != null);

            try
            {
                if (create)
                {
                    conn = conn == null ? new SqlConnection(ConnectionString) : conn;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    trans = trans == null ? conn.BeginTransaction() : trans;
                    context = Context.Create(conn, trans);
                }

                var result = dbAction(context, conn, trans);

                if (create && !ctxonly)
                    trans.Commit();

                return result;
            }
            finally
            {
                if (create && !ctxonly)
                {
                    trans.Dispose();
                    conn.Dispose();
                    context.Dispose();
                }
            }
        }
    }
}
