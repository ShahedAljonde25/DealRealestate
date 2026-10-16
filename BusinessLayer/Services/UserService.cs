using DataLayer.DataAccess;
using DataLayer.Models;
using DealRealState.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealRealState.Business.Services
{
    public class UserService
    {

        public async Task<User?> GetUserByIDAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            // إضافة await هنا لفك الـ Task والحصول على كائن User مباشرة
            return await UserData.GetUserByIDAsync(id);
        }



        public List<User> GetAllUsers()
        {
            return UserData.GetAllUsers();
        }




        public bool UpdateUser(User user)
        {
            return UserData.UpdateUser(user);
        }

        public async Task<bool> VerifyEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return await UserData.VerifyEmailAsync(token);
        }




        public bool DeleteUser(int id)
        {
            return UserData.DeleteUser(id);
        }


        public bool GetuserByEmail(string email)

        {
            var user = UserData.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        // NEW
        public async Task<int> AddUser(CreateUserDTO dto, string token)
        {
            var user = new User
            {
                Name = dto.Name,
                email = dto.Email,
                password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                IsEmailVerified = false,
                EmailVerificationToken = token,
                VerificationTokenExpires = DateTime.UtcNow.AddHours(1) 
            };

            return await UserData.AddUserAsync(user);
        }
    }
}