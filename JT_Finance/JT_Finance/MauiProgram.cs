using JT_Finance.Services;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;

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
            builder.Services.AddSingleton<FiltroService>();
            builder.Services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IReceitaService, ReceitaService>();
            builder.Services.AddScoped<IDespesaService, DespesaService>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
