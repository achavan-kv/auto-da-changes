using System.Data.SqlClient;

namespace Blue.Cosacs
{
    public static class Transaction
    {

        public static ReturnType Execute<ReturnType>(System.Func<SqlConnection, SqlTransaction, ReturnType> method)
        {
            using (var connection = new SqlConnection(STL.DAL.Connections.Default))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var result = method(connection, transaction);
                    transaction.Commit();
                    return result;
                }
            }
        }
        public static void Execute(System.Action<SqlConnection, SqlTransaction> method)
        {
            using (var connection = new SqlConnection(STL.DAL.Connections.Default))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    method(connection, transaction);
                    transaction.Commit();
                }
            }
        }
    }
}
