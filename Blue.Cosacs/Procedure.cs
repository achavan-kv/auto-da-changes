using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Blue.Transactions;

namespace Blue.Cosacs
{
    public abstract class Procedure
    {
        protected Procedure(string procedureName, SqlConnection connection, SqlTransaction transaction)
        {
            cmd = (SqlCommand)Context.Database().GetStoredProcCommand(procedureName);

            if (connection != null)
            {
                cmd.Connection = connection;

                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }
            }

            cmd.CommandTimeout = TimeoutManager.Instance.Timeout;
        }

        public virtual DataSet ExecuteDataSet()
        {
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            new SqlDataAdapter(cmd).Fill(dataSet);
            return dataSet;
        }

        public virtual int CommandTimeout
        {
            get { return cmd.CommandTimeout; }
            set { cmd.CommandTimeout = value; }
        }

        protected readonly SqlCommand cmd;
    }
}
