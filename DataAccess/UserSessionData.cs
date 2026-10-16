using DataLayer.Connection;
using DataLayer.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DataLayer.DataAccess
{
    public class UserSessionData
    {
        public static async Task<bool> AddSessionAsync(UserSession session)
        {
            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_AddUserSession", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserID", session.UserID);
                    command.Parameters.AddWithValue("@RefreshTokenHash", session.RefreshTokenHash);
                    command.Parameters.AddWithValue("@CreatedAt", session.CreatedAt);
                    command.Parameters.AddWithValue("@ExpiresAt", session.ExpiresAt);
                    command.Parameters.AddWithValue("@IpAddress", (object?)session.IpAddress ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UserAgent", (object?)session.UserAgent ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DeviceName", (object?)session.DeviceName ?? DBNull.Value);

                    await connection.OpenAsync();
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        public static async Task<UserSession?> GetSessionByUserIDAsync(int userID)
        {
            UserSession? session = null;

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserSessionByUserID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userID);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            session = new UserSession
                            {
                                SessionID = (int)reader["SessionID"],
                                UserID = (int)reader["UserID"],
                                RefreshTokenHash = reader["RefreshTokenHash"].ToString()!,
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                ExpiresAt = (DateTime)reader["ExpiresAt"],
                                RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTime?)reader["RevokedAt"],
                                IpAddress = reader["IpAddress"] == DBNull.Value ? null : reader["IpAddress"].ToString(),
                                UserAgent = reader["UserAgent"] == DBNull.Value ? null : reader["UserAgent"].ToString(),
                                DeviceName = reader["DeviceName"] == DBNull.Value ? null : reader["DeviceName"].ToString()
                            };
                        }
                    }
                }
            }

            return session;
        }

        public static async Task<bool> RevokeSessionAsync(int sessionID)
        {
            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_RevokeUserSession", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SessionID", sessionID);

                    await connection.OpenAsync();
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        public static async Task<bool> DeleteExpiredSessionsAsync()
        {
            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_DeleteExpiredSessions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        public static async Task<bool> UpdateSessionAsync(UserSession session)
        {
            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_UpdateUserSession", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SessionID", session.SessionID);
                    command.Parameters.AddWithValue("@RefreshTokenHash", session.RefreshTokenHash);
                    command.Parameters.AddWithValue("@CreatedAt", session.CreatedAt);
                    command.Parameters.AddWithValue("@ExpiresAt", session.ExpiresAt);

                    await connection.OpenAsync();
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }
    }
}