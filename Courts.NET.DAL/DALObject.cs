using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using System.Collections.Generic;

namespace STL.DAL
{
    /// <summary>
    /// DALObject provides standard, low-level data access functionality that
    /// the data classes will use.
    /// This class is abstract, as such it can only be inherited, not instantiated.
    /// </summary>
    abstract public class DALObject : CommonObject
    {
        //private SqlConnection Connection;
        protected string connectionStr;
        protected SqlParameter[] parmArray;
        protected int result = 0;
        //protected string User = "";

        protected DALObject()
        {
            connectionStr = Connections.Default;
        }

        static DALObject()
        {
            commandTimeout = GetCommandTimeOut();
        }

        protected DALObject(CosacsDatabase cosacsDatabase)
        {
            switch (cosacsDatabase)
            {
                case CosacsDatabase.Live:
                    connectionStr = Connections.Default;
                    DALObject.commandTimeout = GetCommandTimeOut();
                    break;
                case CosacsDatabase.Reporting:
                    connectionStr = Connections.Report;
                    DALObject.commandTimeout = GetCommandTimeOut(true);
                    break;
            }
        }

        public static int CommandTimeout
        {
            get { return commandTimeout; }
        }

        private static int commandTimeout;

        private static int GetCommandTimeOut(bool isRepoting = false)
        {
            var setKey = isRepoting ? "LongRunningQueryCommandTimeoutInSecs" : "commandTimeout";

            return Convert.ToInt32(ConfigurationManager.AppSettings[setKey]);
        }

        /// <summary>
        /// This method creates a SqlCommand object designed to execute a stored
        /// procedure.
        /// </summary>
        protected SqlCommand CreateCommand(string sprocName, IDataParameter[] parameters, SqlConnection connection, SqlTransaction transaction = null, bool addreturn = true)
        {
            var command = new SqlCommand(sprocName, connection, transaction)
                              {
                                  CommandType = CommandType.StoredProcedure,
                                  CommandTimeout = CommandTimeout,
                              };
            
            // Populate the Sql Command's parameter array.
            if (parameters != null && parameters.Length > 0)
                foreach (SqlParameter parameter in parameters)
                    command.Parameters.Add(parameter);

            // Add a SqlParameter representing the return value from the stored procedure.
            //All stored procedures have an output parameter called @return
            if (addreturn)
            {
            command.Parameters.Add(new SqlParameter("@return",
                SqlDbType.Int,
                4, /* Size */
                ParameterDirection.Output,
                false, /* is nullable */
                0, /* byte precision */
                0, /* byte scale */
                string.Empty,
                DataRowVersion.Default,
                null));
            }

            return command;
        }

        /// <summary>
        /// Creates a SqlCommand object designed to execute a stored procedure with parameters but with no output parameter.
        /// </summary>
        private static SqlCommand CreateCommandWithNoReturn(string sprocName, SqlConnection connection, SqlTransaction transaction = null, IDataParameter[] parameters = null)
        {
            var command = new SqlCommand(sprocName, connection, transaction)
                              {
                                  CommandType = CommandType.StoredProcedure,
                                  CommandTimeout = CommandTimeout
                              };

            // Populate the Sql Command's parameter array.
            if (parameters != null && parameters.Length > 0)
                foreach (SqlParameter parameter in parameters)
                    command.Parameters.Add(parameter);

            return command;
        }

        //Property to expose the database connection string
        protected string DSN
        {
            get { return connectionStr; }
        }

        /// <summary>
        /// RunSP method designed for transactional work. 
        /// </summary>
        /// <param name="conn">A connection object which will have an associated transaction object</param>
        /// <param name="trans">The transaction to participate in</param>
        /// <param name="sprocName">String representing the name of the stored procedure to execute</param>
        /// <param name="parameters">SqlParameter[] array of arguments to the stored procedure</param>
        /// <returns>Integer representing the integer value returned from the stored procedure</returns>
        protected int RunSP(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters)
        {
            var command = CreateCommand(sprocName, parameters, conn, trans);
            command.ExecuteNonQuery();
            return (int)command.Parameters["@return"].Value;
        }

        /// <summary>
        /// RunSP method designed for transactional work. 
        /// </summary>
        /// <param name="conn">A connection object which will have an associated transaction object</param>
        /// <param name="trans">The transaction to participate in</param>
        /// <param name="sprocName">String representing the name of the stored procedure to execute</param>
        /// <param name="dataTable">DataTable into which to place return results from stored procedure</param>
        /// <returns>Integer representing the integer value returned from the stored procedure</returns>
        protected int RunSP(SqlConnection conn, SqlTransaction trans, string sprocName, DataTable dataTable)
        {
            var adapter = new SqlDataAdapter { SelectCommand = CreateCommand(sprocName, null, conn, trans) };
            adapter.Fill(dataTable);
            return (int)adapter.SelectCommand.Parameters["@return"].Value;
        }

        /// <summary>
        /// RunSP method designed for transactional work. 
        /// </summary>
        protected int RunSP(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters, DataSet ds)
        {
            var adapter = new SqlDataAdapter
                            {
                                SelectCommand = CreateCommand(sprocName, parameters, conn, trans)
                            };
            adapter.Fill(ds);
            return (int)adapter.SelectCommand.Parameters["@return"].Value;
        }

        /// <summary>
        /// RunSP method designed for transactional work. 
        /// </summary>
        protected int RunSP(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters, DataTable dataTable)
        {
            var adapter = new SqlDataAdapter
                            {
                                SelectCommand = CreateCommand(sprocName, parameters, conn, trans)
                            };
            adapter.Fill(dataTable);
            return (int)adapter.SelectCommand.Parameters["@return"].Value;
        }

        /// <summary>
        /// Executes a stored procedure within the database
        /// </summary>
        /// <param name="sprocName">String representing the name of the stored procedure to execute</param>
        /// <param name="parameters">SqlParameter[] array of arguments to the stored procedure</param>
        /// <returns>Integer representing the integer value returned from the stored procedure</returns>
        protected int RunSP(string sprocName, IDataParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand(sprocName, parameters, connection);
                command.ExecuteNonQuery();
                return (int)command.Parameters["@return"].Value;
            }
        }

        /// <summary>
        /// Executes a stored procedure, placing results in a DataSet.
        /// </summary>
        protected int RunSP(string sprocName, IDataParameter[] parameters, DataTable dataTable)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var adapter = new SqlDataAdapter
                                {
                                    SelectCommand = CreateCommand(sprocName, parameters, connection)
                                };
                adapter.Fill(dataTable);
                return Convert.ToInt32(adapter.SelectCommand.Parameters["@return"].Value);
            }
        }

        /// <summary>
        /// Executes a stored procedure, placing results in a DataSet.
        /// </summary>
        protected int RunSP(string sprocName, IDataParameter[] parameters, DataSet ds)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var adapter = new SqlDataAdapter
                                {
                                    SelectCommand = CreateCommand(sprocName, parameters, connection)
                                };
                adapter.Fill(ds);
                return (int)adapter.SelectCommand.Parameters["@return"].Value;
            }
        }

        /// <summary>
        /// Executes a stored procedure with no parameters, placing results in a DataSet.
        /// </summary>
        /// <param name="sprocName">String representing the name of the stored procedure to execute.</param>
        /// <param name="dataSet">DataSet into which to place return results from stored procedure</param>
        /// <returns></returns>
        protected int RunSP(string sprocName, DataSet dataSet)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var adapter = new SqlDataAdapter { SelectCommand = CreateCommand(sprocName, null, connection) };
                adapter.Fill(dataSet);
                return (int)adapter.SelectCommand.Parameters["@return"].Value;
            }
        }

        protected int RunSP(SqlConnection conn, SqlTransaction trans, string sprocName)
        {
            var command = CreateCommand(sprocName, null, conn, trans);
            command.ExecuteNonQuery();
            return (int)command.Parameters["@return"].Value;
        }

        /// <summary>
        /// Executes a stored procedure with no parameters, placing results in a DataTable.
        /// </summary>
        protected int RunSP(string sprocName, DataTable dataTable)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var adapter = new SqlDataAdapter { SelectCommand = CreateCommand(sprocName, null, connection) };
                adapter.Fill(dataTable);
                return (int)adapter.SelectCommand.Parameters["@return"].Value;
            }
        }
        /// <summary>
        /// Executes a stored procedure with no parameters, No return.
        /// </summary>
        protected int RunSP(string sprocName)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand(sprocName, null, connection);
                command.ExecuteNonQuery();
                return (int)command.Parameters["@return"].Value;
            }
        }

        /// <summary>
        /// Executes stored procedures that have no @result output parameter and no input parameters, within a transaction
        /// </summary>
        protected void RunSPNoReturn(SqlConnection conn, SqlTransaction trans, string sprocName)
        {
            //Connection = newSqlConnection(connectionStr);
            CreateCommandWithNoReturn(sprocName, conn, trans).ExecuteNonQuery();
        }

        /// <summary>
        /// Executes stored procedures that have no @result output parameter and no input parameters, and returns the result set in a DataTable
        /// </summary>
        protected DataTable RunSPNoReturn(string sprocName)
        {
            var dt = new DataTable();
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                new SqlDataAdapter { SelectCommand = CreateCommandWithNoReturn(sprocName, connection) }.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Returns a single string using a DataReader
        /// </summary>
        protected string RunSPdr(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters)
        {
            var deliveryAcct = String.Empty;
            var command = CreateCommand(sprocName, parameters, conn, trans);

            using (var dr = command.ExecuteReader())
            {
                //dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                    deliveryAcct = dr[0].ToString();
            }

            return deliveryAcct;
        }

        /// <summary>
        /// Returns a single integer using a DataReader
        /// </summary>
        protected int RunSPdrInt(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters)
        {
            var n = 0;

            var command = CreateCommand(sprocName, parameters, conn, trans);

            using (var dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    if (dr[0] != DBNull.Value)
                        n = Convert.ToInt16(dr[0].ToString());
                }
            }

            return n;
        }

        /// <summary>
        /// Returns a generic list of type decimal
        /// </summary>
        protected List<decimal> RunSPdrList(string sprocName, IDataParameter[] parameters)
        {
            SqlDataReader dr;
            var collection = new List<decimal>();

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand(sprocName, parameters, connection);

                using (dr = command.ExecuteReader())
                {
                    //dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        for (var i = 0; i < dr.FieldCount; i++)
                        {
                            var paymentSum = dr[i] == DBNull.Value ? 0 : Convert.ToDecimal(dr[i]);
                            collection.Add(paymentSum);
                        }
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// Returns a generic list of type decimal
        /// </summary>
        protected List<string> RunList(string sprocName, IDataParameter[] parameters = null)
        {
            SqlDataReader dr;
            var collection = new List<string>();

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand(sprocName, parameters, connection);

                using (dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        collection.Add(dr[0].ToString());
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Returns a single integer using ExecuteScalar
        /// </summary>
        protected int RunSPwithExecuteScalar(string sprocName, IDataParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var output = CreateCommandWithNoReturn(sprocName, connection, null, parameters).ExecuteScalar();
                return output != null ? Convert.ToInt16(output.ToString()) : 0;
            }
        }

        /// <summary>
        /// Returns a single integer using ExecuteScalar within a transaction
        /// </summary>
        protected int RunSPwithExecuteScalar(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters)
        {
            var output = CreateCommandWithNoReturn(sprocName, conn, trans, parameters).ExecuteScalar();
            return output != null ? Convert.ToInt16(output.ToString()) : 0;
        }

        protected ArrayList ReturnAL(string sprocName, IDataParameter[] parameters)
        {
            var al = new ArrayList();

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand(sprocName, parameters, connection);

                //dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        al.Add(dr[CN.CustomerID].ToString());
                        al.Add(dr[CN.Title].ToString());
                        al.Add(dr[CN.FirstName].ToString());
                        al.Add(dr[CN.LastName].ToString());
                        al.Add(dr[CN.RFCreditLimit].ToString());
                        al.Add(dr[CN.CreditBlocked].ToString()); //IP - 01/06/10 - UAT(1006) UAT5.2 Log
                    }
                }
            }

            return al;
        }

        private object ReturnScalar(string sprocName, IDataParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                return CreateCommand(sprocName, parameters,connection).ExecuteScalar();
            }
        }

        protected object ReturnScalarNoReturn(string sprocName, IDataParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                return CreateCommandWithNoReturn(sprocName, connection, parameters: parameters).ExecuteScalar();
            }
        }

        protected string ReturnString(string sprocName, IDataParameter[] parameters = null)
        {
            var output = ReturnScalar(sprocName, parameters);

            if (output == null || output == DBNull.Value)
                return string.Empty;

            return output.ToString();
        }

        protected bool ReturnBool(string sprocName, IDataParameter[] parameters)
        {
            var output = ReturnScalar(sprocName, parameters);
            return output != null && Convert.ToBoolean(output);
        }

        protected decimal? ReturnDecimal(string sprocName, IDataParameter[] parameters)
        {
            var output = ReturnScalar(sprocName, parameters);

            if (output == null)
                return null;

            return Convert.ToDecimal(output);
        }

        protected int? ReturnInt(string sprocName, IDataParameter[] parameters)
        {
            var output = ReturnScalar(sprocName, parameters);

            if (output == null || output == DBNull.Value)
                return null;

            return Convert.ToInt32(output);
        }

        protected int? ReturnIntNoReturn(string sprocName, IDataParameter[] parameters)
        {
            var output = ReturnScalarNoReturn(sprocName, parameters);

            if (output == null || output == DBNull.Value)
                return null;

            return Convert.ToInt32(output);
        }

        protected int? ReturnInt(SqlConnection conn, SqlTransaction trans, string sprocName, IDataParameter[] parameters)
        {
            var output = CreateCommand(sprocName, parameters, conn, trans).ExecuteScalar();

            if (output == null || output == DBNull.Value)
                return null;

            return Convert.ToInt32(output);
        }

        protected void RunNonQuery(string sprocName, IDataParameter[] parameters, bool addreturn = true)
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                CreateCommand(sprocName, parameters, connection, addreturn: addreturn).ExecuteNonQuery();
            }
        }
    }
    public enum CosacsDatabase
    {
        Live,
        Reporting
    }
}
