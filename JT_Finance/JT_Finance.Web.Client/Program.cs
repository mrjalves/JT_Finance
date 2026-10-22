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
builder.Services.Configure<FirebaseConfig>(secaoFirebase);
if (dadosFConfig != null)
{
    FirebaseConfig.SetInstancia(dadosFConfig);
    // Registra no container de serviços para injeção (Boa prática!)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

if (JT_Aplicacao.Config.Config.Instancia != null)
{
    JT_Aplicacao.Config.Config.Instancia.ClientId = "JT_Finance";
}

// =========================================================================
// 3. REGISTO DE SERVIÇOS (Injeção de Dependência)
// =========================================================================
// Serviços Globais
builder.Services.AddJTUserInterface();

// Configura o cliente nomeado já registrado pela camada compartilhada.
// O JwtAuthorizationHandler injeta o JWT e o tenant somente nas chamadas da API.
builder.Services.AddHttpClient(string.Empty, client =>
{
    var baseUrl = builder.Configuration["Config:FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient("GeminiCleanClient");

// Serviços Específicos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IModuloMenuProvider, FinanceMenuProvider>();

// Registros dos Wrappers/Interfaces do Frontend
builder.Services.AddScoped<JT_Servicos.Financeiro.DespesaService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.CartaoCreditoService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.ReceitaService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.DashboardService>();

builder.Services.AddScoped<IDespesaService, JT_Finance.Shared.Services.DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, JT_Finance.Shared.Services.CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, JT_Finance.Shared.Services.ReceitaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMFinanceiroConfiguracaoService, MFinanceiroConfiguracaoService>();

// Serviços de IA e Consultor Executivo
builder.Services.AddScoped<JT_Finance.Shared.Services.Interfaces.IIAAnalyticsService, JT_Finance.Shared.Services.Implementations.GeminiRelatorioService>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Implementations.GeminiRelatorioService>();
builder.Services.AddScoped<JT_Finance.Shared.BusinessLogic.RelatorioIALogic>();

builder.Services.AddScoped<ISecureStorageService, JT_Finance.Shared.Services.WebSecureStorageService>();

// Providers de indicadores de dashboard (registrados pela interface para o catálogo)
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.TotalReceitasProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.TotalDespesasProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.SaldoProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.DespesasVencidasProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.ProximasVencimentoProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.DespesasPorCategoriaProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.IDashboardIndicadorProvider, JT_Finance.Shared.Services.Dashboard.DespesasPorTipoProvider>();
builder.Services.AddScoped<JT_Finance.Shared.Services.Dashboard.DashboardIndicadorRegistry>();

builder.Services.AddScoped<IModuloCategoriaProvider, JT_Finance.Shared.Layout.FinanceiroCategoriaProvider>();
builder.Services.AddScoped<JT_UserInterface.Shared.Services.Interfaces.Comum.ITutorialProvider, JT_Finance.Shared.Providers.FinanceTutorialProvider>();

// =========================================================================
// 4. INICIALIZAÇÃO DA APLICAÇÃO
// =========================================================================
await builder.Build().RunAsync();
