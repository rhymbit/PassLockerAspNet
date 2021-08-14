using System;
using System.Data.SqlClient;

namespace TestingConsole
{
    public class CheckSqlConnection
    {
        private const string ConnectionString =
            "Data Source=acc;Initial Catalog=PassLocker;User id=sa;Password=Prateek332@#;";
        
        public static void Connect()
        {
            using var connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connected successfully");
            }
            catch (SqlException exp)
            {
                Console.WriteLine("Sql exception has occurred.");
            }
        }
    }
}