using DataLayer.DataAccess;
using DataLayer.Models;
using DealRealState.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace DealRealState.Business.Services
{
    public class AuthService
    {

        public async Task<TokenResponseDTO?> LoginAsync(LoginRequestDto request)
        {
            // 1. التأكد من أن الطلب ليس null
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                Console.WriteLine("[LOGIN FAILED] Null or empty request");
                return null;
            }

            // 2. البحث عن المستخدم في قاعدة البيانات (باستخدام await والنسخة Async)
            var user = await UserData.GetUserByEmailAsync(request.Email);

            // 3. الفحص أولاً قبل قراءة أي خاصية مثل user.email!
            if (user == null)
            {
                Console.WriteLine($"[LOGIN FAILED] User with email '{request.Email}' NOT FOUND");
                return null;
            }

            Console.WriteLine($"[LOGIN ATTEMPT] Found user: {user.email}");

            // 4. التحقق من كلمة المرور
            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.password);

            if (!passwordValid)
            {
                Console.WriteLine("[LOGIN FAILED] Password incorrect");
                return null;
            }

            // 5. توليد التوكينات والجلسة
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var session = new UserSession
            {
                UserID = user.UserID,
                RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            // حفظ الجلسة بشكل غير متزامن
            await UserSessionData.AddSessionAsync(session);
            Console.WriteLine($"[LOGIN SUCCESS] UserID: {user.UserID} logged in successfully.");

            return new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }






















        private  string GenerateAccessToken(User user)
        {

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()
                ),


                new Claim(
                    ClaimTypes.Email,
                    user.email
                ),


                new Claim(
                    ClaimTypes.Role,
                    user.role
                )
            };



            var jwtKey =
                Environment.GetEnvironmentVariable("JWT_KEY");



            if (string.IsNullOrEmpty(jwtKey))
                throw new Exception("JWT_KEY is missing");



            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );



            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );



            var token = new JwtSecurityToken(
                issuer:
                    Environment.GetEnvironmentVariable("JWT_ISSUER"),

                audience:
                    Environment.GetEnvironmentVariable("JWT_AUDIENCE"),

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(30),

                signingCredentials:
                    credentials
            );



            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }





        private  string GenerateRefreshToken()
        {

            var bytes = new byte[64];


            using var rng =
                RandomNumberGenerator.Create();


            rng.GetBytes(bytes);


            return Convert.ToBase64String(bytes);
        }

        public async Task<bool> LogoutAsync(LogoutRequestDTO request)
        {
            if (request == null)
            {
                Console.WriteLine("[LOGOUT FAILED] Request is null");
                return false;
            }

            // استخدام await للنسخة غير المتزامنة
            var session = await UserSessionData.GetSessionByUserIDAsync(request.UserID);

            if (session == null)
            {
                Console.WriteLine($"[LOGOUT FAILED] No session found for UserID: {request.UserID}");
                return false;
            }

            // فحص ما إذا كانت الجلسة ملغية مسبقاً
            if (session.RevokedAt != null)
            {
                Console.WriteLine($"[LOGOUT FAILED] Session already revoked for UserID: {request.UserID}");
                return false;
            }

            // التحقق من الـ RefreshToken (عملية CPU Bound تبقى كما هي بدون await)
            bool tokenValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, session.RefreshTokenHash);

            if (!tokenValid)
            {
                Console.WriteLine($"[LOGOUT FAILED] RefreshToken does not match stored hash for UserID: {request.UserID}");
                return false;
            }

            // إلغاء الجلسة في قاعدة البيانات بشكل غير متزامن
            bool isRevoked = await UserSessionData.RevokeSessionAsync(session.SessionID);

            if (isRevoked)
            {
                Console.WriteLine($"[LOGOUT SUCCESS] UserID: {request.UserID} logged out.");
            }

            return isRevoked;
        }

        public async Task<TokenResponseDTO?> RefreshTokenAsync(RefreshRequestDTO request)
        {
            if (request == null)
                return null;

            // استخدام await لجلب الجلسة بشكل غير متزامن
            var session = await UserSessionData.GetSessionByUserIDAsync(request.UserID);

            if (session == null)
                return null;

            if (session.RevokedAt != null)
                return null;

            if (session.ExpiresAt <= DateTime.UtcNow)
                return null;

            // التحقق من صحة الـ RefreshToken (CPU Bound)
            bool validToken = BCrypt.Net.BCrypt.Verify(
                request.RefreshToken,
                session.RefreshTokenHash
            );

            if (!validToken)
                return null;

            // استخدام النسخة Async لجلب بيانات المستخدم
            var user = await UserData.GetUserByIDAsync(request.UserID);

            if (user == null)
                return null;

            // Generate new JWT
            var newAccessToken = GenerateAccessToken(user);

            // Token Rotation
            var newRefreshToken = GenerateRefreshToken();

            session.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            session.CreatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow.AddDays(7);

            // تحديث الجلسة في قاعدة البيانات بشكل غير متزامن
            await UserSessionData.UpdateSessionAsync(session);

            return new TokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }


    }
}