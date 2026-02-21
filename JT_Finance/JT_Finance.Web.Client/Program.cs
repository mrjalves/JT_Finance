using JT_Aplicacao;

using JT_Classes.Acessos;

using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Models;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared;
using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddScoped(sp => http); // Registra esse http client para uso geral

try
{
    // Tenta baixar o arquivo appsettings.json "na força"
    using var response = await http.GetAsync("appsettings.json");
    using var stream = await response.Content.ReadAsStreamAsync();

    // Adiciona o conteúdo do arquivo baixado à configuração do Builder
    builder.Configuration.AddJsonStream(stream);

    // Debug: Imprime no console do navegador o que ele leu (para você conferir no F12)
    var dbUrl = builder.Configuration["FirebaseConfig:DatabaseUrl"];
    Console.WriteLine($"[DEBUG LOAD] DatabaseUrl lida: {dbUrl ?? "NULO"}");
}
catch (Exception ex)
{
    Console.WriteLine($"[ERRO CRÍTICO] Falha ao baixar appsettings.json: {ex.Message}");
}
// ===================================================================================

// 2. Configura a classe tipada (Agora o builder.Configuration já tem os dados)
//builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("FirebaseConfig"));

// 3. Serviços Singleton (Globais)
builder.Services.AddJTUserInterface();

// 4. Serviços Scoped
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();


// 1. Localiza a seção "Config" dentro do JSON
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seção para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();

if (dadosConfig != null)
{
    // 3. Define a instância estática para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

// 4. Registra no container de serviços para injeção (Opcional, mas boa prática)
builder.Services.AddSingleton(Config.Instancia);


await builder.Build().RunAsync();
