using KYC_WebPlatform.Models;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Helpers;

namespace KYC_WebPlatform.Services.Data
{
    public class SecurityDAO
    {
        DBContext dbContext = DBContext.GetInstance();

        internal bool CreateUser(SignupDto signupDto)
        {
            bool userCreated = false;
            DateTime dateTime = DateTime.Now;

            if (FindByEmail(signupDto.Email))
            {
                userCreated = false;
                return userCreated;
            }
            else
            {
                InsertRole(signupDto.Role.ToString());
                if (signupDto.Role == UserRole.Admin) 
                { 
                    if(InsertAdmin(signupDto.Username, signupDto.Password))
                    {
                        Debug.WriteLine("Admin Created");
                    }
                    else
                    {
                        Debug.WriteLine("Failure creating admin");
                    }
                }
                if (signupDto.Role == UserRole.DepartmentHead)
                {
                    if(InsertDepartmentHead(signupDto.Username, signupDto.Email, signupDto.Password))
                    {
                        Debug.WriteLine("DeptHead Created");
                    }
                    else
                    {
                        Debug.WriteLine("Failure creating DeptHead");
                    }
                }
                try
                {
                    using (SqlConnection connection = dbContext.GetConnection())
                    {
                        connection.Open();

                        // Insert into Users table
                        string userQuery = "INSERT INTO Users (UserName, PasswordHash, Email, PhoneNumber, RoleId, IsActive, CreatedDate) " +
                                           "VALUES (@Username, @Password, @Email, @PhoneNumber, @RoleId, @IsActive, @CreatedDate)";

                        SqlCommand userCommand = new SqlCommand(userQuery, connection);
                        userCommand.Parameters.AddWithValue("@Username", signupDto.Username);
                        userCommand.Parameters.AddWithValue("@Password", signupDto.Password);
                        userCommand.Parameters.AddWithValue("@Email", signupDto.Email);
                        userCommand.Parameters.AddWithValue("@PhoneNumber", signupDto.PhoneNumber);
                        userCommand.Parameters.AddWithValue("@RoleId", (int)signupDto.Role); // Cast enum to int
                        userCommand.Parameters.AddWithValue("@IsActive", false);
                        userCommand.Parameters.AddWithValue("@CreatedDate", dateTime);

                        userCommand.ExecuteNonQuery();
                        userCreated = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("From CreateUser: " + e.Message);
                }

                return userCreated;
            }
        }

        internal bool FindByUser(string email, string password)
        {
            bool isValidUser = false;

            Debug.WriteLine("From DBContext: " + email);
            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.users WHERE Email = @Email AND PasswordHash = @Password";

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

        internal bool FindByEmail(string email)
        {
            bool isValidUser = false;

            Debug.WriteLine("From DBContext: " + email);
            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.users WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);

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

        internal bool InsertRole(string role) 
        {
            try
            {
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    connection.Open();

                    // Insert into Users table
                    string userQuery = "INSERT INTO Roles (RoleName) " +
                                       "VALUES (@RoleName)";

                    SqlCommand userCommand = new SqlCommand(userQuery, connection);
                    userCommand.Parameters.AddWithValue("@RoleName", role);

                    userCommand.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("From CreateUser: " + e.Message);
                return false;
            }
        }

        internal bool InsertAdmin(string adminUserName, string adminHashPassword)
        {
            bool adminCreated = false;

            using (SqlConnection connection = dbContext.GetConnection())
            {
                string query = "INSERT INTO Admin (AdminUserName, AdminHashPassword) VALUES (@AdminUserName, @AdminHashPassword)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminUserName", adminUserName);
                command.Parameters.AddWithValue("@AdminHashPassword", adminHashPassword);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Check if the insert was successful
                    if (result > 0)
                    {
                        adminCreated = true;
                        return adminCreated;
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred from InsertAdmin: " + ex.Message);
                }

                return adminCreated;
            }
        }

        internal bool InsertDepartmentHead(string name, string email, string phone)
        {
            Random rand = new Random();
            int deptId = rand.Next(999);
            bool deptHeadCreated = false;
            try
            {
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.HeadOfDepartment (PersonName, DepartmentId, Email, Telephone) VALUES (@PersonName, @DepartmentId, @Email, @Telephone)", connection);

                    command.Parameters.AddWithValue("@PersonName", name);
                    command.Parameters.AddWithValue("@DepartmentId", deptId.ToString());
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Telephone", phone);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        deptHeadCreated = true; 
                        return deptHeadCreated;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("From InsertDepartmentHead: " + ex.Message);
            }

            return deptHeadCreated;
            
        }
    }
}