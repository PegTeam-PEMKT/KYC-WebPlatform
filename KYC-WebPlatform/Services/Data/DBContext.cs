using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Services.Data
{
    public class DBContext
    {
        private static DBContext dBInstance;
        private SqlConnection sqlConnection;
        private string _connectionString;

        private DBContext()
        {
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["KYC-Connection"].ConnectionString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static DBContext GetInstance()
        {
            if (dBInstance == null)
            {
                dBInstance = new DBContext();
            }
            return dBInstance;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}