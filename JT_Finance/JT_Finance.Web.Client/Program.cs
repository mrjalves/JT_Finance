using JT_Aplicacao;

using JT_Classes.Acessos;

using JT_Finance.Shared.Layout;
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

/// 1.Localiza a seção "Config" dentro do JSON
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
    FirebaseConfig.SetInstancia(dadosFConfig);
    // 4. Registra no container de serviços para injeção (Opcional, mas boa prática)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// 5. HttpClient (Necessário para o Repository e Auth)
builder.Services.AddScoped(sp =>
{
    // No Server, BaseApiUrl geralmente é a própria URL onde o site está rodando ou vazia,
    // já que o Repository usa URLs absolutas do Firebase.
    var baseUrl = builder.Configuration["FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});
// 4. Serviços Globais
builder.Services.AddJTUserInterface();

// 5. Serviços Scoped (Repositórios e Serviços)
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IDespesaRep, ApiDespRep>();
builder.Services.AddScoped<ICartaoCreditoRep, ApiCartaoCreditoRep>();
builder.Services.AddScoped<IReceitaRep, ApiReceitaRep>();

builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();

await builder.Build().RunAsync();