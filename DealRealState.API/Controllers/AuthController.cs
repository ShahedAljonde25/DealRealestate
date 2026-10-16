using DataLayer.Models;
using DealRealState.Business.Services;
using DealRealState.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace DealRealState.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;


        public AuthController(AuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // استخدام await لأن عملية التحقق وجلب بيانات المستخدم غالباً تمر على الـ DB
            var response = await _authService.LoginAsync(request);

            // إذا عاد null (بسبب عدم وجود المستخدم أو خطأ في كلمة المرور)
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(response);
        }





        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // استخدام await مع دالة الـ Service غير المتزامنة
            var result = await _authService.RefreshTokenAsync(request);

            if (result == null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LogoutAsync(request);

            if (!result)
                return BadRequest("Logout failed");

            return Ok("Logged out successfully");
        }
    }
}