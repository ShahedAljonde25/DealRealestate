using DealRealState.Models;
using DataLayer.Connection;
using Microsoft.Data.SqlClient;
using System.Data;
using DTOs;
using DealRealState.Models;

namespace DealRealState.Data.DataAccess
{
    public class FavoriteData
    {


        public static bool CheckFavorite(int userID, int propertyID)
        {
            bool exists = false;

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                SqlCommand command = new SqlCommand("sp_CheckFavorite", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@PropertyID", propertyID);

                connection.Open();
                exists = Convert.ToBoolean(command.ExecuteScalar());
            }

            return exists;
        }


        public static async Task<List<Favorite>> GetUserFavoritesAsync(int userID)
        {
            List<Favorite> favorites = new List<Favorite>();

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetUserFavorites", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userID);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Favorite favorite = new Favorite();

                            // بيانات المفضلة
                            favorite.FavoriteID = Convert.ToInt32(reader["FavoriteID"]);
                            favorite.UserID = Convert.ToInt32(reader["UserID"]);
                            favorite.PropertyID = Convert.ToInt32(reader["PropertyID"]);
                            favorite.CreatedAt = Convert.ToDateTime(reader["FavoritedAt"]); // حسب ما سميناها في الـ SP

                            // بيانات العقار المدمجة (تأكدي من تطابق أسماء الأعمدة)
                            favorite.Title = reader["Title"]?.ToString();
                            favorite.Description = reader["Description"]?.ToString();
                            favorite.Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0;
                            favorite.PropertyType = reader["PropertyType"]?.ToString();
                            favorite.Location = reader["Location"]?.ToString();
                            favorite.City = reader["City"]?.ToString();
                            favorite.Bedrooms = reader["Bedrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bedrooms"]) : 0;
                            favorite.Bathrooms = reader["Bathrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bathrooms"]) : 0;
                            favorite.AreaSQM = reader["AreaSQM"] != DBNull.Value ? Convert.ToDecimal(reader["AreaSQM"]) : 0;

                            favorites.Add(favorite);
                        }
                    }
                }
            }

            return favorites;
        }


        public static async Task<bool> ProcessFavoriteAsync(int userID, int propertyID, FavoriteMode mode)
        {
            string storedProcedureName = mode switch
            {
                FavoriteMode.Add => "sp_AddFavorite",
                FavoriteMode.Remove => "sp_RemoveFavorite",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                return false;
            }

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@PropertyID", propertyID);

                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }
    }





}
