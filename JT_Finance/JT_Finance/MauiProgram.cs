

using JT_Aplicacao;
using JT_Aplicacao.Config;

using JT_Classes.Acessos;


using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Services.Interfaces;

using JT_Servicos.Financeiro;
using JT_Servicos.Interfaces;


using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Comum;
using JT_UserInterface.Shared.Services.Interfaces.Comum;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

using CartaoCreditoService = JT_Finance.Shared.Services.CartaoCreditoService;
using ReceitaService = JT_Finance.Shared.Services.ReceitaService;

namespace JT_Finance
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {

                var builder = MauiApp.CreateBuilder();

                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    });

                builder.Services.AddMauiBlazorWebView();

                // 1. Carregamento do appsettings.json
                var assembly = Assembly.GetExecutingAssembly();
                // IMPORTANTE: Verifique se o nome do recurso está correto (Namespace.Arquivo)
                using var stream = assembly.GetManifestResourceStream("JT_Finance.appsettings.json");

                if (stream != null)
                {
                    var configRoot = new ConfigurationBuilder().AddJsonStream(stream).Build();
                    builder.Configuration.AddConfiguration(configRoot);

                    var secaoConfig = configRoot.GetSection("Config");
                    var dadosConfig = secaoConfig.Get<Config>();

                    if (dadosConfig != null)
                    {
                        Config.SetInstancia(dadosConfig);
                        builder.Services.AddSingleton(Config.Instancia);
                    }

                }
                else
                {
                    Console.WriteLine("AVISO: appsettings.json não encontrado.");
                }

                // 2. Configuração do Firebase
                if (Config.Instancia.FirebaseConfig != null)
                {
                    FirebaseConfig.SetInstancia(Config.Instancia.FirebaseConfig);
                    builder.Services.AddSingleton(FirebaseConfig.Instancia);
                }
                // 5. HttpClient com ajuste para Android Localhost e Header de Configuração Cliente
                builder.Services.AddScoped(sp =>
                {
                    var baseAddressUrl = FirebaseConfig.Instancia.BaseApiUrl ?? "https://localhost:7166";

                    if (DeviceInfo.Platform == DevicePlatform.Android && baseAddressUrl.Contains("localhost"))
                    {
                        baseAddressUrl = baseAddressUrl.Replace("localhost", "10.0.2.2");
                    }

                    var httpClient = new HttpClient { BaseAddress = new Uri(baseAddressUrl) };

                    // Envia o ClientId para a API
                    var configPayload = new ClientConfigPayload { ClientId = "JT_Finance" };
    JT_Aplicacao.Config.Config.Instancia.ClientId = configPayload.ClientId;
                    var json = System.Text.Json.JsonSerializer.Serialize(configPayload);
                    var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
                    httpClient.DefaultRequestHeaders.Add("X-Client-Config", encoded);

                    return httpClient;
                });


                // 3. Serviços Singleton
                JT_UserInterface.Shared.DependencyInjection.AddJTUserInterface(builder.Services);
                
                // 4. Serviços Específicos deste Projeto
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
                builder.Services.AddScoped<IConfiguracaoTabProvider, JT_UserInterface.Shared.Services.Providers.FinanceConfigTabProvider>();

                // Serviços de IA e Consultor Executivo
                builder.Services.AddScoped<JT_Finance.Shared.Services.Interfaces.IIAAnalyticsService, JT_Finance.Shared.Services.Implementations.GeminiRelatorioService>();
                builder.Services.AddScoped<JT_Finance.Shared.Services.Implementations.GeminiRelatorioService>();
                builder.Services.AddScoped<JT_Finance.Shared.BusinessLogic.RelatorioIALogic>();

                builder.Services.AddScoped<IModuloCategoriaProvider, JT_Finance.Shared.Layout.FinanceiroCategoriaProvider>();
                builder.Services.AddScoped<JT_UserInterface.Shared.Services.Interfaces.Comum.ITutorialProvider, JT_Finance.Shared.Providers.FinanceTutorialProvider>();

#if DEBUG
                builder.Services.AddBlazorWebViewDeveloperTools();
                builder.Logging.AddDebug();
#endif

                var app = builder.Build();


                return app;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar o MauiApp: {ex.Message}");
                throw; // Re-throw para não mascarar o erro
            }
        }
    }
}



