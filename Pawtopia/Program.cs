using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DỊCH VỤ API & DATABASE ---
builder.Services.AddControllers(); // Quan trọng: Để chạy Controller
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Để test API qua giao diện Swagger

// Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Pawtopia.db";
builder.Services.AddDbContext<PawtopiaDbContext>(options =>
    options.UseSqlite(connectionString));

// --- 2. CẤU HÌNH CORS (Để React gọi được API) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // URL của React (Vite hoặc CRA)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Quan trọng nếu dùng Cookie/Identity
    });
});

// --- 3. CẤU HÌNH IDENTITY (Cho Auth) ---
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<PawtopiaDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, o => {
        o.Events.OnRedirectToLogin = context => {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Không redirect về trang Login của Blazor
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// --- 4. CẤU HÌNH PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sử dụng CORS
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Map các Controller như AuthController, ProductController

app.Run();