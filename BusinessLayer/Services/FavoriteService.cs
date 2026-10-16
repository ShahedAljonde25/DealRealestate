using DataLayer.Models;
using DealRealState.Data.DataAccess;
using DealRealState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace BusinessLayer.Services
{
    public class FavoriteService
    {

        public static async Task<List<Favorite>> GetUserFavoritesAsync(int userID)
        {
            if (userID <= 0)
                return new List<Favorite>();

            // استدعاء دالة الـ Data Layer بشكل غير متزامن
            return await FavoriteData.GetUserFavoritesAsync(userID);
        }


        public static async Task<bool> ToggleFavoriteAsync(int userID, int propertyID, FavoriteMode mode)
        {
            // Business Logic Validation: Ensure IDs are valid before making a database call
            if (userID <= 0 || propertyID <= 0)
            {
                return false;
            }

            // Await the asynchronous Data Access Layer method
            return await FavoriteData.ProcessFavoriteAsync(userID, propertyID, mode);
        }

        public bool CheckFavorite(int userID, int propertyID)
        {
            return FavoriteData.CheckFavorite(userID, propertyID);
        }
    }
}
