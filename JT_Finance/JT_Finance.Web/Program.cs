using JT_Aplicacao;

using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Models;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;
using JT_Finance.Web.Components;

using JT_UserInterface.Shared;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração dos Componentes Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// 2. Configuração (O ASP.NET Core já leu o appsettings.json automaticamente)
// Apenas mapeamos a seção para a classe.
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("FirebaseConfig"));

// 3. Serviços Globais (Via Método de Extensão da Shared)
builder.Services.AddJTUserInterface();

// 4. Serviços Específicos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IDespesaRep, ApiDespRep>();
builder.Services.AddScoped<ICartaoCreditoRep, ApiCartaoCreditoRep>();
builder.Services.AddScoped<IReceitaRep, ApiReceitaRep>();
// 1. Localiza a seção "Config" dentro do JSON
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seção para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();

if (dadosConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

secaoConfig = builder.Configuration.GetSection("FirebaseConfig");
var dadosFConfig = secaoConfig.Get<FirebaseConfig>();

if (dadosFConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)

    // 4. Registra no container de serviços para injeção (Opcional, mas boa prática)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// 5. HttpClient (Necessário para o Repository e Auth)
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    // No Server, BaseApiUrl geralmente é a própria URL onde o site está rodando ou vazia,
    // já que o Repository usa URLs absolutas do Firebase.
    var baseUrl = builder.Configuration["FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

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
        typeof(JT_Finance.Web.Client._Imports).Assembly,
         typeof(JT_UserInterface.Shared._Imports).Assembly
        );

app.Run();