using JT_Finance.Shared.Services;
using JT_Finance.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the JT_Finance.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Register ToastService
builder.Services.AddScoped<ToastService>();

// Register HttpClient and ReceitaService
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IReceitaService, ReceitaService>();

await builder.Build().RunAsync();
