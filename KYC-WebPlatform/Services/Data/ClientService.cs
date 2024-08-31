using KYC_WebPlatform.Services.Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;

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
                    connection.Close();
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


        public Dictionary<string, List<object>> ExecuteSelectQuery(string query, params SqlParameter[] parameters)
        {
            // Initialize the dictionary to hold the result
            Dictionary<string, List<object>> resultDictionary = new Dictionary<string, List<object>>();

            // Establish a connection to the database
            using (SqlConnection connection = dbContext.GetConnection())
            {
                try
                {
                    connection.Open();

                    // Create a SqlCommand to execute the query and fill the DataTable
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure; // Assuming you are using a stored procedure

                        // Add the parameters to the SqlCommand
                        foreach (SqlParameter parameter in parameters)
                        {
                            Debug.WriteLine("Adding parameter: " + parameter.ParameterName + " = " + parameter.Value);
                            command.Parameters.Add(parameter);
                        }

                        // Create a SqlDataAdapter
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to hold the results
                            DataTable dataTable = new DataTable();
                            try
                            {
                                // Fill the DataTable with data
                                adapter.Fill(dataTable);

                                Debug.WriteLine("Iterating through rows....");

                                // Iterate through the rows in the DataTable
                                int rowCount = 0;
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    // Create a list to hold the column values for this row
                                    List<object> columnValues = new List<object>();

                                    // Loop through each column in the row
                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        Debug.WriteLine("Getting column value: " + row[column]);
                                        // Get the column value
                                        object columnValue = row[column];

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
                            catch (Exception ex)
                            {
                                Debug.WriteLine("An error occurred while filling the DataTable: " + ex.Message);
                            }
                        }
                    }

                    connection.Close();
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

        public int ExecuteInsertQuery(string query, params SqlParameter[] parameters)
        {
            int rowsAffected = 0;
             using (SqlConnection connection = dbContext.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            Debug.WriteLine(parameters.Length);
                            command.Parameters.AddRange(parameters);
                        }

                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
                catch (SqlException sq)
                {
                    Debug.WriteLine(sq.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            return rowsAffected;
        }

        internal int ExecuteGetIdQuery(string query, string TIN)
        {
            int businessId = 0;

            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@BusinessTIN", TIN);

                    Debug.WriteLine("From ExecuteGetIdQuery:" + TIN);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first record
                            businessId = reader.GetInt32(0);
                        }
                    }
                    return businessId;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From ExecuteGetIdQuery: " + ex.Message);
            }
            return businessId;
            
        }
    }
}
