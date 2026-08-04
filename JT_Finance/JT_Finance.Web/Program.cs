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
using JT_Classes.IA;
using JT_UserInterface.Shared.Layout;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1. ConfiguraÃ§Ã£o dos Componentes Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// 2. ConfiguraÃ§Ã£o (O ASP.NET Core jÃ¡ leu o appsettings.json automaticamente)
// Apenas mapeamos a seÃ§Ã£o para a classe.
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("Config:FirebaseConfig"));


// 1. Localiza a seÃ§Ã£o "Config" dentro do JSON
var secaoConfig = builder.Configuration.GetSection("Config");

// 2. Converte essa seÃ§Ã£o para um objeto da classe Config
var dadosConfig = secaoConfig.Get<Config>();

if (dadosConfig != null)
{
    // 3. Define a instÃ¢ncia estÃ¡tica para uso global (Config.Instancia)
    Config.SetInstancia(dadosConfig);
}

secaoConfig = builder.Configuration.GetSection("Config:FirebaseConfig");
var dadosFConfig = secaoConfig.Get<FirebaseConfig>();

if (dadosFConfig != null)
{
    // 3. Define a instÃ¢ncia estÃ¡tica para uso global (Config.Instancia)
    FirebaseConfig.SetInstancia(dadosFConfig);
    // 4. Registra no container de serviÃ§os para injeÃ§Ã£o (Opcional, mas boa prÃ¡tica)
    builder.Services.AddSingleton(FirebaseConfig.Instancia);
}

// 3. ServiÃ§os Globais (Via MÃ©todo de ExtensÃ£o da Shared)
builder.Services.AddJTUserInterface();
Config.Instancia.ClientId = "JT_Finance";

// Configura o cliente nomeado já registrado pela camada compartilhada.
// O JwtAuthorizationHandler injeta o JWT e o tenant somente nas chamadas da API.
builder.Services.AddHttpClient(string.Empty, client =>
{
    var baseUrl = builder.Configuration["Config:FirebaseConfig:BaseApiUrl"] ?? "https://localhost:7166";
    client.BaseAddress = new Uri(baseUrl);
});

// 4. ServiÃ§os EspecÃ­ficos deste Projeto
builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
builder.Services.AddScoped<IModuloMenuProvider, FinanceMenuProvider>();

// Registros dos Wrappers/Interfaces do Frontend
builder.Services.AddScoped<JT_Servicos.Financeiro.DespesaService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.CartaoCreditoService>();
builder.Services.AddScoped<JT_Servicos.Financeiro.ReceitaService>();

builder.Services.AddScoped<IDespesaService, JT_Finance.Shared.Services.DespesaService>();
builder.Services.AddScoped<ICartaoCreditoService, JT_Finance.Shared.Services.CartaoCreditoService>();
builder.Services.AddScoped<IReceitaService, JT_Finance.Shared.Services.ReceitaService>();
builder.Services.AddScoped<IMFinanceiroConfiguracaoService, MFinanceiroConfiguracaoService>();
builder.Services.AddScoped<ISecureStorageService, JT_Finance.Shared.Services.WebSecureStorageService>();

builder.Services.AddScoped<IModuloCategoriaProvider, JT_Finance.Shared.Layout.FinanceiroCategoriaProvider>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // ðŸ”¥ ESTA LINHA Ã‰ OBRIGATÃ“RIA PARA DEPURAR O CLIENTE
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
// Ele analisa a URL e descobre o destino antes de aplicar a seguranÃ§a
app.UseRouting();

// 2. Agora o Antiforgery sabe para onde a requisiÃ§Ã£o vai e pode validÃ¡-la
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

// Financeiro Config endpoints
app.MapGet("/api/configuracoes-financeiras", async (JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => Results.Ok(await svc.GetAll()));
app.MapGet("/api/configuracoes-financeiras/ativa", async (JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => Results.Ok(await svc.GetAtivaAsync()));
app.MapGet("/api/configuracoes-financeiras/{id:int}", async (int id, JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => await svc.GetById(id) is { } e ? Results.Ok(e) : Results.NotFound());
app.MapPost("/api/configuracoes-financeiras", async ([FromBody] MFinanceiroConfiguracao entity, JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());
app.MapPost("/api/configuracoes-financeiras/ativa", async ([FromBody] MFinanceiroConfiguracao entity, JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());
app.MapPut("/api/configuracoes-financeiras/{id:int}", async (int id, [FromBody] MFinanceiroConfiguracao entity, JT_Servicos.Financeiro.MFinanceiroConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());

// IA Config endpoints
app.MapGet("/api/configuracoes-ia", async (JT_Servicos.IA.ConfiguracaoIAService svc) => Results.Ok(await svc.GetAll()));
app.MapGet("/api/configuracoes-ia/modelos", async ([FromQuery] string? apiKey, JT_Servicos.IA.ConfiguracaoIAService svc) => Results.Ok(await svc.ListarModelosDisponiveisAsync(apiKey)));
app.MapGet("/api/configuracoes-ia/{id:int}", async (int id, JT_Servicos.IA.ConfiguracaoIAService svc) => await svc.GetById(id) is { } e ? Results.Ok(e) : Results.NotFound());
app.MapPost("/api/configuracoes-ia", async ([FromBody] ConfiguracaoIA entity, JT_Servicos.IA.ConfiguracaoIAService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());
app.MapPut("/api/configuracoes-ia/{id:int}", async (int id, [FromBody] ConfiguracaoIA entity, JT_Servicos.IA.ConfiguracaoIAService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());

// Acessos Config endpoints
app.MapGet("/api/configuracoes-acessos", async (JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => Results.Ok(await svc.GetAll()));
app.MapGet("/api/configuracoes-acessos/ativa", async (JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => Results.Ok(await svc.GetAtivaAsync()));
app.MapGet("/api/configuracoes-acessos/{id:int}", async (int id, JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => await svc.GetById(id) is { } e ? Results.Ok(e) : Results.NotFound());
app.MapPost("/api/configuracoes-acessos", async ([FromBody] MAcessosConfiguracao entity, JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());
app.MapPost("/api/configuracoes-acessos/ativa", async ([FromBody] MAcessosConfiguracao entity, JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());
app.MapPut("/api/configuracoes-acessos/{id:int}", async (int id, [FromBody] MAcessosConfiguracao entity, JT_Servicos.Acessos.MAcessosConfiguracaoService svc) => await svc.Save(entity) ? Results.Ok(entity) : Results.BadRequest());

app.Run();


