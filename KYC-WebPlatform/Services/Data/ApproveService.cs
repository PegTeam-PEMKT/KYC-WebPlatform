using KYC_WebPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace KYC_WebPlatform.Services.Data
{
    public class ApproveService
    {
        DBContext dBContext = DBContext.GetInstance();

        public string GetBusinessEmailByIdToApprove(int id)
        {
            string businessEmail = "";

            try
            {
                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    string query = "SELECT Email FROM dbo.ClientBusiness WHERE BusinessId = @ID";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ID", id);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first record
                            businessEmail = reader["Email"].ToString();
                            return businessEmail;   
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From GetBusinessEmailByIdToApprove: " + ex.Message);
                
            }
            return businessEmail;
        }

        public bool ApproveClientByEmail(string email)
        {
            bool isApproved = false;

            try
            {

                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    // Define the query to update the password
                    string query = "UPDATE dbo.Clients SET IsApprovedByBusiness = @Approved, IsRejected = @Rejected WHERE ClientEmail = @ClientEmail";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ClientEmail", email);
                    command.Parameters.AddWithValue("@Approved", true);
                    command.Parameters.AddWithValue("@Rejected", false);

                    sqlConnection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine("Client approved.");
                        isApproved = true;
                        return isApproved;
                    }
                    else
                    {
                        Debug.WriteLine("No user found with the specified email.");
                        return isApproved;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From ApproveClientByEmail: " + ex.Message);
                return isApproved;
            }
        }

        public bool ApproveBusinessByEmail(string clientEmail, string approverEmail)
        {
         
            bool isApproved = false;

            string approverName = FindByEmail(approverEmail);

            try
            {

                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    // Define the query to update the password
                    string query = "UPDATE dbo.ClientBusiness SET IsApproved = @Approved, ApprovedBy = @ApproverName WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", clientEmail);
                    command.Parameters.AddWithValue("@ApproverName", approverName);
                    command.Parameters.AddWithValue("@Approved", true);

                    sqlConnection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine("Client approved.");
                        isApproved = true;
                        return isApproved;
                    }
                    else
                    {
                        Debug.WriteLine("No user found with the specified email.");
                        return isApproved;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From ApproveBusinessByEmail: " + ex.Message);
                return isApproved;
            }
        }

        public bool RejectClientByEmail(string clientEmail)
        {
            bool isRejected = false;

            try
            {

                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    // Define the query to update the password
                    string query = "UPDATE dbo.Clients SET IsApprovedByBusiness = @Approved, IsRejected = @Rejected WHERE ClientEmail = @ClientEmail";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@ClientEmail", clientEmail);
                    command.Parameters.AddWithValue("@Approved", false);
                    command.Parameters.AddWithValue("@Rejected", true);

                    sqlConnection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine("Client approved.");
                        isRejected = true;
                        return isRejected;
                    }
                    else
                    {
                        Debug.WriteLine("No user found with the specified email.");
                        return isRejected;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From ApproveClientByEmail: " + ex.Message);
                return isRejected;
            }
        }

        public bool RejectBusinessByEmail(string clientEmail, string approverEmail)
        {
            bool isApproved = false;

            string approverName = FindByEmail(approverEmail);

            try
            {

                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    // Define the query to update the password
                    string query = "UPDATE dbo.ClientBusiness SET IsApproved = @Approved, ApprovedBy = @ApproverName WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", clientEmail);
                    command.Parameters.AddWithValue("@ApproverName", approverName);
                    command.Parameters.AddWithValue("@Approved", false);

                    sqlConnection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine("Client rejected.");
                        isApproved = true;
                        return isApproved;
                    }
                    else
                    {
                        Debug.WriteLine("Did not execute query");
                        return isApproved;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From ApproveBusinessByEmail: " + ex.Message);
                return isApproved;
            }
        }

        internal string FindByEmail(string email)
        {
            string name = "";

            Debug.WriteLine("From DBContext: " + email);
            try
            {
                using (SqlConnection sqlConnection = dBContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.Users WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            name = reader["Username"].ToString();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("From FindByUser: " + e.Message);
            }

            return name;

        }

    }
}