using KYC_WebPlatform.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Services.Data
{
    public class ClientService : IStorage
    {
        DBContext dbContext = DBContext.GetInstance();
        public List<object> ExecuteSelectQuery(string query)
        {
            // Initialize the list to hold the result
            List<object> resultList = new List<object>();

            // Establish a connection to the database
            using (SqlConnection connection = dbContext.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Create a SqlCommand to execute the query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the query and obtain a SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Iterate through the rows
                            while (reader.Read())
                            {

                                // Loop through each column in the row
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    resultList.Add(reader.GetValue(i));
                                }

                                // Add the row to the result list

                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL errors
                    Console.WriteLine("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle other possible errors
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return resultList;

        }
    }
}