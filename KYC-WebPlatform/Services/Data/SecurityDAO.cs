using KYC_WebPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Helpers;
using System.Web.Security;

namespace KYC_WebPlatform.Services.Data
{
    public class SecurityDAO
    {
        DBContext dbContext = DBContext.GetInstance();

        internal bool CreateUser(SignupDto signupDto)
        {
            bool userCreated = false;
            DateTime dateTime = DateTime.Now;
            int roleId = (int)signupDto.Role;

            // Hash the password
            string hashedPassword = Crypto.HashPassword(signupDto.Password);

            if (FindByEmail(signupDto.Email))
            {
                userCreated = false;
                return userCreated;
            }
            else
            {
                if (signupDto.Role == UserRole.Admin && signupDto.DeptRole == DeptRole.Business)
                {
                    Debug.WriteLine("Failure creating admin");
                    return userCreated;
                }
                if (signupDto.Role == UserRole.Admin && !(signupDto.DeptRole == DeptRole.Business))
                {
                    Debug.WriteLine("Failure creating admin");
                    return userCreated;
                }
                if (signupDto.Role == UserRole.Admin)
                {
                    if (InsertAdmin(signupDto.Username, hashedPassword))
                    {
                        Debug.WriteLine("Admin Created");
                        if (InsertDepartmentHead("BUSINESS#001", signupDto.Username, 5, signupDto.Email, signupDto.PhoneNumber))
                        {
                            Debug.WriteLine("DeptHead Created");
                        }
                        else
                        {
                            Debug.WriteLine("Failure creating DeptHead");
                            return userCreated;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Failure creating admin");
                        return userCreated;
                    }
                }
                if (signupDto.Role == UserRole.DepartmentHead)
                {
                    var deptHeadIds = new Dictionary<int, string>
                    {
                        { 1, "TECH#001" },
                        { 2, "HRLEGAL#001" },
                        { 3, "SUPPORT#001" },
                        { 4, "FINANCE#001" },
                        { 5, "BUSINESS#001" },
                        { 6, "SALES#006" }
                    };

                    string deptHeadId = deptHeadIds.TryGetValue((int)signupDto.DeptRole, out var id) ? id : "DEFAULT_ID";

                    if (InsertDepartmentHead(deptHeadId, signupDto.Username, (int)signupDto.DeptRole, signupDto.Email, signupDto.PhoneNumber))
                    {
                        Debug.WriteLine("DeptHead Created");
                    }
                    else
                    {
                        Debug.WriteLine("Failure creating DeptHead");
                    }
                }
                if (signupDto.Role == UserRole.Client)
                {
                    if (InsertClient(signupDto))
                    {
                        Debug.WriteLine("Client inserted successfully");
                    }
                    else
                    {
                        Debug.WriteLine("Client not inserted!!");
                        return userCreated;
                    }
                }
                try
                {
                    using (SqlConnection connection = dbContext.GetConnection())
                    {
                        if ((int)signupDto.Role == 0)
                        {
                            roleId = 15;
                            if (InsertClient(signupDto))
                            {
                                Debug.WriteLine("Client inserted successfully");
                            }
                            else
                            {
                                Debug.WriteLine("Client not inserted!!");
                            }
                        }
                        connection.Open();

                        // Insert into Users table
                        string userQuery = "INSERT INTO Users (UserName, PasswordHash, Email, PhoneNumber, RoleId, IsActive, CreatedDate) " +
                                           "VALUES (@Username, @Password, @Email, @PhoneNumber, @RoleId, @IsActive, @CreatedDate)";

                        SqlCommand userCommand = new SqlCommand(userQuery, connection);
                        userCommand.Parameters.AddWithValue("@Username", signupDto.Username);
                        userCommand.Parameters.AddWithValue("@Password", hashedPassword);
                        userCommand.Parameters.AddWithValue("@Email", signupDto.Email);
                        userCommand.Parameters.AddWithValue("@PhoneNumber", signupDto.PhoneNumber);
                        userCommand.Parameters.AddWithValue("@RoleId", roleId); // Cast enum to int
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

        internal bool FindByUser(LoginDto loginDto)
        {
            bool isValidUser = false;
            string storedHash = "";

            Debug.WriteLine("From DBContext: " + loginDto.Email);
            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.Users WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", loginDto.Email);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first record
                            storedHash = reader["PasswordHash"].ToString();
                        }
                    }
                }
                if (Crypto.VerifyHashedPassword(storedHash, loginDto.Password))
                {
                    isValidUser = true; // If unhashed password matches user inputed password, the user is valid
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
                    string query = "SELECT * FROM dbo.Users WHERE Email = @Email";

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

        internal bool InsertAdmin(string adminUserName, string adminHashPassword)
        {
            bool adminCreated = false;

            using (SqlConnection connection = dbContext.GetConnection())
            {
                string query = "INSERT INTO Admin (AdminUserName) VALUES (@AdminUserName)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminUserName", adminUserName);

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

        internal bool InsertDepartmentHead(string deptHeadId, string name, int deptId, string email, string phone)
        {
            Random rand = new Random();
            bool deptHeadCreated = false;
            try
            {
                using (SqlConnection connection = dbContext.GetConnection())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO dbo.HeadOfDepartment (DepartmentHeadId, PersonName, DepartmentId, Email, Telephone) VALUES (@DeptHeadId, @PersonName, @DepartmentId, @Email, @Telephone)", connection);

                    command.Parameters.AddWithValue("@DeptHeadId", deptHeadId);
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

        internal int RetrieveRole(string email)
        {
            int roleId = 0;
            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT RoleId FROM dbo.Users WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);

                    Debug.WriteLine("From RetrieveRole: " + email);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first record
                            roleId = reader.GetInt32(0);
                        }
                    }
                    return roleId;
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("From RetrieveRole: " + e.Message);
            }
            return roleId;
        }

        internal string RetrieveName(string email)
        {
            string userName = "";

            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    string query = "SELECT * FROM dbo.Users WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);

                    sqlConnection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first record
                            userName = reader["UserName"].ToString();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("From RetrieveName: " + e.Message);
            }
            return userName;
        }

        internal bool ChangePasswordForEmail(LoginDto loginDto)
        {
            string hashedPassword = Crypto.HashPassword(loginDto.Password);
            bool updatedPassword = false;

            try
            {
                using (SqlConnection sqlConnection = dbContext.GetConnection())
                {
                    // Define the query to update the password
                    string query = "UPDATE dbo.Users SET PasswordHash = @Password WHERE Email = @Email";

                    SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@Email", loginDto.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword); // assuming newPassword is the new password you want to set

                    sqlConnection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine("Password updated successfully.");
                        updatedPassword = true;
                        return updatedPassword;
                    }
                    else
                    {
                        Debug.WriteLine("No user found with the specified email.");
                        return updatedPassword;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("From UpdatePassword: " + e.Message);
                return updatedPassword;
            }
        }

        internal bool InsertClient (SignupDto signupDto)
        {
            bool isClientCreated = false;
            DateTime dateTime = DateTime.Now;

            try
            {
                using (SqlConnection connection = dbContext.GetConnection())
                {

                    connection.Open();

                    // Insert into Users table
                    string userQuery = "INSERT INTO Clients (Username, ClientEmail, PhoneNumber, CreatedDate) " +
                                       "VALUES (@Username, @Email, @PhoneNumber, @CreatedDate)";

                    SqlCommand userCommand = new SqlCommand(userQuery, connection);
                    userCommand.Parameters.AddWithValue("@Username", signupDto.Username);
                    userCommand.Parameters.AddWithValue("@Email", signupDto.Email);
                    userCommand.Parameters.AddWithValue("@PhoneNumber", signupDto.PhoneNumber);
                    userCommand.Parameters.AddWithValue("@CreatedDate", dateTime);

                    userCommand.ExecuteNonQuery();
                    isClientCreated = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("From CreateUser: " + e.Message);
                return isClientCreated;
            }

            return isClientCreated;
        }
    }
}