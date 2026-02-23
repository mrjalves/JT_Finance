using JT_Finance.Services;
using JT_Finance.Shared.Services;
using Microsoft.Extensions.Logging;

namespace JT_Finance
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the JT_Finance.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Register ToastService
            builder.Services.AddSingleton<ToastService>();

            // Register HttpClient and ReceitaService
            builder.Services.AddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001") // Placeholder para MAUI
            });
            builder.Services.AddSingleton<IReceitaService, ReceitaService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
