using JT_Aplicacao;
using JT_Aplicacao.Config;

using JT_Classes.Acessos;

using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Services.Interfaces;
using JT_Servicos.Financeiro;
using JT_Servicos.Interfaces;

using JT_UserInterface.Shared;
using JT_UserInterface.Shared.Layout;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Comum;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// =========================================================================
// 0. CONFIGURAÃ‡Ã•ES DE AMBIENTE (Blazor WebAssembly)
// =========================================================================
// NOTA DIDÃTICA: No Blazor WASM nÃ£o forÃ§amos a leitura com .AddJsonFile() 
// porque o cÃ³digo corre no browser. O builder acima jÃ¡ faz o download automÃ¡tico:
// 1Âº Tenta ler: wwwroot/appsettings.json
// 2Âº Tenta ler: wwwroot/appsettings.{Environment}.json (ex: Development)
//
// Esta linha ajuda a veres na consola do browser em que ambiente estÃ¡s a rodar:
// Linha corrigida
Console.WriteLine($"[INFO] Ambiente atual do Blazor: {builder.HostEnvironment.Environment}");

// =========================================================================
// 1. CARREGAR CONFIGURAÃ‡Ã•ES ESTÃTICAS GLOBAIS
// =========================================================================
// 1. Localiza a seÃ§Ã£o "Config" dentro do JSON (JÃ¡ vem com o ambiente correto resolvido)
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seÃ§Ã£o para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();
if (dadosConfig != null)
{
    // 3. Define a instÃ¢ncia estÃ¡tica para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

// 4. Repete o processo para as configuraÃ§Ãµes do Firebase
var secaoFirebase = builder.Configuration.GetSection("Config:FirebaseConfig");
var dadosFConfig = secaoFirebase.Get<FirebaseConfig>();
if (dadosFConfig != null)
{
    FirebaseConfig.SetInstancia(dadosFConfig);
    // Registra no container de serviÃ§os para injeÃ§Ã£o (Boa prÃ¡tica!)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// =========================================================================
// 2. CONFIGURAÃ‡ÃƒO DE HTTP CLIENT
// =========================================================================
builder.Services.AddScoped(sp =>
{
    // LÃª a BaseApiUrl do ficheiro correto. Se estiver em modo Debug, vai buscar
    // a URL do appsettings.Development.json automaticamente.
    var baseUrl = builder.Configuration["Config:FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

    var configPayload = new JT_Aplicacao.Config.ClientConfigPayload { ClientId = "JT_Finance" };
    JT_Aplicacao.Config.Config.Instancia.ClientId = configPayload.ClientId;
    var json = System.Text.Json.JsonSerializer.Serialize(configPayload);
    var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
    httpClient.DefaultRequestHeaders.Add("X-Client-Config", encoded);

    return httpClient;
});

// =========================================================================
// 3. REGISTO DE SERVIÃ‡OS (InjeÃ§Ã£o de DependÃªncia)
// =========================================================================
// ServiÃ§os Globais
builder.Services.AddJTUserInterface();

// ServiÃ§os EspecÃ­ficos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IModuloMenuProvider, FinanceMenuProvider>();

// Registros dos Wrappers/Interfaces do Frontend
builder.Services.AddScoped<JT_Servicos.Financeiro.DespesaService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.CartaoCreditoService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.ReceitaService>();

builder.Services.AddScoped<IDespesaService, JT_Finance.Shared.Services.DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, JT_Finance.Shared.Services.CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, JT_Finance.Shared.Services.ReceitaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMFinanceiroConfiguracaoService, MFinanceiroConfiguracaoService>();
builder.Services.AddScoped<ISecureStorageService, JT_Finance.Shared.Services.WebSecureStorageService>();

builder.Services.AddScoped<IModuloCategoriaProvider, JT_Finance.Shared.Layout.FinanceiroCategoriaProvider>();

// =========================================================================
// 4. INICIALIZAÃ‡ÃƒO DA APLICAÃ‡ÃƒO
// =========================================================================
await builder.Build().RunAsync();


