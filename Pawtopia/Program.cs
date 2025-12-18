using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Client.Pages;
using Pawtopia.Components;
using Pawtopia.Components.Account;
using Pawtopia.Data;
using Pawtopia.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddScoped(sp =>
{
    // Lấy dịch vụ NavigationManager để biết địa chỉ hiện tại
    var navigationManager = sp.GetRequiredService<NavigationManager>();

    return new HttpClient
    {
        // Tự động lấy địa chỉ gốc (VD: https://localhost:7285/ hoặc https://pawtopia.com/)
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<PawtopiaDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
    .AddEntityFrameworkStores<PawtopiaDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Pawtopia.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Admin API endpoints
app.MapGet("/api/admin/orders", async (PawtopiaDbContext db, string? fromDate, string? toDate) =>
{
    var query = db.Orders
        .Include(o => o.User)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
        .AsQueryable();

    // Lọc theo khoảng ngày nếu có
    if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var from))
    {
        query = query.Where(o => o.CreatedAt >= from);
    }

    if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var to))
    {
        // Thêm 1 ngày để bao gồm cả ngày cuối
        query = query.Where(o => o.CreatedAt < to.AddDays(1));
    }

    var orders = await query
        .OrderByDescending(o => o.CreatedAt)
        .Select(o => new
        {
            Id = o.Id,
            AccountName = o.User.DisplayName,
            CustomerName = o.User.UserName,
            Total = o.OrderItems.Sum(oi => oi.Quantity * oi.ProductItem.Price),
            Status = o.Status.ToString(),
            PaymentStatus = o.PaymentStatus,
            CreatedAt = o.CreatedAt
        })
        .ToListAsync();

    return Results.Ok(orders);
});

app.Run();
