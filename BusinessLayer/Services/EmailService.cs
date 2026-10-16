using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService 
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string verificationUrl)
    {
        try
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            var senderEmail = _config["EmailSettings:SenderEmail"];

            // إزالة المسافات تلقائياً في حال وجودها
            var password = _config["EmailSettings:Password"]?.Replace(" ", "");

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, "DealRealState"),
                Subject = "Verify Your Email - DealRealState",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Welcome to DealRealState!</h2>
                        <p>Please click the button below to verify your email address:</p>
                        <a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Verify Email</a>
                    </div>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            Console.WriteLine($"[EmailSent] Verification email sent successfully to {toEmail}");
        }
        catch (Exception ex)
        {
            // طباعة تفاصيل الخطأ بدقة في الشاشة السوداء (Console)
            Console.WriteLine($"[EmailError] Failed to send email: {ex.Message}");
            throw; // إعادة رمي الخطأ ليتضح لك إذا فشل الطلب
        }
    }
}