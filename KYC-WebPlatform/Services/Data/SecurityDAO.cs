using KYC_WebPlatform.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Helpers;

namespace KYC_WebPlatform.Services.Data
{
    public class SecurityDAO
    {
        DBContext dbContext = DBContext.GetInstance();

        internal bool FindByUser(LoginDto logindto)
        {
            bool isValidUser = false;
            string email = logindto.Email;
            string password = logindto.Password;

            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.users WHERE Email = @Email AND Password = @Password";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            isValidUser = true; // If rows are returned, the user is valid
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("From FindByUser: " + e.Message);
            }
            
            return isValidUser;

        }
    }
}