using JT_Finance.Shared.Models;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;
using JT_Finance.Web.Client.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the JT_Finance.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
// Register ToastService
builder.Services.AddScoped<ToastService>();
// 1. O HttpClient é configurado para usar o endereço do servidor que o hospeda.
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7166")
});

// 2. LÓGICA PRINCIPAL: No cliente, SEMPRE usamos a implementação de API (ApiDespRep).
//    É isso que corrige o erro "Unable to resolve service".
builder.Services.AddScoped<IDespesaRep, ApiDespRep>();
builder.Services.AddScoped<ICartaoCreditoRep, ApiCartaoCreditoRep>();
builder.Services.AddScoped<IReceitaRep, ApiReceitaRep>();
// Register FiltroService
builder.Services.AddScoped<FiltroService>();
builder.Services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddSingleton<CurrentUserState>();


var host = builder.Build();

// Inicializar o CurrentUserState após construir o host
var currentUserState = host.Services.GetRequiredService<CurrentUserState>();
currentUserState.Initialize(host.Services.GetRequiredService<IJSRuntime>());

await host.RunAsync();
