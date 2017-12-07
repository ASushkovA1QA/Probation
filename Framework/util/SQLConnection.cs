using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.util
{
    /// <summary>
    /// For work with SQL Server database
    /// </summary>
    public class SQLConnection
    {
        //List of all opened connections
        private static readonly List<SqlConnection> ConnectionsPool = new List<SqlConnection>();

        /// <summary>
        /// singleton
        /// </summary>
        public static SqlConnection Instance(string connectionString)
        {
            foreach (var connection in ConnectionsPool)
            {
                if (connection.ConnectionString == connectionString)
                    return connection;
            }
            SqlConnection con;
            try
            {
                con = new SqlConnection(connectionString);
                con.Open();
            }
            catch (Exception e)
            {
                Logger.Instance.Error("SqlConnection Error: " + e.Message);
                throw;
            }

            ConnectionsPool.Add(con);
            return con;
        }

        /// <summary>
        /// Sends commandString to the connection with connectionString and reads a stream of rows from a SQL Server database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="commandString">Command for execution</param>
        /// <returns>Data read from a SQL Server database</returns>
        public static List<Object[]> ExecuteReaderCmd(string connectionString, string commandString)
        {
            var dataList = new List<Object[]>();
            try
            {
                using (var command = new SqlCommand(commandString, Instance(connectionString)))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var a = new Object[reader.FieldCount];
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                a[i] = reader[i];
                            }
                            dataList.Add(a);
                        }
                        return dataList;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error("SqlCommand ExecuteReader Error: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends commandString to the connection with connectionString and reads the first column of the first row in the result
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="commandString">Command for execution</param>
        /// <returns>The first column of the first row in the result</returns>
        public static int ExecuteScalarCmd(string connectionString, string commandString)
        {
            try
            {
                using (var command = new SqlCommand(commandString, Instance(connectionString)))
                {
                    return (int) command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error("SqlCommand ExecuteScalar Error: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends commandString to the connection with connectionString and executes a Transact-SQL statement against the connection
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="commandString">Command for execution</param>
        /// <returns>The number of rows affected</returns>
        public static int ExecuteNonQueryCmd(string connectionString, string commandString)
        {
            try
            {
                using (var command = new SqlCommand(commandString, Instance(connectionString)))
                {
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error("SqlCommand ExecuteNonQuery Error: " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Close all connections in ConnectionsPool
        /// </summary>
        public static void CloseConnectons()
        {
            foreach (var connection in ConnectionsPool)
            {
                connection.Close();
            }
        }
    }
}
