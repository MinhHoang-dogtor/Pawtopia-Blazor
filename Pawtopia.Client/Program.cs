using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using Pawtopia.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 🔹 ROOT COMPONENT (BẮT BUỘC)
builder.RootComponents.Add<App>("#app");

// 🔹 HTTP CLIENT GỌI API
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5007/") // PORT Pawtopia API
    });

// 🔹 AUTH (tạm giữ, chưa dùng cũng không sao)
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

await builder.Build().RunAsync();
