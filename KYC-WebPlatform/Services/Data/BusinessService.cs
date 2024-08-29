using KYC_WebPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KYC_WebPlatform.Services.Data
{
    public class BusinessService
    {
        public bool SaveBusinessInfo(AddBusiness_MODEL model)
        {
            bool businessInfoSaved = false;
            try
            {
                DBContext dbContext = DBContext.GetInstance();
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    // Open the connection
                    connection.Open();
                    Debug.WriteLine("NIN: " + model.NIN + " BusinessName: " + model.BusinessName);
                    string sqlCommand = "INSERT INTO Directors (DirectorId, DirectorNIN, BusinessId) VALUES (@DirectorId, @NIN, @BusinessName)";
                    using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                    {
                        command.Parameters.AddWithValue("@DirectorId", (string)model.DirectorPhoneNumber);
                        command.Parameters.AddWithValue("@NIN", model.NIN);
                        command.Parameters.AddWithValue("@BusinessName", model.BusinessName);
                        command.ExecuteNonQuery();
                    }
                    //Close the connection
                    connection.Close();
                }

                businessInfoSaved = true;
                return businessInfoSaved;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return businessInfoSaved;
            }
            
        }
    }
}