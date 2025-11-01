using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Models;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;
using JT_Finance.Web.Components;
using JT_Finance.Web.Services;

using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// ---- CONFIGURAÇÃO DOS SERVIÇOS DO LADO DO SERVIDOR ----

// Serviços de UI e específicos do servidor
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<FiltroService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IThemeService, ThemeService>();
// HttpClient é registrado aqui para o caso de o servidor precisar chamar
// OUTRAS APIs externas no futuro. Não é para chamar a si mesmo.
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7166")
});
// LÓGICA PRINCIPAL: No servidor, SEMPRE usamos a implementação de acesso direto (Desktop).
// Isso evita que o servidor faça chamadas de rede para si mesmo.
//builder.Services.AddScoped<IDespesaRep, DesktopDespRep>();
//builder.Services.AddScoped<ICartaoCreditoRep, DesktopCartaoCreditoRep>();
builder.Services.AddScoped<IDespesaRep, ApiDespRep>();
builder.Services.AddScoped<ICartaoCreditoRep, ApiCartaoCreditoRep>();
builder.Services.AddScoped<IReceitaRep, ApiReceitaRep>();
// Os outros serviços que dependem da fonte de dados (IDespesaRep)
builder.Services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddSingleton<CurrentUserState>();
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    await next();
});
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(JT_Finance.Shared._Imports).Assembly,
        typeof(JT_Finance.Web.Client._Imports).Assembly);

app.Run();