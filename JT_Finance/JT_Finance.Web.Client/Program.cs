using JT_Aplicacao;

using JT_Classes.Acessos;

using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Services.Interfaces;
using JT_Servicos.Financeiro;
using JT_Servicos.Interfaces;

using JT_UserInterface.Shared;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// =========================================================================
// 0. CONFIGURAÇÕES DE AMBIENTE (Blazor WebAssembly)
// =========================================================================
// NOTA DIDÁTICA: No Blazor WASM não forçamos a leitura com .AddJsonFile() 
// porque o código corre no browser. O builder acima já faz o download automático:
// 1º Tenta ler: wwwroot/appsettings.json
// 2º Tenta ler: wwwroot/appsettings.{Environment}.json (ex: Development)
//
// Esta linha ajuda a veres na consola do browser em que ambiente estás a rodar:
// Linha corrigida
Console.WriteLine($"[INFO] Ambiente atual do Blazor: {builder.HostEnvironment.Environment}");

// =========================================================================
// 1. CARREGAR CONFIGURAÇÕES ESTÁTICAS GLOBAIS
// =========================================================================
// 1. Localiza a seção "Config" dentro do JSON (Já vem com o ambiente correto resolvido)
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seção para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();
if (dadosConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

// 4. Repete o processo para as configurações do Firebase
var secaoFirebase = builder.Configuration.GetSection("Config:FirebaseConfig");
var dadosFConfig = secaoFirebase.Get<FirebaseConfig>();
if (dadosFConfig != null)
{
    FirebaseConfig.SetInstancia(dadosFConfig);
    // Registra no container de serviços para injeção (Boa prática!)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// =========================================================================
// 2. CONFIGURAÇÃO DE HTTP CLIENT
// =========================================================================
builder.Services.AddScoped(sp =>
{
    // Lê a BaseApiUrl do ficheiro correto. Se estiver em modo Debug, vai buscar
    // a URL do appsettings.Development.json automaticamente.
    var baseUrl = builder.Configuration["Config:FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

// =========================================================================
// 3. REGISTO DE SERVIÇOS (Injeção de Dependência)
// =========================================================================
// Serviços Globais
builder.Services.AddJTUserInterface();

// Serviços Específicos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();

// Registros dos Wrappers/Interfaces do Frontend
builder.Services.AddScoped<IDespesaService, JT_Finance.Shared.Services.DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, JT_Finance.Shared.Services.CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, JT_Finance.Shared.Services.ReceitaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMFinanceiroConfiguracaoService, MFinanceiroConfiguracaoService>();
builder.Services.AddScoped<IConfiguracaoTabProvider, JT_UserInterface.Shared.Services.Providers.FinanceConfigTabProvider>();
// =========================================================================
// 4. INICIALIZAÇÃO DA APLICAÇÃO
// =========================================================================
await builder.Build().RunAsync();

