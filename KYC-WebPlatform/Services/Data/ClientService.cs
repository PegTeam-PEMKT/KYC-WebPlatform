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
        public Dictionary<string, List<object>> ExecuteSelectQuery(string query)
        {
            // Initialize the dictionary to hold the result
            Dictionary<string, List<object>> resultDictionary = new Dictionary<string, List<object>>();

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
                            int rowCount = 0;
                            while (reader.Read())
                            {
                                // Create a list to hold the column values for this row
                                List<object> columnValues = new List<object>();

                                // Loop through each column in the row
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    // Get the column value
                                    object columnValue = reader.GetValue(i);

                                    // Add the column value to the list
                                    columnValues.Add(columnValue);
                                }

                                // Create a key for the row (e.g., "Row 1", "Row 2", etc.)
                                string rowKey = $"Row {rowCount + 1}";

                                // Add the row to the result dictionary
                                resultDictionary.Add(rowKey, columnValues);

                                rowCount++;
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

            return resultDictionary;
        }
    }
}