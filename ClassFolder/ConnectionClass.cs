using System.Configuration;
using System.Data.SqlClient;

namespace UPZhukov.ClassFolder
{
    class ConnectionClass
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
} 