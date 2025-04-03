using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Reflection;
using MSELData = Microsoft.Practices.EnterpriseLibrary.Data;

namespace Blue.Cosacs
{
    public partial class Context : System.Data.Entity.DbContext
    {
        static Context()
        {
            database = MSELData.DatabaseFactory.CreateDatabase(STL.DAL.Connections.DefaultName);
        }

        internal static MSELData.Database Database()
        {
            return database;
        }

        private static readonly MSELData.Database database;

        public static string ConnectionString
        {
            get { return STL.DAL.Connections.Default; }
        }

        private Context() : base(STL.DAL.Connections.DefaultName) { }
        private Context(DbConnection connection) : base(connection, contextOwnsConnection: false) { }

        public static Context Create(DbConnection connection = null, DbTransaction transaction = null)
        {
            if (connection == null)
                return new Context();

            var context = new Context(connection);
            var objContext = (System.Data.Entity.Infrastructure.IObjectContextAdapter)context;
            //new System.Data.EntityClient.EntityConnection()
            //objContext.ObjectContext.

            if (transaction != null)
                context.Transaction = transaction;
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
