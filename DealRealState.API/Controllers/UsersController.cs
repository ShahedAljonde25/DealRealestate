using Microsoft.AspNetCore.Mvc;
using DealRealState.Business.Services;
using Microsoft.AspNetCore.Authorization;
using DataLayer.Models;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.RateLimiting;
using DealRealState.DTOs;
namespace DealRealState.API.Controllers
{
    [ApiController]
    [Route("api/users")]

    
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        // Dependency Injection via Constructor
        public UsersController(UserService userService,EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [Authorize(Roles = "Seller")]

        [HttpGet]

        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "SellerOrSelf")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIDAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ توليد Token عشوائي وآمن للتوثيق
            var verificationToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

            // 2️⃣ إرسال الـ token للخدمة مع بيانات المستخدم (حفظ الحساب كـ Unverified)
            var userID = await _userService.AddUser(dto, verificationToken);

            if (userID == -1)
                return Conflict(new { message = "Email already exists" });

            // 3️⃣ تجهيز رابط التفعيل وإرسال البريد الإلكتروني
            var verificationUrl = $"https://localhost:8000/api/users/verify-email?token={verificationToken}";
            await _emailService.SendVerificationEmailAsync(dto.Email, verificationUrl);

            return Ok(new
            {
                message = "User created successfully. Please check your email to verify your account.",
                userID = userID
            });
        }

[AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "Token is required." });
        }

        var isVerified = await _userService.VerifyEmail(token);

        if (!isVerified)
        {
            return BadRequest(new { message = "Invalid or expired verification token." });
        }

        // الخيار أ: إرجاع رسالة نجاح JSON
        return Ok(new { message = "Email verified successfully! You can now log in." });

        // الخيار ب (الأفضل عملياً): إعادة توجيه المستخدم لصفحة تسجيل الدخول في الواجهة الأمامية (Frontend)
        // return Redirect("https://your-frontend-domain.com/login?status=verified");
    }

    }

    
}