

using JT_Aplicacao;

using JT_Classes.Acessos;


using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

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

                // 2. Configuração do Firebase via IOptions
                var secaoFConfig = builder.Configuration.GetSection("FirebaseConfig");
                var dadosFConfig = secaoFConfig.Get<FirebaseConfig>();

                if (dadosFConfig != null)
                {
                    FirebaseConfig.SetInstancia(dadosFConfig);
                    builder.Services.AddSingleton(FirebaseConfig.Instancia);
                }
                // 5. HttpClient com ajuste para Android Localhost
                builder.Services.AddScoped(sp =>
                {
                    var baseAddressUrl = FirebaseConfig.Instancia.BaseApiUrl ?? "https://localhost:7166";

                    if (DeviceInfo.Platform == DevicePlatform.Android && baseAddressUrl.Contains("localhost"))
                    {
                        baseAddressUrl = baseAddressUrl.Replace("localhost", "10.0.2.2");
                    }

                    return new HttpClient { BaseAddress = new Uri(baseAddressUrl) };
                });


                // 3. Serviços Singleton
                JT_UserInterface.Shared.DependencyInjection.AddJTUserInterface(builder.Services);

                // 4. Serviços Scoped
                builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();



#if DEBUG
                builder.Services.AddBlazorWebViewDeveloperTools();
                builder.Logging.AddDebug();
#endif

                return builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar o MauiApp: {ex.Message}");
                throw; // Re-throw para não mascarar o erro
            }
        }
    }
}
