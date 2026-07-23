using JT_Aplicacao;

using JT_Finance.Shared.Layout;
using JT_Finance.Web.Components;
using JT_Finance.Shared.Services.Interfaces;
using JT_Classes.Financeiro;
using JT_Servicos.Financeiro;
using JT_Servicos.Interfaces;

using JT_UserInterface.Shared;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

using Microsoft.Extensions.DependencyInjection.Extensions;
using JT_Aplicacao.Config;
using JT_Classes.Acessos;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração dos Componentes Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// 2. Configuração (O ASP.NET Core já leu o appsettings.json automaticamente)
// Apenas mapeamos a seção para a classe.
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("FirebaseConfig"));


// 1. Localiza a seção "Config" dentro do JSON
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seção para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();

if (dadosConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

secaoConfig = builder.Configuration.GetSection("Config:FirebaseConfig");
var dadosFConfig = secaoConfig.Get<FirebaseConfig>();

if (dadosFConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)
    FirebaseConfig.SetInstancia(dadosFConfig);
    // 4. Registra no container de serviços para injeção (Opcional, mas boa prática)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// 5. HttpClient (Necessário para o Repository e Auth)
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    // No Server, BaseApiUrl geralmente é a própria URL onde o site está rodando ou vazia,
    // já que o Repository usa URLs absolutas do Firebase.
    var baseUrl = builder.Configuration["Config:FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});


// 3. Serviços Globais (Via Método de Extensão da Shared)
builder.Services.AddJTUserInterface();

// 4. Serviços Específicos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IModuloMenuProvider, FinanceMenuProvider>();

// Registros dos Wrappers/Interfaces do Frontend
builder.Services.AddScoped<IDespesaService, JT_Finance.Shared.Services.DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, JT_Finance.Shared.Services.CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, JT_Finance.Shared.Services.ReceitaService>();
builder.Services.AddScoped<IMFinanceiroConfiguracaoService, MFinanceiroConfiguracaoService>();
builder.Services.AddScoped<IConfiguracaoTabProvider, JT_UserInterface.Shared.Services.Providers.FinanceConfigTabProvider>();
builder.Services.AddScoped<ISecureStorageService, JT_Finance.Shared.Services.WebSecureStorageService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 🔥 ESTA LINHA É OBRIGATÓRIA PARA DEPURAR O CLIENTE
    //app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});
// 1. ADICIONE O ROUTING AQUI! 
// Ele analisa a URL e descobre o destino antes de aplicar a segurança
app.UseRouting();

// 2. Agora o Antiforgery sabe para onde a requisição vai e pode validá-la
app.UseAntiforgery();

// 3. Nossos mapeamentos de destino
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode() 
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(JT_Finance.Shared._Imports).Assembly,
        typeof(JT_Finance.Web.Client._Imports).Assembly,
        typeof(JT_UserInterface.Shared._Imports).Assembly
    );

app.Run();

