using Microsoft.AspNetCore.Authentication.Cookies; // Thêm cái này để dùng Cookie chuẩn
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Client.Pages;
using Pawtopia.Client.Services;
using Pawtopia.Components;
using Pawtopia.Data;
using Pawtopia.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DỊCH VỤ ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ProductService>();

// --- PHẦN QUAN TRỌNG: CẤU HÌNH AUTHENTICATION RIÊNG (KHÔNG IDENTITY) ---
builder.Services.AddAuthentication("MyCookie") // Thiết lập "MyCookie" làm Default Scheme
    .AddCookie("MyCookie", options => {
        options.Cookie.Name = "PawtopiaAuth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/shop";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        // Quan trọng: Để Client (WASM) có thể đọc được trạng thái login
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOpenApi();

// Cấu hình CORS
builder.Services.AddCors(o =>
{
    o.AddPolicy("allow", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// --- KẾT NỐI DATABASE ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=Pawtopia.db";

builder.Services.AddDbContext<PawtopiaDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// HttpClient dùng địa chỉ động
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["BaseAddress"] ?? "https://localhost:7216/")
});

var app = builder.Build();

// --- 2. CẤU HÌNH PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("allow");

// Thứ tự này cực kỳ quan trọng
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Pawtopia.Client._Imports).Assembly);

app.Run();