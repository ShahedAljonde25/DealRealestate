using DotNetEnv;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using DealRealState.API.Authorization;
using DealRealState.Business.Services;
using BusinessLayer.Services;

// 1. Load .env Configuration
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 1) CORS Configuration
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ===============================
// 2) Authentication (JWT Bearer)
// ===============================
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY is missing from environment variables.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ===============================
// 3) Authorization & Policies
// ===============================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SellerOrSelf", policy =>
    {
        policy.Requirements.Add(new SellerOrSelfRequirement());
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, SellerOrSelfHandler>();

// ===============================
// 4) Rate Limiting
// ===============================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

// ===============================
// 5) Register Controllers & Services
// ===============================
builder.Services.AddControllers();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<FavoriteService>();

// ===============================
// 6) Swagger / OpenAPI
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DealRealState API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<EmailService>();
var app = builder.Build();

// ===============================
// 7) Correct Middleware Pipeline Order
// ===============================

// A. Swagger (Development Only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// B. HTTPS Redirection
app.UseHttpsRedirection();

// C. CORS (Must be early to handle preflight requests & headers)
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
// D. Custom 429 Response Handling (Safe Check)
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests && !context.Response.HasStarted)
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Too many attempts. Please try again later.");
    }
});
// E. Rate Limiter
app.UseRateLimiter();

// F. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
var panoramaPath = @"C:\RealStateUploads\Panoramas";

if (!Directory.Exists(panoramaPath))
{
    Directory.CreateDirectory(panoramaPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"C:\Users\LOQ\Desktop\DealRealstat\frontend"),
    RequestPath = "" 
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"C:\RealStateUploads"),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
    }
});
app.MapControllers();

app.Run();