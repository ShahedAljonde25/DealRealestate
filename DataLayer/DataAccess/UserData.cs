using Microsoft.Data.SqlClient;
using System.Data;
using DataLayer.Connection;
using DataLayer.Models;


namespace DataLayer.DataAccess
{
    public class UserData
    {

        public static async Task<User?> GetUserByIDAsync(int userID)
        {
            User? user = null;

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userID);

                    // فتح الاتصال بشكل غير متزامن
                    await connection.OpenAsync();

                    // تنفيذ القراءة بشكل غير متزامن
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // قراءة السجل بشكل غير متزامن
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                email = reader["email"]?.ToString() ?? string.Empty,
                                password = reader["password"]?.ToString() ?? string.Empty,
                                role = reader["role"]?.ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }

            return user;
        }


        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetAllUsers",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                connection.Open();


                SqlDataReader reader =
                    command.ExecuteReader();



                while (reader.Read())
                {

                    User user = new User();


                    user.UserID =
                        Convert.ToInt32(reader["UserID"]);


                    user.Name =
                        reader["Name"].ToString();


                    user.email =
                        reader["email"].ToString();


                    user.password =
                        reader["password"].ToString();


                    user.role =
                        reader["role"].ToString();



                    users.Add(user);

                }

            }


            return users;
        }



        public static async Task<int> AddUserAsync(User user)
        {
            int userID = 0;

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_AddUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", user.Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", user.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@password", user.password ?? (object)DBNull.Value);

                    command.Parameters.AddWithValue("@EmailVerificationToken", user.EmailVerificationToken ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VerificationTokenExpires", user.VerificationTokenExpires ?? (object)DBNull.Value);

                    await connection.OpenAsync();

                    object result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        userID = Convert.ToInt32(result);
                    }
                }
            }

            return userID;
        }


        public static bool UpdateUser(User user)
        {

            bool result = false;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_UpdateUser",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@UserID",
                    user.UserID
                );


                command.Parameters.AddWithValue(
                    "@Name",
                    user.Name
                );


                command.Parameters.AddWithValue(
                    "@Email",
                    user.email
                );


                connection.Open();


                int rows =
                    command.ExecuteNonQuery();


                result = rows > 0;

            }


            return result;
        }


        public static async Task<bool> VerifyEmailAsync(string token)
        {
            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_VerifyEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Token", token ?? (object)DBNull.Value);

                    await connection.OpenAsync();
                    object result = await command.ExecuteScalarAsync();

                    // يعود بـ 1 في حال نجاح التوثيق، و 0 في حال انتهاء الصلاحية أو خطأ الـ Token
                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
        }
        public static bool DeleteUser(int userID)
        {

            bool result = false;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_DeleteUser",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@UserID",
                    userID
                );


                connection.Open();


                int rows =
                    command.ExecuteNonQuery();


                result = rows > 0;

            }


            return result;
        }
        public static async Task<User?> GetUserByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (var command = new SqlCommand("sp_GetUserByEmail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);

                    // فتح الاتصال بشكل غير متزامن
                    await connection.OpenAsync();

                    // تنفيذ القراءة بشكل غير متزامن
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // استخدام ReadAsync بدلاً من Read العادية
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                email = reader["email"]?.ToString() ?? string.Empty,
                                password = reader["password"]?.ToString() ?? string.Empty,
                                UserID = Convert.ToInt32(reader["UserID"]),
                                role = reader["role"]?.ToString() ?? string.Empty,
                            };
                        }
                    }
                }
            }

            return null; // في حال لم يتم العثور على المستخدم
        }

    }
}